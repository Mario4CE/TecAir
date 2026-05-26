using TecAir.Domain.Models;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define el contrato de persistencia para aviones.
Entradas:
Recibe entidades Avion y parámetros de consulta.
Salidas:
Retorna listas de aviones para consumo de servicios.
Restricciones:
No incluye reglas de negocio.
*/
public interface IAvionRepository
{
    Task<List<Avion>> GetAllAsync();
    Task AddAsync(Avion avion);
    Task SaveChangesAsync();
}
