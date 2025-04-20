using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;

namespace PIBImpact.analysis
{
    public static class Graficador
    {
        public static void GraficarTiempos(long tiempoSecuencial, long tiempoParalelo)
        {
            double[] valores = { tiempoSecuencial, tiempoParalelo };
            string[] etiquetas = { "Secuencial", "Paralela" };

            var plt = new ScottPlot.Plot(600, 400);
            var barras = plt.AddBar(valores);

            plt.XTicks(etiquetas);
            plt.Title("Comparacion de Tiempos de Ejecución");
            plt.YLabel("Tiempo (ms)");
            plt.SetAxisLimits(yMin: 0);

            // Ruta relativa a la raíz del proyecto
            string rutaArchivo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "metrics", "grafica_tiempos.png");

            // Crear directorio si no existe
            string? directorio = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);

            plt.SaveFig(rutaArchivo);
            Console.WriteLine($" Grafica de tiempos guardada en: {Path.GetFullPath(rutaArchivo)}");
        }
         public static void GraficarImpactoPIBGlobal(Dictionary<string, double> impactoPorSector)
 {
     string[] sectores = new string[impactoPorSector.Count];
     double[] valores = new double[impactoPorSector.Count];
     int i = 0;

     foreach (var kvp in impactoPorSector)
     {
         sectores[i] = kvp.Key;
         valores[i] = kvp.Value;
         i++;
     }

     var plt = new ScottPlot.Plot(1000, 800); // más alto por ser barras horizontales

     var barras = plt.AddBar(valores);
     barras.Orientation = ScottPlot.Orientation.Horizontal;
     barras.BarWidth = 0.6;

     // Etiquetas del eje Y (porque es horizontal)
     plt.YTicks(Enumerable.Range(0, sectores.Length).Select(x => (double)x).ToArray(), sectores);
     plt.YAxis.TickLabelStyle(fontSize: 10); // Ajusta tamaño si son muchos

     plt.Title("Impacto del PIB por Sector");
     plt.XLabel("PIB Afectado");
     plt.SetAxisLimits(xMin: 0); // ahora el eje X es el de valores

     // Ruta de guardado
     string rutaArchivo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "metrics", "grafica_pib_global.png");

     string? directorio = Path.GetDirectoryName(rutaArchivo);
     if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
         Directory.CreateDirectory(directorio);

     plt.SaveFig(rutaArchivo);
     Console.WriteLine($" Grafica de impacto global por sector guardada en: {Path.GetFullPath(rutaArchivo)}");
 }
        public static void GraficarImpactoPIBPorPais(Dictionary<string, Dictionary<string, double>> impactoPorPais)
{
    // Ruta base de la carpeta "por_pais"
    string rutaBase = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "metrics", "por_pais");

    // Asegurarse de que la carpeta exista
    if (!Directory.Exists(rutaBase))
        Directory.CreateDirectory(rutaBase);

    foreach (var paisKvp in impactoPorPais)
    {
        string pais = paisKvp.Key.Trim();
        Dictionary<string, double> impactoPorSector = paisKvp.Value;

        // Validar datos
        if (impactoPorSector.Count == 0)
            continue;

        string[] sectores = new string[impactoPorSector.Count];
        double[] valores = new double[impactoPorSector.Count];
        int i = 0;

        foreach (var kvp in impactoPorSector)
        {
            sectores[i] = kvp.Key;
            valores[i] = kvp.Value;
            i++;
        }

        var plt = new ScottPlot.Plot(800, 500);
        plt.AddBar(valores);
        plt.XTicks(sectores);
        plt.Title($"Impacto del PIB por Sector - {pais}");
        plt.YLabel("PIB Afectado");
        plt.SetAxisLimits(yMin: 0);

        // Limpiar nombre del archivo
        string nombreLimpio = string.Concat(pais.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Replace(" ", "_");

        string rutaArchivo = Path.Combine(rutaBase, $"grafica_pib_{nombreLimpio}.png");

        try
        {
            plt.SaveFig(rutaArchivo);
            Console.WriteLine($"Grafica guardada para {pais} en: {Path.GetFullPath(rutaArchivo)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error al guardar la grafica para {pais}: {ex.Message}");
        }

        
            
        }

        }
    }
}