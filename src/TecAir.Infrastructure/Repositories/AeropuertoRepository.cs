using Microsoft.EntityFrameworkCore;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Repositories;

/*
Descripción:
Implementa operaciones de persistencia para aeropuertos.
Entradas:
Recibe TecAirDb y entidades Aeropuerto.
Salidas:
Retorna datos persistidos de aeropuertos.
Restricciones:
No implementa reglas de negocio.
*/
public sealed class AeropuertoRepository(TecAirDb db) : IAeropuertoRepository
{
    /*
    Descripción:
    Obtiene todos los aeropuertos ordenados por nombre.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna una lista de aeropuertos.
    Restricciones:
    Ejecuta lectura sin tracking.
    */
    public async Task<List<Aeropuerto>> GetAllAsync() => await db.Aeropuertos.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();

    /*
    Descripción:
    Agrega un aeropuerto al contexto.
    Entradas:
    Recibe una entidad Aeropuerto.
    Salidas:
    No retorna valor.
    Restricciones:
    Requiere guardado posterior.
    */
    public async Task AddAsync(Aeropuerto aeropuerto) => await db.Aeropuertos.AddAsync(aeropuerto);

    /*
    Descripción:
    Persiste cambios pendientes del contexto.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    No retorna valor.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
