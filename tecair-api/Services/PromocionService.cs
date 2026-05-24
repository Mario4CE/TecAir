using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

public sealed class PromocionService(IPromocionRepository promocionRepository) : IPromocionService
{
    public async Task<List<PromocionResponse>> GetPromocionesAsync() => await promocionRepository.GetPromocionesAsync();
    public async Task<PromocionResponse> CrearPromocionAsync(PromocionRequest datos)
    {
        if (datos.IdRuta <= 0) throw new InvalidOperationException("La ruta es obligatoria.");
        if (datos.Precio <= 0) throw new InvalidOperationException("El precio promocional debe ser mayor a cero.");
        var p = new Promocion { IdRuta = datos.IdRuta, Precio = datos.Precio, FechaInicio = datos.FechaInicio, FechaFin = datos.FechaFin, Imagen = datos.Imagen ?? string.Empty };
        await promocionRepository.AddAsync(p);
        await promocionRepository.SaveChangesAsync();
        return (await promocionRepository.GetPromocionesAsync()).First(x => x.IdPromocion == p.IdPromocion);
    }
}
