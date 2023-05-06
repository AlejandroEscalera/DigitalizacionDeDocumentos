using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface ICreaArchivoCsv
    {
        bool CreaArchivoCsv(string archivoDestino, IEnumerable<Object> detalleAGuardar, bool otroEncoding = true);
    }
}
