namespace TecAir.Api.Models;

/*
Descripción: Representa una promoción comercial asociada a una ruta.
Entradas: IdPromocion, Precio, FechaInicio, FechaFin, Imagen e IdRuta.
Salidas: Entidad persistida en la tabla promocion.
Restricciones: La ruta debe existir y el precio promocional debe ser válido.
*/
public sealed class Promocion
{
    public int IdPromocion { get; set; }
    public decimal Precio { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string Imagen { get; set; } = string.Empty;
    public int IdRuta { get; set; }

    public Ruta? Ruta { get; set; }
}
