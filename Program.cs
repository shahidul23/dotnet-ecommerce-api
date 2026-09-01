using dotnet_ecommerce_api.Controllers;
using dotnet_ecommerce_api.data;
using dotnet_ecommerce_api.Interfaces;
using dotnet_ecommerce_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => 
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
// builder.Services.Configure<ApiBehaviorOptions> (options =>
// {
//     options.InvalidModelStateResponseFactory = context =>
//     {
//         var error = context.ModelState
//             .Where(e =>e.Value !=null && e.Value.Errors.Count > 0)
//             .Select(e => new
//             {
//                 File = e.Key,
//                 Error =e.Value != null ? e.Value.Errors.Select(x => x.ErrorMessage).ToArray() : new string[0]
//             }).ToList();
//             // var errorString = string.Join("; ", error.Select(e => $"{e.File} : {string.Join(",", e.Error)}"));
//         return new BadRequestObjectResult(new
//         {
//             Massage = "Validation Failed",
//             Errors = error
//         });
//     };
// });
builder.Services.Configure<ApiBehaviorOptions> (options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value != null && e.Value.Errors.Count > 0)
            .SelectMany(e =>e.Value?.Errors != null ? e.Value.Errors.Select(x => x.ErrorMessage): new List<string>()).ToList();
        return new BadRequestObjectResult(ApiResponse<object>.ErrorResponse(errors, 400, "Validation Failed"));
    };
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "API is running!");
app.MapControllers();
app.Run();
