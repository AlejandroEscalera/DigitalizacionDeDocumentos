using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.ABSaldos
{
    public class ABSaldosCuentaCsvMapping : CsvMapping<ABSaldosCuentaCarga>
    {
        public ABSaldosCuentaCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Coordinacion);
            MapProperty(01, p => p.Agencia);
            MapProperty(02, p => p.Moneda);
            MapProperty(03, p => p.NumCredito);
            MapProperty(04, p => p.NumProducto);
            MapProperty(05, p => p.StatusCred);
            MapProperty(06, p => p.StatusCont);
            MapProperty(07, p => p.CapVig);
            MapProperty(08, p => p.CtaCapVig);
            MapProperty(09, p => p.CapVen);
            MapProperty(10, p => p.CtaCapVen);
            MapProperty(11, p => p.IntFinVig);
            MapProperty(12, p => p.CtaIntFinVig);
            MapProperty(13, p => p.IntFinVigNp);
            MapProperty(14, p => p.CtaIntFinVigNp);
            MapProperty(15, p => p.IntFinVen);
            MapProperty(16, p => p.CtaIntFinVen);
            MapProperty(17, p => p.IntFinVenNp);
            MapProperty(18, p => p.CtaIntFinVenNp);
            MapProperty(19, p => p.IntNorVig);
            MapProperty(20, p => p.CtaIntNorVig);
            MapProperty(21, p => p.IntNorVigNp);
            MapProperty(22, p => p.CtaIntNorVigNp);
            MapProperty(23, p => p.IntNorVen);
            MapProperty(24, p => p.CtaIntNorVen);
            MapProperty(25, p => p.IntNorVenNp);
            MapProperty(26, p => p.CtaIntNorVenNp);
            MapProperty(27, p => p.IntDesVen);
            MapProperty(28, p => p.CtaIntDesVen);
            MapProperty(29, p => p.IntPen);
            MapProperty(30, p => p.CtaIntPen);
            MapProperty(31, p => p.Sector);
        }
    }
}
