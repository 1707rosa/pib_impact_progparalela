using PIBImpact.Data;
using PIBImpact.Models;
using System;
using System.Linq;

namespace PIBImpact
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataLoader = new DataLoader();

            // Ejemplo de carga de datos desde CSV
            try
            {
                // Reemplazar "path/to/paises.csv" por la ruta real del archivo CSV
                var paises = dataLoader.LoadCsv<Pais>("path/to/paises.csv").ToList();
                Console.WriteLine("Datos de países cargados:");
                foreach (var pais in paises)
                {
                    Console.WriteLine($"Nombre: {pais.Nombre}, PIB: {pais.PIB}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar datos CSV: " + ex.Message);
            }

            // Ejemplo de carga de datos desde JSON (descomentar si se desea probar)
            /*
            try
            {
                // Reemplazar "path/to/paises.json" por la ruta real del archivo JSON
                var paisesJson = dataLoader.LoadJson<Pais>("path/to/paises.json").ToList();
                Console.WriteLine("Datos de países cargados desde JSON:");
                foreach (var pais in paisesJson)
                {
                    Console.WriteLine($"Nombre: {pais.Nombre}, PIB: {pais.PIB}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar datos JSON: " + ex.Message);
            }
            */
        }
    }
}
