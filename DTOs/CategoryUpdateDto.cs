using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.DTOs;

public class CategoryUpdateDto
{
    [StringLength(100, MinimumLength = 2, ErrorMessage ="Category must be between 2 and 100 Cherecter")]
    public string? Name{get;set;}= string.Empty;

     [StringLength(500, ErrorMessage ="Category must be exceed 500 Cherecter")]
    public string? Description{get; set;} = string.Empty;
}
