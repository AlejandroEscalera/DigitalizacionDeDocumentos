using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Config
{
    public interface IObtieneCorreosAgentes
    {
        /// <summary>
        /// Obtiene una lista de Agencias, Agentes, Guarda Valores y sus correos.
        /// </summary>
        /// <returns>Lista de correos por agencia</returns>
        IEnumerable<CorreosAgencia> ObtieneTodosLosCorreosYAgentes();
    }
}
