namespace TecAir.Models
{
    /// <summary>
    /// Clase auxiliar para mostrar vuelos con su información de ruta
    /// </summary>
    public class FlightWithRoute
    {
        public Flight Flight { get; set; }
        public Route Route { get; set; }
        public Airport OriginAirport { get; set; }
        public Airport DestinationAirport { get; set; }

        // Propiedades para binding
        public string FlightNumber => Flight?.FlightNumber;
        public DateTime DepartureTime => Flight?.DepartureTime ?? DateTime.Now;
        public int AvailableSeats => Flight?.AvailableSeats ?? 0;
        public string RouteDisplay => $"{OriginAirport?.Code} → {DestinationAirport?.Code}";
        public decimal BasePrice => Route?.BasePrice ?? 0;
    }
}
