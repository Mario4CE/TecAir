namespace TecAir.Api.Models;

public sealed class Pago
{
    public int IdPago { get; set; }
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = "tarjeta";
    public int IdReservacion { get; set; }

    public Reservacion? Reservacion { get; set; }
}
