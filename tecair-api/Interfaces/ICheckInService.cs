using TecAir.Api.Dtos;

namespace TecAir.Api.Interfaces;

public interface ICheckInService
{
    Task<List<CheckInResponse>> GetCheckInsAsync();
    Task<CheckInResponse> CrearCheckInAsync(CheckInRequest datos);
    Task<PaseAbordarResponse?> GetPaseAbordarAsync(int idCheckIn);
}
