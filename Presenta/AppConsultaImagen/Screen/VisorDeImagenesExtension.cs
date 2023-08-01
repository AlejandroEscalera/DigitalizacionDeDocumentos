using AppConsultaImagen.Pinta;
using CefSharp;
using CefSharp.WinForms;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

/// <summary>
/// Aqui van las rutinas relacionadas con el visor de imágenes
/// </summary>
public partial class MainFRM
{
    // private CefSharp.WinForms.ChromiumWebBrowser browser = new();
    private ArchivoImagenCorta[]? imagenesEncontradas = null;
    private ArchivoImagenCorta[]? imagenesEncontradasSoloGV = null;
    private ArchivoImagenCorta[]? imagenesEncontradasTodasSinGV = null;
    private int imagenActual = 0;
    private int regresaTabNavegacion = 0;
    private int regresaTabReportesFinales = 0;
    private readonly Stack<NavegacionRetorno> datosRegreso = new();

    private string _unidadDeDiscoImagenes = "";
    private bool seTieneGuardaValor = false;
    private bool seMuestraGuardaValor = false;

    protected void InicializaVisorDeImagenes()
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        btnImagenAnterior.Click += BtnImagenAnterior;
        btnImagenSiguiente.Click += BtnImagenSiguiente;
        btnRegresaBusquedaImagenNormal.Click += BtnRegresaBusquedaImagenNormalClick;
        btnVerImagenGuardaValores.Visible = false;
        btnVerImagenGuardaValores.Click += BtnVerImagenGuardaValores_Click;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }

    private ExpedienteDeConsultaGv? BuscaImagenes(string numeroDeCredito, bool directas = true)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            imagenesEncontradas = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaImagenes(numeroDeCredito, directas).ToArray();
            imagenesEncontradasSoloGV = imagenesEncontradas.Where(x => (x.CarpetaDestino??"").Contains(@":\11\GV\",StringComparison.InvariantCultureIgnoreCase) == true).ToArray();
            imagenesEncontradasTodasSinGV = imagenesEncontradas.Where(x => !(x.CarpetaDestino ?? "").Contains(@":\11\GV\", StringComparison.InvariantCultureIgnoreCase) == true).ToArray();

            if (imagenesEncontradasTodasSinGV.Any())
            {
                imagenesEncontradas = imagenesEncontradasTodasSinGV;
            }
            if (imagenesEncontradasSoloGV.Any())
            {
                seTieneGuardaValor = true;
                btnVerImagenGuardaValores.Visible = true;
                btnVerImagenGuardaValores.Enabled = true;
                btnVerImagenGuardaValores.Text = "Ver solo Guarda Valor";
                btnVerImagenGuardaValores.BackColor = Color.FromArgb(109, 135, 39);
                seMuestraGuardaValor = false;

            }
            else
            {
                seTieneGuardaValor = false;
                btnVerImagenGuardaValores.Visible = false;
            }
            if (!imagenesEncontradas.Any() && imagenesEncontradasSoloGV.Any())
            {
                imagenesEncontradas = imagenesEncontradasSoloGV;
                btnVerImagenGuardaValores.Text = "Solo hay Guarda Valor";
                btnVerImagenGuardaValores.BackColor = Color.FromArgb(109, 135, 39);
                btnVerImagenGuardaValores.Visible = true;
                btnVerImagenGuardaValores.Enabled = false;
                seMuestraGuardaValor = true;
            }
            var resultado = BuscaPorNumeroDeCredito(numeroDeCredito).OrderByDescending(x => x.NumCredito).ToList();
            return resultado.FirstOrDefault();
        }
        return null;
    }

    private void BtnVerImagenGuardaValores_Click(object? sender, EventArgs e)
    {
        SeMuestraGuardaValor();
    }

    private void SeMuestraGuardaValor()
    {
        if (seTieneGuardaValor)
        {
            if (seMuestraGuardaValor == true)
            {
                seMuestraGuardaValor = false;
                btnVerImagenGuardaValores.Text = "Ver solo Guarda Valor";
                btnVerImagenGuardaValores.BackColor = Color.FromArgb(109,135,39);

                // Cambio el filtro de imagenes GV
                imagenesEncontradas = imagenesEncontradasTodasSinGV;
                imagenActual = 0;
                MuestraImagen();
            }
            else
            {
                seMuestraGuardaValor = true;
                btnVerImagenGuardaValores.Text = "Ver todo el Expediente";
                btnVerImagenGuardaValores.BackColor = Color.FromArgb(97,16,50);

                // Cambio el filtro de imagenes GV
                imagenesEncontradas = imagenesEncontradasSoloGV;
                imagenActual = 0;
                MuestraImagen();
            }
        }
    }

    private void BtnRegresaBusquedaImagenNormalClick(object? sender, EventArgs e)
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

    protected void CargaImagen(string imageFile) {
        if (File.Exists(imageFile))
        {
            var uri = new System.Uri(imageFile);
            var converted = uri.AbsoluteUri + "#toolbar";
            PnlMuestraImagenes.Visible = false;
            PnlMuestraImagenes.Update();
            Application.DoEvents();
            chromiumWebBrowser1.LoadUrl(converted);

            PnlMuestraImagenes.Visible = true;
            PnlMuestraImagenes.Update();
            Application.DoEvents();
        }
    }

    private void ChromiumWebBrowser1_LoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
    {
        string fileName = string.Format("{0}cargando.txt", _unidadDeDiscoImagenes);
        //chromiumWebBrowser1.Visible = !e.IsLoading;
        if (e.IsLoading)
        {
            if (!File.Exists(fileName))
            {
                System.IO.StreamWriter outputFile = new (fileName);  // System.IO.StreamWriter
                outputFile.WriteLine(fileName);
                outputFile.Close();
            }
        }
        else
            if (File.Exists(fileName))
                File.Delete(fileName);
    }

    private void MuestraExpedienteEnImagen(ExpedienteDeConsultaGv? expediente)
    {
        if (expediente == null)
        {
            pnlVisorDeImagenes.Visible = false;
            return;
        }        
        pnlVisorDeImagenes.Visible = true;
        string numCredito = expediente.NumCredito?? "000000000000000000";
        txtNumCreditoVI.Text = numCredito;
        txtNumCreditoVI.ReadOnly = true;
        txtAcreditadoVI.Text = expediente.Acreditado;
        txtAcreditadoVI.ReadOnly = true;
        if (numCredito.Substring(3, 1).Equals("8"))
        {
            if (_windowsFormsGloablInformation is not null) {
                var listaCreditos = _windowsFormsGloablInformation.ActivaConsultasServices().ObtieneCreditoOrigen(numCredito);
                // pongo en una sola linea la lista de creditos, separado por comas en txtAnalistaVI.Text
                lblAnalista.Text = "Crédito(s) Origen :";
                lblAnalista.TextAlign = ContentAlignment.TopRight;
                txtAnalistaVI.Text = string.Join(",", listaCreditos.Select(x => x));
            }
        }
        else
        {
            lblAnalista.Text = "Analista :";
            lblAnalista.TextAlign = ContentAlignment.TopRight;
            txtAnalistaVI.Text = expediente.Analista;
        }
        txtFechaEntradaMesaVI.Text = expediente.FechaSolicitud.HasValue? expediente.FechaSolicitud.Value.ToShortDateString() : string.Empty;
        txtFechaEntradaMesaVI.ReadOnly = true;
        txtFechaPrimeraMinistracionVI.Text = expediente.FechaInicioMinistracion.HasValue ? expediente.FechaInicioMinistracion.Value.ToShortDateString() : string.Empty;
        txtFechaPrimeraMinistracionVI.ReadOnly = true;
    }

    protected void ImagenAnterior() 
    {
        if (imagenActual == 0)
        {
            MessageBox.Show("Es la primera imagen");
            return;
        }
        else
        {
            imagenActual--;
            if (imagenesEncontradas is not null && imagenesEncontradas.Length > 0)
            {
                MuestraImagen();
            }
        }
        
    }

    private void MuestraImagen()
    {
        if (imagenesEncontradas is not null)
        {
            string archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? "");
            if (!File.Exists(archivoActual))
            {
                archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
            }
            lblCargando.Text = "Cargando...";
            lblCargando.ForeColor = Color.Red;
            lblCargando.Visible = true;
            try
            {
                CargaImagen(archivoActual);
            }
            finally
            {
                lblCargando.Text = String.Format("Imagen {0}/{1}", imagenActual + 1, imagenesEncontradas.Length);
                lblCargando.ForeColor = Color.Black;
                lblCargando.Visible = true;
            }
        }
    }

    protected void ImagenSiguiente()
    {
        if (imagenesEncontradas is null || imagenActual == imagenesEncontradas.Length  -1)
        {
            MessageBox.Show("Es la ultima imagen");
            return;
        }
        else
        {
            imagenActual++;
            if (imagenesEncontradas is not null && imagenesEncontradas.Length > 0)
            {
                MuestraImagen();
                /*
                string archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? "");
                if (!File.Exists(archivoActual))
                {
                    archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                }
                lblCargando.Text = "Cargando...";
                lblCargando.ForeColor = Color.Red;
                lblCargando.Visible = true;
                try
                {
                    CargaImagen(archivoActual);
                }
                finally
                {
                    lblCargando.Text = String.Format("Imagen {0}/{1}", imagenActual + 1, imagenesEncontradas.Length);
                    lblCargando.ForeColor = Color.Black;
                    lblCargando.Visible = true;
                }
                */
            }
        }
    }
    private void BtnImagenSiguiente(object sender, EventArgs e)
    {
        ImagenSiguiente();
    }

    private void BtnImagenAnterior(object sender, EventArgs e)
    {
        ImagenAnterior();
    }
}
