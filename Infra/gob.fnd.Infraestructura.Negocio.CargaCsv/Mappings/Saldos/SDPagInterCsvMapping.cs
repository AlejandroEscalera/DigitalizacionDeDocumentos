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
    public class SDPagInterCsvMapping : CsvMapping<SDPagInterCarga>
    {
        public SDPagInterCsvMapping()
            : base()
        {
            MapProperty(00, p => p.NumCredito);
            //MapProperty(01, p => p.FechaCuota, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(01, p => p.FechaCuota);
            MapProperty(02, p => p.CuotaRec);
            MapProperty(03, p => p.NumCuota);
            MapProperty(04, p => p.MontoCuota);
            MapProperty(05, p => p.MontoRealPag);
            //MapProperty(06, p => p.FechaPag, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(06, p => p.FechaPag);
            MapProperty(07, p => p.FactorMoratorio);
            MapProperty(08, p => p.MontoMoratorio);
            //MapProperty(09, p => p.FechaMoratorio, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(09, p => p.FechaMoratorio);
            MapProperty(10, p => p.DiasMoratorio);
            MapProperty(11, p => p.StatusMoratorio);
            MapProperty(12, p => p.BonifiIntMora);
            MapProperty(13, p => p.PorcPago);
            MapProperty(14, p => p.StatusCuota);
            MapProperty(15, p => p.MontoFinanciado);

        }
    }
}
