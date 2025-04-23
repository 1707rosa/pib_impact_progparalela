using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PIBImpact.Services;
 
public class ResultadoSimulacion
{
    public string Pais { get; set; }
    public double PIBOriginal { get; set; }
    public double TasaArancel { get; set; }
    public double PIBAjustado { get; set; }
    public double CambioPib { get; set; }
    public string Sector { get; set; }
    public List<double> Proyeccion5Annos { get; set; } = new();
}
 
public static class SimulacionProcessor
{
    private const double TasaCrecimiento = 0.02;
 
    public static List<ResultadoSimulacion> EjecutarSecuencial(string ruta, out long tiempoMs)
    {
        var resultados = new List<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();
 
        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine();
            while (!lector.EndOfStream)
            {
                string? linea = lector.ReadLine();
                if (!string.IsNullOrWhiteSpace(linea))
                {
                    var partes = linea.Split(',');
                    if (partes.Length == 3)
                    {
                        try
                        {
                            string pais = partes[0];
                            double pib = double.Parse(partes[1]);
                            double tasa = double.Parse(partes[2]);
 
                            double pibAjustado = TariffImpactCalculator.CalcularPibAjustado(pib, tasa);
                            double cambioPib = (pibAjustado - pib) / pib * 100;
 
                            var resultado = new ResultadoSimulacion
                            {
                                Pais = pais,
                                PIBOriginal = pib,
                                TasaArancel = tasa,
                                PIBAjustado = pibAjustado,
                                CambioPib = cambioPib,
                                Sector = "General",
                                Proyeccion5Annos = PibProjectionService.ProyectarPibAjustado5Years(pib, tasa, TasaCrecimiento)
                            };
 
                            resultados.Add(resultado);
                        }
                        catch { }
                    }
                }
            }
        }
 
        reloj.Stop();
        tiempoMs = reloj.ElapsedMilliseconds;
        return resultados;
    }
 
    public static List<ResultadoSimulacion> EjecutarParallel(string ruta, out long tiempoMs)
    {
        var resultados = new ConcurrentBag<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();
 
        List<string> lineas;
        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine(); // encabezado
            lineas = lector.ReadToEnd()
                           .Split('\n')
                           .Where(l => !string.IsNullOrWhiteSpace(l))
                           .ToList();
        }
 
        Parallel.ForEach(lineas, linea =>
        {
            var partes = linea.Split(',');
            if (partes.Length == 3)
            {
                try
                {
                    string pais = partes[0];
                    double pib = double.Parse(partes[1]);
                    double tasa = double.Parse(partes[2]);
 
                    double pibAjustado = TariffImpactCalculator.CalcularPibAjustado(pib, tasa);
                    double cambioPib = (pibAjustado - pib) / pib * 100;
 
                    var resultado = new ResultadoSimulacion
                    {
                        Pais = pais,
                        PIBOriginal = pib,
                        TasaArancel = tasa,
                        PIBAjustado = pibAjustado,
                        CambioPib = cambioPib,
                        Sector = "General",
                        Proyeccion5Annos = PibProjectionService.ProyectarPibAjustado5Years(pib, tasa, TasaCrecimiento)
                    };
 
                    resultados.Add(resultado);
                }
                catch { }
            }
        });
 
        reloj.Stop();
        tiempoMs = reloj.ElapsedMilliseconds;
        return resultados.ToList();
    }
 
    public static List<ResultadoSimulacion> EjecutarWhenAll(string ruta, out long tiempoMs)
    {
        var resultados = new ConcurrentBag<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();
 
        List<string> lineas;
        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine();
            lineas = lector.ReadToEnd()
                           .Split('\n')
                           .Where(l => !string.IsNullOrWhiteSpace(l))
                           .ToList();
        }
 
        var tareas = lineas.Select(linea => Task.Run(() =>
        {
            var partes = linea.Split(',');
            if (partes.Length == 3)
            {
                try
                {
                    string pais = partes[0];
                    double pib = double.Parse(partes[1]);
                    double tasa = double.Parse(partes[2]);
 
                    double pibAjustado = TariffImpactCalculator.CalcularPibAjustado(pib, tasa);
                    double cambioPib = (pibAjustado - pib) / pib * 100;
 
                    var resultado = new ResultadoSimulacion
                    {
                        Pais = pais,
                        PIBOriginal = pib,
                        TasaArancel = tasa,
                        PIBAjustado = pibAjustado,
                        CambioPib = cambioPib,
                        Sector = "General",
                        Proyeccion5Annos = PibProjectionService.ProyectarPibAjustado5Years(pib, tasa, TasaCrecimiento)
                    };
 
                    resultados.Add(resultado);
                }
                catch { }
            }
        })).ToArray();
 
        Task.WaitAll(tareas);
 
        reloj.Stop();
        tiempoMs = reloj.ElapsedMilliseconds;
        return resultados.ToList();
    }
 
    public static double CalcularPibMundialAjustado(List<ResultadoSimulacion> resultados)
    {
       
        double pibMundialAjustado = resultados.Sum(r => r.PIBAjustado);
       
        return pibMundialAjustado;
    }
}
