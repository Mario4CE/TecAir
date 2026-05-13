namespace TecAir.Api.Models;

public sealed class Ruta
{
    public int IdRuta { get; set; }

    public List<Escala> Escalas { get; set; } = [];
    public List<Vuelo> Vuelos { get; set; } = [];
    public List<Promocion> Promociones { get; set; } = [];
}
