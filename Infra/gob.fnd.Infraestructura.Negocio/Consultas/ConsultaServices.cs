using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Excel.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Juridico;
using gob.fnd.Dominio.Digitalizacion.Excel.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using gob.fnd.Dominio.Digitalizacion.Negocio.Tratamientos;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.DataValidation;
using System.Collections.Generic;
using System.Globalization;

namespace gob.fnd.Infraestructura.Negocio.Consultas;

public partial class ConsultaServices : IConsultaServices
{
    #region Variables privadas
    private readonly ILogger<ConsultaServices> _logger;
    private readonly IConfiguration _configuration;

    #region Servicios
    private readonly IServicioMinistracionesMesa _ministracionesMesaService;
    private readonly IServicioABSaldosActivos _abSaldosActivosService;
    private readonly IServicioABSaldosConCastigo _abSaldosConCastigoService;
    private readonly IServicioImagenes _servicioImagenes;
    private readonly IObtieneCorreosAgentes _obtieneCorreosAgentes;
    private readonly ICreaArchivoCsv _creaArchivoCsvService;
    private readonly IAdministraCargaConsulta _administraCargaConsultaService;
    private readonly IAdministraABSaldosDiario _administraABSaldosDiarioService;
    private readonly IAdministraCargaCorteDiario _administraCargaCorteDiarioService;
    private readonly IServicioCancelados _creditosCanceladosService;
    private readonly IAdministraCargaCreditosCancelados _administraCreditosCanceladosService;
    private readonly IBienesAdjudicados _bienesAdjudicadosService;
    private readonly IAdministraCargaBienesAdjudicados _administraCargaBienesAdjudicados;
    private readonly IAdministraCargaLiquidaciones _administraCargaLiquidaciones;
    private readonly ILiquidaciones _liquidacionesService;
    private readonly IOperacionesTratamientos _operacionesTratamientosService;
    private readonly IAdministraExpedientesJuridicos _administraExpedientesJuridicosService;
    private readonly IJuridico _juridicoService;
    private readonly IObtieneCatalogoProductos _obtieneCatalogoProductos;
    #endregion

    #region Información de los archivos
    private readonly string _archivoExpedientesConsultaCsv;
    private readonly string _archivoSoloPDFProcesado;
    private readonly string _archivoImagenCorta;
    private readonly string _archivoABSaldosDiarioOrigen;
    private readonly string _archivoABSaldosDiarioDestino;
    private readonly string _archivoSaldosDiarioProcesados;
    private readonly string _archivoABSaldosFiltroCreditoDiario;
    private readonly string _archivosImagenesBienesAdjudicados;
    private readonly string _archivosImagenesBienesAdjudicadosCorta;
    private readonly string _archivosImagenesExpedientesJuridico;
    private readonly string _archivosImagenesExpedientesJuridicoCorta;
    private readonly string _archivoBienesAdjudicados;
    private readonly string _carpetaUltimoABSaldos;
    private readonly string _archivoExpedienteJuridico;
    private readonly string _archivoExpedienteJuridicoCsv;
    private readonly string _archivoExpedientesConsultaGvCsv;
    private readonly string _archivoColocacionConPagos;
    private readonly string _archivoColocacionConPagosCsv;
    private readonly string _archivoDeExpedientesCanceladosCsv;
    #endregion

    private readonly DateTime _periodoDelDoctor;
    private readonly string _cargaCreditosCancelados;

    #region Creditos e imagenes
    private IEnumerable<ExpedienteDeConsultaGv>? _expedienteDeConsulta;
    private IEnumerable<CorreosAgencia>? _agencias;
    private IEnumerable<ABSaldosFiltroCreditoDiario>? _saldosConBandera;
    private IEnumerable<ArchivoImagenCorta>? _imagenesCortas;
    private IEnumerable<ArchivoImagenBienesAdjudicadosCorta>? _imagenesCortasBienesAdjudicados;
    private IEnumerable<ArchivoImagenExpedientesCorta>? _imagenesCortasExpedientesJuridico;
    private IEnumerable<ExpedienteJuridico>? _expedientesJuridicos;
    private IEnumerable<CreditosCanceladosAplicacion>? _creditosCanceladosAplicacion;
    private IEnumerable<DetalleBienesAdjudicados>? _detalleBienesAdjudicados;
    private IEnumerable<ColocacionConPagos>? _creditosLiquidados;
    private IEnumerable<ABSaldosCompleta>? _abSaldosCompleta;
    private IEnumerable<GuardaValores>? _guardaValores;
    #endregion

    /// <summary>
    /// La unidad de imagenes cuando se cargaron
    /// </summary>
    private readonly string _driveOrigen;
    /// <summary>
    /// La unidad de imagenes donde se encuentren cargadas
    /// </summary>
    private readonly string _driveDestino;
    /// <summary>
    /// Ultimo archivo procesado que evita que se duerma la unidad de imagenes
    /// </summary>
    private string _ultimoArchivo = "";
    /// <summary>
    /// En caso de que marque error al escribir el archivo temporal, evita volver a entrar al ciclo
    /// </summary>
    private bool _tieneErrorTemporal = false;
    #endregion

