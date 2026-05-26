using Microsoft.EntityFrameworkCore;
using TecAir.Application.Configuration;
using TecAir.Infrastructure.Persistence;
using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Repositories;

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
