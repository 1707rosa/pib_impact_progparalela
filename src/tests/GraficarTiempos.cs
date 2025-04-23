using Xunit;
using ZedGraph;
using PIBImpact.Analysis;

namespace PIBImpact.Tests
{
    public class GraficadorTests
    {
        [Fact]
        public void GraficarCorrectamenteTiempos()
        {
            long tiempoSecuencial = 500;
            long tiempoParalelo = 300;

            ZedGraphControl zgc = new ZedGraphControl();

            Graficador.GraficarTiempos(tiempoSecuencial, tiempoParalelo, zgc);

            var barras = zgc.GraphPane.CurveList;
            Assert.Single(barras); 
            var barra = barras[0] as BarItem;
            Assert.NotNull(barra);
            Assert.Equal(tiempoSecuencial, barra.Points[0].Y);
            Assert.Equal(tiempoParalelo, barra.Points[1].Y);
        }
    }
}
