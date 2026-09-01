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
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
    }
}
