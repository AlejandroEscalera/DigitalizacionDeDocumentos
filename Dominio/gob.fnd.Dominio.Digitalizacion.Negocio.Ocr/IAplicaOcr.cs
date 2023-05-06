using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Ocr
{
    public interface IAplicaOcr
    {
        /// <summary>
        /// Obtiene la información de OCR desde un archivp
        /// </summary>
        /// <param name="nombreArchivo">Nombre del archivo al que se le aplicará el OCR</param>
        /// /// <param name="numeroDePaginas">El número de páginas que tiene el pdf</param>
        /// <param name="paginaInicial">página inicial donde comenzará el OCR</param>
        /// <returns>Si se pudo o no aplicar el OCR</returns>
        bool ObtieneOcrDesdeArchivo(string nombreArchivo, int numeroDePaginas, int paginaInicial = 1);
    }
}
