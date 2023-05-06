using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen
{
    public partial class MainFRM : Form
    {
        private DetalleBienesAdjudicados? _detalleBienAdjudicado = null;
        private IEnumerable<DetalleBienesAdjudicados>? _detalleBienesAdjudicados;

        private void InicializaBusquedaBienesAdjudicados() 
        {
            if (_windowsFormsGloablInformation is not null)
            {
                int cantidadBienesAdjudicados = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaBienesAdjudicados();
                lblMenuDetalleBienesAdjudicados.Text = string.Format(" {0:#,##0} Bienes", cantidadBienesAdjudicados);
            }
//#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
            lblMenuBienesAdjudicados.DoubleClick += EntraBienesAdjudicados;
            pnlMenuBienesAdjudicados.DoubleClick += EntraBienesAdjudicados;
            lblMenuDetalleBienesAdjudicados.DoubleClick += EntraBienesAdjudicados;

            btnbarAnterior.Click += ClickRegresaBarAnterior;
            btnbarSiguiente.Click += ClickContinuaBarSiguiente;
            dgbaResumenBienesAdjudicados.DoubleClick += BaNavegaResumenAEntidad;

            btnbaeAnterior.Click += ClickRegresaBaeAnterior;
            btnbaeSiguiente.Click += ClickContinuaBaeSiguiente;
            dgbaBienesAdjudicadosxRegion.DoubleClick += BaeNavegaEntidadADetalle;

            btnbadAnterior.Click += ClickRegresaBadAnterior;
            btnbadSiguiente.Click += ClickVerImagenBadSiguiente;
            dgbaDetalleBienesAdjudicados.CellDoubleClick += BadNavegaEntidadVerImagen;

            dgbaResumenBienesAdjudicados.CellFormatting += CellFormatingResumenBienesAdjudicados;
            dgbaBienesAdjudicadosxRegion.CellFormatting += CellFormatingBienesAdjudicadosxRegion;
            dgbaDetalleBienesAdjudicados.CellFormatting += CellFormatingDetalleBienesAdjudicados;
            dgbaDetalleBienesAdjudicados.SelectionChanged += DgbaDetalleBienesAdjudicados_SelectionChanged;
            //#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).        
        }

        private void DgbaDetalleBienesAdjudicados_SelectionChanged(object? sender, EventArgs e)
        {
            ActivaBotonVerImagenBienesAdjudicados();
        }

        private void CellFormatingDetalleBienesAdjudicados(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgbaDetalleBienesAdjudicados.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void CellFormatingBienesAdjudicadosxRegion(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgbaBienesAdjudicadosxRegion.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void CellFormatingResumenBienesAdjudicados(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int region = Convert.ToInt32(dgbaResumenBienesAdjudicados.Rows[e.RowIndex].Cells[0].Value);
                if (region == 0)
                {
                    e.CellStyle.BackColor = Color.Black;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void BadNavegaEntidadVerImagen(object? sender, EventArgs e)
        {
            regresaTabNavegacion = 2;
            regresaTabReportesFinales = 9;

            MuestraImagenBienAdjudicado();
        }

        private void MuestraImagenBienAdjudicado()
        {
            if (dgbaDetalleBienesAdjudicados.SelectedRows is not null && dgbaDetalleBienesAdjudicados.CurrentRow is not null)
            {
                string claveBien = Convert.ToString(dgbaDetalleBienesAdjudicados.CurrentRow.Cells[3].Value) ?? string.Empty;
                string numeroDeExpediente = Convert.ToString(dgbaDetalleBienesAdjudicados.CurrentRow.Cells[4].Value) ?? string.Empty;
                string acreditados = Convert.ToString(dgbaDetalleBienesAdjudicados.CurrentRow.Cells[5].Value) ?? string.Empty;
                _detalleBienAdjudicado = _detalleBienesAdjudicados?.FirstOrDefault(x => (x.CveBien ?? "").Equals(claveBien) && (x.ExpedienteElectronico ?? "").Equals(numeroDeExpediente));
                BuscaImagenesBa(numeroDeExpediente, acreditados);
                canNavigate = true;
                try
                {
                    tabNavegacion.SelectedIndex = 5;
                    tabReportesFinales.SelectedIndex = 0;
                }
                finally
                {
                    canNavigate = false;
                }
            }
        }

        private void ClickVerImagenBadSiguiente(object? sender, EventArgs e)
        {
            regresaTabNavegacion = 2;
            regresaTabReportesFinales = 9;

            MuestraImagenBienAdjudicado();
        }

        private void ClickRegresaBadAnterior(object? sender, EventArgs e)
        {
            tabReportesFinales.SelectedIndex = 8;
        }

        private void BaeNavegaEntidadADetalle(object? sender, EventArgs e)
        {
            SeleccionaEntidadBienesAdjudicados();
        }
        
        private void ClickContinuaBaeSiguiente(object? sender, EventArgs e)
        {
            SeleccionaEntidadBienesAdjudicados();
        }

        private void SeleccionaEntidadBienesAdjudicados()
        {
            if (dgbaBienesAdjudicadosxRegion.SelectedRows is not null && dgbaBienesAdjudicadosxRegion.CurrentRow is not null)
            {
                int region = Convert.ToInt32(dgbaBienesAdjudicadosxRegion.CurrentRow.Cells[0].Value);
                object objValue = dgbaBienesAdjudicadosxRegion.CurrentRow.Cells[1].Value;
                string nombreEntidad;
                if (objValue == null)
                    nombreEntidad = string.Empty;
                else
                {
                    nombreEntidad = Convert.ToString(objValue) ?? string.Empty;
                }
                if (region != 0)
                {

                    MuestraDetalleBienesAdjudicados(region, nombreEntidad);

                    tabReportesFinales.SelectedIndex = 9;
                }
            }
        }

        private void MuestraDetalleBienesAdjudicados(int region, string nombreEntidad)
        {
            if (_windowsFormsGloablInformation is not null)
            {
                _detalleBienesAdjudicados = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaDetalleBienesAdjudicados(region, nombreEntidad);
                lblbaDetalleRegionEntidad.Text = String.Format("Bienes Adjudicados de la Region {0} y de la Entidad {1}", region, nombreEntidad);
                //colrdSaldoCarteraImpagos.DataPropertyName = "SaldoCarteraImpagos";
                dgbaDetalleBienesAdjudicados.AutoGenerateColumns = false;
                if (_detalleBienesAdjudicados is not null)
                {
                    DetalleBienesAdjudicados? total = _detalleBienesAdjudicados.Where(x => x.NumRegion == 0 && (x.Entidad??"").Equals("Total", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
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
                dgbaDetalleBienesAdjudicados.DataSource = _detalleBienesAdjudicados;
                ActivaBotonVerImagenBienesAdjudicados();
            }
        }

        private void ActivaBotonVerImagenBienesAdjudicados()
        {
            if (dgbaDetalleBienesAdjudicados.SelectedRows is not null && dgbaDetalleBienesAdjudicados.CurrentRow is not null)
            {
                /*
                bool tieneImagenDirecta = Convert.ToBoolean(dgbaDetalleBienesAdjudicados.CurrentRow.Cells[18].Value);
                bool tieneImagenInDirecta = Convert.ToBoolean(dgbaDetalleBienesAdjudicados.CurrentRow.Cells[19].Value);
                btnbadSiguiente.Visible = tieneImagenDirecta || tieneImagenInDirecta;
                */
            }
        }

        private void ClickRegresaBaeAnterior(object? sender, EventArgs e)
        {
            tabReportesFinales.SelectedIndex = 7;
        }

        private void BaNavegaResumenAEntidad(object? sender, EventArgs e)
        {
            SeleccionaRegionBienesAdjudicados();
        }

        private void ClickContinuaBarSiguiente(object? sender, EventArgs e)
        {
            SeleccionaRegionBienesAdjudicados();
        }

        private void SeleccionaRegionBienesAdjudicados()
        {
            if (dgbaResumenBienesAdjudicados.SelectedRows is not null && dgbaResumenBienesAdjudicados.CurrentRow is not null)
            {
                int region = Convert.ToInt32(dgbaResumenBienesAdjudicados.CurrentRow.Cells[0].Value);
                object objValue = dgbaResumenBienesAdjudicados.CurrentRow.Cells[1].Value;
                string nombreRegion;
                if (objValue == null)
                    nombreRegion = string.Empty;
                else
                {
                    nombreRegion = Convert.ToString(objValue) ?? string.Empty;
                }
                if (region != 0)
                {
                    MuestraDatosEntidadBienesAdjudicados(region, nombreRegion);
                    tabReportesFinales.SelectedIndex = 8;
                }
            }

        }

        private void MuestraDatosEntidadBienesAdjudicados(int region, string nombreRegion)
        {

            if (_windowsFormsGloablInformation is not null)
            {
                IEnumerable<EntidadesBienesAdjudicados>? bienesAdjudicadosEntidades = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaEntidadesBienesAdjudicados(region);
                lblbaAdjudicadosRegion.Text = String.Format("Créditos Adjudicados de la Region : {0}", nombreRegion);
                dgbaBienesAdjudicadosxRegion.AutoGenerateColumns = false;
                dgbaBienesAdjudicadosxRegion.DataSource = bienesAdjudicadosEntidades;
            }
        }

        private void ClickRegresaBarAnterior(object? sender, EventArgs e)
        {
            tabReportesFinales.SelectedIndex = 0;
        }

        private void EntraBienesAdjudicados(object? sender, EventArgs e)
        {
            if (_windowsFormsGloablInformation is not null) 
            {
                dgbaResumenBienesAdjudicados.DataSource =
                    _windowsFormsGloablInformation.ActivaConsultasServices().CalculaResumenBienesAdjudicados();
                tabReportesFinales.SelectedIndex = 7;
            }
        }
    }
}
