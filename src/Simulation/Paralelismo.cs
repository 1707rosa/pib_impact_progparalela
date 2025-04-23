using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PIBImpact.Analysis; // Para usar VisualizadorConsola

public class ResultadoSimulacion
{
    public string Pais { get; set; }
    public double PIBOriginal { get; set; }
    public double TasaArancel { get; set; }
    public double PIBAjustado { get; set; }
    public double CambioPib { get; set; } // en porcentaje
    public string Sector { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        string rutaArchivo = "C:\\Users\\souls\\Source\\Repos\\pib_impact_progparalela\\metrics\\datos_simulacion.csv";
        int cantidadEsperada = ContarLineasValidas(rutaArchivo);

        Console.WriteLine(">>> Método: Secuencial <<<\n");
        long tiempoSecuencial = DatosSecuencial(rutaArchivo, cantidadEsperada);
        Console.WriteLine("--------------------------------------------------\n");

        Console.WriteLine(">>> Método: Parallel.ForEach <<<\n");
        long tiempoParallel = DatosConParalellForeach(rutaArchivo, cantidadEsperada);
        Console.WriteLine("--------------------------------------------------\n");

        Console.WriteLine(">>> Método: Task.WhenAll <<<\n");
        long tiempoWhenAll = DatosConWhenAll(rutaArchivo, cantidadEsperada);
        Console.WriteLine("=============================        =====================\n");

        Graficador.GraficarTiempos(tiempoSecuencial, tiempoParallel);
    }

    static long DatosSecuencial(string ruta, int cantidadEsperada)
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

                            double pibAjustado = pib * (1 - tasa);
                            double cambioPib = (pibAjustado - pib) / pib * 100;

                            resultados.Add(new ResultadoSimulacion
                            {
                                Pais = pais,
                                PIBOriginal = pib,
                                TasaArancel = tasa,
                                PIBAjustado = pibAjustado,
                                CambioPib = cambioPib,
                                Sector = "General"
                            });
                        }
                        catch { }
                    }
                }
            }
        }

        reloj.Stop();
        MostrarEstadisticas(resultados, cantidadEsperada, reloj.ElapsedMilliseconds);
        VisualizadorConsola.MostrarTablaResultados(resultados);
        return reloj.ElapsedMilliseconds;
    }

    static long DatosConParalellForeach(string ruta, int cantidadEsperada, int numHilos = -1)
    {
        var resultados = new ConcurrentBag<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();

        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine();
            var lineas = new List<string>();

            while (!lector.EndOfStream)
            {
                string linea = lector.ReadLine();
                if (!string.IsNullOrWhiteSpace(linea))
                    lineas.Add(linea);
            }

            var opciones = new ParallelOptions();
            if (numHilos > 0) opciones.MaxDegreeOfParallelism = numHilos;

            Parallel.ForEach(lineas, opciones, linea =>
            {
                var partes = linea.Split(',');
                if (partes.Length == 3)
                {
                    try
                    {
                        string pais = partes[0];
                        double pib = double.Parse(partes[1]);
                        double tasa = double.Parse(partes[2]);

                        double pibAjustado = pib * (1 - tasa);
                        double cambioPib = (pibAjustado - pib) / pib * 100;

                        resultados.Add(new ResultadoSimulacion
                        {
                            Pais = pais,
                            PIBOriginal = pib,
                            TasaArancel = tasa,
                            PIBAjustado = pibAjustado,
                            CambioPib = cambioPib,
                            Sector = "General"
                        });
                    }
                    catch { }
                }
            });
        }

        reloj.Stop();
        MostrarEstadisticas(resultados.ToList(), cantidadEsperada, reloj.ElapsedMilliseconds);
        VisualizadorConsola.MostrarTablaResultados(resultados.ToList());
        return reloj.ElapsedMilliseconds;
    }

    static long DatosConWhenAll(string ruta, int cantidadEsperada)
    {
        var resultados = new ConcurrentBag<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();

        List<string> lineas;
        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine();
            lineas = new List<string>();
            while (!lector.EndOfStream)
            {
                string linea = lector.ReadLine();
                if (!string.IsNullOrWhiteSpace(linea))
                    lineas.Add(linea);
            }
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

                    double pibAjustado = pib * (1 - tasa);
                    double cambioPib = (pibAjustado - pib) / pib * 100;

                    resultados.Add(new ResultadoSimulacion
                    {
                        Pais = pais,
                        PIBOriginal = pib,
                        TasaArancel = tasa,
                        PIBAjustado = pibAjustado,
                        CambioPib = cambioPib,
                        Sector = "General"
                    });
                }
                catch { }
            }
        })).ToArray();

        Task.WaitAll(tareas);

        reloj.Stop();
        MostrarEstadisticas(resultados.ToList(), cantidadEsperada, reloj.ElapsedMilliseconds);
        VisualizadorConsola.MostrarTablaResultados(resultados.ToList());
        return reloj.ElapsedMilliseconds;
    }

    static int ContarLineasValidas(string ruta)
    {
        int contador = 0;
        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine();
            while (!lector.EndOfStream)
            {
                var linea = lector.ReadLine();
                if (!string.IsNullOrWhiteSpace(linea))
                    contador++;
            }
        }
        return contador;
    }

    static void MostrarEstadisticas(List<ResultadoSimulacion> resultados, int esperados, long tiempo)
    {
        Console.WriteLine($" Tiempo de ejecución: {tiempo} ms");
        Console.WriteLine($" Resultados obtenidos: {resultados.Count} / Esperados: {esperados}");

        int errores = resultados.Count(r => r.Pais == null || r.PIBAjustado <= 0);
        Console.WriteLine(errores == 0
            ? "  Todos los resultados son válidos"
            : $"   Resultados inválidos: {errores}");

        Console.WriteLine();
    }
}
