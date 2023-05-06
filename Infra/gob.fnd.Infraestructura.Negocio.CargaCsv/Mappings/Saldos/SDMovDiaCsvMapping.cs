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
    public class SDMovDiaCsvMapping : CsvMapping<SDMovDiaCarga>
    {
        public SDMovDiaCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Secuencia);
            //MapProperty(01, p => p.FechaMov, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(01, p => p.FechaMov);
            MapProperty(02, p => p.HoraMov);
            MapProperty(03, p => p.Sucursal);
            MapProperty(04, p => p.NumCredito);
            MapProperty(05, p => p.Plaza);
            MapProperty(06, p => p.TransaccSuc);
            MapProperty(07, p => p.Usuario);
            MapProperty(08, p => p.Monto);
            MapProperty(09, p => p.CodigoFun);
            MapProperty(10, p => p.CodigoRef);
            MapProperty(11, p => p.Divisa);
            MapProperty(12, p => p.Reversado);
            MapProperty(13, p => p.FolioSuc);
            MapProperty(14, p => p.NumProducto);

        }
    }
}
