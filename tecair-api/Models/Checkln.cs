namespace TecAir.Api.Models;

/*
Descripción: Representa el registro de check-in de un usuario para un vuelo.
Entradas: IdCheckin, Asiento, IdUsuario, IdVuelo y maletas asociadas.
Salidas: Entidad persistida en la tabla checkin.
Restricciones: No puede repetirse el mismo asiento para un vuelo.
*/
public sealed class CheckIn
{
    public int IdCheckin { get; set; }
    public string Asiento { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public int IdVuelo { get; set; }

    public Usuario? Usuario { get; set; }
    public Vuelo? Vuelo { get; set; }
    public List<Maleta> Maletas { get; set; } = [];
}
