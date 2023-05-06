using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infaestructura.Negocio.Ocr
{
    public class AdministraAcomodaExpedientesService : IAdministraAcomodaExpedientes
    {
        private readonly ILogger<AdministraOperacionesOCRService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly string _directorioRaizControl;

        public AdministraAcomodaExpedientesService(ILogger<AdministraOperacionesOCRService> logger, IConfiguration configuration, IServicioImagenes servicioImagenes)
        {
            _logger = logger;
            _configuration = configuration;
            _servicioImagenes = servicioImagenes;
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
            return false;

        }
    }
}
