using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel
{
    public interface IServicioABSaldosRegionales
    {
        /// <summary>
        /// Carga Procesa y Limpia ABSaldos antes de preparar cualquier otra situación
        /// </summary>
        /// <param name="archivoOrigen">Ubicación donde se localiza el archivo de AB Saldos Original en .ZIP</param>
        /// <param name="archivoDestino">Nuevo archivo ya limpio y depurado</param>
        /// <returns></returns>
        Task<IEnumerable<ABSaldosRegionales>> CargaProcesaYLimpia(string archivoOrigen="", string archivoDestino="");
    }
}
