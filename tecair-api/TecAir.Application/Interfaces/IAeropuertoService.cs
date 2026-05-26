using TecAir.Contracts.Dtos;
using TecAir.Domain.Models;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de aeropuertos.
Entradas:
Recibe DTOs de request y parámetros de consulta.
Salidas:
Retorna entidades de aeropuerto procesadas.
Restricciones:
Debe validar campos obligatorios antes de persistir.
*/
public interface IAeropuertoService
{
    Task<List<Aeropuerto>> GetAeropuertosAsync();
    Task<Aeropuerto> CrearAeropuertoAsync(AeropuertoRequest datos);
}
