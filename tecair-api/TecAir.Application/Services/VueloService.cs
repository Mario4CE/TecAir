using Microsoft.Extensions.Options;
using TecAir.Application.Configuration;
using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Application.Services;

public sealed class VueloService(IVueloRepository vueloRepository, IOptions<TecAirOptions> options) : IVueloService
{
    public async Task<List<VueloResponse>> GetVuelosAsync(string? origen, string? destino) => await vueloRepository.GetVuelosAsync(origen, destino);

    public async Task<VueloResponse?> GetVueloByIdAsync(int idVuelo) => await vueloRepository.GetVueloByIdAsync(idVuelo);

    public async Task<VueloResponse> CrearVueloAsync(VueloRequest datos)
    {
        if (datos.IdRuta <= 0) throw new InvalidOperationException("La ruta es obligatoria.");
        if (string.IsNullOrWhiteSpace(datos.Matricula)) throw new InvalidOperationException("La matrícula del avión es obligatoria.");
        if (datos.FechaSalida is null) throw new InvalidOperationException("La fecha de salida es obligatoria.");

        var tecAirOptions = options.Value;
        var vuelo = new Vuelo
        {
            IdRuta = datos.IdRuta,
            Matricula = datos.Matricula,
            FechaSalida = datos.FechaSalida.Value,
            HoraSalida = datos.HoraSalida ?? new TimeOnly(8, 0),
            Puerta = datos.Puerta ?? tecAirOptions.DefaultGate,
            Estado = datos.Estado ?? "programado",
            Precio = datos.Precio ?? tecAirOptions.DefaultFlightPrice
        };

        await vueloRepository.AddAsync(vuelo);
        await vueloRepository.SaveChangesAsync();
        return (await vueloRepository.GetVueloByIdAsync(vuelo.IdVuelo))!;
    }

    public async Task<VueloResponse?> CambiarEstadoAsync(int idVuelo, string estado)
    {
        var vuelo = await vueloRepository.GetEntityByIdAsync(idVuelo);
        if (vuelo is null) return null;
        vuelo.Estado = estado;
        await vueloRepository.SaveChangesAsync();
        return await vueloRepository.GetVueloByIdAsync(idVuelo);
    }
}
