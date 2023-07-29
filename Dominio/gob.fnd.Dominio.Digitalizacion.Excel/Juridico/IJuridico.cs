using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Juridico
{
    public interface IJuridico
    {
        IEnumerable<ExpedienteJuridico> GetExpedientesJuridicos(string archivoDeExpedientes = "");
        Task<IEnumerable<ExpedienteJuridico>> GetExpedientesJuridicosAsync(string archivoDeExpedientes = "");
    }
}
