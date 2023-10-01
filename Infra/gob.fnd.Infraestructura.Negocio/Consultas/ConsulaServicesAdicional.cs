using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteLiquidados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace gob.fnd.Infraestructura.Negocio.Consultas;

public partial class ConsultaServices : IConsultaServices
{

    public IEnumerable<CreditosCanceladosAplicacion> CargaCreditosCancelados()
    {
        IEnumerable<CreditosCanceladosAplicacion> resultado;
        FileInfo fi = new(_archivoDeExpedientesCanceladosCsv);
        if (fi.Exists)
        {
            resultado = _administraCreditosCanceladosService.CargaExpedienteCancelados(_archivoDeExpedientesCanceladosCsv);
            _creditosCanceladosAplicacion = resultado;
            return _creditosCanceladosAplicacion;
        }

        var creditosCancelados = _creditosCanceladosService.GetCreditosCancelados(_cargaCreditosCancelados);
        resultado = creditosCancelados.Where(x => x.PrimeraDispersion >= new DateTime(2020, 07, 1))
            .Select(x => new CreditosCanceladosAplicacion()
            {
                NumCreditoCancelado = x.NumCreditoActual,
                NumCreditoOrigen = x.NumCreditoOrigen,
                NumRegion = Convert.ToInt32((x.NumCreditoOrigen ?? "0")[..1]) * 100,
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
        _creaArchivoCsvService.CreaArchivoCsv(_archivoDeExpedientesCanceladosCsv, resultado);
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
        IList<CreditosCanceladosResumen> resultado = new List<CreditosCanceladosResumen>();
        IEnumerable<CreditosCanceladosResumen> resultadoAgrupado = _creditosCanceladosAplicacion.GroupBy(x => x.NumRegion).Select(y => new CreditosCanceladosResumen()
        {
            Region = y.Key,
            CatRegion = y.First().CatRegion,
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
    public IEnumerable<CreditosCanceladosRegion> CalculaCreditosCanceladosRegion(int region)
    {
        if (_creditosCanceladosAplicacion is null)
            return new List<CreditosCanceladosRegion>();
        IList<CreditosCanceladosRegion> resultado = new List<CreditosCanceladosRegion>();
        IEnumerable<CreditosCanceladosRegion> resultadoAgrupado = _creditosCanceladosAplicacion.Where(alfa => alfa.NumRegion == region).ToList().GroupBy(x => x.Agencia).Select(y => new CreditosCanceladosRegion()
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
            PersonaFisica = (y.TipoPersona ?? "").Equals("Fisica", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0,
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
            NumCliente = String.Format("{0:#,##0}", reporteAgencias.GroupBy(x => x.NumCliente).Count()),
            Acreditado = String.Format("Total créditos {0:#,##0}", reporteAgencias.Count()),
            FechaCancelacion = null,
            FechaPrimeraDispersion = null,
            PersonaFisica = reporteAgencias.Sum(x => x.PersonaFisica),
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
            var detalleBienesAdjudicadosCarga = _administraCargaBienesAdjudicados.CargaBienesAdjudicados(_archivoBienesAdjudicados);
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
            DescripcionReducidaBien = (x.DescripcionReducidaBien ?? "").Replace("\r\n", "¬").Replace("\n", "¬"),
            DescripcionCompletaBien = (x.DescripcionCompletaBien ?? "").Replace("\r\n", "¬").Replace("\n", "¬"),
            OficioNotificaiconGCRJ = (x.OficioNotificaiconGCRJ ?? "").Replace("\r\n", "¬").Replace("\n", "¬"),
            NumCredito = (x.NumCredito ?? "").Replace("\r\n", "¬").Replace("\n", "¬"),
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
            DescripcionReducidaBien = (x.DescripcionReducidaBien ?? "").Replace("¬", "\r\n"),
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

        return criterio.Any(x => x.Equals(tipoDeBien, StringComparison.InvariantCultureIgnoreCase));
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
        IEnumerable<DetalleBienesAdjudicados> reporteAgencias = _detalleBienesAdjudicados.Where(alfa => (alfa.Entidad ?? "").Equals(entidad, StringComparison.InvariantCultureIgnoreCase)).ToList();

        ((List<DetalleBienesAdjudicados>)resultado).AddRange(reporteAgencias);

        return resultado.ToList();
    }

    public string ObtieneUnidadImagenes()
    {
        DriveInfo[] drives = DriveInfo.GetDrives().Where(z => ((z.DriveType != DriveType.CDRom) &&(z.DriveType != DriveType.Network)) && (!z.Name.Contains("C:\\", StringComparison.InvariantCultureIgnoreCase)))
            .OrderByDescending(s => s.TotalSize).Take(3).ToArray()
            .OrderByDescending(x => x.Name).ToArray();

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
        IEnumerable<ArchivosImagenes> resultado = _servicioImagenes.CargaImagenesTratadas(_archivosImagenesBienesAdjudicados).Where(x => (x.Extension ?? "").Equals(".pdf", StringComparison.InvariantCultureIgnoreCase));
        var resultadoCorto = resultado.Select(x => new ArchivoImagenBienesAdjudicadosCorta()
        {
            CarpetaDestino = x.CarpetaDestino,
            Hash = x.Hash,
            Id = x.Id,
            Acreditados = ((x.CarpetaDestino ?? "").Split('\\')[4]),
            NombreArchivo = x.NombreArchivo,
            NumPaginas = x.NumPaginas,
            NumExpediente = (x.NumCredito ?? "")[..8]
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
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..7].Equals(numeroDeExpediente[..7], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
            }
            // Se llama a la función "FiltraMasdeUnAcreditado" con la variable "acreditados" y la variable "listaImagenesEncontradas", y se asigna el resultado a "listaImagenesEncontradas".
            listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados ?? "", listaImagenesEncontradas);
            #region Vuelvo a buscar, pero quitando los días
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..6].Equals(numeroDeExpediente[..6], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Vuelvo a buscar, dejando solo el año
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..4].Equals(numeroDeExpediente[..4], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Vuelvo a buscar, quitando el año y dejando mes y día
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "").Substring(4, 4).Equals(numeroDeExpediente.Substring(4, 4), StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Si de plano no hay nada, espero que sea una empresa y sea solo una palabra
            if (!listaImagenesEncontradas.Any())
            {
                listaImagenesEncontradas = _imagenesCortasBienesAdjudicados.OrderByDescending(y => y.NumPaginas).Where(x => numeroDeExpediente.Equals(x.NumExpediente, StringComparison.OrdinalIgnoreCase)).ToList();
                // Ya solo se busca por una palabra
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(acreditados ?? "", listaImagenesEncontradas, true);
            }
            #endregion
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
    /// <param name="soloUnaPalabra">Se busca solo una palabra cuando son empresas</param>
    /// <returns>Lista de imágenes que cumplen con pertenecer a lo descrito en acreditados</returns>
    private static IList<ArchivoImagenBienesAdjudicadosCorta> FiltraMasdeUnAcreditado(string acreditados, IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas, bool soloUnaPalabra = false)
    {
        string?[] listaAcreditadosDirectorio = listaImagenesEncontradas.Select(x => x.Acreditados).Distinct().ToArray();
        if (listaAcreditadosDirectorio.Length == 1)
        {
            return listaImagenesEncontradas;
        }

        char[] splitArray = { '_', '-', '(', ')', ' ', ',', '.', '=', '~', '|' };
        string[] listaAcreditados = acreditados.Split(splitArray).Where(x => !string.IsNullOrEmpty(x)).ToList().Where(x => x.Length > 3).ToArray();
        if (listaAcreditadosDirectorio is not null)
        {
            int encontro = 2;
            IList<string> palabrasPorBuscar = new List<string>();
            int criterio = 0;
            var buscaAcreditado = listaAcreditadosDirectorio;
            bool removioElUltimoCriterio = false;
            while ((encontro != 0) && (criterio < listaAcreditados.Length))
            {
                string palabraActual = RemoveAccentsWithNormalization(listaAcreditados[criterio]);
                if (palabraActual.Equals("Maria", StringComparison.InvariantCultureIgnoreCase))
                {
                    palabraActual = "M";
                }

                palabrasPorBuscar.Add(palabraActual);
                // necesito hacer un query que regrese si hay coincidencias con las palabras por buscar

                var temporalBuscaAcreditado = buscaAcreditado.Where(x => RemoveAccentsWithNormalization(x ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                encontro = temporalBuscaAcreditado.Length;
                if (encontro != 0)
                {
                    buscaAcreditado = temporalBuscaAcreditado;
                    removioElUltimoCriterio = false;
                }
                else
                {
                    // Necesito remover la ultima palabra de la lista de palabras a buscar
                    palabrasPorBuscar.RemoveAt(palabrasPorBuscar.Count - 1);
                    removioElUltimoCriterio = true;
                    /*
                    if (criterio + 1 == listaAcreditados.Length)
                    {
                        break;
                    }
                    */
                    encontro = 2;
                }
                criterio++;
            }
            // Si hay suficientes criterios para dar por buena el cruce, lo regreso, si no, regreso una lista vacía
            if (palabrasPorBuscar.Count > 1 || !removioElUltimoCriterio)
            {
                foreach (var palabraActual in palabrasPorBuscar)
                {
                    listaImagenesEncontradas = listaImagenesEncontradas.Where(x => RemoveAccentsWithNormalization(x.Acreditados ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                }
                return listaImagenesEncontradas;
            }
            else
            {
                if (soloUnaPalabra)
                {
                    foreach (var palabraActual in palabrasPorBuscar)
                    {
                        listaImagenesEncontradas = listaImagenesEncontradas.Where(x => RemoveAccentsWithNormalization(x.Acreditados ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                    }
                    return listaImagenesEncontradas;
                }
                return new List<ArchivoImagenBienesAdjudicadosCorta>();
            }
        }
        return new List<ArchivoImagenBienesAdjudicadosCorta>();
    }

    // Funcion para quitar los acentos a un string y que queden como vocales
    public static string RemoveAccentsWithNormalization(string inputString)
    {
        inputString = inputString.Replace("y", "i", StringComparison.InvariantCultureIgnoreCase);
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

    public IEnumerable<ColocacionConPagos> CargaColocacionConPagos()
    {
        IEnumerable<ColocacionConPagos> resultado;
        FileInfo fi = new(_archivoColocacionConPagosCsv);
        if (fi.Exists)
        {
            resultado = _administraCargaLiquidaciones.CargaLiquidacionesCompleta(_archivoColocacionConPagosCsv);
            _creditosLiquidados = resultado;
            return _creditosLiquidados;
        }

        var creditosLiquidados = _liquidacionesService.GetColocacionConPagos(_archivoColocacionConPagos);
        resultado = creditosLiquidados.ToList();
        _creditosLiquidados = resultado;
        // Aqui se cruza con imágenes
        if (_imagenesCortas is not null)
        {
            CruzaInformacionImagenesConCreditosLiquidados(_imagenesCortas);
        }
        _creaArchivoCsvService.CreaArchivoCsv(_archivoColocacionConPagosCsv, resultado);
        return resultado;
    }

    private bool CruzaInformacionImagenesConCreditosLiquidados(IEnumerable<ArchivoImagenCorta> imagenesCortas)
    {
        if (_creditosLiquidados is not null)
        {
            var cruzaImagenes = (from scb in _creditosLiquidados
                                 join img in imagenesCortas on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
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

            var cruzaImagenesSinImagenDirecta = (from scb in _creditosLiquidados.Where(x => !x.TieneImagenDirecta)
                                                 join img in imagenesCortas on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
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

    public int CalculaCreditosLiquidados()
    {
        if (_creditosLiquidados is not null)
        {
            return _creditosLiquidados.Count();
        }
        return 0;
    }

    public IEnumerable<CreditosLiquidadosRegiones> CalculaCreditosLiquidadosRegiones()
    {
        if (_creditosLiquidados is null)
            return new List<CreditosLiquidadosRegiones>();
        IList<CreditosLiquidadosRegiones> resultado = new List<CreditosLiquidadosRegiones>();
        IEnumerable<CreditosLiquidadosRegiones> resultadoAgrupado = _creditosLiquidados.GroupBy(x => x.Agencia.ToString()[..1]).Select(y => new CreditosLiquidadosRegiones()
        {
            Region = Convert.ToInt32(y.Key) * 100,
            CatRegion = y.First().CatRegionMigrado,
            CantidadAnterioresJul2020 = y.Count(z => z.FechaApertura < new DateTime(2020, 7, 1)),
            CantidadSegundoSemestre2020 = y.Count(z => z.FechaApertura >= new DateTime(2020, 7, 1) && z.FechaApertura <= new DateTime(2020, 12, 31)),
            Cantidad2021 = y.Count(z => z.FechaApertura >= new DateTime(2021, 1, 1) && z.FechaApertura <= new DateTime(2021, 12, 31)),
            Cantidad2022 = y.Count(z => z.FechaApertura >= new DateTime(2022, 1, 1) && z.FechaApertura <= new DateTime(2022, 12, 31)),
            Cantidad2023 = y.Count(z => z.FechaApertura >= new DateTime(2023, 1, 1) && z.FechaApertura <= new DateTime(2023, 12, 31)),
            TotalCreditos = y.Count(),
            CantidadReestructura = y.Count(z => (z.Reestructura ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)),
            CantidadCancelaciones = y.Count(z => (z.Cancelacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)),
            CantidadLiquidados = y.Count(z => !((z.Cancelacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase))
                                        && !((z.Reestructura ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)))
        });

        foreach (var reporteRegiones in resultadoAgrupado)
        {
            var datosRegion = _creditosLiquidados.
                Where(x => Convert.ToInt32(x.Agencia.ToString()[..1]) * 100 == reporteRegiones.Region);
            reporteRegiones.TotalClientes = datosRegion.GroupBy(x => x.NumCte).Count();

            var datosParcialesAnteriores = _creditosLiquidados.
                Where(x => (Convert.ToInt32(x.Agencia.ToString()[..1]) * 100 == reporteRegiones.Region) && (x.FechaApertura < new DateTime(2020, 7, 1)));

            SoloTotales resultadoSuma = datosParcialesAnteriores.Aggregate(
                new SoloTotales(), (total, colocacion) =>
                {
                    total.PagoCapital += colocacion.PagoCapital;
                    total.PagoInteres += colocacion.PagoInteres;
                    total.PagoMoratorios += colocacion.PagoMoratorios;
                    total.PagoTotal += colocacion.PagoTotal;
                    return total;
                }
            );

            reporteRegiones.PagoCapitalAntesPrimerSemestre2020 = resultadoSuma.PagoCapital;
            reporteRegiones.PagoInteresAntesPrimerSemestre2020 = resultadoSuma.PagoInteres;
            reporteRegiones.PagoMoratoriosAntesPrimerSemestre2020 = resultadoSuma.PagoMoratorios;
            reporteRegiones.PagoTotalAntesPrimerSemestre2020 = resultadoSuma.PagoTotal;

            var datosParcialesActuales = _creditosLiquidados.
                Where(x => (Convert.ToInt32(x.Agencia.ToString()[..1]) * 100 == reporteRegiones.Region) && (x.FechaApertura >= new DateTime(2020, 7, 1)));

            resultadoSuma = datosParcialesActuales.Aggregate(
                new SoloTotales(), (total, colocacion) =>
                {
                    total.PagoCapital += colocacion.PagoCapital;
                    total.PagoInteres += colocacion.PagoInteres;
                    total.PagoMoratorios += colocacion.PagoMoratorios;
                    total.PagoTotal += colocacion.PagoTotal;
                    return total;
                }
            );

            reporteRegiones.PagoCapitalDespuesSegundoSemestre2020 = resultadoSuma.PagoCapital;
            reporteRegiones.PagoInteresDespuesSegundoSemestre2020 = resultadoSuma.PagoInteres;
            reporteRegiones.PagoMoratoriosDespuesSegundoSemestre2020 = resultadoSuma.PagoMoratorios;
            reporteRegiones.PagoTotalDespuesSegundoSemestre2020 = resultadoSuma.PagoTotal;

            resultado.Add(reporteRegiones);
        }

        CreditosLiquidadosRegiones registroTotales = resultado.Aggregate(
                new CreditosLiquidadosRegiones(), (total, colocacion) =>
                {
                    total.Region = 0;
                    total.CatRegion = "Totales";
                    total.TotalClientes += colocacion.TotalClientes;
                    total.CantidadAnterioresJul2020 += colocacion.CantidadAnterioresJul2020;
                    total.CantidadSegundoSemestre2020 += colocacion.CantidadSegundoSemestre2020;
                    total.Cantidad2021 += colocacion.Cantidad2021;
                    total.Cantidad2022 += colocacion.Cantidad2022;
                    total.Cantidad2023 += colocacion.Cantidad2023;
                    total.TotalCreditos += colocacion.TotalCreditos;
                    total.CantidadReestructura += colocacion.CantidadReestructura;
                    total.CantidadCancelaciones += colocacion.CantidadCancelaciones;
                    total.CantidadLiquidados += colocacion.CantidadLiquidados;
                    total.PagoCapitalAntesPrimerSemestre2020 += colocacion.PagoCapitalAntesPrimerSemestre2020;
                    total.PagoInteresAntesPrimerSemestre2020 += colocacion.PagoInteresAntesPrimerSemestre2020;
                    total.PagoMoratoriosAntesPrimerSemestre2020 += colocacion.PagoMoratoriosAntesPrimerSemestre2020;
                    total.PagoTotalAntesPrimerSemestre2020 += colocacion.PagoTotalAntesPrimerSemestre2020;


                    total.PagoCapitalDespuesSegundoSemestre2020 += colocacion.PagoCapitalDespuesSegundoSemestre2020;
                    total.PagoInteresDespuesSegundoSemestre2020 += colocacion.PagoInteresDespuesSegundoSemestre2020;
                    total.PagoMoratoriosDespuesSegundoSemestre2020 += colocacion.PagoMoratoriosDespuesSegundoSemestre2020;
                    total.PagoTotalDespuesSegundoSemestre2020 += colocacion.PagoTotalDespuesSegundoSemestre2020;

                    return total;
                }
            );


        resultado.Add(registroTotales);
        return resultado.ToList();
    }

    private class SoloTotales
    {
        public decimal PagoCapital { get; set; }
        public decimal PagoInteres { get; set; }
        public decimal PagoMoratorios { get; set; }
        public decimal PagoTotal { get; set; }
    }

    public IEnumerable<CreditosLiquidadosAgencias> CalculaCreditosLiquidadosRegion(int region)
    {
        if (_creditosLiquidados is null)
            return new List<CreditosLiquidadosAgencias>();
        IList<CreditosLiquidadosAgencias> resultado = new List<CreditosLiquidadosAgencias>();
        IEnumerable<CreditosLiquidadosAgencias> resultadoAgrupado = _creditosLiquidados.Where(x => (Convert.ToInt32(x.Agencia.ToString()[..1]) * 100) == region).GroupBy(x => x.Agencia).Select(y => new CreditosLiquidadosAgencias()
        {
            Region = region,
            Agencia = y.Key,
            CatAgencia = y.First().CatAgenciaMigrado,
            CantidadAnterioresJul2020 = y.Count(z => z.FechaApertura < new DateTime(2020, 7, 1)),
            CantidadSegundoSemestre2020 = y.Count(z => z.FechaApertura >= new DateTime(2020, 7, 1) && z.FechaApertura <= new DateTime(2020, 12, 31)),
            Cantidad2021 = y.Count(z => z.FechaApertura >= new DateTime(2021, 1, 1) && z.FechaApertura <= new DateTime(2021, 12, 31)),
            Cantidad2022 = y.Count(z => z.FechaApertura >= new DateTime(2022, 1, 1) && z.FechaApertura <= new DateTime(2022, 12, 31)),
            Cantidad2023 = y.Count(z => z.FechaApertura >= new DateTime(2023, 1, 1) && z.FechaApertura <= new DateTime(2023, 12, 31)),
            TotalCreditos = y.Count(),
            CantidadReestructura = y.Count(z => (z.Reestructura ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)),
            CantidadCancelaciones = y.Count(z => (z.Cancelacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)),
            CantidadLiquidados = y.Count(z => !((z.Cancelacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase))
                                        && !((z.Reestructura ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)))
        });

        foreach (var reporteRegiones in resultadoAgrupado)
        {
            var datosRegion = _creditosLiquidados.
                Where(x => x.Agencia == reporteRegiones.Agencia);
            reporteRegiones.TotalClientes = datosRegion.GroupBy(x => x.NumCte).Count();

            var datosParcialesAnteriores = datosRegion.
                Where(x => (x.Agencia == reporteRegiones.Agencia) && (x.FechaApertura < new DateTime(2020, 7, 1)));

            SoloTotales resultadoSuma = datosParcialesAnteriores.Aggregate(
                new SoloTotales(), (total, colocacion) =>
                {
                    total.PagoCapital += colocacion.PagoCapital;
                    total.PagoInteres += colocacion.PagoInteres;
                    total.PagoMoratorios += colocacion.PagoMoratorios;
                    total.PagoTotal += colocacion.PagoTotal;
                    return total;
                }
            );

            reporteRegiones.PagoCapitalAntesPrimerSemestre2020 = resultadoSuma.PagoCapital;
            reporteRegiones.PagoInteresAntesPrimerSemestre2020 = resultadoSuma.PagoInteres;
            reporteRegiones.PagoMoratoriosAntesPrimerSemestre2020 = resultadoSuma.PagoMoratorios;
            reporteRegiones.PagoTotalAntesPrimerSemestre2020 = resultadoSuma.PagoTotal;

            var datosParcialesActuales = datosRegion.
                Where(x => x.Agencia == reporteRegiones.Agencia && (x.FechaApertura >= new DateTime(2020, 7, 1)));

            resultadoSuma = datosParcialesActuales.Aggregate(
                new SoloTotales(), (total, colocacion) =>
                {
                    total.PagoCapital += colocacion.PagoCapital;
                    total.PagoInteres += colocacion.PagoInteres;
                    total.PagoMoratorios += colocacion.PagoMoratorios;
                    total.PagoTotal += colocacion.PagoTotal;
                    return total;
                }
            );

            reporteRegiones.PagoCapitalDespuesSegundoSemestre2020 = resultadoSuma.PagoCapital;
            reporteRegiones.PagoInteresDespuesSegundoSemestre2020 = resultadoSuma.PagoInteres;
            reporteRegiones.PagoMoratoriosDespuesSegundoSemestre2020 = resultadoSuma.PagoMoratorios;
            reporteRegiones.PagoTotalDespuesSegundoSemestre2020 = resultadoSuma.PagoTotal;

            resultado.Add(reporteRegiones);
        }

        CreditosLiquidadosAgencias registroTotales = resultado.Aggregate(
                new CreditosLiquidadosAgencias(), (total, colocacion) =>
                {
                    total.Region = 0;
                    total.Agencia = 0;
                    total.CatAgencia = "Totales";
                    total.TotalClientes += colocacion.TotalClientes;
                    total.CantidadAnterioresJul2020 += colocacion.CantidadAnterioresJul2020;
                    total.CantidadSegundoSemestre2020 += colocacion.CantidadSegundoSemestre2020;
                    total.Cantidad2021 += colocacion.Cantidad2021;
                    total.Cantidad2022 += colocacion.Cantidad2022;
                    total.Cantidad2023 += colocacion.Cantidad2023;
                    total.TotalCreditos += colocacion.TotalCreditos;
                    total.CantidadReestructura += colocacion.CantidadReestructura;
                    total.CantidadCancelaciones += colocacion.CantidadCancelaciones;
                    total.CantidadLiquidados += colocacion.CantidadLiquidados;
                    total.PagoCapitalAntesPrimerSemestre2020 += colocacion.PagoCapitalAntesPrimerSemestre2020;
                    total.PagoInteresAntesPrimerSemestre2020 += colocacion.PagoInteresAntesPrimerSemestre2020;
                    total.PagoMoratoriosAntesPrimerSemestre2020 += colocacion.PagoMoratoriosAntesPrimerSemestre2020;
                    total.PagoTotalAntesPrimerSemestre2020 += colocacion.PagoTotalAntesPrimerSemestre2020;


                    total.PagoCapitalDespuesSegundoSemestre2020 += colocacion.PagoCapitalDespuesSegundoSemestre2020;
                    total.PagoInteresDespuesSegundoSemestre2020 += colocacion.PagoInteresDespuesSegundoSemestre2020;
                    total.PagoMoratoriosDespuesSegundoSemestre2020 += colocacion.PagoMoratoriosDespuesSegundoSemestre2020;
                    total.PagoTotalDespuesSegundoSemestre2020 += colocacion.PagoTotalDespuesSegundoSemestre2020;

                    return total;
                }
            );

        resultado.Add(registroTotales);
        return resultado.ToList();
    }

    public IEnumerable<CreditosLiquidadosDetalle> CalculaCreditosLiquidadosAgencia(int agencia)
    {
        if (_creditosLiquidados is null)
            return new List<CreditosLiquidadosDetalle>();
        IList<CreditosLiquidadosDetalle> resultado = new List<CreditosLiquidadosDetalle>();
        IEnumerable<CreditosLiquidadosDetalle> reporteAgencias = _creditosLiquidados.Where(alfa => alfa.Agencia == agencia).Select(y => new CreditosLiquidadosDetalle()
        {
            Region = Convert.ToInt32(y.Agencia.ToString()[..1]) * 100,
            Agencia = y.Agencia,
            NumCredito = y.NumCredito,
            NumCte = y.NumCte,
            Acreditado = y.Acreditado,
            FechaApertura = y.FechaApertura,
            FechaVencim = y.FechaVencim,
            MontoOtorgado = y.MontoOtorgado,
            Ministraciones = y.Ministraciones,
            FecPrimMinistra = y.FecPrimMinistra,
            FecUltimaMinistra = y.FecUltimaMinistra,
            MontoMinistrado = y.MontoMinistrado,
            Reestructura = y.Reestructura,
            Cancelacion = y.Cancelacion,
            PagoCapital = y.PagoCapital,
            PagoInteres = y.PagoInteres,
            PagoMoratorios = y.PagoMoratorios,
            PagoTotal = y.PagoTotal,
            TieneImagenDirecta = y.TieneImagenDirecta,
            TieneImagenIndirecta = y.TieneImagenIndirecta
        });

        var registroTotales = new CreditosLiquidadosDetalle()
        {
            Region = 0,
            Agencia = 0,
            NumCredito = "",
            NumCte = String.Format("{0:#,##0}", reporteAgencias.GroupBy(x => x.NumCte).Count()),
            Acreditado = String.Format("Total créditos {0:#,##0}", reporteAgencias.Count()),
            FechaApertura = null,
            FechaVencim = null,
            MontoOtorgado = reporteAgencias.Sum(x => x.MontoOtorgado),
            Ministraciones = Convert.ToInt32(reporteAgencias.Average(x => x.Ministraciones)),
            FecPrimMinistra = null,
            FecUltimaMinistra = null,
            MontoMinistrado = reporteAgencias.Sum(x => x.MontoMinistrado),
            Reestructura = String.Format("{0:#,##0}", reporteAgencias.Count(x => (x.Reestructura ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase))),
            Cancelacion = String.Format("{0:#,##0}", reporteAgencias.Count(x => (x.Cancelacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase))),
            PagoCapital = reporteAgencias.Sum(x => x.PagoCapital),
            PagoInteres = reporteAgencias.Sum(x => x.PagoInteres),
            PagoMoratorios = reporteAgencias.Sum(x => x.PagoMoratorios),
            PagoTotal = reporteAgencias.Sum(x => x.PagoTotal),
            TieneImagenDirecta = false,
            TieneImagenIndirecta = false
        };
        ((List<CreditosLiquidadosDetalle>)resultado).AddRange(reporteAgencias);
        resultado.Add(registroTotales);
        return resultado.ToList();
    }


    /// <summary>
    /// Evita que se duerma la unidad de las imagenes
    /// </summary>
    public void CreaTemporal()
    {
        if (_tieneErrorTemporal)
        {
            return;
        }
        string temporalDir = Path.Combine(_driveDestino, "Temp");
        try
        {
            if (!Directory.Exists(temporalDir))
            {
                Directory.CreateDirectory(temporalDir);
            }
        }
        catch
        {
            _tieneErrorTemporal = true;
        }
        if (!string.IsNullOrEmpty(_ultimoArchivo))
        {
            if (File.Exists(_ultimoArchivo))
            {
                try
                {
                    File.Delete(_ultimoArchivo);
                }
                catch { }
            }
        }
        _ultimoArchivo = Path.Combine(temporalDir, Guid.NewGuid().ToString());
        if (!File.Exists(_ultimoArchivo))
        {
            // Abre el archivo en modo de escritura (si no existe, lo crea)
#pragma warning disable IDE0063 // Use la instrucción "using" simple
            try
            {
                using (StreamWriter archivo = new(_ultimoArchivo))
                {
                    // Escribe el texto en el archivo
                    archivo.WriteLine(".");
                }
            }
            catch
            {

            }
#pragma warning restore IDE0063 // Use la instrucción "using" simple
        }
    }

    #region Tratamientos
    public int CalculaTratamientos()
    {
        if (_expedienteDeConsulta is not null)
        {
            IList<ResumenDeCreditos> resultado = new List<ResumenDeCreditos>();
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar == true && (x.NumCredito ?? "").Substring(3, 1).Equals("8")).ToList();
            return creditosAReportar.Count;
        }
        return 0;
    }
    public IEnumerable<ResumenDeCreditos> CalculaCreditosRegionesTratamientos()
    {
        if (_expedienteDeConsulta is not null)
        {
            IList<ResumenDeCreditos> resultado = new List<ResumenDeCreditos>();
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar == true && (x.NumCredito ?? "").Substring(3, 1).Equals("8")).ToList();
            var grupoPorRegion = creditosAReportar.GroupBy(x => x.Region).Select(y => new ResumenDeCreditos()
            {
                Region = y.First().Region,
                CatRegion = y.First().CatRegion,
                CantidadDeCreditosVigente = y.Count(z => z.StatusCarteraVigente == true),
                CantidadDeCreditosImpago = y.Count(z => z.StatusImpago == true),
                CantidadDeCreditosVencida = y.Count(z => z.StatusCarteraVencida == true),
                TotalDeCreditos = y.Count()
            });
            foreach (var reporteRegiones in grupoPorRegion)
            {
                var datosRegion = creditosAReportar.Where(x => x.Region == reporteRegiones.Region);
                reporteRegiones.CantidadClientesImpago = datosRegion.Where(x => x.StatusImpago).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.CantidadClientesVencida = datosRegion.Where(x => x.StatusCarteraVencida).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.CantidadClientesVigente = datosRegion.Where(x => x.StatusCarteraVigente).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.TotalDeClientes = datosRegion.GroupBy(x => x.NumCliente).Count();
                reporteRegiones.SaldosCarteraImpago = datosRegion.Where(x => x.StatusImpago).Sum(z => z.SldoTotContval);
                reporteRegiones.SaldosCarteraVencida = datosRegion.Where(x => x.StatusCarteraVencida).Sum(z => z.SldoTotContval);
                reporteRegiones.SaldosCarteraVigente = datosRegion.Where(x => x.StatusCarteraVigente).Sum(z => z.SldoTotContval);
                reporteRegiones.SaldosTotal = datosRegion.Sum(z => z.SldoTotContval);
                reporteRegiones.ICV = (reporteRegiones.SaldosCarteraVencida / reporteRegiones.SaldosTotal) * 100;
                resultado.Add(reporteRegiones);
            }


            var registroTotales = new ResumenDeCreditos()
            {
                Region = 0,
                CatRegion = "Totales",
                CantidadDeCreditosVigente = resultado.Sum(x => x.CantidadDeCreditosVigente),
                CantidadDeCreditosImpago = resultado.Sum(x => x.CantidadDeCreditosImpago),
                CantidadDeCreditosVencida = resultado.Sum(x => x.CantidadDeCreditosVencida),
                TotalDeCreditos = resultado.Sum(x => x.TotalDeCreditos),
                CantidadClientesImpago = resultado.Sum(x => x.CantidadClientesImpago),
                CantidadClientesVencida = resultado.Sum(x => x.CantidadClientesVencida),
                CantidadClientesVigente = resultado.Sum(x => x.CantidadClientesVigente),
                TotalDeClientes = resultado.Sum(x => x.TotalDeClientes),
                SaldosCarteraImpago = resultado.Sum(x => x.SaldosCarteraImpago),
                SaldosCarteraVencida = resultado.Sum(x => x.SaldosCarteraVencida),
                SaldosCarteraVigente = resultado.Sum(x => x.SaldosCarteraVigente),
                SaldosTotal = resultado.Sum(x => x.SaldosTotal)
            };

            if (registroTotales.SaldosTotal == 0)
            {
                registroTotales.ICV = 100;
            }
            else
            {
                registroTotales.ICV = (registroTotales.SaldosCarteraVencida / registroTotales.SaldosTotal) * 100;
            }

            resultado.Add(registroTotales);


            return resultado.ToList();
        }

        return new List<ResumenDeCreditos>();
    }

    public IEnumerable<CreditosRegion> CalculaCreditosRegionTratamientos(int region)
    {
        if (_expedienteDeConsulta is not null && region != 0)
        {
            IList<CreditosRegion> resultado = new List<CreditosRegion>();
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar && x.Region == region && (x.NumCredito ?? "").Substring(3, 1).Equals("8")).ToList();
            var grupoPorAgencia = creditosAReportar.GroupBy(x => x.Agencia).Select(y => new CreditosRegion()
            {
                Agencia = y.First().Agencia,
                CatAgencia = y.First().CatAgencia,
                CantidadDeCreditosVigente = y.Count(z => z.StatusCarteraVigente == true),
                CantidadDeCreditosImpago = y.Count(z => z.StatusImpago == true),
                CantidadDeCreditosVencida = y.Count(z => z.StatusCarteraVencida == true),
                TotalDeCreditos = y.Count()
            });
            foreach (var reporteAgencia in grupoPorAgencia)
            {
                var datosAgencia = creditosAReportar.Where(x => x.Agencia == reporteAgencia.Agencia);
                reporteAgencia.CantidadClientesImpago = datosAgencia.Where(x => x.StatusImpago).GroupBy(x => x.NumCliente).Count();
                reporteAgencia.CantidadClientesVencida = datosAgencia.Where(x => x.StatusCarteraVencida).GroupBy(x => x.NumCliente).Count();
                reporteAgencia.CantidadClientesVigente = datosAgencia.Where(x => x.StatusCarteraVigente).GroupBy(x => x.NumCliente).Count();
                reporteAgencia.TotalDeClientes = datosAgencia.GroupBy(x => x.NumCliente).Count();
                reporteAgencia.SaldosCarteraImpago = datosAgencia.Where(x => x.StatusImpago).Sum(z => z.SldoTotContval);
                reporteAgencia.SaldosCarteraVencida = datosAgencia.Where(x => x.StatusCarteraVencida).Sum(z => z.SldoTotContval);
                reporteAgencia.SaldosCarteraVigente = datosAgencia.Where(x => x.StatusCarteraVigente).Sum(z => z.SldoTotContval);
                reporteAgencia.SaldosTotal = datosAgencia.Sum(z => z.SldoTotContval);
                if (reporteAgencia.SaldosTotal != 0)
                {
                    reporteAgencia.ICV = (reporteAgencia.SaldosCarteraVencida / reporteAgencia.SaldosTotal) * 100;
                }
                else
                    reporteAgencia.ICV = 100;
                resultado.Add(reporteAgencia);
            }


            var registroTotales = new CreditosRegion()
            {
                Agencia = 0,
                CatAgencia = "Totales",
                CantidadDeCreditosVigente = resultado.Sum(x => x.CantidadDeCreditosVigente),
                CantidadDeCreditosImpago = resultado.Sum(x => x.CantidadDeCreditosImpago),
                CantidadDeCreditosVencida = resultado.Sum(x => x.CantidadDeCreditosVencida),
                TotalDeCreditos = resultado.Sum(x => x.TotalDeCreditos),
                CantidadClientesImpago = resultado.Sum(x => x.CantidadClientesImpago),
                CantidadClientesVencida = resultado.Sum(x => x.CantidadClientesVencida),
                CantidadClientesVigente = resultado.Sum(x => x.CantidadClientesVigente),
                TotalDeClientes = resultado.Sum(x => x.TotalDeClientes),
                SaldosCarteraImpago = resultado.Sum(x => x.SaldosCarteraImpago),
                SaldosCarteraVencida = resultado.Sum(x => x.SaldosCarteraVencida),
                SaldosCarteraVigente = resultado.Sum(x => x.SaldosCarteraVigente),
                SaldosTotal = resultado.Sum(x => x.SaldosTotal)
            };
            if (registroTotales.SaldosTotal != 0)
            {
                registroTotales.ICV = (registroTotales.SaldosCarteraVencida / registroTotales.SaldosTotal) * 100;
            }
            else
            {
                registroTotales.ICV = 100;
            }
            resultado.Add(registroTotales);
            return resultado.ToList();

        }

        return new List<CreditosRegion>();
    }

    private string ObtieneListaCreditos(string numCreditoTratamiento)
    {
        IList<string> listaCreditos = _operacionesTratamientosService.ObtieneTratamientosOrigen(numCreditoTratamiento).ToList();
        bool tieneSeparador = false;
        StringBuilder sb = new();
        foreach (var credito in listaCreditos)
        {
            if (tieneSeparador)
            {
                sb.Append("\n\r");
            }
            sb.Append(credito);
            tieneSeparador = true;
        }
        return sb.ToString();
    }

    public IEnumerable<DetalleCreditosTratamientos> CalculaCreditosAgenciaTratamientos(int agencia)
    {
        // 
        if (_expedienteDeConsulta is not null && agencia != 0)
        {
            var detalleAgencia = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar && x.Agencia == agencia && (x.NumCredito ?? "").Substring(3, 1).Equals("8")).ToList();

            var resultadoDetalleCreditos = detalleAgencia.Select(x => new DetalleCreditosTratamientos()
            {
                Region = x.Region,
                Agencia = x.Agencia,
                NumCredito = x.NumCredito,
                Acreditado = x.Acreditado,
                FechaApertura = x.FechaApertura,
                SaldoCarteraVigente = (x.StatusCarteraVigente == true) ? x.SldoTotContval : 0M,
                SaldoCarteraImpagos = (x.StatusImpago == true) ? x.SldoTotContval : 0M,
                SaldosCarteraVencida = (x.StatusCarteraVencida == true) ? x.SldoTotContval : 0M,
                SaldosTotal = x.SldoTotContval,
                EsVigente = x.StatusCarteraVigente,
                EsImpago = x.StatusImpago,
                EsVencida = x.StatusCarteraVencida,
                EsOrigenDelDr = x.EsOrigenDelDr,
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta,
                CreditoOrigen = ObtieneListaCreditos(x.NumCredito ?? "")
            }
            ).ToList();
            int totalCred = resultadoDetalleCreditos.Count;
            int totalCtes = resultadoDetalleCreditos.GroupBy(x => x.Acreditado).Count();
            decimal totalSaldoCarteraVigente = resultadoDetalleCreditos.Sum(x => x.SaldoCarteraVigente);
            decimal totalSaldoCarteraImpagos = resultadoDetalleCreditos.Sum(x => x.SaldoCarteraImpagos);
            decimal totalSaldosCarteraVencida = resultadoDetalleCreditos.Sum(x => x.SaldosCarteraVencida);
            decimal totalSaldosTotal = resultadoDetalleCreditos.Sum(x => x.SaldosTotal);

            DetalleCreditosTratamientos total = new()
            {
                Region = 0,
                Agencia = 0,
                NumCredito = "",
                Acreditado = string.Format("Total Cred {0:#,###} Total Ctes {1:#,###}", totalCred, totalCtes),
                SaldoCarteraVigente = totalSaldoCarteraVigente,
                SaldosCarteraVencida = totalSaldosCarteraVencida,
                SaldoCarteraImpagos = totalSaldoCarteraImpagos,
                SaldosTotal = totalSaldosTotal
            };
            resultadoDetalleCreditos.Add(total);
            return resultadoDetalleCreditos.ToList();
        }
        return new List<DetalleCreditosTratamientos>();
    }

    public IEnumerable<string> ObtieneCreditoOrigen(string numeroDeCreditoTratamiento)
    {
        IList<string> listaCreditos = _operacionesTratamientosService.ObtieneTratamientosOrigen(numeroDeCreditoTratamiento).ToList();
        return listaCreditos;
    }
    #endregion


    #region Expedientes Jurídicos
    public IEnumerable<ExpedienteJuridico> CargaExpedientesJuridicos()
    {
        IEnumerable<ExpedienteJuridico> resultado;
        FileInfo fi = new(_archivoExpedienteJuridicoCsv);
        if (fi.Exists)
        {
            resultado = _administraExpedientesJuridicosService.CargaExpedientesJuridicos(_archivoExpedienteJuridicoCsv);
            foreach (var elemento in resultado)
            {
                elemento.NumCreditos = (elemento.NumCreditos ?? "").Replace("¬", "\n");
                elemento.JuzgadoUbicacion = (elemento.JuzgadoUbicacion ?? "").Replace("¬", "\n");
                elemento.Observaciones = (elemento.Observaciones ?? "").Replace("¬", "\n");
            }

            _expedientesJuridicos = resultado;
            return _expedientesJuridicos;
        }

        var expedientesJuridicos = _juridicoService.GetExpedientesJuridicos(_archivoExpedienteJuridico);
        resultado = expedientesJuridicos.Select(x=>new ExpedienteJuridico() { 
            Region = ObtieneRegionJuridico(x.CatRegion??""),
            CatRegion = (x.CatRegion ?? "").Replace("\n", "").Replace("|",""),
            Agencia = ObtieneAgenciaJuridico(ObtieneRegionJuridico(x.CatRegion ?? ""), x.Estado??""),
            CatAgencia = (x.Estado ?? "").Replace("\n", "").Replace("|", ""),
            Estado = (x.CatAgencia ?? "").Replace("\n", "").Replace("|", ""),
            NombreDemandado = (x.NombreDemandado ?? "").Replace("\n", "").Replace("|", ""),
            TipoCredito = (x.TipoCredito ?? "").Replace("\n", "").Replace("|", ""),
            NumContrato = (x.NumContrato??"").Replace("\n","").Replace("|", ""),
            NumCte = (x.NumCte ?? "").Replace("\n", "").Replace("|", ""),
            NumCredito = (x.NumCredito ?? "").Replace("\n", "").Replace("|", ""),
            NumCreditos = (x.NumCreditos??"").Trim().Replace("\n", "¬").Replace(";", "¬").Replace(" ", "¬").Replace("|", "").Replace("¬¬", "¬").Replace("¬¬", "¬"),
            FechaTranspasoJuridico = x.FechaTranspasoJuridico,
            FechaTranspasoExterno = x.FechaTranspasoExterno,
            FechaDemanda = x.FechaDemanda,
            Juicio = (x.Juicio ?? "").Replace("\n", "").Replace("|", ""),
            CapitalDemandado = x.CapitalDemandado,
            Dlls = x.Dlls,
            JuzgadoUbicacion = (x.JuzgadoUbicacion ?? "").Replace("\n", "¬").Replace("|", ""),
            Expediente = (x.Expediente ?? "").Replace("\n", "").Replace("|", ""),
            ClaveProcesal = (x.ClaveProcesal ?? "").Replace("\n", "").Replace("|", ""),
            EtapaProcesal = (x.EtapaProcesal ?? "").Replace("\n", "").Replace("|", ""),
            Rppc = (x.Rppc ?? "").Replace("\n", "").Replace("|", ""),
            Ran = (x.Ran ?? "").Replace("\n", "").Replace("|", ""),
            RedCredAgricola = (x.RedCredAgricola ?? "").Replace("\n", "").Replace("|", ""),
            FechaRegistro = x.FechaRegistro,
            Tipo = (x.Tipo ?? "").Replace("\n", "").Replace("|", ""),
            TipoGarantias = (x.TipoGarantias ?? "").Replace("\n", "").Replace("|", ""),
            Descripcion = (x.Descripcion ?? "").Replace("\n", "¬").Replace("|", ""),
            ValorRegistro = x.ValorRegistro,
            FechaAvaluo = x.FechaAvaluo,
            GradoPrelacion = (x.GradoPrelacion ?? "").Replace("\n", "").Replace("|", ""),
            CAval = (x.CAval ?? "").Replace("\n", "").Replace("|", ""),
            SAval = (x.SAval ?? "").Replace("\n", "").Replace("|", ""),
            Inscripcion = (x.Inscripcion ?? "").Replace("\n", "").Replace("|", ""),
            Clave = (x.Clave ?? "").Replace("\n", "").Replace("|", ""),
            NumFolio = (x.NumFolio ?? "").Replace("\n", "").Replace("|", ""),
            AbogadoResponsable = (x.AbogadoResponsable ?? "").Replace("\n", "").Replace("|", ""),
            Observaciones = (x.Observaciones ?? "").Replace("\n", "¬").Replace("|", ""),
            FechaUltimaActuacion = x.FechaUltimaActuacion,
            DescripcionActuacion = (x.DescripcionActuacion ?? "").Replace("\n", "¬").Replace("|", ""),
            Piso = (x.Piso ?? "").Replace("\n", "").Replace("|", ""),
            Fonaga = (x.Fonaga ?? "").Replace("\n", "").Replace("|", ""),
            PequenioProductor = (x.PequenioProductor ?? "").Replace("\n", "").Replace("|", ""),
            FondoMutual = (x.FondoMutual ?? "").Replace("\n", "").Replace("|", ""),
            ExpectativasRecuperacion = (x.ExpectativasRecuperacion ?? "").Replace("\n", "").Replace("|", ""),
            TieneImagenDirecta = x.TieneImagenDirecta,
            TieneImagenIndirecta = x.TieneImagenIndirecta,
            TieneImagenExpediente = x.TieneImagenExpediente
        }).ToList();
        _expedientesJuridicos = resultado;
        // Aqui se cruza con imágenes
        
        if (_imagenesCortasExpedientesJuridico is not null)
        {
            CruzaInformacionImagenesExpedientesJuridicos();  // _imagenesCortasExpedientesJuridico
        }
        if (_imagenesCortas is not null) 
        {
            CruzaInformacionImagenesConExpedientesJuridicosExpedientes(_imagenesCortas);
        }

        _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedienteJuridicoCsv, resultado);
        foreach (var elemento in resultado)
        { 
            elemento.NumCreditos = (elemento.NumCreditos??"").Replace("¬", "\n");
            elemento.JuzgadoUbicacion = (elemento.JuzgadoUbicacion??"").Replace("¬", "\n");
            elemento.Observaciones = (elemento.Observaciones ?? "").Replace("¬", "\n");
        }
        return resultado;
    }

    
    private static int ObtieneRegionJuridico(string catRegion)
    {
        if (catRegion.Equals("CENTRO OCCIDENTE", StringComparison.InvariantCultureIgnoreCase))
            return 100;
        if (catRegion.Equals("NORTE", StringComparison.InvariantCultureIgnoreCase))
            return 300;
        if (catRegion.Equals("SUR", StringComparison.InvariantCultureIgnoreCase))
            return 500;
        if (catRegion.Equals("SURESTE", StringComparison.InvariantCultureIgnoreCase))
            return 600;
        return 200;
//        = SI(C2 = "CENTRO OCCIDENTE", 100, SI(C2 = "NORTE", 300, SI(C2 = "SUR", 500, SI(C2 = "SURESTE", "600", "200"))))
    }

    private int ObtieneAgenciaJuridico(int region, string CatAgencia)
    {
        if (CatAgencia.Contains("Ciudad", StringComparison.InvariantCultureIgnoreCase))
            CatAgencia = CatAgencia.Replace("Ciudad", "", StringComparison.InvariantCultureIgnoreCase);
        if (CatAgencia.Contains("CD.", StringComparison.InvariantCultureIgnoreCase))
            CatAgencia = CatAgencia.Replace("CD.", "", StringComparison.InvariantCultureIgnoreCase);
        if (CatAgencia.Contains("CD ", StringComparison.InvariantCultureIgnoreCase))
            CatAgencia = CatAgencia.Replace("CD ", "", StringComparison.InvariantCultureIgnoreCase);
        if (CatAgencia.Contains("Autlán de Navarro", StringComparison.InvariantCultureIgnoreCase))
            CatAgencia = CatAgencia.Replace("Autlán de Navarro", "Autlán", StringComparison.InvariantCultureIgnoreCase);

        _agencias ??= _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();
        var agencia = _agencias.FirstOrDefault(x => (Convert.ToInt32(Convert.ToString(x.NoAgencia)[..1]) * 100) == region && RemoverAcentos(x.Agencia).Contains(RemoverAcentos(CatAgencia), StringComparison.InvariantCultureIgnoreCase));
        if (agencia is not null)
            return agencia.NoAgencia;
        else
        {
            if (CatAgencia.Contains("Nuevo Casas Grandes", StringComparison.InvariantCultureIgnoreCase))
                return 306;
            if (CatAgencia.Contains("Minatitlan", StringComparison.InvariantCultureIgnoreCase))
                return 520; // San Andres Tuxtla
            return 0;
        }
    
    }

    static string RemoverAcentos(string? texto)
    {
        if (texto is null)
            return "";
        texto = texto.Trim();
        string normalizedString = texto.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new ();

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                if (c == 'ñ' || c == 'Ñ')
                {
                    stringBuilder.Append('n');
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public int CalculaExpedientesJuridicos()
    {
        if (_expedientesJuridicos is not null)
            return _expedientesJuridicos.Count();
        return 0;
    }
    public IEnumerable<ExpedientesJuridicoRegion> CalculaExpedientesJuridicoRegiones()
    {
        if (_expedientesJuridicos is null)
            return new List<ExpedientesJuridicoRegion>();
        IList<ExpedientesJuridicoRegion> resultado = new List<ExpedientesJuridicoRegion>();

        IEnumerable<ExpedientesJuridicoRegion> resultadoAgrupado = _expedientesJuridicos.GroupBy(x => x.Region).Select(y => new ExpedientesJuridicoRegion()
        {
            Region = y.Key,
            CatRegion = y.First().CatRegion,
            CantDesfavorable = y.Count(z => (z.ExpectativasRecuperacion ?? "").Equals("DESFAVORABLE", StringComparison.InvariantCultureIgnoreCase)),
            CantFavorable = y.Count(z => (z.ExpectativasRecuperacion ?? "").Equals("FAVORABLE", StringComparison.InvariantCultureIgnoreCase)),
            CantNoFavorable = y.Count(z => (z.ExpectativasRecuperacion ?? "").Equals("NO FAVORABLE", StringComparison.InvariantCultureIgnoreCase)),
            CantReservado = y.Count(z => (z.ExpectativasRecuperacion ?? "").Equals("RESERVADO", StringComparison.InvariantCultureIgnoreCase)),

            TotalCapitalDemandado = y.Sum(z => z.CapitalDemandado),
            TotalCapitalDesfavorable = y.Where(x=>(x.ExpectativasRecuperacion ?? "").Equals("DESFAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z=>z.CapitalDemandado),
            TotalCapitalFavorable = y.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado),
            TotalCapitalNoFavorable = y.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("NO FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado),
            TotalCapitalReservado = y.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("RESERVADO", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado)
        });

        foreach (var reporteRegiones in resultadoAgrupado)
        {
            var datosRegion = _expedientesJuridicos.
                Where(x => x.Region == reporteRegiones.Region);
            reporteRegiones.CantContratos = datosRegion.GroupBy(x=>x.NumContrato).Count();
            reporteRegiones.CantClientes = datosRegion.GroupBy(x => x.NumCte).Count();
            reporteRegiones.CantCreditosHomologados = datosRegion.GroupBy(x => x.NumCredito).Count();

            resultado.Add(reporteRegiones);
        }

        ExpedientesJuridicoRegion registroTotales = resultado.Aggregate(
                new ExpedientesJuridicoRegion(), (total, colocacion) =>
                {
                    total.Region = 0;
                    total.CatRegion = "Totales";
                    total.CantContratos += colocacion.CantContratos;
                    total.CantClientes  += colocacion.CantClientes;
                    total.CantCreditosHomologados += colocacion.CantCreditosHomologados;

                    total.CantDesfavorable += colocacion.CantDesfavorable;
                    total.CantFavorable += colocacion.CantFavorable;
                    total.CantNoFavorable += colocacion.CantNoFavorable;
                    total.CantReservado += colocacion.CantReservado;
                    total.TotalCapitalDemandado += colocacion.TotalCapitalDemandado;
                    total.TotalCapitalDesfavorable += colocacion.TotalCapitalDesfavorable;
                    total.TotalCapitalFavorable += colocacion.TotalCapitalFavorable;
                    total.TotalCapitalNoFavorable += colocacion.TotalCapitalNoFavorable;
                    total.TotalCapitalReservado += colocacion.TotalCapitalReservado;

                    return total;
                }
            );

        resultado.Add(registroTotales);
        return resultado.ToList();
    }
    public IEnumerable<ExpedientesJuridicoAgencia> CalculaExpedientesJuridicoAgencia(int region)
    {
        if (_expedientesJuridicos is null)
            return new List<ExpedientesJuridicoAgencia>();
        IList<ExpedientesJuridicoAgencia> resultado = new List<ExpedientesJuridicoAgencia>();
        IEnumerable<ExpedientesJuridicoAgencia> resultadoAgrupado = _expedientesJuridicos.Where(x => x.Region == region).GroupBy(x => x.Agencia).Select(y => new ExpedientesJuridicoAgencia()
        {
            Region = region,
            Agencia = y.Key,
            CatAgencia = y.First().CatAgencia,

            TotalCapitalDemandado = y.Sum(z => z.CapitalDemandado),
        });

        foreach (var reporteRegiones in resultadoAgrupado)
        {
            var datosRegion = _expedientesJuridicos.
                Where(x => x.Agencia == reporteRegiones.Agencia && x.Region == region);
            reporteRegiones.CantContratos = datosRegion.GroupBy(x => x.NumContrato).Count();
            reporteRegiones.CantClientes = datosRegion.GroupBy(x => x.NumCte).Count();
            reporteRegiones.CantCreditosHomologados = datosRegion.GroupBy(x => x.NumCredito).Count();
            reporteRegiones.CantDesfavorable = datosRegion.Where(z => (z.ExpectativasRecuperacion ?? "").Equals("DESFAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Count();
            reporteRegiones.CantFavorable = datosRegion.Where(z => (z.ExpectativasRecuperacion ?? "").Equals("FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Count();
            reporteRegiones.CantNoFavorable = datosRegion.Where(z => (z.ExpectativasRecuperacion ?? "").Equals("NO FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Count();
            reporteRegiones.CantReservado = datosRegion.Where(z => (z.ExpectativasRecuperacion ?? "").Equals("RESERVADO", StringComparison.InvariantCultureIgnoreCase)).Count();
            reporteRegiones.TotalCapitalDesfavorable = datosRegion.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("DESFAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado);
            reporteRegiones.TotalCapitalFavorable = datosRegion.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado);
            reporteRegiones.TotalCapitalNoFavorable = datosRegion.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("NO FAVORABLE", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado);
            reporteRegiones.TotalCapitalReservado = datosRegion.Where(x => (x.ExpectativasRecuperacion ?? "").Equals("RESERVADO", StringComparison.InvariantCultureIgnoreCase)).Sum(z => z.CapitalDemandado);

            resultado.Add(reporteRegiones);
        }

        ExpedientesJuridicoAgencia registroTotales = resultado.Aggregate(
                new ExpedientesJuridicoAgencia(), (total, colocacion) =>
                {
                    total.Region = 0;
                    total.Agencia = 0;
                    total.CatAgencia = "Totales";


                    total.CantContratos += colocacion.CantContratos;
                    total.CantClientes += colocacion.CantClientes;
                    total.CantCreditosHomologados += colocacion.CantCreditosHomologados;
                    total.CantDesfavorable += colocacion.CantDesfavorable;
                    total.CantFavorable += colocacion.CantFavorable;
                    total.CantNoFavorable += colocacion.CantNoFavorable;
                    total.CantReservado += colocacion.CantReservado;
                    total.TotalCapitalDemandado += colocacion.TotalCapitalDemandado;
                    total.TotalCapitalDesfavorable += colocacion.TotalCapitalDesfavorable;
                    total.TotalCapitalFavorable += colocacion.TotalCapitalFavorable;
                    total.TotalCapitalNoFavorable += colocacion.TotalCapitalNoFavorable;
                    total.TotalCapitalReservado += colocacion.TotalCapitalReservado;

                    return total;
                }
            );

        resultado.Add(registroTotales);
        return resultado.ToList();
    }
    public IEnumerable<ExpedienteJuridicoDetalle> CalculaExpedienteJuridicoDetalle(int agencia)
    {
        if (_expedientesJuridicos is null)
            return new List<ExpedienteJuridicoDetalle>();
        IList<ExpedienteJuridicoDetalle> resultado = new List<ExpedienteJuridicoDetalle>();
        IEnumerable<ExpedienteJuridicoDetalle> reporteAgencias = _expedientesJuridicos.Where(alfa => alfa.Agencia == agencia).Select(y => new ExpedienteJuridicoDetalle()
        {
            Region = y.Region,
            Agencia = y.Agencia,
            Estado = y.Estado,
            NumContrato = y.NumContrato,
            NombreDemandado = y.NombreDemandado,
            NumCte = y.NumCte,
            TipoCredito = y.TipoCredito,
            NumCredito = y.NumCredito,
            NumCreditos = y.NumCreditos,
            FechaTranspasoJuridico = y.FechaTranspasoJuridico,
            FechaTranspasoExterno = y.FechaTranspasoExterno,
            FechaDemanda = y.FechaDemanda,
            Juicio = y.Juicio,
            TipoGarantias = y.TipoGarantias,
            CapitalDemandado = y.CapitalDemandado,
            Fonaga = y.Fonaga,
            PequenioProductor = y.PequenioProductor,
            FondoMutual = y.FondoMutual,
            Expediente = y.Expediente,
            AbogadoResponsable = y.AbogadoResponsable,
            ExpectativasRecuperacion = y.ExpectativasRecuperacion,
            TieneImagenIndirecta = y.TieneImagenIndirecta,
            TieneImagenDirecta = y.TieneImagenDirecta,
            TieneImagenExpediente = y.TieneImagenExpediente
        });

        var registroTotales = new ExpedienteJuridicoDetalle()
        {
            Region = 0,
            Agencia = 0,
            Estado = "",
            NumContrato = String.Format("{0:#,##0}", reporteAgencias.GroupBy(x => x.NumContrato).Count()),
            NombreDemandado = String.Format("Total créditos {0:#,##0}", reporteAgencias.Count()),
            NumCte = String.Format("{0:#,##0}", reporteAgencias.GroupBy(x => x.NumCte).Count()),
            TipoCredito = "",
            NumCredito = "",
            NumCreditos = "",
            FechaTranspasoJuridico = null,
            FechaTranspasoExterno = null,
            FechaDemanda = null,
            Juicio = "",
            TipoGarantias = "",
            CapitalDemandado = reporteAgencias.Sum(x => x.CapitalDemandado),
            Fonaga = "",
            PequenioProductor = "",
            FondoMutual = "",
            ExpectativasRecuperacion = "",
            TieneImagenDirecta = false,
            TieneImagenIndirecta = false,
            TieneImagenExpediente = false
        };
        ((List<ExpedienteJuridicoDetalle>)resultado).AddRange(reporteAgencias);
        resultado.Add(registroTotales);
        return resultado.ToList();
    }


    public IEnumerable<ArchivoImagenExpedientesCorta> BuscaImagenesExpedientes(string contrato, string numCredito)
    {
        // Se crea una nueva lista de objetos "ArchivoImagenExpedientesCorta"
        IList<ArchivoImagenExpedientesCorta> listaImagenesEncontradas = new List<ArchivoImagenExpedientesCorta>();
        // Se comprueba si la variable "_imagenesCortasExpedientesJuridico" no es nula,
        // tiene algún elemento y las variables "contrato" y
        // "numCredito" no están vacías.
        if (_imagenesCortasExpedientesJuridico is not null && _imagenesCortasExpedientesJuridico.Any() && !string.IsNullOrEmpty(contrato) && !string.IsNullOrEmpty(numCredito))
        {

            // Se filtran los elementos de "_imagenesCortasExpedientesJuridico" y se asignan a la variable "listaImagenesEncontradas".
            listaImagenesEncontradas = _imagenesCortasExpedientesJuridico.OrderByDescending(y => y.NumPaginas).Where(x => (contrato ?? "000000000000000").Equals((x.NumContrato+ "000000000000000" ?? "000000000000000")[..15], StringComparison.OrdinalIgnoreCase)
             && (numCredito ?? "000000000000000000").Equals(x.NumCredito, StringComparison.OrdinalIgnoreCase)            
            ).ToList();

            if (!listaImagenesEncontradas.Any())
            {
                listaImagenesEncontradas = _imagenesCortasExpedientesJuridico.OrderByDescending(y => y.NumPaginas).Where(x => (contrato ?? "").Equals((x.NumContrato + "000000000000000" ?? "")[..15], StringComparison.OrdinalIgnoreCase)                 
                ).ToList();
            }

            if (!listaImagenesEncontradas.Any())
            {
                listaImagenesEncontradas = _imagenesCortasExpedientesJuridico.OrderByDescending(y => y.NumPaginas).Where(x => (numCredito ?? "").Equals(x.NumCredito, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }


            // Si la variable "listaImagenesEncontradas" está vacía 
            // se buscan imágenes adicionales que coincidan parcialmente con el "contrato"
            // y se agregan a "listaImagenesEncontradas". Sirve para agregar cuando no hay coincidencias exactas.
            if (!listaImagenesEncontradas.Any())
            {
                listaImagenesEncontradas = _imagenesCortasExpedientesJuridico.OrderByDescending(y => y.NumPaginas).Where(x => (contrato ?? "").Equals(x.NumContrato, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
        // Se devuelve la variable "listaImagenesEncontradas" con las imágenes filtradas, sin duplicados y ordenadas.
        return listaImagenesEncontradas;
    }
    public IEnumerable<ArchivoImagenExpedientesCorta> CargaInformacionImagenesExpedientesJuridicos()
    {
        FileInfo fi = new(_archivosImagenesExpedientesJuridicoCorta);
        if (fi.Exists)
        {
            _imagenesCortasExpedientesJuridico = _administraCargaConsultaService.CargaArchivoImagenExpedientesCortas(_archivosImagenesExpedientesJuridicoCorta);
            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");

            CambiaUnidadExpedientesJuridico();

            return _imagenesCortasExpedientesJuridico;
        }
        // IList<ArchivoImagenCorta> lista = new List<ArchivoImagenCorta>();
        IEnumerable<ArchivosImagenes> resultado = _servicioImagenes.CargaImagenesTratadas(_archivosImagenesExpedientesJuridico).Where(x => (x.Extension ?? "").Equals(".pdf", StringComparison.InvariantCultureIgnoreCase));
        var resultadoCorto = resultado.Select(x => new ArchivoImagenExpedientesCorta()
        {
            Id = x.Id,
            NumContrato = x.NumContrato,
            NumCredito = x.NumCredito,
            NombreArchivo = x.NombreArchivo,
            CarpetaDestino = x.CarpetaDestino,
            NumPaginas = x.NumPaginas,
            Hash = x.Hash,
            EsTurno = (x.UsuarioDeModificacion??"").Equals("Si", StringComparison.InvariantCultureIgnoreCase),
            EsCobranza = (x.ClasifMesa ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)
        }).ToList();
        _ = _creaArchivoCsvService.CreaArchivoCsv(_archivosImagenesExpedientesJuridicoCorta, resultadoCorto);
        _imagenesCortasExpedientesJuridico = resultadoCorto;
        CambiaUnidadExpedientesJuridico();

        CruzaInformacionImagenesExpedientesJuridicos();

        if (_expedientesJuridicos is not null)
        {
            if (File.Exists(_archivoExpedienteJuridicoCsv))
                File.Delete(_archivoExpedienteJuridicoCsv);

            /// Se evita el cambio de estado y agencia, ya debe estar correcto al entrar aqui
            #region Agrega las imagenes
            var resultado2 = _expedientesJuridicos.Select(x => new ExpedienteJuridico()
            {
                Region = x.Region,
                CatRegion = (x.CatRegion ?? "").Replace("\n", "").Replace("|", ""),
                Agencia = x.Agencia,
                CatAgencia = (x.CatAgencia ?? "").Replace("\n", "").Replace("|", ""),
                Estado = (x.Estado ?? "").Replace("\n", "").Replace("|", ""),
                NombreDemandado = (x.NombreDemandado ?? "").Replace("\n", "").Replace("|", ""),
                TipoCredito = (x.TipoCredito ?? "").Replace("\n", "").Replace("|", ""),
                NumContrato = (x.NumContrato ?? "").Replace("\n", "").Replace("|", ""),
                NumCte = (x.NumCte ?? "").Replace("\n", "").Replace("|", ""),
                NumCredito = (x.NumCredito ?? "").Replace("\n", "").Replace("|", ""),
                NumCreditos = (x.NumCreditos ?? "").Trim().Replace("\n", "¬").Replace(";", "¬").Replace(" ", "¬").Replace("|", ""),
                FechaTranspasoJuridico = x.FechaTranspasoJuridico,
                FechaTranspasoExterno = x.FechaTranspasoExterno,
                FechaDemanda = x.FechaDemanda,
                Juicio = (x.Juicio ?? "").Replace("\n", "").Replace("|", ""),
                CapitalDemandado = x.CapitalDemandado,
                Dlls = x.Dlls,
                JuzgadoUbicacion = (x.JuzgadoUbicacion ?? "").Replace("\n", "¬").Replace("|", ""),
                Expediente = (x.Expediente ?? "").Replace("\n", "").Replace("|", ""),
                ClaveProcesal = (x.ClaveProcesal ?? "").Replace("\n", "").Replace("|", ""),
                EtapaProcesal = (x.EtapaProcesal ?? "").Replace("\n", "").Replace("|", ""),
                Rppc = (x.Rppc ?? "").Replace("\n", "").Replace("|", ""),
                Ran = (x.Ran ?? "").Replace("\n", "").Replace("|", ""),
                RedCredAgricola = (x.RedCredAgricola ?? "").Replace("\n", "").Replace("|", ""),
                FechaRegistro = x.FechaRegistro,
                Tipo = (x.Tipo ?? "").Replace("\n", "").Replace("|", ""),
                TipoGarantias = (x.TipoGarantias ?? "").Replace("\n", "").Replace("|", ""),
                Descripcion = (x.Descripcion ?? "").Replace("\n", "¬").Replace("|", ""),
                ValorRegistro = x.ValorRegistro,
                FechaAvaluo = x.FechaAvaluo,
                GradoPrelacion = (x.GradoPrelacion ?? "").Replace("\n", "").Replace("|", ""),
                CAval = (x.CAval ?? "").Replace("\n", "").Replace("|", ""),
                SAval = (x.SAval ?? "").Replace("\n", "").Replace("|", ""),
                Inscripcion = (x.Inscripcion ?? "").Replace("\n", "").Replace("|", ""),
                Clave = (x.Clave ?? "").Replace("\n", "").Replace("|", ""),
                NumFolio = (x.NumFolio ?? "").Replace("\n", "").Replace("|", ""),
                AbogadoResponsable = (x.AbogadoResponsable ?? "").Replace("\n", "").Replace("|", ""),
                Observaciones = (x.Observaciones ?? "").Replace("\n", "¬").Replace("|", ""),
                FechaUltimaActuacion = x.FechaUltimaActuacion,
                DescripcionActuacion = (x.DescripcionActuacion ?? "").Replace("\n", "¬").Replace("|", ""),
                Piso = (x.Piso ?? "").Replace("\n", "").Replace("|", ""),
                Fonaga = (x.Fonaga ?? "").Replace("\n", "").Replace("|", ""),
                PequenioProductor = (x.PequenioProductor ?? "").Replace("\n", "").Replace("|", ""),
                FondoMutual = (x.FondoMutual ?? "").Replace("\n", "").Replace("|", ""),
                ExpectativasRecuperacion = (x.ExpectativasRecuperacion ?? "").Replace("\n", "").Replace("|", ""),
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta,
                TieneImagenExpediente = x.TieneImagenExpediente
            }).ToList();
            #endregion
            _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedienteJuridicoCsv, resultado2);
        }
        return _imagenesCortasExpedientesJuridico;
    }

    private void CruzaInformacionImagenesExpedientesJuridicos()
    {
        if (_expedientesJuridicos is null || !_expedientesJuridicos.Any())
        {
            return;
        }
        if (_imagenesCortasExpedientesJuridico is null || !_imagenesCortasExpedientesJuridico.Any())
        {
            return;
        }
        // _imagenesCortasExpedientesJuridico
        var expedientesDirectos = from exp in _expedientesJuridicos
                                  join img in _imagenesCortasExpedientesJuridico on new { exp.NumContrato, exp.NumCredito } equals new { NumContrato = (img.NumContrato??"")[..15], img.NumCredito } into gj
                                    from subimg in gj.DefaultIfEmpty()
                                    select new { exp, subimg };

        foreach (var item in expedientesDirectos)
        {
            if (item.subimg is not null)
            {
                item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
            }
        }

        var expedientesInDirectos = from exp in _expedientesJuridicos.Where(x=>!x.TieneImagenDirecta && !x.TieneImagenIndirecta)
                                  join img in _imagenesCortasExpedientesJuridico on new { exp.NumContrato } equals new { NumContrato = (img.NumContrato ?? "")[..15] } into gj
                                  from subimg in gj.DefaultIfEmpty()
                                  select new { exp, subimg };

        foreach (var item in expedientesInDirectos)
        {
            if (item.subimg is not null)
            {
                item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
            }
        }

        var expedientesInDirectosCredito = from exp in _expedientesJuridicos.Where(x => !x.TieneImagenDirecta && !x.TieneImagenIndirecta)
                                    join img in _imagenesCortasExpedientesJuridico on new { exp.NumCredito } equals new { img.NumCredito } into gj
                                    from subimg in gj.DefaultIfEmpty()
                                    select new { exp, subimg };

        foreach (var item in expedientesInDirectosCredito)
        {
            if (item.subimg is not null)
            {
                item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
            }
        }
    }

    private bool CruzaInformacionImagenesConExpedientesJuridicosExpedientes(IEnumerable<ArchivoImagenCorta> imagenesCortas)
    {
        if (_expedientesJuridicos is not null)
        {
            var cruzaImagenes = (from scb in _expedientesJuridicos
                                 join img in imagenesCortas on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                                 select new
                                 {
                                     Saldos = scb,
                                     Imagenes = img
                                 }).ToList();
            foreach (var cruce in cruzaImagenes)
            {
                cruce.Saldos.TieneImagenExpediente = true;
            }
            //

            cruzaImagenes = (from scb in _expedientesJuridicos
                             join img in imagenesCortas on QuitaCastigoContrato(scb.NumCredito) equals QuitaCastigoContrato(img.NumCredito)
                             select new
                             {
                                 Saldos = scb,
                                 Imagenes = img
                             }).ToList();
            foreach (var cruce in cruzaImagenes)
            {
                cruce.Saldos.TieneImagenExpediente = true;
            }

        }
        return true;
    }


    public ExpedienteJuridicoDetalle? BuscaExpedienteJuridico(string contrato, string numCredito)
    {
        if (_expedientesJuridicos is null || !_expedientesJuridicos.Any())
        {
            return null;
        }   
        return _expedientesJuridicos.Where(x=>(x.NumContrato??"").Equals(contrato, StringComparison.OrdinalIgnoreCase) && (x.NumCredito??"").Equals(numCredito, StringComparison.OrdinalIgnoreCase)).
            Select(y => new ExpedienteJuridicoDetalle()
            {
                Region = y.Region,
                Agencia = y.Agencia,
                Estado = y.Estado,
                NumContrato = y.NumContrato,
                NombreDemandado = y.NombreDemandado,
                NumCte = y.NumCte,
                TipoCredito = y.TipoCredito,
                NumCredito = y.NumCredito,
                NumCreditos = y.NumCreditos,
                FechaTranspasoJuridico = y.FechaTranspasoJuridico,
                FechaTranspasoExterno = y.FechaTranspasoExterno,
                FechaDemanda = y.FechaDemanda,
                Juicio = y.Juicio,
                TipoGarantias = y.TipoGarantias,
                CapitalDemandado = y.CapitalDemandado,
                Fonaga = y.Fonaga,
                PequenioProductor = y.PequenioProductor,
                FondoMutual = y.FondoMutual,
                Expediente = y.Expediente,
                AbogadoResponsable = y.AbogadoResponsable,
                ExpectativasRecuperacion = y.ExpectativasRecuperacion,
                TieneImagenExpediente = y.TieneImagenExpediente,
                TieneImagenDirecta = y.TieneImagenDirecta,
                TieneImagenIndirecta = y.TieneImagenIndirecta
            }).FirstOrDefault();
    }

    #endregion
}
