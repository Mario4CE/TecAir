namespace TecAir.Domain.Models;

/*
Descripción: Representa una ruta compuesta por una secuencia de escalas.
Entradas: IdRuta y listas de escalas, vuelos y promociones asociadas.
Salidas: Entidad persistida en la tabla ruta.
Restricciones: Una ruta queda definida por al menos dos escalas.
*/
public sealed class Ruta
{
    public int IdRuta { get; set; }

    public List<Escala> Escalas { get; set; } = [];
    public List<Vuelo> Vuelos { get; set; } = [];
    public List<Promocion> Promociones { get; set; } = [];
}
