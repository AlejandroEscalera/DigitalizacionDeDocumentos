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
    public class SDPagoCapitCsvMapping : CsvMapping<SDPagoCapitCarga>
    {
        public SDPagoCapitCsvMapping()
            : base()
        {
            MapProperty(00, p => p.NumCredito);
            MapProperty(01, p => p.FechaCuota, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(02, p => p.CuotaRec);
            MapProperty(03, p => p.NumCuota);
            MapProperty(04, p => p.MontoCuota);
            MapProperty(05, p => p.SaldoCuota);
            MapProperty(06, p => p.ImpCapitalizado);
            MapProperty(07, p => p.FactorAjuste);
            MapProperty(08, p => p.MontoRealPag);
            MapProperty(09, p => p.FechaPago);
            MapProperty(10, p => p.FactorMoratorio);
            MapProperty(11, p => p.MontoMoratorio);
            MapProperty(12, p => p.FechaMoratorio);
            MapProperty(13, p => p.DiasMoratorios);
            MapProperty(14, p => p.StatusMoratorio);
            MapProperty(15, p => p.NumPagares);
            MapProperty(16, p => p.PorcPago);
            MapProperty(17, p => p.BanderaMinistra);
            MapProperty(18, p => p.StatusCuota);
        }
    }
}
