using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define operaciones de persistencia para promociones.
Entradas:
Recibe entidades Promocion y parámetros de consulta.
Salidas:
Retorna DTOs o entidades para la capa de servicios.
Restricciones:
No contiene reglas de negocio complejas.
*/
public interface IPromocionRepository
{
    Task<List<PromocionResponse>> GetPromocionesAsync();
    Task AddAsync(Promocion promocion);
    Task SaveChangesAsync();
}
