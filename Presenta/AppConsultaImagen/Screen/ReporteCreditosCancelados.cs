using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM : Form
{
    IEnumerable<CreditosCanceladosAplicacion>? _creditosCancelados;

    private void InicializaCreditosCancelados()
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        lblCreditosCancelados.DoubleClick += CcDobleClick;
        lblDetalleCancelados.DoubleClick += CcDobleClick;
        label41.DoubleClick += CcDobleClick;


        btnccrSiguiente.Click += ClickRegionCancelados;
        dgbccResumenCreditosCancelados.DoubleClick += ClickRegionCancelados;
        btnccrAnterior.Click += NavegaRegresoResumen;


        btnccxrAnterior.Click += NavegaAnteriorCreditosCanceladoRegion;
        btnccxrSiguiente.Click += NavegaClickAgenciaCancelados;
        dgvCreditosCanceladosPorRegion.DoubleClick += NavegaClickAgenciaCancelados;


        btnccxdAnterior.Click += NavegaClickDetalleAgenciasCanceladosAnterior;
        btnccxdVerImagen.Click += VerImagenCancelados;
        dgvCreditosCanceladosDetalle.CellDoubleClick += VerImagenCancelados;

        dgbccResumenCreditosCancelados.CellFormatting += RegionesCancelados_CellFormatting;
        dgvCreditosCanceladosPorRegion.CellFormatting += AgenciasCancelados_CellFormatting;
        dgvCreditosCanceladosDetalle.CellFormatting += DetalleCancelados_CellFormatting;
        dgvCreditosCanceladosDetalle.SelectionChanged += DetalleCancelados_SelectionChanged;

#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        if (_creditosCancelados is null && _windowsFormsGloablInformation is not null)
        {
            _creditosCancelados = _windowsFormsGloablInformation.ActivaConsultasServices().CargaCreditosCancelados();
        }
        else
            if (_windowsFormsGloablInformation is null)
            return;
        int cantidadDeCreditosCancelados = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosCancelados();
        lblDetalleCancelados.Text = string.Format("{0:#,##0} Créditos Cancelados", cantidadDeCreditosCancelados);
    }


    private void DetalleCancelados_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex != -1)
        {
            int region = Convert.ToInt32(dgvCreditosCanceladosDetalle.Rows[e.RowIndex].Cells[0].Value);
            if (region == 0)
            {
                e.CellStyle.BackColor = Color.Black;
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }

    private void VerImagenCancelados(object? sender, EventArgs e)
    {
        regresaTabNavegacion = 2;
        regresaTabReportesFinales = 6;
        datosRegreso.Push(new NavegacionRetorno()
        {
            TabPrincipal = regresaTabNavegacion,
            TabReporte = regresaTabReportesFinales
        });
        MuestraImagenRdCancelados();
    }

    private void NavegaClickDetalleAgenciasCanceladosAnterior(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 5;
    }

    private void NavegaClickAgenciaCancelados(object? sender, EventArgs e)
    {
        SeleccionaAgenciaCancelados();
    }

    private void NavegaAnteriorCreditosCanceladoRegion(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 4;
    }

    private void NavegaRegresoResumen(object? sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 0;
    }

    private void ClickRegionCancelados(object? sender, EventArgs e)
    {
        SeleccionaRegionCancelados();
    }

    private void CcDobleClick(object sender, EventArgs e)
    {
        ObtieneInformacionCancelados();
    }

    private void ObtieneInformacionCancelados()
    {
        if (_creditosCancelados is null && _windowsFormsGloablInformation is not null)
        {
            _creditosCancelados = _windowsFormsGloablInformation.ActivaConsultasServices().CargaCreditosCancelados();
        }
        else
            if (_windowsFormsGloablInformation is null)
            return;

        IEnumerable<CreditosCanceladosResumen> listaCreditosCanceladosResumen = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosCanceladosRegiones();
        dgbccResumenCreditosCancelados.DataSource = listaCreditosCanceladosResumen;
        /*        
    IEnumerable<CreditosCanceladosRegion> CalculaCreditosCanceladosRegion(int region);
    IEnumerable<CreditosCanceladosDetalle> CalculaCreditosCanceladosAgencia(int agencia);
        */
        tabReportesFinales.SelectedIndex = 4;
    }

    private void SeleccionaRegionCancelados()
    {
        if (dgbccResumenCreditosCancelados.SelectedRows is not null && dgbccResumenCreditosCancelados.CurrentRow is not null)
        {
            int region = Convert.ToInt32(dgbccResumenCreditosCancelados.CurrentRow.Cells[0].Value);
            object objValue = dgbccResumenCreditosCancelados.CurrentRow.Cells[1].Value;
            string nombreRegion;
            if (objValue == null)
                nombreRegion = string.Empty;
            else
            {
                nombreRegion = Convert.ToString(objValue) ?? string.Empty;
            }
            if (region != 0)
            {
                MuestraDatosAgenciaCancelados(region, nombreRegion);
                tabReportesFinales.SelectedIndex = 5;
            }
        }


    }

    private void MuestraDatosAgenciaCancelados(int region, string nombreRegion)
    {

        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<CreditosCanceladosRegion>? creditosRegion = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosCanceladosRegion(region);
            lblccxRegion.Text = String.Format("Créditos Cancelados de la Region : {0}", nombreRegion);
            dgvCreditosCanceladosPorRegion.AutoGenerateColumns = false;
            dgvCreditosCanceladosPorRegion.DataSource = creditosRegion;
        }
    }

    private void SeleccionaAgenciaCancelados()
    {
        if (dgvCreditosCanceladosPorRegion.SelectedRows is not null && dgvCreditosCanceladosPorRegion.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvCreditosCanceladosPorRegion.CurrentRow.Cells[0].Value);
            object objValue = dgvCreditosCanceladosPorRegion.CurrentRow.Cells[1].Value;
            string nombreAgencia;
            if (objValue == null)
                nombreAgencia = string.Empty;
            else
            {
                nombreAgencia = Convert.ToString(objValue) ?? string.Empty;
            }
            if (agencia != 0)
            {

                MuestraDetalleAgenciaCancelados(agencia, nombreAgencia);

                tabReportesFinales.SelectedIndex = 6;
            }
        }
    }

    private void MuestraDetalleAgenciaCancelados(int agencia, string nombreAgencia)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<CreditosCanceladosDetalle>? detalleCreditos = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosCanceladosAgencia(agencia);
            lblccdxAgencia.Text = String.Format("Creditos de la Agencia {0}", nombreAgencia);
            //colrdSaldoCarteraImpagos.DataPropertyName = "SaldoCarteraImpagos";
            dgvCreditosCanceladosDetalle.AutoGenerateColumns = false;
            if (detalleCreditos is not null)
            {
                CreditosCanceladosDetalle? total = detalleCreditos.Where(x => x.Region == 0 && x.Agencia == 0).FirstOrDefault();
                if (total is not null)
                {
                    /*
                    totalCeditos 
                    totalVigentes = detalleCreditos.Count(x => x.EsVigente == true);
                    totalImpago = detalleCreditos.Count(x => x.EsImpago == true);
                    totalVencida = detalleCreditos.Count(x => x.EsVencida == true);
                    totalOrigenDelDr = detalleCreditos.Count(x => x.EsOrigenDelDr == true);
                    */
                }
            }
            dgvCreditosCanceladosDetalle.DataSource = detalleCreditos;
            ActivaBotonVerImagenCanceladas();
        }
    }

    private void MuestraImagenRdCancelados()
    {
        // throw new NotImplementedException();

        if (dgvCreditosCanceladosDetalle.SelectedRows is not null && dgvCreditosCanceladosDetalle.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvCreditosCanceladosDetalle.CurrentRow.Cells[1].Value);
            object objValue = dgvCreditosCanceladosDetalle.CurrentRow.Cells[2].Value;
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
                    MuestraExpedienteEnImagen(expediente);
                    NavegaVisorImagenes();
                    tabReportesFinales.SelectedIndex = 0;
                }
            }
        }




    }

    private void RegionesCancelados_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex != -1)
        {
            int region = Convert.ToInt32(dgbccResumenCreditosCancelados.Rows[e.RowIndex].Cells[0].Value);
            if (region == 0)
            {
                e.CellStyle.BackColor = Color.Black;
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }
    private void AgenciasCancelados_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex != -1)
        {
            int agencia = Convert.ToInt32(dgvCreditosCanceladosPorRegion.Rows[e.RowIndex].Cells[0].Value);
            if (agencia == 0)
            {
                e.CellStyle.BackColor = Color.Black;
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }

    /*
    private bool BuscaImagenesCanceladas(string numeroDeCredito, bool directas = true)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            imagenesEncontradas = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaImagenes(numeroDeCredito, directas).ToArray();
            _informacionDeConsulta = BuscaPorNumeroDeCredito(numeroDeCredito).OrderByDescending(x => x.NumCredito).ToList();
            return imagenesEncontradas.Any();
        }
        return false;
    }
    */

    private void ActivaBotonVerImagenCanceladas() {
        if (dgvCreditosCanceladosDetalle.SelectedRows is not null && dgvCreditosCanceladosDetalle.CurrentRow is not null)
        {

            bool tieneImagenDirecta = Convert.ToBoolean(dgvCreditosCanceladosDetalle.CurrentRow.Cells[18].Value);
            bool tieneImagenInDirecta = Convert.ToBoolean(dgvCreditosCanceladosDetalle.CurrentRow.Cells[19].Value);
            btnccxdVerImagen.Visible = tieneImagenDirecta || tieneImagenInDirecta;
        }
    }

    private void DetalleCancelados_SelectionChanged(object? sender, EventArgs e)
    {
        ActivaBotonVerImagenCanceladas();
    }
}
