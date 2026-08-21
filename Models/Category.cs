using System;

namespace dotnet_ecommerce_api.Models;

public class Category
{
    public Guid CategortId{get;set;}
    public string? Name{get;set;}
    public string? Description{get; set;} = string.Empty;
    public DateTime createdAt{get;set;}
}

