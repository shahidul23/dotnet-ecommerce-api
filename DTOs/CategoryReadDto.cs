using System;

namespace dotnet_ecommerce_api.DTOs;

public class CategoryReadDto
{
    public Guid CategortId{set;get;}
    public string? Name{get;set;}
    public string? Description{get; set;} = string.Empty;
    public DateTime createdAt{get;set;}
}
