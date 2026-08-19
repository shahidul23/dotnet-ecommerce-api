using System;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController:ControllerBase
{
    private static List<Category> categories = new List<Category> ();
    // Get Request /api/categories
    [HttpGet]
    public IActionResult GetCategories([FromQuery] string searchValue="")
    {
        if (!string.IsNullOrEmpty(searchValue))
        {
            var searchCat = categories.Where(c =>!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchValue ,StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(searchCat);

        }
        return Ok(categories);
    }   
    
    [HttpPost]
    public IActionResult CreateCategory([FromBody] Category category)
    {
        if (string.IsNullOrEmpty(category.Name))
        {
            return BadRequest("Category Name is Required and can not be empty");
        }
        var newCategory =new Category
        {
            CategortId = Guid.NewGuid(),
            Name = category.Name,
            Description = category.Description,
            createdAt = DateTime.UtcNow,
        };
        categories.Add(newCategory);

        return Created($"/api/categories/{newCategory.CategortId}",newCategory);
    }
    
    [HttpPut("{categoryId:guid}")]
    public IActionResult UpdateCatagory(Guid categoryId, [FromBody] Category category)
    {
        var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory==null)
        {
            return NotFound("Category dose not exist");
        }
        if (string.IsNullOrEmpty(category.Name))
        {
            return BadRequest("Category Name is Required and can not be empty");
        }
        if(category == null)
        {
            return BadRequest("Category with this is dose not exists");
        }
        foundCategory.Name = category.Name ?? category.Name;
        foundCategory.Description = category.Description ?? category.Description;
        return NoContent();
    }
    
    [HttpDelete("{categoryId:guid}")]
    public IActionResult DeleteCategory(Guid categoryId)
    {
        var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
        if (foundCategory==null)
        {
            return NotFound("Category dose not exist");
        }
        categories.Remove(foundCategory);
        return NoContent();
    }
}
