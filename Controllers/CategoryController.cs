using System;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controllers;

[ApiController]
[Route("v1/api/categories")]
public class CategoryController:ControllerBase
{
    private static List<Category> categories = new List<Category> ();
    // Get Request /api/categories
    [HttpGet]
    public IActionResult GetCategories([FromQuery] string searchValue="")
    {
        // if (!string.IsNullOrEmpty(searchValue))
        // {
        //     var searchCat = categories.Where(c =>!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchValue ,StringComparison.OrdinalIgnoreCase)).ToList();

        //     return Ok(searchCat);

        // }
        var categotyList = categories.Select(c => new CategoryReadDto
        {
            CategortId = c.CategortId,
            Name = c.Name,
            Description = c.Description,
            createdAt = c.createdAt
        }).ToList();
        return Ok(ApiResponse<List<CategoryReadDto>>.SuccessResponse(categotyList, 200, "Category Return Successfully"));
    }  
    // Read a category Id 
    [HttpGet("{categoryId:guid}")]
    public IActionResult getCategoryById(Guid categoryId)
    {
        var foundCategory = categories.FirstOrDefault(c => c.CategortId == categoryId);
        if (foundCategory == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string>{"Category not found with this id"}, 404, "Validation Error"));
        }
        var categoryRead = new CategoryReadDto
        {
            CategortId = foundCategory.CategortId,
            Name = foundCategory.Name,
            Description = foundCategory.Description,
            createdAt = foundCategory.createdAt
        };
        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(categoryRead, 200, "Category Return Successfully"));
    }
    
    [HttpPost]
    public IActionResult CreateCategory([FromBody] CategoryCreateDto category)
    {
        var newCategory =new Category
        {
            CategortId = Guid.NewGuid(),
            Name = category.Name,
            Description = category.Description,
            createdAt = DateTime.UtcNow,
        };
        categories.Add(newCategory);
        var categoryRead = new CategoryReadDto
        {
            CategortId = newCategory.CategortId,
            Name = newCategory.Name,
            Description = newCategory.Description,
            createdAt = newCategory.createdAt
        };
        return Created(nameof(getCategoryById),ApiResponse<CategoryReadDto>.SuccessResponse(categoryRead, 201, "Category Create Successfully"));
    }
    
    [HttpPut("{categoryId:guid}")]
    public IActionResult UpdateCatagory(Guid categoryId, [FromBody] CategoryUpdateDto category)
    {
        var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory==null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string>{"Category not Found with this id"}, 400, "Validation failed"));
        }
        foundCategory.Name = category.Name ?? category.Name;
        foundCategory.Description = category.Description ?? category.Description;
        return Ok(ApiResponse<object>.SuccessResponse(null, 204, "Category Update Successfully"));
    }
    
    [HttpDelete("{categoryId:guid}")]
    public IActionResult DeleteCategory(Guid categoryId)
    {
        var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory==null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string>{"Category not Found with this id"}, 400, "Validation failed"));
        }
        categories.Remove(foundCategory);
        return Ok(ApiResponse<object>.SuccessResponse(null, 204, "Category Deleted Successfully"));
    }
}
