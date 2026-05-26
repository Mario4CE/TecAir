namespace TecAir.Models
{
    /// <summary>
    /// Clase auxiliar para mostrar reservaciones con información completa del vuelo
    /// </summary>
    public class ReservationWithFlight
    {
        public Reservation Reservation { get; set; }
        public Flight Flight { get; set; }
        public Route Route { get; set; }
        public Airport OriginAirport { get; set; }
        public Airport DestinationAirport { get; set; }

        // Propiedades para binding
        public int Id => Reservation?.Id ?? 0;
        public int FlightId => Reservation?.FlightId ?? 0;
        public string FlightNumber => Flight?.FlightNumber ?? "N/A";
        public DateTime DepartureTime => Flight?.DepartureTime ?? DateTime.Now;
        public DateTime ReservationDate => Reservation?.ReservationDate ?? DateTime.Now;
        public string SeatNumber => Reservation?.SeatNumber ?? "N/A";
        public int LuggageCount => Reservation?.LuggageCount ?? 0;
        public decimal TotalPrice => Reservation?.TotalPrice ?? 0;
        public int Status => Reservation?.Status ?? 0;
        
        // Información de la ruta
        public string RouteDisplay => $"{OriginAirport?.Code} → {DestinationAirport?.Code}";
        public string CityDisplay => $"{OriginAirport?.City} → {DestinationAirport?.City}";
        public decimal BasePrice => Route?.BasePrice ?? 0;

        // Estado legible
        public string StatusDisplay => Status switch
        {
            0 => "Pendiente",
            1 => "Confirmada",
            2 => "Check-in",
            3 => "Abordada",
            4 => "Cancelada",
            5 => "No presentada",
            _ => "Desconocido"
        };

        // Color del estado
        public string StatusColor => Status switch
        {
            0 => "#FF9500",    // Pendiente - Naranja
            1 => "#34C759",    // Confirmada - Verde
            2 => "#007AFF",    // Check-in - Azul
            3 => "#5AC8FA",    // Abordada - Azul claro
            4 => "#FF3B30",    // Cancelada - Rojo
            5 => "#8E8E93",    // No presentada - Gris
            _ => "#000000"     // Negro por defecto
        };
    }
}
