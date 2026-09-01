using System;

namespace dotnet_ecommerce_api.Models;

public class Category
{
    public Guid CategortId{get;set;}
    public string Name{get;set;} = string.Empty;
    public string Description{get; set;} = string.Empty;
    public DateTime createdAt{get;set;}
    public ICollection<Product> Products {get; set;} = new List<Product>(); 
    public Category()
    {
        CategortId = Guid.NewGuid();
        createdAt = DateTime.UtcNow;
    }
}

