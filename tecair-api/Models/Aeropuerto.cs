namespace TecAir.Api.Models;

/*
Descripción: Representa un aeropuerto disponible para rutas y vuelos.
Entradas: IdAeropuerto, Nombre, Ubicacion y Escalas asociadas.
Salidas: Entidad persistida en la tabla aeropuerto.
Restricciones: El nombre no puede estar vacío.
*/
public sealed class Aeropuerto
{
    public int IdAeropuerto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;

    public List<Escala> Escalas { get; set; } = [];
}
