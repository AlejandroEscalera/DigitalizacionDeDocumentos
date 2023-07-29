using AppConsultaImagen.Dialogo;
using AppConsultaImagen.Pinta;
using CefSharp;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM
{
    /// <summary>
    /// Listado de imagenes encontradas en la búsqueda de expedientes jurídicos
    /// </summary>
    private ArchivoImagenExpedientesCorta[]? _imagenesEncontradasCobranza = null;
    private ArchivoImagenExpedientesCorta[]? _todasLasImagenesEncontradasCobranza = null;    
    private ExpedienteJuridicoParaImagenes? _infoExpedienteCobranza = null;

    /// <summary>
    /// Es el número de imagen que se muestra en pantalla
    /// </summary>
    private int imagenActualExpedienteCobranza = 0;

    /// <summary>
    /// Es la etapa del expediente que se muestra en pantalla
    /// </summary>
    private int tipoInformacionExpedienteCobranza = 0;

    protected void InicializaVisorDeImagenesExpendedientesCobranza()
    {
        btnRegresaBusquedaImagenNormal3.Click += BtnRegresaBusquedaImagenNormalClick;
        btnjiAnteriorImagen.Click += BtnjiAnteriorImagenClick;
        btnjiSiguienteImagen.Click += BtnjiSiguienteImagenClick;
        btnjiExpediente.Click += BtnjiExpedienteClick;


        btnjiTurnoCobranza.Click += BtnjiTurnoCobranzaClick;
        btnjiTurnoJuridico.Click += BtnjiTurnoJuridicoClick;
        btnjiAsuntosDemandados.Click += BtnjiAsuntosDemandadosClick;
        btnCreditosRelacionadosEJ.Click += BtnCreditosRelacionadosEJClick;
        chromiumWebBrowser3.LoadingStateChanged += ChromiumWebBrowser3_LoadingStateChanged;
    }

    private void BtnCreditosRelacionadosEJClick(object? sender, EventArgs e)
    {
        if (creditosRelacionadosBienesAdjudicados.Length != 0)
        {
            regresaTabNavegacion = tabNavegacion.SelectedIndex;
            regresaTabReportesFinales = tabReportesFinales.SelectedIndex;
            datosRegreso.Push(new NavegacionRetorno()
            {
                TabPrincipal = regresaTabNavegacion,
                TabReporte = regresaTabReportesFinales
            });
            SeleccionaExpedienteFRM frm = new();
            frm.FormaPrincipal = this;
            frm.CargaListaDeExpedientes(creditosRelacionadosBienesAdjudicados);
        }
    }

    private void BtnjiExpedienteClick(object? sender, EventArgs e)
    {
        SeleccionaEtapaJi(4);
        imagenActualExpedienteCobranza = -2;
        imagenActual = -1;
        ImagenJiSiguiente();
    }

    private void ChromiumWebBrowser3_LoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
    {
        string fileName = string.Format("{0}cargando.txt", _unidadDeDiscoImagenes);
        if (e.IsLoading)
        {
            if (!File.Exists(fileName))
            {
                System.IO.StreamWriter outputFile = new(fileName);  // System.IO.StreamWriter
                outputFile.WriteLine(fileName);
                outputFile.Close();
            }
        }
        else
            if (File.Exists(fileName))
            File.Delete(fileName);
    }

    private void BtnjiAsuntosDemandadosClick(object? sender, EventArgs e)
    {
        SeleccionaEtapaJi(3);
        imagenActualExpedienteCobranza = -1;
        ImagenJiSiguiente();
    }

    private void BtnjiTurnoJuridicoClick(object? sender, EventArgs e)
    {
        SeleccionaEtapaJi(2);
        imagenActualExpedienteCobranza = -1;
        ImagenJiSiguiente();
    }

    private void BtnjiTurnoCobranzaClick(object? sender, EventArgs e)
    {
        SeleccionaEtapaJi(1);
        imagenActualExpedienteCobranza = -1;
        ImagenJiSiguiente();
    }

    private void SeleccionaEtapaJi(int etapa)
    {
        tipoInformacionExpedienteCobranza = etapa;
        _imagenesEncontradasCobranza = _infoExpedienteCobranza?.Etapas.Where(x => x.Id == etapa).SelectMany(x => x.Imagenes).ToArray();
        btnjiTurnoCobranza.BackColor = (etapa != 1) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnjiTurnoJuridico.BackColor = (etapa != 2) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnjiAsuntosDemandados.BackColor = (etapa != 3) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnjiExpediente.BackColor = (etapa != 4) ? Color.FromArgb(40, 92, 77) : Color.Olive;
    }

    private void BtnjiSiguienteImagenClick(object? sender, EventArgs e)
    {
        ImagenJiSiguiente();
    }

    private void ImagenJiSiguiente()
    {
        if (imagenActualExpedienteCobranza != -2)
        {
            if (_imagenesEncontradasCobranza is null || imagenActualExpedienteCobranza == _imagenesEncontradasCobranza.Length - 1)
            {
                /// Todo ir al anterior del carrusel
                if (_imagenesEncontradasCobranza is not null && _imagenesEncontradasCobranza.Length != 0)
                {
                    MessageBox.Show("Es la ultima imagen");
                }
                return;
            }
            else
            {
                imagenActualExpedienteCobranza++;
                if (_imagenesEncontradasCobranza is not null && _imagenesEncontradasCobranza.Length > 0)
                {
                    string archivoActual = Path.Combine(_imagenesEncontradasCobranza[imagenActualExpedienteCobranza].CarpetaDestino ?? "", _imagenesEncontradasCobranza[imagenActualExpedienteCobranza].NombreArchivo ?? "");
                    if (!File.Exists(archivoActual))
                    {
                        archivoActual = Path.Combine(_imagenesEncontradasCobranza[imagenActualExpedienteCobranza].CarpetaDestino ?? "", _imagenesEncontradasCobranza[imagenActualExpedienteCobranza].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                    }
                    lblJiCargando.Text = "Cargando...";
                    lblJiCargando.ForeColor = Color.Red;
                    lblJiCargando.Visible = true;
                    try
                    {
                        CargaImagenJi(archivoActual);
                    }
                    finally
                    {
                        lblJiCargando.Text = String.Format("Imagen {0}/{1}", imagenActualExpedienteCobranza + 1, _imagenesEncontradasCobranza.Length);
                        lblJiCargando.ForeColor = Color.Black;
                        lblJiCargando.Visible = true;
                    }
                }
            }
        }
        else 
        {
            ImagenSiguienteExpediente();
        }
    }

    protected void CargaImagenJi(string imageFile)
    {
        if (File.Exists(imageFile))
        {
            pnlJiVisorImagen.Visible = false;
            pnlJiVisorImagen.Update();
            Application.DoEvents();
            chromiumWebBrowser3.Load(imageFile);
            pnlJiVisorImagen.Visible = true;
            pnlJiVisorImagen.Update();
            Application.DoEvents();
        }
    }

    private void BtnjiAnteriorImagenClick(object? sender, EventArgs e)
    {
        ImagenJiAnterior();
    }

    private void ImagenJiAnterior()
    {
        if (imagenActualExpedienteCobranza != -2)
        {

            if (imagenActualExpedienteCobranza == 0)
            {
                MessageBox.Show("Es la primera imagen");
                return;
            }
            else
            {
                imagenActualExpedienteCobranza--;
                if (_imagenesEncontradasCobranza is not null && _imagenesEncontradasCobranza.Length > 0)
                {
                    string archivoActual = Path.Combine(_imagenesEncontradasCobranza[imagenActualExpedienteCobranza].CarpetaDestino ?? "", _imagenesEncontradasCobranza[imagenActualExpedienteCobranza].NombreArchivo ?? "");
                    if (!File.Exists(archivoActual))
                    {
                        archivoActual = Path.Combine(_imagenesEncontradasCobranza[imagenActualExpedienteCobranza].CarpetaDestino ?? "", _imagenesEncontradasCobranza[imagenActualExpedienteCobranza].NombreArchivo ?? ""); //.Replace("F:", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                    }
                    lblJiCargando.Text = "Cargando...";
                    lblJiCargando.ForeColor = Color.Red;
                    lblJiCargando.Visible = true;
                    try
                    {
                        CargaImagenJi(archivoActual);
                    }
                    finally
                    {
                        lblJiCargando.Text = String.Format("Imagen {0}/{1}", imagenActualExpedienteCobranza + 1, _imagenesEncontradasCobranza.Length);
                        lblJiCargando.ForeColor = Color.Black;
                        lblJiCargando.Visible = true;
                    }
                }
            }
        }
        else
        {
            ImagenAnteriorExpediente();
        }

    }

    private ExpedienteJuridicoParaImagenes? BuscaImagenesExpedienteJuridico(string contrato, string numCredito)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            _detalleExpedienteJuridico = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaExpedienteJuridico(contrato, numCredito);
            // _infoExpedienteCobranza = _detalleExpedienteJuridico;
            if (_detalleExpedienteJuridico is not null)
            {
                _todasLasImagenesEncontradasCobranza = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaImagenesExpedientes(contrato, numCredito).ToArray();
                var resultado = BuscaPorExpedienteJuridico(_detalleExpedienteJuridico);
                imagenActualExpedienteCobranza = tipoInformacionExpedienteCobranza != 4? - 1: -2;
                ImagenJiSiguiente();
                MuestaInformacionJuridico(resultado);
                return resultado;
            }
        }
        return null;
    }

    public void MuestaInformacionJuridico(ExpedienteJuridicoParaImagenes datosExpediente) 
    {
        txtjiContrato.Text = datosExpediente.Expediente.NumContrato;
        txtjiNumExpediente.Text = datosExpediente.Expediente.Expediente;
        txtjiNomDemandado.Text = datosExpediente.Expediente.NombreDemandado;
        txtjiJuicio.Text = datosExpediente.Expediente.Juicio;
        txtjiTipoGarantias.Text = datosExpediente.Expediente.TipoGarantias;
        txtjiAResponsable.Text = datosExpediente.Expediente.AbogadoResponsable;
        txtjiExpectativa.Text = datosExpediente.Expediente.ExpectativasRecuperacion;
    }


    public ExpedienteJuridicoParaImagenes BuscaPorExpedienteJuridico(ExpedienteJuridicoDetalle detalleExpedienteJuridico)
    {
        ExpedienteJuridicoParaImagenes resultado = new()
        {
            Expediente = detalleExpedienteJuridico ?? new()
        };
        _infoExpedienteCobranza = resultado;
        if (_todasLasImagenesEncontradasCobranza is not null)
        {
            foreach (var etapa in resultado.Etapas)
            {
                /*
                var busquedaEtapas = (etapa.DescripcionEtapa ?? "").Split(' ');
                var imagenesEncontradas = _todasLasImagenesEncontradasCobranza;
                foreach (var etapaBusqueda in busquedaEtapas)
                {
                    imagenesEncontradas = imagenesEncontradas.Where(x => (x.CarpetaDestino ?? "").Contains(etapaBusqueda, StringComparison.OrdinalIgnoreCase)).ToArray();
                }
                */

                var imagenesEncontradas = _todasLasImagenesEncontradasCobranza;

                switch (etapa.Id)
                {
                    case 1:
                        imagenesEncontradas = imagenesEncontradas.Where(x => x.EsTurno).ToArray();
                        ((List<ArchivoImagenExpedientesCorta>)etapa.Imagenes).AddRange(imagenesEncontradas);
                        etapa.TieneImagenes = etapa.Imagenes.Any();
                        btnjiTurnoCobranza.Visible = etapa.TieneImagenes;
                        break;
                    case 2:
                        imagenesEncontradas = imagenesEncontradas.Where(x => x.EsCobranza).ToArray();
                        ((List<ArchivoImagenExpedientesCorta>)etapa.Imagenes).AddRange(imagenesEncontradas);
                        etapa.TieneImagenes = etapa.Imagenes.Any();
                        btnjiTurnoJuridico.Visible = etapa.TieneImagenes;
                        break;
                    case 3:
                        btnjiAsuntosDemandados.Visible = false; // etapa.TieneImagenes;
                        break;
                    case 4:
                        if (detalleExpedienteJuridico is not null)
                        {
                            btnjiExpediente.Visible = detalleExpedienteJuridico.TieneImagenExpediente;
                            ((List<ArchivoImagenExpedientesCorta>)etapa.Imagenes).AddRange(imagenesEncontradas); // ImagenesExpedienteJuridico;
                            etapa.TieneImagenes = detalleExpedienteJuridico.TieneImagenExpediente;
                        }
                        break;
                    default:
                        break;
                }
            }
            SeleccionaPrimeraEtapaJi(resultado);
        }
        return resultado;
    }

    private void SeleccionaPrimeraEtapaJi(ExpedienteJuridicoParaImagenes resultado)
    {
        var etapa = resultado.Etapas.Where(z => z.TieneImagenes).OrderBy(x => x.Id).Select(alfa => alfa.Id).FirstOrDefault();
        if (etapa != 0)
        {
            tipoInformacionExpedienteCobranza = etapa;
        }
        SeleccionaEtapaJi(tipoInformacionExpedienteCobranza);
    }

    protected void NavegaVisorImagenesJuridico()
    {
        canNavigate = true;
        tabNavegacion.SelectedIndex = 6;
        canNavigate = false;
        imagenActual = 1;
        ImagenJiAnterior();
    }

    protected void ImagenAnteriorExpediente()
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
                string archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? "");
                if (!File.Exists(archivoActual))
                {
                    archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                }
                lblJiCargando.Text = "Cargando...";
                lblJiCargando.ForeColor = Color.Red;
                lblJiCargando.Visible = true;
                try
                {
                    CargaImagenJi(archivoActual);
                }
                finally
                {
                    lblJiCargando.Text = String.Format("Imagen {0}/{1}", imagenActual + 1, imagenesEncontradas.Length);
                    lblJiCargando.ForeColor = Color.Black;
                    lblJiCargando.Visible = true;
                }
            }
        }
    }

    protected void ImagenSiguienteExpediente()
    {
        if (imagenesEncontradas is null || imagenActual == imagenesEncontradas.Length - 1)
        {
            MessageBox.Show("Es la ultima imagen");
            return;
        }
        else
        {
            imagenActual++;
            if (imagenesEncontradas is not null && imagenesEncontradas.Length > 0)
            {
                string archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? "");
                if (!File.Exists(archivoActual))
                {
                    archivoActual = Path.Combine(imagenesEncontradas[imagenActual].CarpetaDestino ?? "", imagenesEncontradas[imagenActual].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                }
                lblJiCargando.Text = "Cargando...";
                lblJiCargando.ForeColor = Color.Red;
                lblJiCargando.Visible = true;
                try
                {
                    CargaImagenJi(archivoActual);
                }
                finally
                {
                    lblJiCargando.Text = String.Format("Imagen {0}/{1}", imagenActual + 1, imagenesEncontradas.Length);
                    lblJiCargando.ForeColor = Color.Black;
                    lblJiCargando.Visible = true;
                }
            }
        }
    }
}