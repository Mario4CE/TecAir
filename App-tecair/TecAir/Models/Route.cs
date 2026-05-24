using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Ruta para la base de datos local SQLite
    /// </summary>
    [Table("Routes")]
    public class Route
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int OriginAirportId { get; set; }

        [NotNull]
        public int DestinationAirportId { get; set; }

        [NotNull]
        public decimal BasePrice { get; set; }

        public int Duration { get; set; } // Duración estimada en minutos

        public string IntermediateAirports { get; set; } // JSON con IDs de aeropuertos intermedios
    }
}
