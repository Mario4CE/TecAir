namespace TecAir.Api.Models;

public sealed class Maleta
{
    public string NumMaleta { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public string Color { get; set; } = string.Empty;
    public int IdCheckin { get; set; }

    public CheckIn? CheckIn { get; set; }
}
