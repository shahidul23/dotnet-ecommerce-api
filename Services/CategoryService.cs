using System;
using AutoMapper;
using dotnet_ecommerce_api.Common;
using dotnet_ecommerce_api.Controllers;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Enums;
using dotnet_ecommerce_api.Helpers;
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

    public async Task<PaginatedResult<CategoryReadDto>> GetAllcategories(QueryParameters queryParameters)
    {
        var pageNumber = queryParameters.PageNumber;
        var pageSize = queryParameters.PageSize;
        var search = queryParameters.Search;
        var sortOrder = queryParameters.SortOrder;



        IQueryable<Category> query = _appDbContext.Categories;

        // search by name and description
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var formatValue = $"%{search.Trim()}%";
            query = query.Where(c=> EF.Functions.ILike(c.Name ,formatValue) || EF.Functions.ILike(c.Description, formatValue));
        }

        if (string.IsNullOrWhiteSpace(sortOrder))
        {
            query = query.OrderBy(c=>c.Name);
        }
        else
        {
            var formatedOsrtOrder = sortOrder.Trim().ToLower();
            if(Enum.TryParse<SortOrder>(formatedOsrtOrder, true, out var order))
            {
                query = order switch
                {
                    SortOrder.NameAsc => query.OrderBy(c=>c.Name),
                    SortOrder.NameDesc  => query.OrderByDescending(c => c.Name),
                    SortOrder.DescriptionAsc => query.OrderBy(c=>c.Description),
                    SortOrder.DescriptionDesc => query.OrderByDescending(c => c.Description),
                    SortOrder.CreatedAtAsc => query.OrderBy(c=>c.CreatedAt),
                    SortOrder.CreatedAtDesc => query.OrderByDescending(c => c.CreatedAt)
                };
            }
        }

        var totalCount = await query.CountAsync();
        // paginateion
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        var result = _mapper.Map<List<CategoryReadDto>>(items);
        return new PaginatedResult<CategoryReadDto>
        {
            Items = result,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize =pageSize
        };
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
