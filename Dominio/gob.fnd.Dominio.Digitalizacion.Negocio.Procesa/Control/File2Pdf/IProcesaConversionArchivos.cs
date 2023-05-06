using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf
{
    public interface IProcesaConversionArchivos
    {
        /// <summary>
        /// Convierte un archivo a PDF
        /// </summary>
        /// <param name="origen">Información del registro a procesar</param>
        /// <param name="fileName">Información del archivo de origen</param>
        /// <param name="directorioDestino">directorio donde quedará el archivo</param>
        /// <param name="subArchivo">número de subarchivo relacionado</param>
        /// <returns></returns>
        ArchivosImagenes ConvierteArchivoDesde(ArchivosImagenes origen, string fileName, string directorioDestino, ref int subArchivo);
    }
}
