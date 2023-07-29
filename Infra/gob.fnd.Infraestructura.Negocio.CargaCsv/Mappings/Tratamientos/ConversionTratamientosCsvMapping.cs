using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Tratamientos
{
    public class ConversionTratamientosCsvMapping : CsvMapping<ConversionTratamientos>
    {
        public ConversionTratamientosCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Id);
            MapProperty(01, p => p.NumCte);
            MapProperty(02, p => p.Origen);
            MapProperty(03, p => p.Destino);

        }
    }
}
