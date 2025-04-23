using System;
using Xunit;

namespace PIBImpact.Tests
{
    public class TariffImpactCalculatorTests
    {
        [Fact]
        public void TariffImpactCalculatorTest()
        {
            double pibOriginal = 1000;
            double tasaArancel = 1000;

            double pibAjustado = TariffImpactCalculator.CalcularPibAjustado(pibOriginal, tasaArancel);

            double pibAjustadoEsperado = pibOriginal * Math.Exp(-1.5 * tasaArancel);
            Assert.Equal(pibAjustadoEsperado, pibAjustado, 2);


        }

        
    }

}