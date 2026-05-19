namespace TecAir.Api.Models;

/*
Descripción: Representa una maleta registrada durante el check-in.
Entradas: NumMaleta, Peso, Color e IdCheckin.
Salidas: Entidad persistida en la tabla maleta.
Restricciones: La maleta debe estar asociada a un check-in existente.
*/
public sealed class Maleta
{
    public string NumMaleta { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public string Color { get; set; } = string.Empty;
    public int IdCheckin { get; set; }

    public CheckIn? CheckIn { get; set; }
}
