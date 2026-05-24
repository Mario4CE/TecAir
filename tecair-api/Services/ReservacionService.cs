using Microsoft.Extensions.Options;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

public sealed class ReservacionService(IReservacionRepository reservacionRepository, IOptions<TecAirOptions> options) : IReservacionService
{
    public async Task<List<ReservacionResponse>> GetReservacionesAsync(int? idUsuario) => await reservacionRepository.GetReservacionesAsync(idUsuario);
    public async Task<ReservacionResponse?> GetReservacionByIdAsync(int idReservacion) => await reservacionRepository.GetReservacionByIdAsync(idReservacion);

    public async Task<ReservacionResponse> CrearReservacionAsync(ReservacionRequest datos)
    {
        var idUsuario = datos.IdUsuario ?? datos.UsuarioId ?? throw new InvalidOperationException("El usuario es obligatorio.");
        var idVuelo = datos.IdVuelo ?? datos.VueloId ?? throw new InvalidOperationException("El vuelo es obligatorio.");

        var usuario = await reservacionRepository.GetUsuarioByIdAsync(idUsuario) ?? throw new KeyNotFoundException("Usuario no encontrado.");
        if (!await reservacionRepository.ExisteVueloAsync(idVuelo)) throw new KeyNotFoundException("Vuelo no encontrado.");

        var reservacion = new Reservacion
        {
            Estado = datos.Estado ?? "pendiente_pago",
            FechaReservacion = DateTime.UtcNow,
            IdUsuario = idUsuario,
            IdVuelo = idVuelo
        };

        usuario.Millas += options.Value.LoyaltyMilesPerReservation;
        await reservacionRepository.AddAsync(reservacion);
        await reservacionRepository.SaveChangesAsync();
        return (await reservacionRepository.GetReservacionByIdAsync(reservacion.IdReservacion))!;
    }

    public async Task<ReservacionResponse?> CancelarReservacionAsync(int idReservacion)
    {
        var reservacion = await reservacionRepository.GetEntityByIdAsync(idReservacion);
        if (reservacion is null) return null;
        reservacion.Estado = "cancelada";
        await reservacionRepository.SaveChangesAsync();
        return await reservacionRepository.GetReservacionByIdAsync(idReservacion);
    }
}
