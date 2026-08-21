using System;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Helpers;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using dotnet_ecommerce_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controllers;

[ApiController]
[Route("v1/api/categories")]
public class CategoryController:ControllerBase
{
    // private CategoryService _categoryService;
    // public CategoryController(CategoryService categoryService)
    // {
    //     _categoryService = categoryService;
    // }
    private ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    // Get Request /api/categories
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] QueryParameters queryParameters)
    {
        // if (!string.IsNullOrEmpty(searchValue))
        // {
        //     var searchCat = categories.Where(c =>!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchValue ,StringComparison.OrdinalIgnoreCase)).ToList();

        //     return Ok(searchCat);

        // }
        // Console.WriteLine($" Page Nummber : {pageNumber} PageSize : {pageSize}");
        queryParameters.Validate();
        var categotyList =await _categoryService.GetAllcategories(queryParameters);
        
        return Ok(ApiResponse<PaginatedResult<CategoryReadDto>>.SuccessResponse(categotyList, 200, "Category Return Successfully"));
    }  
    // Read a category Id 
    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> getCategoryById(Guid categoryId)
    {
       var category = await _categoryService.GetCategoryById(categoryId);
       if (category == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string> {"Category with this ID Dose not exist"}, 400, "Validation Failed"));
        }
        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(category, 200, "Category Return Successfully"));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto category)
    {
        var categoryRead =await _categoryService.CreateCategory(category);
        return Created(nameof(getCategoryById),ApiResponse<CategoryReadDto>.SuccessResponse(categoryRead, 201, "Category Create Successfully"));
    }
    
    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCatagory(Guid categoryId, [FromBody] CategoryUpdateDto category)
    {
        var Category =await _categoryService.UpdateCategoryById(categoryId, category);
        if (Category==null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string>{"Category not Found with this id"}, 400, "Validation failed"));
        }

        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(Category, 204, "Category Update Successfully"));
    }
    
    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var category = await _categoryService.DeleteCategoryById(categoryId);
        if (!category)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(new List<string>{"Category not Found with this id"}, 400, "Validation failed"));
        }
        return Ok(ApiResponse<object>.SuccessResponse(null, 204, "Category Deleted Successfully"));
    }
}
