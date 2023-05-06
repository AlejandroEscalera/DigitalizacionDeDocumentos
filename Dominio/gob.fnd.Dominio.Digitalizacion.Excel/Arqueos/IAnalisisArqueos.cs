using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Arqueos
{
    public interface IAnalisisArqueos
    {
        /// <summary>
        /// Guarda la información de los arqueos
        /// </summary>
        /// <param name="informacion">Detalle de los archivos encontrados</param>
        /// <param name="archivoDestino">Archivo de Excel que servirá de control y análisis</param>
        /// <returns></returns>
        bool GuardaInformacionArqueos(IEnumerable<ArchivosArqueos> informacion, string archivoDestino);
        /// <summary>
        /// Abre cada archivo de arqueos y analisa la información contenida
        /// </summary>
        /// <param name="informacion">lista de archivos a analizar</param>
        /// <returns></returns>
        bool AnalisaArchivosArqueos(IEnumerable<ArchivosArqueos> informacion);
    }
}
