using System;
using dotnet_ecommerce_api.DTOs;

namespace dotnet_ecommerce_api.Interfaces;

public interface ICategoryService
{
    List<CategoryReadDto> GetAllcategories();
    CategoryReadDto? GetCategoryById(Guid categoryId);
    CategoryReadDto CreateCategory(CategoryCreateDto categoryCreateDto);
    CategoryReadDto? UpdateCategoryById(Guid categoryId, CategoryUpdateDto category);
    bool DeleteCategoryById(Guid categoryId);
}
