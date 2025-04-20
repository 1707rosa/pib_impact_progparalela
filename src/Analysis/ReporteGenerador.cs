using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using PIBImpact.Models;

namespace PIBImpact.analysis
{
    public static class ReporteGenerador
    {
        public static void GenerarReporteCsv(List<ResultadoSimulacion> resultados, string? rutaArchivo = null, int topSectores = 5)
        {
            // Definir ruta segura relativa a la raíz del proyecto
            if (string.IsNullOrEmpty(rutaArchivo))
            {
                rutaArchivo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "metrics", "reporte_global.csv");
            }

            // Asegurar que el directorio exista
            string? directorio = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);

            // Eliminar archivo si ya existe
            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            // Calcular metricas globales
            double pibTotal = ResultAnalyzer.CalcularPibTotalAfectado(resultados);
            var sectoresImpactados = ResultAnalyzer.ObtenerSectoresMasImpactados(resultados, topSectores);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (var writer = new StreamWriter(rutaArchivo, false))
            using (var csv = new CsvWriter(writer, config))
            {
                // Sección global
                csv.WriteField("=====");
                csv.WriteField("Resultado Global");
                csv.NextRecord();

                csv.WriteField("PIB Total Afectado");
                csv.WriteField(pibTotal);
                csv.NextRecord();

                csv.WriteField("Top Sectores Mas Impactados (Global)");
                csv.NextRecord();

                foreach (var sector in sectoresImpactados)
                {
                    csv.WriteField(sector.Sector);
                    csv.WriteField(sector.ImpactoTotal);
                    csv.NextRecord();
                }

                csv.NextRecord();
                csv.WriteField("=====");
                csv.WriteField("Resultados por Pais");
                csv.NextRecord();

                // Agrupar por pais
                var resultadosPorPais = resultados
                    .GroupBy(r => r.Pais)
                    .OrderBy(g => g.Key);

                foreach (var grupoPais in resultadosPorPais)
                {
                    string pais = grupoPais.Key;
                    double pibPais = grupoPais.Sum(x => x.CambioPib);
                    var sectoresPais = grupoPais
                        .GroupBy(r => r.Sector)
                        .Select(g => new
                        {
                            Sector = g.Key,
                            ImpactoTotal = g.Sum(x => x.CambioPib)
                        })
                        .OrderByDescending(s => Math.Abs(s.ImpactoTotal))
                        .Take(topSectores);

                    csv.WriteField($"Pais: {pais}");
                    csv.NextRecord();

                    csv.WriteField("PIB Total Afectado");
                    csv.WriteField(pibPais);
                    csv.NextRecord();

                    csv.WriteField("Top Sectores Mas Impactados");
                    csv.NextRecord();

                    foreach (var s in sectoresPais)
                    {
                        csv.WriteField(s.Sector);
                        csv.WriteField(s.ImpactoTotal);
                        csv.NextRecord();
                    }

                    csv.NextRecord();
                }
            }

            Console.WriteLine($" Reporte generado en: {rutaArchivo}");
        }
    }
}
