using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones
{
    public interface IServicioMinistracionesMesa
    {
        /// <summary>
        /// Obtiene una lista de Ministraciones de la mesa
        /// </summary>
        /// <returns>Lista de ministraciones</returns>
        IEnumerable<MinistracionesMesa> GetMinistraciones();
        /// <summary>
        /// Obtiene una lista de Ministraciones de la mesa
        /// </summary>
        /// <returns>Lista de ministraciones</returns>
        Task<IEnumerable<MinistracionesMesa>> GetMinistracionesAsync();
        /// <summary>
        /// Guarda la información de las ministraciones
        /// </summary>
        /// <param name="lista">Lista de ministraciones despues de cruce</param>
        /// <returns>Si lo pudo guardar o no</returns>
        bool GuardaMinistraciones(IEnumerable<MinistracionesMesa> lista);
        /// <summary>
        /// Obtiene las ministraciones Procesadas
        /// </summary>
        /// <returns>Lista de ministraciones</returns>
        IEnumerable<MinistracionesMesa> GetMinistracionesProcesadas();
    }
}
