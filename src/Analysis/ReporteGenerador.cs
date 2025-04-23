using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using PIBImpact.Models;

namespace PIBImpact.Analysis
{
    public static class ReporteGenerador
    {
        public static void GenerarReportePibAfectado(List<ResultadoSimulacion> resultados, string rutaArchivo = null)
        {
            // Ruta por defecto si no se especifica
            if (string.IsNullOrEmpty(rutaArchivo))
            {
                rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "pib_afectado.csv");
            }

            // Asegurar que el directorio exista
            var directorio = Path.GetDirectoryName(rutaArchivo);
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (var writer = new StreamWriter(rutaArchivo))
            using (var csv = new CsvWriter(writer, config))
            {
                // Encabezados
                csv.WriteField("País");
                csv.WriteField("PIB Original");
                csv.WriteField("PIB Ajustado");
                csv.WriteField("PIB Afectado");
                csv.WriteField("Cambio (%)");
                csv.NextRecord();

                // Datos por país
                foreach (var grupo in resultados.GroupBy(r => r.Pais))
                {
                    var primerResultado = grupo.First();
                    double pibAfectado = primerResultado.PIBOriginal - primerResultado.PIBAjustado;

                    csv.WriteField(grupo.Key); // País
                    csv.WriteField(primerResultado.PIBOriginal.ToString("N2"));
                    csv.WriteField(primerResultado.PIBAjustado.ToString("N2"));
                    csv.WriteField(pibAfectado.ToString("N2"));
                    csv.WriteField(primerResultado.CambioPib.ToString("N2") + "%");
                    csv.NextRecord();
                }

                // Total global
                double totalOriginal = resultados.Sum(r => r.PIBOriginal);
                double totalAjustado = resultados.Sum(r => r.PIBAjustado);
                double totalAfectado = totalOriginal - totalAjustado;
                double cambioTotal = ((totalAjustado - totalOriginal) / totalOriginal) * 100;

                csv.WriteField("TOTAL GLOBAL");
                csv.WriteField(totalOriginal.ToString("N2"));
                csv.WriteField(totalAjustado.ToString("N2"));
                csv.WriteField(totalAfectado.ToString("N2"));
                csv.WriteField(cambioTotal.ToString("N2") + "%");
                csv.NextRecord();
            }

            Console.WriteLine($"Reporte de PIB afectado generado en: {rutaArchivo}");
        }
    }
}