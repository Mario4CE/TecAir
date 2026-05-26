using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de pagos.
Entradas:
Recibe DTOs de pago y parámetros de consulta.
Salidas:
Retorna DTOs de salida de pagos.
Restricciones:
Debe validar reservación y monto antes de persistir.
*/
public interface IPagoService
{
    Task<List<PagoResponse>> GetPagosAsync();
    Task<PagoResponse> CrearPagoAsync(PagoRequest datos);
}
