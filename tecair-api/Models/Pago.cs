namespace TecAir.Api.Models;

/*
Descripción: Representa el pago asociado a una reservación.
Entradas: IdPago, Monto, Metodo e IdReservacion.
Salidas: Entidad persistida en la tabla pago.
Restricciones: Solo puede existir un pago por reservación.
*/
public sealed class Pago
{
    public int IdPago { get; set; }
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = "tarjeta";
    public int IdReservacion { get; set; }

    public Reservacion? Reservacion { get; set; }
}
