using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraABSaldosDiario
    {
        /// <summary>
        /// Copia el archivo de AB Saldos desde el servidor donde se depositan los cierres diarios directorio del corte, ejemplo: \\172.26.X.X
        /// </summary>
        /// <param name="origen"></param>
        /// <param name="destino"></param>
        /// <returns></returns>
        bool CopiaABSaldosDiario(string origen = "", string destino = "");
        bool MueveABSaldosDiario(string origen = "", string destino = "");
        bool DescargaABSaldosDiario(string origen = "", string destino = "");

        Task<bool> CopiaABSaldosDiarioAync(string origen = "", string destino = "");
        Task<bool> DescargaABSaldosDiarioAync(string origen = "", string destino = "");
    }
}
