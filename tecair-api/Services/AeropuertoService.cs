using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

/*
Descripción:
Implementa la lógica de negocio para aeropuertos.
Entradas:
Recibe IAeropuertoRepository y DTOs de solicitud.
Salidas:
Retorna entidades Aeropuerto válidas para respuestas.
Restricciones:
No depende de detalles HTTP.
*/
public sealed class AeropuertoService(IAeropuertoRepository aeropuertoRepository) : IAeropuertoService
{
    /*
    Descripción:
    Lista todos los aeropuertos disponibles.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna una lista de aeropuertos.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task<List<Aeropuerto>> GetAeropuertosAsync() => await aeropuertoRepository.GetAllAsync();

    /*
    Descripción:
    Crea un nuevo aeropuerto aplicando validaciones básicas.
    Entradas:
    Recibe AeropuertoRequest con nombre y ubicación.
    Salidas:
    Retorna la entidad Aeropuerto creada.
    Restricciones:
    El nombre es obligatorio; lanza InvalidOperationException si falta.
    */
    public async Task<Aeropuerto> CrearAeropuertoAsync(AeropuertoRequest datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre))
        {
            throw new InvalidOperationException("El nombre del aeropuerto es obligatorio.");
        }

        var aeropuerto = new Aeropuerto
        {
            Nombre = datos.Nombre,
            Ubicacion = datos.Ubicacion ?? string.Empty
        };

        await aeropuertoRepository.AddAsync(aeropuerto);
        await aeropuertoRepository.SaveChangesAsync();
        return aeropuerto;
    }
}
