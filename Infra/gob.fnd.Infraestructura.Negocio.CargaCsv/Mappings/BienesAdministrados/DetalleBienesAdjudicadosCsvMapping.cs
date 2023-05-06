using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.BienesAdministrados
{
    public class DetalleBienesAdjudicadosCsvMapping : CsvMapping<DetalleBienesAdjudicados>
    {
        public DetalleBienesAdjudicadosCsvMapping()
            : base()
        {
            MapProperty(00, p => p.NumRegion);
            MapProperty(01, p => p.CatRegion);
            MapProperty(02, p => p.Entidad);
            MapProperty(03, p => p.CveBien);
            MapProperty(04, p => p.ExpedienteElectronico);
            MapProperty(05, p => p.Acreditado);
            MapProperty(06, p => p.TipoDeBien);
            MapProperty(07, p => p.DescripcionReducidaBien);
            MapProperty(08, p => p.DescripcionCompletaBien);
            MapProperty(09, p => p.OficioNotificaiconGCRJ);
            MapProperty(10, p => p.NumCredito);
            MapProperty(11, p => p.TipoAdjudicacion);
            MapProperty(12, p => p.AreaResponsable);
            MapProperty(13, p => p.ImporteIndivAdjudicacion);
            MapProperty(14, p => p.ImporteIndivImplicado);
            MapProperty(15, p => p.Estatus);
        }
    }
}
