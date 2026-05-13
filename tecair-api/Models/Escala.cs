namespace TecAir.Api.Models;

public sealed class Escala
{
    public int IdRuta { get; set; }
    public int Orden { get; set; }
    public int IdAeropuerto { get; set; }
    public string Tipo { get; set; } = string.Empty;

    public Ruta? Ruta { get; set; }
    public Aeropuerto? Aeropuerto { get; set; }
}
