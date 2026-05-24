using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define operaciones de acceso a datos para vuelos.
Entradas:
Recibe filtros, ids y entidades Vuelo.
Salidas:
Retorna DTOs de vuelo y entidades persistidas.
Restricciones:
No contiene reglas de negocio complejas.
*/
public interface IVueloRepository
{
    Task<List<VueloResponse>> GetVuelosAsync(string? origen, string? destino);
    Task<VueloResponse?> GetVueloByIdAsync(int idVuelo);
    Task AddAsync(Vuelo vuelo);
    Task<Vuelo?> GetEntityByIdAsync(int idVuelo);
    Task SaveChangesAsync();
}
