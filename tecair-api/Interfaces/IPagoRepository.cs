using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define operaciones de persistencia para pagos.
Entradas:
Recibe ids y entidades de pago/reservación.
Salidas:
Retorna DTOs y entidades para la capa de servicios.
Restricciones:
No aplica reglas de negocio de validación.
*/
public interface IPagoRepository
{
    Task<List<PagoResponse>> GetPagosAsync();
    Task<Reservacion?> GetReservacionByIdAsync(int idReservacion);
    Task AddAsync(Pago pago);
    Task SaveChangesAsync();
}
