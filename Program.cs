using dotnet_ecommerce_api.Extensions;
var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddApiValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API is running!");
app.MapControllers();
app.Run();
