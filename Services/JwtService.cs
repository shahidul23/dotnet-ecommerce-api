using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Exceptions;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_ecommerce_api.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResult GenerateJwtToken(
        ApplicationUser? user,
        IList<string> roles
        )
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new UnauthorizedException(
                "JWT Key is not configured."
            );


        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new UnauthorizedException(
                "JWT Issuer is not configured."
            );


        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new UnauthorizedException(
                "JWT Audience is not configured."
            );


        var expirationMinutes = int.TryParse(
            _configuration["Jwt:ExpirationMinutes"],
            out var minutes
        )
            ? minutes
            : throw new BadRequestException(
                "JWT expiration minutes is not configured or invalid."
            );


        var jwtId = Guid.NewGuid().ToString();


        var expiresAt = DateTime.UtcNow.AddMinutes(
            expirationMinutes
        );
        var AuthClaims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Jti,
                jwtId
            ),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id
            ),

            new Claim(
                ClaimTypes.Name,
                user.UserName ?? string.Empty
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty
            )
        };
        foreach (var role in roles)
        {
            AuthClaims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                )
            );
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(AuthClaims),
            Expires = expiresAt,
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateToken(
            tokenDescriptor
        );

        var token = tokenHandler.WriteToken(
            securityToken
        );

        return new JwtTokenResult
        {
            Token = token,
            ExpiresAt = expiresAt,
            JwtId = jwtId
        };
    }

    public DateTime GetExpiration()
    {
        var expirationMinutes = _configuration
            .GetValue<int?>("Jwt:ExpirationMinutes")
            ?? 60;
        return DateTime.UtcNow.AddMinutes(
            expirationMinutes
        );
    }
}
