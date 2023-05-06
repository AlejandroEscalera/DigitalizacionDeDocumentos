using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Imagenes
{
    public class ArchivoImagenCortaCsvMapping : CsvMapping<ArchivoImagenCorta>
    {
        public ArchivoImagenCortaCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Id);
            MapProperty(01, p => p.NumCredito);
            MapProperty(02, p => p.NombreArchivo);
            MapProperty(03, p => p.CarpetaDestino);
            MapProperty(04, p => p.NumPaginas);
            MapProperty(05, p => p.Hash);
        }
    }
}
