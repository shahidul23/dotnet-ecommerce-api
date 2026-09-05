using System;
using System.Text;
using AutoMapper;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.Exceptions;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using dotnet_ecommerce_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_ecommerce_api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // Databases

        services.AddDbContext<AppDbContext>(option => 
        option.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection")
            )
        );
        
        // Controllers

        services.AddControllers();

        // Auto Mapper

        services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
        // services.AddAutoMapper(typeof(Program).Assembly);

        // Application service

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Swagger

        services.AddEndpointsApiExplorer();
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing from configuration.");

        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing from configuration.");

        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing from configuration.");
        // token validation paramiter
        var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        services.AddSingleton(tokenValidationParameter);    
        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        // Add Authentication 
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        
        //Add JWT Bearer
        .AddJwtBearer(option =>
        {
            option.SaveToken = true;
            option.RequireHttpsMetadata = false;
            option.TokenValidationParameters = tokenValidationParameter;
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        services.AddSwaggerGen();

        //Open Api
        services.AddOpenApi();

        return services;
    }
}
