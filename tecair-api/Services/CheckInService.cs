using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

public sealed class CheckInService(ICheckInRepository checkInRepository, IMaletaRepository maletaRepository, ICalculoCobroMaletaStrategy strategy) : ICheckInService
{
    public async Task<List<CheckInResponse>> GetCheckInsAsync() => await checkInRepository.GetCheckInsAsync();
    public async Task<CheckInResponse> CrearCheckInAsync(CheckInRequest datos)
    {
        var idUsuario = datos.IdUsuario ?? datos.UsuarioId ?? throw new InvalidOperationException("El usuario es obligatorio.");
        var idVuelo = datos.IdVuelo ?? datos.VueloId ?? throw new InvalidOperationException("El vuelo es obligatorio.");
        if (string.IsNullOrWhiteSpace(datos.Asiento)) throw new InvalidOperationException("El asiento es obligatorio.");
        if (!await checkInRepository.ExisteUsuarioAsync(idUsuario)) throw new KeyNotFoundException("Usuario no encontrado.");
        if (!await checkInRepository.ExisteVueloAsync(idVuelo)) throw new KeyNotFoundException("Vuelo no encontrado.");
        if (await checkInRepository.ExisteAsientoAsync(idVuelo, datos.Asiento)) throw new ApplicationException("El asiento ya está ocupado para este vuelo.");
        var c = new CheckIn { IdUsuario = idUsuario, IdVuelo = idVuelo, Asiento = datos.Asiento };
        await checkInRepository.AddAsync(c); await checkInRepository.SaveChangesAsync();
        return new CheckInResponse(c.IdCheckin, c.IdUsuario, c.IdVuelo, c.Asiento);
    }
    public async Task<PaseAbordarResponse?> GetPaseAbordarAsync(int idCheckIn)
    {
        var basePase = await checkInRepository.GetPaseAbordarAsync(idCheckIn);
        if (basePase is null) return null;
        var total = await maletaRepository.ContarMaletasPorCheckInAsync(idCheckIn);
        var costo = strategy.Calcular(total);
        return basePase with { TotalMaletas = total, CostoExtraMaletas = costo };
    }
}
