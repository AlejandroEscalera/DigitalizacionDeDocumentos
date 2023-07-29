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
    public class ProcesaDescargaInformacionService : IProcesaDescargaInformacion
    {
        private readonly ILogger<ProcesaDescargaInformacionService> _logger;
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private readonly IDescargaInformacionOneDrive _descargaInformacionOneDrive;
        private readonly IDescargaInformacionSharePoint _descargaInformacionSharePoint;

        public ProcesaDescargaInformacionService(ILogger<ProcesaDescargaInformacionService> logger, IConfiguration configuration, IDescargaInformacionOneDrive descargaInformacionOneDrive, IDescargaInformacionSharePoint descargaInformacionSharePoint)
        {
            _logger = logger;
            _configuration = configuration;
            _descargaInformacionOneDrive = descargaInformacionOneDrive;
            _descargaInformacionSharePoint = descargaInformacionSharePoint;
        }

        public bool DescargaArchivo(ArchivosImagenes origen)
        {
            string primerLimpieza = (origen.NombreArchivo ?? "").Replace(",", "_").Replace(";", "_");
            /// string baseFileName = Path.Combine((origen.CarpetaDestino ?? "").Replace(",", "_").Replace(";", "_"), primerLimpieza);
            origen.NombreArchivo = primerLimpieza;
            // Aqui se cambia 
            origen.CarpetaDestino = (origen.CarpetaDestino ?? "").Replace(",", "_").Replace(";", "_").Replace("F:\\","F:\\"); /// Quite el cambio de ruta 
            origen.UrlArchivo = (origen.UrlArchivo ?? "").Replace("F:\\", "Y:\\");
            IDescargaInformacion descargaInformacion;
            descargaInformacion = SeleccionaDescargaInformacionFactory(origen);
            return (descargaInformacion.DescargaInformacion(origen, origen.CarpetaDestino));
        }

        private IDescargaInformacion SeleccionaDescargaInformacionFactory(ArchivosImagenes origen)
        {
            IDescargaInformacion descargaInformacion;
            if ((origen.UrlArchivo ?? "").Contains("https:", StringComparison.InvariantCultureIgnoreCase))
            {
                descargaInformacion = _descargaInformacionSharePoint;
                _logger.LogTrace("Se decidio descargar de SharePoint");
            }
            else
            {
                descargaInformacion = _descargaInformacionOneDrive;
                _logger.LogTrace("Se decidio descargar de OneDrive");
            }
            
            return descargaInformacion;
        }
    }
}

