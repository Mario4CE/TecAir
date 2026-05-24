using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define la lógica de negocio para operaciones de usuarios del API.
Entradas:
Requests DTOs y parámetros de identificación de usuario.
Salidas:
Retorna entidades de usuario procesadas por reglas de negocio.
Restricciones:
Debe preservar compatibilidad de validaciones y campos públicos existentes.
*/
public interface IUsuarioService
{
    Task<List<Usuario>> GetUsuariosAsync();
    Task<Usuario?> GetUsuarioByIdAsync(int idUsuario);
    Task<Usuario?> GetPerfilAsync(int idUsuario);
    Task<Usuario> CrearUsuarioAsync(UsuarioRequest datos);
    Task<Usuario?> ActualizarPerfilAsync(int idUsuario, UsuarioRequest datos);
}
