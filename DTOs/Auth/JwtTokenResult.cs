using System;

namespace dotnet_ecommerce_api.DTOs.Auth;

public class JwtTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string JwtId { get; set; } = string.Empty;
}
