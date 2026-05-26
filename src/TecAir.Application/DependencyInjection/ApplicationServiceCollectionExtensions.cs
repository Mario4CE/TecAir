using Microsoft.Extensions.DependencyInjection;
using TecAir.Application.Abstractions;
using TecAir.Application.Interfaces;
using TecAir.Application.Services;

namespace TecAir.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTecAirApplication(this IServiceCollection services)
    {
        services.AddScoped<IFlightService, FlightService>();

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAeropuertoService, AeropuertoService>();
        services.AddScoped<IAvionService, AvionService>();
        services.AddScoped<IRutaService, RutaService>();
        services.AddScoped<IVueloService, VueloService>();
        services.AddScoped<IReservacionService, ReservacionService>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IPromocionService, PromocionService>();
        services.AddScoped<ICheckInService, CheckInService>();
        services.AddScoped<IMaletaService, MaletaService>();
        services.AddScoped<ICalculoCobroMaletaStrategy, CalculoCobroMaletaStrategy>();

        return services;
    }
}
