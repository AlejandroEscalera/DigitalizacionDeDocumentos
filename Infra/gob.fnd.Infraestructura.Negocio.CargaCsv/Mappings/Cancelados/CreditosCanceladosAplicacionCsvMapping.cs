using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Cancelados;

public class CreditosCanceladosAplicacionCsvMapping : CsvMapping<CreditosCanceladosAplicacionCarga>
{
    public CreditosCanceladosAplicacionCsvMapping()
        : base()
    {
        MapProperty(00, p => p.NumCreditoCancelado);
        MapProperty(01, p => p.NumCreditoOrigen);
        MapProperty(02, p => p.NumRegion);
        MapProperty(03, p => p.CatRegion);
        MapProperty(04, p => p.Agencia);
        MapProperty(05, p => p.CatAgencia);
        MapProperty(06, p => p.NumCliente);
        MapProperty(07, p => p.Acreditado);
        MapProperty(08, p => p.TipoPersona);
        MapProperty(09, p => p.AnioOriginacion);
        MapProperty(10, p => p.PisoCredito);
        MapProperty(11, p => p.Portafolio);
        MapProperty(12, p => p.Concepto);
        MapProperty(13, p => p.FechaCancelacion);
        MapProperty(14, p => p.FechaPrimeraDispersion);
        MapProperty(15, p => p.TieneImagenDirecta);
        MapProperty(16, p => p.TieneImagenIndirecta);
    }
}
