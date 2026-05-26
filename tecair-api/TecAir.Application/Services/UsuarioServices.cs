using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Application.Services;

/*
Descripción:
Implementa las reglas de negocio del módulo de usuarios.
Entradas:
Recibe IUsuarioRepository por inyección y DTOs de entrada para altas y edición.
Salidas:
Entrega entidades de usuario creadas, consultadas o actualizadas.
Restricciones:
Mantiene las validaciones actuales sin modificar contratos JSON públicos.
*/
public sealed class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
{
    /*
    Descripción:
    Obtiene todos los usuarios registrados.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna la colección completa de usuarios.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task<List<Usuario>> GetUsuariosAsync() => await usuarioRepository.GetAllAsync();

    /*
    Descripción:
    Recupera un usuario específico por id.
    Entradas:
    idUsuario: identificador de usuario.
    Salidas:
    Retorna el usuario encontrado o null.
    Restricciones:
    El identificador debe corresponder a un registro existente.
    */
    public async Task<Usuario?> GetUsuarioByIdAsync(int idUsuario) => await usuarioRepository.GetByIdAsync(idUsuario);

    /*
    Descripción:
    Recupera los datos de perfil de un usuario.
    Entradas:
    idUsuario: identificador del usuario autenticado.
    Salidas:
    Retorna el perfil del usuario o null.
    Restricciones:
    Usa consulta sin tracking al no requerir actualización.
    */
    public async Task<Usuario?> GetPerfilAsync(int idUsuario) => await usuarioRepository.GetByIdAsync(idUsuario);

    /*
    Descripción:
    Crea un usuario nuevo con validaciones de datos obligatorios.
    Entradas:
    datos: DTO con información del usuario.
    Salidas:
    Retorna la entidad creada con su id generado.
    Restricciones:
    Requiere correo y nombre válidos; arroja InvalidOperationException en caso contrario.
    */
    public async Task<Usuario> CrearUsuarioAsync(UsuarioRequest datos)
    {
        var correo = datos.Correo ?? datos.Email;
        if (string.IsNullOrWhiteSpace(datos.Nombre1) && string.IsNullOrWhiteSpace(datos.NombreCompleto))
            throw new InvalidOperationException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(correo))
            throw new InvalidOperationException("El correo es obligatorio.");

        var nombre = SepararNombre(datos.NombreCompleto);
        var usuario = new Usuario
        {
            Nombre1 = datos.Nombre1 ?? nombre.Nombre1,
            Nombre2 = datos.Nombre2 ?? nombre.Nombre2,
            Apellido1 = datos.Apellido1 ?? nombre.Apellido1,
            Apellido2 = datos.Apellido2 ?? nombre.Apellido2,
            Telefono = datos.Telefono ?? string.Empty,
            Correo = correo,
            EsEstudiante = datos.EsEstudiante ?? false,
            Universidad = datos.Universidad ?? string.Empty,
            Carnet = datos.Carnet ?? string.Empty,
            Millas = datos.Millas ?? 0,
            EsAdmin = datos.EsAdmin ?? false
        };

        await usuarioRepository.AddAsync(usuario);
        await usuarioRepository.SaveChangesAsync();
        return usuario;
    }

    /*
    Descripción:
    Actualiza el perfil de un usuario existente con datos parciales.
    Entradas:
    idUsuario: identificador del usuario objetivo.
    datos: DTO con campos editables.
    Salidas:
    Retorna el usuario actualizado o null si no existe.
    Restricciones:
    Solo reemplaza campos con valores presentes en el request.
    */
    public async Task<Usuario?> ActualizarPerfilAsync(int idUsuario, UsuarioRequest datos)
    {
        var usuario = await usuarioRepository.GetByIdAsync(idUsuario, track: true);
        if (usuario is null) return null;

        var nombre = SepararNombre(datos.NombreCompleto);
        usuario.Nombre1 = datos.Nombre1 ?? (string.IsNullOrWhiteSpace(nombre.Nombre1) ? usuario.Nombre1 : nombre.Nombre1);
        usuario.Nombre2 = datos.Nombre2 ?? (string.IsNullOrWhiteSpace(nombre.Nombre2) ? usuario.Nombre2 : nombre.Nombre2);
        usuario.Apellido1 = datos.Apellido1 ?? (string.IsNullOrWhiteSpace(nombre.Apellido1) ? usuario.Apellido1 : nombre.Apellido1);
        usuario.Apellido2 = datos.Apellido2 ?? (string.IsNullOrWhiteSpace(nombre.Apellido2) ? usuario.Apellido2 : nombre.Apellido2);
        usuario.Telefono = datos.Telefono ?? usuario.Telefono;
        usuario.Correo = datos.Correo ?? datos.Email ?? usuario.Correo;
        usuario.EsEstudiante = datos.EsEstudiante ?? usuario.EsEstudiante;
        usuario.Universidad = datos.Universidad ?? usuario.Universidad;
        usuario.Carnet = datos.Carnet ?? usuario.Carnet;
        usuario.Millas = datos.Millas ?? usuario.Millas;
        usuario.EsAdmin = datos.EsAdmin ?? usuario.EsAdmin;

        await usuarioRepository.SaveChangesAsync();
        return usuario;
    }

    /*
    Descripción:
    Separa una cadena de nombre completo en componentes individuales.
    Entradas:
    nombreCompleto: texto opcional con nombres y apellidos.
    Salidas:
    Retorna una tupla con nombre1, nombre2, apellido1 y apellido2.
    Restricciones:
    Si faltan segmentos, completa con cadenas vacías.
    */
    private static (string Nombre1, string Nombre2, string Apellido1, string Apellido2) SepararNombre(string? nombreCompleto)
    {
        var partes = (nombreCompleto ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (
            partes.ElementAtOrDefault(0) ?? string.Empty,
            partes.ElementAtOrDefault(1) ?? string.Empty,
            partes.ElementAtOrDefault(2) ?? string.Empty,
            string.Join(' ', partes.Skip(3)));
    }
}
