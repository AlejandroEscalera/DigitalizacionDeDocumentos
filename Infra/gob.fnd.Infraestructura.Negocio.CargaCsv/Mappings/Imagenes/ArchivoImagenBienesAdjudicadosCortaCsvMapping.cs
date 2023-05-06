using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Imagenes
{
    internal class ArchivoImagenBienesAdjudicadosCortaCsvMapping : CsvMapping<ArchivoImagenBienesAdjudicadosCorta>
    {
        public ArchivoImagenBienesAdjudicadosCortaCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Id);
            MapProperty(01, p => p.NumExpediente);
            MapProperty(02, p => p.Acreditados);
            MapProperty(03, p => p.NombreArchivo);
            MapProperty(04, p => p.CarpetaDestino);
            MapProperty(05, p => p.NumPaginas);
            MapProperty(06, p => p.Hash);
        }
    }
}
