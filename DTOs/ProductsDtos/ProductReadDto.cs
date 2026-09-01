using System;

namespace dotnet_ecommerce_api.DTOs.ProductsDtos;

public class ProductReadDto
{
    public Guid ProductId {get; set;}
    public string? Name {get; set;} = string.Empty;
    public string? Description {get; set;} = string.Empty;
    public decimal Price {get; set;}
    public int StockQuantity {get; set;}
    public Guid CategortId {get; set;}
    public DateTime CreateAt {get; set;}

}
