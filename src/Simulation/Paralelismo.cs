using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class ResultadoSimulacion
{
    public string Pais { get; set; }
    public double PIBOriginal { get; set; }
    public double TasaArancel { get; set; }
    public double PIBAjustado { get; set; }
}

class Program
{
    static void Main(string[] args)
    {

        int[] hilos = [1, 2, 4, 8, 16];
        string rutaArchivo = "C:\\Users\\souls\\Desktop\\ITLA C6\\Programacion paralela\\datos.csv";
        int cantidadEsperada = ContarLineasValidas(rutaArchivo);

        Console.WriteLine("--------- COMPARANDO ESTADISTICAS -----------");

        // EJECUTANDO POR CADA NUMERO DE HILOS
        foreach (int numHilos in hilos)
        {
            Console.WriteLine($"Ejecutando con {numHilos} hilos:");
            DatosConParalellForeach(rutaArchivo, cantidadEsperada, numHilos);
        }
        DatosConWhenAll(rutaArchivo, cantidadEsperada);
    }

    static void DatosConParalellForeach(string ruta, int cantidadEsperada, int numHilos)
    {
        var resultados = new ConcurrentBag<ResultadoSimulacion>();
        var reloj = Stopwatch.StartNew();

        using (var lector = new StreamReader(ruta))
        {
            lector.ReadLine(); // Saltar encabezado
            var lineas = new List<string>();

            // Leer todas las líneas del archivo
            while (!lector.EndOfStream)
            {
                string linea = lector.ReadLine();
                if (!string.IsNullOrWhiteSpace(linea))
                    lineas.Add(linea);
            }

            Parallel.ForEach(lineas, new ParallelOptions { MaxDegreeOfParallelism = numHilos }, linea =>
            {

                //Agregando el resultado a la lista
                var partes = linea.Split(',');
                if (partes.Length == 3)
                {
                    try
                    {
                        string pais = partes[0];
                        double pib = double.Parse(partes[1]);
                        double tasa = double.Parse(partes[2]);

                        double pibAjustado = pib * (1 - tasa);

                        resultados.Add(new ResultadoSimulacion
                        {
                            Pais = pais,
                            PIBOriginal = pib,
                            TasaArancel = tasa,
                            PIBAjustado = pibAjustado
                        });
                    }
                    catch { }
                }
            });
        }

        reloj.Stop();

        Console.WriteLine("Parallel.ForEach:");
        MostrarEstadisticas(resultados.ToList(), cantidadEsperada, reloj.ElapsedMilliseconds);
    }

    static void DatosConWhenAll(string ruta, int cantidadEsperada)
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

                    resultados.Add(new ResultadoSimulacion
                    {
                        Pais = pais,
                        PIBOriginal = pib,
                        TasaArancel = tasa,
                        PIBAjustado = pibAjustado
                    });
                }
                catch { }
            }
        })).ToArray();

        Task.WaitAll(tareas);

        reloj.Stop();

        Console.WriteLine(">> Task.WhenAll:");
        MostrarEstadisticas(resultados.ToList(), cantidadEsperada, reloj.ElapsedMilliseconds);
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
        Console.WriteLine($" Tiempo: {tiempo} ms");
        Console.WriteLine($" Resultados: {resultados.Count} / Esperados: {esperados}");

        int errores = resultados.Count(r => r.Pais == null || r.PIBAjustado <= 0);
        Console.WriteLine(errores == 0 ? " * Todos los resultados son válidos" : $" - Resultados inválidos: {errores}");
        Console.WriteLine();
    }
}
