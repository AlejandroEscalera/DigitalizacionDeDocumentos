using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteLiquidados;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM : Form
{
    IEnumerable<ColocacionConPagos>? _colocacionConPagos;

    private void InicializaCreditosLiquidados()
    {
// #pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        lblCreditosLiquidados.DoubleClick += ClDoubleClick;
        lblDetalleLiquidados.DoubleClick += ClDoubleClick;
        lblGloboLiquidados.DoubleClick += ClDoubleClick;

        // 10
        btnlrSiguiente.Click += ClickRegionLiquidados;
        dgvLiquidadosRegiones.DoubleClick += ClickRegionLiquidados;
        btnlrAnterior.Click += NavegaRegresoResumen;

        // 11
        btnlaSiguiente.Click += ClickAgenciaLiquidados;
        dgvLiquidadosAgencias.DoubleClick += ClickAgenciaLiquidados;
        btnlaAnterior.Click += ClRegresaRegionLiquidadosClick;

        // 12
        btnldAnterior.Click += NavegaClickAgenciaLiquidados;
        btnldVerImagen.Click += VerImagenLiquidados;
        dgvLiquidadosDetalle.CellDoubleClick += VerImagenLiquidados;

        dgvLiquidadosRegiones.CellFormatting += DgvLiquidadosRegiones_CellFormatting;
        dgvLiquidadosAgencias.CellFormatting += DgvLiquidadosAgencias_CellFormatting;
        dgvLiquidadosDetalle.CellFormatting += DgvLiquidadosDetalle_CellFormatting;
        dgvLiquidadosDetalle.SelectionChanged += DetalleLiquidados_SelectionChanged;

        // #pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        if (_colocacionConPagos is null && _windowsFormsGloablInformation is not null)
        {
            _colocacionConPagos = _windowsFormsGloablInformation.ActivaConsultasServices().CargaColocacionConPagos();
        }
        else
            if (_windowsFormsGloablInformation is null)
            return;
        int cantidadCreditosLiquidados = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosLiquidados();
        lblDetalleLiquidados.Text = string.Format("{0:#,##0} Créditos Liquidados", cantidadCreditosLiquidados);
    }

    private void ActivaBotonVerImagenLiquidados()
    {
        if (_estoyPintandoDatos)
            return;
        if (dgvLiquidadosDetalle.SelectedRows is not null && dgvLiquidadosDetalle.CurrentRow is not null)
        {

            bool tieneImagenDirecta = Convert.ToBoolean(dgvLiquidadosDetalle.CurrentRow.Cells[18].Value);
            bool tieneImagenInDirecta = Convert.ToBoolean(dgvLiquidadosDetalle.CurrentRow.Cells[19].Value);
            _estoyPintandoDatos = true;
            try
            {
                btnldVerImagen.Visible = tieneImagenDirecta || tieneImagenInDirecta;
            }
            finally
            {
                _estoyPintandoDatos = false;
            }
        }
    }

    private void DetalleLiquidados_SelectionChanged(object? sender, EventArgs e)
    {
        ActivaBotonVerImagenLiquidados();
    }

    private void DgvLiquidadosDetalle_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos && e.RowIndex != -1)
        {
            int region = Convert.ToInt32(dgvLiquidadosDetalle.Rows[e.RowIndex].Cells[0].Value);
            if (region == 0)
            {                
                e.CellStyle.BackColor = Color.Black;
                e.CellStyle.ForeColor = Color.White;
                //await Task.Delay(0);
                //Application.DoEvents();
            }
        }
    }
    private void DgvLiquidadosAgencias_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvLiquidadosAgencias.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }
    }

    protected void DgvLiquidadosRegiones_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvLiquidadosRegiones.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                    // e.FormattingApplied = true;
                }
            }
        }
    }

    private void NavegaClickAgenciaLiquidados(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 11;
    }

    private void VerImagenLiquidados(object? sender, EventArgs e)
    {
        regresaTabNavegacion = 2;
        regresaTabReportesFinales = 12;
        datosRegreso.Push(new NavegacionRetorno()
        {
            TabPrincipal = regresaTabNavegacion,
            TabReporte = regresaTabReportesFinales
        });
        MuestraImagenRdLiquidados();
    }

    private void MuestraImagenRdLiquidados()
    {
        if (_estoyPintandoDatos)
            return;
        if (dgvLiquidadosDetalle.SelectedRows is not null && dgvLiquidadosDetalle.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvLiquidadosDetalle.CurrentRow.Cells[1].Value);
            object objValue = dgvLiquidadosDetalle.CurrentRow.Cells[2].Value;
            string numeroDeCredito;
            if (objValue == null)
                numeroDeCredito = string.Empty;
            else
            {
                numeroDeCredito = Convert.ToString(objValue) ?? string.Empty;
            }
            if (agencia != 0 && !string.IsNullOrEmpty(numeroDeCredito))
            {
                _expCasExpedienteNumCreditoSeleccionado = numeroDeCredito;
                ExpedienteDeConsultaGv? expediente = BuscaImagenes(_expCasExpedienteNumCreditoSeleccionado);
                if (expediente is not null && (imagenesEncontradas is not null) && (imagenesEncontradas.Any()))
                {
                    _estoyPintandoDatos = true;
                    try
                    {
                        MuestraExpedienteEnImagen(expediente);
                        NavegaVisorImagenes();
                        tabReportesFinales.SelectedIndex = 0;
                    }
                    finally
                    {
                        _estoyPintandoDatos = false;
                    }
                }
            }
        }
    }

    private void ClickAgenciaLiquidados(object? sender, EventArgs e)
    {
        SeleccionaAgenciaLiquidados();
    }

    private void ClDoubleClick(object? sender, EventArgs e)
    {
        ObtieneInformacionLiquidados();
    }

    private void ObtieneInformacionLiquidados()
    {
        if (_estoyPintandoDatos)
            return;
        if (_colocacionConPagos is null && _windowsFormsGloablInformation is not null)
        {
            _colocacionConPagos = _windowsFormsGloablInformation.ActivaConsultasServices().CargaColocacionConPagos();
        }
        else
    if (_windowsFormsGloablInformation is null)
            return;
        int cantidadCreditosLiquidados = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosLiquidados();
        lblDetalleLiquidados.Text = string.Format("{0:#,##0} Créditos Liquidados", cantidadCreditosLiquidados);


        IEnumerable<CreditosLiquidadosRegiones> listaCreditosLiquidadosRegiones = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosLiquidadosRegiones();
        _estoyPintandoDatos = true;
        try
        {
            dgvLiquidadosRegiones.DataSource = listaCreditosLiquidadosRegiones;
        }
        finally
        {
            _estoyPintandoDatos = false;
        }
        tabReportesFinales.SelectedIndex = 10;
    }

    private void ClRegresaRegionLiquidadosClick(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 10;
    }

    private void SeleccionaAgenciaLiquidados()
    {
        if (!_estoyPintandoDatos)
        {
            if (dgvLiquidadosAgencias.SelectedRows is not null && dgvLiquidadosAgencias.CurrentRow is not null)
            {
                int agencia = Convert.ToInt32(dgvLiquidadosAgencias.CurrentRow.Cells[1].Value);
                object objValue = dgvLiquidadosAgencias.CurrentRow.Cells[2].Value;
                string nombreAgencia;
                if (objValue == null)
                    nombreAgencia = string.Empty;
                else
                {
                    nombreAgencia = Convert.ToString(objValue) ?? string.Empty;
                }
                if (agencia != 0)
                {
                    lblCargaInformacionAgencias.Visible = true;
                    lblCargaInformacionAgencias.Text = string.Format("Cargando información del detalle de creditos de la Agencia {0}, Espere un momento por favor...", nombreAgencia);
                    lblCargaInformacionAgencias.ForeColor = Color.Red;
                    Application.DoEvents();
                    try
                    {
                        MuestraDetalleAgenciaLiquidados(agencia, nombreAgencia);
                        tabReportesFinales.SelectedIndex = 12;
                    }
                    finally
                    {
                        lblCargaInformacionAgencias.Visible = false;
                        Application.DoEvents();
                    }

                }
            }
        }
    }

    private void MuestraDetalleAgenciaLiquidados(int agencia, string nombreAgencia)
    {
        if (_estoyPintandoDatos)
            return;
        if (_windowsFormsGloablInformation is not null)
        {
            BindingList<CreditosLiquidadosDetalle>? detalleCreditos = new(_windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosLiquidadosAgencia(agencia).ToList());
            lblldEncabezado.Text = String.Format("Creditos Liquidados de la Agencia {0}", nombreAgencia);
            dgvLiquidadosDetalle.AutoGenerateColumns = false;
            dgvLiquidadosDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            foreach (DataGridViewColumn column in dgvLiquidadosDetalle.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (detalleCreditos is not null)
            {
                CreditosLiquidadosDetalle? total = detalleCreditos.Where(x => x.Region == 0 && x.Agencia == 0).FirstOrDefault();
                /*
                if (total is not null)
                {
                    totalCeditos 
                    totalVigentes = detalleCreditos.Count(x => x.EsVigente == true);
                    totalImpago = detalleCreditos.Count(x => x.EsImpago == true);
                    totalVencida = detalleCreditos.Count(x => x.EsVencida == true);
                    totalOrigenDelDr = detalleCreditos.Count(x => x.EsOrigenDelDr == true);
                }
                */
            }
            _estoyPintandoDatos = true;
            //SuspendLayout();
            dgvLiquidadosDetalle.SuspendLayout();
            Cursor = Cursors.WaitCursor;
            dgvLiquidadosDetalle.DataSource = null;
            Application.DoEvents();
            try
            {
                dgvLiquidadosDetalle.DataSource = detalleCreditos;
            }
            finally
            {
                _estoyPintandoDatos = false;
                dgvLiquidadosDetalle.ResumeLayout();
                //ResumeLayout(false);
                Cursor = Cursors.Default;
            }
            ActivaBotonVerImagenLiquidados();
        }
    }

    private void SeleccionaRegionLiquidados()
    {
        if (dgvLiquidadosRegiones.SelectedRows is not null && dgvLiquidadosRegiones.CurrentRow is not null)
        {
            int region = Convert.ToInt32(dgvLiquidadosRegiones.CurrentRow.Cells[0].Value);
            object objValue = dgvLiquidadosRegiones.CurrentRow.Cells[1].Value;
            string nombreRegion;
            if (objValue == null)
                nombreRegion = string.Empty;
            else
            {
                nombreRegion = Convert.ToString(objValue) ?? string.Empty;
            }
            if (region != 0)
            {
                lblCargandoInformacionRegionales.Text = "Cargando Información de las agencias de la Región... Espere un momento!!";
                lblCargandoInformacionRegionales.ForeColor = Color.Red;
                lblCargandoInformacionRegionales.Visible = true;
                Application.DoEvents();
                try
                {
                    MuestraDatosAgenciaLiquidados(region, nombreRegion);
                    tabReportesFinales.SelectedIndex = 11;
                }
                finally
                {
                    lblCargandoInformacionRegionales.Visible = false;
                    Application.DoEvents();
                }

            }
        }

    }

    private void MuestraDatosAgenciaLiquidados(int region, string nombreRegion)
    {
        if (_estoyPintandoDatos)
            return; 
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<CreditosLiquidadosAgencias>? creditosRegion = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosLiquidadosRegion(region);
            lbllaEncabezado.Text = String.Format("Créditos Liquidados de la Region : {0}", nombreRegion);
            dgvLiquidadosAgencias.AutoGenerateColumns = false;
            _estoyPintandoDatos = true;
            try
            {                
                dgvLiquidadosAgencias.DataSource = creditosRegion;
            }
            finally
            {

                _estoyPintandoDatos = false;
            }
        }
    }

    private void ClickRegionLiquidados(object? sender, EventArgs e)
    {
        SeleccionaRegionLiquidados();
    }
}
