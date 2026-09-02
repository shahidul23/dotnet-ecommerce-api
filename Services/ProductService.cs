using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.DTOs.ProductsDtos;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_ecommerce_api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }
    // public async Task<List<ProductReadDto>> GetAllProduct()
    // {
    //     var products =  await _appDbContext.Products.ToListAsync();
    //     var result = _mapper.Map<List<ProductReadDto>>(products);

    //     return result;
    //     // return await _appDbContext.Products.ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider).ToListAsync();
    // }
    public async Task<List<ProductReadDto>> GetAllProduct()
    {
        return await _appDbContext.Products
            .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // public async Task<ProductReadDto?> GetProductById(Guid ProductId)
    // {
    //     var findProduct = await _appDbContext.Products.FindAsync(ProductId);
    //     return findProduct == null ? null : _mapper.Map<ProductReadDto>(findProduct);
    // }

    // public async Task<ProductWithCategoryReadDto?> GetProductById(Guid ProductId)
    // {
    //     return await _appDbContext.Products
    //         .Where(p => p.ProductId == ProductId)
    //         .ProjectTo<ProductWithCategoryReadDto>(_mapper.ConfigurationProvider)
    //         .FirstOrDefaultAsync();
    // }
    public async Task<ProductWithCategoryReadDto?> GetProductById(Guid ProductId)
    {
        var product = await(
            from p in _appDbContext.Products
            join c in _appDbContext.Categories on p.CategortId equals c.CategortId
            into categoryGroup
            from c in categoryGroup.DefaultIfEmpty()

            where p.ProductId == ProductId

            select new ProductWithCategoryReadDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategortId = p.CategortId,
                CreateAt = p.CreateAt,
                Category = c == null
                    ? null
                    : new CategoryReadDto
                    {
                        CategortId = c.CategortId,
                        Name = c.Name,
                        Description = c.Description
                    }
            }
        ).FirstOrDefaultAsync();
        return product;
    }

    public async Task<ProductReadDto> CreateProduct(ProductCreateDto productCreateDto)
    {
        var categoryExist = await _appDbContext.Categories.AnyAsync(c => c.CategortId == productCreateDto.CategoryId);
        if (!categoryExist)
        {
            throw new KeyNotFoundException(
                "Category not found."
            );
        }
        var newProduct = _mapper.Map<Product>(productCreateDto);
        await _appDbContext.Products.AddAsync(newProduct);
        await _appDbContext.SaveChangesAsync();
        return _mapper.Map<ProductReadDto>(newProduct);
    }

    public async Task<ProductReadDto?> ProductUPdateById(
        Guid productId, 
        ProductUpdateDto product
    )
    {
        var foundProduct = await _appDbContext.Products
        .FindAsync(productId);

        if (foundProduct == null)
        {
            return null;
        }

        var categoryExist = await _appDbContext.Categories
            .AnyAsync(c => c.CategortId == product.CategoryId);
        
        if (!categoryExist)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        _mapper.Map(product, foundProduct);

        await _appDbContext.SaveChangesAsync();
        
        return _mapper.Map<ProductReadDto>(foundProduct);
    }

    public async Task<bool> ProductDeleteById(Guid productId)
    {
        var foundProduct = await _appDbContext.Products.FindAsync(productId);

        if (foundProduct == null)
        {
            return false;
        }
        _appDbContext.Products.Remove(foundProduct);
        await _appDbContext.SaveChangesAsync();
        return true;
    }


}
