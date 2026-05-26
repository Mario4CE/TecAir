using Microsoft.EntityFrameworkCore;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Repositories;

public sealed class ReservacionRepository(TecAirDb db) : IReservacionRepository
{
    public async Task<List<ReservacionResponse>> GetReservacionesAsync(int? idUsuario)
    {
        var query = db.Reservaciones
            .AsNoTracking()
            .OrderByDescending(x => x.FechaReservacion)
            .AsQueryable();

        if (idUsuario.HasValue)
        {
            query = query.Where(x => x.IdUsuario == idUsuario.Value);
        }

        var items = await query
            .Include(x => x.Usuario)
            .Include(x => x.Pago)
            .ToListAsync();

        var vuelos = await db.Vuelos
            .AsNoTracking()
            .ToDictionaryAsync(x => x.IdVuelo);

        return items.Select(r => new ReservacionResponse(
            r.IdReservacion,
            r.Estado,
            r.FechaReservacion,
            r.IdUsuario,
            r.IdVuelo,
            r.Usuario is null ? null : new
            {
                r.Usuario.IdUsuario,
                r.Usuario.Nombre1,
                r.Usuario.Apellido1,
                r.Usuario.Correo,
                r.Usuario.Telefono
            },
            vuelos.TryGetValue(r.IdVuelo, out var v)
                ? new
                {
                    v.IdVuelo,
                    v.FechaSalida,
                    v.HoraSalida,
                    v.Puerta,
                    v.Estado,
                    v.Matricula,
                    v.IdRuta,
                    v.Precio
                }
                : null,
            r.Pago is null ? null : new
            {
                r.Pago.IdPago,
                r.Pago.IdReservacion,
                r.Pago.Monto,
                r.Pago.Metodo
            }
        )).ToList();
    }

    public async Task<ReservacionResponse?> GetReservacionByIdAsync(int idReservacion) =>
        (await GetReservacionesAsync(null)).FirstOrDefault(x => x.IdReservacion == idReservacion);

    public async Task<Usuario?> GetUsuarioByIdAsync(int idUsuario) =>
        await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

    public async Task<bool> ExisteVueloAsync(int idVuelo) =>
        await db.Vuelos.AnyAsync(x => x.IdVuelo == idVuelo);

    public async Task AddAsync(Reservacion reservacion) =>
        await db.Reservaciones.AddAsync(reservacion);

    public async Task<Reservacion?> GetEntityByIdAsync(int idReservacion) =>
        await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);

    public async Task SaveChangesAsync() =>
        await db.SaveChangesAsync();
}
