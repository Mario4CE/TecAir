using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de aviones.
Entradas:
Recibe DTOs de aviones y parámetros operativos.
Salidas:
Retorna entidades Avion validadas.
Restricciones:
Debe validar matrícula y capacidad antes de guardar.
*/
public interface IAvionService
{
    Task<List<Avion>> GetAvionesAsync();
    Task<Avion> CrearAvionAsync(AvionRequest datos);
}
