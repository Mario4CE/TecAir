using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

public sealed class PagoService(IPagoRepository pagoRepository) : IPagoService
{
    public async Task<List<PagoResponse>> GetPagosAsync() => await pagoRepository.GetPagosAsync();

    public async Task<PagoResponse> CrearPagoAsync(PagoRequest datos)
    {
        var idReservacion = datos.IdReservacion ?? datos.ReservacionId ?? throw new InvalidOperationException("La reservación es obligatoria.");
        if (datos.Monto <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");

        var reservacion = await pagoRepository.GetReservacionByIdAsync(idReservacion) ?? throw new KeyNotFoundException("Reservación no encontrada.");

        var pago = new Pago { IdReservacion = idReservacion, Monto = datos.Monto, Metodo = datos.Metodo ?? "tarjeta" };
        reservacion.Estado = "pagada";
        await pagoRepository.AddAsync(pago);
        await pagoRepository.SaveChangesAsync();
        return new PagoResponse(pago.IdPago, pago.IdReservacion, pago.Monto, pago.Metodo);
    }
}
