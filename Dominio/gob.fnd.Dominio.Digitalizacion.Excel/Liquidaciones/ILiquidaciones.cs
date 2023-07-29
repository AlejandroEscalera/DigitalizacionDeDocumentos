using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Liquidaciones
{
    /// <summary>
    /// Servicio de Liquidaciones
    /// </summary>
    public interface ILiquidaciones
    {
        /// <summary>
        /// Obtiene la lista de las colocaciones con pagos
        /// </summary>
        /// <returns>Lista de créditos colocados</returns>
        IEnumerable<ColocacionConPagos> GetColocacionConPagos(string archivoLiquidaciones);
        /// <summary>
        /// Obtiene la lista de las colocaciones con pagos
        /// </summary>
        /// <returns>Lista de créditos colocados</returns>
        Task<IEnumerable<ColocacionConPagos>> GetColocacionConPagosAsync(string archivoLiquidaciones);
    }
}
