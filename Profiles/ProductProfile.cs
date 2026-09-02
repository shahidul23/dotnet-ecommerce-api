using System;
using AutoMapper;
using dotnet_ecommerce_api.DTOs.ProductsDtos;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Profiles;

public class ProductProfile:Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductReadDto>();
        CreateMap<Product, ProductWithCategoryReadDto>();
        CreateMap<ProductCreateDto, Product>()
            .ForMember(
                dest => dest.CategortId,
                opt => opt.MapFrom(src => src.CategoryId)
            );
        CreateMap<ProductUpdateDto, Product>()
        .ForMember(
            dest => dest.CategortId,
            opt => opt.MapFrom(src => src.CategoryId)
        );

    }
}
