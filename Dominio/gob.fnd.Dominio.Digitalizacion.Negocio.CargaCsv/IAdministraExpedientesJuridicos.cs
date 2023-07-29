using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraExpedientesJuridicos
    {
        /// <summary>
        /// Carga los expedientes jurídicos
        /// </summary>
        /// <param name="archivoExpedientesJuridicos"></param>
        /// <returns></returns>
        IEnumerable<ExpedienteJuridico> CargaExpedientesJuridicos(string archivoExpedientesJuridicos = "");
    }
}
