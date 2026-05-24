using TecAir.Api.Dtos;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de promociones.
Entradas:
Recibe DTOs de request y parámetros de consulta.
Salidas:
Retorna DTOs de promociones para endpoints.
Restricciones:
Debe validar ruta y precio antes de crear.
*/
public interface IPromocionService
{
    Task<List<PromocionResponse>> GetPromocionesAsync();
    Task<PromocionResponse> CrearPromocionAsync(PromocionRequest datos);
}
