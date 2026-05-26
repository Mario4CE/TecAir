namespace TecAir.Domain.Entities;

public class Flight
{
    public int Id { get; private set; }
    public string FlightNumber { get; private set; } = string.Empty;
    public string OriginCode { get; private set; } = string.Empty;
    public string DestinationCode { get; private set; } = string.Empty;

    public Flight(string flightNumber, string originCode, string destinationCode)
    {
        FlightNumber = flightNumber;
        OriginCode = originCode;
        DestinationCode = destinationCode;
    }
}
