using TecAir.Api.Services;

namespace TecAir.Api.Tests;

/*
Descripción:
Pruebas unitarias para validar la estrategia de cobro de maletas.
Entradas:
No recibe parámetros directos.
Salidas:
No retorna valor.
Restricciones:
No presenta restricciones adicionales.
*/
public class StrategyTests
{
    /*
    Descripción:
    Valida el cobro adicional según total de maletas.
    Entradas:
    totalMaletas y montoEsperado definidos por teoría.
    Salidas:
    No retorna valor.
    Restricciones:
    No presenta restricciones adicionales.
    */
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 50)]
    [InlineData(3, 125)]
    [InlineData(5, 275)]
    public void Calcular_DebeRetornarMontoEsperado(int totalMaletas, decimal montoEsperado)
    {
        var strategy = new CalculoCobroMaletaStrategy();
        var resultado = strategy.Calcular(totalMaletas);
        Assert.Equal(montoEsperado, resultado);
    }
}
