using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Tilføj Controller-tjenester til DI containeren
builder.Services.AddControllers();

// 2. Tilføj OpenAPI-støtte (krævet af Scalar)
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. Konfigurer OpenAPI og Scalar UI i Development-miljøet
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Viser interaktivt Scalar UI på /scalar/v1
}

app.UseHttpsRedirection();

// 4. Mappe controller-routes
app.MapControllers();

app.Run();