using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Ocr
{
    public class FiltroBusquedaOcr
    {
        /// <summary>
        /// Número de página
        /// </summary>
        public int Pagina { get; set; }
        /// <summary>
        /// Suma de dos string's encontrados antes del valor
        /// </summary>
        public string? Busqueda { get; set; }
        /// <summary>
        /// Valor que cumple el criterio de búsqueda
        /// </summary>
        public string? Valor { get; set; }
    }
}
