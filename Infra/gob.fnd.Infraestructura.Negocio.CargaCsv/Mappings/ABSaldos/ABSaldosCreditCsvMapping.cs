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
    public class ABSaldosCreditCsvMapping : CsvMapping<ABSaldosCreditCarga>
    {
        public ABSaldosCreditCsvMapping()
            : base()
        {
            MapProperty(00, p => p.Coord);
            MapProperty(01, p => p.Sucursal);
            MapProperty(02, p => p.Numprom);
            MapProperty(03, p => p.Nomprom);
            MapProperty(04, p => p.Numcte);
            MapProperty(05, p => p.Nomcte);
            MapProperty(06, p => p.CodProd);
            MapProperty(07, p => p.CodCalifica);
            MapProperty(08, p => p.Sector);
            MapProperty(09, p => p.TpoPersona);
            MapProperty(10, p => p.EstadoInegi);
            MapProperty(11, p => p.NumContrato);
            MapProperty(12, p => p.NumCredito);
            MapProperty(13, p => p.TipoCredito);
            MapProperty(14, p => p.NumProducto);
            MapProperty(15, p => p.Divisa);
            MapProperty(16, p => p.StatusContable);
            MapProperty(17, p => p.TipoCartera);
            /*
            MapProperty(18, p => p.FechaApertura);
            MapProperty(19, p => p.FechaVencim);
            */
            MapProperty(18, p => p.FechaApertura, new DateTimeConverter("MM/dd/yyyy"));
            MapProperty(19, p => p.FechaVencim, new DateTimeConverter("MM/dd/yyyy"));
            //MapProperty(20, p => p.Fecha1Aminis, new DateTimeConverter("MM/dd/yyyy"));
            /*
            */
            MapProperty(20, p => p.Fecha1Aminis);
            MapProperty(21, p => p.TasaInteres);
            MapProperty(22, p => p.Factor);
            MapProperty(23, p => p.PuntosAdic);
            MapProperty(24, p => p.TasaFijaOVar);
            MapProperty(25, p => p.CodTasaBase);
            MapProperty(26, p => p.CobroCapital);
            MapProperty(27, p => p.CobroInteres);
            MapProperty(28, p => p.Actividad);
            MapProperty(29, p => p.RelGarcred);
            MapProperty(30, p => p.CodLinea);
            MapProperty(31, p => p.CodInversion);
            MapProperty(32, p => p.Superficie);
            MapProperty(33, p => p.CodCaract);
            MapProperty(34, p => p.CodCaract2);
            MapProperty(35, p => p.CodAgricola);
            MapProperty(36, p => p.ApoyoFirco);
            MapProperty(37, p => p.FechaApyfirco);
            MapProperty(38, p => p.EsUnionCredito);
            MapProperty(39, p => p.CodSujetoFj);
            MapProperty(40, p => p.CalEdosFin);
            MapProperty(41, p => p.MontoAutorizado);
            MapProperty(42, p => p.MontoEjercido);
            MapProperty(43, p => p.MontoDisponible);
            MapProperty(44, p => p.CapVig);
            MapProperty(45, p => p.IntFinVig);
            MapProperty(46, p => p.IntFinVigNp);
            MapProperty(47, p => p.CapVen);
            MapProperty(48, p => p.IntFinVen);
            MapProperty(49, p => p.IntFinVenNp);
            MapProperty(50, p => p.IntNorVig);
            MapProperty(51, p => p.IntNorVigNp);
            MapProperty(52, p => p.IntCobAnt2601);
            MapProperty(53, p => p.IntNorVen);
            MapProperty(54, p => p.IntNorVenNp);
            MapProperty(55, p => p.IntDesVen);
            MapProperty(56, p => p.IntPen);
            MapProperty(57, p => p.FechaIngusgaap);
            MapProperty(58, p => p.CuotasCap);
            MapProperty(59, p => p.CuotasCapVen);
            MapProperty(60, p => p.CuotasInt);
            MapProperty(61, p => p.CuotasIntVen);
            MapProperty(62, p => p.CuotasMora);
            MapProperty(63, p => p.DiasVenusgaap);
            MapProperty(64, p => p.DiasVennopag);
            MapProperty(65, p => p.ClaveAsigusgaap);
            MapProperty(66, p => p.Capital);
            MapProperty(67, p => p.Interes);
            MapProperty(68, p => p.Subtotal);
            MapProperty(69, p => p.Moratorios);
            MapProperty(70, p => p.Total);
            MapProperty(71, p => p.V2601Pt);
            MapProperty(72, p => p.TipoCambio);
            MapProperty(73, p => p.CapitalVal);
            MapProperty(74, p => p.InteresVal);
            MapProperty(75, p => p.SubtotalVal);
            MapProperty(76, p => p.MoratoriosVal);
            MapProperty(77, p => p.TotalVal);
            MapProperty(78, p => p.V2601PtVal);
            MapProperty(79, p => p.FechaEmision);
            MapProperty(80, p => p.Medida);
            MapProperty(81, p => p.TipoCalculo);
            MapProperty(82, p => p.NumCreditoFira);
            MapProperty(83, p => p.NumControlFira);
            MapProperty(84, p => p.ConcFinan);
            MapProperty(85, p => p.NombFinan);
            MapProperty(86, p => p.NomProgra);
            MapProperty(87, p => p.CveProgra);
            MapProperty(88, p => p.Almacenadora);

        }
    }
}
