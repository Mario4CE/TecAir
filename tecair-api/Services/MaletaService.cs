using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Services;

public sealed class MaletaService(IMaletaRepository maletaRepository, ICalculoCobroMaletaStrategy strategy) : IMaletaService
{
    public async Task<List<MaletaResponse>> GetMaletasAsync() => await maletaRepository.GetMaletasAsync();
    public async Task<MaletaRegistroResponse> CrearMaletaAsync(MaletaRequest datos)
    {
        if (string.IsNullOrWhiteSpace(datos.NumMaleta)) throw new InvalidOperationException("El número de maleta es obligatorio.");
        if (!await maletaRepository.ExisteCheckInAsync(datos.IdCheckin)) throw new KeyNotFoundException("Check-in no encontrado.");
        if (await maletaRepository.ExisteNumMaletaAsync(datos.NumMaleta)) throw new InvalidOperationException("El número de maleta ya existe.");
        var m = new Maleta { NumMaleta = datos.NumMaleta, Peso = datos.Peso, Color = datos.Color, IdCheckin = datos.IdCheckin };
        await maletaRepository.AddAsync(m); await maletaRepository.SaveChangesAsync();
        var total = await maletaRepository.ContarMaletasPorCheckInAsync(datos.IdCheckin);
        var maletas = await maletaRepository.GetMaletasPorCheckInAsync(datos.IdCheckin);
        return new MaletaRegistroResponse(new MaletaResponse(m.NumMaleta, m.Peso, m.Color, m.IdCheckin), new ResumenMaletaResponse(total, strategy.Calcular(total), maletas));
    }
}
