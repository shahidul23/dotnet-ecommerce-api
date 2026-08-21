using System;
using dotnet_ecommerce_api.DTOs;

namespace dotnet_ecommerce_api.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryReadDto>> GetAllcategories();
    Task<CategoryReadDto?> GetCategoryById(Guid categoryId);
    Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryCreateDto);
    Task<CategoryReadDto?> UpdateCategoryById(Guid categoryId, CategoryUpdateDto category);
    Task<bool> DeleteCategoryById(Guid categoryId);
}
