using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Control.Descarga
{
    public class DescargaInformacionOneDriveService : IDescargaInformacionOneDrive
    {
        private readonly ILogger<DescargaInformacionOneDriveService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _usuario;

        public DescargaInformacionOneDriveService(ILogger<DescargaInformacionOneDriveService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _usuario = (_configuration.GetValue<string>("usuario") ?? "");
        }
        public bool DescargaInformacion(ArchivosImagenes archivoADescargar, string carpetaDestino)
        {
            // Aqui cambie la unidad de origen de F: a c:
            string sRutaArchivoOrigen = (archivoADescargar.UrlArchivo ?? "").Replace("f:","g:",StringComparison.InvariantCultureIgnoreCase);
            FileInfo fi = new(sRutaArchivoOrigen);
            string archivoDestino = Path.Combine(carpetaDestino, archivoADescargar.NombreArchivo??"");
            if (!fi.Exists) 
            {
                archivoADescargar.ErrorAlDescargar = true;
                archivoADescargar.MensajeDeErrorAlDescargar = "No existe el archivo en la ruta de origen";
                _logger.LogTrace("{x}", _usuario);
                return false;
            }
            try
            {
                /// Lo estoy obviando para ahorrar tiempo
                if (!File.Exists(archivoDestino))
                {
                    //File.Delete(archivoDestino);
                    Directory.CreateDirectory(carpetaDestino);
                    _logger.LogTrace("Iniciando descarga del archivo {id} con el {nombreArchivoDestino}", archivoADescargar.Id, archivoADescargar.NombreArchivo);
                    File.Copy(fi.FullName, archivoDestino, true);
                    _logger.LogInformation("Se descargó el archivo {id} con el nombre {nombreArchivoDestino} con {numKB} kb", archivoADescargar.Id, archivoADescargar.NombreArchivo, fi.Length / 1024);
                }
                else {
                    //File.Delete(archivoDestino);
                }
            }
            catch (Exception ex)
            {
                archivoADescargar.ErrorAlDescargar = true;
                archivoADescargar.MensajeDeErrorAlDescargar = String.Format("Error al copiar {0}",ex.Message);
                return false;
            }
            return true;
        }
    }
}
