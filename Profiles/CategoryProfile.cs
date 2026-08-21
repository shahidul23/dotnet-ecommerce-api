using System;
using AutoMapper;
using dotnet_ecommerce_api.DTOs;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Profiles;

public class CategoryProfile:Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryReadDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();
    }
}
