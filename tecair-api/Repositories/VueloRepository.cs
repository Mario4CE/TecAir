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
            .Include(x => x.Avion)
            .Include(x => x.Reservaciones)
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

            var reservacionesActivas = vuelo.Reservaciones.Count(x => x.Estado != "cancelada");

            respuestas.Add(new VueloResponse(
                vuelo.IdVuelo,
                vuelo.FechaSalida,
                vuelo.HoraSalida,
                vuelo.Puerta,
                vuelo.Estado,
                vuelo.Matricula,
                vuelo.IdRuta,
                vuelo.Precio,
                vuelo.Avion?.Capacidad ?? 0,
                escalaOrigen?.Nombre ?? string.Empty,
                escalaDestino?.Nombre ?? string.Empty,
                escalaOrigen?.IdAeropuerto ?? 0,
                escalaDestino?.IdAeropuerto ?? 0,
                (vuelo.Avion?.Capacidad ?? 0) - reservacionesActivas,
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
