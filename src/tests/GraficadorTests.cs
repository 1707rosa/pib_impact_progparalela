using System.IO;
using Xunit;
using ZedGraph;
using PIBImpact.Analysis;
using System.Collections.Generic;

public class GraficadorTests
{
    [Fact]
    public void GraficarProyeccionPIB()
    {
        var resultados = new List<ResultadoSimulacion>
        {
            new ResultadoSimulacion { Proyeccion5Annos = new List<double> { 1000, 1100, 1200, 1300, 1400 } }
        };

        var zedGraphControl = new ZedGraphControl();

        string tempDir = Path.Combine(Path.GetTempPath(), "grafico_prueba");
        string rutaArchivo = Path.Combine(tempDir, "pib_prueba.png");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true); // Limpiar antes

        // Act
        Graficador.GraficarProyeccionPIB(resultados, zedGraphControl, rutaArchivo);

        // Assert
        Assert.True(File.Exists(rutaArchivo), "La imagen no fue guardada.");
    }
}
