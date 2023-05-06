using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.CruceInformacion
{
    public class CruceInformacion : ICruceInformacion
    {
        private readonly ILogger<CruceInformacion> _logger;
        private readonly IConfiguration _config;
        private readonly DateTime _periodoDelDoctor;
        public CruceInformacion(ILogger<CruceInformacion> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            string periodoDelDoctor = _config.GetValue<string>("periodoDelDoctor") ?? "01/07/2020";
            _periodoDelDoctor = DateTime.ParseExact(periodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture); //??new DateTime(2020,7,1)
        }

        public void CruzaInformacionAbSaldos(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<ABSaldosActivos> saldosNaturales)
        {
            var resultados = from salAct in saldosNaturales
                             join salCast in saldosCorporativo on QuitaCastigo(salAct.NumCredito) equals QuitaCastigo(salCast.NumCredito)

                             select new { salAct, salCast };
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = saldosNaturales.Count();

            foreach ( var item in resultados ) {
                noSaldosNaturales++;
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando ambos saldos {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.salAct.NumCredito);
                }
                #region Saldos Activos = Saldos Con Castigo
                item.salAct.Castigo = item.salCast.Castigo;
                item.salAct.EsCarteraActiva = true;
                item.salAct.CatRegion = item.salCast.CatRegion??string.Empty;
                item.salAct.CatSucursal = item.salCast.CatSucursal??string.Empty;
                #endregion
                #region Saldos con Castigo = Saldos Activo
                item.salCast.EsCarteraActiva = true;
                if (!(string.IsNullOrEmpty(item.salAct.Acreditado) || string.IsNullOrWhiteSpace(item.salAct.Acreditado)))
                    item.salCast.Acreditado = item.salAct.Acreditado;
                item.salCast.NumProducto = item.salAct.NumProducto;
                #endregion
            }
        }


        public void CruzaInformacionAbSaldosCorporativoImagenes(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<ArchivosImagenes> imagenes)
        {
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = saldosCorporativo.Count();
            
            var eliminaImagenesVacias = imagenes.Where(x => QuitaCastigo(x.NumCredito).Length > 17).Select(y => y).ToList();

            var resultados = from saldos in saldosCorporativo
                             join imgs in eliminaImagenesVacias on QuitaCastigo(saldos.NumCredito)[..14] equals QuitaCastigo(imgs.NumCredito)[..14]

                             select new { saldos, imgs };
            int noImagenes = 0;
            string numCredito = "000000000000000000";
            foreach (var item in resultados)
            {
                if (!numCredito[..14].Equals(QuitaCastigo(item.saldos.NumCredito)[..14]))
                {
                    numCredito = QuitaCastigo(item.saldos.NumCredito);
                    noImagenes = 0;
                }
                noImagenes++;
                item.saldos.ExpedienteDigital = true;
                item.saldos.NoPDF = noImagenes;
                noSaldosNaturales++;
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.saldos.NumCredito);
                }
                item.imgs.NumCte = item.saldos.NumeroCliente;
                if (string.IsNullOrEmpty(item.imgs.Acreditado) || string.IsNullOrWhiteSpace(item.imgs.Acreditado))
                {
                    if (!(string.IsNullOrEmpty(item.saldos.Acreditado) || string.IsNullOrWhiteSpace(item.saldos.Acreditado)))
                        item.imgs.Acreditado = item.saldos.Acreditado;
                }
                item.imgs.FechaApertura = item.saldos.FechaApertura;
                item.imgs.EsOrigenDelDoctor = item.saldos.EsOrigenDelDoctor;
                item.imgs.Castigo = item.saldos.Castigo;
                item.imgs.NumeroCreditoActivo = item.saldos.NumCredito; // Lo pongo con el castigo
            }

            // Calculo las imagenes por saldo
            var cantidadImagenes = (from img in eliminaImagenesVacias
                                    group img by img.NumCredito into cnt
                                    select new
                                    {
                                        cnt.First().NumCredito,
                                        Cantidad = cnt.Count()
                                    }).ToList();
            var rsltImgs = from saldos in saldosCorporativo
                           join imgs in cantidadImagenes on QuitaCastigo(saldos.NumCredito)[..14] equals QuitaCastigo(imgs.NumCredito)[..14]

                           select new { saldos, imgs };
            noImagenes = 0;
            numCredito = "000000000000000000";
            foreach (var item in rsltImgs)
            {
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.saldos.NumCredito);
                }
                item.saldos.NoPDF = item.imgs.Cantidad;
            }
        }

        public static string QuitaCastigo(string? origen) 
        {
            if (origen is null)
                return "000000000000000000";

            if (origen.Length < 18)
                return "000000000000000000";
            string digito = origen.Substring(3, 1);
            string digito2 = origen.Substring(4, 1);
            string nuevoOrigen = origen;
            if ((digito == "1" || digito == "9")&&(digito2!="0"))
            {
                nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
                // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
            }
            return nuevoOrigen;
        }
        public void CruzaInformacionAbSaldosImagenes(ref IEnumerable<ABSaldosActivos> saldosNaturales, ref IEnumerable<ArchivosImagenes> imagenes)
        {
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = saldosNaturales.Count();

            var eliminaImagenesVacias = imagenes.Where(x => QuitaCastigo(x.NumCredito).Length > 17).Select(y => y).ToList();

            var resultados = from saldos in saldosNaturales
                             join imgs in eliminaImagenesVacias on QuitaCastigo(saldos.NumCredito )[..14] equals QuitaCastigo(imgs.NumCredito)[..14]

                             select new { saldos, imgs };
            int noImagenes = 0;
            string numCredito = "000000000000000000";
            foreach (var item in resultados)
            {
                if (!numCredito[..14].Equals(QuitaCastigo(item.saldos.NumCredito)[..14]))
                {
                    numCredito = QuitaCastigo(item.saldos.NumCredito);
                    noImagenes = 0;
                }
                noImagenes++;
                item.saldos.ExpedienteDigital = true;
                item.saldos.NoPDF = noImagenes;
                noSaldosNaturales++;
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.saldos.NumCredito);
                }
                item.imgs.NumCte = item.saldos.NumCte;
                if (string.IsNullOrEmpty(item.imgs.Acreditado) || string.IsNullOrWhiteSpace(item.imgs.Acreditado))
                {
                    if (!(string.IsNullOrEmpty(item.saldos.Acreditado) || string.IsNullOrWhiteSpace(item.saldos.Acreditado)))
                        item.imgs.Acreditado = item.saldos.Acreditado;
                }
                item.imgs.NumContrato = item.saldos.NumContrato;
                item.imgs.FechaApertura = item.saldos.FechaApertura;
                item.imgs.EsOrigenDelDoctor = item.saldos.EsOrigenDelDoctor;
                item.imgs.Castigo = item.saldos.Castigo;
                item.imgs.EsCarteraActiva = true;
                item.imgs.NumeroCreditoActivo = item.saldos.NumCredito; // Con castigo
            }

            // Calculo las imagenes por saldo
            var cantidadImagenes = (from img in eliminaImagenesVacias
                                    group img by img.NumCredito into cnt
                                    select new
                                    {
                                        cnt.First().NumCredito,
                                        Cantidad = cnt.Count()
                                    }).ToList();
            var rsltImgs = from saldos in saldosNaturales
                           join imgs in cantidadImagenes on QuitaCastigo(saldos.NumCredito)[..14] equals QuitaCastigo(imgs.NumCredito)[..14]

                           select new { saldos, imgs };
            noImagenes = 0;
            numCredito = "000000000000000000";
            foreach (var item in rsltImgs)
            {
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.saldos.NumCredito);
                }
                item.saldos.NoPDF = item.imgs.Cantidad;
            }
        }

        public void CruzaInformacionAbSaldosCorporativoGV(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<InformacionGuardaValor> detalleGuardaValores)
        {
            var resultados = from gv in detalleGuardaValores
                             join salCor in saldosCorporativo on QuitaCastigo(gv.NumeroCredito) equals QuitaCastigo(salCor.NumCredito)

                             select new { gv, salCor };
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = detalleGuardaValores.Count();

            foreach (var item in resultados)
            {
                noSaldosNaturales++;
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando GV con saldos Corporativos {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.salCor.NumCredito);
                }
                if (!(string.IsNullOrEmpty(item.salCor.Acreditado) || string.IsNullOrWhiteSpace(item.salCor.Acreditado)))
                    item.gv.Acreditado = item.salCor.Acreditado;
                item.gv.NumProducto = item.salCor.NumProducto;
                item.gv.CatTipoCredito = item.salCor.CatProducto;
                item.gv.Importe = item.salCor.MontoCredito??0 + item.salCor.InteresCont??0;
                item.gv.FechaDeVencimiento = item.salCor.FechaVencimiento;
                if (!string.IsNullOrEmpty(item.gv.TipoDeHallazgo))
                {
                    if (!string.IsNullOrEmpty(item.gv.CorrectoIncorrecto) && "INCORRECTO".Contains(item.gv.CorrectoIncorrecto, StringComparison.InvariantCultureIgnoreCase))
                    {
                        item.gv.MontoDelHallazgo = item.gv.Importe;
                    }
                }

                // Fixes
                item.gv.NumCte = item.salCor.NumeroCliente;
                // Saco de AB Saldos
                item.gv.TieneExpedienteDigital = item.salCor.ExpedienteDigital;
                item.gv.Castigo = item.salCor.Castigo;
                item.gv.AperturaTiempoDoctor = item.salCor.EsOrigenDelDoctor;
                item.gv.AunEnABSaldos = true;
                // Agrego la bandera en Saldos Corporativos
                item.salCor.CuentaConGuardaValores = true;
            }

        }

        public void CruzaInformacionAbSaldosNaturalGV(ref IEnumerable<ABSaldosActivos> saldosNaturales, ref IEnumerable<InformacionGuardaValor> detalleGuardaValores)
        {
            var resultados = from gv in detalleGuardaValores
                             join salNat in saldosNaturales on QuitaCastigo(gv.NumeroCredito) equals QuitaCastigo(salNat.NumCredito)
                             // (gv.NumeroCredito ?? "") equals (salNat.NumCredito ?? "")
                             // where gv.AunEnABSaldos == false
                             select new { gv, salNat };
                
                /*
                from gv in detalleGuardaValores
                             join salNat in saldosNaturales on ComparaCreditos(gv.NumeroCredito, salNat.NumCredito)
                             // (gv.NumeroCredito ?? "") equals (salNat.NumCredito ?? "")
                             // where gv.AunEnABSaldos == false
                             select new { gv, salNat };
                */
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = detalleGuardaValores.Count();

            foreach (var item in resultados)
            {
                noSaldosNaturales++;
                if (noSaldosNaturales % 670000 == 0)
                {
                    _logger.LogInformation("Se estan comparando GV con saldos Corporativos {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.salNat.NumCredito);
                }
                if (!item.gv.AunEnABSaldos)
                {
                    if (!(string.IsNullOrEmpty(item.salNat.Acreditado) || string.IsNullOrWhiteSpace(item.salNat.Acreditado)))
                        item.gv.Acreditado = item.salNat.Acreditado;
                    item.gv.NumProducto = item.salNat.NumProducto;
                    // item.gv.CatTipoCredito = item.salCor.CatalogoProducto;
                    item.gv.Importe = item.salNat.SldoTotContVal;
                    item.gv.FechaDeVencimiento = item.salNat.FechaVencim;
                    if (!string.IsNullOrEmpty(item.gv.TipoDeHallazgo))
                    {
                        if (!string.IsNullOrEmpty(item.gv.CorrectoIncorrecto) && "INCORRECTO".Contains(item.gv.CorrectoIncorrecto, StringComparison.InvariantCultureIgnoreCase))
                        {
                            item.gv.MontoDelHallazgo = item.gv.Importe;
                        }
                    }

                    // Fixes
                    item.gv.NumCte = item.salNat.NumCte;
                    // Saco de AB Saldos
                    item.gv.TieneExpedienteDigital = item.salNat.ExpedienteDigital;
                    item.gv.Castigo = item.salNat.Castigo;
                    item.gv.AperturaTiempoDoctor = item.salNat.EsOrigenDelDoctor;
                    // item.gv.AunEnABSaldos = true;
                    item.gv.EsNuevo = true;
                    // Agrego la bandera en Saldos Corporativos
                }
                item.salNat.CuentaConGuardaValores = true;
                item.gv.Region = item.salNat.Regional;
                item.gv.Sucursal = item.salNat.Sucursal;

            }
        }

        public void IncluyeRegionYAgenciaGV(ref IEnumerable<InformacionGuardaValor> detalleGuardaValores)
        {
            var primerFiltro = (from dgv in detalleGuardaValores
                               where dgv.Region != 0
                               select new { dgv.ArchivoOrigenInformacion, dgv.Region, dgv.Sucursal })
                               .Distinct();                

            var resultados = from gv in detalleGuardaValores
                             join pf in primerFiltro on (gv.ArchivoOrigenInformacion ?? "") equals (pf.ArchivoOrigenInformacion ?? "")
                             where gv.Region == 0
                             select new { gv, pf };

            foreach (var item in resultados)
            {
                item.gv.Region = item.pf.Region;
                item.gv.Sucursal = item.pf.Sucursal;
            }
        }

        public void CruzaInformacionGvImagenes(ref IEnumerable<InformacionGuardaValor> detalleGuardaValores, ref IEnumerable<ArchivosImagenes> imagenes)
        {
            long noSaldosNaturales = 0;
            long totalSaldosNaturales = detalleGuardaValores.Count();
            try
            {
                var eliminaImagenesVacias = imagenes.Where(x => QuitaCastigo(x.NumCredito).Length > 17).Select(y => y).ToList();

                var eliminaCreditosVacios = detalleGuardaValores.Where(x => QuitaCastigo(x.NumeroCredito).Length > 17).Select(y => y).ToList();

                var resultados = from gv in eliminaCreditosVacios
                                 join imgs in eliminaImagenesVacias on QuitaCastigo(gv.NumeroCredito)[..14] equals QuitaCastigo(imgs.NumCredito)[..14]

                                 select new { gv, imgs };
                int noImagenes = 0;
                string numCredito = "000000000000000000";
                foreach (var item in resultados)
                {
                    if (!(numCredito[..14]).Equals(QuitaCastigo(item.gv.NumeroCredito)[..14]))
                    {
                        numCredito = QuitaCastigo(item.gv.NumeroCredito);
                        noImagenes = 0;
                    }
                    noImagenes++;
                    item.gv.TieneExpedienteDigital = true;
                    // item.saldos.NoPDF = noImagenes;
                    noSaldosNaturales++;
                    if (noSaldosNaturales % 670000 == 0)
                    {
                        _logger.LogInformation("Se estan comparando GuardaValores {noSaldo}/{totalSaldos} y vamos en el {numcredito}", noSaldosNaturales, totalSaldosNaturales, item.gv.NumeroCredito);
                    }
                    if (string.IsNullOrEmpty(item.imgs.NumCte))
                        item.imgs.NumCte = item.gv.NumCte;
                    if (string.IsNullOrEmpty(item.imgs.Acreditado) || string.IsNullOrWhiteSpace(item.imgs.Acreditado))
                        item.imgs.Acreditado = item.gv.Acreditado;

                    item.imgs.EsOrigenDelDoctor = item.gv.AperturaTiempoDoctor;
                    if (string.IsNullOrEmpty(item.imgs.NumeroCreditoActivo))
                        item.imgs.NumeroCreditoActivo = item.gv.NumeroCredito;
                }
            }
            catch { }
        }

        public void CruzaInformacionSaldosRegionalesSaldosNaturales(ref IEnumerable<ABSaldosRegionales> saldosRegionales, ref IEnumerable<ABSaldosActivos> saldosNaturales)
        {
            var resultados = from sn in saldosNaturales
                             join sr in saldosRegionales on QuitaCastigo(sn.NumCredito) equals QuitaCastigo(sr.NumCredito)
                             select new { sn, sr };

            foreach (var item in resultados)
            {
                item.sn.NumProducto = item.sr.NumProducto;
                //item.sn.CatProducto = item.sr.CatProducto;
                item.sn.TipoCartera = item.sr.TipoCartera;
                item.sn.FechaInicioMinistracion = item.sr.FechaMinistra;
                item.sn.MontoMinistraCap = item.sr.MontoMinistracion;
                item.sn.NombreEjecutivo = item.sr.NombreEjecutivo;
            }
        }

        public void CruzaInformacionSaldosRegionalesSaldosConCastigo(ref IEnumerable<ABSaldosRegionales> saldosRegionales, ref IEnumerable<ABSaldosConCastigo> saldosConCastigo)
        {
            var resultados = from sn in saldosConCastigo
                             join sr in saldosRegionales on QuitaCastigo(sn.NumCredito) equals QuitaCastigo(sr.NumCredito)
                             select new { sn, sr };

            foreach (var item in resultados)
            {
                item.sn.NumProducto = item.sr.NumProducto;
                //item.sn.CatProducto = item.sr.CatProducto;
                item.sn.TipoCartera = item.sr.TipoCartera;
                item.sn.FechaMinistra = item.sr.FechaMinistra;
                item.sn.MontoMinistraCap = item.sr.MontoMinistracion;
                item.sn.NombreEjecutivo = item.sr.NombreEjecutivo;
            }
        }

        public void CruzaInformacionSaldosActivosCreditosCancelados(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<CreditosCancelados> creditosCancelados)
        {
            // Tienen que ser iguales porque hay varias ministraciones, solo se cancela una
            var resultados = from sa in saldosActivos
                             join cc in creditosCancelados on QuitaCastigo(sa.NumCredito) equals QuitaCastigo(cc.NumCreditoActual)
                             select new { sa, cc };

            foreach (var item in resultados)
            {
                #region Creditos Activos vs Creditos Cancelados
                item.sa.NumCreditoCancelado = item.cc.NumCreditoNvo;
                item.sa.NumCreditoOriginal = item.cc.NumCreditoOrigen;
                item.sa.SesionDeAutorizacion = item.cc.SesionAutorizacion;
                item.sa.MesCastigo = item.cc.MesCastigo;
                item.sa.AnioCastigo = item.cc.AnioCastigo;
                item.sa.Generacion = item.cc.Generacion;
                item.sa.FechaCancelacion = item.cc.FechaCancelacion;
                item.sa.AnioOriginacion = item.cc.AnioOrignacion;
                item.sa.EstaEnCreditosCancelados = true;

                item.sa.EsCancelacionDelDoctor = (item.sa.FechaCancelacion >= _periodoDelDoctor);
                item.sa.EsOrigenDelDoctor = (item.cc.PrimeraDispersion >= _periodoDelDoctor);
                #endregion

                item.cc.EstaEnCreditosActivos = true;
            }
        }

        public void CruzaInformacionSaldosConCastigoCreditosCancelados(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<CreditosCancelados> creditosCancelados)
        {
            // Tienen que ser iguales porque hay varias ministraciones, solo se cancela una
            var resultados = from scc in saldosConCastigo
                             join cc in creditosCancelados on QuitaCastigo(scc.NumCredito) equals QuitaCastigo(cc.NumCreditoActual)
                             select new { scc, cc };

            foreach (var item in resultados)
            {
                //item.scc.NumCreditoCancelado = item.cc.NumCreditoNvo;
                item.scc.NumCreditoOriginal = item.cc.NumCreditoOrigen;
                item.scc.SesionDeAutorizacion = item.cc.SesionAutorizacion;
                item.scc.MesCastigo = item.cc.MesCastigo;
                item.scc.AnioCastigo = item.cc.AnioCastigo;
                item.scc.Generacion = item.cc.Generacion;
                item.scc.FechaCancelacion = item.cc.FechaCancelacion;
                item.scc.AnioOriginacion = item.cc.AnioOrignacion;
                item.scc.EstaEnCreditosCancelados = true;

                item.cc.EstaEnCreditosCastigados = true;
                item.cc.Castigo = item.scc.Castigo;

                item.scc.EsCancelacionDelDoctor = (item.scc.FechaCancelacion >= _periodoDelDoctor);
                item.scc.EsOrigenDelDoctor = (item.cc.PrimeraDispersion >= _periodoDelDoctor);

            }
        }

        public void CruzaInformacionSaldosConCastigoConPagados(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<MinistracionesMesa> pagados)
        {
            var resultados = from scc in saldosConCastigo
                             join pag in pagados on QuitaCastigo(scc.NumCredito)[..14] equals QuitaCastigo(pag.NumCredito)[..14]
                             orderby pag.NumCredito descending
                             select new { scc, pag };

            foreach (var item in resultados)
            {
                item.scc.NumCreditoOriginal = item.pag.NumCredito;
                item.scc.NumeroMinistracion = item.pag.NumMinistracion;
                // Solo para cuando no exista fecha de ministración
                if (!item.scc.FechaMinistra.HasValue)
                {
                    /// Por si es otra ministración, no lo agrego
                    if (QuitaCastigo(item.scc.NumCredito).Equals(item.pag.NumCredito))
                        item.scc.FechaMinistra = item.pag.FechaInicio;
                }
                item.scc.FechaInicioMinistracion = item.pag.FechaInicio; //  Fecha de apertura del credito
                item.scc.ClasificacionMesa = item.pag.Descripcion;
                item.scc.FechaDeSolicitud = item.pag.FechaAsignacion; //  Fecha de Asignacion de la solicitud
                item.scc.MontoOtorgado = item.pag.MontoOtorgado;
                item.scc.EstaEnMesa = true;
                item.scc.EsOrigenDelDoctor = (item.pag.FechaInicio >= _periodoDelDoctor);

                item.pag.EstaEnSaldosCastigos = true;
                item.pag.Castigo = item.scc.Castigo;
                item.pag.NumCreditoActual = item.scc.NumCredito;

            }
        }

        public void CruzaInformacionSaldosActivosConPagados(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<MinistracionesMesa> pagados)
        {
            var resultados = from sa in saldosActivos
                             join pag in pagados on QuitaCastigo(sa.NumCredito)[..14] equals QuitaCastigo(pag.NumCredito)[..14]
                             orderby pag.NumCredito descending
                             select new { sa, pag };

            foreach (var item in resultados)
            {
                item.sa.NumCreditoOriginal = item.pag.NumCredito;
                item.sa.NumeroMinistracion = item.pag.NumMinistracion;
                // Solo para cuando no exista fecha de ministración
                item.sa.FechaInicioMinistracion = item.pag.FechaInicio; //  Fecha de apertura del credito
                item.sa.ClasificacionMesa = item.pag.Descripcion;
                item.sa.FechaDeSolicitud = item.pag.FechaAsignacion; //  Fecha de Asignacion de la solicitud
                item.sa.MontoOtorgado = item.pag.MontoOtorgado;
                item.sa.EstaEnMesa = true;

                item.sa.EsOrigenDelDoctor = (item.pag.FechaInicio >= _periodoDelDoctor);

                item.pag.EstaEnSaldosActivos = true;
                item.pag.NumCreditoActual = item.sa.NumCredito;
            }
        }

        public void CruzaInformacionMesaConImagenes(ref IEnumerable<MinistracionesMesa> pagados, ref IEnumerable<ArchivosImagenes> archivosImagenes)
        {
            // Deberían coincidir los creditos [..14]
            #region Creditos directos
            var resultados = (from img in archivosImagenes
                             join pag in pagados on QuitaCastigo(img.NumCredito) equals QuitaCastigo(pag.NumCredito)
                             orderby pag.NumCredito descending
                             select new { img, pag }).ToList();

            foreach (var item in resultados)
            {
                if (QuitaCastigo(item.img.NumCredito).Equals("116600009720000001"))
                    _logger.LogInformation("Comienza debug acreditado!");
                if (string.IsNullOrEmpty(item.img.NumeroCreditoActivo))
                    item.img.NumeroCreditoActivo = item.pag.NumCredito;
                
                item.img.NumeroMinistracion = item.pag.NumMinistracion;
                item.img.FechaInicio = item.pag.FechaInicio; //  Fecha de apertura del credito
                item.img.ClasifMesa = item.pag.Descripcion;
                item.img.FechaAsignacionEntrada = item.pag.FechaAsignacion; //  Fecha de Asignacion de la solicitud
                item.img.MontoOtorgado = item.pag.MontoOtorgado;
                if (string.IsNullOrEmpty(item.img.Analista) || string.IsNullOrWhiteSpace(item.img.Analista))
                    if (!(string.IsNullOrEmpty(item.pag.Acreditado) || string.IsNullOrWhiteSpace(item.pag.Acreditado)))
                        item.img.Acreditado = item.pag.Acreditado;
                item.img.Analista = item.pag.Analista;
                item.img.CatAgencia = item.pag.CatAgencia;
                item.img.Regional = item.pag.Regional;
                item.img.Sucursal = item.pag.Sucursal;
                item.pag.TieneImagen = true;
            }
            #endregion
            #region Indirecto en imagenes
            var imagenesSinAcreditado = (from img in archivosImagenes
                                        where string.IsNullOrEmpty(img.Acreditado) || string.IsNullOrWhiteSpace(img.Acreditado)
                                        select img).ToList();

            resultados = (from img in imagenesSinAcreditado
                         join pag in pagados on QuitaCastigo(img.NumCredito)[..14] equals QuitaCastigo(pag.NumCredito)[..14]
                         orderby pag.NumCredito descending
                         select new { img, pag }).ToList();

            foreach (var item in resultados)
            {
                if (QuitaCastigo(item.img.NumCredito).Equals("116600009720000001"))
                    _logger.LogInformation("Comienza debug acreditado!");
                if (string.IsNullOrEmpty(item.img.Analista) || string.IsNullOrWhiteSpace(item.img.Analista))
                {
                    if (string.IsNullOrEmpty(item.img.NumeroCreditoActivo))
                        item.img.NumeroCreditoActivo = item.pag.NumCredito;

                    item.img.NumeroMinistracion = item.pag.NumMinistracion;
                    item.img.FechaInicio = item.pag.FechaInicio; //  Fecha de apertura del credito
                    item.img.ClasifMesa = item.pag.Descripcion;
                    item.img.FechaAsignacionEntrada = item.pag.FechaAsignacion; //  Fecha de Asignacion de la solicitud
                    item.img.MontoOtorgado = item.pag.MontoOtorgado;
                    if (!(string.IsNullOrEmpty(item.pag.Acreditado) || string.IsNullOrWhiteSpace(item.pag.Acreditado)))
                        item.img.Acreditado = item.pag.Acreditado;
                    item.img.Analista = item.pag.Analista;
                    item.img.CatAgencia = item.pag.CatAgencia;
                    item.img.Regional = item.pag.Regional;
                    item.img.Sucursal = item.pag.Sucursal;
                    item.pag.TieneImagenIndirecta = true;
                }
            }
            #endregion

            #region Indirecto en imagenes
            var pagadosSinImagen = (from pag in pagados
                                    where string.IsNullOrEmpty(pag.Acreditado) || string.IsNullOrWhiteSpace(pag.Acreditado)
                                         select pag).ToList();

            resultados = (from img in imagenesSinAcreditado
                         join pag in pagadosSinImagen on QuitaCastigo(img.NumCredito)[..14] equals QuitaCastigo(pag.NumCredito)[..14]
                         orderby pag.NumCredito descending
                         select new { img, pag }).ToList();

            foreach (var item in resultados)
            {
                item.pag.TieneImagenIndirecta = true;
            }
            #endregion
        }

        public void CruzaInformacionTipoProductoSaldosActivos(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<CatalogoProductos> productos)
        {
            var resultados = (from saldos in saldosActivos
                              join prods in productos on (saldos.NumProducto ?? "").Trim() equals (prods.NumProducto ?? "").Trim()
                              select new { saldos, prods }).ToList();
            foreach (var item in resultados)
            {
                item.saldos.CatProducto = item.prods.CatProducto;
            }
        }

        public void CruzaInformacionTipoProductoSaldosConCastigo(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<CatalogoProductos> productos)
        {
            var resultados = (from saldos in saldosConCastigo
                              join prods in productos on (saldos.NumProducto ?? "").Trim() equals (prods.NumProducto ?? "").Trim()
                              select new { saldos, prods }).ToList();
            foreach (var item in resultados)
            {
                item.saldos.CatProducto = item.prods.CatProducto;
            }
        }

        public void CruzaInformacionMinistracionesGuardaValores(ref IEnumerable<MinistracionesMesa> pagosMesa, ref IEnumerable<InformacionGuardaValor> guardaValores)
        {
            var resultados = (from pags in pagosMesa
                              join gv in guardaValores on QuitaCastigo(pags.NumCredito) equals QuitaCastigo(gv.NumeroCredito)
                              select new { gv, pags }).ToList();
            foreach (var item in resultados)
            {
                item.gv.AperturaTiempoDoctor = item.pags.EsOrigenDelDoctor;
                item.pags.CuentaConGuardaValores = true;
            }
        }
    }
}
