using System;
using System.Collections.Generic;

namespace PBImpact.Services
{
    public class PibProjectionService
    {
        public static List<double> ProyectarPibAjustado5Years(double pibOriginal, double tasaArancel, double tasaCrecimiento)
        {
            var resultados = new List<double>();
            
            
            double pibAjustado = TariffImpactCalculator.CalcularPibAjustado(pibOriginal, tasaArancel);

           
            for (int year = 1; year <= 5; year++)
            {
                double pibProyectado = pibAjustado * Math.Pow(1 + tasaCrecimiento, year);
                resultados.Add(pibProyectado);
            }

            return resultados;
        }
    }
}
