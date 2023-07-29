using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppConsultaImagen;

public partial class MainFRM
{

    private ExpedienteJuridicoDetalle? _detalleExpedienteJuridico = null;
    private IEnumerable<ExpedienteJuridicoDetalle>? _detalleExpedienteJuridicos;
    public void InicializaExpedienteJuridico()
    {
        #region Seleccion del Menu
        lblCobranzaJudicial.DoubleClick += SeleccionaCobranzaJudicial;
        lblDetalleCobranzaJudicial.DoubleClick += SeleccionaCobranzaJudicial;
        label42.DoubleClick += SeleccionaCobranzaJudicial;
        #endregion

        #region Navegación entre contratos en cobranza judicial de la region vs agencias 16
        dgvExpedientesJuridicosRegion.DoubleClick += ClickRegionCobranzaJudicial;
        btncjrSiguiente.Click += ClickRegionCobranzaJudicial;
        btncjrAnterior.Click += NavegaRegresoResumen;
        #endregion

        #region Navegación entre contratos en cobranza judicial de la agencia vs detalle de expedientes 17
        dgvExpedientesJuridicosAgencias.DoubleClick += ClickAgenciasCobranzaJudicial;
        btncjaAnterior.Click += ClickRegresaResumenCobranzaJudicial;
        btncjaSiguiente.Click += ClickAgenciasCobranzaJudicial;
        #endregion

        #region Navegación entre contratos en cobranza judicial de detalle de expedientes vs Imagenes 18
        dgvExpedientesJuridicosDetalle.DoubleClick += DgvExpedientesJuridicosCobranzaJudicial;
        dgvExpedientesJuridicosDetalle.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        btncjdAnterior.Click += NavegaRegresoAgencias;
        btncjdVerImagen.Click += ClickVerImagenCobranzaJudicial;
        #endregion


        #region Formatos y comportamiento de DataGridViews 16
        dgvExpedientesJuridicosRegion.CellFormatting += FormatoCeldasExpedientesJuridicosRegion;
        dgvExpedientesJuridicosAgencias.CellFormatting += FormatoCeldasExpedientesJuridicosAgencias;
        dgvExpedientesJuridicosDetalle.CellFormatting += FormatoCeldasExpedientesJuridicosDetalle;
        dgvExpedientesJuridicosDetalle.CellPainting += DgvExpedientesJuridicosDetalle_CellPainting;
        dgvExpedientesJuridicosDetalle.SelectionChanged += SeleccionaCreditoExpedientesJuridicosDetalle;
        #endregion
        if (_windowsFormsGloablInformation is null)
            return;
        _windowsFormsGloablInformation.ActivaConsultasServices().CargaExpedientesJuridicos();
        _windowsFormsGloablInformation.ActivaConsultasServices().CargaInformacionImagenesExpedientesJuridicos();

        lblDetalleCobranzaJudicial.Text = string.Format("{0:#,##0} contratos en cobranza judicial",_windowsFormsGloablInformation.ActivaConsultasServices().CalculaExpedientesJuridicos());
    }

