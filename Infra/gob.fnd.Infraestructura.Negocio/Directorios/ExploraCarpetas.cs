using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Negocio.Directorios;
using gob.fnd.Infraestructura.Negocio.Main;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Directorios
{
    public class ExploraCarpetas : IExploraCarpetas
    {
        private readonly ILogger<ExploraCarpetas> _logger;
        private readonly IConfiguration _configuration;
        private readonly string? _carpetaArqueos;

        public ExploraCarpetas(ILogger<ExploraCarpetas> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _carpetaArqueos = _configuration.GetValue<string>("carpetaArqueos") ?? "";
            _carpetaArqueos += Path.DirectorySeparatorChar;
        }
        public IEnumerable<ArchivosArqueos> ObtieneListaArchivosDeArqueos()
        {
            if (!Directory.Exists(_carpetaArqueos))
                _logger.LogError("No existe la carpeta para obtener la información de los archivos de los arqueos {carpetaArqueos}", _carpetaArqueos);
            IList<ArchivosArqueos> resultado = new List<ArchivosArqueos>();
            int numeroArchivos = 0;
            resultado =  ObtieneArchivos(_carpetaArqueos??"", numeroArchivos, resultado);
            return resultado.ToArray<ArchivosArqueos>();
        }

        private IList<ArchivosArqueos> ObtieneArchivos(string directorioBusqueda, int numeroArchivos, IList<ArchivosArqueos> resultado)
        {
            
            string[] carpetas = Directory.GetDirectories(directorioBusqueda);
            string[] archivos = Directory.GetFiles(directorioBusqueda, "*.xls*");

            numeroArchivos = AniadeRango(resultado, numeroArchivos, archivos, _carpetaArqueos??"");
            foreach (string carpeta in carpetas)
            {
                AniadeRango(ObtieneArchivos(carpeta, numeroArchivos, resultado), numeroArchivos, archivos, _carpetaArqueos??"");
            }
            return resultado;
        }

        private static int AniadeRango(IList<ArchivosArqueos> resultado, int numeroArchivos, string[] archivos, string carpetaArqueos)
        {
            foreach (string archivo in archivos)
            {
                numeroArchivos++;
                FileInfo fi = new(archivo);
                ArchivosArqueos archivosArqueo = new()
                {
                    Id = numeroArchivos,
                    RutaCompleta = fi.FullName,
                    Carpeta = fi.DirectoryName,
                    NombreArchivo = fi.Name
                };
                archivosArqueo.Carpeta = (archivosArqueo.Carpeta ?? "").Replace(carpetaArqueos, "", StringComparison.OrdinalIgnoreCase);
                resultado.Add(archivosArqueo);
            }

            return numeroArchivos;
        }
    }
}
