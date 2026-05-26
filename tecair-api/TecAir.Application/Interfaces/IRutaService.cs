using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define la lógica de negocio para operaciones del módulo de rutas.
Entradas:
Recibe DTOs y parámetros operativos del módulo.
Salidas:
Retorna resultados de consulta y creación de rutas.
Restricciones:
Debe validar que una ruta tenga origen y destino.
*/
public interface IRutaService
{
    Task<List<RutaResponse>> GetRutasAsync();
    Task<RutaResponse> CrearRutaAsync(RutaRequest datos);
}
