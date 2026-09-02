using System;
using dotnet_ecommerce_api.Common;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddApiValidation(
        this IServiceCollection services
    )
    {
        services.Configure<ApiBehaviorOptions>(option =>{
            option.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => 
                        e.Value != null &&
                        e.Value.Errors.Count > 0
                    )
                    .SelectMany(e => 
                        e.Value!.Errors.Select(error => 
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value." 
                            : error.ErrorMessage
                        )
                    )
                    .ToList();
                return new BadRequestObjectResult(
                    ApiResponse<object>.ErrorResponse(
                        errors,
                        400,
                        "Validation Failed"
                    )
                );
            };
        });
        return services;
    }
}
