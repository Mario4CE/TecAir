using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

/*
Descripción:
Define la lógica de negocio del módulo de reservaciones.
Entradas:
Recibe parámetros de filtro, ids y DTOs de solicitud.
Salidas:
Retorna DTOs de reservación para respuestas HTTP.
Restricciones:
Debe mantener compatibilidad con contratos públicos existentes.
*/
public interface IReservacionService
{
    Task<List<ReservacionResponse>> GetReservacionesAsync(int? idUsuario);
    Task<ReservacionResponse?> GetReservacionByIdAsync(int idReservacion);
    Task<ReservacionResponse> CrearReservacionAsync(ReservacionRequest datos);
    Task<ReservacionResponse?> CancelarReservacionAsync(int idReservacion);
}
