using Microsoft.Extensions.DependencyInjection;
using TecAir.Application.Interfaces;
using TecAir.Infrastructure.Repositories;

namespace TecAir.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddTecAirInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAeropuertoRepository, AeropuertoRepository>();
        services.AddScoped<IAvionRepository, AvionRepository>();
        services.AddScoped<IRutaRepository, RutaRepository>();
        services.AddScoped<IVueloRepository, VueloRepository>();
        services.AddScoped<IReservacionRepository, ReservacionRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IPromocionRepository, PromocionRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();
        services.AddScoped<IMaletaRepository, MaletaRepository>();

        return services;
    }
}
