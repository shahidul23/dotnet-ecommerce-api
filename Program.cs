using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions> (options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var error = context.ModelState
            .Where(e =>e.Value !=null && e.Value.Errors.Count > 0)
            .Select(e => new
            {
                File = e.Key,
                Error =e.Value != null ? e.Value.Errors.Select(x => x.ErrorMessage).ToArray() : new string[0]
            }).ToList();
            // var errorString = string.Join("; ", error.Select(e => $"{e.File} : {string.Join(",", e.Error)}"));
        return new BadRequestObjectResult(new
        {
            Massage = "Validation Failed",
            Errors = error
        });
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
