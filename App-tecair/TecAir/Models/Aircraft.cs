using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Aeronave para la base de datos local SQLite
    /// </summary>
    [Table("Aircrafts")]
    public class Aircraft
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Unique]
        public string Registration { get; set; } // Matrícula del avión

        [NotNull]
        public string Model { get; set; }

        public int Capacity { get; set; } // Número de pasajeros

        public string Manufacturer { get; set; }
    }
}
