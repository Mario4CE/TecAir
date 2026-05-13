namespace TecAir.Api.Models;

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
