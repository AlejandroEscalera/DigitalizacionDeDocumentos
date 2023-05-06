using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf
{
    public interface IFile2Pdf
    {
        /// <summary>
        /// Convierte un documento a pdf
        /// </summary>
        /// <param name="archivo"></param>
        /// <param name="directorioDestino"></param>
        /// <returns></returns>
        IList<string> ConvierteArchivo(string archivo, string directorioDestino);
    }
}
