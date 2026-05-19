using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TecAir.Api;
using TecAir.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TecAirOptions>(builder.Configuration.GetSection("TecAir"));
var tecAirOptions = builder.Configuration.GetSection("TecAir").Get<TecAirOptions>() ?? new TecAirOptions();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("TecAirCors", policy =>
    {
        if (tecAirOptions.CorsOrigin == "*")
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

builder.Services.AddDbContext<TecAirDb>(options =>
{
    //var connectionString = builder.Configuration.GetConnectionString("TecAirDb") ?? "Data Source=data/tecair.sqlite";
    //options.UseSqlite(connectionString);
     options.UseInMemoryDatabase("TecAirDb");
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/api"));

app.UseCors("TecAirCors");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TecAirDb>();
    await DatabaseSeeder.SeedAsync(db);
}

app.MapTecAirApiEndpoints();

await app.RunAsync();

