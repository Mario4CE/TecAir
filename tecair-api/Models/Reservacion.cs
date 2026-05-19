namespace TecAir.Api.Models;

/*
Descripción: Representa la reserva de un usuario sobre un vuelo.
Entradas: IdReservacion, Estado, FechaReservacion, IdUsuario e IdVuelo.
Salidas: Entidad persistida en la tabla reservacion.
Restricciones: Debe estar asociada a un usuario y a un vuelo existentes.
*/
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
