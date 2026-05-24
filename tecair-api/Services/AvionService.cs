using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

/*
Descripción:
Implementa reglas de negocio para la gestión de aviones.
Entradas:
Recibe IAvionRepository y DTOs de entrada.
Salidas:
Retorna entidades Avion validadas para respuesta.
Restricciones:
No depende de detalles HTTP.
*/
public sealed class AvionService(IAvionRepository avionRepository) : IAvionService
{
    /*
    Descripción:
    Lista todos los aviones registrados.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna una lista de aviones.
    Restricciones:
    No presenta restricciones adicionales.
    */
    public async Task<List<Avion>> GetAvionesAsync() => await avionRepository.GetAllAsync();

    /*
    Descripción:
    Crea un avión aplicando validaciones básicas.
    Entradas:
    Recibe AvionRequest con matrícula y capacidad.
    Salidas:
    Retorna el avión creado.
    Restricciones:
    Matrícula obligatoria y capacidad mayor a cero.
    */
    public async Task<Avion> CrearAvionAsync(AvionRequest datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Matricula))
            throw new InvalidOperationException("La matrícula es obligatoria.");

        if (datos.Capacidad <= 0)
            throw new InvalidOperationException("La capacidad debe ser mayor a cero.");

        var avion = new Avion
        {
            Matricula = datos.Matricula,
            Capacidad = datos.Capacidad
        };

        await avionRepository.AddAsync(avion);
        await avionRepository.SaveChangesAsync();
        return avion;
    }
}
