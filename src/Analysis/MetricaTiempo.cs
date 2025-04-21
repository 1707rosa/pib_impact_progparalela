using System;
using System.Diagnostics;

namespace PIBImpact.Analysis
{
    public static class MetricaTiempo
    {
        public static T MedirTiempo<T>(Func<T> funcion, out long milisegundos)
        {
            var stopwatch = Stopwatch.StartNew();
            T resultado = funcion();
            stopwatch.Stop();
            milisegundos = stopwatch.ElapsedMilliseconds;
            return resultado;
        }

        public static void MedirTiempo(Action accion, out long milisegundos)
        {
            var stopwatch = Stopwatch.StartNew();
            accion();
            stopwatch.Stop();
            milisegundos = stopwatch.ElapsedMilliseconds;
        }
    }
}
