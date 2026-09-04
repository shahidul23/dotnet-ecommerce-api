using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.DTOs.Auth;

public class TokenRequestDto
{
    [Required]
    public string Token {get; set;} = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;

}
