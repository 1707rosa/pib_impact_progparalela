using PIBImpact.Data;
using PIBImpact.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace PIBImpact.Tests
{
    public class DataLoaderTests
    {
        private readonly DataLoader _dataLoader = new DataLoader();

        [Fact]
        public void LoadCsv_ShouldReturnRecords_WhenCsvIsValid()
        {
            // Preparar contenido CSV válido para la prueba
            var csvContent = "Nombre,PIB\nSpain,1400.50\nFrance,1800.75";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, csvContent);

            // Ejecutar la carga del CSV
            IEnumerable<Pais> paises = _dataLoader.LoadCsv<Pais>(tempFile);
            var paisList = paises.ToList();

            // Validar que se hayan cargado correctamente los registros
            Assert.Equal(2, paisList.Count);
            Assert.Equal("Spain", paisList[0].Nombre);
            Assert.Equal(1400.50m, paisList[0].PIB);

            // Limpieza del archivo temporal
            File.Delete(tempFile);
        }

        [Fact]
        public void LoadJson_ShouldReturnRecords_WhenJsonIsValid()
        {
            // Preparar contenido JSON válido para la prueba
            var jsonContent = "[{\"Nombre\":\"Germany\",\"PIB\":2000.0}, {\"Nombre\":\"Italy\",\"PIB\":1500.0}]";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, jsonContent);

            // Ejecutar la carga del JSON
            IEnumerable<Pais> paises = _dataLoader.LoadJson<Pais>(tempFile);
            var paisList = paises.ToList();

            // Validar que se hayan cargado correctamente los registros
            Assert.Equal(2, paisList.Count);
            Assert.Equal("Germany", paisList[0].Nombre);
            Assert.Equal(2000.0m, paisList[0].PIB);

            // Limpieza del archivo temporal
            File.Delete(tempFile);
        }
    }
}
