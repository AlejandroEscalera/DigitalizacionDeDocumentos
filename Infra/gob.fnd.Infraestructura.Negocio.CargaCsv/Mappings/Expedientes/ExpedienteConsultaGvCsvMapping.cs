using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes
{
    public class ExpedienteConsultaGvCsvMapping : CsvMapping<ExpedienteDeConsultaGvCarga>
    {
        public ExpedienteConsultaGvCsvMapping()
            : base()
        {
            MapProperty(00, p => p.TieneGuardaValor);
            MapProperty(01, p => p.NumCreditoCancelado);
            MapProperty(02, p => p.EsSaldosActivo);
            MapProperty(03, p => p.EsCancelado);
            MapProperty(04, p => p.EsOrigenDelDr);
            MapProperty(05, p => p.EsCanceladoDelDr);
            MapProperty(06, p => p.EsCastigado);
            MapProperty(07, p => p.TieneArqueo);
            MapProperty(08, p => p.Acreditado);
            MapProperty(09, p => p.FechaApertura);
            MapProperty(10, p => p.FechaCancelacion);
            MapProperty(11, p => p.Castigo);
            MapProperty(12, p => p.NumProducto);
            MapProperty(13, p => p.CatProducto);
            MapProperty(14, p => p.TipoDeCredito);
            MapProperty(15, p => p.Ejecutivo);
            MapProperty(16, p => p.Analista);
            MapProperty(17, p => p.FechaInicioMinistracion);
            MapProperty(18, p => p.FechaSolicitud);
            MapProperty(19, p => p.MontoCredito);
            MapProperty(20, p => p.InterCont);
            MapProperty(21, p => p.Region);
            MapProperty(22, p => p.Agencia);
            MapProperty(23, p => p.CatRegion);
            MapProperty(24, p => p.CatAgencia);

            MapProperty(25, p => p.EsCreditoAReportar);
            MapProperty(26, p => p.StatusImpago);
            MapProperty(27, p => p.StatusCarteraVencida);
            MapProperty(28, p => p.StatusCarteraVigente);
            MapProperty(29, p => p.TieneImagenDirecta);
            MapProperty(30, p => p.TieneImagenIndirecta);
            MapProperty(31, p => p.SldoTotContval);
            MapProperty(32, p => p.NumCliente);

            MapProperty(33, p => p.NumCredito);

        }
    }
}
