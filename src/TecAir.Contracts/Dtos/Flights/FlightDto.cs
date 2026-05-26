namespace TecAir.Contracts.Dtos.Flights;

public sealed record FlightDto(
    int Id,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset DepartureUtc,
    DateTimeOffset ArrivalUtc,
    decimal BasePrice,
    FlightStatus Status);
