using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCargaLiquidaciones
    {
        IEnumerable<ColocacionConPagos> CargaLiquidacionesCompleta(string archivoLiquidaciones = "");
    }
}
