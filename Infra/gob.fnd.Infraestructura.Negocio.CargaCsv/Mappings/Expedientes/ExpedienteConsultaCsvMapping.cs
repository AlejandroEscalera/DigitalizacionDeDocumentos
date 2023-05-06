using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes
{
    public class ExpedienteConsultaCsvMapping : CsvMapping<ExpedienteDeConsultaCarga>
    {
        public ExpedienteConsultaCsvMapping()
            : base()
        {
            MapProperty(00, p => p.NumCreditoCancelado);
            MapProperty(01, p => p.EsSaldosActivo);
            MapProperty(02, p => p.EsCancelado);
            MapProperty(03, p => p.EsOrigenDelDr);
            MapProperty(04, p => p.EsCanceladoDelDr);
            MapProperty(05, p => p.EsCastigado);
            MapProperty(06, p => p.TieneArqueo);
            MapProperty(07, p => p.Acreditado);
            MapProperty(08, p => p.FechaApertura);
            MapProperty(09, p => p.FechaCancelacion);
            MapProperty(10, p => p.Castigo);
            MapProperty(11, p => p.NumProducto);
            MapProperty(12, p => p.CatProducto);
            MapProperty(13, p => p.TipoDeCredito);
            MapProperty(14, p => p.Ejecutivo);
            MapProperty(15, p => p.Analista);
            MapProperty(16, p => p.FechaInicioMinistracion);
            MapProperty(17, p => p.FechaSolicitud);
            MapProperty(18, p => p.MontoCredito);
            MapProperty(19, p => p.InterCont);
            MapProperty(20, p => p.Region);
            MapProperty(21, p => p.Agencia);
            MapProperty(22, p => p.CatRegion);
            MapProperty(23, p => p.CatAgencia);

            MapProperty(24, p => p.EsCreditoAReportar);
            MapProperty(25, p => p.StatusImpago);
            MapProperty(26, p => p.StatusCarteraVencida);
            MapProperty(27, p => p.StatusCarteraVigente);
            MapProperty(28, p => p.TieneImagenDirecta);
            MapProperty(29, p => p.TieneImagenIndirecta);
            MapProperty(30, p => p.SldoTotContval);
            MapProperty(31, p => p.NumCliente);

            MapProperty(32, p => p.NumCredito);
        }
    }
}
