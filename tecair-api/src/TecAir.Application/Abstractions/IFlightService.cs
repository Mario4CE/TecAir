using TecAir.Contracts.Common;
using TecAir.Contracts.Dtos.Flights;
using TecAir.Contracts.Requests.Flights;

namespace TecAir.Application.Abstractions;

public interface IFlightService
{
    Task<FlightDto> CreateAsync(CreateFlightRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<FlightDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
