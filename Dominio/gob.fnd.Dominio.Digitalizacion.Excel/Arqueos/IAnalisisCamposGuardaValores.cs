using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Arqueos
{
    public interface IAnalisisCamposGuardaValores
    {
        IEnumerable<ArchivoAnalisisCamposArqueos> CargaInformacion();
    }
}
