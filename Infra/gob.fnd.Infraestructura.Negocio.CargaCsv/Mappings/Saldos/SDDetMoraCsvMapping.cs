using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Saldos
{
    public class SDDetMoraCsvMapping : CsvMapping<SDDetMoraCarga>
    {
        public SDDetMoraCsvMapping()
            : base()
        {
            MapProperty(00, p => p.NumCredito);
            MapProperty(01, p => p.IdentifiRec);
            MapProperty(02, p => p.NumCuota);
            MapProperty(03, p => p.SdoAcumMesMora);
            MapProperty(04, p => p.TasaOrdinaria);
            MapProperty(05, p => p.ProviMoraOrdi);
            MapProperty(06, p => p.TasaCopete);
            MapProperty(07, p => p.PriviMoraCope);
            MapProperty(08, p => p.SdoMoraOrdi);
            MapProperty(09, p => p.SdoMoraCope);
        }
    }
}
