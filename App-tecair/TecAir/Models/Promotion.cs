using SQLite;

namespace TecAir.Models
{
    /// <summary>
    /// Modelo de Promoción para la base de datos local SQLite
    /// </summary>
    [Table("Promotions")]
    public class Promotion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int OriginAirportId { get; set; }

        [NotNull]
        public int DestinationAirportId { get; set; }

        [NotNull]
        public decimal PromotionalPrice { get; set; }

        [NotNull]
        public DateTime StartDate { get; set; }

        [NotNull]
        public DateTime EndDate { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        // ID de la promoción en el API remoto
        public int ApiId { get; set; } = 0;
    }
}
