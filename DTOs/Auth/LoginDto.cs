using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string UserNameOrEmail {get; set;} = string.Empty;

    [Required]
    public string Password {get; set;} = string.Empty;
}
