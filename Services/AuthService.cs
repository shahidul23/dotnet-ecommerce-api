using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_ecommerce_api.Common;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_ecommerce_api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly TokenValidationParameters _validationParameters;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext appDbContext,
        IConfiguration configuration,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        TokenValidationParameters validationParameters
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
        _configuration = configuration;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _validationParameters = validationParameters;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            return null;
        }

        var newUser = new ApplicationUser()
        {
            Email = dto.Email,
            UserName = dto.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(
            newUser,
            dto.Password
        );

        if (!result.Succeeded)
        {
            return null;
        }

        return new AuthResponseDto
        {
            User = new UserReadDto
            {
                Id = newUser.Id,
                UserName = newUser.UserName ?? string.Empty,
                Email = newUser.Email ?? string.Empty
            }
        };
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginDto login)
    {
        ApplicationUser? existingUser;
        if (login.UserNameOrEmail.Contains("@"))
        {
            existingUser = await _userManager.FindByEmailAsync(
                login.UserNameOrEmail
            );
        }
        else
        {
            existingUser = await _userManager.FindByNameAsync(
                login.UserNameOrEmail
            );
        }
        if (existingUser == null)
        {
            return null;
        }
        var IsPasswordValid = await _userManager.CheckPasswordAsync(
            existingUser,
            login.Password
        );
        if (!IsPasswordValid)
        {
            return null;
        }
        var jwtResult = _jwtService.GenerateJwtToken(existingUser);
        var refreshToken = await _refreshTokenService.CreateAsync(
            existingUser,
            jwtResult.JwtId
        );
        return new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = refreshToken.Token,
            ExpiresAt = jwtResult.ExpiresAt,
            User = new UserReadDto
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName ?? string.Empty,
                Email = existingUser.Email ?? string.Empty
            }
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequest)
    {
        try
        {
            var result = await VerifyAndGenerateTokenAsync(tokenRequest);
            return result;
        }
        catch (Exception )
        {
            throw;
        }
    }

    private async Task<AuthResponseDto?> VerifyAndGenerateTokenAsync(
        TokenRequestDto requestDto
    )
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        // 1. Validate JWT Token

        ClaimsPrincipal tokenInVerification;
        SecurityToken validatedToken;
        try
        {
            tokenInVerification = jwtTokenHandler.ValidateToken(
                requestDto.Token,
                _validationParameters,
                out validatedToken
            );
        }
        catch (SecurityTokenExpiredException)
        {
            tokenInVerification = jwtTokenHandler.ValidateToken(
                requestDto.Token,
                GetValidationParameters(),
                out validatedToken
            );
        }
        catch
        {
            return null;
        }
        // 2. Validate JWT format

        if (validatedToken is not JwtSecurityToken jwtSecurityToken)
        {
            return null;
        }
        // 3. Validate encryption algorithm

        var isValidAlgorithm =
            jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            );

        if (!isValidAlgorithm)
        {
            return null;
        }
        // 4. Get JWT ID

        var jti = tokenInVerification
            .Claims
            .FirstOrDefault(
                claim => claim.Type == JwtRegisteredClaimNames.Jti
            )
            ?.Value;

        if (string.IsNullOrEmpty(jti))
        {
            return null;
        }
        // 5. Get JWT expiration

        var expClaim = tokenInVerification
            .Claims
            .FirstOrDefault(
                claim => claim.Type == JwtRegisteredClaimNames.Exp
            )
            ?.Value;

        if (!long.TryParse(
            expClaim,
            out var utcExpiryDate
        ))
        {
            return null;
        }

        var expiryDate = UnixTimeStampToDateTimeInUTC(
            utcExpiryDate
        );


        // Access token must be expired
        if (expiryDate > DateTime.UtcNow)
        {
            return null;
            // throw new Exception(
            //     "Token has not expired yet!"
            // );
        }
        // 6. Find refresh token

        var dbRefreshToken =
            await _refreshTokenService.GetByTokenAsync(
                requestDto.RefreshToken
            );

        if (dbRefreshToken == null)
        {
            return null;
            // throw new Exception(
            //     "Refresh token does not exist in our database."
            // );
        }
        // 7. Validate JWT ID

        if (dbRefreshToken.JwtId != jti)
        {
            return null;
            // throw new Exception(
            //     "Refresh token does not match the access token."
            // );
        }
        // 8. Validate refresh token

        if (dbRefreshToken.DateExpire <= DateTime.UtcNow)
        {
            return null;
            // throw new Exception(
            //     "Your refresh token has expired. Please login again."
            // );
        }
        if (dbRefreshToken.IsRevoked)
        {
            return null;
            // throw new Exception(
            //     "Refresh token is revoked."
            // );
        }
        // 9. Get user

        var dbUserData = await _userManager.FindByIdAsync(
            dbRefreshToken.UserId
        );
        if (dbUserData == null)
        {
            return null;
        }
        // 10. Get user roles

        // var roles = await _userManager.GetRolesAsync(
        //     dbUserData
        // );

        // 11. Revoke old refresh token

        dbRefreshToken.IsRevoked = true;

        await _appDbContext.SaveChangesAsync();
        // 12. Generate new JWT
        var jwtResult = _jwtService.GenerateJwtToken(
            dbUserData
        );
        // 13. Generate new refresh token
        var newRefreshToken =
            await _refreshTokenService.CreateAsync(
                dbUserData,
                jwtResult.JwtId
            );
        // 14. Return response

        return new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = jwtResult.ExpiresAt,
            User = new UserReadDto
            {
                Id = dbUserData.Id,
                UserName = dbUserData.UserName
                    ?? string.Empty,
                Email = dbUserData.Email
                    ?? string.Empty
            }
        };
    }
    // private DateTime UnixTimeStampToDateTimeInUTC(long unixTimeStamp)
    // {
    //     var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //     dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
    //     return dateTimeVal;
    // }
    private static DateTime UnixTimeStampToDateTimeInUTC(
        long unixTimeStamp
    )
    {
        return DateTimeOffset
            .FromUnixTimeSeconds(unixTimeStamp)
            .UtcDateTime;
    }
    private TokenValidationParameters GetValidationParameters(
        bool validateLifetime = true
    )
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is not configured."
            );

        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured."
            );

        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured."
            );


        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateIssuer = true,

            ValidIssuer = jwtIssuer,

            ValidateAudience = true,

            ValidAudience = jwtAudience,

            ValidateLifetime = validateLifetime,

            ClockSkew = TimeSpan.Zero
        };
    }
}
