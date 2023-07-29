using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.Tratamientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppConsultaImagen;

public partial class MainFRM
{
    protected void InicializaTratamientos()
    {
        #region Seleccion del Menú los tratamientos
        lblTratamientosExplicacion.DoubleClick += CambiaTabTratamientosRegion;
        lblConteoTratamientosDetalle.DoubleClick += CambiaTabTratamientosRegion;
        label44.DoubleClick += CambiaTabTratamientosRegion;
        #endregion

        #region Navegación entre tratamientos resumen a region 13
        dgvTratamientosRegion.DoubleClick += ClickRegionTratamientos;
        btntrSiguiente.Click += ClickRegionTratamientos;
        btntrAnterior.Click += NavegaRegresoResumen;
        #endregion

        #region Navegación entre tratamientos resumen a agencias 14
        dgvTratamientosAgencias.DoubleClick += ClickAgenciaTratamientos;
        btntaSiguiente.Click += ClickAgenciaTratamientos;
        btntaAnterior.Click += ClickRegresaResumenTratamientos;
        #endregion

        #region Navegación entre tratamientos agencia a detalle 15
        dgvTratamientosDetalle.DoubleClick += ClickVerImagenCreditoTratamiento;
        dgvTratamientosDetalle.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        btntdImagen.Click += ClickVerImagenCreditoTratamiento;
        btntdAnterior.Click += ClickRegresaAgenciaTratamientos;
        #endregion

        #region Formatos y comportamiento de DataGridViews 16
        dgvTratamientosRegion.CellFormatting += FormatoCeldasTratamientosRegion;
        dgvTratamientosAgencias.CellFormatting += FormatoCeldasTratamientosAgencias;
        dgvTratamientosDetalle.CellFormatting += FormatoCeldasTratamientosDetalle;
        dgvTratamientosDetalle.SelectionChanged += SeleccionaCreditoTratamientoDetalle;
        #endregion
        if (_windowsFormsGloablInformation is null)
            return;

        int cantidadTratamientos = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaTratamientos();
        lblConteoTratamientosDetalle.Text = string.Format("{0:#,##0} Tratamientos activos", cantidadTratamientos);
    }

