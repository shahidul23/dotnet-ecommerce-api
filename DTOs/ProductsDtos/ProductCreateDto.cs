using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.DTOs.ProductsDtos;
public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Product name must be between 2 and 100 characters."
    )]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "Price must be greater than 0."
    )]
    public decimal Price { get; set; }

    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "Stock quantity cannot be negative."
    )]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }
}
