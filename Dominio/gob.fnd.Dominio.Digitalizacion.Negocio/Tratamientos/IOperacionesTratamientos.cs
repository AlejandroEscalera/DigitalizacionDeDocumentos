using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Tratamientos
{
    public interface IOperacionesTratamientos
    {
        /// <summary>
        /// Se obtiene el tratamiento que originó el número de crédito actual
        /// </summary>
        /// <param name="numCredito">Número de crédito de tratamiento</param>
        /// <returns>lista con los números de crédito que originan este crédito</returns>
        IEnumerable<string> ObtieneTratamientosOrigen(string numCredito);
        /// <summary>
        /// Desde un número de crédito o un tratamiento, obtengo todos los tratamientos que se han generado a partir de este
        /// </summary>
        /// <param name="numCredito">Número de credito o número de crédito con tratamiento</param>
        /// <returns>lista de números de crédito de los tratamientos que tiene este crédito</returns>
        IEnumerable<string> ObtieneTratamientos(string numCredito);
    }
}
