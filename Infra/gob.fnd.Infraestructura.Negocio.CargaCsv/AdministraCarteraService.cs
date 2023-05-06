using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraCarteraService : IAdministraCartera
    {
        public IEnumerable<ABSaldosCompleta> FiltraABSaldosCarteraContableCredito(IEnumerable<ABSaldosCompleta> completa)
        {
            string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');
            var filtaABSaldosCompletaPorProducto = completa.Where(x => !arrayProductosFiltra.Any(y => y.Equals((x.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase))).ToList();
            filtaABSaldosCompletaPorProducto = filtaABSaldosCompletaPorProducto.Where(x => !(x.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase)).ToList();

            return filtaABSaldosCompletaPorProducto.ToList();
        }
    }
}
