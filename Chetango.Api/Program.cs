using Microsoft.EntityFrameworkCore;
using Chetango.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 📌 Agregamos el DbContext con la cadena de conexión del appsettings.json
builder.Services.AddDbContext<ChetangoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChetangoConnection")));

// Azure Entra ID (Azure AD) JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                // Placeholder para lógica de aprovisionamiento de usuario interno (claim oid)
                var oid = ctx.Principal?.FindFirst("oid")?.Value ?? ctx.Principal?.FindFirst("sub")?.Value;
                // TODO: consultar DbContext y crear registro Usuario si no existe.
                await Task.CompletedTask;
            }
        };
    }, options => { builder.Configuration.Bind("AzureAd", options); });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 👉 Endpoint de prueba generado por plantilla
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Endpoint de prueba protegido
app.MapGet("/auth/ping", (ClaimsPrincipal user) =>
{
    var oid = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    return Results.Ok(new { message = "pong", oid });
}).RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

