using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace AppConsultaImagen;

/// <summary>
/// Aqui va la codificación del listado de agencias de una region
/// </summary>
public partial class MainFRM
{
    private void InicializaReporteFinalInicializacionAgencias() {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).

        dgvraAgencias.CellDoubleClick += DgvraAgencias_CellDoubleClick;
        btnraSiguiente.Click += BtnRaSiguiente;



        btnrdAnterior.Click += BtnAnteriorRd;
        btnrdVerImagen.Click += BtnMuestraImagenRd;

        dgvrdDetalleAgencias.CellDoubleClick += DgvrdDetalleAgencias_CellDoubleClick;
        dgvrdDetalleAgencias.SelectionChanged += DgvrdDetalleAgencias_SelectionChange;
        dgvrdDetalleAgencias.CellFormatting += DgvrdDetalleAgencias_CellFormatting;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }

    private void DgvrdDetalleAgencias_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        int region = 1;

        if (e.RowIndex != -1)
        {
            region = Convert.ToInt32(dgvrdDetalleAgencias.Rows[e.RowIndex].Cells[0].Value);
            if (region == 0)
            {
                e.CellStyle.BackColor = Color.Black;
                e.CellStyle.ForeColor = Color.White;
            }
        }
        if (e.RowIndex != -1 && (e.ColumnIndex >= 9 && e.ColumnIndex <= 12))
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
                    case 9: 
                        e.Value = string.Format("{0:#,###}", totalVigentes);
                        break;
                    case 10:
                        e.Value = string.Format("{0:#,###}", totalImpago);
                        break;
                    case 11:
                        e.Value = string.Format("{0:#,###}", totalVencida);
                        break;
                    case 12:
                        e.Value = string.Format("{0:#,###}", totalOrigenDelDr);
                        break;
                    default:
                        break;
                }
            }
            /*
                 int totalVigenges = 0;
    int totalImpago = 0;
    int totalVencida = 0;
    int totalOrigenDelDr = 0;
*/ 
             

            e.FormattingApplied = true;
        }
    }

    private void DgvraAgencias_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        SeleccionaAgencia();
    }

    private void DgvrdDetalleAgencias_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        regresaTabNavegacion = 2;
        regresaTabReportesFinales = 3;
        if (dgvrdDetalleAgencias.SelectedRows is not null && dgvrdDetalleAgencias.CurrentRow is not null)
        {
            object objValue = dgvrdDetalleAgencias.CurrentRow.Cells[2].Value;
            string numeroDeCredito;
            if (objValue == null)
                numeroDeCredito = string.Empty;
            else
            {
                numeroDeCredito = Convert.ToString(objValue) ?? string.Empty;
                bool tieneImagenDirecta = Convert.ToBoolean(dgvrdDetalleAgencias.CurrentRow.Cells[13].Value);
                bool tieneImagenInDirecta = Convert.ToBoolean(dgvrdDetalleAgencias.CurrentRow.Cells[14].Value);
                if (tieneImagenDirecta || tieneImagenInDirecta) {
                    MuestraImagenRd();
                }
            }
            Clipboard.SetText(numeroDeCredito);
        }
    }

    private void DgvrdDetalleAgencias_SelectionChange(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (dgvrdDetalleAgencias.SelectedRows is not null && dgvrdDetalleAgencias.CurrentRow is not null)
            {
                bool tieneImagenDirecta = Convert.ToBoolean(dgvrdDetalleAgencias.CurrentRow.Cells[13].Value);
                bool tieneImagenInDirecta = Convert.ToBoolean(dgvrdDetalleAgencias.CurrentRow.Cells[14].Value);
                btnrdVerImagen.Visible = tieneImagenDirecta || tieneImagenInDirecta;
            }
        }

    }

    private void MuestraDatosAgencia(int region, string nombreRegion)
    {
        
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<CreditosRegion>? creditosRegion = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosRegion(region);
            lblraRegion.Text = String.Format("Agencias de la region {0}", nombreRegion);
            dgvraAgencias.AutoGenerateColumns = false;
            dgvraAgencias.DataSource = creditosRegion;
        }
    }
    private void BtnRaAnterior(object sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 1;
    }

    private void BtnRaSiguiente(object sender, EventArgs e)
    {
        SeleccionaAgencia();
        // Falta seleccionar el renglon
    }

    private void SeleccionaAgencia()
    {
        if (dgvraAgencias.SelectedRows is not null && dgvraAgencias.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvraAgencias.CurrentRow.Cells[0].Value);
            object objValue = dgvraAgencias.CurrentRow.Cells[1].Value;
            string nombreAgencia;
            if (objValue == null)
                nombreAgencia = string.Empty;
            else
            {
                nombreAgencia = Convert.ToString(objValue) ?? string.Empty;
            }
            if (agencia != 0)
            {

                MuestraDetalleAgencia(agencia, nombreAgencia);

                tabReportesFinales.SelectedIndex = 3;
            }
        }
    }

    int totalVigentes = 0;
    int totalImpago = 0;
    int totalVencida = 0;
    int totalOrigenDelDr = 0;
    private void MuestraDetalleAgencia(int agencia, string nombreAgencia)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            IEnumerable<DetalleCreditos>? detalleCreditos = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosAgencia(agencia);
            lblrdDetalleAgencia.Text = String.Format("Creditos de la Agencia {0}", nombreAgencia);
            colrdSaldoCarteraImpagos.DataPropertyName = "SaldoCarteraImpagos";
            dgvrdDetalleAgencias.AutoGenerateColumns = false;
            if (detalleCreditos is not null)
            {
                DetalleCreditos? total = detalleCreditos.Where(x => x.Region == 0 && x.Agencia == 0).FirstOrDefault();
                if (total is not null)
                {
                    
                    totalVigentes = detalleCreditos.Count(x => x.EsVigente == true);
                    totalImpago = detalleCreditos.Count(x => x.EsImpago == true);
                    totalVencida = detalleCreditos.Count(x => x.EsVencida == true);
                    totalOrigenDelDr = detalleCreditos.Count(x => x.EsOrigenDelDr == true);                    
                }
            }
            dgvrdDetalleAgencias.DataSource = detalleCreditos;
        }
    }

    private void BtnAnteriorRd(object sender, EventArgs e)
    {

        tabReportesFinales.SelectedIndex = 2;
    }

    private void BtnMuestraImagenRd(object sender, EventArgs e)
    {
        regresaTabNavegacion = 2;
        regresaTabReportesFinales = 3;
        MuestraImagenRd();
    }

    private void MuestraImagenRd()
    {
        // throw new NotImplementedException();

        if (dgvrdDetalleAgencias.SelectedRows is not null && dgvrdDetalleAgencias.CurrentRow is not null)
        {
            int agencia = Convert.ToInt32(dgvrdDetalleAgencias.CurrentRow.Cells[1].Value);
            object objValue = dgvrdDetalleAgencias.CurrentRow.Cells[2].Value;
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
                ExpedienteDeConsulta? expediente = BuscaImagenes(_expCasExpedienteNumCreditoSeleccionado);
                if (expediente is not null)
                {
                    MuestraExpedienteEnImagen(expediente);
                    NavegaVisorImagenes();
                    tabReportesFinales.SelectedIndex = 0;
                }
            }
        }




    }

    private ExpedienteDeConsulta? BuscaImagenes(string numeroDeCredito, bool directas = true)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            imagenesEncontradas = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaImagenes(numeroDeCredito, directas).ToArray();
            var resultado = BuscaPorNumeroDeCredito(numeroDeCredito).OrderByDescending(x => x.NumCredito).ToList();
            return resultado.FirstOrDefault();
        }
        return null;
    }
}

