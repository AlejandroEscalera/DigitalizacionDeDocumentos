using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

/// <summary>
/// Aquí va la búsqueda por expediente
/// </summary>
public partial class MainFRM
{
    IEnumerable<ExpedienteDeConsultaGv>? _informacionDeConsulta;
    bool _estoyPintandoDatos = false;

    protected void InicializaBusquedaPorExpediente() 
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        btnBuscarPorExpediente.Click += BtnBuscarPorExpediente;
        btnReiniciarBusquedaxE.Click += BtnReiniciaBusquedaPorExpediente;
        btnVerImagenxE.Click += BtnMuestraImagen;
        tblBusquedaPorExpediente.SelectionChanged += (this.TblBusquedaPorExpediente_SelectionChanged);
        tblBusquedaPorExpediente.CellDoubleClick += BtnMuestraImagen;
        bntRegresaAnteriorBxE.Click += RegresaAnteriorBxE;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }

    protected void RegresaAnteriorBxE(object sender, EventArgs e)
    {
        canNavigate = true;
        try
        {
            var datosARegresar = datosRegreso.Pop();
            regresaTabNavegacion = datosARegresar.TabPrincipal;
            regresaTabReportesFinales = datosARegresar.TabReporte;

            pnlRegresaAnteriorBxE.Visible = datosRegreso.Count != 0;
            tabNavegacion.SelectedIndex = regresaTabNavegacion;
            tabReportesFinales.SelectedIndex = regresaTabReportesFinales;
        }
        finally
        {
            canNavigate = false;
        }
    }

    protected void BtnBuscarPorExpediente(object sender, EventArgs e)
    {
        BuscaPorExpediente();
    }

    private void BuscaPorExpediente()
    {
        if (_estoyPintandoDatos)
        {
            return;
        }
        string numeroDeCredito = txtNumeroExpediente.Text.Replace(" ", "0");
        _informacionDeConsulta = BuscaPorNumeroDeCredito(numeroDeCredito).OrderByDescending(x => x.NumCredito).ToList();
        if (!_informacionDeConsulta.Any())
        {
            MessageBox.Show("No se encontró información");
            txtNumeroExpediente.Focus();
            pnlDetalleBusquedaPorExpediente.Visible = false;
            return;
        }
        tblBusquedaPorExpediente.AutoGenerateColumns = false;

        _estoyPintandoDatos = true;
        try
        {
            tblBusquedaPorExpediente.DataSource = _informacionDeConsulta.ToList();
            var expediente = _informacionDeConsulta.FirstOrDefault();
            MuestraExpedientePorNumero(expediente);
            pnlDetalleBusquedaPorExpediente.Visible = true;
        }
        finally
        {
            _estoyPintandoDatos = false;
        }
    }

    private void MuestraExpedientePorNumero(ExpedienteDeConsultaGv? expediente)
    {
        if (expediente == null)
        {
            pnlDetalleBusquedaPorExpediente.Visible = false;
            return;
        }
        txtNumCreditoxE.Text = expediente.NumCredito;
        txtNumCancelacionxE.Text = expediente.NumCreditoCancelado;
        chkActivoxE.Checked = expediente.EsSaldosActivo;
        chkCanceladoxE.Checked = expediente.EsCancelado;
        chkAperturaxE.Checked = expediente.EsOrigenDelDr;
        chkCancelacionDrxE.Checked = expediente.EsCanceladoDelDr;
        chkCastigadoxE.Checked = expediente.EsCastigado;
        chkArqueoxE.Checked = expediente.TieneArqueo;
        txtAcreditadoxE.Text = expediente.Acreditado;

        chkActivoxE.Text = expediente.EsSaldosActivo ? "Es activo" : "No esta activo";
        chkCanceladoxE.Text = expediente.EsCancelado ? "Esta cancelado" : "No esta cancelado";
        chkAperturaxE.Text = expediente.EsOrigenDelDr ? "Es origen Dr" : "Previo Dr";
        chkCancelacionDrxE.Text = expediente.EsCanceladoDelDr ? "Cancelado del Dr" : "Previo Dr";
        chkCastigadoxE.Text = expediente.EsCastigado ? "Castigado contable" : "Normal";
        chkArqueoxE.Text = expediente.TieneArqueo ? "Revision GV" : "Sin auditar";

        if (expediente.FechaApertura.HasValue) {
            txtFechaAperturaxE.Text = expediente.FechaApertura.Value.ToShortDateString();
        }
        if (expediente.FechaCancelacion.HasValue)
        {
            txtFechaCancelacionxE.Text = expediente.FechaCancelacion.Value.ToShortDateString();
        }
        txtCastigoxE.Text = expediente.Castigo;
        txtNumProductoxE.Text = expediente.NumProducto;
        txtProductoxE.Text = expediente.CatProducto;
        txtTipoCreditoxE.Text = expediente.TipoDeCredito;
        txtEjecutivoxE.Text = expediente.Ejecutivo;
        if (_archivosImagenes is not null)
        {
            var expedienteImagen = BuscaImagenes(QuitaCastigo(expediente.NumCredito));
            btnVerImagenxE.Visible = expedienteImagen is not null && (expedienteImagen.TieneImagenDirecta || expedienteImagen.TieneImagenIndirecta);
            /*
            if ()
            imagenesEncontradas = _archivosImagenes.OrderByDescending(y=>y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(expediente.NumCredito, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (!imagenesEncontradas.Any())
            {
                // 214960012450000001
                imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(expediente.NumCreditoCancelado, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            imagenActual = 0;
            btnVerImagenxE.Visible = imagenesEncontradas.Length != 0;
            */
        }

    }

    protected void BtnReiniciaBusquedaPorExpediente(object sender, EventArgs e)
    { 
        txtNumeroExpediente.Text = "000000000000000000";
        pnlDetalleBusquedaPorExpediente.Visible = false;
    }

    protected void BtnMuestraImagen(object sender, EventArgs e)
    {
        if (btnVerImagenxE.Visible)
        {
            regresaTabNavegacion = 0;
            regresaTabReportesFinales = 0;
            datosRegreso.Push(new NavegacionRetorno()
            {
                TabPrincipal = regresaTabNavegacion,
                TabReporte = regresaTabReportesFinales
            });
            string numCredito = txtNumCreditoxE.Text;
            if (_informacionDeConsulta is not null)
            {
                // var expediente = _informacionDeConsulta.Where(x => (x.NumCredito ?? "").Equals(numCredito)).FirstOrDefault();
                var expediente = BuscaImagenes(QuitaCastigo(numCredito));
                pnlDetalleBusquedaPorExpediente.Visible = false;
                MuestraExpedienteEnImagen(expediente);
                NavegaVisorImagenes();
            }
        }
    }

    private void TblBusquedaPorExpediente_SelectionChanged(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (_informacionDeConsulta is not null)
            {
                string numCredito = (Convert.ToString(tblBusquedaPorExpediente.CurrentRow.Cells[0].Value) ?? "").Trim();
                var expediente = _informacionDeConsulta.Where(x => (x.NumCredito ?? "").Equals(numCredito)).FirstOrDefault();
                if (expediente != null)
                {
                    MuestraExpedientePorNumero(expediente);
                }
            }
        }
    }


    private void TblBusquedaPorExpediente_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
        /*
        if (!_estoyPintandoDatos)
        {
            if (_informacionDeConsulta is not null)
            {
                string numCredito = Convert.ToString(tblBusquedaPorExpediente.Rows[e.RowIndex].Cells[1].Value) ?? "";
                var expediente = _informacionDeConsulta.Where(x => (x.NumCredito ?? "").Equals(numCredito)).FirstOrDefault();
                if (expediente != null)
                {
                    MuestraExpedientePorNumero(expediente);
                }
            }
        }
        */
    }

}
