using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TecAir.Api;
using TecAir.Api.Data;
using TecAir.Api.Config;
using TecAir.Api.Interfaces;
using TecAir.Api.Repositories;
using TecAir.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TecAirOptions>(builder.Configuration.GetSection("TecAir"));
var tecAirOptions = builder.Configuration.GetSection("TecAir").Get<TecAirOptions>() ?? new TecAirOptions();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiGlobals.CorsPolicyName, policy =>
    {
        if (tecAirOptions.CorsOrigin == ApiGlobals.DefaultCorsOrigin)
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(tecAirOptions.CorsOrigin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAeropuertoRepository, AeropuertoRepository>();
builder.Services.AddScoped<IAeropuertoService, AeropuertoService>();
builder.Services.AddDbContext<TecAirDb>(options =>

{
    //var connectionString = builder.Configuration.GetConnectionString("TecAirDb") ?? "Data Source=data/tecair.sqlite";
    //options.UseSqlite(connectionString);
     options.UseInMemoryDatabase("TecAirDb");
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect(ApiGlobals.ApiBasePath));

app.UseCors(ApiGlobals.CorsPolicyName);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TecAirDb>();
    await DatabaseSeeder.SeedAsync(db);
}

app.MapTecAirApiEndpoints();

await app.RunAsync();

