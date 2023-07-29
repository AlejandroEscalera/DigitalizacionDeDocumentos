using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP
{
    public interface IComprimeArchivoZIP
    {
        bool ComprimirArchivo(string archivoAComprimir, string archivoZipFinal, string password);
    }
}
