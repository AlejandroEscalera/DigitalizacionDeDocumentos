using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Carga
{
    public interface ICargaImagenes
    {
        Task<IEnumerable<ArchivoImagenCorta>> CargaImagenesLiquidadosAsync(string directorioOrigen, IProgress<ReporteProgresoProceso> progreso, string directorioTemporal ="");
        Task<IList<string>> ObtieneListaDirectorios(string directorioOrigen);

        Task<IEnumerable<ArchivoImagenCorta>> CargaImagenesGuardaValoresAsync(string directorioOrigen, IProgress<ReporteProgresoProceso> progreso, string unidadTemporal = "");
    }
}
