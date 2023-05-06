using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.ABSaldos;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Saldos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Tokenizer.RFC4180;
using TinyCsvParser;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraCargaCorteDiarioService : IAdministraCargaCorteDiario
    {
        private readonly string _archivoABSaldosCompleta;
        private readonly string _archivoABSaldosCredit;
        private readonly string _archivoABSaldosCuenta;
        private readonly string _archivoABSaldosEspecial;
        private readonly string _archivoABSaldosGen;
        private readonly string _formatDateTime;
        private readonly string _formatDateTimeSdoGen;
        private readonly string _archivoSDPagoCapit;
        private readonly string _archivoSDPagInter;
        private readonly string _archivoSDDetMor;
        private readonly string _archivoSDMovDia;

        public AdministraCargaCorteDiarioService()
        {
            _archivoABSaldosCompleta = "";
            _archivoABSaldosCredit = "";
            _archivoABSaldosCuenta = "";
            _archivoABSaldosEspecial = "";
            _archivoABSaldosGen = "";
            _archivoSDPagoCapit = "";
            _archivoSDPagInter = "";
            _archivoSDDetMor = "";
            _archivoSDMovDia = "";
            _formatDateTime = "MM/dd/yyyy";
            _formatDateTimeSdoGen = "dd/MM/yy";
        }

        private DateTime? GetDateTimeFromString(string? fechaAConvertir)
        {
            if (string.IsNullOrEmpty(fechaAConvertir) || string.IsNullOrWhiteSpace(fechaAConvertir))
            {
                return null;
            }
            fechaAConvertir = fechaAConvertir.Trim();
            if (!DateTime.TryParseExact(fechaAConvertir, _formatDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado))
            {
                return null;
            }
            return resultado;
        }

        private DateTime? GetDateTimeSdoGenFromString(string? fechaAConvertir)
        {
            if (string.IsNullOrEmpty(fechaAConvertir) || string.IsNullOrWhiteSpace(fechaAConvertir))
            {
                return null;
            }
            fechaAConvertir = fechaAConvertir.Trim();
            if (!DateTime.TryParseExact(fechaAConvertir, _formatDateTimeSdoGen, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado))
            {
                return null;
            }
            return resultado;
        }
        public IEnumerable<ABSaldosCompleta> CargaABSaldosCompleta(string archivoABSaldosCompleta = "")
        {
            if (string.IsNullOrEmpty(archivoABSaldosCompleta))
                archivoABSaldosCompleta = _archivoABSaldosCompleta;
            IList<ABSaldosCompleta> resultado = new List<ABSaldosCompleta>();
            if (!File.Exists(archivoABSaldosCompleta))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoABSaldosCompleta);
            IEnumerable<ABSaldosCompletaCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<ABSaldosCompletaCarga>(options, new ABSaldosCompletaCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Where(z=>z is not null).Select(x => new ABSaldosCompleta()
            {
                Regional = x.Regional,
                NumCte = x.NumCte,
                Sucursal = x.Sucursal,
                ApellPaterno = (x.ApellPaterno ?? "").Replace("~", "\""),
                ApellMaterno = (x.ApellMaterno ?? "").Replace("~", "\""),
                Nombre1 = (x.Nombre1 ?? "").Replace("~", "\""),
                Nombre2 = (x.Nombre2 ?? "").Replace("~", "\""),
                Tnfinal = x.Tnfinal,
                RazonSocial = (x.RazonSocial ?? "").Replace("~", "\""),
                ActividadPrinc = x.ActividadPrinc,
                Sector = x.Sector ?? 0,
                TpoPersona = x.TpoPersona,
                EstadoInegi = x.EstadoInegi,
                MunicipioInegi = x.MunicipioInegi,
                LocalidadInegi = x.LocalidadInegi,
                NumCredito = x.NumCredito,
                NumProducto = x.NumProducto,
                Divisa = x.Divisa,
                CodLinea = x.CodLinea,
                PorcRecProp = x.PorcRecProp,
                TipoCartera = x.TipoCartera,
                FechaApertura = x.FechaApertura,
                FechaVencim = x.FechaVencim,
                TasaInteres = x.TasaInteres,
                TasaFijaOVar = x.TasaFijaOVar,
                CodTasaBase = x.CodTasaBase,
                Superficie = x.Superficie ?? 0,
                Actividad = x.Actividad,
                RelGarcred = x.RelGarcred,
                CodigoInsCre = x.CodigoInsCre,
                CodInversion = x.CodInversion,
                CodCaract = x.CodCaract,
                CodCaract2 = x.CodCaract2,
                CodAgricola = x.CodAgricola,
                CodProd = x.CodProd,
                StatusContable = x.StatusContable,
                PorcentajeEstim = x.PorcentajeEstim,
                ElegiblePf = x.ElegiblePf,
                PorcentajeFiFo = x.PorcentajeFiFo,
                BanderaFiFo = x.BanderaFiFo,
                MontoOtorgado = x.MontoOtorgado,
                MtoMinistraCap = x.MtoMinistraCap,
                SldoCapCont = x.SldoCapCont,
                SldoIntCont = x.SldoIntCont,
                SldoTotCont = x.SldoTotCont,
                SldoTotContval = x.SldoTotContval,
                NumContrato = x.NumContrato,
                FechaSuscripcion = GetDateTimeFromString(x.FechaSuscripcion),
                FechaVencCont = GetDateTimeFromString(x.FechaVencCont),
                MontoAutoCont = x.MontoAutoCont,
                MontoEjercido = x.MontoEjercido,
                TipoCreditoCont = x.TipoCreditoCont,
                EsUnionCredito = x.EsUnionCredito,
                DatosRegistro = x.DatosRegistro,
                CodCalifica = x.CodCalifica,
                NoCreditos = x.NoCreditos,
                NoClientes = x.NoClientes,
                CodigoIns = x.CodigoIns,
                PorcenRedesc = x.PorcenRedesc,
                FactorAplica = x.FactorAplica,
                TipoMargen = x.TipoMargen,
                MargenInter = x.MargenInter,
                LineaDesc = x.LineaDesc,
                TasaFon = x.TasaFon,
                NumControl = x.NumControl,
                Marcacred = x.Marcacred,
                CodSujetoFj = x.CodSujetoFj,
                CalEdosFin = x.CalEdosFin,
                CapVig = x.CapVig,
                IntFinVig = x.IntFinVig,
                IntFinVigNp = x.IntFinVigNp,
                CapVen = x.CapVen,
                IntFinVen = x.IntFinVen,
                IntFinVenNp = x.IntFinVenNp,
                IntNorVig = x.IntNorVig,
                IntNorVigNp = x.IntNorVigNp,
                IntNorVen = x.IntNorVen,
                IntNorVenNp = x.IntNorVenNp,
                IntDesVen = x.IntDesVen,
                IntPen = x.IntPen,
                DCapVig = x.DCapVig,
                DIntFinVig = x.DIntFinVig,
                DIntFinVigNp = x.DIntFinVigNp,
                DIntNorVig = x.DIntNorVig,
                DIntNorVigNp = x.DIntNorVigNp,
                TipoCredito = x.TipoCredito,
                TipoDescuento = x.TipoDescuento,
                FechaIngusgaap = GetDateTimeFromString(x.FechaIngusgaap),
                CuotasCap = x.CuotasCap,
                CuotasCapVen = x.CuotasCapVen,
                CuotasInt = x.CuotasInt,
                CuotasIntVen = x.CuotasIntVen,
                DiasVenusgaap = x.DiasVenusgaap,
                DiasVennopag = x.DiasVennopag,
                ClaveAsigusgaap = x.ClaveAsigusgaap,
                FolioAserca = x.FolioAserca,
                CredDemandado = x.CredDemandado,
                ImpPgsCredmnd = x.ImpPgsCredmnd ?? 0,
                NumCreditoFira = x.NumCreditoFira,
                NumControlFira = x.NumControlFira,

            }).ToList();
            #endregion
            return resultado;
        }

        private static string EliminaInconsistencias(string archivoABSaldosCompleta, bool otroEncoding = true)
        {
            Encoding cualVoyAUsar = Encoding.UTF8;
            if (otroEncoding)
                cualVoyAUsar = Encoding.GetEncoding("ISO-8859-1");
            string contenidoArchivo = File.ReadAllText(archivoABSaldosCompleta, cualVoyAUsar);
            contenidoArchivo = contenidoArchivo.Replace("\"", "~");
            string nuevoContenidoArchivo = contenidoArchivo.Replace("| |", "||");
            return nuevoContenidoArchivo;
        }

        public IEnumerable<ABSaldosCredit> CargaABSaldosCredit(string archivoABSaldosCredit = "")
        {
            if (string.IsNullOrEmpty(archivoABSaldosCredit))
                archivoABSaldosCredit = _archivoABSaldosCredit;
            IList<ABSaldosCredit> resultado = new List<ABSaldosCredit>();
            if (!File.Exists(archivoABSaldosCredit))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoABSaldosCredit);
            IEnumerable<ABSaldosCreditCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<ABSaldosCreditCarga>(options, new ABSaldosCreditCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new ABSaldosCredit()
            {
                Coord = x.Coord,
                Sucursal = x.Sucursal,
                Numprom = x.Numprom,
                Nomprom = (x.Nomprom ?? "").Replace("~", "\""),
                Numcte = x.Numcte,
                Nomcte = (x.Nomcte ?? "").Replace("~", "\""),
                CodProd = x.CodProd,
                CodCalifica = x.CodCalifica,
                Sector = x.Sector,
                TpoPersona = x.TpoPersona,
                EstadoInegi = x.EstadoInegi,
                NumContrato = x.NumContrato,
                NumCredito = x.NumCredito,
                TipoCredito = x.TipoCredito,
                NumProducto = x.NumProducto,
                Divisa = x.Divisa,
                StatusContable = x.StatusContable,
                TipoCartera = x.TipoCartera,
                FechaApertura = x.FechaApertura,
                FechaVencim = x.FechaVencim,
                Fecha1Aminis = x.Fecha1Aminis,
                TasaInteres = x.TasaInteres ?? 0,
                Factor = x.Factor,
                PuntosAdic = x.PuntosAdic ?? 0,
                TasaFijaOVar = x.TasaFijaOVar,
                CodTasaBase = x.CodTasaBase,
                CobroCapital = x.CobroCapital,
                CobroInteres = x.CobroInteres,
                Actividad = x.Actividad,
                RelGarcred = x.RelGarcred ?? 0,
                CodLinea = x.CodLinea,
                CodInversion = x.CodInversion,
                Superficie = x.Superficie ?? 0,
                CodCaract = x.CodCaract,
                CodCaract2 = x.CodCaract2,
                CodAgricola = x.CodAgricola,
                ApoyoFirco = x.ApoyoFirco,
                FechaApyfirco = GetDateTimeFromString(x.FechaApyfirco),
                EsUnionCredito = x.EsUnionCredito,
                CodSujetoFj = x.CodSujetoFj ?? 0,
                CalEdosFin = x.CalEdosFin,
                MontoAutorizado = x.MontoAutorizado ?? 0,
                MontoEjercido = x.MontoEjercido ?? 0,
                MontoDisponible = x.MontoDisponible ?? 0,
                CapVig = x.CapVig ?? 0,
                IntFinVig = x.IntFinVig ?? 0,
                IntFinVigNp = x.IntFinVigNp ?? 0,
                CapVen = x.CapVen ?? 0,
                IntFinVen = x.IntFinVen ?? 0,
                IntFinVenNp = x.IntFinVenNp ?? 0,
                IntNorVig = x.IntNorVig ?? 0,
                IntNorVigNp = x.IntNorVigNp ?? 0,
                IntCobAnt2601 = x.IntCobAnt2601 ?? 0,
                IntNorVen = x.IntNorVen ?? 0,
                IntNorVenNp = x.IntNorVenNp ?? 0,
                IntDesVen = x.IntDesVen ?? 0,
                IntPen = x.IntPen ?? 0,
                FechaIngusgaap = GetDateTimeFromString(x.FechaIngusgaap),
                CuotasCap = x.CuotasCap ?? 0,
                CuotasCapVen = x.CuotasCapVen ?? 0,
                CuotasInt = x.CuotasInt ?? 0,
                CuotasIntVen = x.CuotasIntVen ?? 0,
                CuotasMora = x.CuotasMora ?? 0,
                DiasVenusgaap = x.DiasVenusgaap ?? 0,
                DiasVennopag = x.DiasVennopag ?? 0,
                ClaveAsigusgaap = x.ClaveAsigusgaap,
                Capital = x.Capital ?? 0,
                Interes = x.Interes ?? 0,
                Subtotal = x.Subtotal ?? 0,
                Moratorios = x.Moratorios ?? 0,
                Total = x.Total ?? 0,
                V2601Pt = x.V2601Pt ?? 0,
                TipoCambio = x.TipoCambio ?? 0,
                CapitalVal = x.CapitalVal ?? 0,
                InteresVal = x.InteresVal ?? 0,
                SubtotalVal = x.SubtotalVal ?? 0,
                MoratoriosVal = x.MoratoriosVal ?? 0,
                TotalVal = x.TotalVal ?? 0,
                V2601PtVal = x.V2601PtVal ?? 0,
                FechaEmision = GetDateTimeFromString(x.FechaEmision),
                Medida = x.Medida,
                TipoCalculo = x.TipoCalculo,
                NumCreditoFira = x.NumCreditoFira,
                NumControlFira = x.NumControlFira,
                ConcFinan = x.ConcFinan,
                NombFinan = x.NombFinan,
                NomProgra = x.NomProgra,
                CveProgra = x.CveProgra,
                Almacenadora = x.Almacenadora
            }).ToList();
            #endregion
            return resultado;
        }

        public IEnumerable<ABSaldosCuenta> CargaABSaldosCuenta(string archivoABSaldosCuenta = "")
        {
            if (string.IsNullOrEmpty(archivoABSaldosCuenta))
                archivoABSaldosCuenta = _archivoABSaldosCuenta;
            IList<ABSaldosCuenta> resultado = new List<ABSaldosCuenta>();
            if (!File.Exists(archivoABSaldosCuenta))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoABSaldosCuenta);
            IEnumerable<ABSaldosCuentaCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<ABSaldosCuentaCarga>(options, new ABSaldosCuentaCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new ABSaldosCuenta()
            {
                Coordinacion = x.Coordinacion,
                Agencia = x.Agencia,
                Moneda = x.Moneda,
                NumCredito = x.NumCredito,
                NumProducto = x.NumProducto,
                StatusCred = x.StatusCred,
                StatusCont = x.StatusCont,
                CapVig = x.CapVig,
                CtaCapVig = x.CtaCapVig,
                CapVen = x.CapVen,
                CtaCapVen = x.CtaCapVen,
                IntFinVig = x.IntFinVig,
                CtaIntFinVig = x.CtaIntFinVig,
                IntFinVigNp = x.IntFinVigNp,
                CtaIntFinVigNp = x.CtaIntFinVigNp,
                IntFinVen = x.IntFinVen,
                CtaIntFinVen = x.CtaIntFinVen,
                IntFinVenNp = x.IntFinVenNp,
                CtaIntFinVenNp = x.CtaIntFinVenNp,
                IntNorVig = x.IntNorVig,
                CtaIntNorVig = x.CtaIntNorVig,
                IntNorVigNp = x.IntNorVigNp,
                CtaIntNorVigNp = x.CtaIntNorVigNp,
                IntNorVen = x.IntNorVen,
                CtaIntNorVen = x.CtaIntNorVen,
                IntNorVenNp = x.IntNorVenNp,
                CtaIntNorVenNp = x.CtaIntNorVenNp,
                IntDesVen = x.IntDesVen,
                CtaIntDesVen = x.CtaIntDesVen,
                IntPen = x.IntPen,
                CtaIntPen = x.CtaIntPen,
                Sector = x.Sector ?? 0
            }).ToList();
            #endregion

            return resultado;
        }

        public IEnumerable<ABSaldosEspecial> CargaABSaldosEspecial(string archivoABSaldosEspecial = "")
        {
            if (string.IsNullOrEmpty(archivoABSaldosEspecial))
                archivoABSaldosEspecial = _archivoABSaldosEspecial;
            IList<ABSaldosEspecial> resultado = new List<ABSaldosEspecial>();
            if (!File.Exists(archivoABSaldosEspecial))
                return resultado;

            string nuevoContenidoArchivo = EliminaInconsistencias(archivoABSaldosEspecial);
            IEnumerable<ABSaldosEspecialCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<ABSaldosEspecialCarga>(options, new ABSaldosEspecialCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new ABSaldosEspecial()  // 
            {
                Regional = x.Regional,
                NumCte = x.NumCte,
                Sucursal = x.Sucursal,
                ApellPaterno = (x.ApellPaterno ?? "").Replace("~", "\""),
                ApellMaterno = (x.ApellMaterno ?? "").Replace("~", "\""),
                Nombre1 = (x.Nombre1 ?? "").Replace("~", "\""),
                Nombre2 = (x.Nombre2 ?? "").Replace("~", "\""),
                Tnfinal = x.Tnfinal,
                RazonSocial = (x.RazonSocial ?? "").Replace("~", "\""),
                ActividadPrinc = x.ActividadPrinc,
                Sector = x.Sector ?? 0,
                TpoPersona = x.TpoPersona,
                EstadoInegi = x.EstadoInegi,
                MunicipioInegi = x.MunicipioInegi,
                LocalidadInegi = x.LocalidadInegi,
                NumCredito = x.NumCredito,
                NumProducto = x.NumProducto,
                Divisa = x.Divisa,
                CodLinea = x.CodLinea,
                PorcRecProp = x.PorcRecProp ?? 0,
                TipoCartera = x.TipoCartera,
                FechaApertura = x.FechaApertura,
                FechaVencim = x.FechaVencim,
                TasaInteres = x.TasaInteres ?? 0,
                TasaFijaOVar = x.TasaFijaOVar ?? 0,
                CodTasaBase = x.CodTasaBase,
                Superficie = x.Superficie ?? 0,
                Actividad = x.Actividad,
                RelGarcred = x.RelGarcred ?? 0,
                CodigoInsCre = x.CodigoInsCre,
                CodInversion = x.CodInversion,
                CodCaract = x.CodCaract,
                CodCaract2 = x.CodCaract2,
                CodAgricola = x.CodAgricola,
                CodProd = x.CodProd,
                StatusContable = x.StatusContable,
                PorcentajeEstim = x.PorcentajeEstim ?? 0,
                ElegiblePf = x.ElegiblePf,
                PorcentajeFiFo = x.PorcentajeFiFo ?? 0,
                BanderaFiFo = x.BanderaFiFo,
                MontoOtorgado = x.MontoOtorgado ?? 0,
                MtoMinistraCap = x.MtoMinistraCap ?? 0,
                SldoCapCont = x.SldoCapCont ?? 0,
                SldoIntCont = x.SldoIntCont ?? 0,
                SldoTotCont = x.SldoTotCont ?? 0,
                SldoTotContval = x.SldoTotContval ?? 0,
                NumContrato = x.NumContrato,
                FechaSuscripcion = GetDateTimeFromString(x.FechaSuscripcion),
                FechaVencCont = GetDateTimeFromString(x.FechaVencCont),
                MontoAutoCont = x.MontoAutoCont ?? 0,
                MontoEjercido = x.MontoEjercido ?? 0,
                TipoCreditoCont = x.TipoCreditoCont,
                EsUnionCredito = x.EsUnionCredito,
                DatosRegistro = x.DatosRegistro,
                CodCalifica = x.CodCalifica,
                Nocreditos = x.Nocreditos ?? 0,
                Noclientes = x.Noclientes ?? 0,
                CodigoIns = x.CodigoIns,
                PorcenRedesc = x.PorcenRedesc ?? 0,
                FactorAplica = x.FactorAplica,
                TipoMargen = x.TipoMargen,
                MargenInter = x.MargenInter ?? 0,
                LineaDesc = x.LineaDesc,
                TasaFon = x.TasaFon ?? 0,
                NumControl = x.NumControl,
                Marcacred = x.Marcacred ?? 0,
                CodSujetoFj = x.CodSujetoFj ?? 0,
                CalEdosFin = x.CalEdosFin,
                CapVig = x.CapVig ?? 0,
                IntFinVig = x.IntFinVig ?? 0,
                IntFinVigNp = x.IntFinVigNp ?? 0,
                CapVen = x.CapVen ?? 0,
                IntFinVen = x.IntFinVen ?? 0,
                IntFinVenNp = x.IntFinVenNp ?? 0,
                IntNorVig = x.IntNorVig ?? 0,
                IntNorVigNp = x.IntNorVigNp ?? 0,
                IntNorVen = x.IntNorVen ?? 0,
                IntNorVenNp = x.IntNorVenNp ?? 0,
                IntDesVen = x.IntDesVen ?? 0,
                IntPen = x.IntPen ?? 0,
                DCapVig = x.DCapVig ?? 0,
                DIntFinVig = x.DIntFinVig ?? 0,
                DIntFinVigNp = x.DIntFinVigNp ?? 0,
                DIntNorVig = x.DIntNorVig ?? 0,
                DIntNorVigNp = x.DIntNorVigNp ?? 0,
                TipoCredito = x.TipoCredito,
                TipoDescuento = x.TipoDescuento,
                FechaIngusgaap = GetDateTimeFromString(x.FechaIngusgaap),
                CuotasCap = x.CuotasCap ?? 0,
                CuotasCapVen = x.CuotasCapVen ?? 0,
                CuotasInt = x.CuotasInt ?? 0,
                CuotasIntVen = x.CuotasIntVen ?? 0,
                DiasVenusgaap = x.DiasVenusgaap ?? 0,
                DiasVennopag = x.DiasVennopag ?? 0,
                ClaveAsigusgaap = x.ClaveAsigusgaap,
                Ejecutivo = x.Ejecutivo,
                Unidad = x.Unidad,
                FechaMinis = GetDateTimeFromString(x.FechaMinis),
                CobroCapital = x.CobroCapital ?? 0,
                CobroInt = x.CobroInt ?? 0,
                FolioAserca = x.FolioAserca,
                CredDemandado = x.CredDemandado,
                ImpPgsCredmnd = x.ImpPgsCredmnd ?? 0
            }).ToList();
            #endregion


            return resultado;
        }

        public IEnumerable<ABSaldosGen> CargaABSaldosGen(string archivoABSaldosGen = "")
        {
            if (string.IsNullOrEmpty(archivoABSaldosGen))
                archivoABSaldosGen = _archivoABSaldosGen;
            IList<ABSaldosGen> resultado = new List<ABSaldosGen>();
            if (!File.Exists(archivoABSaldosGen))
                return resultado;


            string nuevoContenidoArchivo = File.ReadAllText(archivoABSaldosGen);
            IEnumerable<ABSaldosGenCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new Options('"', '\\', ',');
                var tokenizer = new RFC4180Tokenizer(options);
                CsvParserOptions csvParserOptions = new(true, tokenizer);
                var parser = new CsvParser<ABSaldosGenCarga>(csvParserOptions, new ABSaldosGenCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Where(y => y is not null).Select(x => new ABSaldosGen()
            {
                Regional = x.Regional,
                NumCte = x.NumCte,
                Sucursal = x.Sucursal,
                ApellPaterno = (x.ApellPaterno ?? "").Replace("~", "\""),
                ApellMaterno = (x.ApellMaterno ?? "").Replace("~", "\""),
                Nombre1 = (x.Nombre1 ?? "").Replace("~", "\""),
                Nombre2 = (x.Nombre2 ?? "").Replace("~", "\""),
                TnFinal = x.TnFinal,
                RazonSocial = (x.RazonSocial ?? "").Replace("~", "\""),
                ActividadPrinc = x.ActividadPrinc,
                Sector = x.Sector,
                TpoPersona = x.TpoPersona,
                EstadoInegi = x.EstadoInegi,
                MunicipioInegi = x.MunicipioInegi,
                LocalidadInegi = x.LocalidadInegi,
                NumCredito = x.NumCredito,
                NumProducto = x.NumProducto,
                Divisa = x.Divisa,
                CodLinea = x.CodLinea,
                PorcRecProp = x.PorcRecProp ?? 0,
                TipoCartera = x.TipoCartera,
                FechaApertura = GetDateTimeSdoGenFromString(x.FechaApertura),
                FechaVencim = GetDateTimeSdoGenFromString(x.FechaVencim),
                TasaInteres = x.TasaInteres ?? 0,
                TasaFijaOVar = x.TasaFijaOVar,
                CodTasaBase = x.CodTasaBase,
                Superficie = x.Superficie ?? 0,
                Actividad = x.Actividad,
                RelGarcred = x.RelGarcred ?? 0,
                CodigoInsCre = x.CodigoInsCre,
                CodInversion = x.CodInversion,
                CodCaract = x.CodCaract,
                CodCaract2 = x.CodCaract2,
                CodAgricola = x.CodAgricola,
                CodProd = x.CodProd,
                StatusContable = x.StatusContable,
                PorcentajeEstim = x.PorcentajeEstim ?? 0,
                ElegiblePf = x.ElegiblePf,
                PorcentajeFiFo = x.PorcentajeFiFo ?? 0,
                BanderaFiFo = x.BanderaFiFo,
                MontoOtorgado = x.MontoOtorgado ?? 0,
                MtoMinistraCap = x.MtoMinistraCap ?? 0,
                SldoCapCont = x.SldoCapCont ?? 0,
                SldoIntCont = x.SldoIntCont ?? 0,
                SldoTotCont = x.SldoTotCont ?? 0,
                SldoTotContval = x.SldoTotContval ?? 0,
                NumContrato = x.NumContrato,
                FechaSuscripcion = GetDateTimeSdoGenFromString(x.FechaSuscripcion),
                FechaVencCont = GetDateTimeSdoGenFromString(x.FechaVencCont),
                MontoAutoCont = x.MontoAutoCont ?? 0,
                MontoEjercido = x.MontoEjercido ?? 0,
                TipoCreditoCont = x.TipoCreditoCont,
                EsUnionCredito = x.EsUnionCredito,
                DatosRegistro = x.DatosRegistro,
                CodCalifica = x.CodCalifica,
                Nocreditos = x.Nocreditos ?? 0,
                Noclientes = x.Noclientes ?? 0,
                CodigoIns = x.CodigoIns,
                PorcenRedesc = x.PorcenRedesc ?? 0,
                FactorAplica = x.FactorAplica,
                TipoMargen = x.TipoMargen,
                MargenInter = x.MargenInter ?? 0,
                LineaDesc = x.LineaDesc,
                TasaFon = x.TasaFon ?? 0,
                NumControl = x.NumControl,
                Marcacred = x.Marcacred,
                CodSujetoFj = x.CodSujetoFj ?? 0,
                CalEdosFin = x.CalEdosFin,
                CapVig = x.CapVig ?? 0,
                IntFinVig = x.IntFinVig ?? 0,
                IntFinVigNp = x.IntFinVigNp ?? 0,
                CapVen = x.CapVen ?? 0,
                IntFinVen = x.IntFinVen ?? 0,
                IntFinVenNp = x.IntFinVenNp ?? 0,
                IntNorVig = x.IntNorVig ?? 0,
                IntNorVigNp = x.IntNorVigNp ?? 0,
                IntNorVen = x.IntNorVen ?? 0,
                IntNorVenNp = x.IntNorVenNp ?? 0,
                IntDesVen = x.IntDesVen ?? 0,
                IntPen = x.IntPen ?? 0,
                DCapVig = x.DCapVig ?? 0,
                DIntFinVig = x.DIntFinVig ?? 0,
                DIntFinVigNp = x.DIntFinVigNp ?? 0,
                DIntNorVig = x.DIntNorVig ?? 0,
                DIntNorVigNp = x.DIntNorVigNp ?? 0,
                TipoCredito = x.TipoCredito,
                TipoDescuento = x.TipoDescuento,
                FechaIngusgaap = GetDateTimeSdoGenFromString(x.FechaIngusgaap),
                CuotasCap = x.CuotasCap ?? 0,
                CuotasCapVen = x.CuotasCapVen ?? 0,
                CuotasInt = x.CuotasInt ?? 0,
                CuotasIntVen = x.CuotasIntVen ?? 0,
                DiasVenusgaap = x.DiasVenusgaap ?? 0,
                DiasVennopag = x.DiasVennopag ?? 0,
                ClaveAsigusgaap = x.ClaveAsigusgaap,
                Ejecutivo = x.Ejecutivo,
                Unidad = x.Unidad,
                FechaMinis = GetDateTimeSdoGenFromString(x.FechaMinis),
                CobroInt = x.CobroInt,
                CobroCapital = x.CobroCapital,
                FolioAserca = x.FolioAserca,
                CredDemandado = x.CredDemandado,
                ImpPgsCredmnd = x.ImpPgsCredmnd ?? 0,
                FactorSobretasa = x.FactorSobretasa,
                TipoCalculo = x.TipoCalculo,
                Sobretasa = x.Sobretasa ?? 0,
                Medida = x.Medida,
                Numprom = x.Numprom,
                Nomprom = x.Nomprom,
                CuotasMora = x.CuotasMora ?? 0,
                Capital = x.Capital ?? 0,
                Interes = x.Interes ?? 0,
                Subtotal = x.Subtotal ?? 0,
                Moratorios = x.Moratorios ?? 0,
                Total = x.Total ?? 0,
                V2601Pt = x.V2601Pt ?? 0,
                TipoCambio = x.TipoCambio ?? 0,
                CapitalVal = x.CapitalVal ?? 0,
                InteresVal = x.InteresVal ?? 0,
                SubtotalVal = x.SubtotalVal ?? 0,
                MoratoriosVal = x.MoratoriosVal ?? 0,
                TotalVal = x.TotalVal ?? 0,
                V2601PtVal = x.V2601PtVal ?? 0,
                MontoDisponible = x.MontoDisponible ?? 0,
                ApoyoFirco = x.ApoyoFirco,
                FechaApyfirco = GetDateTimeSdoGenFromString(x.FechaApyfirco),
                IntCobAnt2601 = x.IntCobAnt2601 ?? 0,
                FechaEmision = GetDateTimeSdoGenFromString(x.FechaEmision),
                NumCreditoFira = x.NumCreditoFira,
                NumControlFira = x.NumControlFira,
                Almacenadora = x.Almacenadora
            }).ToList();
            #endregion

            return resultado;
        }

        public IEnumerable<SDPagoCapit> CargaSDPagoCapit(string archivoSDPagoCapit = "")
        {
            if (string.IsNullOrEmpty(archivoSDPagoCapit))
                archivoSDPagoCapit = _archivoSDPagoCapit;
            IList<SDPagoCapit> resultado = new List<SDPagoCapit>();
            if (!File.Exists(archivoSDPagoCapit))
                return resultado;

            string nuevoContenidoArchivo = EliminaInconsistencias(archivoSDPagoCapit);
            IEnumerable<SDPagoCapitCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<SDPagoCapitCarga>(options, new SDPagoCapitCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new SDPagoCapit()  // 
            {
                NumCredito = x.NumCredito,
                FechaCuota = x.FechaCuota,
                CuotaRec = x.CuotaRec,
                NumCuota = x.NumCuota ?? 0,
                MontoCuota = x.MontoCuota ?? 0,
                SaldoCuota = x.SaldoCuota ?? 0,
                ImpCapitalizado = x.ImpCapitalizado ?? 0,
                FactorAjuste = x.FactorAjuste ?? 0,
                MontoRealPag = x.MontoRealPag ?? 0,
                FechaPago = GetDateTimeSdoGenFromString(x.FechaPago),
                FactorMoratorio = x.FactorMoratorio ?? 0,
                MontoMoratorio = x.MontoMoratorio ?? 0,
                FechaMoratorio = GetDateTimeSdoGenFromString(x.FechaMoratorio),
                DiasMoratorios = x.DiasMoratorios ?? 0,
                StatusMoratorio = x.StatusMoratorio,
                NumPagares = x.NumPagares ?? 0,
                PorcPago = x.PorcPago ?? 0,
                BanderaMinistra = x.BanderaMinistra,
                StatusCuota = x.StatusCuota
            }).ToList();
            #endregion


            return resultado;
        }

        public IEnumerable<SDPagInter> CargaSDPagInter(string archivoSDPagInter = "")
        {
            if (string.IsNullOrEmpty(archivoSDPagInter))
                archivoSDPagInter = _archivoSDPagInter;
            IList<SDPagInter> resultado = new List<SDPagInter>();
            if (!File.Exists(archivoSDPagInter))
                return resultado;

            string nuevoContenidoArchivo = EliminaInconsistencias(archivoSDPagInter);
            IEnumerable<SDPagInterCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<SDPagInterCarga>(options, new SDPagInterCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new SDPagInter()  // 
            {
                NumCredito = x.NumCredito,
                FechaCuota = GetDateTimeFromString(x.FechaCuota),
                CuotaRec = x.CuotaRec,
                NumCuota = x.NumCuota,
                MontoCuota = x.MontoCuota,
                MontoRealPag = x.MontoRealPag,
                FechaPag = GetDateTimeFromString(x.FechaPag),
                FactorMoratorio = x.FactorMoratorio,
                MontoMoratorio = x.MontoMoratorio,
                FechaMoratorio = GetDateTimeFromString(x.FechaMoratorio),
                DiasMoratorio = x.DiasMoratorio,
                StatusMoratorio = x.StatusMoratorio,
                BonifiIntMora = x.BonifiIntMora,
                PorcPago = x.PorcPago,
                StatusCuota = x.StatusCuota,
                MontoFinanciado = x.MontoFinanciado

            }).ToList();
            #endregion


            return resultado;
        }

        public IEnumerable<SDDetMora> CargaSDDetMor(string archivoSDDetMor = "")
        {
            if (string.IsNullOrEmpty(archivoSDDetMor))
                archivoSDDetMor = _archivoSDDetMor;
            IList<SDDetMora> resultado = new List<SDDetMora>();
            if (!File.Exists(archivoSDDetMor))
                return resultado;

            string nuevoContenidoArchivo = EliminaInconsistencias(archivoSDDetMor);
            IEnumerable<SDDetMoraCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<SDDetMoraCarga>(options, new SDDetMoraCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new SDDetMora()  // 
            {
                NumCredito = x.NumCredito,
                IdentifiRec = x.IdentifiRec,
                NumCuota = x.NumCuota,
                SdoAcumMesMora = x.SdoAcumMesMora,
                TasaOrdinaria = x.TasaOrdinaria,
                ProviMoraOrdi = x.ProviMoraOrdi,
                TasaCopete = x.TasaCopete,
                PriviMoraCope = x.PriviMoraCope,
                SdoMoraOrdi = x.SdoMoraOrdi,
                SdoMoraCope = x.SdoMoraCope
            }).ToList();
            #endregion


            return resultado;
        }

        public IEnumerable<SDMovDia> CargaSDMovDia(string archivoSDMovDia = "")
        {
            if (string.IsNullOrEmpty(archivoSDMovDia))
                archivoSDMovDia = _archivoSDMovDia;
            IList<SDMovDia> resultado = new List<SDMovDia>();
            if (!File.Exists(archivoSDMovDia))
                return resultado;

            string nuevoContenidoArchivo = EliminaInconsistencias(archivoSDMovDia);
            IEnumerable<SDMovDiaCarga> abSaldosCompleta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(false, '|');
                var parser = new CsvParser<SDMovDiaCarga>(options, new SDMovDiaCsvMapping());
                abSaldosCompleta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = abSaldosCompleta.Select(x => new SDMovDia()  // 
            {
                Secuencia = x.Secuencia,
                FechaMov = GetDateTimeFromString(x.FechaMov),
                HoraMov = x.HoraMov,
                Sucursal = x.Sucursal,
                NumCredito = x.NumCredito,
                Plaza = x.Plaza,
                TransaccSuc = x.TransaccSuc,
                Usuario = x.Usuario,
                Monto = x.Monto,
                CodigoFun = x.CodigoFun,
                CodigoRef = x.CodigoRef,
                Divisa = x.Divisa,
                Reversado = x.Reversado,
                FolioSuc = x.FolioSuc,
                NumProducto = x.NumProducto
            }).ToList();
            #endregion


            return resultado;
        }
    }
}
