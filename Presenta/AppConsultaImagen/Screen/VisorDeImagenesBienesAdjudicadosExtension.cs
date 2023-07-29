using AppConsultaImagen.Dialogo;
using AppConsultaImagen.Pinta;
using CefSharp;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM
{
    /// <summary>
    /// Listado de imagenes encontradas en la busqueda de bienes adjudicados
    /// </summary>
    private ArchivoImagenBienesAdjudicadosCorta[]? _imagenesEncontradasBienesAdjudicados = null;
    private ArchivoImagenBienesAdjudicadosCorta[]? _todasLasImagenesEncontradasBienesAdjudicados = null;
    private ExpedienteBienAdjudicado? _infoExpedienteBienesAdjudicados = null;
    /// <summary>
    /// Es el número de imagen que se muestra en pantalla
    /// </summary>
    private int imagenActualBienesAdjudicados = 0;
    /// <summary>
    /// Es la etapa de los Bienes Adjudicados
    /// </summary>
    private int etapaBienesAdjudicados = 0;

    protected void InicializaVisorDeImagenesBienesAdjudicados()
    {
//#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        btnRegresaBusquedaImagenNormal2.Click += BtnRegresaBusquedaImagenNormalClick;
        btnBaAnteriorImagen.Click += BtnBaAnteriorClick;
        btnBaSiguienteImagen.Click += BtnBaSiguienteClick;
        btnBaIdentCaso.Click += BtnBaIdentCasoClick;
        btnBaRegistroContable.Click += BtnBaRegistroContableClick;
        btnBaAdminSol.Click += BtnBaAdminSolClick;
        btnBaDocDestino.Click += BtnBaDocDestinoClick;
        btnBaConJur.Click += BtnBaConJurClick;
        btnBaDestino.Click += BtnBaDestinoClick;
        btnVerCreditosRelacionados.Click += BtnVerCreditosRelacionadosClick;
//#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }

    private void BtnVerCreditosRelacionadosClick(object? sender, EventArgs e)
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
            var frm = new SeleccionaExpedienteFRM();
            frm.FormaPrincipal = this;
            frm.CargaListaDeExpedientes(creditosRelacionadosBienesAdjudicados);
        }
    }

    private void BtnBaIdentCasoClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(1);
        imagenActualBienesAdjudicados = -1;        
        ImagenBaSiguiente();
    }

    private void BtnBaRegistroContableClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(2);
        imagenActualBienesAdjudicados = -1;
        ImagenBaSiguiente();
    }
    private void BtnBaAdminSolClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(3);
        imagenActualBienesAdjudicados = -1;
        ImagenBaSiguiente();
    }
    private void BtnBaDocDestinoClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(4);
        imagenActualBienesAdjudicados = -1;
        ImagenBaSiguiente();
    }
    private void BtnBaConJurClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(5);
        imagenActualBienesAdjudicados = -1;
        ImagenBaSiguiente();
    }
    private void BtnBaDestinoClick(object? sender, EventArgs e)
    {
        SeleccionaEtapa(6);
        imagenActualBienesAdjudicados = -1;
        ImagenBaSiguiente();
    }
    private void BtnBaSiguienteClick(object? sender, EventArgs e)
    {
        ImagenBaSiguiente();
    }

    private ExpedienteBienAdjudicado? BuscaImagenesBa(string numeroDeExpediente, string acreditados, bool directas = true)
    {
        if (_windowsFormsGloablInformation is not null)
        {
            _todasLasImagenesEncontradasBienesAdjudicados = _windowsFormsGloablInformation.ActivaConsultasServices().BuscaImagenesBienesAdjudicados(numeroDeExpediente, acreditados, directas).ToArray();

            var resultado = BuscaPorNumeroDeExpedienteBa();
            imagenActualBienesAdjudicados = -1;
            ImagenBaSiguiente();
            MuestaInformacionExpediente(resultado);
            return resultado;
        }
        return null;
    }

    public ExpedienteBienAdjudicado BuscaPorNumeroDeExpedienteBa()
    {
        if (_infoExpedientes is null)
        {
            Close();
            throw new Exception("Tienen que existir expedientes");
        }
        ExpedienteBienAdjudicado resultado = new() {
                Expediente = _detalleBienAdjudicado ?? new()
            };
        _infoExpedienteBienesAdjudicados = resultado;
        if (_todasLasImagenesEncontradasBienesAdjudicados is not null)
        {
            foreach (var etapa in resultado.Etapas)
            {
                ((List<ArchivoImagenBienesAdjudicadosCorta>)etapa.Imagenes).AddRange(
                    _todasLasImagenesEncontradasBienesAdjudicados.Where(x => (x.CarpetaDestino ?? "").Split('\\')[5][..1].Equals(etapa.Id.ToString()))
                    );
                etapa.TieneImagenes = etapa.Imagenes.Any();
                switch (etapa.Id)
                {
                    case 1:
                        btnBaIdentCaso.Visible = etapa.TieneImagenes;
                        break;
                    case 2:
                        btnBaRegistroContable.Visible = etapa.TieneImagenes;
                        break;
                    case 3:
                        btnBaAdminSol.Visible = etapa.TieneImagenes;
                        break;
                    case 4:
                        btnBaDocDestino.Visible = etapa.TieneImagenes;
                        break;
                    case 5:
                        btnBaConJur.Visible = etapa.TieneImagenes;
                        break;
                    case 6:
                        btnBaDestino.Visible = etapa.TieneImagenes;
                        break;
                    default:
                        break;
                }
            }
            SeleccionaPrimeraEtapa(resultado);
        }
        return resultado;
    }

    private void SeleccionaPrimeraEtapa(ExpedienteBienAdjudicado resultado)
    {
        var etapa = resultado.Etapas.Where(z => z.TieneImagenes).OrderBy(x => x.Id).Select(alfa=>alfa.Id).FirstOrDefault();
        if (etapa != 0)
        { 
            etapaBienesAdjudicados = etapa;
        }
        SeleccionaEtapa(etapaBienesAdjudicados);
        // _imagenesEncontradasBienesAdjudicados = resultado.Etapas.Where(x => x.Id == etapaBienesAdjudicados).SelectMany(x => x.Imagenes).ToArray();
    }

    private void SeleccionaEtapa(int etapa)
    {
        _imagenesEncontradasBienesAdjudicados = _infoExpedienteBienesAdjudicados?.Etapas.Where(x => x.Id == etapa).SelectMany(x => x.Imagenes).ToArray();
        btnBaIdentCaso.BackColor = (etapa != 1)? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnBaRegistroContable.BackColor = (etapa != 2) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnBaAdminSol.BackColor = (etapa != 3) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnBaDocDestino.BackColor = (etapa != 4) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnBaConJur.BackColor = (etapa != 5) ? Color.FromArgb(40, 92, 77) : Color.Olive;
        btnBaDestino.BackColor = (etapa != 6) ? Color.FromArgb(40, 92, 77) : Color.Olive;
    }

    private void ImagenBaSiguiente()
    {
        if (_imagenesEncontradasBienesAdjudicados is null || imagenActualBienesAdjudicados == _imagenesEncontradasBienesAdjudicados.Length - 1)
        {
            /// Todo ir al anterior del carrusel
            MessageBox.Show("Es la ultima imagen");
            return;
        }
        else
        {
            imagenActualBienesAdjudicados++;
            if (_imagenesEncontradasBienesAdjudicados is not null && _imagenesEncontradasBienesAdjudicados.Length > 0)
            {
                string archivoActual = Path.Combine(_imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].CarpetaDestino ?? "", _imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].NombreArchivo ?? "");
                if (!File.Exists(archivoActual))
                {
                    archivoActual = Path.Combine(_imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].CarpetaDestino ?? "", _imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].NombreArchivo ?? ""); // .Replace("F:\\", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                }
                lblBaCargando.Text = "Cargando...";
                lblBaCargando.ForeColor = Color.Red;
                lblBaCargando.Visible = true;
                try
                {
                    CargaImagenBA(archivoActual);
                }
                finally
                {
                    lblBaCargando.Text = String.Format("Imagen {0}/{1}", imagenActualBienesAdjudicados + 1, _imagenesEncontradasBienesAdjudicados.Length);
                    lblBaCargando.ForeColor = Color.Black;
                    lblBaCargando.Visible = true;
                }
            }
        }
    }

    private void BtnBaAnteriorClick(object? sender, EventArgs e)
    {
        ImagenBaAnterior();
    }

    private void ImagenBaAnterior()
    {
        if (imagenActualBienesAdjudicados == 0)
        {
            MessageBox.Show("Es la primera imagen");
            return;
        }
        else
        {
            imagenActualBienesAdjudicados--;
            if (_imagenesEncontradasBienesAdjudicados is not null && _imagenesEncontradasBienesAdjudicados.Length > 0)
            {
                string archivoActual = Path.Combine(_imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].CarpetaDestino ?? "", _imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].NombreArchivo ?? "");
                if (!File.Exists(archivoActual))
                {
                    archivoActual = Path.Combine(_imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].CarpetaDestino ?? "", _imagenesEncontradasBienesAdjudicados[imagenActualBienesAdjudicados].NombreArchivo ?? ""); //.Replace("F:", _unidadDeDiscoImagenes, StringComparison.OrdinalIgnoreCase);
                }
                lblBaCargando.Text = "Cargando...";
                lblBaCargando.ForeColor = Color.Red;
                lblBaCargando.Visible = true;
                try
                {
                    CargaImagenBA(archivoActual);
                }
                finally
                {
                    lblBaCargando.Text = String.Format("Imagen {0}/{1}", imagenActualBienesAdjudicados + 1, _imagenesEncontradasBienesAdjudicados.Length);
                    lblBaCargando.ForeColor = Color.Black;
                    lblBaCargando.Visible = true;
                }
            }
        }

    }


    /// <summary>
    /// Muestro la información en pantalla del expediente seleccionado
    /// </summary>
    /// <param name="datosExpediente">Información del expediente del cual se muestra en pantalla</param>
    protected void MuestaInformacionExpediente(ExpedienteBienAdjudicado datosExpediente)
    {
        txtBaNumExpediente.Text = datosExpediente.Expediente.ExpedienteElectronico;
        txtBaNumExpediente.ReadOnly = true;
        txtBaCveBien.Text = datosExpediente.Expediente.CveBien;
        txtBaCveBien.ReadOnly = true;
        txtBaAcreditados.Text = datosExpediente.Expediente.Acreditado;
        txtBaAcreditados.ReadOnly = true;
        txtBaDescripcion.Text = datosExpediente.Expediente.DescripcionReducidaBien;
        txtBaDescripcion.ReadOnly = true;
    }

    protected void CargaImagenBA(string imageFile)
    {
        if (File.Exists(imageFile))
        {
            pnlBaVisorImagen.Visible = false;
            pnlBaVisorImagen.Update();
            Application.DoEvents();
            chromiumWebBrowser2.Load(imageFile);
            pnlBaVisorImagen.Visible = true;
            pnlBaVisorImagen.Update();
            Application.DoEvents();
        }
    }

    private void ChromiumWebBrowser2_LoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
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


}