    private void DgvExpedientesJuridicosDetalle_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != 7) // Ignore header row and columns
            return;

        if (dgvExpedientesJuridicosDetalle.Columns[e.ColumnIndex].DefaultCellStyle.WrapMode == DataGridViewTriState.True)
        {
            int newHeight = (int)(e.Graphics.MeasureString(e.Value.ToString(), dgvExpedientesJuridicosDetalle.Font, e.CellBounds.Width).Height);

            if (newHeight > 500) // Limit max height to 500
                dgvExpedientesJuridicosDetalle.Rows[e.RowIndex].Height = 500;
        }
    }

    private void DetalleExpedientesJuridicosCobranzaJudicial_SelectionChanged(object? sender, EventArgs e)
    {
        ActivaBotonVerImagenExpedientesJuridicos();
    }

    private void SeleccionaCreditoExpedientesJuridicosDetalle(object? sender, EventArgs e)
    {
        ActivaBotonVerImagenExpedientesJuridicos();
    }

    private void ActivaBotonVerImagenExpedientesJuridicos()
    {
        if (_estoyPintandoDatos)
            return;
        if (dgvExpedientesJuridicosDetalle.SelectedRows is not null && dgvExpedientesJuridicosDetalle.CurrentRow is not null)
        {

            bool tieneImagenDirecta = Convert.ToBoolean(dgvExpedientesJuridicosDetalle.CurrentRow.Cells[18].Value);
            bool tieneImagenInDirecta = Convert.ToBoolean(dgvExpedientesJuridicosDetalle.CurrentRow.Cells[19].Value);
            bool tieneImagenExpediente = Convert.ToBoolean(dgvExpedientesJuridicosDetalle.CurrentRow.Cells[20].Value);
            _estoyPintandoDatos = true;
            try
            {
                btncjdVerImagen.Visible = tieneImagenDirecta || tieneImagenInDirecta || tieneImagenExpediente;
            }
            finally
            {
                _estoyPintandoDatos = false;
            }
        }
    }

    private void FormatoCeldasExpedientesJuridicosDetalle(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            int region; // = 1; 

            if (e.RowIndex != -1)
            {
                region = Convert.ToInt32(dgvExpedientesJuridicosDetalle.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }

            if (e.ColumnIndex == 7) // Ignore header row and columns
            {
                if (dgvExpedientesJuridicosDetalle.Columns[e.ColumnIndex].DefaultCellStyle.WrapMode == DataGridViewTriState.True)
                {
                    string originalValue = Convert.ToString(e.Value) ?? "";
                    string[] creditos = originalValue.Split('\n');

                    if (creditos.Length > 15)
                    {
                        StringBuilder sb = new ();

                        for (int i = 0; i < 15; i++)
                        {
                            sb.Append(creditos[i]);
                            sb.Append('\n');
                        }

                        sb.Append("...");

                        originalValue = sb.ToString();
                    }

                    e.Value = originalValue;
                    e.FormattingApplied = true;
                    /*
                    int newHeight = (int)(e.Graphics.MeasureString(e.Value.ToString(), dgvExpedientesJuridicosDetalle.Font, e.CellBounds.Width).Height);

                    if (newHeight > 500) // Limit max height to 500
                        dgvExpedientesJuridicosDetalle.Rows[e.RowIndex].Height = 500;
                    */
                }
            }

            /*
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
                        */

        }
    }

    private void FormatoCeldasExpedientesJuridicosAgencias(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvExpedientesJuridicosAgencias.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                    //e.FormattingApplied = true;
                }
            }
        }
    }

    private void FormatoCeldasExpedientesJuridicosRegion(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgvExpedientesJuridicosRegion.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                    // e.FormattingApplied = true;
                }
            }
        }
    }

    private void ClickVerImagenCobranzaJudicial(object? sender, EventArgs e)
    {         
        VerImagenCobrazaJudicial();
    }

    private void NavegaRegresoAgencias(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 17;
    }

    private void DgvExpedientesJuridicosCobranzaJudicial(object? sender, EventArgs e)
    {
        VerImagenCobrazaJudicial();
    }

    private void VerImagenCobrazaJudicial()
    {
        if (btncjdVerImagen.Visible)
        {
            regresaTabNavegacion = 2;
            regresaTabReportesFinales = 18;
            datosRegreso.Push(new NavegacionRetorno()
            {
                TabPrincipal = regresaTabNavegacion,
                TabReporte = regresaTabReportesFinales
            });
            MuestraImagenesCobranzaJudicial();
        }
    }

    private void MuestraImagenesCobranzaJudicial()
    {
        if (dgvExpedientesJuridicosDetalle.SelectedRows is not null && dgvExpedientesJuridicosDetalle.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvExpedientesJuridicosDetalle.CurrentRow.Cells[1].Value);
            object objValue = dgvExpedientesJuridicosDetalle.CurrentRow.Cells[2].Value;
            object objValueCredito = dgvExpedientesJuridicosDetalle.CurrentRow.Cells[6].Value;
            string numeroDeContrato;
            string numeroDeCredito;
            if (objValue == null)
                numeroDeContrato = string.Empty;
            else
            {
                numeroDeContrato = Convert.ToString(objValue) ?? string.Empty;
            }
            if (objValueCredito == null)
                numeroDeCredito = string.Empty;
            else
            {
                numeroDeCredito = Convert.ToString(objValueCredito) ?? string.Empty;
            }
            if (agencia != 0 && !string.IsNullOrEmpty(numeroDeContrato))
            {
                // TODO: Cambiar a contrato en lugar de numero de crédito

                _expCasExpedienteNumCreditoSeleccionado = numeroDeCredito;

                ExpedienteJuridicoParaImagenes? expediente = BuscaImagenesExpedienteJuridico(numeroDeContrato, _expCasExpedienteNumCreditoSeleccionado);
                if (expediente is not null)
                {
                    char[] delimitadores = new char[] { ',', '¬', '\n' };
                    creditosRelacionadosBienesAdjudicados = (expediente?.Expediente.NumCreditos ?? "").Split(delimitadores);

                    BuscaImagenes(numeroDeCredito, false);
                    if (imagenesEncontradas is not null && !imagenesEncontradas.Any())
                    {
                        BuscaImagenes(numeroDeContrato[..14]+"0000", false);
                    }
                    if (expediente is not null)
                    {
                        MuestaInformacionJuridico(expediente);
                    }
                    NavegaVisorImagenesJuridico();                    
                }
            }
        }
    }

    private void ClickRegresaResumenCobranzaJudicial(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 16;
    }

    private void ClickAgenciasCobranzaJudicial(object? sender, EventArgs e)
    {
        SeleccionaAgenciaCobranzaJudicial();
    }

    private void SeleccionaAgenciaCobranzaJudicial()
    {
        if (dgvExpedientesJuridicosAgencias.SelectedRows is not null && dgvExpedientesJuridicosAgencias.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvExpedientesJuridicosAgencias.CurrentRow.Cells[0].Value);
            object objValue = dgvExpedientesJuridicosAgencias.CurrentRow.Cells[1].Value;
            string nombreAgencia;
            if (objValue == null)
                nombreAgencia = string.Empty;
            else
            {
                nombreAgencia = Convert.ToString(objValue) ?? string.Empty;
            }
            if (agencia != 0)
            {

                MuestraDetalleAgenciaCobranzaJudicial(agencia, nombreAgencia);

                tabReportesFinales.SelectedIndex = 18;
            }
        }
    }

    private void MuestraDetalleAgenciaCobranzaJudicial(int agencia, string nombreAgencia)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            _detalleExpedienteJuridicos = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaExpedienteJuridicoDetalle(agencia);
            lblExpedientesJuridicosDetalle.Text = String.Format("Cobranza judicial - Expedientes de las Agencias de la Agencia {0}", nombreAgencia);
            dgvExpedientesJuridicosDetalle.AutoGenerateColumns = false;
            if (_detalleExpedienteJuridicos is not null)
            {
                ExpedienteJuridicoDetalle? total = _detalleExpedienteJuridicos.Where(x => x.Region == 0 && x.Agencia == 0).FirstOrDefault();
            }
            dgvExpedientesJuridicosDetalle.DataSource = _detalleExpedienteJuridicos;
        }
    }

    private void ClickRegionCobranzaJudicial(object? sender, EventArgs e)
    {
        SeleccionaRegionCobranzaJudicial();
    }

    public void SeleccionaRegionCobranzaJudicial()
    {
        if (dgvExpedientesJuridicosRegion.SelectedRows is not null && dgvExpedientesJuridicosRegion.CurrentRow is not null)
        {
            int region = Convert.ToInt32(dgvExpedientesJuridicosRegion.CurrentRow.Cells[0].Value);
            object objValue = dgvExpedientesJuridicosRegion.CurrentRow.Cells[1].Value;
            string nombreRegion;
            if (objValue == null)
                nombreRegion = string.Empty;
            else
            {
                nombreRegion = Convert.ToString(objValue) ?? string.Empty;
            }
            if (region != 0)
            {
                MuestraDatosAgenciaCobranzaJudicial(region, nombreRegion);
                tabReportesFinales.SelectedIndex = 17;
            }
        }

    }

    private void MuestraDatosAgenciaCobranzaJudicial(int region, string nombreRegion)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<ExpedientesJuridicoAgencia>? expedientesJuridicosAgencia = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaExpedientesJuridicoAgencia(region);
            lblcjaTitulo.Text = String.Format("Cobranza judicial - Expedientes de las Agencias de la Región {0}", nombreRegion);
            dgvExpedientesJuridicosAgencias.AutoGenerateColumns = false;
            dgvExpedientesJuridicosAgencias.DataSource = expedientesJuridicosAgencia;
        }
    }

    private void SeleccionaCobranzaJudicial(object? sender, EventArgs e)
    {
        ObtieneInformacionExpedientesJuridicos();
    }

    private void ObtieneInformacionExpedientesJuridicos()
    {
        if (_estoyPintandoDatos)
            return;

        IEnumerable<ExpedientesJuridicoRegion>? expedientesJuridicosRegion;
        if (_windowsFormsGloablInformation is not null)
        {
            expedientesJuridicosRegion = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaExpedientesJuridicoRegiones();
            _estoyPintandoDatos = true;
            try
            {
                dgvExpedientesJuridicosRegion.DataSource = expedientesJuridicosRegion;
            }
            finally
            {
                _estoyPintandoDatos = false;
            }
        }
        tabReportesFinales.SelectedIndex = 16;
    }
}
