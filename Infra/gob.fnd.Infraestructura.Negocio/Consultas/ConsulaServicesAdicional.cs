using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace gob.fnd.Infraestructura.Negocio.Consultas
{
    public partial class ConsultaServices : IConsultaServices
    {
        private readonly string _cargaCreditosCancelados;
        private readonly string _archivoDeExpedientesCancelados;
        private IEnumerable<CreditosCanceladosAplicacion>? _creditosCanceladosAplicacion;
        private IEnumerable<DetalleBienesAdjudicados>? _detalleBienesAdjudicados;

        public IEnumerable<CreditosCanceladosAplicacion> CargaCreditosCancelados() 
        {
            IEnumerable<CreditosCanceladosAplicacion> resultado; 
            FileInfo fi = new (_archivoDeExpedientesCancelados);
            if (fi.Exists)
            {
                resultado = _administraCreditosCanceladosService.CargaExpedienteCancelados(_archivoDeExpedientesCancelados);
                _creditosCanceladosAplicacion = resultado;
                return _creditosCanceladosAplicacion;
            }

            var creditosCancelados = _creditosCanceladosService.GetCreditosCancelados(_cargaCreditosCancelados);
            resultado = creditosCancelados.Where(x => x.PrimeraDispersion >= new DateTime(2020, 07, 1))
                .Select(x => new CreditosCanceladosAplicacion() 
                { 
                    NumCreditoCancelado = x.NumCreditoActual,
                    NumCreditoOrigen = x.NumCreditoOrigen,
                    NumRegion = Convert.ToInt32((x.NumCreditoOrigen??"0")[..1])*100,
                    CatRegion = x.CoordinacionRegional,
                    Agencia = Convert.ToInt32((x.NumCreditoOrigen ?? "000")[..3]),
                    CatAgencia = x.Agencia,
                    NumCliente = x.NumCliente,
                    Acreditado = x.Acreditado,
                    TipoPersona = x.TipoPersona,
                    AnioOriginacion = x.AnioOrignacion,
                    PisoCredito = x.Piso,
                    Portafolio = x.Portafolio,
                    Concepto = x.Concepto,
                    FechaCancelacion = x.FechaCancelacion,
                    FechaPrimeraDispersion = x.PrimeraDispersion,
                    TieneImagenDirecta = false,
                    TieneImagenIndirecta = false
                }).ToList();

            _creditosCanceladosAplicacion = resultado;
            // Aqui se cruza con imágenes
            if (_imagenesCortas is not null)
            {
                CruzaInformacionImagenesConCreditosCancelados(_imagenesCortas);
            }
            _creaArchivoCsvService.CreaArchivoCsv(_archivoDeExpedientesCancelados, resultado);
            return resultado;
        }
        public int CalculaCreditosCancelados() 
        {
            if (_creditosCanceladosAplicacion is not null) {
                return _creditosCanceladosAplicacion.Count();
            }
            return 0;
        }
        public IEnumerable<CreditosCanceladosResumen> CalculaCreditosCanceladosRegiones() 
        {
            if (_creditosCanceladosAplicacion is null)
                return new List<CreditosCanceladosResumen>();
            IList < CreditosCanceladosResumen > resultado = new List<CreditosCanceladosResumen >();  
            IEnumerable <CreditosCanceladosResumen> resultadoAgrupado = _creditosCanceladosAplicacion.GroupBy(x => x.NumRegion).Select(y => new CreditosCanceladosResumen()
            {
                Region = y.Key,
                CatRegion = y.First().CatRegion,
                CantidadCreditos = y.Count(),
                CantidadPersonasFisicas = y.Count(z => (z.TipoPersona ?? "").Equals("Fisica", StringComparison.InvariantCultureIgnoreCase)),
                CantidadPersonasMorales = y.Count(z => (z.TipoPersona ?? "").Equals("Moral", StringComparison.InvariantCultureIgnoreCase)),
                CantidadAnio2020 = y.Count(z=>z.AnioOriginacion == 2020),
                CantidadAnio2021 = y.Count(z => z.AnioOriginacion == 2021),
                CantidadAnio2022 = y.Count(z => z.AnioOriginacion == 2022),
                CantidadAnio2023 = y.Count(z => z.AnioOriginacion == 2023),
                CantidadPrimerPiso = y.Count(z=> z.PisoCredito == 1),
                CantidadSegundoPiso = y.Count(z => z.PisoCredito == 2),
                CantidadFira = y.Count(z => (z.Concepto?? "").Equals("Fira", StringComparison.InvariantCultureIgnoreCase)),
                CantidadFondosMutuales = y.Count(z => (z.Concepto ?? "").Equals("Fondos Mutuales", StringComparison.InvariantCultureIgnoreCase)),
                CantidadReservasPreventivas = y.Count(z => (z.Concepto ?? "").Equals("Reservas Preventivas", StringComparison.InvariantCultureIgnoreCase))
            });

            foreach (var reporteRegiones in resultadoAgrupado)
            {
                var datosRegion = _creditosCanceladosAplicacion.
                    Where(x => x.NumRegion == reporteRegiones.Region);
                reporteRegiones.CantidadClientes = datosRegion.GroupBy(x => x.NumCliente).Count();

                resultado.Add(reporteRegiones);
            }
            var registroTotales = new CreditosCanceladosResumen()
            { 
                Region = 0,
                CatRegion = "Totales",
                CantidadCreditos = resultado.Sum(x => x.CantidadCreditos ),
                CantidadClientes = resultado.Sum(x=>x.CantidadClientes),
                CantidadPersonasFisicas = resultado.Sum(x => x.CantidadPersonasFisicas),
                CantidadPersonasMorales = resultado.Sum(x => x.CantidadPersonasMorales),
                CantidadAnio2020 = resultado.Sum(x => x.CantidadAnio2020),
                CantidadAnio2021 = resultado.Sum(x => x.CantidadAnio2021),
                CantidadAnio2022 = resultado.Sum(x => x.CantidadAnio2022),
                CantidadAnio2023 = resultado.Sum(x => x.CantidadAnio2023),
                CantidadPrimerPiso = resultado.Sum(x => x.CantidadPrimerPiso),
                CantidadSegundoPiso = resultado.Sum(x => x.CantidadSegundoPiso),
                CantidadFira = resultado.Sum(x => x.CantidadFira),
                CantidadFondosMutuales = resultado.Sum(x => x.CantidadFondosMutuales),
                CantidadReservasPreventivas = resultado.Sum(x => x.CantidadReservasPreventivas)
            };

            resultado.Add(registroTotales);
            return resultado.ToList();
        }
        public IEnumerable<CreditosCanceladosRegion> CalculaCreditosCanceladosRegion(int region)
        {
            if (_creditosCanceladosAplicacion is null)
                return new List<CreditosCanceladosRegion>();
            IList<CreditosCanceladosRegion> resultado = new List<CreditosCanceladosRegion>();
            IEnumerable<CreditosCanceladosRegion> resultadoAgrupado = _creditosCanceladosAplicacion.Where(alfa=>alfa.NumRegion == region).ToList().GroupBy(x => x.Agencia).Select(y => new CreditosCanceladosRegion()
            {
                // Region = region,
                Agencia = y.Key,
                CatAgencia = y.First().CatAgencia,
                CantidadCreditos = y.Count(),
                CantidadPersonasFisicas = y.Count(z => (z.TipoPersona ?? "").Equals("Fisica", StringComparison.InvariantCultureIgnoreCase)),
                CantidadPersonasMorales = y.Count(z => (z.TipoPersona ?? "").Equals("Moral", StringComparison.InvariantCultureIgnoreCase)),
                CantidadAnio2020 = y.Count(z => z.AnioOriginacion == 2020),
                CantidadAnio2021 = y.Count(z => z.AnioOriginacion == 2021),
                CantidadAnio2022 = y.Count(z => z.AnioOriginacion == 2022),
                CantidadAnio2023 = y.Count(z => z.AnioOriginacion == 2023),
                CantidadPrimerPiso = y.Count(z => z.PisoCredito == 1),
                CantidadSegundoPiso = y.Count(z => z.PisoCredito == 2),
                CantidadFira = y.Count(z => (z.Concepto ?? "").Equals("Fira", StringComparison.InvariantCultureIgnoreCase)),
                CantidadFondosMutuales = y.Count(z => (z.Concepto ?? "").Equals("Fondos Mutuales", StringComparison.InvariantCultureIgnoreCase)),
                CantidadReservasPreventivas = y.Count(z => (z.Concepto ?? "").Equals("Reservas Preventivas", StringComparison.InvariantCultureIgnoreCase))
            });

            foreach (var reporteAgencias in resultadoAgrupado)
            {
                var datosRegion = _creditosCanceladosAplicacion.
                    Where(x => x.Agencia == reporteAgencias.Agencia);
                reporteAgencias.CantidadClientes = datosRegion.GroupBy(x => x.NumCliente).Count();

                resultado.Add(reporteAgencias);
            }
            var registroTotales = new CreditosCanceladosRegion()
            {
                Agencia = 0,
                CatAgencia = "Totales",
                CantidadCreditos = resultado.Sum(x => x.CantidadCreditos),
                CantidadClientes = resultado.Sum(x => x.CantidadClientes),
                CantidadPersonasFisicas = resultado.Sum(x => x.CantidadPersonasFisicas),
                CantidadPersonasMorales = resultado.Sum(x => x.CantidadPersonasMorales),
                CantidadAnio2020 = resultado.Sum(x => x.CantidadAnio2020),
                CantidadAnio2021 = resultado.Sum(x => x.CantidadAnio2021),
                CantidadAnio2022 = resultado.Sum(x => x.CantidadAnio2022),
                CantidadAnio2023 = resultado.Sum(x => x.CantidadAnio2023),
                CantidadPrimerPiso = resultado.Sum(x => x.CantidadPrimerPiso),
                CantidadSegundoPiso = resultado.Sum(x => x.CantidadSegundoPiso),
                CantidadFira = resultado.Sum(x => x.CantidadFira),
                CantidadFondosMutuales = resultado.Sum(x => x.CantidadFondosMutuales),
                CantidadReservasPreventivas = resultado.Sum(x => x.CantidadReservasPreventivas)
            };

            resultado.Add(registroTotales);
            return resultado.ToList();
        }
        public IEnumerable<CreditosCanceladosDetalle> CalculaCreditosCanceladosAgencia(int agencia)
        {
            if (_creditosCanceladosAplicacion is null)
                return new List<CreditosCanceladosDetalle>();
            IList<CreditosCanceladosDetalle> resultado = new List<CreditosCanceladosDetalle>();
            IEnumerable<CreditosCanceladosDetalle> reporteAgencias = _creditosCanceladosAplicacion.Where(alfa => alfa.Agencia == agencia).Select(y => new CreditosCanceladosDetalle()
            {
                Region = y.NumRegion,
                Agencia = y.Agencia,
                NumCreditoCancelado = y.NumCreditoCancelado,
                NumCliente = y.NumCliente,
                Acreditado = y.Acreditado,
                FechaCancelacion = y.FechaCancelacion,
                FechaPrimeraDispersion = y.FechaPrimeraDispersion,
                PersonaFisica = (y.TipoPersona ?? "").Equals("Fisica", StringComparison.InvariantCultureIgnoreCase)?1:0,
                PersonaMoral = (y.TipoPersona ?? "").Equals("Moral", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0,
                Anio2020 = (y.AnioOriginacion == 2020) ? 1 : 0,
                Anio2021 = (y.AnioOriginacion == 2021) ? 1 : 0,
                Anio2022 = (y.AnioOriginacion == 2022) ? 1 : 0,
                Anio2023 = (y.AnioOriginacion == 2023) ? 1 : 0,
                PrimerPiso = (y.PisoCredito == 1) ? 1 : 0,
                SegundoPiso = (y.PisoCredito == 2) ? 1 : 0,
                Fira = (y.Concepto ?? "").Equals("Fira", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0,
                FondosMutuales = (y.Concepto ?? "").Equals("Fondos Mutuales", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0,
                ReservasPreventivas = (y.Concepto ?? "").Equals("Reservas Preventivas", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0,
                TieneImagenDirecta = y.TieneImagenDirecta,
                TieneImagenIndirecta = y.TieneImagenIndirecta

            });

            var registroTotales = new CreditosCanceladosDetalle()
            {
                Region = 0,
                Agencia = 0,
                NumCliente = String.Format("{0:#,##0}", reporteAgencias.GroupBy(x=>x.NumCliente).Count()),
                Acreditado = String.Format("Total créditos {0:#,##0}",reporteAgencias.Count()),
                FechaCancelacion = null,
                FechaPrimeraDispersion = null,
                PersonaFisica = reporteAgencias.Sum(x=>x.PersonaFisica),
                PersonaMoral = reporteAgencias.Sum(x => x.PersonaMoral),
                Anio2020 = reporteAgencias.Sum(x => x.Anio2020),
                Anio2021 = reporteAgencias.Sum(x => x.Anio2021),
                Anio2022 = reporteAgencias.Sum(x => x.Anio2022),
                Anio2023 = reporteAgencias.Sum(x => x.Anio2023),
                PrimerPiso = reporteAgencias.Sum(x => x.PrimerPiso),
                SegundoPiso = reporteAgencias.Sum(x => x.SegundoPiso),
                Fira = reporteAgencias.Sum(x => x.Fira),
                FondosMutuales = reporteAgencias.Sum(x => x.FondosMutuales),
                ReservasPreventivas = reporteAgencias.Sum(x => x.ReservasPreventivas)
            };
            ((List<CreditosCanceladosDetalle>)resultado).AddRange(reporteAgencias);
            resultado.Add(registroTotales);
            return resultado.ToList();
        }

        private bool CruzaInformacionImagenesConCreditosCancelados(IEnumerable<ArchivoImagenCorta> imagenes)
        {
            if (_creditosCanceladosAplicacion is not null)
            {
                var cruzaImagenes = (from scb in _creditosCanceladosAplicacion
                                     join img in imagenes on QuitaCastigo(scb.NumCreditoCancelado) equals QuitaCastigo(img.NumCredito)
                                     select new
                                     {
                                         Saldos = scb,
                                         Imagenes = img
                                     }).ToList();
                foreach (var cruce in cruzaImagenes)
                {
                    cruce.Saldos.TieneImagenDirecta = true;
                    cruce.Saldos.TieneImagenIndirecta = false;
                }

                var cruzaImagenesSinImagenDirecta = (from scb in _creditosCanceladosAplicacion.Where(x => !x.TieneImagenDirecta)
                                                     join img in imagenes on QuitaCastigo(scb.NumCreditoCancelado)[..14] equals QuitaCastigo(img.NumCredito)[..14]
                                                     select new
                                                     {
                                                         Saldos = scb,
                                                         Imagenes = img
                                                     }).ToList();

                foreach (var cruce in cruzaImagenesSinImagenDirecta)
                {
                    if (!cruce.Saldos.TieneImagenDirecta)
                    {
                        // cruce.Saldos.TieneImagenDirecta = true;
                        cruce.Saldos.TieneImagenIndirecta = true;
                    }
                }

            }
            return true;
        }

        /// <summary>
        /// Obtiene el número de region dependiendo del string de Entregado de Bienes Adjudicados
        /// 
        /// Los nombres de la region pueden ser:
        /// 100 C-O Centro Occidente
        /// 200 N-O Noroeste
        /// 300 Nte Norte
        /// 500 Sur Sur
        /// 600 S-E Sur este
        /// </summary>
        /// <param name="catRegion">Nombre de la región</param>
        /// <returns></returns>
        private static int ObtieneRegion(string? catRegion) {
            if (string.IsNullOrEmpty(catRegion))
                return 0;
            if (catRegion.Equals("C-O", StringComparison.InvariantCultureIgnoreCase) || catRegion.Equals("Centro Occidente", StringComparison.InvariantCultureIgnoreCase))
                return 100;
            if (catRegion.Equals("N-O", StringComparison.InvariantCultureIgnoreCase) || catRegion.Equals("Noroeste", StringComparison.InvariantCultureIgnoreCase))
                return 200;
            if (catRegion.Equals("Nte", StringComparison.InvariantCultureIgnoreCase) || catRegion.Equals("Norte", StringComparison.InvariantCultureIgnoreCase))
                return 300;
            if (catRegion.Equals("Sur", StringComparison.InvariantCultureIgnoreCase) || catRegion.Equals("Sur", StringComparison.InvariantCultureIgnoreCase))
                return 500;
            if (catRegion.Equals("S-E", StringComparison.InvariantCultureIgnoreCase) || catRegion.Equals("Sur este", StringComparison.InvariantCultureIgnoreCase))
                return 600;

            return 0; 
        }

        private static string ObtieneCatRegion(int region) {
            return region switch
            {
                100 => "Centro occidente",
                200 => "Noroeste",
                300 => "Norte",
                500 => "Sur",
                600 => "Sur este",
                _ => string.Empty,
            };
        }

        public int CalculaBienesAdjudicados()
        {
            if (_detalleBienesAdjudicados is not null && !File.Exists(_archivoBienesAdjudicados)) {
                return _detalleBienesAdjudicados.Count();
            }
            if (File.Exists(_archivoBienesAdjudicados)) {
                var detalleBienesAdjudicadosCarga = _administraCargaBienesAdjudicados.CargaBiensAdjudicados(_archivoBienesAdjudicados);
                _detalleBienesAdjudicados = ConvierteDetalle(detalleBienesAdjudicadosCarga);
                return _detalleBienesAdjudicados.Count();
            }
            var fuenteBienesAdjudicados = _bienesAdjudicadosService.ObtieneFuenteBienesAdjudicados();
            IList<DetalleBienesAdjudicados> detalleBienesAdjudicados = new List<DetalleBienesAdjudicados>();
            var resultadoFuenteBienesAdjudicados = fuenteBienesAdjudicados.Select(x => new DetalleBienesAdjudicados() {
                NumRegion = ObtieneRegion(x.CoordinacionRegional),
                CatRegion = ObtieneCatRegion(ObtieneRegion(x.CoordinacionRegional)),
                Entidad = x.EntidadFederativa,
                CveBien = x.CveBien,
                ExpedienteElectronico = x.ExpedienteElectronico,
                Acreditado = x.Acreditado,
                TipoDeBien = x.TipoDeBien,
                DescripcionReducidaBien = (x.DescripcionReducidaBien??"").Replace("\r\n", "¬").Replace("\n", "¬"),
                DescripcionCompletaBien = (x.DescripcionCompletaBien??"").Replace("\r\n", "¬").Replace("\n", "¬"),
                OficioNotificaiconGCRJ = (x.OficioNotificaiconGCRJ ?? "").Replace("\r\n", "¬").Replace("\n", "¬"),
                NumCredito = (x.NumCredito??"").Replace("\r\n", "¬").Replace("\n", "¬"),
                TipoAdjudicacion = x.TipoDeAdjudicacion,
                AreaResponsable = x.AreaResponsable,
                ImporteIndivAdjudicacion = x.ImporteIndivAdjudicacion,
                ImporteIndivImplicado = x.ImporteIndivImplicado,
                Estatus = x.Estatus
            }).ToList();
            // Dejo espacio para hacer ajustes

            ((List<DetalleBienesAdjudicados>)detalleBienesAdjudicados).AddRange(resultadoFuenteBienesAdjudicados);
            // _detalleBienesAdjudicados = detalleBienesAdjudicados.ToList();
            _creaArchivoCsvService.CreaArchivoCsv(_archivoBienesAdjudicados, detalleBienesAdjudicados);
            _detalleBienesAdjudicados = ConvierteDetalle(detalleBienesAdjudicados);
            return fuenteBienesAdjudicados.Count();
        }

        private static IEnumerable<DetalleBienesAdjudicados> ConvierteDetalle(IEnumerable<DetalleBienesAdjudicados> origen) 
        {
            IEnumerable<DetalleBienesAdjudicados> resultado = origen.Select(x => new DetalleBienesAdjudicados() {
                NumRegion = x.NumRegion,
                CatRegion = x.CatRegion,
                Entidad = x.Entidad,
                CveBien = x.CveBien,
                ExpedienteElectronico = x.ExpedienteElectronico,
                Acreditado = x.Acreditado,
                TipoDeBien = x.TipoDeBien,
                DescripcionReducidaBien = (x.DescripcionReducidaBien??"").Replace("¬", "\r\n"),
                DescripcionCompletaBien = (x.DescripcionCompletaBien ?? "").Replace("¬", "\r\n"),
                OficioNotificaiconGCRJ = (x.OficioNotificaiconGCRJ ?? "").Replace("¬", "\r\n"),
                NumCredito = (x.NumCredito ?? "").Replace("¬", "\r\n"),
                TipoAdjudicacion = x.TipoAdjudicacion,
                AreaResponsable = x.AreaResponsable,
                ImporteIndivAdjudicacion = x.ImporteIndivAdjudicacion,
                ImporteIndivImplicado = x.ImporteIndivImplicado,
                Estatus = x.Estatus

            }).ToList();
            return resultado;
        }
        private static bool EsBienMueble(string? tipoDeBien) { 
            if (string.IsNullOrEmpty(tipoDeBien))
                return false;
            string[] criterio = new string[] { "Mueble", "Mueble (Diversos)", "Mueble (Maquinaria)", "Mueble (Vehíclos)" };
            
            return criterio.Any(x=>x.Equals(tipoDeBien,StringComparison.InvariantCultureIgnoreCase));
        }
        private static bool EsBienInMueble(string? tipoDeBien)
        {
            if (string.IsNullOrEmpty(tipoDeBien))
                return false;
            string[] criterio = new string[] { "Inmueble", "Inmueble rústico", "Inmueble urbano" };

            return criterio.Any(x => x.Equals(tipoDeBien, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool EsAdjudicacionJudicial(string? tipoDeAdjuicacion)
        {
            if (string.IsNullOrEmpty(tipoDeAdjuicacion))
                return false;
            // Se sigue manejando el criterio para simplificar en caso de que venga diferente en el excel de origen
            string[] criterio = new string[] { "Adjudicación Judicial" };

            return criterio.Any(x => x.Equals(tipoDeAdjuicacion, StringComparison.InvariantCultureIgnoreCase));
        }
        
        private static bool EsDacionDePago(string? tipoDeAdjuicacion)
        {
            if (string.IsNullOrEmpty(tipoDeAdjuicacion))
                return false;

            // Se sigue manejando el criterio para simplificar en caso de que venga diferente en el excel de origen
            string[] criterio = new string[] { "Dación en pago" };

            return criterio.Any(x => x.Equals(tipoDeAdjuicacion, StringComparison.InvariantCultureIgnoreCase));
        }


        public IEnumerable<ResumenBienesAdjudicados> CalculaResumenBienesAdjudicados()
        {
            if (_detalleBienesAdjudicados is null)
                return new List<ResumenBienesAdjudicados>();
            IList<ResumenBienesAdjudicados> resultado = new List<ResumenBienesAdjudicados>();
            IEnumerable<ResumenBienesAdjudicados> resultadoAgrupado = _detalleBienesAdjudicados.GroupBy(x => x.NumRegion).Select(y => new ResumenBienesAdjudicados()
            {
                Region = y.Key,
                CatRegion = y.First().CatRegion,
                CantidadDeBienes = y.Count(),

                CantidadBienesMuebles = y.Count(z=> EsBienMueble(z.TipoDeBien)),
                CantidadBienesInmuebles = y.Count(z => EsBienInMueble(z.TipoDeBien)),
                TipoAdjudicacionJudicial = y.Count(z => EsAdjudicacionJudicial(z.TipoAdjudicacion)),
                TipoDacionDePago = y.Count(z => EsDacionDePago(z.TipoAdjudicacion)),

                AreaResponsableBaja = y.Count(z => (z.AreaResponsable ??"").Equals("Baja",StringComparison.InvariantCultureIgnoreCase) ),
                AreaResponsableIndep = y.Count(z => (z.AreaResponsable ?? "").Equals("INDEP", StringComparison.InvariantCultureIgnoreCase)),
                AreaResponsableIndepVenta = y.Count(z => (z.AreaResponsable ?? "").Equals("INDEP Venta/confirmar", StringComparison.InvariantCultureIgnoreCase)),
                AreaResponsableJuridico = y.Count(z => (z.AreaResponsable ?? "").Equals("Jurídico", StringComparison.InvariantCultureIgnoreCase)),
                EnIntegracionDeExpediente = y.Count(z => (z.AreaResponsable ?? "").Equals("DERMS", StringComparison.InvariantCultureIgnoreCase))
            });

            foreach (var reporteRegiones in resultadoAgrupado)
            {
                var datosRegion = _detalleBienesAdjudicados.
                    Where(x => x.NumRegion == reporteRegiones.Region);
                reporteRegiones.CantidadClientes = datosRegion.GroupBy(x => x.Acreditado).Count();

                resultado.Add(reporteRegiones);
            }
            var registroTotales = new ResumenBienesAdjudicados()
            {
                Region = 0,
                CatRegion = "Totales",
                CantidadDeBienes = resultado.Sum(x => x.CantidadDeBienes),
                CantidadBienesMuebles = resultado.Sum(x => x.CantidadBienesMuebles),
                CantidadBienesInmuebles = resultado.Sum(x => x.CantidadBienesInmuebles),
                CantidadClientes = resultado.Sum(x => x.CantidadClientes),
                TipoAdjudicacionJudicial = resultado.Sum(x => x.TipoAdjudicacionJudicial),
                TipoDacionDePago = resultado.Sum(x => x.TipoDacionDePago),
                AreaResponsableBaja = resultado.Sum(x => x.AreaResponsableBaja),
                AreaResponsableIndep = resultado.Sum(x => x.AreaResponsableIndep),
                AreaResponsableIndepVenta = resultado.Sum(x => x.AreaResponsableIndepVenta),
                AreaResponsableJuridico = resultado.Sum(x => x.AreaResponsableJuridico),
                EnIntegracionDeExpediente = resultado.Sum(x => x.EnIntegracionDeExpediente)
            };

            resultado.Add(registroTotales);
            return resultado.ToList();

        }

        public IEnumerable<EntidadesBienesAdjudicados> CalculaEntidadesBienesAdjudicados(int region)
        {
            if (_detalleBienesAdjudicados is null)
                return new List<EntidadesBienesAdjudicados>();
            IList<EntidadesBienesAdjudicados> resultado = new List<EntidadesBienesAdjudicados>();
            IEnumerable<EntidadesBienesAdjudicados> resultadoAgrupado = _detalleBienesAdjudicados.Where(alfa => alfa.NumRegion == region).ToList().GroupBy(x => x.Entidad).Select(y => new EntidadesBienesAdjudicados()
            {
                NumRegion = region,
                CatEntidad = y.First().Entidad,
                CantidadDeBienes = y.Count(),
                CantidadBienesMuebles = y.Count(z => EsBienMueble(z.TipoDeBien)),
                CantidadBienesInmuebles = y.Count(z => EsBienInMueble(z.TipoDeBien)),
                TipoAdjudicacionJudicial = y.Count(z => EsAdjudicacionJudicial(z.TipoAdjudicacion)),
                TipoDacionDePago = y.Count(z => EsDacionDePago(z.TipoAdjudicacion)),

                AreaResponsableBaja = y.Count(z => (z.AreaResponsable ?? "").Equals("Baja", StringComparison.InvariantCultureIgnoreCase)),
                AreaResponsableIndep = y.Count(z => (z.AreaResponsable ?? "").Equals("INDEP", StringComparison.InvariantCultureIgnoreCase)),
                AreaResponsableIndepVenta = y.Count(z => (z.AreaResponsable ?? "").Equals("INDEP Venta/confirmar", StringComparison.InvariantCultureIgnoreCase)),
                AreaResponsableJuridico = y.Count(z => (z.AreaResponsable ?? "").Equals("Jurídico", StringComparison.InvariantCultureIgnoreCase)),
                EnIntegracionDeExpediente = y.Count(z => (z.AreaResponsable ?? "").Equals("DERMS", StringComparison.InvariantCultureIgnoreCase))
            });

            foreach (var reporteAgencias in resultadoAgrupado)
            {
                var datosRegion = _detalleBienesAdjudicados.
                    Where(x => x.Entidad == reporteAgencias.CatEntidad);
                reporteAgencias.CantidadClientes = datosRegion.GroupBy(x => x.Acreditado).Count();

                resultado.Add(reporteAgencias);
            }
            var registroTotales = new EntidadesBienesAdjudicados()
            {
                NumRegion = 0,
                CatEntidad = "Totales",
                CantidadDeBienes = resultado.Sum(x => x.CantidadDeBienes),
                CantidadBienesMuebles = resultado.Sum(x => x.CantidadBienesMuebles),
                CantidadBienesInmuebles = resultado.Sum(x => x.CantidadBienesInmuebles),
                CantidadClientes = resultado.Sum(x => x.CantidadClientes),
                TipoAdjudicacionJudicial = resultado.Sum(x => x.TipoAdjudicacionJudicial),
                TipoDacionDePago = resultado.Sum(x => x.TipoDacionDePago),
                AreaResponsableBaja = resultado.Sum(x => x.AreaResponsableBaja),
                AreaResponsableIndep = resultado.Sum(x => x.AreaResponsableIndep),
                AreaResponsableIndepVenta = resultado.Sum(x => x.AreaResponsableIndepVenta),
                AreaResponsableJuridico = resultado.Sum(x => x.AreaResponsableJuridico),
                EnIntegracionDeExpediente = resultado.Sum(x => x.EnIntegracionDeExpediente)
            };

            resultado.Add(registroTotales);
            return resultado.ToList();
        }

        public IEnumerable<DetalleBienesAdjudicados> CalculaDetalleBienesAdjudicados(int region, string? entidad)
        {
            if (_detalleBienesAdjudicados is null)
                return new List<DetalleBienesAdjudicados>();
            IList<DetalleBienesAdjudicados> resultado = new List<DetalleBienesAdjudicados>();
            IEnumerable<DetalleBienesAdjudicados> reporteAgencias = _detalleBienesAdjudicados.Where(alfa => (alfa.Entidad??"").Equals(entidad,StringComparison.InvariantCultureIgnoreCase) ).ToList();
            
            ((List<DetalleBienesAdjudicados>)resultado).AddRange(reporteAgencias);
            
            return resultado.ToList();
        }

        public string ObtieneUnidadImagenes()
        {
            DriveInfo[] drives = DriveInfo.GetDrives().Where(z=>(z.DriveType != DriveType.CDRom) && (!z.Name.Contains("C:\\", StringComparison.InvariantCultureIgnoreCase)))
                .OrderByDescending(s=>s.TotalSize).Take(3).ToArray()
                .OrderByDescending(x=>x.Name).ToArray();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.CDRom)
                {
                    // La unidad es un CD/DVD, no se puede verificar la carpeta "imagenes".
                    continue;
                }

                if (drive.RootDirectory.GetDirectories().Any(d => d.Name == "11"))
                {
                    return drive.Name;
                    // Console.WriteLine("La carpeta 'imagenes' existe en la unidad {0}.", drive.Name);
                }
            }
            return @"G:\";
        }


        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        public IEnumerable<ArchivoImagenBienesAdjudicadosCorta> CargaInformacionImagenesBienesAdjudicados()
        {
            FileInfo fi = new(_archivosImagenesBienesAdjudicadosCorta);
            if (fi.Exists)
            {
                _imagenesCortasBienesAdjudicados = _administraCargaConsultaService.CargaArchivoImagenBienesAdjudicadosCorta(_archivosImagenesBienesAdjudicadosCorta);
                _logger.LogInformation("Cargo la información de los expedientes previamente procesada");

                CambiaUnidadBienesAdjudicados();

                return _imagenesCortasBienesAdjudicados;
            }
            // IList<ArchivoImagenCorta> lista = new List<ArchivoImagenCorta>();
            IEnumerable<ArchivosImagenes> resultado = _servicioImagenes.CargaImagenesTratadas(_archivosImagenesBienesAdjudicados).Where(x=>(x.Extension??"").Equals(".pdf",StringComparison.InvariantCultureIgnoreCase));
            var resultadoCorto = resultado.Select(x => new ArchivoImagenBienesAdjudicadosCorta()
            {
                CarpetaDestino = x.CarpetaDestino,
                Hash = x.Hash,
                Id = x.Id,
                Acreditados = ((x.CarpetaDestino??"").Split('\\')[4]),
                NombreArchivo = x.NombreArchivo,
                NumPaginas = x.NumPaginas,
                NumExpediente = (x.NumCredito??"")[..8]
            }).ToList();
            _ = _creaArchivoCsvService.CreaArchivoCsv(_archivosImagenesBienesAdjudicadosCorta, resultadoCorto);
            _imagenesCortasBienesAdjudicados = resultadoCorto;
            CambiaUnidadBienesAdjudicados();
            return _imagenesCortasBienesAdjudicados;
        }
        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        public IEnumerable<ArchivoImagenBienesAdjudicadosCorta> BuscaImagenesBienesAdjudicados(string numeroDeExpediente, string acreditados, bool directas = false)
        {
            // Se crea una nueva lista de objetos "ArchivoImagenBienesAdjudicadosCorta"
            IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas = new List<ArchivoImagenBienesAdjudicadosCorta>();
            // Se comprueba si la variable "_imagenesCortasBienesAdjudicados" no es nula,
            // tiene algún elemento y las variables "numeroDeExpediente" y
            // "acreditados" no están vacías.
            if (_imagenesCortasBienesAdjudicados is not null && _imagenesCortasBienesAdjudicados.Any() && !string.IsNullOrEmpty(numeroDeExpediente) && !string.IsNullOrEmpty(acreditados))
            {

                // Se filtran los elementos de "_imagenesCortasBienesAdjudicados" y se asignan a la variable "listaImagenesEncontradas".
                listaImagenesEncontradas = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (numeroDeExpediente ?? "").Equals(x.NumExpediente, StringComparison.OrdinalIgnoreCase)).ToList();
                // Si la variable "listaImagenesEncontradas" está vacía o la variable "directas" es falsa,
                // se buscan imágenes adicionales que coincidan parcialmente con el "numeroDeExpediente"
                // y se agregan a "listaImagenesEncontradas". Sirve para agregar cuando no hay coincidencias exactas.
                if (!listaImagenesEncontradas.Any() || !directas)
                {
                    var imagenesAdicionales = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..7].Equals(numeroDeExpediente[..7], StringComparison.OrdinalIgnoreCase)).ToArray();
                    ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                }
                // Se llama a la función "FiltraMasdeUnAcreditado" con la variable "acreditados" y la variable "listaImagenesEncontradas", y se asigna el resultado a "listaImagenesEncontradas".
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados, listaImagenesEncontradas);
                // Se eliminan las imágenes duplicadas de la lista "listaImagenesEncontradas".
                listaImagenesEncontradas = listaImagenesEncontradas.GroupBy(x => x.Hash).Select(g => g.First()).ToList();
                // Se ordena la lista "listaImagenesEncontradas" de menor a mayor por la fecha que viene en el nombre del archivo.
                listaImagenesEncontradas = listaImagenesEncontradas.OrderBy(y => y.NombreArchivo).ToList();
            }
            // Se devuelve la variable "listaImagenesEncontradas" con las imágenes filtradas, sin duplicados y ordenadas.
            return listaImagenesEncontradas;
        }

        /// <summary>
        /// Como en los expedientes, los números se pueden repetir, pero con otros acreditados, 
        /// se observa el acreditado declarado en el expediente y se busca en las imágenes
        /// Filtrando aquellos que cumplan el criterio de manera parcial
        /// 
        /// Por ejemplo en el expediente, el acreditado aparece como: José Jaime Sánchez Chavez y
        /// en las imáges aparece cmo 20160913 José Jaime Sánchez Chávez
        /// 
        /// Por tanto busco por José en los acreditados, cuando encuentre más de un acreditado,
        /// y si el resultante ya es un registor único, lo regreso, si no, busco por Jaime, 
        /// y así sucesivamente
        /// </summary>
        /// <param name="acreditados">Nombres que aparecen en el expediente</param>
        /// <param name="listaImagenesEncontradas">Lista de imágenes, con su campo de acreditado por expediente</param>
        /// <returns>Lista de imágenes que cumplen con pertenecer a lo descrito en acreditados</returns>
        private static IList<ArchivoImagenBienesAdjudicadosCorta> FiltraMasdeUnAcreditado(string acreditados, IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas)
        {
            string?[] listaAcreditadosDirectorio = listaImagenesEncontradas.Select(x => x.Acreditados).Distinct().ToArray();
            if (listaAcreditadosDirectorio.Length == 1)
            {
                return listaImagenesEncontradas;
            }

            char[] splitArray = { '_', '-', '(', ')', ' ' };
            string[] listaAcreditados = acreditados.Split(splitArray);
            if (listaAcreditadosDirectorio is not null)
            {
                int encontro = 10;
                IList<string> palabrasPorBuscar = new List<string>();
                int criterio = 0;
                var buscaAcreditado = listaAcreditadosDirectorio;
                while ((encontro != 1) && (encontro != 0))
                {
                    palabrasPorBuscar.Add(RemoveAccentsWithNormalization(listaAcreditados[criterio]));
                    buscaAcreditado = buscaAcreditado.Where(x => RemoveAccentsWithNormalization(x ?? "").Contains(string.Join(" ", palabrasPorBuscar), StringComparison.OrdinalIgnoreCase)).ToArray();
                    encontro = buscaAcreditado.Length;
                    if (encontro == 0 && criterio == 0) // No existe informacion del acreditado, regresa lo que está
                    {
                        break;
                    }
                    else
                    {
                        listaImagenesEncontradas = listaImagenesEncontradas.Where(x => RemoveAccentsWithNormalization(x.Acreditados ?? "").Contains(string.Join(" ", palabrasPorBuscar), StringComparison.OrdinalIgnoreCase)).ToArray();
                    }
                    criterio++;
                }
            }

            return listaImagenesEncontradas;
        }

        // Funcion para quitar los acentos a un string y que queden como vocales
        public static string RemoveAccentsWithNormalization(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;
            inputString = inputString.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in inputString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
