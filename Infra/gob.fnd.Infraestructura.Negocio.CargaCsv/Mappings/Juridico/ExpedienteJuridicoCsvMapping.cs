using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Entidades.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Juridico;
public class ExpedienteJuridicoCsvMapping : CsvMapping<ExpedienteJuridicoCarga>
{
    public ExpedienteJuridicoCsvMapping()
        : base()
    {
        MapProperty(00, p => p.Region);
        
        MapProperty(01, p => p.CatRegion);
        MapProperty(02, p => p.Agencia);
        MapProperty(03, p => p.CatAgencia);
        MapProperty(04, p => p.Estado);
        MapProperty(05, p => p.NombreDemandado);
        MapProperty(06, p => p.TipoCredito);
        MapProperty(07, p => p.NumContrato);
        MapProperty(08, p => p.NumCte);
        MapProperty(09, p => p.NumCredito);
        MapProperty(10, p => p.NumCreditos);
        MapProperty(11, p => p.FechaTranspasoJuridico);
        MapProperty(12, p => p.FechaTranspasoExterno);
        MapProperty(13, p => p.FechaDemanda);
        MapProperty(14, p => p.Juicio);
        MapProperty(15, p => p.CapitalDemandado);
        MapProperty(16, p => p.Dlls);
        MapProperty(17, p => p.JuzgadoUbicacion);
        MapProperty(18, p => p.Expediente);
        MapProperty(19, p => p.ClaveProcesal);
        MapProperty(20, p => p.EtapaProcesal);
        MapProperty(21, p => p.Rppc);
        MapProperty(22, p => p.Ran);
        MapProperty(23, p => p.RedCredAgricola);
        MapProperty(24, p => p.FechaRegistro);
        MapProperty(25, p => p.Tipo);
        MapProperty(26, p => p.TipoGarantias);
        MapProperty(27, p => p.Descripcion);
        MapProperty(28, p => p.ValorRegistro);
        MapProperty(29, p => p.FechaAvaluo);
        MapProperty(30, p => p.GradoPrelacion);
        MapProperty(31, p => p.CAval);
        MapProperty(32, p => p.SAval);
        MapProperty(33, p => p.Inscripcion);
        MapProperty(34, p => p.Clave);
        MapProperty(35, p => p.NumFolio);
        MapProperty(36, p => p.AbogadoResponsable);
        MapProperty(37, p => p.Observaciones);
        MapProperty(38, p => p.FechaUltimaActuacion);
        MapProperty(39, p => p.DescripcionActuacion);
        MapProperty(40, p => p.Piso);
        MapProperty(41, p => p.Fonaga);
        MapProperty(42, p => p.PequenioProductor);
        MapProperty(43, p => p.FondoMutual);
        MapProperty(44, p => p.ExpectativasRecuperacion);
        MapProperty(45, p => p.TieneImagenDirecta);
        MapProperty(46, p => p.TieneImagenIndirecta);
        MapProperty(47, p => p.TieneImagenExpediente);
        /**/
    }
}
