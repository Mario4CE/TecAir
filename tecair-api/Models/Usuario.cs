namespace TecAir.Api.Models;

public sealed class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre1 { get; set; } = string.Empty;
    public string Nombre2 { get; set; } = string.Empty;
    public string Apellido1 { get; set; } = string.Empty;
    public string Apellido2 { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public bool EsEstudiante { get; set; }
    public string Universidad { get; set; } = string.Empty;
    public string Carnet { get; set; } = string.Empty;
    public int Millas { get; set; }
    public bool EsAdmin { get; set; }

    public List<Reservacion> Reservaciones { get; set; } = [];
    public List<CheckIn> CheckIns { get; set; } = [];
}
