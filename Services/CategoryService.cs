using System;
using AutoMapper;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_ecommerce_api.Services;

public class CategoryService:ICategoryService
{
    // private static readonly List<Category> _categories = new List<Category> ();

    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    public async Task<List<CategoryReadDto>> GetAllcategories()
    {
        var categories = await _appDbContext.Categories.ToListAsync();
       return _mapper.Map<List<CategoryReadDto>>(categories);
        // return _categories.Select(c => new CategoryReadDto
        // {
        //     CategortId = c.CategortId,
        //     Name = c.Name,
        //     Description = c.Description,
        //     createdAt = c.createdAt
        // }).ToList();
    }
    public async Task<CategoryReadDto?> GetCategoryById(Guid categoryId)
    {
        var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);

        return foundCategory == null ? null : _mapper.Map<CategoryReadDto>(foundCategory);
        // if (foundCategory == null)
        // {
        //     return null;
        // }
        // return new CategoryReadDto
        // {
        //     CategortId = foundCategory.CategortId,
        //     Name = foundCategory.Name,
        //     Description = foundCategory.Description,
        //     createdAt = foundCategory.createdAt
        // };
        // return _mapper.Map<CategoryReadDto>(foundCategory);
    }

    public async Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        // var newCategory =  new Category
        // {
        //     CategortId = Guid.NewGuid(),
        //     Name = categoryCreateDto.Name,
        //     Description = categoryCreateDto.Description,
        //     createdAt = DateTime.UtcNow,
        // };
        var newCategory = _mapper.Map<Category>(categoryCreateDto);
        newCategory.CategortId = Guid.NewGuid();
        newCategory.Name = categoryCreateDto.Name;
        newCategory.Description = categoryCreateDto.Description;
        await _appDbContext.Categories.AddAsync(newCategory);
        await _appDbContext.SaveChangesAsync();
        // return new CategoryReadDto
        // {
        //     CategortId = newCategory.CategortId,
        //     Name = newCategory.Name,
        //     Description = newCategory.Description,
        //     createdAt = newCategory.createdAt
        // };
        return _mapper.Map<CategoryReadDto>(newCategory);
    }
    public async Task<CategoryReadDto?> UpdateCategoryById(Guid categoryId, CategoryUpdateDto category)
    {
        var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
        if (foundCategory==null)
        {
            return null;
        }
        _mapper.Map(category, foundCategory);
        _appDbContext.Categories.Update(foundCategory);
        await _appDbContext.SaveChangesAsync();
        // foundCategory.Name = category.Name;
        // foundCategory.Description = category.Description;
        return _mapper.Map<CategoryReadDto>(foundCategory);
        // return new CategoryReadDto
        // {
        //     CategortId = foundCategory.CategortId,
        //     Name = foundCategory.Name,
        //     Description = foundCategory.Description,
        //     createdAt = foundCategory.createdAt
        // };
    }
    public async Task<bool> DeleteCategoryById(Guid categoryId)
    {
        var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
        if (foundCategory == null)
        {
            return false;
        }
        _appDbContext.Categories.Remove(foundCategory);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}
