using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

public sealed class VueloRepository(TecAirDb db) : IVueloRepository
{
    public async Task<List<VueloResponse>> GetVuelosAsync(string? origen, string? destino)
    {
        var vuelos = await db.Vuelos.AsNoTracking()
            .Where(x => x.IdRuta > 0 && x.Matricula != null)
            .Select(x => new
            {
                x.IdVuelo,
                x.IdRuta,
                x.Matricula,
                x.Precio,
                FechaSalida = (DateOnly?)x.FechaSalida,
                HoraSalida = (TimeOnly?)x.HoraSalida,
                x.Puerta,
                x.Estado,
                Capacidad = x.Avion != null ? (int?)x.Avion.Capacidad : null,
                ReservacionesActivas = x.Reservaciones.Count(r => r.Estado != "cancelada")
            })
            .OrderBy(x => x.FechaSalida)
            .ThenBy(x => x.HoraSalida)
            .ToListAsync();

        var respuestas = new List<VueloResponse>();

        foreach (var vuelo in vuelos)
        {
            var escalas = await db.Escalas.AsNoTracking()
                .Where(x => x.IdRuta == vuelo.IdRuta)
                .OrderBy(x => x.Orden)
                .Join(db.Aeropuertos.AsNoTracking(),
                    escala => escala.IdAeropuerto,
                    aeropuerto => aeropuerto.IdAeropuerto,
                    (escala, aeropuerto) => new EscalaResponse(
                        escala.IdRuta,
                        escala.Orden,
                        escala.Tipo,
                        aeropuerto.IdAeropuerto,
                        aeropuerto.Nombre,
                        aeropuerto.Ubicacion))
                .ToListAsync();

            var escalaOrigen = escalas.FirstOrDefault(x => x.Tipo == "origen");
            var escalaDestino = escalas.FirstOrDefault(x => x.Tipo == "destino");

            if (!CoincideAeropuerto(escalaOrigen, origen) || !CoincideAeropuerto(escalaDestino, destino))
                continue;

            var capacidad = vuelo.Capacidad ?? 0;
            var reservacionesActivas = vuelo.ReservacionesActivas;

            respuestas.Add(new VueloResponse(
                vuelo.IdVuelo,
                vuelo.FechaSalida ?? DateOnly.MinValue,
                vuelo.HoraSalida ?? TimeOnly.MinValue,
                vuelo.Puerta ?? string.Empty,
                vuelo.Estado ?? "programado",
                vuelo.Matricula ?? string.Empty,
                vuelo.IdRuta,
                vuelo.Precio,
                capacidad,
                escalaOrigen?.Nombre ?? string.Empty,
                escalaDestino?.Nombre ?? string.Empty,
                escalaOrigen?.IdAeropuerto ?? 0,
                escalaDestino?.IdAeropuerto ?? 0,
                capacidad - reservacionesActivas,
                escalas));
        }

        return respuestas;
    }

    public async Task<VueloResponse?> GetVueloByIdAsync(int idVuelo) =>
        (await GetVuelosAsync(null, null)).FirstOrDefault(x => x.IdVuelo == idVuelo);

    public async Task AddAsync(Vuelo vuelo) => await db.Vuelos.AddAsync(vuelo);

    public async Task<Vuelo?> GetEntityByIdAsync(int idVuelo) => await db.Vuelos.FirstOrDefaultAsync(x => x.IdVuelo == idVuelo);

    public async Task SaveChangesAsync() => await db.SaveChangesAsync();

    private static bool CoincideAeropuerto(EscalaResponse? escala, string? filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro)) return true;
        if (escala is null) return false;
        return escala.IdAeropuerto.ToString() == filtro || escala.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase);
    }
}
