using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCargaCorteDiario
    {
        IEnumerable<ABSaldosCompleta> CargaABSaldosCompleta(string archivoABSaldosCompleta = "");
        IEnumerable<ABSaldosCredit> CargaABSaldosCredit(string archivoABSaldosCredit = "");
        IEnumerable<ABSaldosCuenta> CargaABSaldosCuenta(string archivoABSaldosCuenta = "");
        IEnumerable<ABSaldosEspecial> CargaABSaldosEspecial(string archivoABSaldosEspecial = "");
        IEnumerable<ABSaldosGen> CargaABSaldosGen(string archivoABSaldosGen = "");
        IEnumerable<SDPagoCapit> CargaSDPagoCapit(string archivoSDPagoCapit = "");
        IEnumerable<SDPagInter> CargaSDPagInter(string archivoSDPagInter = "");
        IEnumerable<SDDetMora> CargaSDDetMor(string archivoSDDetMora = "");
        IEnumerable<SDMovDia> CargaSDMovDia(string archivoSDMovDia = "");
    }
}