    protected void FormatoCeldasTratamientosRegion(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvTratamientosRegion.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                    // e.FormattingApplied = true;
                }
            }
        }
    }

    protected void FormatoCeldasTratamientosAgencias(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvTratamientosAgencias.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                    //e.FormattingApplied = true;
                }
            }
        }
    }

    protected void FormatoCeldasTratamientosDetalle(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            int region = 1;

            if (e.RowIndex != -1)
            {
                region = Convert.ToInt32(dgvTratamientosDetalle.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
            if (e.RowIndex != -1 && (e.ColumnIndex >= 10 && e.ColumnIndex <= 13))
            {
                if (region != 0)
                {
                    bool originalValue = Convert.ToBoolean(e.Value);
                    if (originalValue)
                    {
                        e.Value = "X";
                    }
                    else
                    {
                        e.Value = " ";
                    }
                }
                else
                {
                    switch (e.ColumnIndex)
                    {
                        case 10:
                            e.Value = string.Format("{0:#,###}", totalVigentes);
                            break;
                        case 11:
                            e.Value = string.Format("{0:#,###}", totalImpago);
                            break;
                        case 12:
                            e.Value = string.Format("{0:#,###}", totalVencida);
                            break;
                        case 13:
                            e.Value = string.Format("{0:#,###}", totalOrigenDelDr);
                            break;
                        default:
                            break;
                    }
                }
                e.FormattingApplied = true;
            }
        }
    }

    protected void SeleccionaCreditoTratamientoDetalle(object? sender, EventArgs e)
    {
        ActivaBotonVerImagenTratamientos();
    }

    protected void ActivaBotonVerImagenTratamientos()
    {
        if (_estoyPintandoDatos)
            return;
        if (dgvTratamientosDetalle.SelectedRows is not null && dgvTratamientosDetalle.CurrentRow is not null)
        {

            bool tieneImagenDirecta = Convert.ToBoolean(dgvTratamientosDetalle.CurrentRow.Cells[14].Value);
            bool tieneImagenInDirecta = Convert.ToBoolean(dgvTratamientosDetalle.CurrentRow.Cells[15].Value);
            _estoyPintandoDatos = true;
            try
            {
                btntdImagen.Visible = tieneImagenDirecta || tieneImagenInDirecta;
            }
            finally
            {
                _estoyPintandoDatos = false;
            }
        }
    }

    protected void ClickRegionTratamientos(object? sender, EventArgs e)
    {
        SeleccionaRegionTratamientos();
    }

    protected void SeleccionaRegionTratamientos() 
    {
        if (dgvTratamientosRegion.SelectedRows is not null && dgvTratamientosRegion.CurrentRow is not null)
        {
            int region = Convert.ToInt32(dgvTratamientosRegion.CurrentRow.Cells[0].Value);
            object objValue = dgvTratamientosRegion.CurrentRow.Cells[1].Value;
            string nombreRegion;
            if (objValue == null)
                nombreRegion = string.Empty;
            else
            {
                nombreRegion = Convert.ToString(objValue) ?? string.Empty;
            }
            if (region != 0)
            {
                MuestraDatosAgenciaTratamientos(region, nombreRegion);
                tabReportesFinales.SelectedIndex = 14;
            }
        }
    }

    private void MuestraDatosAgenciaTratamientos(int region, string nombreRegion)
    {

        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<CreditosRegion>? creditosRegion = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosRegionTratamientos(region);
            lbltaRegion.Text = String.Format("Agencias de la region {0}", nombreRegion);
            dgvTratamientosAgencias.AutoGenerateColumns = false;
            dgvTratamientosAgencias.DataSource = creditosRegion;
        }
    }

    protected void ClickAgenciaTratamientos(object? sender, EventArgs e)
    {
        SeleccionaAgenciaTratamientos();
    }

    protected void SeleccionaAgenciaTratamientos()
    {
        if (dgvTratamientosAgencias.SelectedRows is not null && dgvTratamientosAgencias.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvTratamientosAgencias.CurrentRow.Cells[0].Value);
            object objValue = dgvTratamientosAgencias.CurrentRow.Cells[1].Value;
            string nombreAgencia;
            if (objValue == null)
                nombreAgencia = string.Empty;
            else
            {
                nombreAgencia = Convert.ToString(objValue) ?? string.Empty;
            }
            if (agencia != 0)
            {

                MuestraDetalleAgenciaTratamientos(agencia, nombreAgencia);

                tabReportesFinales.SelectedIndex = 15;
            }
        }

    }


    private void MuestraDetalleAgenciaTratamientos(int agencia, string nombreAgencia)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<DetalleCreditosTratamientos>? detalleCreditos = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosAgenciaTratamientos(agencia);
            lbltdAgencia.Text = String.Format("Creditos de la Agencia {0}", nombreAgencia);
            dataGridViewTextBoxColumn40.DataPropertyName = "SaldoCarteraImpagos";
            dgvTratamientosDetalle.AutoGenerateColumns = false;
            if (detalleCreditos is not null)
            {
                DetalleCreditosTratamientos? total = detalleCreditos.Where(x => x.Region == 0 && x.Agencia == 0).FirstOrDefault();
                if (total is not null)
                {

                    totalVigentes = detalleCreditos.Count(x => x.EsVigente == true);
                    totalImpago = detalleCreditos.Count(x => x.EsImpago == true);
                    totalVencida = detalleCreditos.Count(x => x.EsVencida == true);
                    totalOrigenDelDr = detalleCreditos.Count(x => x.EsOrigenDelDr == true);
                }
            }
            dgvTratamientosDetalle.DataSource = detalleCreditos;
        }
    }

    protected void CambiaTabTratamientosRegion(object? sender, EventArgs e)
    {
        ObtieneInformacionTratamientos();
    }

    protected void ClickRegresaResumenTratamientos(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 13;
    }

    protected void ClickVerImagenCreditoTratamiento(object? sender, EventArgs e)
    {
        if (btntdImagen.Visible)
        {
            regresaTabNavegacion = 2;
            regresaTabReportesFinales = 15;
            datosRegreso.Push(new NavegacionRetorno()
            {
                TabPrincipal = regresaTabNavegacion,
                TabReporte = regresaTabReportesFinales
            });
            MuestraImagenRdTratamiento();
        }
    }

    private void MuestraImagenRdTratamiento()
    {
        // throw new NotImplementedException();

        if (dgvTratamientosDetalle.SelectedRows is not null && dgvTratamientosDetalle.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvTratamientosDetalle.CurrentRow.Cells[1].Value);
            object objValue = dgvTratamientosDetalle.CurrentRow.Cells[2].Value;
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
                if (expediente is not null)
                {
                    MuestraExpedienteEnImagen(expediente);
                    NavegaVisorImagenes();
                    // tabReportesFinales.SelectedIndex = 0;
                }
            }
        }
    }
    protected void ClickRegresaAgenciaTratamientos(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 14;
    }

    private void ObtieneInformacionTratamientos() 
    {
        if (_estoyPintandoDatos)
            return;

        IEnumerable<ResumenDeCreditos>? creditosRegionales;
        if (_windowsFormsGloablInformation is not null)
        {
            creditosRegionales = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosRegionesTratamientos();
            _estoyPintandoDatos = true;
            try
            {
                dgvTratamientosRegion.DataSource = creditosRegionales;
            }
            finally
            {
                _estoyPintandoDatos = false;
            }
        }
        tabReportesFinales.SelectedIndex = 13;
    }
}
