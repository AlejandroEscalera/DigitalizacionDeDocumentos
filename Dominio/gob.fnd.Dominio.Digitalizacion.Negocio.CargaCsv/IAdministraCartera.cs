using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCartera
    {
        /// <summary>
        /// Filtra la cartera para empatarla con el AB Saldos de crédito
        /// </summary>
        /// <param name="completa">AB Saldos completo</param>
        /// <returns>ABSaldos filtrado</returns>
        IEnumerable<ABSaldosCompleta> FiltraABSaldosCarteraContableCredito(IEnumerable<ABSaldosCompleta> completa);
    }
}
