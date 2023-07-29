using gob.fnd.Dominio.Digitalizacion.Entidades.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Liquidaciones;

public class ColocacionConPagosCsvMapping : CsvMapping<ColocacionConPagosCarga>
{
    public ColocacionConPagosCsvMapping()
        : base()
    {
        MapProperty(00, p => p.Agencia);
        MapProperty(01, p => p.CatAgencia);
        MapProperty(02, p => p.NumCte);
        MapProperty(03, p => p.Acreditado);
        MapProperty(04, p => p.NumCredito);
        MapProperty(05, p => p.FechaApertura);
        MapProperty(06, p => p.FechaVencim);
        MapProperty(07, p => p.MontoOtorgado);
        MapProperty(08, p => p.CatRegionMigrado);
        MapProperty(09, p => p.CatEstadoMigrado);
        MapProperty(10, p => p.CatAgenciaMigrado);
        MapProperty(11, p => p.Ministraciones);
        MapProperty(12, p => p.FecPrimMinistra);
        MapProperty(13, p => p.FecUltimaMinistra);
        MapProperty(14, p => p.MontoMinistrado);
        MapProperty(15, p => p.Reestructura);
        MapProperty(16, p => p.Cancelacion);
        MapProperty(17, p => p.PagoCapital);
        MapProperty(18, p => p.PagoInteres);
        MapProperty(19, p => p.PagoMoratorios);
        MapProperty(20, p => p.PagoTotal);
        MapProperty(21, p => p.TieneImagenDirecta);
        MapProperty(22, p => p.TieneImagenIndirecta);
    }
}
