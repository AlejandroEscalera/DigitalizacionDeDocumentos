using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga
{
    public interface IProcesaDescargaInformacion
    {
        bool DescargaArchivo(ArchivosImagenes origen);
    }
}
