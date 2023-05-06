using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using PnP.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using PnP.Framework;
using System.IO;
using System.Net;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Descarga
{
    public class DescargaInformacionSharePointService : IDescargaInformacionSharePoint
    {
        private readonly ILogger<DescargaInformacionSharePointService> _logger;
        private readonly IConfiguration _configuration;
        private PnP.Framework.AuthenticationManager? _authManager;
        private ClientContext? _sharePointContext;

        private string _lastSiteSharePoint = string.Empty;

        #region sec
        private readonly string _usuario;
        private readonly SecureString _password;
        #endregion

        public DescargaInformacionSharePointService(ILogger<DescargaInformacionSharePointService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _authManager = null;
            _sharePointContext = null;
            #region sec
            _password = (_configuration.GetValue<string>("contrasenia") ?? "").ToSecureString();
            _usuario = (_configuration.GetValue<string>("usuario") ?? "");
            #endregion
        }
        public bool DescargaInformacion(ArchivosImagenes archivoADescargar, string carpetaDestino)
        {
            try
            {
                string siteSharePoint = ObtieneSiteADescargar(archivoADescargar.UrlArchivo ?? "");
                string urlArchivoOrigen = archivoADescargar.UrlArchivo ?? "";
                string archivoDestino = Path.Combine(carpetaDestino, archivoADescargar.NombreArchivo ?? "");

                if (!siteSharePoint.Equals(_lastSiteSharePoint))
                {
                    _authManager = PnP.Framework.AuthenticationManager.CreateWithCredentials(_usuario, _password.Decrypt());
                    _sharePointContext = _authManager.GetContext(siteSharePoint);
                    _lastSiteSharePoint = siteSharePoint;
                }

                if (_sharePointContext != null)
                {
                    Microsoft.SharePoint.Client.File spFile = _sharePointContext.Web.GetFileByUrl(urlArchivoOrigen);
                    ClientResult<Stream> crstream = spFile.OpenBinaryStream();

                    _sharePointContext.Load(spFile);
                    _logger.LogTrace("Iniciando descarga del archivo {id} con el nombre {nombreArchivoDestino}", archivoADescargar.Id, archivoADescargar.NombreArchivo);
                    _sharePointContext.ExecuteQuery();
                    FileInfo fiArchivoDestino = new(archivoDestino);
                    if (fiArchivoDestino.Exists)
                        fiArchivoDestino.Delete();
                    if (!Directory.Exists(fiArchivoDestino.DirectoryName))
                        Directory.CreateDirectory(fiArchivoDestino.DirectoryName ?? "");
                    using (FileStream fs = System.IO.File.OpenWrite(archivoDestino))
                    {
                        crstream.Value.CopyTo(fs);
                    }
                    _logger.LogInformation("Se descargó el archivo {id} con el nombre {nombreArchivoDestino} con {numKB} kb", archivoADescargar.Id, archivoADescargar.NombreArchivo, spFile.Length / 1024);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al descargar {mensaje}", ex.Message);
                archivoADescargar.ErrorAlDescargar = true;
                archivoADescargar.MensajeDeErrorAlDescargar = ex.Message;
                return false;
            }
            return true;
        }

        private static string ObtieneSiteADescargar(string urlDeDescarga)
        {
            if (urlDeDescarga.Contains("sites/Paperless", StringComparison.InvariantCultureIgnoreCase))
            {
                return "https://fndgob.sharepoint.com/sites/Paperless/";
            }
            if (urlDeDescarga.Contains("personal/drivemesacontrol_fnd_gob_mx"))
                return "https://fndgob-my.sharepoint.com/personal/drivemesacontrol_fnd_gob_mx/MesaControl/";
            throw new Exception("Sitio no encontrado");
        }
    }
}
