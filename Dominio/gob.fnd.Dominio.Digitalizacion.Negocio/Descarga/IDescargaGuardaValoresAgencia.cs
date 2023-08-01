using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Descarga
{
    public interface IDescargaGuardaValoresAgencia
    {
        Task<bool> DescargaGuardaValoresAgencia(IEnumerable<GuardaValores> guardaValores, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> avance);
    }
}
