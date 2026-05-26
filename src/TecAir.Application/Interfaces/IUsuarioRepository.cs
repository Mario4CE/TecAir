using TecAir.Domain.Models;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define las operaciones de persistencia para la entidad Usuario.
Entradas:
Parámetros de búsqueda o entidades Usuario según la operación.
Salidas:
Retorna usuarios, colecciones de usuarios o confirmación implícita al guardar cambios.
Restricciones:
No contiene reglas de negocio; solo acceso a datos.
*/
public interface IUsuarioRepository
{
    /*
    Descripción:
    Obtiene todos los usuarios ordenados por identificador.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna la lista de usuarios persistidos.
    Restricciones:
    Devuelve la colección vacía si no existen registros.
    */
    Task<List<Usuario>> GetAllAsync();

    /*
    Descripción:
    Busca un usuario por su identificador.
    Entradas:
    idUsuario: identificador único del usuario.
    Salidas:
    Retorna el usuario encontrado o null.
    Restricciones:
    El identificador debe ser mayor a cero para resultados válidos.
    */
    Task<Usuario?> GetByIdAsync(int idUsuario, bool track = false);

    /*
    Descripción:
    Agrega un nuevo usuario a la unidad de trabajo.
    Entradas:
    usuario: entidad usuario a persistir.
    Salidas:
    No retorna valor.
    Restricciones:
    Requiere ejecutar SaveChangesAsync para confirmar en base de datos.
    */
    Task AddAsync(Usuario usuario);

    /*
    Descripción:
    Confirma los cambios pendientes en la base de datos.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    No retorna valor.
    Restricciones:
    Propaga excepciones de persistencia al llamador.
    */
    Task SaveChangesAsync();
}
