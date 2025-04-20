using System;
using System.Collections.Generic;
using System.Linq;
using PIBImpact.Models;

namespace PIBImpact.Analysis
{
    public static class ResultAnalyzer
    {
        // Calcula el PIB total afectado sumando los cambios
        public static double CalcularPibTotalAfectado(List<ResultadoSimulacion> resultados)
        {
            if (resultados == null || resultados.Count == 0)
                return 0;

            return resultados.Sum(r => r.CambioPib);
        }

        // Obtiene los sectores mas impactados (mayor cambio en PIB, positivo o negativo)
        public static List<(string Sector, double ImpactoTotal)> ObtenerSectoresMasImpactados(List<ResultadoSimulacion> resultados, int topN = 5)
        {
            if (resultados == null || resultados.Count == 0)
                return new List<(string, double)>();

            return resultados
                .GroupBy(r => r.Sector)
                .Select(g => new
                {
                    Sector = g.Key,
                    ImpactoTotal = g.Sum(r => r.CambioPib)
                })
                .OrderByDescending(x => Math.Abs(x.ImpactoTotal)) // Ordenado por magnitud
                .Take(topN)
                .Select(x => (x.Sector, x.ImpactoTotal))
                .ToList();
        }
    }
}
