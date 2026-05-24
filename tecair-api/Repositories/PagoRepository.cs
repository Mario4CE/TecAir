using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

public sealed class PagoRepository(TecAirDb db) : IPagoRepository
{
    public async Task<List<PagoResponse>> GetPagosAsync() =>
        await db.Pagos.AsNoTracking().OrderByDescending(x => x.IdPago)
            .Select(x => new PagoResponse(x.IdPago, x.IdReservacion, x.Monto, x.Metodo))
            .ToListAsync();

    public async Task<Reservacion?> GetReservacionByIdAsync(int idReservacion) =>
        await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);

    public async Task AddAsync(Pago pago) => await db.Pagos.AddAsync(pago);
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
