using TecAir.Api.Dtos;

namespace TecAir.Api.Interfaces;

public interface IMaletaService
{
    Task<List<MaletaResponse>> GetMaletasAsync();
    Task<MaletaRegistroResponse> CrearMaletaAsync(MaletaRequest datos);
}
