using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

public sealed class PromocionRepository(TecAirDb db) : IPromocionRepository
{
    public async Task<List<PromocionResponse>> GetPromocionesAsync()
    {
        var promociones = await db.Promociones.AsNoTracking().OrderByDescending(x => x.FechaInicio).ToListAsync();
        var resultado = new List<PromocionResponse>();
        foreach (var p in promociones)
        {
            var escalas = await db.Escalas.AsNoTracking().Where(x => x.IdRuta == p.IdRuta)
                .Join(db.Aeropuertos.AsNoTracking(), e => e.IdAeropuerto, a => a.IdAeropuerto, (e, a) => new { e.Tipo, a.Nombre }).ToListAsync();
            resultado.Add(new PromocionResponse(p.IdPromocion, p.Precio, p.FechaInicio, p.FechaFin, p.Imagen, p.IdRuta,
                escalas.FirstOrDefault(x => x.Tipo == "origen")?.Nombre ?? string.Empty,
                escalas.FirstOrDefault(x => x.Tipo == "destino")?.Nombre ?? string.Empty));
        }
        return resultado;
    }

    public async Task AddAsync(Promocion promocion) => await db.Promociones.AddAsync(promocion);
    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
