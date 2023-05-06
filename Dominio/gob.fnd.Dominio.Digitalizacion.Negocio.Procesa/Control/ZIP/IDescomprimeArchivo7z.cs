using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP
{
    public interface IDescomprimeArchivo7z : IDescomprimeArchivo
    {
        const string C_STR_TIPO_ARCHIVO = "7Z";
        /// Se maneja un 7z
    }
}
