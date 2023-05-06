using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Control.ZIP
{
    public class ProcesaArchivoCompreso : IProcesaArchivoCompreso
    {
        private readonly IDescomprimeArchivo7z _descompresor7Z;
        private readonly IDescomprimeArchivoZip _descompresorZip;

        public ProcesaArchivoCompreso(IDescomprimeArchivo7z descompresor7z, IDescomprimeArchivoZip descompresorZip)
        {
            _descompresor7Z = descompresor7z;
            _descompresorZip = descompresorZip;
        }

        public IEnumerable<ArchivosImagenes> ObtieneArchivosDesde(ArchivosImagenes origen, string fileName, string directorioDestino, ref int subArchivo)
        {        
            IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
            FileInfo fi = new(fileName);
            if (fi.Exists)
            {
                try
                {
                    bool es7Zip = fi.Extension.Contains("7z", StringComparison.InvariantCultureIgnoreCase);
                    IDescomprimeArchivo descompresor = ObtieneDescompresor(es7Zip);
                    IList<string> archivos = descompresor.DescomprimeArchivo(fileName, directorioDestino);
                    foreach (string archivo in archivos)
                    {
                        FileInfo fi2 = new(archivo);
                        ArchivosImagenes archivoNuevo = ClonaArchivo(origen);
                        archivoNuevo.NombreArchivo = fi2.Name;
                        archivoNuevo.ArchivoOrigen = origen.UrlArchivo;
                        archivoNuevo.UrlArchivo = fi.FullName;
                        archivoNuevo.CarpetaDestino = fi2.DirectoryName;
                        archivoNuevo.Extension = fi2.Extension;
                        archivoNuevo.Hash = "";
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
            else {
                origen.ErrorAlDescargar = true;
                origen.MensajeDeErrorAlDescargar = String.Format("No se descargó el archivo {0}", fileName);
            }
            return resultado.ToList();
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

        private IDescomprimeArchivo ObtieneDescompresor(bool es7Zip)
        {
            if (es7Zip)
            {
                return _descompresor7Z;

            }
            else {
                return _descompresorZip;
            }
        }
    }
}
