using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Rar
{
    public interface IDescomprimeArchivoRar : IDescomprimeArchivo
    {
        const string C_STR_TIPO_ARCHIVO = "RAR";
        // Implementacion ZIP
    }
}
