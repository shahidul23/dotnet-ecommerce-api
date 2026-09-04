using System;
using System.Security.Cryptography;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_ecommerce_api.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;
    public RefreshTokenService(
        AppDbContext appDbContext, 
        IConfiguration configuration
        )
    {
        _appDbContext = appDbContext;
        _configuration = configuration;
    }
    public async Task<RefreshToken> CreateAsync(ApplicationUser user, string jwtId)
    {
        var expirationDays = _configuration.GetValue<int?>("Jwt:RefreshTokenExpirationDays") ?? 7;
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateToken(),
            JwtId = jwtId,
            IsRevoked = false,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddDays(expirationDays),
        };
        
        await _appDbContext.AddAsync(refreshToken);
        await _appDbContext.SaveChangesAsync();

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
    
}
