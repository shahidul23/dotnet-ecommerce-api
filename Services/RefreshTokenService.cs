using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Exceptions;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_ecommerce_api.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IJwtService _jwtService;
    public RefreshTokenService(
        AppDbContext appDbContext, 
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        TokenValidationParameters tokenValidationParameters,
        IJwtService jwtService
        )
    {
        _appDbContext = appDbContext;
        _configuration = configuration;
        _userManager = userManager;
        _tokenValidationParameters = tokenValidationParameters;
        _jwtService = jwtService;
    }
    public async Task<RefreshToken> CreateAsync(ApplicationUser? user, string jwtId, string existingRefreshToken)
    {
        var expirationDays = _configuration.GetValue<int?>("Jwt:RefreshTokenExpirationDays") ?? 1;
        var refreshToken = new RefreshToken();

        if (string.IsNullOrEmpty(existingRefreshToken))
        {
            refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateToken(),
                JwtId = jwtId,
                IsRevoked = false,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMinutes(expirationDays),
            };
            await _appDbContext.AddAsync(refreshToken);
            await _appDbContext.SaveChangesAsync();
        }
        
        return refreshToken;
    }
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _appDbContext.RefreshTokens.Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(
                refreshToken => refreshToken.Token == token
            );
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        await _appDbContext.SaveChangesAsync();
    }

    public Task<bool> ValidateAsync(RefreshToken refreshToken)
    {
        var IsValid = 
            !refreshToken.IsRevoked &&
            refreshToken.DateExpire > DateTime.UtcNow;
        return Task.FromResult(IsValid);
    }
    private static string GenerateToken()
    {
        var randomBytes  = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
    public async Task<AuthResponseDto?> VerifyAndGenerateTokenAsync(
        TokenRequestDto payload)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // 1. Validate JWT signature and structure
            var tokenInVerification = jwtTokenHandler.ValidateToken(
                payload.Token,
                _tokenValidationParameters,
                out var validatedToken
            );

            // 2. Check JWT algorithm
            if (validatedToken is not JwtSecurityToken jwtSecurityToken)
            {
                throw new UnauthorizedException( "Invalid token format." );
            }

            var isValidAlgorithm = jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            );

            if (!isValidAlgorithm)
            {
                throw new UnauthorizedException( "Invalid token algorithm." );
            }

            // 3. Get JWT expiration claim
            var expiryClaim = tokenInVerification.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);

            if (expiryClaim == null)
            {
                throw new UnauthorizedException( "Token expiration claim is missing." );
            }

            if (!long.TryParse(expiryClaim.Value, out var utcExpiryDate))
            {
                throw new UnauthorizedException( "Invalid token expiration value." );
            }

            var expiryDate = UnixTimeStampToDateTimeInUTC(utcExpiryDate);

            // Access token must be expired before refresh
            if (expiryDate > DateTime.UtcNow)
            {
                throw new UnauthorizedException ("Token has not expired yet");
            }

            // 4. Get JTI from access token
            var jtiClaim = tokenInVerification.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);

            if (jtiClaim == null)
            {
                throw new UnauthorizedException("Token JTI is missing");
            }

            var jti = jtiClaim.Value;

            // 5. Find refresh token in database
            var dbRefreshToken = await _appDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == payload.RefreshToken);

            if (dbRefreshToken == null)
            {
                throw new UnauthorizedException(
                    "Refresh token does not exist in our database"
                );
            }

            // 6. Make sure refresh token belongs to this JWT
            if (dbRefreshToken.JwtId != jti)
            {
                throw new UnauthorizedException("Refresh token does not match the JWT");
            }

            // 7. Check refresh token expiration
            if (dbRefreshToken.DateExpire <= DateTime.UtcNow)
            {
                throw new UnauthorizedException(
                    "Your refresh token has expired. Please authenticate again."
                );
            }

            // 8. Check if refresh token has already been revoked
            if (dbRefreshToken.IsRevoked)
            {
                throw new UnauthorizedException("Refresh token has been revoked");
            }

            // 9. Get user
            var dbUserData = await _userManager.FindByIdAsync(
                dbRefreshToken.UserId
            );

            if (dbUserData == null)
            {
                throw new UnauthorizedException("User associated with token was not found");
            }

            // 10. Generate new JWT
            var jwtResult = _jwtService.GenerateJwtToken(dbUserData);

            // 11. Rotate refresh token
            var newRefreshToken = await CreateAsync(
                dbUserData,
                jwtResult.JwtId,
                payload.RefreshToken
            );

            // 12. Return authentication response
            return new AuthResponseDto
            {
                Token = jwtResult.Token,

                RefreshToken = newRefreshToken.Token,

                ExpiresAt = jwtResult.ExpiresAt,

                User = new UserReadDto
                {
                    Id = dbUserData.Id,

                    UserName = dbUserData.UserName ?? string.Empty,

                    Email = dbUserData.Email ?? string.Empty
                }
            };
        }
        catch (SecurityTokenException)
        {
            var dbRefreshToken = await _appDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == payload.RefreshToken);
             var dbUserData = await _userManager.FindByIdAsync(
                dbRefreshToken.UserId
            );
            var jwtResult = _jwtService.GenerateJwtToken(dbUserData);

            // 11. Rotate refresh token
            var newRefreshToken = await CreateAsync(
                dbUserData,
                jwtResult.JwtId,
                payload.RefreshToken
            );
            return new AuthResponseDto
            {
                Token = jwtResult.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = jwtResult.ExpiresAt,
                User = new UserReadDto
                {
                    Id = dbUserData.Id,
                    UserName = dbUserData.UserName ?? string.Empty,
                    Email = dbUserData.Email ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            throw new UnauthorizedException( $"Invalid JWT token: {ex.Message}" );
        };
    }

    private DateTime UnixTimeStampToDateTimeInUTC(long unixTimeStamp)
    {
        var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
        return dateTimeVal;
    }
    
}
