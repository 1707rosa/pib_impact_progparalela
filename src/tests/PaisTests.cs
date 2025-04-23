using Xunit;
using PIBImpact.Models;

namespace PIBImpact.Tests
{
    public class PaisTests
    {
        [Fact]
        public void Pais()
        {
            var nombreEsperado = "China";
            var pibesperado = 89357.98815m;

            var pais = new Pais
            {
                Nombre = nombreEsperado,
                PIB = pibesperado
            };

            Assert.Equal(nombreEsperado, pais.Nombre);
            Assert.Equal(pibesperado, pais.PIB);
        }
    }
}
