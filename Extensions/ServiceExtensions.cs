using System;
using AutoMapper;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Services;
using Microsoft.EntityFrameworkCore;

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

        // Swagger

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        //Open Api
        services.AddOpenApi();

        return services;
    }
}
