using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TecAir.Api;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Api.Config;
using TecAir.Application.DependencyInjection;
using TecAir.Infrastructure.DependencyInjection;

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

builder.Services
    .AddTecAirApplication()
    .AddTecAirInfrastructure();

builder.Services.AddDbContext<TecAirDb>(options =>

{

    //builder.Services.AddCors(); // Agrega servicios de CORS para permitir solicitudes desde el frontend
    //var connectionString = builder.Configuration.GetConnectionString("TecAirDb") ?? "Data Source=data/tecair.sqlite";
    //options.UseSqlite(connectionString);
    //options.UseInMemoryDatabase("TecAirDb");

    var connectionString = builder.Configuration.GetConnectionString("TecAirDb");
    options.UseNpgsql(connectionString);
    
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect(ApiGlobals.ApiBasePath));

app.UseCors(ApiGlobals.CorsPolicyName);

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<TecAirDb>();
//    await DatabaseSeeder.SeedAsync(db);
//}

app.MapTecAirApiEndpoints();


await app.RunAsync();

