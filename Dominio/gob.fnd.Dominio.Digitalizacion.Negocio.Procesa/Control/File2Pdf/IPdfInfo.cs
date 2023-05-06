using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf
{
    public interface IPdfInfo
    {
        int DameElNumeroDePaginas(string fileName);
    }
}
