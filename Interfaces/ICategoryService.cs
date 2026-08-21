using System;
using dotnet_ecommerce_api.Controllers;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Helpers;

namespace dotnet_ecommerce_api.Interfaces;

public interface ICategoryService
{
    Task<PaginatedResult<CategoryReadDto>> GetAllcategories(QueryParameters queryParameters);
    Task<CategoryReadDto?> GetCategoryById(Guid categoryId);
    Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryCreateDto);
    Task<CategoryReadDto?> UpdateCategoryById(Guid categoryId, CategoryUpdateDto category);
    Task<bool> DeleteCategoryById(Guid categoryId);
}
