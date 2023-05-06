using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;

namespace gob.fnd.Infaestructura.Negocio.Ocr
{
    public class AdministraOperacionesOCRService : IAdministraOperacionesOCRService
    {
        const string C_STR_NUM_CREDITO_NULO = "000000000000000000";

        private readonly ILogger<AdministraOperacionesOCRService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly IAplicaOcr _aplicaOcr;
        private readonly IAnalisisOcr _analisisOcr;
        private readonly string _directorioRaizControl;

        public AdministraOperacionesOCRService(ILogger<AdministraOperacionesOCRService> logger, IConfiguration configuration, IServicioImagenes servicioImagenes,
            IAplicaOcr aplicaOcr, IAnalisisOcr analisisOcr)
        {
            _logger = logger;
            _configuration = configuration;
            _servicioImagenes = servicioImagenes;
            _aplicaOcr = aplicaOcr;
            _analisisOcr = analisisOcr;
            _directorioRaizControl = _configuration.GetValue<string>("directorioRaizControl") ?? "";
        }

        private static IEnumerable<string> RecorreDirectorios(string directorioRaiz, int threadId, int maxJobs, string control, bool exacta = false)
        {
            IList<string> archivo = new List<string>();
            string directorioACrear = Path.Combine(directorioRaiz, control); // Proceso de descarga
            string directorioThread = Path.Combine(directorioACrear, String.Format("{0:000}", threadId));
            int inicial = !exacta ? 1 : maxJobs;
            for (int i = inicial; i <= maxJobs; i++)
            {
                string directorioJob = Path.Combine(directorioThread, String.Format("{0:000}", i));
                string fileName = Path.Combine(directorioJob, string.Format("{0}-{1}-{2:000}-{3:000}.xlsx", "IMG", control, threadId, i));
                archivo.Add(fileName);
            }
            return archivo.ToList();
        }

