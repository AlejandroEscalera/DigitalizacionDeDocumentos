using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Mancomunados;
using gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
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
        private readonly bool _soloCargaImagenesProcesadas;
        private readonly bool _preparaImagenesComplementarias;
        private readonly bool _preparaImagenesComplementariasSegundaParte;
        private readonly string _archivoErroresDescarga;
        private readonly string _archivoABSaldosDiarioDestino;
        private readonly bool _soloProcesaErroresDescarga;
        private readonly bool _soloCreaResultadoDescarga;
        private readonly bool _soloPreparaControlOcr;
        private readonly bool _cargaSoloPDF;
        private readonly bool _detectaMancomunados;

        private readonly bool _cargaMancomunadasVsActivos;
        private readonly bool _cargaPF1RevisionFaltantesCancelados;

        private readonly bool _cargaImagenesBienesAdjudicados;
        private readonly bool _soloOCRBienesAdjudicadosContable;



        #region Información del trabajo a atender
        private readonly int _threadId;
        private readonly int _trabajoInicial;
        private readonly int _trabajoFinal;
        private readonly bool _seCruzaInformacionImagenesABSaldosCredito;

        private readonly bool _analizaResultadoOcr;

        #endregion
        #region sec
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly string _usuario;
        private readonly SecureString _password;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        #endregion
        public MainApp(ILogger<MainApp> logger, IConfiguration configuration, IServicioImagenes servicioImagenes, IAdministraImagenesService administraImagenesService,
            IServicioDirToXls servicioDirToXls, IAdministraCargaCorteDiario administraCargaCorteDiario, IAdministraCartera administraCartera, IServicioMinistracionesMesa servicioMinistracionesMesa, IAdministraCargaCorteDiario administraCargaCorteDiarioService,
            ICreaArchivoCsv creaArchivoCsv,
            IAdministraCargaConsulta administraCargaConsultaService,
            IAdministraCargaCreditosCancelados administraCreditosCanceladosService,
            IAnalizaResultadoOCR analizaResultadoOCRService
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
            _soloCargaImagenesProcesadas = (_configuration.GetValue<string>("soloCargaImagenesProcesadas") ?? "").Equals("Si");
            _archivoErroresDescarga = (_configuration.GetValue<string>("archivoErroresDescarga") ?? "");
            _soloProcesaErroresDescarga = true;
            #region sec
            _password = (_configuration.GetValue<string>("contrasenia") ?? "").ToSecureString();
            _usuario = (_configuration.GetValue<string>("usuario") ?? "");
            #endregion
            _archivoABSaldosDiarioDestino = _configuration.GetValue<string>("archivoABSaldosDiarioDestino") ?? "";
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

        public void Run()
        {
            // N1F1c13r42019$
            IEnumerable<ArchivosImagenes> imagenes;

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
                string expedienteConsulta = @"C:\202302 Digitalizacion\2. Saldos procesados\ExpedientesConsulta.Csv";
                IEnumerable<ExpedienteDeConsulta> resultado = _administraCargaConsultaService.CargaExpedienteDeConsulta(expedienteConsulta);
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

                IEnumerable < ExpedienteDeConsulta > soloExpedientesMancomunadosYActivos = compara.Where(x => x.Mancomunados.EsActivo == true).Select(y => y.CreditosActivos).ToList().OrderBy(x => x.NumCredito).ToList();
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
                IEnumerable<ExpedienteDeConsulta> soloExpedientesSinMancomunadosEImagenesDirectas = comparaActivosVsMancomunadosSinImagenDirecta.Where(x=>x.Mancomunados is null).ToList().Select(y=>y.CreditosActivos);

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
    }
}
