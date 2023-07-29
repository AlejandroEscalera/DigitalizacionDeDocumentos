using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraTratamientoBase
    {
        IEnumerable<ConversionTratamientos> GetConversionTratamientosCsv(string archivoTratamientoBase);
    }
}
