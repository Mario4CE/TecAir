namespace TecAir.Application.Configuration;

/*
Descripción:
Define opciones configurables del API que se cargan desde appsettings y entorno.
Entradas:
Recibe valores de configuración mediante binding de ASP.NET Core.
Salidas:
Proporciona propiedades tipadas consumidas por Program y servicios.
Restricciones:
Los valores por defecto deben preservar compatibilidad en ausencia de configuración externa.
*/
public sealed class TecAirOptions
{
    public string CorsOrigin { get; set; } = "*"; // Permitir solicitudes desde cualquier origen
    public string DefaultGate { get; set; } = "A1";
    public decimal DefaultFlightPrice { get; set; } = 120m;
    public int LoyaltyMilesPerReservation { get; set; } = 100;
}
