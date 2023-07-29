using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP
{
    public interface IDescomprimeArchivo
    {
        IList<string> DescomprimeArchivo(string fileName, string carpetaDestino);

        Task<IList<string>> DescomprimeArchivoAsync(string fileName, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> progress);    
    }
}
