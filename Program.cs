using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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

List<Category> categories = new List<Category> ();

app.MapGet("/api/categories", ([FromQuery] string searchValue="") =>
{
    if (!string.IsNullOrEmpty(searchValue))
    {
        var searchCat = categories.Where(c =>!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchValue ,StringComparison.OrdinalIgnoreCase)).ToList();

        return Results.Ok(searchCat);

    }
    return Results.Ok(categories);
});

// app.MapPost("/api/categories", () =>
// {
//     var newCategory =new Category
//     {
//         CategortId = Guid.Parse("ce622e28-2244-44ef-96b4-68792e941b02"),
//         Name ="Electronics",
//         Description="This is a new Elctronic device for everyone",
//         createdAt = DateTime.UtcNow,
//     };
//     categories.Add(newCategory);

//     return Results.Created($"/api/categories/{newCategory.CategortId}",newCategory);
// });
app.MapPost("/api/categories", ([FromBody] Category categoryData) =>
{
    var newCategory =new Category
    {
        CategortId = Guid.NewGuid(),
        Name = categoryData.Name,
        Description = categoryData.Description,
        createdAt = DateTime.UtcNow,
    };
    categories.Add(newCategory);

    return Results.Created($"/api/categories/{newCategory.CategortId}",newCategory);
});
app.MapDelete("/api/categories/{categoryId}", (Guid categoryId) =>
{
    var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
    if (foundCategory==null)
    {
        return Results.NotFound("Category dose not exist");
    }
    categories.Remove(foundCategory);
    return Results.NoContent();
});
app.MapPut("/api/categories/{categoryId}", (Guid categoryId, [FromBody] Category categoryDate) =>
{
    var foundCategory = categories.FirstOrDefault(category => category.CategortId == categoryId);
    if (foundCategory==null)
    {
        return Results.NotFound("Category dose not exist");
    }
    foundCategory.Name = categoryDate.Name;
    foundCategory.Description = categoryDate.Description;
    return Results.NoContent();
});




// app.MapGet("/hello", () =>
// {
//     return new
//     {
//         message = "Hello World!",
//         status = true
//     };
// });

// app.MapGet("/users/{id}", (int id) =>
// {
//     return new
//     {
//         id = id,
//         name = "Shahidul",
//         message = "User found"
//     };
// });
// app.MapPost("/user/create", ()=>
// {
//     return new
//     {
//         name = "Shahidul Ialam",
//         massage ="Hello World"
//     };
// });
// var userGroup = app.MapGroup("/user");

// userGroup.MapGet("/", () =>
// {
//     return "Get all users";
// });

// userGroup.MapGet("/{id}", (int id) =>
// {
//     return $"Get user {id}";
// });

// userGroup.MapPost("/", () =>
// {
//     return "Create user";
// });

app.Run();

public record Product{
    public Guid GetGuid {get; set;}
    public string? Name {get; set;}
    public string? Description{get;set;}
    public decimal Price {get; set;}
    public int StockQuantity{get;set;}
    public string? CategoryName{get;set;}
}
public record Category
{
    public Guid CategortId{set;get;}
    public string? Name{get;set;}
    public string? Description{get; set;}
    public DateTime createdAt{get;set;}
}