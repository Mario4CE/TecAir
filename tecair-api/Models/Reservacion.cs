namespace TecAir.Api.Models;

public sealed class Reservacion
{
    public int IdReservacion { get; set; }
    public string Estado { get; set; } = "pendiente_pago";
    public DateTime FechaReservacion { get; set; }
    public int IdUsuario { get; set; }
    public int IdVuelo { get; set; }

    public Usuario? Usuario { get; set; }
    public Vuelo? Vuelo { get; set; }
    public Pago? Pago { get; set; }
}
