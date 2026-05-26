using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de vuelos.
Entradas:
Recibe parámetros de consulta y DTOs de request.
Salidas:
Retorna DTOs de salida para endpoints de vuelos.
Restricciones:
Debe preservar compatibilidad de rutas y respuestas públicas.
*/
public interface IVueloService
{
    Task<List<VueloResponse>> GetVuelosAsync(string? origen, string? destino);
    Task<VueloResponse?> GetVueloByIdAsync(int idVuelo);
    Task<VueloResponse> CrearVueloAsync(VueloRequest datos);
    Task<VueloResponse?> CambiarEstadoAsync(int idVuelo, string estado);
}
