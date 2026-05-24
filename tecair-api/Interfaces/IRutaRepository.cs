using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define el acceso a datos del módulo de rutas y escalas.
Entradas:
Recibe entidades Ruta/Escala y parámetros de consulta.
Salidas:
Retorna entidades y estructuras de consulta para rutas.
Restricciones:
No aplica reglas de negocio de la capa de servicios.
*/
public interface IRutaRepository
{
    Task<List<RutaResponse>> GetRutasDetalladasAsync();
    Task<RutaResponse> GetRutaDetalladaAsync(int idRuta);
    Task AddRutaAsync(Ruta ruta);
    Task AddEscalaAsync(Escala escala);
    Task SaveChangesAsync();
}
