using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCargaCreditosCancelados
    {
        IEnumerable<CreditosCanceladosAplicacion> CargaExpedienteCancelados(string archivoDeExpedientesCancelados = "");

    }
}
