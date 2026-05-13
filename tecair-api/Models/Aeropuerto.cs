namespace TecAir.Api.Models;

public sealed class Aeropuerto
{
    public int IdAeropuerto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;

    public List<Escala> Escalas { get; set; } = [];
}
