namespace TecAir.Domain.Models;

/*
Descripción: Representa una parada u origen/destino dentro de una ruta.
Entradas: IdRuta, Orden, IdAeropuerto y Tipo.
Salidas: Entidad persistida en la tabla escala.
Restricciones: Cada combinación de ruta y orden debe ser única.
*/
public sealed class Escala
{
    public int IdRuta { get; set; }
    public int Orden { get; set; }
    public int IdAeropuerto { get; set; }
    public string Tipo { get; set; } = string.Empty;

    public Ruta? Ruta { get; set; }
    public Aeropuerto? Aeropuerto { get; set; }
}
