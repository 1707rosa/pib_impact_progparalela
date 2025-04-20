using PIBImpact.Models;
using System;
using System.Collections.Generic;

namespace PIBImpact.Analysis
{
        public static class VisualizadorConsola
        {
            public static void MostrarTablaResultados(List<ResultadoSimulacion> resultados)
            {
                    Console.WriteLine("\n Resultados detallados por país y sector:");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("{0,-10} | {1,-15} | {2,10}", "Pais", "Sector", "Cambio PIB");
                    Console.WriteLine("-------------------------------------------");

                    foreach (var r in resultados)
                    {
                        Console.WriteLine("{0,-10} | {1,-15} | {2,10:+0.00;-0.00}%", r.Pais, r.Sector, r.CambioPib);
                    }

                    Console.WriteLine("-------------------------------------------\n");
        }
    }
}