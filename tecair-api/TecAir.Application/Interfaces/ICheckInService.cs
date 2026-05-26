using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

public interface ICheckInService
{
    Task<List<CheckInResponse>> GetCheckInsAsync();
    Task<CheckInResponse> CrearCheckInAsync(CheckInRequest datos);
    Task<PaseAbordarResponse?> GetPaseAbordarAsync(int idCheckIn);
}
