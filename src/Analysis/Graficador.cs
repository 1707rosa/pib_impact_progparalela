using ZedGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PIBImpact.Analysis
{
    public static class Graficador
    {
        public static void GraficarTiempos(List<int> hilos, List<long> tiemposParalelos, long tiempoSecuencial, ZedGraphControl zgc)
        {
            GraphPane pane = zgc.GraphPane;
            pane.CurveList.Clear();

            pane.Title.Text = "Comparación de Tiempos de Ejecución";
            pane.XAxis.Title.Text = "Cantidad de Hilos";
            pane.YAxis.Title.Text = "Tiempo (ms)";

            PointPairList puntos = new PointPairList();
            for (int i = 0; i < hilos.Count; i++)
            {
                puntos.Add(hilos[i], tiemposParalelos[i]);
            }

            LineItem curva = pane.AddCurve("Paralelo", puntos, Color.Blue, SymbolType.Circle);

            // Línea horizontal para la ejecución secuencial
            LineItem lineaSecuencial = pane.AddCurve("Secuencial", new PointPairList
    {
        new PointPair(hilos.Min(), tiempoSecuencial),
        new PointPair(hilos.Max(), tiempoSecuencial)
    }, Color.Red, SymbolType.None);

            curva.Line.Width = 2.0F;
            lineaSecuencial.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            lineaSecuencial.Line.Width = 2.0F;

            zgc.AxisChange();
            zgc.Invalidate();
        }
        public static void GraficarProyeccionPIB(List<ResultadoSimulacion> resultados, ZedGraphControl zedGraphControl, string? rutaGuardar = null)
        {
            GraphPane myPane = zedGraphControl.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();

            myPane.Title.Text = "Proyección del PIB Mundial - Próximos 5 años";
            myPane.XAxis.Title.Text = "Año";
            myPane.YAxis.Title.Text = "PIB Mundial Ajustado (USD)";


            double[] pibMundialPorAnno = new double[5];
            foreach (var resultado in resultados)
            {
                for (int i = 0; i < 5; i++)
                {
                    pibMundialPorAnno[i] += resultado.Proyeccion5Annos[i];
                }
            }

            PointPairList puntos = new PointPairList();
            for (int i = 0; i < 5; i++)
            {
                puntos.Add(i + 1, pibMundialPorAnno[i]);
            }

            LineItem curva = myPane.AddCurve("PIB Mundial Ajustado", puntos, Color.DarkCyan, SymbolType.Circle);
            curva.Line.Width = 2.5F;
            curva.Symbol.Fill = new Fill(Color.White);


            for (int i = 0; i < 5; i++)
            {
                string texto = pibMundialPorAnno[i].ToString("F2");
                TextObj etiqueta = new TextObj(texto, i + 1, pibMundialPorAnno[i], CoordType.AxisXYScale, AlignH.Left, AlignV.Bottom);
                etiqueta.FontSpec.Size = 10;
                myPane.GraphObjList.Add(etiqueta);
            }


            myPane.XAxis.Scale.Min = 0.5;
            myPane.XAxis.Scale.Max = 5.5;
            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.TextLabels = new[] { "1", "2", "3", "4", "5" };
            myPane.XAxis.Type = AxisType.Text;

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();

            if (!string.IsNullOrEmpty(rutaGuardar))
            {
                AsegurarDirectorio(rutaGuardar);
                myPane.GetImage().Save(rutaGuardar, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void AsegurarDirectorio(string ruta)
        {
            string directorio = Path.GetDirectoryName(ruta);


            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
            {
                try
                {
                    Directory.CreateDirectory(directorio);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear el directorio: {ex.Message}");
                }
            }
        }

        internal static void GraficarTiempos(long tiempoSecuencial, long tiempoParalelo, ZedGraphControl zgc)
    {
        throw new NotImplementedException();
    }

    }
}