    public ConsultaServices(ILogger<ConsultaServices> logger, IConfiguration configuration,
        #region Servicios
        IServicioMinistracionesMesa ministracionesMesaService,
        IServicioABSaldosActivos abSaldosActivosService,
        IServicioABSaldosConCastigo abSaldosConCastigoService,
        IServicioImagenes servicioImagenes,
        IObtieneCorreosAgentes obtieneCorreosAgentes,
        ICreaArchivoCsv creaArchivoCsvService,
        IAdministraCargaConsulta administraCargaConsultaService,
        IAdministraABSaldosDiario administraABSaldosDiarioService,
        IAdministraCargaCorteDiario administraCargaCorteDiarioService,
        IServicioCancelados creditosCanceladosService,
        IAdministraCargaCreditosCancelados administraCreditosCanceladosService,
        IBienesAdjudicados bienesAdjudicadosService,
        IAdministraCargaBienesAdjudicados administraCargaBienesAdjudicados,
        IAdministraCargaLiquidaciones administraCargaLiquidaciones,
        ILiquidaciones liquidacionesService,
        IOperacionesTratamientos operacionesTratamientosService,
        IAdministraExpedientesJuridicos administraExpedientesJuridicosService,
        IJuridico juridicoService,
        IObtieneCatalogoProductos obtieneCatalogoProductos
    #endregion
        )
    {
        _logger = logger;
        _configuration = configuration;

        _ministracionesMesaService = ministracionesMesaService;
        _abSaldosActivosService = abSaldosActivosService;
        _abSaldosConCastigoService = abSaldosConCastigoService;
        _servicioImagenes = servicioImagenes;
        _obtieneCorreosAgentes = obtieneCorreosAgentes;
        _creaArchivoCsvService = creaArchivoCsvService;
        _administraCargaConsultaService = administraCargaConsultaService;
        _administraABSaldosDiarioService = administraABSaldosDiarioService;
        _administraCargaCorteDiarioService = administraCargaCorteDiarioService;
        _creditosCanceladosService = creditosCanceladosService;
        _administraCreditosCanceladosService = administraCreditosCanceladosService;
        _bienesAdjudicadosService = bienesAdjudicadosService;
        _administraCargaBienesAdjudicados = administraCargaBienesAdjudicados;
        _administraCargaLiquidaciones = administraCargaLiquidaciones;
        _liquidacionesService = liquidacionesService;
        _operacionesTratamientosService = operacionesTratamientosService;
        _administraExpedientesJuridicosService = administraExpedientesJuridicosService;
        _juridicoService = juridicoService;
        _obtieneCatalogoProductos = obtieneCatalogoProductos;
        _archivoSoloPDFProcesado = _configuration.GetValue<string>("archivoSoloPDFProcesado") ?? "";
        _archivoExpedientesConsultaCsv = _configuration.GetValue<string>("archivoExpedientesConsulta") ?? "";
        _archivoExpedientesConsultaGvCsv = _configuration.GetValue<string>("archivoExpedientesConsultaGv") ?? "";
        _archivoImagenCorta = _configuration.GetValue<string>("archivoImagenCorta") ?? "";
        _archivoABSaldosFiltroCreditoDiario = _configuration.GetValue<string>("archivoABSaldosFiltroCreditoDiario") ?? "";
        _archivoABSaldosDiarioOrigen = _configuration.GetValue<string>("archivoABSaldosDiarioOrigen") ?? "";
        _archivoABSaldosDiarioOrigen = String.Format(_archivoABSaldosDiarioOrigen, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        _archivoABSaldosDiarioDestino = _configuration.GetValue<string>("archivoABSaldosDiarioDestino") ?? "";
        _archivoABSaldosDiarioDestino = String.Format(_archivoABSaldosDiarioDestino, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        _archivoSaldosDiarioProcesados = _configuration.GetValue<string>("archivoSaldosDiarioProcesados") ?? "";
        _archivoSaldosDiarioProcesados = String.Format(_archivoSaldosDiarioProcesados, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        _archivoDeExpedientesCanceladosCsv = _configuration.GetValue<string>("archivosCreditosCancelados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\HistoricoCancelados.csv";
        _archivoBienesAdjudicados = _configuration.GetValue<string>("archivoBienesAdjudicados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados.csv";
        _archivosImagenesBienesAdjudicados = _configuration.GetValue<string>("archivosImagenesBienesAdjudicados") ?? "C:\\202302 Digitalizacion\\6. Resultado de las imagenes\\ImagenesBienesAdjudicadosTodos.xlsx";
        _archivosImagenesBienesAdjudicadosCorta = _configuration.GetValue<string>("archivosImagenesBienesAdjudicadosCorta") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\ImagenesBienesAdjudicadosTodos.Csv";
        _archivoColocacionConPagos = _configuration.GetValue<string>("archivoLiquidacionesOrigen") ?? "C:\\202302 Digitalizacion\\1. AB Saldos\\Liquidados\\historico colocacion con pagos 09mayo2023.xlsx";
        _archivoColocacionConPagosCsv = _configuration.GetValue<string>("archivoLiquidaciones") ?? "C:\\202302 Digitalizacion\\1. AB Saldos\\Liquidados\\historico colocacion con pagos final.csv";
        _driveOrigen = _configuration.GetValue<string>("unidadAnteriorImagenes") ?? "F:\\";
        _driveDestino = _configuration.GetValue<string>("unidadDeImagenes") ?? ObtieneUnidadImagenes();
        _carpetaUltimoABSaldos = _configuration.GetValue<string>("carpetaUltimoABSaldos") ?? "";
        _cargaCreditosCancelados = _configuration.GetValue<string>("cargaCreditosCancelados") ?? "C:\\202302 Digitalizacion\\1. AB SALDOS\\Cancelados Historico\\31marzo2023Historico cancelado.xlsx";
        _archivoExpedienteJuridico = _configuration.GetValue<string>("archivoExpedientesJuridicos") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\ExpedientesJuridicos\\BITACORA GENERAL AL 31 DE MARZO 2023.xlsx";
        _archivoExpedienteJuridicoCsv = _configuration.GetValue<string>("archivoExpedientesJuridicosCsv") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\ExpedientesJuridicos\\BITACORA GENERAL AL 31 DE MARZO 2023.csv";
        _archivosImagenesExpedientesJuridico = _configuration.GetValue<string>("archivosImagenesExpedientesJuridico") ?? "C:\\202302 Digitalizacion\\6. Resultado de las imagenes\\TodosLosTurnos.xlsx";
        _archivosImagenesExpedientesJuridicoCorta = _configuration.GetValue<string>("archivosImagenesExpedientesJuridicoCorta") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\TodosLosTurnos.csv";

        if (!File.Exists(_archivoSaldosDiarioProcesados))
            _archivoSaldosDiarioProcesados = ObtieneUltimoABSaldosProcesados(_archivoSaldosDiarioProcesados);

        _saldosConBandera = null;
        _expedienteDeConsulta = null;
        string stringFechaPeriodoDelDoctor = _configuration.GetValue<string>("periodoDelDoctor") ?? "01/07/2020";
        _periodoDelDoctor = DateTime.ParseExact(stringFechaPeriodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        
        _imagenesCortas = null;
    }
    public IEnumerable<ExpedienteDeConsultaGv> CargaInformacion()
    {
        _expedienteDeConsulta = null; // new List<ExpedienteDeConsulta>();
        bool ultimosSaldos = false;
        if (_administraABSaldosDiarioService.CopiaABSaldosDiario())
        {
            _abSaldosCompleta = _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino);
            LlenaInformacionABSaldosFiltroCredito(_abSaldosCompleta);
            ultimosSaldos = true;
        }
        else
        {
            if (!File.Exists(_archivoABSaldosDiarioDestino))
            {
                if (_administraABSaldosDiarioService.DescargaABSaldosDiario())
                {
                    _abSaldosCompleta = _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino);
                    LlenaInformacionABSaldosFiltroCredito(_abSaldosCompleta);
                    ultimosSaldos = true;
                }
            }
            else
            {
                string lastFileName = ObtieneUltimoABSaldos();
                if (string.IsNullOrEmpty(lastFileName))
                {
                    lastFileName = _archivoSaldosDiarioProcesados;
                }
                /*
                else
                    ultimosSaldos = true;
                */

                if (File.Exists(lastFileName))
                {
                    _abSaldosCompleta = _administraCargaCorteDiarioService.CargaABSaldosCompleta(lastFileName);
                    LlenaInformacionABSaldosFiltroCredito(_abSaldosCompleta);
                    // ultimosSaldos = false;
                    ultimosSaldos = true;
                }
                else
                {
                    if (File.Exists(_archivoABSaldosDiarioDestino))
                    {
                        _abSaldosCompleta = _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino);
                        LlenaInformacionABSaldosFiltroCredito(_abSaldosCompleta);
                        ultimosSaldos = true;
                    }
                }
            }
        }

        FileInfo fi = new(_archivoExpedientesConsultaGvCsv);
        FileInfo fi2 = new(_archivoExpedientesConsultaCsv);
        if (fi.Exists || fi2.Exists)
        {
            IEnumerable<ExpedienteDeConsultaGv> resultado;
            if (!fi.Exists && fi2.Exists)
            {
                IEnumerable<ExpedienteDeConsulta> compatibilidad = _administraCargaConsultaService.CargaExpedienteDeConsulta(_archivoExpedientesConsultaCsv);
                resultado = compatibilidad.Select(x => new ExpedienteDeConsultaGv(x));
            }
            else {
                resultado = _administraCargaConsultaService.CargaExpedienteDeConsultaGv(_archivoExpedientesConsultaGvCsv);
            }

            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");
            if (ultimosSaldos)
            {
                foreach (var item in resultado)
                {
                    item.EsCreditoAReportar = false;
                    item.StatusCarteraVencida = false;
                    item.StatusCarteraVencida = false;
                    item.StatusImpago = false;
                }

                resultado = CruceConSaldosCuandoHayCambios(resultado);
            }
            _expedienteDeConsulta = resultado.ToList();
            return _expedienteDeConsulta;
        }

        _logger.LogInformation("Carga la información para las consultas de imágenes (Información de Expedientes)");
        var ministracionesMesa = _ministracionesMesaService.GetMinistraciones();
        _logger.LogInformation("Se cargaron {numMinistraciones} ministraciones de la mesa", ministracionesMesa.Count());

        _logger.LogInformation("Se carga la información de los saldos activos");
        var saldosActivos = _abSaldosActivosService.GetABSaldosActivos();
        _logger.LogInformation("Se cargaron {numSaldosActivos} expedientes activos", saldosActivos.Count());

        _logger.LogInformation("Se carga la información de los saldos con Castigo");
        var saldosConCastigo = _abSaldosConCastigoService.ObtieneABSaldosConCastigoProcesados();
        _logger.LogInformation("Se cargaron {numSaldosConCastigo} saldos clasificados con Castigos", saldosConCastigo.Count());

        IEnumerable<ExpedienteDeConsultaGv> expedientesCreditosActivos = CruzaInformacionMesa(ministracionesMesa, saldosActivos, saldosConCastigo);
        expedientesCreditosActivos = CruceConSaldosCuandoHayCambios(expedientesCreditosActivos);
        _expedienteDeConsulta = expedientesCreditosActivos.ToList();

        return _expedienteDeConsulta;
    }

    private IEnumerable<ExpedienteDeConsultaGv> CruceConSaldosCuandoHayCambios(IEnumerable<ExpedienteDeConsultaGv> expedientesCreditosActivos)
    {
        expedientesCreditosActivos = CruzaInformacionMesa(expedientesCreditosActivos);

        _agencias = _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();
        CruzaInformacionAgencias(_agencias, expedientesCreditosActivos);

        _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedientesConsultaGvCsv, expedientesCreditosActivos);
        if (File.Exists(_archivoExpedientesConsultaCsv))
        {
            File.Delete(_archivoExpedientesConsultaCsv);
        }

        _logger.LogInformation("Se guardo el archivo {archivoExpedienteConsulta} para una carga rápida en el futuro", _archivoExpedientesConsultaCsv);
        
        return expedientesCreditosActivos;
    }

    private string ObtieneUltimoABSaldos()
    {
        if (!Directory.Exists(_carpetaUltimoABSaldos))
            return string.Empty;
        var csvFiles = Directory.GetFiles(_carpetaUltimoABSaldos, "*.csv");

        if (csvFiles.Length == 0)
        {
            return string.Empty;
        }

        var latestCsvFile = csvFiles.OrderByDescending(file => File.GetLastWriteTime(file)).First();

        return latestCsvFile;
    }

    private static string ObtieneUltimoABSaldosProcesados(string abSaldosProcesados)
    {
        string carpetaABSaldosProcesados = Path.GetDirectoryName(abSaldosProcesados) ?? "";
        if (!Directory.Exists(carpetaABSaldosProcesados))
            return abSaldosProcesados;
        var csvFiles = Directory.GetFiles(carpetaABSaldosProcesados, "ABSDO202*.csv");

        if (csvFiles.Length == 0)
        {
            return abSaldosProcesados;
        }

        var latestCsvFile = csvFiles.OrderByDescending(file => File.GetLastWriteTime(file)).First();

        return latestCsvFile;
    }

    private static string ObtieneEstadoCartera(ABSaldosCompleta abSaldosCartera)
    {
        if ((abSaldosCartera.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && ((abSaldosCartera.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)))
            return "Impago";
        if ((abSaldosCartera.StatusContable ?? "").Equals("B", StringComparison.InvariantCultureIgnoreCase))
            return "Vencida";
        if ((abSaldosCartera.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && (!(abSaldosCartera.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)))
            return "Vigente";
        return "No clasificada";
    }

    private void LlenaInformacionABSaldosFiltroCredito(IEnumerable<ABSaldosCompleta> abSaldosCompleta)
    {
        string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');
        var filtaABSaldosCompletaPorProducto = abSaldosCompleta.Where(x => !arrayProductosFiltra.Any(y => y.Equals((x.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase))).ToList();
        filtaABSaldosCompletaPorProducto = filtaABSaldosCompletaPorProducto.Where(x => !(x.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase)).ToList();

        _saldosConBandera = filtaABSaldosCompletaPorProducto.Select(x =>
        new ABSaldosFiltroCreditoDiario()
        {
            Regional = x.Regional,
            NumCte = x.NumCte,
            Sucursal = x.Sucursal,
            ApellPaterno = x.ApellPaterno,
            ApellMaterno = x.ApellMaterno,
            Nombre1 = x.Nombre1,
            Nombre2 = x.Nombre2,
            RazonSocial = x.RazonSocial,
            EstadoInegi = x.EstadoInegi,
            NumCredito = x.NumCredito,
            NumProducto = x.NumProducto,
            TipoCartera = x.TipoCartera,
            FechaApertura = x.FechaApertura,
            FechaVencim = x.FechaVencim,
            StatusContable = x.StatusContable,
            SldoTotContval = x.SldoTotContval,
            NumContrato = x.NumContrato,
            FechaSuscripcion = x.FechaSuscripcion,
            FechaVencCont = x.FechaVencCont,
            TipoCreditoCont = x.TipoCreditoCont,
            StatusImpago = (x.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && ((x.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)),
            StatusCarteraVencida = (x.StatusContable ?? "").Equals("B", StringComparison.InvariantCultureIgnoreCase),
            StatusCarteraVigente = (x.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && (!(x.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)),
            TieneImagenDirecta = false,
            TieneImagenIndirecta = false
        }
        ).ToList();
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
        if ((digito == "1" || digito == "9") && (digito2 != "0"))
        {
            nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
            // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
        }
        return nuevoOrigen;
    }

    public static string QuitaCastigoContrato(string? origen)
    {
        if (origen is null)
            return "000000000000000000";

        if (origen.Length < 18)
            return "000000000000000000";
        string digito = origen.Substring(3, 1);
        string digito2 = origen.Substring(4, 1);
        string nuevoOrigen = origen;
        if ((digito == "1" || digito == "9") && (digito2 != "0"))
        {
            nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
            // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
        }
        nuevoOrigen = nuevoOrigen[..14] + "0000";
        return nuevoOrigen;
    }


    public static string QuitaCastigoIndirecto(string? origen)
    {
        if (origen is null)
            return "000000000000000000";

        if (origen.Length < 14)
            return "000000000000000000";
        string digito = origen.Substring(3, 1);
        string digito2 = origen.Substring(4, 1);
        string nuevoOrigen = origen;
        if ((digito == "1" || digito == "9") && (digito2 != "0"))
        {
            nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
            nuevoOrigen = nuevoOrigen[..14] + "0000";
            // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
        }
        else
            nuevoOrigen = nuevoOrigen[..14] + "0000";
        return nuevoOrigen;
    }

    private static bool CruzaInformacionAgencias(IEnumerable<CorreosAgencia> correos, IEnumerable<ExpedienteDeConsultaGv> expedientes) {
        var cruzaInformacion = (from exp in expedientes
                                join correo in correos on exp.Agencia equals correo.NoAgencia
                                select new { Expedientes = exp, Agencias = correo }).ToList();
        foreach (var info in cruzaInformacion) {
            info.Expedientes.CatAgencia = info.Agencias.Agencia;
            info.Expedientes.CatRegion = info.Agencias.Region;
        }

        return true;
    }

    private static IEnumerable<ExpedienteDeConsultaGv> CruzaInformacionMesa(IEnumerable<MinistracionesMesa> ministracionesMesa, IEnumerable<ABSaldosActivos> saldosActivos, IEnumerable<ABSaldosConCastigo> saldosConCastigo)
    {

        #region las ministraciones unicas
        var ministracionesUnicas = (from min in ministracionesMesa
                                    group min by min.NumCredito into grupo
                                    let laMayorMinistracion = grupo.OrderByDescending(x => x.NumMinistracion).FirstOrDefault()
                                    select laMayorMinistracion).ToList();
        var leftJoinResult = (from min in ministracionesUnicas
                              join sa in saldosActivos on min.NumCredito equals QuitaCastigo(sa.NumCredito) into sa2Group
                              from sa2Result in sa2Group.DefaultIfEmpty()
                              join t3 in saldosConCastigo on min.NumCredito equals QuitaCastigo(t3.NumCredito) into t3Group
                              from t3Result in t3Group.DefaultIfEmpty()
                              select new { Ministracion = min,
                                  SaldosActivos = sa2Result,
                                  SaldosConCastigo = t3Result }
                              ).ToList();

        #endregion

        #region Cruzo la información de los Saldos Activos vs la Mesa
        IList<ExpedienteDeConsultaGv> lista = new List<ExpedienteDeConsultaGv>();
        var resultados = (from a in leftJoinResult
                          select new ExpedienteDeConsultaGv {
                              NumCredito = a.Ministracion.NumCredito,
                              NumCreditoCancelado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumCredito : (a.SaldosActivos is not null) ? a.SaldosActivos.NumCreditoCancelado : string.Empty,
                              EsSaldosActivo = a.SaldosActivos is not null,
                              EsCancelado = a.SaldosConCastigo is not null,
                              EsOrigenDelDr = a.Ministracion.EsOrigenDelDoctor,
                              EsCanceladoDelDr = a.SaldosConCastigo is not null && a.SaldosConCastigo.EsCancelacionDelDoctor,
                              EsCastigado = (a.SaldosConCastigo is not null) && (a.SaldosConCastigo.Castigo ?? "").Equals("Castigado", StringComparison.OrdinalIgnoreCase),
                              TieneArqueo = (a.SaldosActivos is not null) ? a.SaldosActivos.CuentaConGuardaValores : (a.SaldosConCastigo is not null) && a.SaldosConCastigo.CuentaConGuardaValores,
                              Acreditado = a.Ministracion.Acreditado,
                              FechaApertura = (a.SaldosActivos is not null) ? a.SaldosActivos.FechaApertura : a.SaldosConCastigo?.FechaApertura,
                              FechaCancelacion = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.FechaCancelacion : a.SaldosActivos?.FechaCancelacion,
                              Castigo = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Castigo : a.SaldosActivos?.Castigo,
                              NumProducto = (a.SaldosActivos is not null) ? a.SaldosActivos.NumProducto : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumProducto : "",
                              CatProducto = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : "",
                              TipoDeCredito = a.Ministracion.Descripcion,
                              Ejecutivo = (a.SaldosActivos is not null) ? a.SaldosActivos.NombreEjecutivo : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NombreEjecutivo : "",
                              Analista = a.Ministracion.Analista,
                              FechaInicioMinistracion = a.Ministracion.FechaAsignacion,
                              FechaSolicitud = a.Ministracion.FechaAsignacion,
                              MontoCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.MontoCredito ?? 0 :
                               (a.SaldosActivos is not null) ? a.SaldosActivos.MontoMinistraCap : 0,
                              InterCont = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.InteresCont ?? 0 :
                               (a.SaldosActivos is not null) ? a.SaldosActivos.SldoIntCont : 0,
                              Region = a.Ministracion.Regional,
                              Agencia = a.Ministracion.Sucursal,
                              CatRegion = (a.SaldosConCastigo is not null && !string.IsNullOrEmpty(a.SaldosConCastigo.CatRegion)) ? a.SaldosConCastigo.CatRegion ?? string.Empty :
                               (a.SaldosActivos is not null && !string.IsNullOrEmpty(a.SaldosActivos.CatRegion)) ? a.SaldosActivos.CatRegion : string.Empty,
                              CatAgencia = a.Ministracion.CatAgencia,
                              NumCliente = ObtieneNumCliente(a.SaldosConCastigo, a.SaldosActivos)

                          }).ToList();

        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultados);
        #endregion

        #region Con castigo sin ministracion
        var leftJoinResult2 = (from sc in saldosConCastigo
                               join min in ministracionesUnicas on QuitaCastigo(sc.NumCredito) equals min.NumCredito into t2Group
                               from t2Result in t2Group.DefaultIfEmpty()
                               join sa in saldosActivos on QuitaCastigo(sc.NumCredito) equals QuitaCastigo(sa.NumCredito) into t3Group
                               from t3Result in t3Group.DefaultIfEmpty()
                               select new
                               {
                                   SaldosConCastigo = sc,
                                   Ministracion = t2Result,
                                   SaldosActivos = t3Result
                               }
                              ).ToList();

        // Elimino las que tienen ministración, esas ya las atendí
        leftJoinResult2 = leftJoinResult2.Where(x => x.Ministracion is null).ToList();

        var resultadosNuevos = (from a in leftJoinResult2
                                select new ExpedienteDeConsultaGv
                                {
                                    NumCredito = QuitaCastigo(a.SaldosConCastigo.NumCredito),
                                    NumCreditoCancelado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumCredito : (a.SaldosActivos is not null) ? a.SaldosActivos.NumCreditoCancelado : String.Empty,
                                    EsSaldosActivo = a.SaldosActivos is not null,
                                    EsCancelado = a.SaldosConCastigo is not null,
                                    EsOrigenDelDr = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.EsOrigenDelDoctor : (a.SaldosActivos is not null) && a.SaldosActivos.EsOrigenDelDoctor,
                                    EsCanceladoDelDr = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.EsCancelacionDelDoctor : (a.SaldosActivos is not null) && a.SaldosActivos.EsCancelacionDelDoctor,
                                    EsCastigado = (a.SaldosConCastigo is not null) && (a.SaldosConCastigo.Castigo ?? "").Equals("Castigado", StringComparison.OrdinalIgnoreCase),
                                    TieneArqueo = (a.SaldosActivos is not null) ? a.SaldosActivos.CuentaConGuardaValores : (a.SaldosConCastigo is not null) && a.SaldosConCastigo.CuentaConGuardaValores,
                                    Acreditado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Acreditado : (a.SaldosActivos is not null) ? a.SaldosActivos.Acreditado : String.Empty,
                                    FechaApertura = (a.SaldosActivos is not null) ? a.SaldosActivos.FechaApertura : a.SaldosConCastigo?.FechaApertura,
                                    FechaCancelacion = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.FechaCancelacion : a.SaldosActivos?.FechaCancelacion,
                                    Castigo = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Castigo : a.SaldosActivos?.Castigo,
                                    NumProducto = (a.SaldosActivos is not null) ? a.SaldosActivos.NumProducto : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumProducto : String.Empty,
                                    CatProducto = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : "",
                                    TipoDeCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : String.Empty,
                                    Ejecutivo = (a.SaldosActivos is not null) ? a.SaldosActivos.NombreEjecutivo : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NombreEjecutivo : String.Empty,
                                    Analista = String.Empty,
                                    FechaInicioMinistracion = a.SaldosActivos?.FechaInicioMinistracion,
                                    FechaSolicitud = null,
                                    MontoCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.MontoCredito ?? 0 :
                                       (a.SaldosActivos is not null) ? a.SaldosActivos.MontoMinistraCap : 0,
                                    InterCont = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.InteresCont ?? 0 :
                                       (a.SaldosActivos is not null) ? a.SaldosActivos.SldoIntCont : 0,
                                    Region = (a.SaldosActivos is not null) ? a.SaldosActivos.Regional : 0,
                                    Agencia = (a.SaldosConCastigo is not null) ? Convert.ToInt32(a.SaldosConCastigo.Sucursal) : (a.SaldosActivos is not null) ? a.SaldosActivos.Regional : 0,
                                    CatRegion = (a.SaldosConCastigo is not null && !string.IsNullOrEmpty(a.SaldosConCastigo.CatRegion)) ? a.SaldosConCastigo.CatRegion ?? string.Empty :
                                       (a.SaldosActivos is not null && !string.IsNullOrEmpty(a.SaldosActivos.CatRegion)) ? a.SaldosActivos.CatRegion : string.Empty,
                                    CatAgencia = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatSucursal : (a.SaldosActivos is not null) ? a.SaldosActivos.CatSucursal : (a.Ministracion is not null) ? a.Ministracion.CatAgencia : string.Empty,
                                    NumCliente = ObtieneNumCliente(a.SaldosConCastigo, a.SaldosActivos)
                                }).ToList();
        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultadosNuevos);
        #endregion

        #region Agrego regiones y Agencias
        foreach (var item in lista)
        {
            if (item.Region == 0)
                item.Region = Convert.ToInt32((item.NumCredito ?? "0")[..1]) * 100;
            if (item.Agencia == 0)
                item.Agencia = Convert.ToInt32((item.NumCredito ?? "000")[..3]);
            // Falta agregar el catalogo de regiones y de agencias
        }
        #endregion
        return lista.OrderBy(x => x.NumCredito).ToList();
    }

    private static string ObtieneNumCliente(ABSaldosConCastigo? saldosConCastigo, ABSaldosActivos? saldosActivos) {
        if (saldosConCastigo is not null && !string.IsNullOrEmpty(saldosConCastigo.NumeroCliente))
        {
            return saldosConCastigo.NumeroCliente;
        }
        if (saldosActivos is not null && !string.IsNullOrEmpty(saldosActivos.NumCte))
        {
            return saldosActivos.NumCte;
        }
        return string.Empty;
    }

    private static string ObtieneNumCliente(ABSaldosFiltroCreditoDiario? saldosFiltro,
        ExpedienteDeConsulta? expedienteConsulta)
    {
        if (saldosFiltro is not null && !string.IsNullOrEmpty(saldosFiltro.NumCte))
        {
            return saldosFiltro.NumCte;
        }
        if (expedienteConsulta is not null && !string.IsNullOrEmpty(expedienteConsulta.NumCliente))
        {
            return expedienteConsulta.NumCliente;
        }
        return string.Empty;
    }
    private IEnumerable<ExpedienteDeConsultaGv> CruzaInformacionMesa(IEnumerable<ExpedienteDeConsultaGv> origen)
    {
        if (_saldosConBandera is null) {
            return origen;
        }
        var leftJoinResult = (from scb in _saldosConBandera
                              join sa in origen on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(sa.NumCredito) into sa2Group
                              from sa2Result in sa2Group.DefaultIfEmpty()
                              select new
                              {
                                  SalBan = scb,
                                  ExpCons = sa2Result
                              }
                              ).ToList();

        IList<ExpedienteDeConsultaGv> lista = new List<ExpedienteDeConsultaGv>();
        var resultados = (from a in leftJoinResult
                          where a.ExpCons is not null
                          select new ExpedienteDeConsultaGv
                          {
                              NumCredito = QuitaCastigo(a.SalBan.NumCredito),
                              NumCreditoCancelado = (a.ExpCons is not null) ? a.ExpCons.NumCreditoCancelado : String.Empty,
                              EsSaldosActivo = (a.ExpCons is not null) && a.ExpCons.EsSaldosActivo,
                              EsCancelado = (a.ExpCons is not null) && a.ExpCons.EsCancelado,
                              EsOrigenDelDr = a.SalBan.FechaApertura >= _periodoDelDoctor,
                              EsCanceladoDelDr = (a.ExpCons is not null) && a.ExpCons.EsCanceladoDelDr,
                              EsCastigado = (a.ExpCons is not null) && a.ExpCons.EsCastigado,
                              TieneArqueo = (a.ExpCons is not null) && a.ExpCons.TieneArqueo,
                              Acreditado = ObtieneAcreditado(a.SalBan.ApellPaterno, a.SalBan.ApellMaterno, a.SalBan.Nombre1, a.SalBan.Nombre2, a.SalBan.RazonSocial),
                              FechaApertura = a.SalBan.FechaApertura,
                              FechaCancelacion = a.ExpCons?.FechaCancelacion,
                              Castigo = (a.ExpCons is not null) ? a.ExpCons.Castigo : string.Empty,
                              NumProducto = a.SalBan.NumProducto,
                              CatProducto = (a.ExpCons is not null) ? a.ExpCons.CatProducto : string.Empty,
                              TipoDeCredito = (a.ExpCons is not null) ? a.ExpCons.TipoDeCredito : string.Empty,
                              Ejecutivo = (a.ExpCons is not null) ? a.ExpCons.Ejecutivo : string.Empty,
                              Analista = (a.ExpCons is not null) ? a.ExpCons.Analista : string.Empty,
                              FechaInicioMinistracion = a.ExpCons?.FechaInicioMinistracion,
                              FechaSolicitud = a.ExpCons?.FechaSolicitud,
                              MontoCredito = (a.ExpCons is not null) ? a.ExpCons.MontoCredito : 0M,
                              InterCont = (a.ExpCons is not null) ? a.ExpCons.InterCont : 0M,
                              Region = a.SalBan.Regional,
                              Agencia = a.SalBan.Sucursal,
                              CatRegion = (a.ExpCons is not null) ? a.ExpCons.CatRegion : string.Empty,
                              CatAgencia = (a.ExpCons is not null) ? a.ExpCons.CatAgencia : string.Empty,
                              EsCreditoAReportar = true,
                              StatusImpago = a.SalBan.StatusImpago,
                              StatusCarteraVencida = a.SalBan.StatusCarteraVencida,
                              StatusCarteraVigente = a.SalBan.StatusCarteraVigente,
                              TieneImagenDirecta = a.SalBan.TieneImagenDirecta,
                              TieneImagenIndirecta = a.SalBan.TieneImagenIndirecta,
                              SldoTotContval = a.SalBan.SldoTotContval,
                              NumCliente = ObtieneNumCliente(a.SalBan, a.ExpCons)
                          }).ToList();

        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultados);

        var leftJoinResultExpConsulta = (from sa in origen
                                         join scb in lista on QuitaCastigo(sa.NumCredito) equals QuitaCastigo(scb.NumCredito) into sa2Group
                                         from sa2Result in sa2Group.DefaultIfEmpty()
                                         select new
                                         {
                                             ExpCons = sa,
                                             SalBan = sa2Result
                                         }
                      ).ToList();
        var soloOrigen = leftJoinResultExpConsulta.Where(x => x.SalBan is null).Select(x => x.ExpCons).ToList();
        ((List<ExpedienteDeConsultaGv>)lista).AddRange(soloOrigen);

        return lista.ToList();
    }

    private static string ObtieneAcreditado(string? apellidoPaterno, string? apellidoMaterno, string? primerNombre, string? segundoNombre, string? razonSocial) {
        if (!string.IsNullOrWhiteSpace(razonSocial))
            return razonSocial.Trim();
        else
        {
            string nombreCompleto = (primerNombre ?? "") + " " + (segundoNombre ?? "") + " " + (apellidoPaterno ?? "") + " " + (apellidoMaterno ?? "");
            return nombreCompleto.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        }
    }

    private static bool CruzaInformacionAgencias(IEnumerable<CorreosAgencia> correos, IEnumerable<GuardaValores> guardaValores)
    {
        var cruzaInformacion = (from gv in guardaValores
                                join correo in correos on gv.Sucursal equals correo.NoAgencia
                                select new { GuardaValor = gv, Agencias = correo }).ToList();
        foreach (var info in cruzaInformacion)
        {
            info.GuardaValor.CatAgencia = info.Agencias.Agencia;
            info.GuardaValor.CatRegional = info.Agencias.Region;
        }

        return true;
    }

    private static string ObtieneCatalogoDeProducto(IEnumerable<CatalogoProductos> productos, string numProducto)
    {
        var producto = productos.FirstOrDefault(x => (x.NumProducto??"").Equals(numProducto, StringComparison.InvariantCultureIgnoreCase));
        return producto?.CatProducto ?? string.Empty;
    }
    private IEnumerable<GuardaValores> ObtieneSaldosFiltrados(IEnumerable<CatalogoProductos> productos) {
        string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');
        var filtaABSaldosCompletaPorProducto = _abSaldosCompleta?.Where(x => !arrayProductosFiltra.Any(y => y.Equals((x.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase))).ToList();
        filtaABSaldosCompletaPorProducto = filtaABSaldosCompletaPorProducto?.Where(x => !(x.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase)).ToList();

        var resultado  = (filtaABSaldosCompletaPorProducto?.Select(x=> new GuardaValores() { 
             Regional = x.Regional,
             EstadoInegi = x.EstadoInegi,
             Sucursal = x.Sucursal,
             NumCte = x.NumCte,
             Acreditado = ObtieneAcreditado(x.ApellPaterno, x.ApellMaterno, x.Nombre1, x.Nombre2, x.RazonSocial),
             NumContrato = x.NumContrato,
             NumCredito = x.NumCredito,
             NumProducto = x.NumProducto,
             CatProducto = ObtieneCatalogoDeProducto(productos, x.NumProducto ?? "   "),
             TipoCreditoCont = x.TipoCreditoCont,
             EstadoDeLaCartera = ObtieneEstadoCartera(x),
             SldoTotContval = x.SldoTotContval,
             EsCreditoAReportar = ObtieneEsCreditoAReportar(x),
             Imagen = ""
         }));
        return resultado??new List<GuardaValores>();
    }

    public IEnumerable<GuardaValores> ObtieneGuardaValores(int Agencia) 
    {
        if (_guardaValores is not null && _guardaValores.Any())
            return _guardaValores;

        if (_imagenesCortas is not null)
        {
            var productos = _obtieneCatalogoProductos.ObtieneElCatalogoDelProducto();

            IEnumerable<GuardaValores> resultado = _imagenesCortas.Where(x => (x.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase)).
                Select(x => new GuardaValores()
                {
                    NumContrato = (x.NumCredito ?? "000000000000000")[..15],
                    Imagen = Path.Combine(x.CarpetaDestino??"",x.NombreArchivo??"")
                });
            resultado = resultado.Distinct().ToList();
            // Realizar el Left Join y filtrar los resultados
            var resultadoAbSaldos = 
                (from abSaldos in _abSaldosCompleta
                            join guardaValores in resultado
                            on abSaldos.NumContrato equals guardaValores.NumContrato 
                            select new { saldos = abSaldos, gv = guardaValores }).Select(x=> new GuardaValores()
                            { 
                                Regional = x.saldos.Regional,
                                EstadoInegi = x.saldos.EstadoInegi,
                                Sucursal = x.saldos.Sucursal,
                                NumCte = x.saldos.NumCte,
                                Acreditado = ObtieneAcreditado(x.saldos.ApellPaterno, x.saldos.ApellMaterno, x.saldos.Nombre1, x.saldos.Nombre2, x.saldos.RazonSocial),
                                NumContrato = x.saldos.NumContrato,
                                NumCredito = x.saldos.NumCredito,
                                NumProducto = x.saldos.NumProducto,
                                CatProducto = ObtieneCatalogoDeProducto(productos, x.saldos.NumProducto??"   "),
                                TipoCreditoCont = x.saldos.TipoCreditoCont,
                                EstadoDeLaCartera = ObtieneEstadoCartera(x.saldos),
                                SldoTotContval = x.saldos.SldoTotContval,
                                EsCreditoAReportar = ObtieneEsCreditoAReportar(x.saldos),
                                Imagen = x.gv.Imagen
                            });

            _guardaValores = resultadoAbSaldos.Distinct().ToList();
            _agencias ??= _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();
            CruzaInformacionAgencias(_agencias, _guardaValores);


            var losSaldosAReportar = ObtieneSaldosFiltrados(productos);
            var faltantes = losSaldosAReportar.Where(x => !_guardaValores.Any(z => (z.NumCredito ?? "").Equals(x.NumCredito))).ToList();
            CruzaInformacionAgencias(_agencias, faltantes);


            if (File.Exists(@"C:\Demos\Palomino\GV.CSV"))
                File.Delete(@"C:\Demos\Palomino\GV.CSV");
            _ = _creaArchivoCsvService.CreaArchivoCsv(@"C:\Demos\Palomino\GV.CSV", _guardaValores);
            if (File.Exists(@"C:\Demos\Palomino\GV-Faltantes.CSV"))
                File.Delete(@"C:\Demos\Palomino\GV-Faltantes.CSV");
            _ = _creaArchivoCsvService.CreaArchivoCsv(@"C:\Demos\Palomino\GV-Faltantes.CSV", faltantes);

            if (Agencia !=0)
            {
                IEnumerable<GuardaValores> resultadoResumen = _guardaValores.Where(x => x.Sucursal == Agencia).GroupBy(x => x.NumContrato).Select(
                    x => new GuardaValores() { 
                        Regional =x.First().Regional,
                        CatRegional = x.First().CatRegional,
                        EstadoInegi = x.First().EstadoInegi,
                        Sucursal = x.First().Sucursal,
                        CatAgencia = x.First().CatAgencia,
                        NumCte = x.First().NumCte,
                        Acreditado = x.First().Acreditado,
                        NumContrato = x.Key,
                        NumProducto =x.First().NumProducto,
                        CatProducto = x.First().CatProducto,
                        EstadoDeLaCartera = x.First().EstadoDeLaCartera,
                        SldoTotContval = x.Sum(y=>y.SldoTotContval),
                        Imagen = x.First().Imagen,
                        TieneTurnoCobranza = x.Any(y=>y.TieneTurnoCobranza),
                        TieneTurnoJuridico = x.Any(y=>y.TieneTurnoJuridico),
                    });
                return resultadoResumen.ToList();
            }
            return _guardaValores;
        }

        return new List<GuardaValores>();
    }

    private static bool ObtieneEsCreditoAReportar(ABSaldosCompleta saldos)
    {
        string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');

        bool primerFiltro = !arrayProductosFiltra.Any(y => y.Equals((saldos.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase));
        bool segundoFiltro = !(saldos.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase);
        return primerFiltro && segundoFiltro;
    }

    private bool CruzaInformacionImagenes(IEnumerable<ArchivoImagenCorta> imagenes)
    {
        if (_saldosConBandera is not null)
        {
            var cruzaImagenes = (from scb in _saldosConBandera
                                join img in imagenes on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                                select new
                                {
                                    Saldos = scb,
                                    Imagenes = img
                                }).ToList();
            foreach (var cruce in cruzaImagenes) {
                cruce.Saldos.TieneImagenDirecta = true;
                cruce.Saldos.TieneImagenIndirecta = false;
            }

            var cruzaImagenesSinImagenDirecta = (from scb in _saldosConBandera.Where(x=>!x.TieneImagenDirecta)
                                                 join img in imagenes on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
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

            IEnumerable<ABSaldosFiltroCreditoDiario> saldosFiltroCreditosDiario = _saldosConBandera;
            _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoABSaldosFiltroCreditoDiario, saldosFiltroCreditosDiario);
        }

        if (_expedienteDeConsulta is not null) {
            var cruzaImagenes = (from scb in _expedienteDeConsulta
                                 join img in imagenes on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                                 select new
                                 {
                                     Saldos = scb,
                                     Imagenes = img
                                 }).ToList();
            foreach (var cruce in cruzaImagenes)
            {
                cruce.Saldos.TieneImagenDirecta = true;
                cruce.Saldos.TieneImagenIndirecta = false;
                cruce.Saldos.TieneGuardaValor = cruce.Saldos.TieneGuardaValor || (cruce.Imagenes.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase);

            }

            var cruzaImagenesSinImagenDirecta = (from scb in _expedienteDeConsulta.Where(x => !x.TieneImagenDirecta)
                                                 join img in imagenes on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
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
                cruce.Saldos.TieneGuardaValor = cruce.Saldos.TieneGuardaValor || (cruce.Imagenes.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase);
            }

            if (File.Exists(_archivoExpedientesConsultaGvCsv))
                File.Delete(_archivoExpedientesConsultaGvCsv);
            _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedientesConsultaGvCsv, _expedienteDeConsulta);
            _logger.LogInformation("Se guardo el archivo {archivoExpedienteConsulta} para una carga rápida en el futuro", _archivoExpedientesConsultaGvCsv);

        }
        return true;
    }

    public IEnumerable<ArchivoImagenCorta> CargaInformacionImagenes()
    {
        /*
        _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedientesConsulta, expedientesCreditosActivos);
        _logger.LogInformation("Se guardo el archivo {archivoExpedienteConsulta} para una carga rápida en el futuro", _archivoExpedientesConsulta);
         */
        
        // _imagenesCortas = new List<ArchivoImagenCorta>();

        FileInfo fi = new (_archivoImagenCorta);
        if (fi.Exists)
        {
            _imagenesCortas = _administraCargaConsultaService.CargaArchivoImagenCorta(_archivoImagenCorta);
            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");

            if (_saldosConBandera is not null && (!_saldosConBandera.Where(x => x.StatusImpago == true || x.StatusCarteraVencida == true || x.StatusCarteraVigente == true).Any(y => y.TieneImagenDirecta == true)))
            {
                CruzaInformacionImagenes(_imagenesCortas);
            }

            CambiaUnidad();

            return _imagenesCortas;
        }
        // IList<ArchivoImagenCorta> lista = new List<ArchivoImagenCorta>();
        IEnumerable < ArchivosImagenes > resultado = _servicioImagenes.CargaImagenesTratadas(_archivoSoloPDFProcesado);
        var resultadoCorto = resultado.Select(x => new ArchivoImagenCorta() { 
            CarpetaDestino = x.CarpetaDestino,
            Hash = x.Hash,
            Id = x.Id,
            NombreArchivo = x.NombreArchivo,
            NumPaginas = x.NumPaginas,
            NumCredito = x.NumCredito
        }).ToList();
        if (_saldosConBandera is not null)
        {
            CruzaInformacionImagenes(resultadoCorto);
        }
        _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoImagenCorta, resultadoCorto);
        _imagenesCortas = resultadoCorto;
        CambiaUnidad();
        return _imagenesCortas;
    }

    private void CambiaUnidad()
    {
        if (_imagenesCortas is not null)
        {
            foreach (var imagen in _imagenesCortas)
            {
                imagen.CarpetaDestino = (imagen.CarpetaDestino ?? "").Replace("G:\\","F:\\", StringComparison.InvariantCultureIgnoreCase).Replace(_driveOrigen, _driveDestino, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }

    private void CambiaUnidadBienesAdjudicados()
    {
        if (_imagenesCortasBienesAdjudicados is not null)
        {
            foreach (var imagen in _imagenesCortasBienesAdjudicados)
            {
                imagen.CarpetaDestino = (imagen.CarpetaDestino ?? "").Replace("G:\\","F:\\", StringComparison.InvariantCultureIgnoreCase).Replace(_driveOrigen, _driveDestino, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }

    private void CambiaUnidadExpedientesJuridico()
    {
        if (_imagenesCortasExpedientesJuridico is not null)
        {
            foreach (var imagen in _imagenesCortasExpedientesJuridico)
            {
                imagen.CarpetaDestino = (imagen.CarpetaDestino ?? "").Replace("G:\\", "F:\\", StringComparison.InvariantCultureIgnoreCase).Replace(_driveOrigen, _driveDestino, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }

    public IEnumerable<ExpCasRegiones> CalculaRegiones(IEnumerable<ExpedienteDeConsultaGv> expedientes)
    {
        IList<ExpCasRegiones> listado = new List<ExpCasRegiones>();
        var filtraExpedientes = (from exp in expedientes
                                 orderby exp.NumCredito
                                   where (exp.EsCanceladoDelDr || exp.EsOrigenDelDr ) && (exp.Region > 0 && exp.Region != 400) &&((exp.Castigo??"").Equals("S",StringComparison.InvariantCultureIgnoreCase) || (exp.Castigo ?? "").Equals("R", StringComparison.InvariantCultureIgnoreCase))
                                   select exp).ToList();
        var totalPorRegion = filtraExpedientes
                            .GroupBy(l => l.Region)
                            .Select(cl => new ExpCasRegiones { 
                                NumRegion = cl.First().Region,
                                CatRegion = cl.First().CatRegion,
                                MontoCredito = cl.Sum(c=>c.MontoCredito),
                                InteresCont = cl.Sum(c=>c.InterCont),
                                TotalActivos = cl.Count(c=>c.EsSaldosActivo),
                                TotalCancelados = cl.Count(c=>c.EsCancelado),
                                TotalConArqueos = cl.Count(c=>c.TieneArqueo),
                                TotalExpCastigados = cl.Count(c=>(c.Castigo??"").Equals("S", StringComparison.OrdinalIgnoreCase)),
                                TotalReportos = cl.Count(c => (c.Castigo ?? "").Equals("R", StringComparison.OrdinalIgnoreCase))                                                                
                            }).ToList();
        ((List<ExpCasRegiones>)listado).AddRange(totalPorRegion);
        /// TODO: Considerar un renglon de totales
        return listado.ToList();
    }

    public IEnumerable<ExpCasAgencias> CalculaAgencias(IEnumerable<ExpedienteDeConsultaGv> expedientes, int region)
    {
        IList<ExpCasAgencias> listado = new List<ExpCasAgencias>();
        var filtraExpedientes = (from exp in expedientes
                                 orderby exp.NumCredito
                                 where (exp.EsCanceladoDelDr || exp.EsOrigenDelDr ) && (exp.Region > 0 && exp.Region != 400) && ((exp.Castigo ?? "").Equals("S", StringComparison.InvariantCultureIgnoreCase) || (exp.Castigo ?? "").Equals("R", StringComparison.InvariantCultureIgnoreCase)) && (exp.Region == region)
                                 select exp).ToList();
        var totalPorAgencia = filtraExpedientes
                            .GroupBy(l => l.Agencia)
                            .Select(cl => new ExpCasAgencias
                            {
                                NumRegion = cl.First().Region,
                                NumAgencia = cl.First().Agencia,
                                CatAgencia = cl.First().CatAgencia,
                                MontoCredito = cl.Sum(c => c.MontoCredito),
                                InteresCont = cl.Sum(c => c.InterCont),
                                TotalActivos = cl.Count(c => c.EsSaldosActivo),
                                TotalCancelados = cl.Count(c => c.EsCancelado),
                                TotalConArqueos = cl.Count(c => c.TieneArqueo),
                                TotalExpCastigados = cl.Count(c => (c.Castigo ?? "").Equals("S", StringComparison.OrdinalIgnoreCase)),
                                TotalReportos = cl.Count(c => (c.Castigo ?? "").Equals("R", StringComparison.OrdinalIgnoreCase))
                            }).ToList();
        ((List<ExpCasAgencias>)listado).AddRange(totalPorAgencia);
        /// TODO: Considerar un renglon de totales
        return listado.ToList();
    }

    public IEnumerable<ExpCasExpedientes> CalculaExpedientes(IEnumerable<ExpedienteDeConsultaGv> expedientes, int agencia)
    {
        IList<ExpCasExpedientes> listado = new List<ExpCasExpedientes>();
        var filtraExpedientes = (from exp in expedientes
                                 orderby exp.NumCredito
                                 where (exp.EsCanceladoDelDr || exp.EsOrigenDelDr ) && (exp.Region > 0 && exp.Region != 400) && ((exp.Castigo ?? "").Equals("S", StringComparison.InvariantCultureIgnoreCase) || (exp.Castigo ?? "").Equals("R", StringComparison.InvariantCultureIgnoreCase)) && (exp.Agencia == agencia)
                                 select exp).ToList();
        var listaDeExpedientes = filtraExpedientes
                            .Select(cl => new ExpCasExpedientes
                            {
                                NumRegion = cl.Region,
                                NumAgencia = cl.Agencia,
                                NumCredito = cl.NumCredito,
                                Acreditado = cl.Acreditado,
                                MontoCredito = cl.MontoCredito,
                                InteresCont = cl.InterCont,
                                EstaActivo = cl.EsSaldosActivo,
                                EstaCancelado = cl.EsCancelado,
                                CuentaConArqueo = cl.TieneArqueo,
                                Castigo = (cl.Castigo??"").Equals("S", StringComparison.InvariantCultureIgnoreCase)?"Castigado": string.Empty,
                                Reporto = (cl.Castigo ?? "").Equals("R", StringComparison.InvariantCultureIgnoreCase) ? "Reporto" : string.Empty
                            }).ToList();
        ((List<ExpCasExpedientes>)listado).AddRange(listaDeExpedientes);
        /// TODO: Considerar un renglon de totales
        return listado.ToList();
    }

    public CreditosTotales CalculaCreditosTotales()
    {
        CreditosTotales creditosTotales = new ();
        if (_expedienteDeConsulta is not null)
        {
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar == true).ToList();
            creditosTotales.TotalDeCreditos = creditosAReportar.Count;
            creditosTotales.CreditosVigentes = creditosAReportar.Count(x=> x.StatusCarteraVigente == true);
            creditosTotales.Impagos = creditosAReportar.Count(x=> x.StatusImpago == true);
            creditosTotales.CarteraVencida = creditosAReportar.Count(x=> x.StatusCarteraVencida == true);
        }
        return creditosTotales;
    }

    public IEnumerable<ResumenDeCreditos> CalculaCreditosRegiones()
    {
        if (_expedienteDeConsulta is not null)
        {
            IList<ResumenDeCreditos> resultado = new List<ResumenDeCreditos>();
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar == true).ToList();
            var grupoPorRegion = creditosAReportar.GroupBy(x => x.Region).Select(y => new ResumenDeCreditos() { 
                Region = y.First().Region,
                CatRegion = y.First().CatRegion,
                CantidadDeCreditosVigente = y.Count(z=>z.StatusCarteraVigente == true),
                CantidadDeCreditosImpago = y.Count(z => z.StatusImpago == true),
                CantidadDeCreditosVencida = y.Count(z => z.StatusCarteraVencida == true),
                TotalDeCreditos = y.Count() 
            });
            foreach (var reporteRegiones in grupoPorRegion)
            {
                var datosRegion = creditosAReportar.Where(x => x.Region == reporteRegiones.Region);
                reporteRegiones.CantidadClientesImpago = datosRegion.Where(x=>x.StatusImpago).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.CantidadClientesVencida = datosRegion.Where(x => x.StatusCarteraVencida).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.CantidadClientesVigente = datosRegion.Where(x => x.StatusCarteraVigente).GroupBy(x => x.NumCliente).Count();
                reporteRegiones.TotalDeClientes = datosRegion.GroupBy(x => x.NumCliente).Count();
                reporteRegiones.SaldosCarteraImpago = datosRegion.Where(x => x.StatusImpago).Sum(z => z.SldoTotContval);
                reporteRegiones.SaldosCarteraVencida = datosRegion.Where(x => x.StatusCarteraVencida).Sum(z => z.SldoTotContval);
                reporteRegiones.SaldosCarteraVigente = datosRegion.Where(x => x.StatusCarteraVigente).Sum(z=>z.SldoTotContval);
                reporteRegiones.SaldosTotal = datosRegion.Sum(z => z.SldoTotContval);
                reporteRegiones.ICV = (reporteRegiones.SaldosCarteraVencida / reporteRegiones.SaldosTotal) * 100;
                resultado.Add(reporteRegiones);
            }


            var registroTotales = new ResumenDeCreditos() 
            { 
                Region = 0,
                CatRegion = "Totales",
                CantidadDeCreditosVigente = resultado.Sum(x=> x.CantidadDeCreditosVigente),
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

    public IEnumerable<CreditosRegion> CalculaCreditosRegion(int region)
    {
        if (_expedienteDeConsulta is not null && region != 0)
        {
            IList<CreditosRegion> resultado = new List<CreditosRegion>();
            var creditosAReportar = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar && x.Region == region).ToList();
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
                TotalDeCreditos = resultado.Sum(x=> x.TotalDeCreditos),
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
            else {
                registroTotales.ICV = 100;
            }
            resultado.Add(registroTotales);
            return resultado.ToList();

        }

        return new List<CreditosRegion>();
    }

    public IEnumerable<DetalleCreditos> CalculaCreditosAgencia(int agencia)
    {
        if (_expedienteDeConsulta is not null && agencia != 0)
        {
            var detalleAgencia = _expedienteDeConsulta.Where(x => x.EsCreditoAReportar && x.Agencia == agencia).ToList();

            var resultadoDetalleCreditos = detalleAgencia.Select(x => new DetalleCreditos()
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
                TieneImagenIndirecta = x.TieneImagenIndirecta
            }
            ).ToList();
            int totalCred = resultadoDetalleCreditos.Count;
            int totalCtes = resultadoDetalleCreditos.GroupBy(x=>x.Acreditado).Count();
            decimal totalSaldoCarteraVigente = resultadoDetalleCreditos.Sum(x => x.SaldoCarteraVigente);
            decimal totalSaldoCarteraImpagos = resultadoDetalleCreditos.Sum(x => x.SaldoCarteraImpagos);
            decimal totalSaldosCarteraVencida = resultadoDetalleCreditos.Sum(x => x.SaldosCarteraVencida);
            decimal totalSaldosTotal = resultadoDetalleCreditos.Sum(x => x.SaldosTotal);

            DetalleCreditos total = new()
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
        return new List<DetalleCreditos>();
    }

    public IEnumerable<ArchivoImagenCorta> BuscaImagenes(string numeroDeCredito, bool directas = false)
    {
        IList<ArchivoImagenCorta> listaImagenesEncontradas = new List<ArchivoImagenCorta>();
        if (_imagenesCortas is not null && _imagenesCortas.Any() && !string.IsNullOrEmpty(numeroDeCredito))
        {
            listaImagenesEncontradas = _imagenesCortas.OrderByDescending(y => y.NumPaginas).Where(x => QuitaCastigo(x.NumCredito).Equals(QuitaCastigo(numeroDeCredito), StringComparison.OrdinalIgnoreCase)).ToList();

            #region Busco por el contrato en vez del crédito para traer GV del contrato
            string contrato = QuitaCastigoContrato(numeroDeCredito);
            var contratoYGV = _imagenesCortas.OrderByDescending(y => y.NumPaginas).Where(x => QuitaCastigo(x.NumCredito).Equals(contrato, StringComparison.OrdinalIgnoreCase)).ToList();
            ((List<ArchivoImagenCorta>)listaImagenesEncontradas).AddRange(contratoYGV);
            #endregion

            if (!listaImagenesEncontradas.Any() || !directas)
            { 
                var imagenesAdicionales = _imagenesCortas.OrderByDescending(y => y.NumPaginas).Where(x => QuitaCastigoIndirecto(x.NumCredito).Equals(QuitaCastigoIndirecto(numeroDeCredito), StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
            }
            // Se eliminan las imagenes duplicadas
            listaImagenesEncontradas = listaImagenesEncontradas.GroupBy(x => x.Hash).Select(g => g.First()).ToList();
            // Se ordena el resultado de mayor numero de páginas al menor número de páginas
            listaImagenesEncontradas = listaImagenesEncontradas.OrderByDescending(y => y.NumPaginas).ToList();
        }
        return listaImagenesEncontradas;
    }

}