        public bool ProcesaHiloYTrabajo(int threadId, int job)
        {
            try
            {
                IEnumerable<string> archivos = RecorreDirectorios(_directorioRaizControl, threadId, job, "PO", true);

                IList<ArchivosImagenes> imagenesConOcrYAnalisis = new List<ArchivosImagenes>();

                foreach (string archivo in archivos)
                {
                    FileInfo fi = new(archivo);
                    if (fi.Exists)
                    {
                        var recuperaImagenes = _servicioImagenes.CargaImagenesTratadas(archivo).ToList();
                        int imagenesRecuperadas = recuperaImagenes.Count;
                        int noImagen = 0;
                        /*
                        recuperaImagenes = (from arc in recuperaImagenes

                                                // where (new int[] { 2766, 2767, 4511, 4512, 4513 }).Any(x => arc.Id == x)
                                                // 148444,148445,148446,148447,148448,148449,148450,148451,148452,
                                                //9512,9513,40229,94290,94291,94292,94293,94761,94764,
                                                //148340,148341,148342,148343,148344,148345,148346,148347,148348

                                            where (new int[] { 232344 }).Any(x => arc.Id == x)
                                            select arc).ToList();
                        */
                        foreach (var imagen in recuperaImagenes)
                        {
                            noImagen++;
                            string fileName = Path.Combine(imagen.CarpetaDestino ?? "", imagen.NombreArchivo ?? "").Replace("F:\\","G:\\");
                            _logger.LogInformation("Obtiendo OCR del thread:{theadId} job:{job} el archivo {fileName} de {noImagen} de {cantidadImagenes}", threadId, job, fileName, noImagen, imagenesRecuperadas);
                            FileInfo fi2 = new(fileName);
                            if (fi2.Exists && fi2.Extension.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase)) // && !imagen.ErrorAlDescargar
                            {
                                bool SePudoAplicarOcr = _aplicaOcr.ObtieneOcrDesdeArchivo(fileName, imagen.NumPaginas);
                                if (!SePudoAplicarOcr)
                                {
                                    imagen.ErrorOcr = true;
                                    imagen.MensajeDeErrorOCR = "No se pudo aplicar OCR";
                                }
                                else
                                {
                                    imagen.ErrorAlDescargar = false;
                                    imagen.MensajeDeErrorAlDescargar = string.Empty;

                                    string cadenaDeCredito = string.Empty;
                                    string cadenaDeCliente = string.Empty;
                                    string numeroDeCredito = string.Empty;
                                    string numeroDeCliente = string.Empty;
                                    string archivoTexto = fi2.FullName.Replace(".pdf", ".txt", StringComparison.InvariantCultureIgnoreCase);

                                    bool SePudoAnalizarOcr = _analisisOcr.ObtieneInformacionOCR(archivoTexto, ref cadenaDeCredito, ref cadenaDeCliente, ref numeroDeCliente, ref numeroDeCredito);
                                    if (!SePudoAplicarOcr)
                                    {
                                        imagen.ErrorAOcr = true;
                                        imagen.MensajeDeErrorOCR = String.Format("No se pudo llevar a cabo el analisis del archivo {0}", archivoTexto);
                                    }
                                    else
                                    {
                                        imagen.TieneNumCredito = !string.IsNullOrEmpty(numeroDeCredito) && cadenaDeCredito.Contains(imagen.NumCredito ?? "", StringComparison.InvariantCultureIgnoreCase);
                                        imagen.CadenaCredito = cadenaDeCredito;
                                        imagen.CadenaCliente = cadenaDeCliente;
                                        imagen.OcrNumCliente = numeroDeCliente;
                                        imagen.OcrNumCredito = numeroDeCredito;
                                        if (string.IsNullOrEmpty(cadenaDeCredito) && string.IsNullOrEmpty(cadenaDeCliente))
                                        {
                                            imagen.ErrorAOcr = true;
                                            imagen.MensajeDeErrorOCR = String.Format("No se encontró informacion de cliente o de crédito en el archivo {0}", archivoTexto);
                                        }
                                        else
                                        {
                                            #region Se recupera el numero en automático?
                                            if (!(imagen.NumCredito ?? "").Equals(numeroDeCredito))
                                            {
                                                #region Si no tenía crédito anteriormente se asigna
                                                if ((imagen.NumCredito ?? "").Equals(C_STR_NUM_CREDITO_NULO))
                                                {
                                                    imagen.NumCredito = numeroDeCredito;
                                                    imagen.NumCte = numeroDeCliente;
                                                }
                                                #endregion
                                                #region Si no tiene cruce con otro registro se asigna
                                                if (!imagen.FechaAsignacionEntrada.HasValue && !imagen.EsCarteraActiva && !imagen.EsCancelacionDelDoctor && !imagen.EsOrigenDelDoctor)
                                                {
                                                    imagen.NumCredito = numeroDeCredito;
                                                    imagen.NumCte = numeroDeCliente;
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            #region Se hace la evaluación si es el mismo valor del OCR
                                            imagen.IgualNumCredito = ((imagen.NumCredito ?? "").Equals(numeroDeCredito));
                                            #endregion
                                        }
                                    }

                                }
                            }
                            ((List<ArchivosImagenes>)imagenesConOcrYAnalisis).Add(imagen);
                        }
                    }

                    // Get a list of files to save
                    IEnumerable<string> archivosPorGuardar = RecorreDirectorios(_directorioRaizControl, threadId, job, "AO", true);

                    // Iterate through the list of files
                    foreach (var item in archivosPorGuardar)
                    {
                        // Save images with OCR and analysis to file destination
                        _servicioImagenes.GuardarImagenes(imagenesConOcrYAnalisis, item);

                        // Log information that the result of the download was saved in the file destination
                        _logger.LogInformation("Se guardo el resultado de la descarga en el archivo {fileDestination}!", item);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Marcó error al procesar el ocr : {mensaje}", ex.Message);
                return false;
            }
            return true;
        }

    }
}
