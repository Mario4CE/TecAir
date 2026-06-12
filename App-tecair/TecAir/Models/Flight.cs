using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Vuelo para la base de datos local SQLite
    /// </summary>
    [Table("Flights")]
    public class Flight
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Unique]
        public string FlightNumber { get; set; }

        [NotNull]
        public int RouteId { get; set; }

        [NotNull]
        public int AircraftId { get; set; }

        [NotNull]
        public DateTime DepartureTime { get; set; }

        [NotNull]
        public DateTime ArrivalTime { get; set; }

        public int Status { get; set; } = 0; // 0: Scheduled, 1: CheckedIn, 2: Open, 3: Closed, 4: Cancelled

        public int AvailableSeats { get; set; }

        // ID del vuelo en el API remoto (0 = no sincronizado aún)
        public int ApiId { get; set; } = 0;

        // Información del origen del vuelo
        public string Origin { get; set; } = "";

        // Información del destino del vuelo
        public string Destination { get; set; } = "";

        // Precio del vuelo
        public decimal Price { get; set; }
    }
}
