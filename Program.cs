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

app.MapGet("/hello", () =>
{
    return new
    {
        message = "Hello World!",
        status = true
    };
});

app.MapGet("/users/{id}", (int id) =>
{
    return new
    {
        id = id,
        name = "Shahidul",
        message = "User found"
    };
});

app.Run();