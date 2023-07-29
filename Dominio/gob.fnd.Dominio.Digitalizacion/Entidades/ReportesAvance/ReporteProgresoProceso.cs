using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;

public class ReporteProgresoProceso
{
    public long NumeroPasoActual { get; set; }
    public long CantidadDePasos { get; set; }
    public string? EtiquetaDePaso { get; set; }
    public long NumeroProgresoSubProceso { get; set; }
    public long TotalProgresoSubProceso { get; set; }
    public string? EtiquetaSubProceso { get; set; }

    public long ArchivoProcesado { get; set; }
    public long CantidadArchivos { get; set; }
    public string? InformacionArchivo { get; set; }
}
