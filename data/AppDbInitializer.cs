using System;
using dotnet_ecommerce_api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace dotnet_ecommerce_api.data;

public class AppDbInitializer
{
    public static async Task SeedRole(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateAsyncScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if(!await roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));
            }
            if(!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
        }
    }
}
