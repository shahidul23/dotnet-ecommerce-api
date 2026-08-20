using dotnet_ecommerce_api.Controllers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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
