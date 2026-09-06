using dotnet_ecommerce_api.Common;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.DTOs.ProductsDtos;
using dotnet_ecommerce_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controllers.Product
{
    [Authorize(Roles = UserRoles.User)]
    [Route("v1/api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService =productService;
        }

        // Get All Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var productsList = await _productService.GetAllProduct();

            return Ok(
                ApiResponse<List<ProductReadDto>>.SuccessResponse(
                    productsList,
                    200,
                    "Products returned successfully"
                )
            );
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var product = await _productService.GetProductById(productId);

            if(product == null)
            {
                return NotFound(
                    ApiResponse<object>.ErrorResponse(
                        new List<string> {"Product with this ID Dose not exist"},
                        404,
                        "Product not found"
                    )
                );
            }
            return Ok(
                ApiResponse<ProductWithCategoryReadDto>.SuccessResponse(
                    product,
                    201,
                    "Product returned successfully"
                )
            );
        }

        // product Create
        [HttpPost]
        public async Task<IActionResult> ProductCreate([FromBody] ProductCreateDto product)
        {
            var Newproduct = await _productService.CreateProduct(product);
            return Created(
                nameof(GetProductById), 
                ApiResponse<ProductReadDto>.SuccessResponse(
                    Newproduct,
                    201,
                    "Product Create Successfully"
                )
            );

        }
        // Product update by Id
        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> ProductUPdateById(Guid productId, [FromBody] ProductUpdateDto product)
        {
            var updateProduct = await _productService.ProductUPdateById(productId,product);

            if (updateProduct == null)
            {
                return NotFound(
                    ApiResponse<object>.ErrorResponse(new List<string>{"Product with this ID Dose not exist"}, 404, "Product not found")
                );
            }
            return Ok(ApiResponse<ProductReadDto>.SuccessResponse(updateProduct, 204, "Product Update Successfully"));
        }

        // product Delete by id
        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> ProductDeleteById(Guid productId)
        {
            var product = await _productService.ProductDeleteById(productId);
            if (!product)
            {
                return NotFound(
                    ApiResponse<object>.ErrorResponse(
                        new List<string>{"Product with this ID Dose not exist"}, 404, "Product not Found"
                    )
                );
            }
            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null,
                    205,
                    "Product Delete Successfully"
                )
            );
        }
    }
}
