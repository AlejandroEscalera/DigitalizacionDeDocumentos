using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance
{
    public class ReporteProgresoDescompresionArchivos
    {
        public long ArchivoProcesado { get; set; }
        public long CantidadArchivos { get; set; }
        public string? InformacionArchivo { get; set; }
    }
}
