using ZedGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIBImpact.Analysis;

namespace PIBImpact.UI
{
    public partial class FrmPrincipal : Form
    {
        private ZedGraphControl zedGraphTiempos;
        private ZedGraphControl zedGraphProyeccion;
        private List<ResultadoSimulacion> resultados;

        public FrmPrincipal()
        {
            InicializarControles();
        }

        private void InicializarControles()
        {
            this.Text = "Simulación del Impacto del PIB";
            this.Width = 1300;
            this.Height = 800;

            var btnCargar = new Button() { Text = "Cargar CSV", Left = 10, Top = 10, Width = 120 };
            var btnSimular = new Button() { Text = "Simular", Left = 140, Top = 10, Width = 120 };
            var btnGraficar = new Button() { Text = "Graficar", Left = 270, Top = 10, Width = 120 };

            btnCargar.Click += BtnCargar_Click;
            btnSimular.Click += BtnSimular_Click;
            btnGraficar.Click += BtnGraficar_Click;

            zedGraphTiempos = new ZedGraphControl() { Left = 10, Top = 50, Width = 600, Height = 350 };
            zedGraphProyeccion = new ZedGraphControl() { Left = 620, Top = 50, Width = 600, Height = 350 };

            var dgvResultados = new DataGridView()
            {
                Name = "dgvResultados",
                Left = 10,
                Top = 420,
                Width = 1210,
                Height = 300,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(btnCargar);
            this.Controls.Add(btnSimular);
            this.Controls.Add(btnGraficar);
            this.Controls.Add(zedGraphTiempos);
            this.Controls.Add(zedGraphProyeccion);
            this.Controls.Add(dgvResultados);
        }

        private string rutaArchivo;

        private void BtnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos CSV (*.csv)|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                rutaArchivo = ofd.FileName;
                MessageBox.Show("Archivo cargado correctamente.");
            }
        }

        private void BtnSimular_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rutaArchivo))
            {
                MessageBox.Show("Primero debes cargar un archivo CSV.");
                return;
            }

            long tiempoSecuencial;
            var resultadosSecuencial = SimulacionProcessor.EjecutarSecuencial(rutaArchivo, out tiempoSecuencial);

            long tiempoParalelo;
            var resultadosParalelo = SimulacionProcessor.EjecutarParallel(rutaArchivo, out tiempoParalelo);

            var dgv = this.Controls.Find("dgvResultados", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                dgv.DataSource = resultadosParalelo.Select(r => new
                {
                    r.Pais,
                    r.PIBOriginal,
                    r.TasaArancel,
                    r.PIBAjustado,
                    r.CambioPib,
                    Proyeccion = string.Join(" -> ", r.Proyeccion5Annos.Select(p => p.ToString("F2")))
                }).ToList();
            }

            double pibMundialAjustado = SimulacionProcessor.CalcularPibMundialAjustado(resultadosParalelo);
            MessageBox.Show($"Simulación completada en {tiempoParalelo} ms (Paralelo).\nPIB Mundial Ajustado: {pibMundialAjustado:F2}");

            Graficador.GraficarTiempos(tiempoSecuencial, tiempoParalelo, zedGraphTiempos);

           
            resultados = resultadosParalelo;
        }

        private void BtnGraficar_Click(object sender, EventArgs e)
        {
            if (resultados == null || resultados.Count == 0)
            {
                MessageBox.Show("Primero ejecuta una simulación.");
                return;
            }

            Graficador.GraficarProyeccionPIB(resultados, zedGraphProyeccion);
        }
    }
}
