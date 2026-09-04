using System;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateAsync(
        ApplicationUser? user,
        string jwtId
    );
    Task<RefreshToken?> GetByTokenAsync(
        string token
    );
    Task<bool> ValidateAsync(
        RefreshToken refreshToken
    );
    Task RevokeAsync(
        RefreshToken refreshToken
    );
}
