using System;

namespace dotnet_ecommerce_api.Models;

public class Product{
    public Guid GetGuid {get; set;}
    public string? Name {get; set;}
    public string? Description{get;set;}
    public decimal Price {get; set;}
    public int StockQuantity{get;set;}
    public string? CategoryName{get;set;}
}
