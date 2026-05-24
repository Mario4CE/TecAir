using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Pase de Abordar para la base de datos local SQLite
    /// </summary>
    [Table("BoardingPasses")]
    public class BoardingPass
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ReservationId { get; set; }

        public string BoardingGate { get; set; }

        public string SeatNumber { get; set; }

        public int SequenceNumber { get; set; }

        public DateTime IssuedAt { get; set; }
    }
}
