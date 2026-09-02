using System;
using dotnet_ecommerce_api.DTOs.ProductsDtos;

namespace dotnet_ecommerce_api.Interfaces;

public interface IProductService
{
    Task<List<ProductReadDto>> GetAllProduct();
    Task<ProductWithCategoryReadDto?> GetProductById(Guid ProductId);
    Task<ProductReadDto> CreateProduct(ProductCreateDto product);
    Task<ProductReadDto?> ProductUPdateById(Guid productId, ProductUpdateDto product);
    Task<bool> ProductDeleteById(Guid productId);
}
