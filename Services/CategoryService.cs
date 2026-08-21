using System;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Services;

public class CategoryService:ICategoryService
{
    private static readonly List<Category> _categories = new List<Category> ();

    public List<CategoryReadDto> GetAllcategories()
    {
       return _categories.Select(c => new CategoryReadDto
        {
            CategortId = c.CategortId,
            Name = c.Name,
            Description = c.Description,
            createdAt = c.createdAt
        }).ToList();
    }
    public CategoryReadDto? GetCategoryById(Guid categoryId)
    {
         var foundCategory = _categories.FirstOrDefault(c => c.CategortId == categoryId);
        if (foundCategory == null)
        {
            return null;
        }
        return new CategoryReadDto
        {
            CategortId = foundCategory.CategortId,
            Name = foundCategory.Name,
            Description = foundCategory.Description,
            createdAt = foundCategory.createdAt
        };
    }

    public CategoryReadDto CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        var newCategory =  new Category
        {
            CategortId = Guid.NewGuid(),
            Name = categoryCreateDto.Name,
            Description = categoryCreateDto.Description,
            createdAt = DateTime.UtcNow,
        };
        _categories.Add(newCategory);
        return new CategoryReadDto
        {
            CategortId = newCategory.CategortId,
            Name = newCategory.Name,
            Description = newCategory.Description,
            createdAt = newCategory.createdAt
        };
    }
    public CategoryReadDto? UpdateCategoryById(Guid categoryId, CategoryUpdateDto category)
    {
        var foundCategory = _categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory==null)
        {
            return null;
        }
        foundCategory.Name = category.Name;
        foundCategory.Description = category.Description;
        return new CategoryReadDto
        {
            CategortId = foundCategory.CategortId,
            Name = foundCategory.Name,
            Description = foundCategory.Description,
            createdAt = foundCategory.createdAt
        };
    }
    public bool DeleteCategoryById(Guid categoryId)
    {
        var foundCategory = _categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory == null)
        {
            return false;
        }
        _categories.Remove(foundCategory);
        return true;
    }
}
