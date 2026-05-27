using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Maleta/Equipaje para la base de datos local SQLite
    /// </summary>
    [Table("Luggages")]
    public class Luggage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ReservationId { get; set; }

        [NotNull, Unique]
        public string LuggageNumber { get; set; }

        public decimal Weight { get; set; }

        public string Color { get; set; }

        public decimal Fee { get; set; }
    }
}
