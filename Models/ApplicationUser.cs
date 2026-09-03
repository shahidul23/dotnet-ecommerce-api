using System;
using Microsoft.AspNetCore.Identity;

namespace dotnet_ecommerce_api.Models;

public class ApplicationUser:IdentityUser
{
    public string Custom { get; set; } = string.Empty;
}
