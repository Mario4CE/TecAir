using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

/*
Descripción:
Implementa acceso a datos para aviones con Entity Framework.
Entradas:
Recibe TecAirDb y entidades Avion.
Salidas:
Retorna datos persistidos de aviones.
Restricciones:
No aplica validaciones de negocio.
*/
public sealed class AvionRepository(TecAirDb db) : IAvionRepository
{
    /*
    Descripción:
    Obtiene todos los aviones ordenados por matrícula.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna lista de aviones.
    Restricciones:
    Ejecuta lectura sin tracking.
    */
    public async Task<List<Avion>> GetAllAsync() => await db.Aviones.AsNoTracking().OrderBy(x => x.Matricula).ToListAsync();

    /*
    Descripción:
    Agrega un avión al contexto.
    Entradas:
    Recibe la entidad Avion a persistir.
    Salidas:
    No retorna valor.
    Restricciones:
    Requiere SaveChangesAsync para confirmar.
    */
    public async Task AddAsync(Avion avion) => await db.Aviones.AddAsync(avion);

    /*
    Descripción:
    Persiste cambios pendientes en base de datos.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    No retorna valor.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
