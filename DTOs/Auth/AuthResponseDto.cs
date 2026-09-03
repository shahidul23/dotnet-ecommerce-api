using System;

namespace dotnet_ecommerce_api.DTOs.Auth;

public class AuthResponseDto
{
    public string Token {get; set; } = string.Empty;
    public string RefreshToken {get; set;} = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserReadDto User {get; set;} = new();
}
