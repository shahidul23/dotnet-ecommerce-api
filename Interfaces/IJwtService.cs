using System;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Interfaces;

public interface IJwtService
{
    JwtTokenResult GenerateJwtToken(
        ApplicationUser? user,
        IList<string> roles
        );
    DateTime GetExpiration();
}
