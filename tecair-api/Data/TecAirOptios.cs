namespace TecAir.Api.Data;

public sealed class TecAirOptions
{
    public string CorsOrigin { get; set; } = "*"; // Permitir solicitudes desde cualquier origen
    public string DefaultGate { get; set; } = "A1";
    public decimal DefaultFlightPrice { get; set; } = 120m;
    public int LoyaltyMilesPerReservation { get; set; } = 100;
}
