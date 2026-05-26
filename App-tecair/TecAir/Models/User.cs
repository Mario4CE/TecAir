using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Usuario para la base de datos local SQLite
    /// </summary>
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string FullName { get; set; }

        [NotNull, Unique]
        public string Email { get; set; }

        [NotNull]
        public string Password { get; set; }

        [NotNull]
        public string Phone { get; set; }

        public bool IsStudent { get; set; }

        public string University { get; set; }

        public string StudentID { get; set; }

        public decimal LoyaltyMiles { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Role { get; set; } = 0; // 0: Customer, 1: AirportStaff, 2: Admin
    }
}
