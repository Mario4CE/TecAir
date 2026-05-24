using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define el contrato de acceso a datos para aeropuertos.
Entradas:
Recibe parámetros de búsqueda y entidades Aeropuerto.
Salidas:
Retorna colecciones o entidades de aeropuerto.
Restricciones:
No aplica reglas de negocio.
*/
public interface IAeropuertoRepository
{
    Task<List<Aeropuerto>> GetAllAsync();
    Task AddAsync(Aeropuerto aeropuerto);
    Task SaveChangesAsync();
}
