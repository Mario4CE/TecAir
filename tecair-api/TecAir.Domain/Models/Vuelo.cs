namespace TecAir.Domain.Models;

/*
Descripción: Representa un vuelo programado sobre una ruta y un avión.
Entradas: IdVuelo, FechaSalida, HoraSalida, Puerta, Estado, Matricula, IdRuta y Precio.
Salidas: Entidad persistida en la tabla vuelo.
Restricciones: Debe apuntar a una ruta y a un avión existentes.
*/
public sealed class Vuelo
{
    public int IdVuelo { get; set; }
    public DateOnly FechaSalida { get; set; }
    public TimeOnly HoraSalida { get; set; }
    public string Puerta { get; set; } = string.Empty;
    public string Estado { get; set; } = "programado";
    public string Matricula { get; set; } = string.Empty;
    public int IdRuta { get; set; }
    public decimal Precio { get; set; }

    public Avion? Avion { get; set; }
    public Ruta? Ruta { get; set; }
    public List<Reservacion> Reservaciones { get; set; } = [];
    public List<CheckIn> CheckIns { get; set; } = [];
}
