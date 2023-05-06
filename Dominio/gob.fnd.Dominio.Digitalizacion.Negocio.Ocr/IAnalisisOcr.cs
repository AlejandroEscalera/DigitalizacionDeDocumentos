using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Ocr
{
    public interface IAnalisisOcr
    {
        bool ObtieneInformacionOCR(string archivoOrigen, ref string cadenaCredito, ref string cadenaCliente, ref string numeroCliente, ref string numeroCredito);
    }
}
