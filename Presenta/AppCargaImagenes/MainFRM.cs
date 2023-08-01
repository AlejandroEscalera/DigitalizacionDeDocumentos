using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Negocio.Carga;
using System.Reactive;

namespace AppCargaImagenes
{
    public partial class MainFRM : Form
    {
        private WindowsFormsGlobalInformation? _windowsFormsGloablInformation;
        public MainFRM()
        {
            InitializeComponent();
            btnSeleccionaCarpetaGuardaValores.Click += BtnSeleccionaCarpetaGuardaValores;
            btnSekeccionDirectorioCargaExpedientes.Click += BtnSeleccionCarpetaExpedientes;
            btnCargaInformacionExpedientes.Click += BtnCargaImagenesCargaExpedientesClick;
        }

        private void BtnSalirClick(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSeleccionaCarpeta(object sender, EventArgs e)
        {
            if (fbdSeleccionaCarpetaImagenes.ShowDialog() == DialogResult.OK)
            {
                txtCarpetaArchivo.Text = fbdSeleccionaCarpetaImagenes.SelectedPath;
            }
        }

        private void BtnSeleccionaCarpetaGuardaValores(object? sender, EventArgs e)
        {
            if (fbdSeleccionaCarpetaImagenes.ShowDialog() == DialogResult.OK)
            {
                txtDirectorioCargaGuardaValores.Text = fbdSeleccionaCarpetaImagenes.SelectedPath;
            }
        }

        private void BtnSeleccionCarpetaExpedientes(object? sender, EventArgs e)
        {
            if (fbdSeleccionaCarpetaImagenes.ShowDialog() == DialogResult.OK)
            {
                txtDirectorioCargaExpedientes.Text = fbdSeleccionaCarpetaImagenes.SelectedPath;
            }
        }

        private void DeshabilitaBotones()
        {
            btnCargaInformacion.Enabled = false;
            btnCargaInformacionGuardaValores.Enabled = false;
            btnCargaInformacionExpedientes.Enabled = false;
        }
        private void HabilitaBotones()
        {
            btnCargaInformacion.Enabled = true;
            btnCargaInformacionGuardaValores.Enabled = true;
            btnCargaInformacionExpedientes.Enabled = true;
        }

        private async void BtnCargaImagenesClick(object? sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo de confirmación
            DialogResult resultado = MessageBox.Show("¿Desea continuar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }

            DeshabilitaBotones();
            try
            {
                pnlReportaAvances.Visible = true;
                if (_windowsFormsGloablInformation is not null)
                {
                    btnSalir.Enabled = false;
                    try
                    {
                        string directorioTemporal = txtDirectorioTemporal.Text;
                        Progress<ReporteProgresoProceso> reporteProgresoProceso = new();
                        reporteProgresoProceso.ProgressChanged += ReportProgress;
                        ICargaImagenes cargaImagenes = _windowsFormsGloablInformation.
                            ActivaCargaImagenesService();
                        if (cargaImagenes is not null)
                        {
                            await cargaImagenes.
                                CargaImagenesLiquidadosAsync(txtCarpetaArchivo.Text,
                                reporteProgresoProceso, directorioTemporal);
                            MessageBox.Show("Proceso de carga de imágenes terminado");
                        }
                    }
                    finally
                    {
                        btnSalir.Enabled = true;

                    }
                }

            }
            finally
            {
                HabilitaBotones();
            }
        }

        private async void BtnCargaImagenesGuardaValoresClick(object? sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo de confirmación
            DialogResult resultado = MessageBox.Show("¿Desea continuar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }

            DeshabilitaBotones();
            try
            {
                pnlReportaAvances.Visible = true;
                if (_windowsFormsGloablInformation is not null)
                {
                    btnSalir.Enabled = false;
                    try
                    {
                        string unidadTemporal = txtUnidadTemporal.Text;
                        Progress<ReporteProgresoProceso> reporteProgresoProceso = new();
                        reporteProgresoProceso.ProgressChanged += ReportProgress;
                        ICargaImagenes cargaImagenes = _windowsFormsGloablInformation.
                            ActivaCargaImagenesService();
                        if (cargaImagenes is not null)
                        {
                            await cargaImagenes.
                                CargaImagenesGuardaValoresAsync(txtDirectorioCargaGuardaValores.Text,
                                reporteProgresoProceso, unidadTemporal);
                            MessageBox.Show("Proceso de carga de imágenes terminado");
                        }
                    }
                    finally
                    {
                        btnSalir.Enabled = true;

                    }
                }

            }
            finally
            {
                HabilitaBotones();
            }
        }

        private async void BtnCargaImagenesCargaExpedientesClick(object? sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo de confirmación
            DialogResult resultado = MessageBox.Show("¿Desea continuar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }

            DeshabilitaBotones();
            try
            {
                pnlReportaAvances.Visible = true;
                if (_windowsFormsGloablInformation is not null)
                {
                    btnSalir.Enabled = false;
                    try
                    {
                        string unidadTemporal = txtUnidadTemporal.Text;
                        Progress<ReporteProgresoProceso> reporteProgresoProceso = new();
                        reporteProgresoProceso.ProgressChanged += ReportProgress;
                        ICargaImagenes cargaImagenes = _windowsFormsGloablInformation.
                            ActivaCargaImagenesService();
                        if (cargaImagenes is not null)
                        {
                            await cargaImagenes.
                                CargaImagenesExpedientesAsync(txtDirectorioCargaGuardaValores.Text,
                                reporteProgresoProceso, unidadTemporal);
                            MessageBox.Show("Proceso de carga de imágenes terminado");
                        }
                    }
                    finally
                    {
                        btnSalir.Enabled = true;

                    }
                }

            }
            finally
            {
                HabilitaBotones();
            }
        }

        private void ReportProgress(object? sender, ReporteProgresoProceso e)
        {
            lblEtapa.Text = e.EtiquetaDePaso;
            if (e.CantidadDePasos > 0)
            {
                prgEtapa.Maximum = 100; // Convert.ToInt32(e.CantidadDePasos);
                prgEtapa.Step = 1;
                prgEtapa.Value = Convert.ToInt32((e.NumeroPasoActual * 100) / e.CantidadDePasos);
            }
            if (string.IsNullOrEmpty(e.EtiquetaSubProceso))
            {
                if (lblSubProcesoEtiqueta.Visible)
                {
                    lblSubProcesoEtiqueta.Visible = false;
                }
            }
            else
            {
                if (!lblSubProcesoEtiqueta.Visible)
                {
                    lblSubProcesoEtiqueta.Visible = true;
                }
            }
            lblSubProceso.Text = e.EtiquetaSubProceso;
            if (e.TotalProgresoSubProceso > 0)
            {
                if (!prgSubProceso.Visible)
                {
                    prgSubProceso.Visible = true;
                }
                prgSubProceso.Maximum = 100; // Convert.ToInt32(e.TotalProgresoSubProceso);
                prgSubProceso.Step = 1;
                prgSubProceso.Value = Convert.ToInt32((e.NumeroProgresoSubProceso * 100) / e.TotalProgresoSubProceso);
                // prgSubEtapa.Value = Convert.ToInt32((e.ProgresoSubProceso * 100) / e.TotalProgresoSubProceso);
            }
            else
            {
                if (prgSubProceso.Visible)
                {
                    prgSubProceso.Visible = false;
                }
            }

            if (string.IsNullOrEmpty(e.InformacionArchivo))
            {
                if (lblArchivoEtiqueta.Visible)
                {
                    lblArchivoEtiqueta.Visible = false;
                }
            }
            else
            {
                if (!lblArchivoEtiqueta.Visible)
                {
                    lblArchivoEtiqueta.Visible = true;
                }
            }
            lblArchivo.Text = e.InformacionArchivo;
            if (e.CantidadArchivos > 0)
            {
                if (!prgArchivo.Visible)
                {
                    prgArchivo.Visible = true;
                }
                prgArchivo.Maximum = 100; // Convert.ToInt32(e.CantidadArchivos);
                prgArchivo.Step = 1;
                prgArchivo.Value = Convert.ToInt32((e.ArchivoProcesado * 100) / e.CantidadArchivos);
                // prgArchivos.Value = Convert.ToInt32((e.ArchivosProcesados * 100) / e.CantidadArchivos);
            }
            else
            {
                if (prgArchivo.Visible)
                {
                    prgArchivo.Visible = false;
                }
            }
            // Application.DoEvents();
            // throw new NotImplementedException();
        }

        private async Task CargaInformacionAsync()
        {
            await Task.Run(() => _windowsFormsGloablInformation = WindowsFormsGlobalInformation.Instance);
            await Task.Run(() => _windowsFormsGloablInformation?.LlenaInformacionGlobal());
        }
        private async void MainFRM_Load(object sender, EventArgs e)
        {
            txtDirectorioTemporal.Text = Path.GetTempPath();
            fbdSeleccionaTemporal.InitialDirectory = Path.GetTempPath();
            fbdSeleccionaTemporal.SelectedPath = Path.GetTempPath();
            await CargaInformacionAsync();
            btnCargaInformacion.Visible = true;
        }

        private void BtnSeleccionaTemporalClick(object sender, EventArgs e)
        {

            if (fbdSeleccionaTemporal.ShowDialog() == DialogResult.OK)
            {
                txtDirectorioTemporal.Text = fbdSeleccionaTemporal.SelectedPath;
            }
        }
    }
}