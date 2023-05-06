using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP
{
    public interface IProcesaArchivoCompreso
    {
        IEnumerable<ArchivosImagenes> ObtieneArchivosDesde(ArchivosImagenes origen, string fileName, string directorioDestino, ref int subArchivo);
    }
}
