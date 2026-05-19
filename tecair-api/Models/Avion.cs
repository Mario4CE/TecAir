namespace TecAir.Api.Models;

/*
Descripción: Representa un avión disponible para operar vuelos.
Entradas: Matricula, Capacidad y lista de vuelos asociados.
Salidas: Entidad persistida en la tabla avion.
Restricciones: La matrícula funciona como clave primaria y debe ser única.
*/
public sealed class Avion
{
    public string Matricula { get; set; } = string.Empty;
    public int Capacidad { get; set; }

    public List<Vuelo> Vuelos { get; set; } = [];
}
