using System;
using Xunit;
using PIBImpact.Services;
using System.Collections.Generic;

namespace PIBImpact.Tests
{
    public class PibProjectionServiceTests
    {
        [Fact]
        public void PIB()
        {
            double pibOriginal = 1000; 
            double tasaArancel = 0.1;
            double tasaCrecimiento = 0.05; 

            List<double> resultados = PibProjectionService.ProyectarPibAjustado5Years(pibOriginal, tasaArancel, tasaCrecimiento);

            Assert.Equal(5, resultados.Count);

            double pibAjustadoEsperado = pibOriginal * (1 + tasaArancel);
            Assert.Equal(pibAjustadoEsperado, resultados[0], 2); 

            for (int i = 1; i < resultados.Count; i++)
            {
                double pibProyectadoEsperado = pibAjustadoEsperado * Math.Pow(1 + tasaCrecimiento, i);
                Assert.Equal(pibProyectadoEsperado, resultados[i], 2); 
            }
        }

        [Fact]
        public void PIBNegativo()
        {
            double pibOriginal = -1000;
            double tasaArancel = 0.1;
            double tasaCrecimiento = 0.05;

            Assert.Throws<ArgumentException>(() => PibProjectionService.ProyectarPibAjustado5Years(pibOriginal, tasaArancel, tasaCrecimiento));
        }

        [Fact]
        public void CrecimientoCero()
        {
            double pibOriginal = 1000;
            double tasaArancel = 0.1;
            double tasaCrecimiento = 0.0; 

            List<double> resultados = PibProjectionService.ProyectarPibAjustado5Years(pibOriginal, tasaArancel, tasaCrecimiento);

            double pibAjustado = pibOriginal * (1 + tasaArancel);
            foreach (var resultado in resultados)
            {
                Assert.Equal(pibAjustado, resultado);
            }
        }
        
    }
}
