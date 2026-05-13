namespace TecAir.Api.Models;

public sealed class Avion
{
    public string Matricula { get; set; } = string.Empty;
    public int Capacidad { get; set; }

    public List<Vuelo> Vuelos { get; set; } = [];
}
