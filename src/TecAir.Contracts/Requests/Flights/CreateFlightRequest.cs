namespace TecAir.Contracts.Requests.Flights;

public sealed record CreateFlightRequest(
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset DepartureUtc,
    DateTimeOffset ArrivalUtc,
    decimal BasePrice);
