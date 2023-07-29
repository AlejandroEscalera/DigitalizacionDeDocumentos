using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AppConsultaImagen;

/// <summary>
/// Aqui va la codificación de la búsqeda por acreditado
/// </summary>
public partial class MainFRM
{
    protected void InicializaBusquedaPorAcreditado()
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        btnBuscaPorAcreditado.Click += BtnBuscarPorAcreditado;
        btnReiniciarBusquedaxA.Click += BtnReiniciaBusquedaPorAcreditado;
        btnVerImagenxA.Click += BtnMuestraImagenxA;
        tblBusquedaPorAcreditado.CellDoubleClick += BtnMuestraImagenxA;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }
    protected void BtnBuscarPorAcreditado(object sender, EventArgs e)
    {
        if (_estoyPintandoDatos)
            return;
        string nombreAcreditado = txtNombreAcreditadoxA.Text.Trim();
        _informacionDeConsulta = BuscaPorAcreditado(nombreAcreditado).OrderByDescending(x => x.NumCredito).ToList();
        if (!_informacionDeConsulta.Any())
        {
            MessageBox.Show("No se encontró información");
            txtNombreAcreditadoxA.Focus();
            pnlDetalleBusquedaxA.Visible = false;
            return;
        }
        tblBusquedaPorAcreditado.AutoGenerateColumns = false;
        _estoyPintandoDatos = true;
        try
        {
            tblBusquedaPorAcreditado.DataSource = _informacionDeConsulta.ToList();
            var expediente = _informacionDeConsulta.FirstOrDefault();
            MuestraExpedientePorAcreditado(expediente);
            pnlDetalleBusquedaxA.Visible = true;
        }
        finally 
        {
            _estoyPintandoDatos = false;
        }
    }

    private void MuestraExpedientePorAcreditado(ExpedienteDeConsultaGv? expediente)
    {
        if (expediente == null)
        {
            pnlDetalleBusquedaxA.Visible = false;
            return;
        }
        txtNumeroCreditoxA.Text = expediente.NumCredito;
        txtNumeroCancelacionxA.Text = expediente.NumCreditoCancelado;
        chkActivoxA.Text = expediente.EsSaldosActivo ? "Es activo" : "No esta activo";
        chkActivoxA.Checked = expediente.EsSaldosActivo;
        chkCanceladoxA.Text = expediente.EsCancelado ? "Esta cancelado" : "No esta cancelado";
        chkCanceladoxA.Checked = expediente.EsCancelado;
        chkAperturaxA.Text = expediente.EsOrigenDelDr ? "Es origen Dr" : "Previo Dr";
        chkAperturaxA.Checked = expediente.EsOrigenDelDr;
        chkCancelacionDrxA.Text = expediente.EsCanceladoDelDr ? "Cancelado del Dr" : "Previo Dr";
        chkCancelacionDrxA.Checked = expediente.EsCanceladoDelDr;
        chkCastigadoxA.Text = expediente.EsCastigado ? "Castigado contable" : "Normal";
        chkCastigadoxA.Checked = expediente.EsCastigado;
        chkArqueoxA.Text = expediente.TieneArqueo ? "Revision GV" : "Sin auditar";
        chkArqueoxA.Checked = expediente.TieneArqueo;
        txtAcreditadoxA.Text = expediente.Acreditado;
        if (expediente.FechaApertura.HasValue)
        {
            txtFechaAperturaxA.Text = expediente.FechaApertura.Value.ToShortDateString();
        }
        if (expediente.FechaCancelacion.HasValue)
        {
            txtFechaCancelacionxA.Text = expediente.FechaCancelacion.Value.ToShortDateString();
        }
        txtCastigoxA.Text = expediente.Castigo;
        txtNumProductoxA.Text = expediente.NumProducto;
        txtProductoxA.Text = expediente.CatProducto;
        txtTipoDeCreditoxA.Text = expediente.TipoDeCredito;
        txtEjecutivoxA.Text = expediente.Ejecutivo;

        if (_archivosImagenes is not null)
        {
            /*
            imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(expediente.NumCredito, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (!imagenesEncontradas.Any())
            {
                // 214960012450000001
                imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(expediente.NumCreditoCancelado, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            imagenActual = 0;
            btnVerImagenxA.Visible = imagenesEncontradas.Length != 0;
            */
            var expedienteImagen = BuscaImagenes(QuitaCastigo(expediente.NumCredito));
            btnVerImagenxA.Visible = expedienteImagen is not null && (expedienteImagen.TieneImagenDirecta || expedienteImagen.TieneImagenIndirecta);

        }

    }

    protected void BtnReiniciaBusquedaPorAcreditado(object sender, EventArgs e)
    {
        txtNombreAcreditadoxA.Text = string.Empty;
        pnlDetalleBusquedaxA.Visible = false;
    }
    protected void BtnMuestraImagenxA(object sender, EventArgs e)
    {
        if (btnVerImagenxA.Visible)
        {
            regresaTabNavegacion = 1;
            regresaTabReportesFinales = 0;
            datosRegreso.Push(new NavegacionRetorno()
            {
                TabPrincipal = regresaTabNavegacion,
                TabReporte = regresaTabReportesFinales
            });
            string numCredito = txtNumeroCreditoxA.Text;
            pnlDetalleBusquedaxA.Visible = false;
            if (_informacionDeConsulta is not null)
            {
                // var expediente = _informacionDeConsulta.Where(x => (x.NumCredito ?? "").Equals(numCredito)).FirstOrDefault();
                var expediente = BuscaImagenes(QuitaCastigo(numCredito));
                pnlDetalleBusquedaxA.Visible = false;
                MuestraExpedienteEnImagen(expediente);
                NavegaVisorImagenes();
            }
        }
    }

    private void TblBusquedaPorAcreditado_SelectionChanged(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            if (_informacionDeConsulta is not null)
            {
                string numCredito = (Convert.ToString(tblBusquedaPorAcreditado.CurrentRow.Cells[0].Value) ?? "").Trim();
                var expediente = _informacionDeConsulta.Where(x => (x.NumCredito ?? "").Equals(numCredito)).FirstOrDefault();
                expediente ??= _informacionDeConsulta.Where(x => QuitaCastigo(x.NumCredito).Equals(QuitaCastigo(numCredito))).FirstOrDefault();
                if (expediente != null)
                {
                    MuestraExpedientePorAcreditado(expediente);
                }
            }
        }
    }
}
