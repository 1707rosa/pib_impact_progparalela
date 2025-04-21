using CsvHelper;
using CsvHelper.Configuration;
using PIBImpact.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace PIBImpact.Data
{
    public class DataLoader
    {
        /// <summary>
        /// Carga registros desde un archivo CSV utilizando CsvHelper.
        /// </summary>
        /// <typeparam name="T">Tipo de registro (por ejemplo, Pais, SectorEconomico, Arancel)</typeparam>
        /// <param name="filePath">Ruta del archivo CSV</param>
        /// <param name="configuration">Configuración opcional para CsvHelper</param>
        /// <returns>Colección de registros mapeados</returns>
      
        public IEnumerable<T> LoadCsv<T>(string filePath, CsvConfiguration configuration = null)
        {
            try 
            {
                if (configuration == null)
                {
                    configuration = new CsvConfiguration(CultureInfo.InvariantCulture);
                }
                
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, configuration))
                {
                    var records = csv.GetRecords<T>();
                    // Convertir a lista para facilitar el manejo 
                    return new List<T>(records);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar CSV: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Carga registros desde un archivo JSON usando System.Text.Json.
        /// </summary>
        /// <typeparam name="T">Tipo de registro</typeparam>
        /// <param name="filePath">Ruta del archivo JSON</param>
        /// <returns>Colección de registros</returns>
        public IEnumerable<T> LoadJson<T>(string filePath)
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                var records = JsonSerializer.Deserialize<IEnumerable<T>>(jsonString);
                // Retorna una lista vacía si la deserialización resulta en null
                return records ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar JSON: {ex.Message}");
                throw;
            }
        }
    }
}
