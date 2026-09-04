using System;
using dotnet_ecommerce_api.DTOs.Auth;

namespace dotnet_ecommerce_api.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto register);
    Task<AuthResponseDto?> LoginAsync(LoginDto login);
    Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequest);
}
