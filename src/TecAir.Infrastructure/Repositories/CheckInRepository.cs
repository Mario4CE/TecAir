using Microsoft.EntityFrameworkCore;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Repositories;

public sealed class CheckInRepository(TecAirDb db) : ICheckInRepository
{
    public async Task<List<CheckInResponse>> GetCheckInsAsync() => await db.CheckIns.AsNoTracking().OrderByDescending(x => x.IdCheckin)
        .Select(x => new CheckInResponse(x.IdCheckin, x.IdUsuario, x.IdVuelo, x.Asiento)).ToListAsync();
    public async Task<bool> ExisteAsientoAsync(int idVuelo, string asiento) => await db.CheckIns.AnyAsync(x => x.IdVuelo == idVuelo && x.Asiento == asiento);
    public async Task<bool> ExisteUsuarioAsync(int idUsuario) => await db.Usuarios.AnyAsync(x => x.IdUsuario == idUsuario);
    public async Task<bool> ExisteVueloAsync(int idVuelo) => await db.Vuelos.AnyAsync(x => x.IdVuelo == idVuelo);
    public async Task AddAsync(CheckIn checkIn) => await db.CheckIns.AddAsync(checkIn);
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    public async Task<CheckIn?> GetEntityByIdAsync(int idCheckIn) => await db.CheckIns.AsNoTracking().FirstOrDefaultAsync(x => x.IdCheckin == idCheckIn);
    public async Task<PaseAbordarResponse?> GetPaseAbordarAsync(int idCheckIn)
    {
        var c = await db.CheckIns.AsNoTracking().Include(x => x.Usuario).Include(x => x.Vuelo).FirstOrDefaultAsync(x => x.IdCheckin == idCheckIn);
        if (c?.Usuario is null || c.Vuelo is null) return null;
        return new PaseAbordarResponse(c.IdCheckin, c.Asiento, c.Usuario.IdUsuario, c.Usuario.Nombre1, c.Usuario.Apellido1, c.Vuelo.IdVuelo, c.Vuelo.Puerta, c.Vuelo.FechaSalida, c.Vuelo.HoraSalida);
    }
}
