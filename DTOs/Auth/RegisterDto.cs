using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dotnet_ecommerce_api.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength =3)]
    public string UserName{get; set;} = string.Empty;

    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;

    [Required]
    [StringLength(100, MinimumLength =6)]
    public string Password {get; set;} =string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword {get; set;} = string.Empty;
}
