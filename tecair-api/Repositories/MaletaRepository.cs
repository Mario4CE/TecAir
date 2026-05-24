using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

public sealed class MaletaRepository(TecAirDb db) : IMaletaRepository
{
    public async Task<List<MaletaResponse>> GetMaletasAsync() => await db.Maletas.AsNoTracking().OrderBy(x => x.IdCheckin).ThenBy(x => x.NumMaleta)
        .Select(x => new MaletaResponse(x.NumMaleta, x.Peso, x.Color, x.IdCheckin)).ToListAsync();
    public async Task<bool> ExisteCheckInAsync(int idCheckIn) => await db.CheckIns.AnyAsync(x => x.IdCheckin == idCheckIn);
    public async Task<bool> ExisteNumMaletaAsync(string numMaleta) => await db.Maletas.AnyAsync(x => x.NumMaleta == numMaleta);
    public async Task<int> ContarMaletasPorCheckInAsync(int idCheckIn) => await db.Maletas.CountAsync(x => x.IdCheckin == idCheckIn);
    public async Task<List<MaletaResponse>> GetMaletasPorCheckInAsync(int idCheckIn) => await db.Maletas.AsNoTracking().Where(x => x.IdCheckin == idCheckIn).OrderBy(x => x.NumMaleta)
        .Select(x => new MaletaResponse(x.NumMaleta, x.Peso, x.Color, x.IdCheckin)).ToListAsync();
    public async Task AddAsync(Maleta maleta) => await db.Maletas.AddAsync(maleta);
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
