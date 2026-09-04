using System;
using dotnet_ecommerce_api.Common;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;

namespace dotnet_ecommerce_api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext appDbContext,
        IConfiguration configuration,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
        _configuration = configuration;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
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
}
