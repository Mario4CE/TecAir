namespace TecAir.Api.Models;

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
