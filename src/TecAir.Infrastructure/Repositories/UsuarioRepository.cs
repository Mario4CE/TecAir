using Microsoft.EntityFrameworkCore;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Repositories;

/*
Descripción:
Implementa el acceso a datos de usuarios usando Entity Framework Core.
Entradas:
Recibe TecAirDb por inyección de dependencias y parámetros de consulta.
Salidas:
Retorna entidades Usuario y persiste cambios solicitados.
Restricciones:
No aplica reglas de negocio ni transforma contratos de API.
*/
public sealed class UsuarioRepository(TecAirDb db) : IUsuarioRepository
{
    /*
    Descripción:
    Obtiene todos los usuarios en orden ascendente por id.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna una lista de entidades Usuario.
    Restricciones:
    Ejecuta lectura sin tracking.
    */
    public async Task<List<Usuario>> GetAllAsync() => await db.Usuarios.AsNoTracking().OrderBy(x => x.IdUsuario).ToListAsync();

    /*
    Descripción:
    Busca un usuario por su identificador.
    Entradas:
    idUsuario: identificador del usuario.
    track: define si la consulta requiere seguimiento de cambios.
    Salidas:
    Retorna el usuario encontrado o null.
    Restricciones:
    Cuando track es false utiliza lectura sin tracking.
    */
    public async Task<Usuario?> GetByIdAsync(int idUsuario, bool track = false)
    {
        var query = track ? db.Usuarios : db.Usuarios.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
    }

    /*
    Descripción:
    Agrega un usuario al contexto de persistencia.
    Entradas:
    usuario: entidad a registrar.
    Salidas:
    No retorna valor.
    Restricciones:
    Requiere guardado posterior para persistir.
    */
    public async Task AddAsync(Usuario usuario) => await db.Usuarios.AddAsync(usuario);

    /*
    Descripción:
    Persiste en base de datos los cambios pendientes del contexto.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    No retorna valor.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
