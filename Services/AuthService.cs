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

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext appDbContext,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
        _configuration = configuration;
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
}
