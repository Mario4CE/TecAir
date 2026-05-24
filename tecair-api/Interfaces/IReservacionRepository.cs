using TecAir.Api.Dtos;
using TecAir.Api.Models;

namespace TecAir.Api.Interfaces;

/*
Descripción:
Define operaciones de persistencia para reservaciones.
Entradas:
Recibe ids, filtros y entidades de reservación.
Salidas:
Retorna DTOs y entidades para la capa de servicios.
Restricciones:
No contiene lógica de negocio compleja.
*/
public interface IReservacionRepository
{
    Task<List<ReservacionResponse>> GetReservacionesAsync(int? idUsuario);
    Task<ReservacionResponse?> GetReservacionByIdAsync(int idReservacion);
    Task<Usuario?> GetUsuarioByIdAsync(int idUsuario);
    Task<bool> ExisteVueloAsync(int idVuelo);
    Task AddAsync(Reservacion reservacion);
    Task<Reservacion?> GetEntityByIdAsync(int idReservacion);
    Task SaveChangesAsync();
}
