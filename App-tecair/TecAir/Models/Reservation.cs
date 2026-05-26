using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Reservación para la base de datos local SQLite
    /// </summary>
    [Table("Reservations")]
    public class Reservation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [NotNull]
        public int FlightId { get; set; }

        [NotNull]
        public DateTime ReservationDate { get; set; }

        public int Status { get; set; } = 0; // 0: Pending, 1: Confirmed, 2: CheckedIn, 3: Boarded, 4: Cancelled, 5: NoShow

        public decimal TotalPrice { get; set; }

        public string SeatNumber { get; set; }

        public bool PreCheckIn { get; set; }

        public int LuggageCount { get; set; } = 0;

        // Indica si la reservación ya fue sincronizada con el API
        public bool IsSynced { get; set; } = false;
    }
}
