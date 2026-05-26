using TecAir.Contracts.Dtos;

namespace TecAir.Application.Interfaces;

public interface IMaletaService
{
    Task<List<MaletaResponse>> GetMaletasAsync();
    Task<MaletaRegistroResponse> CrearMaletaAsync(MaletaRequest datos);
}
