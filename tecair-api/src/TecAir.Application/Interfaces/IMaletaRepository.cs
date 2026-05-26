using TecAir.Contracts.Dtos;
using TecAir.Domain.Models;

namespace TecAir.Application.Interfaces;

public interface IMaletaRepository
{
    Task<List<MaletaResponse>> GetMaletasAsync();
    Task<bool> ExisteCheckInAsync(int idCheckIn);
    Task<bool> ExisteNumMaletaAsync(string numMaleta);
    Task<int> ContarMaletasPorCheckInAsync(int idCheckIn);
    Task<List<MaletaResponse>> GetMaletasPorCheckInAsync(int idCheckIn);
    Task AddAsync(Maleta maleta);
    Task SaveChangesAsync();
}
