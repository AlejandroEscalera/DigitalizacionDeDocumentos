using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC
{
    public interface IServicioABSaldosActivos
    {
        /// <summary>
        /// Obtiene una lista de AB Saldos activos desde el archivo de origen
        /// </summary>
        /// <returns>Lista de ABSaldos activos</returns>
        IEnumerable<ABSaldosActivos> GetABSaldosActivos();
        /// <summary>
        /// Guarda la información resultante de los AB Saldos activos
        /// </summary>
        /// <param name="lista">Lista de registros por guardar</param>
        /// <returns>Si lo pudo guardar o no</returns>
        bool GuardaABSaldosActivos(IEnumerable<ABSaldosActivos> lista);
        /// <summary>
        /// Agrega la información de los agentes y de los guarda valores
        /// </summary>
        /// <param name="origen"></param>
        /// <param name="agentes"></param>
        /// <returns></returns>
        IEnumerable<ABSaldosActivos> AgregaAgentesYGuardaValores(IEnumerable<ABSaldosActivos> origen, IEnumerable<CorreosAgencia> agentes);
        /// <summary>
        /// Igual que GetABSaldosActivos, solo que trae la información previamente procesada para no recalcularse
        /// </summary>
        /// <returns>Lista de ABSaldos activos</returns>
        IEnumerable<ABSaldosActivos> ObtieneABSaldosActivosProcesados();
        /// <summary>
        /// Obtiene una lista de AB Saldos activos desde el archivo de origen
        /// </summary>
        /// <returns>Lista de ABSaldos activos</returns>
        Task<IEnumerable<ABSaldosActivos>> GetABSaldosActivosAsync();
    }
}
