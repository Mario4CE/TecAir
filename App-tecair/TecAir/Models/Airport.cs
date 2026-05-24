using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Aeropuerto para la base de datos local SQLite
    /// </summary>
    [Table("Airports")]
    public class Airport
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Unique]
        public string Code { get; set; } // Código IATA (SJO, LAX, etc)

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string City { get; set; }

        [NotNull]
        public string Country { get; set; }
    }
}
