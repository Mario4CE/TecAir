using TecAir.Api.Interfaces;

namespace TecAir.Api.Services;

public sealed class CalculoCobroMaletaStrategy : ICalculoCobroMaletaStrategy
{
    public decimal Calcular(int totalMaletas)
    {
        if (totalMaletas <= 1) return 0;
        return 50 + Math.Max(totalMaletas - 2, 0) * 75;
    }
}
