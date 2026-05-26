using TecAir.Contracts.Dtos;
using TecAir.Domain.Models;

namespace TecAir.Application.Interfaces;

public interface ICheckInRepository
{
    Task<List<CheckInResponse>> GetCheckInsAsync();
    Task<bool> ExisteAsientoAsync(int idVuelo, string asiento);
    Task<bool> ExisteUsuarioAsync(int idUsuario);
    Task<bool> ExisteVueloAsync(int idVuelo);
    Task AddAsync(CheckIn checkIn);
    Task SaveChangesAsync();
    Task<CheckIn?> GetEntityByIdAsync(int idCheckIn);
    Task<PaseAbordarResponse?> GetPaseAbordarAsync(int idCheckIn);
}
