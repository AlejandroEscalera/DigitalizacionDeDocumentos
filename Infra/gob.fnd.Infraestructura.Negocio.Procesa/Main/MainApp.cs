using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Mancomunados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Descarga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using gob.fnd.Dominio.Digitalizacion.Negocio.Tratamientos;
using gob.fnd.Infraestructura.Negocio.Procesa.Control;
using gob.fnd.Infraestructura.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using Jaec.Helper.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Main
{
    public class MainApp : IMainControlApp
    {
        private readonly ILogger<MainApp> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly IAdministraImagenesService _administraImagenesService;
        private readonly IServicioDirToXls _servicioDirToXls;
        private readonly IAdministraCargaCorteDiario _administraCargaCorteDiario;
        private readonly IAdministraCartera _administraCartera;
        private readonly IServicioMinistracionesMesa _servicioMinistracionesMesa;
        private readonly IAdministraCargaCorteDiario _administraCargaCorteDiarioService;
        private readonly ICreaArchivoCsv _creaArchivoCsv;
        private readonly IAdministraCargaConsulta _administraCargaConsultaService;
        private readonly IAdministraCargaCreditosCancelados _administraCreditosCanceladosService;
        private readonly IAnalizaResultadoOCR _analizaResultadoOCRService;
        private readonly IObtieneCorreosAgentes _obtieneCorreosAgentes;
        private readonly IAnalizaInformacionBienesAdjudicados _analizaInformacionBienesAdjudicadosService;
        private readonly ITratamientos _tratamientosService;
        //private readonly IAdministraTratamientoBase _administraTratamientoBaseService;
        private readonly IOperacionesTratamientos _operacionesTratamientosService;
        private readonly IDescargaExpedientes _descargaExpedientesService;
        private readonly IAdministraCargaBienesAdjudicados _administraCargaBienesAdjudicados;
        private readonly bool _soloCargaImagenesProcesadas;
        private readonly bool _preparaImagenesComplementarias;
        private readonly bool _preparaImagenesComplementariasSegundaParte;
        private readonly string _archivoErroresDescarga;
        private readonly string _archivoABSaldosDiarioDestino;
        private readonly string _archivoBienesAdjudicados;
        private readonly string _archivoExpedientesConsultaGv;

        private readonly bool _soloProcesaErroresDescarga;
        private readonly bool _soloCreaResultadoDescarga;
        private readonly bool _soloPreparaControlOcr;
        private readonly bool _cargaSoloPDF;
        private readonly bool _detectaMancomunados;

        private readonly bool _cargaMancomunadasVsActivos;
        private readonly bool _cargaPF1RevisionFaltantesCancelados;

        private readonly bool _cargaImagenesBienesAdjudicados;
        private readonly bool _soloOCRBienesAdjudicadosContable;
        private readonly bool _soloCargaFaltantes;
        private readonly bool _soloCargaFaltantesUltimosExpedientes;
        private readonly bool _soloCargaImagenesCreditosLiquidados;
        private readonly bool _soloCargaCruceTratamientos;
        private readonly bool _cargaImagenesUltimosExpedientes;
        private readonly bool _soloCargaCreditosBienesAdjudicados;


        //        private readonly string _archivoABSaldosDiarioDestino;


        #region Información del trabajo a atender
        private readonly int _threadId;
        private readonly int _trabajoInicial;
        private readonly int _trabajoFinal;
        private readonly bool _seCruzaInformacionImagenesABSaldosCredito;

        private readonly bool _analizaResultadoOcr;
        private readonly bool _realizaCrucesInformacionBienesAdjudicados;

        #endregion
        #region sec
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly string _usuario;
        private readonly SecureString _password;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private readonly bool _creaInfoImagenesDeListaDeExpedientes;
        #endregion
        public MainApp(ILogger<MainApp> logger, IConfiguration configuration, IServicioImagenes servicioImagenes, IAdministraImagenesService administraImagenesService,
            IServicioDirToXls servicioDirToXls, IAdministraCargaCorteDiario administraCargaCorteDiario, IAdministraCartera administraCartera, IServicioMinistracionesMesa servicioMinistracionesMesa, IAdministraCargaCorteDiario administraCargaCorteDiarioService,
            ICreaArchivoCsv creaArchivoCsv,
            IAdministraCargaConsulta administraCargaConsultaService,
            IAdministraCargaCreditosCancelados administraCreditosCanceladosService,
            IAnalizaResultadoOCR analizaResultadoOCRService,
            IObtieneCorreosAgentes obtieneCorreosAgentes,
            IAnalizaInformacionBienesAdjudicados analizaInformacionBienesAdjudicadosService,
            ITratamientos tratamientosService,
            // IAdministraTratamientoBase administraTratamientoBaseService,
            IOperacionesTratamientos operacionesTratamientosService,
            IDescargaExpedientes descargaExpedientesService,
            IAdministraCargaBienesAdjudicados administraCargaBienesAdjudicados
            )
        {
            _logger = logger;
            _configuration = configuration;
            _servicioImagenes = servicioImagenes;
            _administraImagenesService = administraImagenesService;
            _servicioDirToXls = servicioDirToXls;
            _administraCargaCorteDiario = administraCargaCorteDiario;
            _administraCartera = administraCartera;
            _servicioMinistracionesMesa = servicioMinistracionesMesa;
            _administraCargaCorteDiarioService = administraCargaCorteDiarioService;
            _creaArchivoCsv = creaArchivoCsv;
            _administraCargaConsultaService = administraCargaConsultaService;
            _administraCreditosCanceladosService = administraCreditosCanceladosService;
            _analizaResultadoOCRService = analizaResultadoOCRService;
            _obtieneCorreosAgentes = obtieneCorreosAgentes;
            _analizaInformacionBienesAdjudicadosService = analizaInformacionBienesAdjudicadosService;
            _tratamientosService = tratamientosService;
            //_administraTratamientoBaseService = administraTratamientoBaseService;
            _operacionesTratamientosService = operacionesTratamientosService;
            _descargaExpedientesService = descargaExpedientesService;
            _administraCargaBienesAdjudicados = administraCargaBienesAdjudicados;
            _soloCargaImagenesProcesadas = (_configuration.GetValue<string>("soloCargaImagenesProcesadas") ?? "").Equals("Si");
            _archivoErroresDescarga = (_configuration.GetValue<string>("archivoErroresDescarga") ?? "");
            _soloProcesaErroresDescarga = true;
            #region sec
            _password = (_configuration.GetValue<string>("contrasenia") ?? "").ToSecureString();
            _usuario = (_configuration.GetValue<string>("usuario") ?? "");
            #endregion
            _archivoABSaldosDiarioDestino = _configuration.GetValue<string>("archivoABSaldosDiarioDestino") ?? "";
            _archivoABSaldosDiarioDestino = String.Format(_archivoABSaldosDiarioDestino, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _threadId = _configuration.GetValue<int>("threadId");
            _trabajoInicial = _configuration.GetValue<int>("TrabajoInicial");
            _trabajoFinal = _configuration.GetValue<int>("TrabajoFinal") +1;
            _preparaImagenesComplementarias = true;
            _soloCreaResultadoDescarga = true;
            _cargaSoloPDF = true;
            _preparaImagenesComplementariasSegundaParte = true;
            _soloPreparaControlOcr = true;
            _seCruzaInformacionImagenesABSaldosCredito = true;
            _detectaMancomunados = true;
            _cargaMancomunadasVsActivos = true;
            _cargaPF1RevisionFaltantesCancelados = true;
            // Es para copiar los PDF's de la 14 a la 11
            _cargaImagenesBienesAdjudicados = true;

            #region Para el analisis de OCR
            _soloOCRBienesAdjudicadosContable = false;
            // El OCR se ejecuta en el otro proyecto
            _analizaResultadoOcr = true;
            #endregion

            _realizaCrucesInformacionBienesAdjudicados = true;

            _soloCargaFaltantes = true;
            _soloCargaImagenesCreditosLiquidados = true;

            _soloCargaCruceTratamientos = true;
            _soloCargaFaltantesUltimosExpedientes = true;
            _cargaImagenesUltimosExpedientes = true;
            _creaInfoImagenesDeListaDeExpedientes = true;
            _soloCargaCreditosBienesAdjudicados = true;
            _archivoBienesAdjudicados = _configuration.GetValue<string>("archivoBienesAdjudicados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados.csv";
            _archivoExpedientesConsultaGv = _configuration.GetValue<string>("archivoExpedientesConsultaGv") ?? "";
        }

        private static string QuitaCastigo(string? origen)
        {
            try
            {
                if (origen is null)
                    return "000000000000000000";

                if (origen.Length < 18)
                    return "000000000000000000";
                string digito = origen.Substring(3, 1);
                string digito2 = origen.Substring(4, 1);
                string nuevoOrigen = origen;
                if ((digito == "1" || digito == "9") && (digito2 != "0")) // || digito =="5" || digito == "6"
                {
                    nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
                    // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
                }
                return nuevoOrigen;
            }
            catch 
            {
                return "000000000000000000";
            }
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

        private static IEnumerable<ABSaldosFiltroCreditoDiario> LlenaInformacionABSaldosFiltroCredito(IEnumerable<ABSaldosCompleta> abSaldosCompleta)
        {
            string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');
            var filtaABSaldosCompletaPorProducto = abSaldosCompleta.Where(x => !arrayProductosFiltra.Any(y => y.Equals((x.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase))).ToList();
            filtaABSaldosCompletaPorProducto = filtaABSaldosCompletaPorProducto.Where(x => !(x.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase)).ToList();

            var saldosConBandera = filtaABSaldosCompletaPorProducto.Select(x =>
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
            return saldosConBandera;
        }

        private static string[] ObtieneTratamientoOrigen(string numCredito, IEnumerable<ConversionTratamientos> tratamientos) 
        { 
            IList<string> listaCreditos = new List<string>();
            var listaTratamientos = tratamientos.Where(x => (x.Origen??"").Equals(numCredito)).ToList(); 
            foreach (var tratamiento in listaTratamientos)
            {
                string numCreditoNuevo = tratamiento.Destino??"";
                listaCreditos.Add(numCreditoNuevo);
                if (numCreditoNuevo.Length> 3 && numCreditoNuevo.Substring(3, 1).Equals("8"))
                {
                    var listaCreditosRecursivo = ObtieneTratamientoOrigen(numCreditoNuevo, tratamientos).ToList();
                    foreach (var creditoRecursivo in listaCreditosRecursivo)
                    {
                        listaCreditos.Add(creditoRecursivo);
                    }
                }
            }
            return listaCreditos.Distinct().ToArray();
        }

        private static string[] ObtieneTratamientoDestino(string numCreditoOrigen, IEnumerable<ConversionTratamientos> tratamientos)
        {
            
            IList<string> listaCreditos = new List<string>();
            if (string.IsNullOrEmpty(numCreditoOrigen))
                return listaCreditos.ToArray();
            var listaTratamientos = tratamientos.Where(x => (x.Destino ?? "").Equals(numCreditoOrigen)).ToList();
            foreach (var tratamiento in listaTratamientos)
            {
                string numCreditoDestino = tratamiento.Origen ?? "";
                listaCreditos.Add(numCreditoDestino);
                var listaCreditosRecursivo = ObtieneTratamientoDestino(numCreditoDestino, tratamientos).ToList();
                foreach (var creditoRecursivo in listaCreditosRecursivo)
                {
                    listaCreditos.Add(creditoRecursivo);
                }
            }
            return listaCreditos.Distinct().ToArray();
        }

        private static bool TieneTurno(string? urlArchivo, string? nombreArchivo)
        {
            if ((urlArchivo ?? "").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase))
                return false;
            if ((nombreArchivo ?? "").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if ((urlArchivo ?? "").Contains("Turno", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((urlArchivo ?? "").Contains("Turnado", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains(" T ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains(" T_", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("(T)", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("_T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains(" T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("-T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("-T ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("0T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("1T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("2T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("3T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("4T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("5T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("6T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("7T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("8T.", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("9T.", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if ((nombreArchivo ?? "").Contains("0TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("1TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("2TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("3TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("4TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("5TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("6TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("7TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("8TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("9TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains(" TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("_TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("-TJ", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if ((nombreArchivo ?? "").Contains("0T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("1T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("2T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("3T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("4T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("5T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("6T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("7T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("8T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("9T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains(" T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("_T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if ((nombreArchivo ?? "").Contains("-T-J", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public void Run()
        {

            IEnumerable<ArchivosImagenes> imagenes;

            #region Carga Bienes Adjudicados y revisa estado de los creditos
            if (_soloCargaCreditosBienesAdjudicados)
            {
                string fnLqTramitados = @"C:\OD\OneDrive - FND\Entrega\Liquidados\LQTramitados.csv";
                string[] LQTramitados = File.ReadAllLines(fnLqTramitados);
                string fnLqPorTramitar = @"C:\OD\OneDrive - FND\Entrega\Liquidados\LQPorTramitar.csv";
                string[] LQPorTramitar = File.ReadAllLines(fnLqPorTramitar);

                var detalleBienesAdjudicadosCarga = _administraCargaBienesAdjudicados.CargaBienesAdjudicados(_archivoBienesAdjudicados);
                IEnumerable<ExpedienteDeConsultaGv> resultado = _administraCargaConsultaService.CargaExpedienteDeConsultaGv(_archivoExpedientesConsultaGv);

                IList<CruceLiquidadosBienesAdjudicados> listaCruceCreditos = new List<CruceLiquidadosBienesAdjudicados>();
                foreach (var detalleBienesAdjudicados in detalleBienesAdjudicadosCarga)
                {
                    char[] divideCreditos = { ',','¬','\n' };
                    IEnumerable<string> creditos = (detalleBienesAdjudicados.NumCredito??"").Split(divideCreditos).Where(x=>!string.IsNullOrWhiteSpace(x)).ToArray();
                    foreach (var credito in creditos)
                    {
                        CruceLiquidadosBienesAdjudicados creditoIndividual = new()
                        {
                            NumExpediente = detalleBienesAdjudicados.ExpedienteElectronico,
                            NumCredito = credito,
                            Adjudicado = detalleBienesAdjudicados.Acreditado
                        };
                        
                        var creditoEncontrado = resultado.FirstOrDefault(x => QuitaCastigo(x.NumCredito) == QuitaCastigo(credito));

                        if (creditoEncontrado is null)
                        {
                            creditoIndividual.TieneInfo = false;
                            creditoIndividual.TieneImg = false;
                        }
                        else {
                            creditoIndividual.TieneInfo = true;
                            creditoIndividual.TieneImg = creditoEncontrado.TieneImagenDirecta;
                        }

                        listaCruceCreditos.Add(creditoIndividual);

                    }

                }

                _logger.LogInformation("Terminó el proceso de los bienes adjudicados, ahora se cruzan con la información de liquidados existentes y liquidados recien pedidos");
                // SeSolicitoLiquidado
                // SeTieneLiquidado
                var cruceConImagenes = (from img in LQTramitados
                                        join abs in listaCruceCreditos on QuitaCastigo(img) equals QuitaCastigo(abs.NumCredito)
                                        select new { img, abs }).OrderBy(x => x.img);
                foreach (var item in cruceConImagenes.Where(x=>x.abs is not null))
                {
                    item.abs.SeTieneLiquidado = true;
                }

                var cruceConImagenesDos = (from img in LQPorTramitar
                                        join abs in listaCruceCreditos on QuitaCastigo(img) equals QuitaCastigo(abs.NumCredito)
                                        select new { img, abs }).OrderBy(x => x.img);
                foreach (var item in cruceConImagenesDos.Where(x => x.abs is not null))
                {
                    item.abs.SeSolicitoLiquidado = true;
                }

                string archivoBienesAdjudicadosDestino = @"C:\Demos\BienesAdjudicados.csv";
                if (File.Exists(archivoBienesAdjudicadosDestino))
                    File.Delete(archivoBienesAdjudicadosDestino);
                _creaArchivoCsv.CreaArchivoCsv(archivoBienesAdjudicadosDestino, listaCruceCreditos.OrderBy(x=>x.NumCredito).Distinct());
                
                return;
            }

            #endregion 

            #region Crea información de imagenes basada en lista de expedientes
            if (_creaInfoImagenesDeListaDeExpedientes)
            {
                _descargaExpedientesService.DescargaListaDeExpedientes(@"C:\202302 Digitalizacion\8. Contratos\230621ListaDeContratos.txt", @"F:\EXP");
                return;
            }
            #endregion

            #region Copia las imágenes en otra carpeta, y convierte .zip /.msg a pdf
            if (_cargaImagenesUltimosExpedientes)
            {
                // imagenes = _servicioImagenes.CargaImagenesTratadas(archivoImagenUETodas);
                _administraImagenesService.ProcesaHiloYTrabajo(114, 1);
                return;
            }
            #endregion

            #region Carga de imagenes faltantes de ultimos expedientes
            if (_soloCargaFaltantesUltimosExpedientes)
            {
                string archivoFaltantePorProcesar = "C:\\202302 Digitalizacion\\5. Imagenes sin procesar\\UltimExpedientes\\ue.xlsx"; //
                var imagenesFaltantes = _servicioDirToXls.ObtieneImagenesComplementariasTerceraParte(archivoFaltantePorProcesar, 628490).Where(x=> !(x.NumCredito ?? "").Equals("000000000000000000")).ToList();
                _logger.LogInformation("Se tienen {cantidadImagenes} imagenes faltantes", imagenesFaltantes.Count);
                var imagenesConCobranza = imagenesFaltantes.Where(x => (x.UrlArchivo??"").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase)).ToList();
                _logger.LogInformation("Se tienen {cantidadImagenes} imagenes de cobranza", imagenesConCobranza.Count);
                var imagenesConTurno = imagenesFaltantes.Where(x => TieneTurno(x.UrlArchivo,x.UrlArchivo)).ToList();
                _logger.LogInformation("Se tienen {cantidadImagenes} imagenes de cobranza", imagenesConTurno.Count);
                var imagenesRestantes = imagenesFaltantes.Where(x => !(TieneTurno(x.UrlArchivo, x.UrlArchivo) || (x.UrlArchivo ?? "").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase))).ToList();
                _logger.LogInformation("Se tienen {cantidadImagenes} imagenes restantes", imagenesRestantes.Count);

                foreach (var imagen in imagenesFaltantes)
                {
                    imagen.ThreadId = 114;
                    imagen.Job = 1;
                    imagen.UsuarioDeModificacion = (imagen.UrlArchivo ?? "").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase)?"Si":"";
                    imagen.ClasifMesa = TieneTurno(imagen.UrlArchivo, imagen.UrlArchivo) ? "Si" : "";
                }

                string archivoImagenUETodas = @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesUETodas.xlsx";
                if (File.Exists(archivoImagenUETodas))
                    File.Delete(archivoImagenUETodas);
                _servicioImagenes.GuardarImagenes(imagenesFaltantes, archivoImagenUETodas);

                string archivoImagenConCobranza = @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesUEConCobranza.xlsx";
                if (File.Exists(archivoImagenConCobranza))
                    File.Delete(archivoImagenConCobranza);
                _servicioImagenes.GuardarImagenes(imagenesConCobranza, archivoImagenConCobranza);
                string archivoImagenConTurno = @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesUEConTurno.xlsx";
                if (File.Exists(archivoImagenConTurno))
                    File.Delete(archivoImagenConTurno);
                _servicioImagenes.GuardarImagenes(imagenesConTurno, archivoImagenConTurno);                
                string archivoImagenRestantes = @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesUERestantes.xlsx";
                if (File.Exists(archivoImagenRestantes))
                    File.Delete(archivoImagenRestantes);
                _servicioImagenes.GuardarImagenes(imagenesRestantes, archivoImagenRestantes);

                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenesFaltantes);

                return;
            }

            #endregion


            #region Maestro para conseguir base de tratamientos
            if (_soloCargaCruceTratamientos) {

                string archivoTratamientosActivos = "C:\\202302 Digitalizacion\\1. AB Saldos\\Tratamientos\\TratamientosActivos.xlsx";
                IList<ConversionTratamientos> listaTratamientosActivos = new List<ConversionTratamientos>();
                var origenListaTratamientosActivos = _tratamientosService.GetConversionTratamientos(archivoTratamientosActivos);
                //((List<ConversionTratamientos>)listaTratamientosActivos).AddRange(origenListaTratamientosActivos);

                string primerArchivoTratamientos = "C:\\202302 Digitalizacion\\1. AB Saldos\\Tratamientos\\PrimerAnalisis.xlsx";
                IList<ConversionTratamientos> listaTratamientosMultiples = new List<ConversionTratamientos>();
                var origenListaTratamientosMultiples = _tratamientosService.GetConversionTratamientos(primerArchivoTratamientos);
                ((List < ConversionTratamientos > )listaTratamientosMultiples).AddRange(origenListaTratamientosMultiples);

                string archivoTratamientosCancelados = "C:\\202302 Digitalizacion\\1. AB Saldos\\Tratamientos\\TratamientosYCancelados.xlsx";
                IList<ConversionTratamientos> listaTratamientosCancelados = new List<ConversionTratamientos>();
                var origenListaTratamientosCancelados = _tratamientosService.GetConversionTratamientos(archivoTratamientosCancelados);
                ((List<ConversionTratamientos>)listaTratamientosCancelados).AddRange(origenListaTratamientosCancelados);

                foreach (var tratamiento in origenListaTratamientosActivos)
                {
                    ConversionTratamientos? encontado;
                    encontado = ObtieneTratamiento(listaTratamientosMultiples, listaTratamientosCancelados, tratamiento);
                }

                _logger.LogError("Tratamientos no encontrados {noEncontrados}", origenListaTratamientosActivos.Where(x => string.IsNullOrEmpty(x.Destino)).Count());
                _logger.LogInformation("Tratamientos encontrados {siEncontrados}", origenListaTratamientosActivos.Where(x => !string.IsNullOrEmpty(x.Destino)).Count());

                // Sumo ambos tratamientos, los multiples y los cancelados y luego genero un solo archivo sin duplicados
                foreach (var tratamiento in listaTratamientosMultiples)
                {
                    tratamiento.Id = 0;
                    tratamiento.NumCte = (tratamiento.NumCte??"").Trim();
                    tratamiento.Origen = (tratamiento.Origen ?? "").Trim();
                    tratamiento.Destino = (tratamiento.Destino ?? "").Trim();
                }
                foreach (var tratamiento in listaTratamientosCancelados)
                {
                    tratamiento.Id = 0;
                    tratamiento.NumCte = (tratamiento.NumCte ?? "").Trim();
                    tratamiento.Origen = (tratamiento.Origen ?? "").Trim();
                    tratamiento.Destino = (tratamiento.Destino ?? "").Trim();
                }
                var listaTratamientos = listaTratamientosMultiples.Union(listaTratamientosCancelados).ToList().Where(x=>!(x.Destino??"").Equals(x.Origen)).OrderBy(x=>x.Origen).Distinct().ToList();
                int i = 1;
                foreach (var tratamiento in listaTratamientos.OrderBy(x=>x.Origen))
                {
                    if (!listaTratamientosActivos.Any(x => (x.Origen ?? "").Equals(tratamiento.Origen) && (x.Destino ?? "").Equals(tratamiento.Destino)) )
                    {
                        tratamiento.Id = i;
                        listaTratamientosActivos.Add(tratamiento);
                        i++;
                    }
                }

                //_creaArchivoCsv.CreaArchivoCsv(@"C:\202302 Digitalizacion\2. Saldos procesados\Tratamientos\TratamientosBase.csv", listaTratamientosActivos);
                /*

                var validaCargaTratamientosActivos = _administraTratamientoBaseService.GetConversionTratamientosCsv(@"C:\202302 Digitalizacion\2. Saldos procesados\Tratamientos\TratamientosBase.csv");
                _logger.LogInformation("Tratamientos encontrados {siEncontrados}", validaCargaTratamientosActivos.Count());

                _logger.LogInformation("Se busca de 1 a 1 \"202800000140000001\"");
                */
                var listaCreditos = _operacionesTratamientosService.ObtieneTratamientosOrigen("202800000140000001");
                foreach (var credito in listaCreditos)
                {
                    _logger.LogInformation("Credito {credito}", credito);
                }

                _logger.LogInformation("Se busca de 1 a muchos \"210800000330000001\"");
                listaCreditos = _operacionesTratamientosService.ObtieneTratamientosOrigen("210800000330000001");
                foreach (var credito in listaCreditos)
                {
                    _logger.LogInformation("Credito {credito}", credito);
                }


                _logger.LogInformation("Se inversa de 1 a 1 \"210600014270000001\"");
                listaCreditos = _operacionesTratamientosService.ObtieneTratamientos("210600014270000001");
                foreach (var credito in listaCreditos)
                {
                    _logger.LogInformation("Credito {credito}", credito);
                }
                _logger.LogInformation("Se inversa de 1 a muchos \"611700003790000004\"");
                listaCreditos = _operacionesTratamientosService.ObtieneTratamientos("611700003790000004");
                foreach (var credito in listaCreditos)
                {
                    _logger.LogInformation("Credito {credito}", credito);
                }

                return;
            }
            #endregion

            #region Carga de imagenes faltantes
            if (_soloCargaFaltantes)
            {
                string archivoFaltantePorProcesar = "C:\\202302 Digitalizacion\\5. Imagenes sin procesar\\Comps2\\faltantes.xlsx"; //
                var imagenesFaltantes = _servicioDirToXls.ObtieneImagenesComplementariasSegundaParte(archivoFaltantePorProcesar);

                string filename = @"C:\Demos\PorChecar.txt";
                string[] creditos = File.ReadAllLines(filename);
                StringBuilder sb = new();
                int encontrados = 0;
                int noEncontrados = 0;

                IEnumerable<CorreosAgencia> agencias = _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();

                var abSaldosCompleta = LlenaInformacionABSaldosFiltroCredito(_administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino));
                var cruceConImagenes = (from img in creditos
                                        join abs in abSaldosCompleta on QuitaCastigoIndirecto(img) equals QuitaCastigoIndirecto(abs.NumCredito)
                                        select new { img, abs }).OrderBy(x => x.img);

                foreach (var credito in cruceConImagenes)
                {
                    if (!imagenesFaltantes.Any(x => QuitaCastigoIndirecto(x.NumCredito).Equals(QuitaCastigoIndirecto(credito.img), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var agencia = agencias.Where(x => x.NoAgencia == credito.abs.Sucursal).FirstOrDefault();
                        _logger.LogError("No se encontro el credito {credito} {sinCastigo}", credito.img, QuitaCastigo(credito.img));
                        sb.Append(credito.img);
                        sb.Append('|');
                        sb.Append(credito.abs.Regional);
                        sb.Append('|');
                        sb.Append(agencia?.Region);
                        sb.Append('|');
                        sb.Append(credito.abs.Sucursal);
                        sb.Append('|');
                        sb.Append(agencia?.Agencia);
                        sb.Append('|');
                        sb.Append(credito.abs.StatusCarteraVigente);
                        sb.Append('|');
                        sb.Append(credito.abs.StatusImpago);
                        sb.Append('|');
                        sb.Append(credito.abs.StatusCarteraVencida);
                        sb.Append("\r\n");
                        noEncontrados++;
                    }
                    else
                    {
                        encontrados++;
                        _logger.LogInformation("Lo encontró {credito} {sinCastigo}", credito.img, QuitaCastigo(credito.img));
                    }
                }

                _logger.LogInformation("Se encontraron {encontrados} y no se encontraron {noEncontrados}", encontrados, noEncontrados);
                string archivoDestino = "C:\\Demos\\NoEncontrados02.txt";
                if (File.Exists(archivoDestino))
                {
                    File.Delete(archivoDestino);
                }
                File.WriteAllText(archivoDestino, sb.ToString());

                _logger.LogWarning("Solo se cargaran los archivos faltantes");
                return;
            }

            #endregion


            #region Copia las imágenes en otra carpeta, y convierte .zip /.msg a pdf
            if (_cargaImagenesBienesAdjudicados)
            {
                _administraImagenesService.ProcesaHiloYTrabajo(112, 1);
                /*
                imagenes = _servicioDirToXls.ObtieneBienasAdjudicados();
                _servicioImagenes.GuardarImagenes(imagenes, @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesBienesAdjudicados.xlsx");
                */
                return;
            }
            #endregion


            if (_soloCargaImagenesCreditosLiquidados) 
            {
                /*
                imagenes = _servicioDirToXls.ObtieneImagenesCreditosLiquidados();
                foreach (var item in imagenes)
                {
                    item.ThreadId = 113;
                    item.Job = 1;
                    //_logger.LogInformation("Imagen {imagen}", item.NombreArchivo);
                }
                _servicioImagenes.GuardarImagenes(imagenes, @"C:\202302 Digitalizacion\6. Resultado de las imagenes\ImagenesCreditosLiquidados.xlsx");
                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenes);
                */
                _administraImagenesService.ProcesaHiloYTrabajo(113, 1);
                return;
            }

            #region Bienes Adjudicados
            if (_realizaCrucesInformacionBienesAdjudicados)
            {
                _analizaInformacionBienesAdjudicadosService.RealizaAnalisisBienesAdjudicados();
                return;
            }

            #region Aqui analizo el OCR resultante, aqui saco los 3 csv, el último cruzado con los acreditados, creditos, etc
            if (_analizaResultadoOcr) {
                _analizaResultadoOCRService.AnalizaResultado();
                return;
            }
            #endregion

            #region Este crea el archivo de control, para poder luego lanzar el OCR
            if (_soloOCRBienesAdjudicadosContable)
            {
                imagenes = _servicioImagenes.CargaImagenesTratadas(@"C:\202302 Digitalizacion\7. Descargas\PD\112\001\IMG-PD-112-001.xlsx");
                _administraImagenesService.CreaArchivosYCarpetasDeControlOcr(imagenes);
                return;
            }
            #endregion
            #endregion


            IEnumerable<MancomunadosActivos> finales = new List<MancomunadosActivos>();

            if (_cargaPF1RevisionFaltantesCancelados) {
                string archivoConPF1 = @"C:\202302 Digitalizacion\5. Imagenes sin procesar\Complementos\pf1s.xlsx";
                imagenes =  _servicioDirToXls.ObtieneImagenesComplementariasSegundaParte(archivoConPF1);

                string expedientesCancelados = @"C:\202302 Digitalizacion\2. Saldos procesados\HistoricoCancelados.csv";
                IEnumerable<CreditosCanceladosAplicacion> cancelados = _administraCreditosCanceladosService.CargaExpedienteCancelados(expedientesCancelados);
                var comparaActivosVsMancomunadosSinImagenDirecta = (from rsl in cancelados
                                                                    join img in imagenes on QuitaCastigoIndirecto(rsl.NumCreditoCancelado) equals QuitaCastigoIndirecto(img.NumCredito) into grupoLeft
                                                                    from rsltLeft in grupoLeft.DefaultIfEmpty()
                                                                    select new
                                                                    {
                                                                        Cancelados = rsl,
                                                                        Pf1 = rsltLeft
                                                                    }).ToList();


                IEnumerable<CreditosCanceladosAplicacion> canceladosSinPF1 = comparaActivosVsMancomunadosSinImagenDirecta.Where(x => x.Pf1 is null).Select(y => y.Cancelados).ToList();
                string archivoCanceladosSinPF1 = @"C:\202302 Digitalizacion\2. Saldos procesados\HistoricoCanceladosSinPF1.csv";
                _creaArchivoCsv.CreaArchivoCsv(archivoCanceladosSinPF1, canceladosSinPF1);

                return;
            }

            if (_detectaMancomunados) {
                var ministraciones = _servicioMinistracionesMesa.GetMinistracionesProcesadas();
                var mancomunados = ministraciones.GroupBy(x => (x.NumCredito ?? "")[..14]).Select(x => new { x.Key, Cantidad = x.Count() }).ToList().OrderByDescending(z=>z.Cantidad).Where(a=>a.Cantidad>1);

                var sacaMancomunados = (from man in mancomunados
                                       join min in ministraciones on man.Key equals (min.NumCredito??"")[..14]
                                       select new { man, min }).ToList();

                var otroFiltro = sacaMancomunados.Select(saca => new { saca.man.Key, saca.man.Cantidad, saca.min.Acreditado }).ToList();
                var otroGrupo = otroFiltro.GroupBy(grp => grp.Acreditado).Select(y => new { Acreditado = y.Key, NumCred = y.First().Key, y.First().Cantidad, NuevaCantidad = y.Count() });

                /*
                foreach (var otro in otroGrupo.Where(x=>x.NuevaCantidad == 1))    //.Where(x => (x.NumCred ?? "").Equals("60760000619000")))
                {
                    Console.WriteLine(string.Format("{0} {1} {2} {3}", otro.NumCred, otro.Acreditado, otro.Cantidad, otro.NuevaCantidad));
                }
                */

                finales = otroGrupo.Where(x => x.NuevaCantidad == 1).ToList().GroupBy(y=>y.NumCred).Select(z=> new MancomunadosActivos() { NumCredito = z.Key, EsActivo = false }).ToList();
/*
                foreach (var otro in finales)    //.Where(x => (x.NumCred ?? "").Equals("60760000619000")))
                {
                    Console.WriteLine(string.Format("{0}", otro.NumCredito));
                }
*/
                var abSaldosCompleta = _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino);
                string resultado = @"C:\202302 Digitalizacion\2. Saldos procesados\CreditosMultiples.csv";
                if (File.Exists(resultado))
                    File.Delete(resultado);
                _creaArchivoCsv.CreaArchivoCsv(resultado,finales);

                // ExpedientesConsulta.Csv

                Console.WriteLine(finales.Count());
  //              return;
            }

            if (_cargaMancomunadasVsActivos)
            {
                string expedienteConsulta = @"C:\202302 Digitalizacion\2. Saldos procesados\ExpedientesConsultaGv.Csv";
                IEnumerable<ExpedienteDeConsultaGv> resultado = _administraCargaConsultaService.CargaExpedienteDeConsultaGv(expedienteConsulta);
                var compara = (from fin in finales
                               join rsl in resultado.Where(x=>x.EsCreditoAReportar) on fin.NumCredito equals QuitaCastigoIndirecto(rsl.NumCredito)[..14] into grupoLeft
                               from rsltLeft in grupoLeft.DefaultIfEmpty()
                               select new
                               {
                                   Mancomunados = fin,
                                   CreditosActivos = rsltLeft
                               }).ToList();
                foreach (var item in compara)
                {
                    if (item.CreditosActivos is not null)
                    {
                        item.Mancomunados.EsActivo = true;
                        item.Mancomunados.EsImpago = item.Mancomunados.EsImpago || item.CreditosActivos.StatusImpago;
                        item.Mancomunados.EsVencido = item.Mancomunados.EsVencido || item.CreditosActivos.StatusCarteraVencida;
                    }
                }
                // 833
                IEnumerable<MancomunadosActivos> siguenActivos = compara.Where(x=>x.Mancomunados.EsActivo == true).Select(y=>y.Mancomunados).ToList().OrderBy(x=>x.NumCredito).ToList();
                Console.WriteLine(siguenActivos.Count());
                string archivoAunActivos = @"C:\202302 Digitalizacion\2. Saldos procesados\MancomunadosAunActivos.csv";
                if (File.Exists(archivoAunActivos))
                    File.Delete(archivoAunActivos);
                _creaArchivoCsv.CreaArchivoCsv(archivoAunActivos, siguenActivos);

                IEnumerable < ExpedienteDeConsultaGv > soloExpedientesMancomunadosYActivos = compara.Where(x => x.Mancomunados.EsActivo == true).Select(y => y.CreditosActivos).ToList().OrderBy(x => x.NumCredito).ToList();
                string archivoExpedientesAunActivos = @"C:\202302 Digitalizacion\2. Saldos procesados\ExpedientesMancomunadosAunActivos.csv";
                if (File.Exists(archivoExpedientesAunActivos))
                    File.Delete(archivoExpedientesAunActivos);
                _creaArchivoCsv.CreaArchivoCsv(archivoExpedientesAunActivos, soloExpedientesMancomunadosYActivos);

                // sigue quitar los mancomunados activos y obtener los que no tienen imagen directa y los que no tienen imagen

                var comparaActivosVsMancomunadosSinImagenDirecta = (from rsl in resultado.Where(x => x.EsCreditoAReportar).ToList().Where(y=>y.TieneImagenDirecta == false)
                                                                    join fin in finales on QuitaCastigoIndirecto(rsl.NumCredito)[..14] equals fin.NumCredito into grupoLeft
                               from rsltLeft in grupoLeft.DefaultIfEmpty()
                               select new
                               {
                                   Mancomunados = rsltLeft,
                                   CreditosActivos = rsl
                               }).ToList();
                IEnumerable<ExpedienteDeConsultaGv> soloExpedientesSinMancomunadosEImagenesDirectas = comparaActivosVsMancomunadosSinImagenDirecta.Where(x=>x.Mancomunados is null).ToList().Select(y=>y.CreditosActivos);

                string archivoExpedientesSinMancomunadosEImagenes = @"C:\202302 Digitalizacion\2. Saldos procesados\ExpedientesSinImagen.csv";
                if (File.Exists(archivoExpedientesSinMancomunadosEImagenes))
                    File.Delete(archivoExpedientesSinMancomunadosEImagenes);
                _creaArchivoCsv.CreaArchivoCsv(archivoExpedientesSinMancomunadosEImagenes, soloExpedientesSinMancomunadosEImagenesDirectas);

                return;
            }


            if (_seCruzaInformacionImagenesABSaldosCredito )
            {
                _logger.LogInformation("Comenzamos a cargar las imagenes!!!");
                imagenes = _servicioImagenes.CargaImagenesTratadas(@"D:\202302 Digitalizacion\6. Resultado de las imagenes\Todos los PDFs procesados Sin Error.xlsx");
                _logger.LogInformation("Se cargaron todas las imagenes!!!");
                var abSaldos = _administraCargaCorteDiario.CargaABSaldosCompleta(@"C:\Users\jesus.escalera.FINRURAL\Downloads\ABSDO20230324.csv");
                var abSaldosFiltrados = _administraCartera.FiltraABSaldosCarteraContableCredito(abSaldos);

            }

            if (_soloPreparaControlOcr)
            {
                imagenes = _servicioImagenes.CargaImagenesTratadas(@"D:\202302 Digitalizacion\6. Resultado de las imagenes\Todos los PDFs procesados Sin Error.xlsx");
                _administraImagenesService.CreaArchivosYCarpetasDeControlOcr(imagenes);
                return;
            }


            if (_cargaSoloPDF)
            {
                imagenes = _servicioImagenes.CargaImagenesTratadas(@"D:\202302 Digitalizacion\6. Resultado de las imagenes\Todos los PDFs procesados.xlsx");
                foreach (var imagen in imagenes.Where(x=>(x.NumCredito??"").Equals("000000000000000000")))
                {
                    imagen.NumCte = "";
                    imagen.Acreditado = "";
                }
                imagenes = imagenes.Where(x => x.NumPaginas > 0).ToList();
                _servicioImagenes.GuardarImagenes(imagenes, @"D:\202302 Digitalizacion\6. Resultado de las imagenes\Todos los PDFs procesados Sin Error.xlsx");
                //_administraImagenesService.CargaSoloPdf(@"D:\202302 Digitalizacion\7. Descargas\Todo el control-PD.xlsx");
                return;
            }


            if (_soloCreaResultadoDescarga)
            {
                _administraImagenesService.RecolectaArchivosYCarpetasDeControl(111, 120, "PD");
                return;
            }


            if (_preparaImagenesComplementariasSegundaParte)
            {
                IEnumerable<ArchivosImagenes> imagenesSegundaParte;
                imagenes = _servicioDirToXls.ObtieneImagenesComplementarias("", 515968);
                imagenesSegundaParte = _servicioDirToXls.ObtieneImagenesComplementariasSegundaParte("", 564561);
                _servicioImagenes.GuardarImagenes(imagenesSegundaParte, "D:\\202302 Digitalizacion\\6. Resultado de las imagenes\\ImagenesComplementariasSegundaParte.xlsx");
                var imagenesNoProcesadas = _servicioDirToXls.ObtieneSoloDiferencias(imagenes, imagenesSegundaParte);
                _servicioImagenes.GuardarImagenes(imagenesNoProcesadas, "D:\\202302 Digitalizacion\\6. Resultado de las imagenes\\ImagenesComplementariasDiferencia.xlsx");
                //var imagenesNoProcesadas = _servicioDirToXls.ObtieneImagenesDesdeDirectorio("", 564561);
                foreach (var imagen in imagenesNoProcesadas)
                {
                    imagen.ThreadId = 111;
                    imagen.Job = 1;
                }
                //((List<ArchivosImagenes>)imagenes).AddRange(imagenesNoProcesadas);
                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenesNoProcesadas);
                return;
            }



            if (_cargaSoloPDF)
            {
                _administraImagenesService.CargaSoloPdf();
                return;
            }

            if (_preparaImagenesComplementarias) {
                imagenes = _servicioDirToXls.ObtieneImagenesComplementarias("", 515968);
                _servicioImagenes.GuardarImagenes(imagenes, "D:\\202302 Digitalizacion\\6. Resultado de las imagenes\\ImagenesComplementarias.xlsx");
                var imagenesNoProcesadas = _servicioDirToXls.ObtieneImagenesDesdeDirectorio("", 515968);
                foreach(var imagen in imagenesNoProcesadas) {
                    imagen.ThreadId = 110;
                    imagen.Job = 1;
                }
                ((List<ArchivosImagenes>)imagenes).AddRange(imagenesNoProcesadas);
                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenes);
                return;
            }

            if (_soloProcesaErroresDescarga)
            {
                imagenes = _servicioImagenes.CargaImagenesTratadas(_archivoErroresDescarga);
                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenes);
                _logger.LogInformation("Se ha terminado de preparar el control de las imagenes!!");
                _logger.LogInformation("Comenzamos a descargar la información del thread {theadid} y del trabajo {jobId}", _threadId, _trabajoInicial);
                for (int i = _trabajoInicial; i < _trabajoFinal; i++)
                {
                    _administraImagenesService.ProcesaHiloYTrabajo(_threadId, i);
                }
                return;
            }

            if (_soloCargaImagenesProcesadas)
            {
                imagenes = _servicioImagenes.CargaImagenesTratadas();
                _administraImagenesService.CreaArchivosYCarpetasDeControlInicial(imagenes);
                _logger.LogInformation("Se ha terminado de preparar el control de las imagenes!!");
                DateTime inicio = DateTime.Now;
                _administraImagenesService.RecolectaArchivosYCarpetasDeControl(imagenes,"PD2");
                _logger.LogInformation("Se ha terminado de recuperar el control de las imagenes!!");
                DateTime final = DateTime.Now;
                TimeSpan diferencia = final - inicio;
                _logger.LogInformation("Se tardó en recorrer {horas}:{minutos}:{segundos}!!",diferencia.Hours, diferencia.Minutes, diferencia.Seconds);
                return;
            }

            /* Pruebas de sharepoint */

            /*
            for (int i = 21; i < 46; i++)
            {
                _administraImagenesService.ProcesaHiloYTrabajo(1, i);
            }
            */
            //_administraImagenesService.ProcesaHiloYTrabajo(1, 21); //
            _logger.LogInformation("Comenzamos a descargar la información del thread {theadid} y del trabajo {jobId}", _threadId, _trabajoInicial);
            for (int i = _trabajoInicial; i< _trabajoFinal; i++)
            {
                _administraImagenesService.ProcesaHiloYTrabajo(_threadId, i);
            }
            
        }

        private ConversionTratamientos? ObtieneTratamiento(IList<ConversionTratamientos> listaTratamientosMultiples, IList<ConversionTratamientos> listaTratamientosCancelados, ConversionTratamientos? tratamiento)
        {
            if (tratamiento is not null)
            {
                if (listaTratamientosMultiples.Any(x => (x.Origen ?? "").Equals(tratamiento.Origen)))
                {
                    tratamiento.Destino = listaTratamientosMultiples.FirstOrDefault(x => (x.Origen ?? "").Equals(tratamiento.Origen))?.Destino;                    
                    _logger.LogInformation("Lo encontró en multiples {numCredito}/{numCreditoDestino}", tratamiento.Origen, tratamiento.Destino);
                    bool destino = (tratamiento.Destino ?? "").Substring(3,1).Equals("8");
                    if (destino) {
                        tratamiento.Origen = tratamiento.Destino;
                        tratamiento.Destino = "";
                        var resultadoTratamiento = ObtieneTratamiento(listaTratamientosMultiples, listaTratamientosCancelados, tratamiento);
                        if (listaTratamientosCancelados.Any(x => (x.Origen ?? "").Equals(resultadoTratamiento?.Origen)))
                        {
                            _logger.LogError("!!!!!!Lo encontró en Pagados {numCredito}/{numCreditoDestino}", resultadoTratamiento?.Origen, resultadoTratamiento?.Destino);
                        }
                        return resultadoTratamiento;
                    }
                    return tratamiento;
                }
                else
                {
                    if (listaTratamientosCancelados.Any(x => (x.Origen ?? "").Equals(tratamiento.Origen)))
                    {
                        tratamiento.Destino = listaTratamientosCancelados.FirstOrDefault(x => (x.Origen ?? "").Equals(tratamiento.Origen))?.Destino;
                        _logger.LogError("Lo encontró en Pagados {numCredito}/{numCreditoDestino}", tratamiento.Origen, tratamiento.Destino);
                        return tratamiento;
                    }
                    else
                    {
                        _logger.LogError("No lo encontró en Pagados {numCredito}", tratamiento.Origen);
                        return null;
                    }
                }
            }
            return null;
        }
    }
}
