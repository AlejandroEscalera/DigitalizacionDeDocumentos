using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados
{
    public interface IBienesAdjudicadosIdentificados
    {
        IEnumerable<IdentificacionClaveBien> ObtieneFuenteBienesAdjudicadosIdentificacion(string archivo = "");
    }
}
