using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Control.File2Pdf
{
    public class ProcesaConversionArchivosService : IProcesaConversionArchivos
    {
        private readonly IDoc2Pdf _doc2Pdf;
        private readonly IImagen2Pdf _imagen2Pdf;
        private readonly ITiff2Pdf _tiff2Pdf;
        private readonly IMsg2Pdf _msg2Pdf;

        public ProcesaConversionArchivosService(IDoc2Pdf doc2Pdf, IImagen2Pdf imagen2Pdf, ITiff2Pdf tiff2Pdf, IMsg2Pdf msg2Pdf)
        {
            _doc2Pdf = doc2Pdf;
            _imagen2Pdf = imagen2Pdf;
            _tiff2Pdf = tiff2Pdf;
            _msg2Pdf = msg2Pdf;
        }

        private static ArchivosImagenes ClonaArchivo(ArchivosImagenes origen)
        {
            ArchivosImagenes archivosImagenes = new()
            {
                Id = origen.Id,
                IdSearch = origen.IdSearch,
                NombreArchivo = origen.NombreArchivo,
                NumCredito = origen.NumCredito,
                FechaDeModificacion = origen.FechaDeModificacion,
                UsuarioDeModificacion = origen.UsuarioDeModificacion,
                TamanioArchivo = origen.TamanioArchivo,
                RutaDeGuardado = origen.RutaDeGuardado,
                UrlArchivo = origen.UrlArchivo,
                Ministracion = origen.Ministracion,
                VigenciaDelContato = origen.VigenciaDelContato,
                FechaVigenciaDelaDispOContrato = origen.FechaVigenciaDelaDispOContrato,
                CheckList = origen.CheckList,
                ValidadoPorCalidad = origen.ValidadoPorCalidad,
                ArchivoOrigen = origen.ArchivoOrigen,
                NumeroCreditoActivo = origen.NumeroCreditoActivo,
                Castigo = origen.Castigo,
                EsOrigenDelDoctor = origen.EsOrigenDelDoctor,
                EsCarteraActiva = origen.EsCarteraActiva,
                NumCte = origen.NumCte,
                Acreditado = origen.Acreditado,
                NumContrato = origen.NumContrato,
                FechaApertura = origen.FechaApertura,
                EsCancelacionDelDoctor = origen.EsCancelacionDelDoctor,
                NumeroMinistracion = origen.NumeroMinistracion,
                FechaInicio = origen.FechaInicio,
                ClasifMesa = origen.ClasifMesa,
                FechaAsignacionEntrada = origen.FechaAsignacionEntrada,
                MontoOtorgado = origen.MontoOtorgado,
                Analista = origen.Analista,
                Regional = origen.Regional,
                Sucursal = origen.Sucursal,
                CatAgencia = origen.CatAgencia,
                Extension = origen.Extension,
                CarpetaDestino = origen.CarpetaDestino,
                ThreadId = origen.ThreadId,
                Job = origen.Job,
                PorDescargar = origen.PorDescargar,
                ErrorAlDescargar = origen.ErrorAlDescargar,
                MensajeDeErrorAlDescargar = origen.MensajeDeErrorAlDescargar,
                PorOcr = origen.PorOcr,
                ErrorOcr = origen.ErrorOcr,
                MensajeDeErrorOCR = origen.MensajeDeErrorOCR,
                PorAOcr = origen.PorAOcr,
                ErrorAOcr = origen.ErrorAOcr,
                MensajeDeErrorAOCR = origen.MensajeDeErrorAOCR,
                PorAcomodar = origen.PorAcomodar,
                ErrorAcomodar = origen.ErrorAcomodar,
                MensajeDeErrorAcomodar = origen.MensajeDeErrorAcomodar,
                Termino = origen.Termino,
                Hash = origen.Hash,
                CadenaCredito = origen.CadenaCredito,
                CadenaCliente = origen.CadenaCliente,
                OcrNumCredito = origen.OcrNumCredito,
                OcrNumCliente = origen.OcrNumCliente,
                IgualNumCredito = origen.IgualNumCredito,
                TieneNumCredito = origen.TieneNumCredito,
                ChecarTona = origen.ChecarTona,
                NumPaginas = origen.NumPaginas,
                SubArchivo = origen.SubArchivo
            };
            return archivosImagenes;
        }

        public ArchivosImagenes ConvierteArchivoDesde(ArchivosImagenes origen, string fileName, string directorioDestino, ref int subArchivo)
        {
            IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
            FileInfo fi = new(fileName);
            if (fi.Exists)
            {
                try
                {
                    IFile2Pdf file2Pdf = ObtieneFile2Pdf(fi.Extension);
                    IList<string> archivos = file2Pdf.ConvierteArchivo(fileName, directorioDestino);
                    foreach (string archivo in archivos)
                    {
                        ArchivosImagenes archivoNuevo = ClonaArchivo(origen);
                        archivoNuevo.NombreArchivo = Path.GetFileName(archivo);
                        archivoNuevo.ArchivoOrigen = origen.UrlArchivo;
                        archivoNuevo.UrlArchivo = archivo;
                        archivoNuevo.CarpetaDestino = directorioDestino+Path.DirectorySeparatorChar;
                        archivoNuevo.Extension = ".pdf";
                        subArchivo++;
                        archivoNuevo.SubArchivo = subArchivo;
                        resultado.Add(archivoNuevo);
                    }
                }
                catch (Exception ex)
                {
                    origen.ErrorAlDescargar = true;
                    origen.MensajeDeErrorAlDescargar = ex.Message;

                }
            }
            else
            {
                origen.ErrorAlDescargar = true;
                origen.MensajeDeErrorAlDescargar = String.Format("No se descargó el archivo {0}", fileName);
            }
            return resultado.FirstOrDefault()?? origen;
        }

        private IFile2Pdf ObtieneFile2Pdf(string extension)
        {
            if (extension.Contains(".tif", StringComparison.InvariantCultureIgnoreCase) )
            {
                return _tiff2Pdf;
            }
            if (extension.Contains(".doc", StringComparison.InvariantCultureIgnoreCase))
            {
                return _doc2Pdf;
            }
            if (extension.Contains(".msg", StringComparison.InvariantCultureIgnoreCase))
            {
                return _msg2Pdf;
            }
            return _imagen2Pdf;
        }
    }
}
