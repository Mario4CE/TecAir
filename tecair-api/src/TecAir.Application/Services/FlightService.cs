using TecAir.Application.Abstractions;
using TecAir.Contracts;
using TecAir.Contracts.Common;
using TecAir.Contracts.Dtos.Flights;
using TecAir.Contracts.Requests.Flights;

namespace TecAir.Application.Services;

public sealed class FlightService : IFlightService
{
    public Task<FlightDto> CreateAsync(CreateFlightRequest request, CancellationToken cancellationToken = default)
    {
        var dto = new FlightDto(
            Id: 0,
            FlightNumber: request.FlightNumber,
            OriginCode: request.OriginCode,
            DestinationCode: request.DestinationCode,
            DepartureUtc: request.DepartureUtc,
            ArrivalUtc: request.ArrivalUtc,
            BasePrice: request.BasePrice,
            Status: FlightStatus.Scheduled);

        return Task.FromResult(dto);
    }

    public Task<PagedResult<FlightDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = new PagedResult<FlightDto>(Array.Empty<FlightDto>(), page, pageSize, 0);
        return Task.FromResult(result);
    }
}
