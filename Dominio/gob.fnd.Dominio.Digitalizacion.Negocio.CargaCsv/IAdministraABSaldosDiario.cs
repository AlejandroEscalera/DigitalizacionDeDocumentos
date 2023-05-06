using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraABSaldosDiario
    {
        bool CopiaABSaldosDiario(string origen = "", string destino = "");
        bool MueveABSaldosDiario(string origen = "", string destino = "");        
    }
}
