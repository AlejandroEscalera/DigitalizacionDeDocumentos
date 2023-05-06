using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP
{
    public interface IDescomprimeArchivo
    {
        IList<string> DescomprimeArchivo(string fileName, string carpetaDestino);
    }
}
