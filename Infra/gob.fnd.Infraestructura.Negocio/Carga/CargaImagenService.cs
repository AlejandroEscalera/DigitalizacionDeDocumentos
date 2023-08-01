using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.Carga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Rar;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Imagenes;
using gob.fnd.Infraestructura.Negocio.Consultas;
using Jaec.Helper.Procesos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Carga
{
    public class CargaImagenService : ICargaImagenes
    {
        #region Servicios
        private readonly IDescomprimeArchivo _descompresor7Z;
        private readonly IDescomprimeArchivo _descompresorRar;
        private readonly IConsultaServices _consultaServices;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly IAdministraImagenesService _administraImagenesService;
        private readonly IConsultaServices _consultaService;
        private readonly IDescomprimeArchivo _descompresorZip;
        #endregion

        #region Helpers para la clase
        private readonly ILogger<CargaImagenService> _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Informacion de los archivos
        private readonly string _driveImagenes;
        /*
        private readonly string _archivoExpedientesConsultaCsv;
        private readonly string _archivoExpedienteJuridicoCsv;
        private readonly string _archivoDeExpedientesCanceladosCsv;
        private readonly string _archivoColocacionConPagosCsv;
        */
        private readonly string _archivoImagenCorta;
        private readonly string _archivosImagenesExpedientesJuridico;
        #endregion

        public CargaImagenService(ILogger<CargaImagenService> logger, IConfiguration configuration, IDescomprimeArchivo7z descompresor7z, IDescomprimeArchivoZip descompresorZip, IDescomprimeArchivoRar descompresorRar, IConsultaServices consultaServices, IServicioImagenes servicioImagenes, IAdministraImagenesService administraImagenesService,
            IConsultaServices consultaService)
        {
            _logger = logger;
            _configuration = configuration;
            _descompresor7Z = descompresor7z;
            _descompresorZip = descompresorZip;
            _descompresorRar = descompresorRar;
            _consultaServices = consultaServices;
            _servicioImagenes = servicioImagenes;
            _administraImagenesService = administraImagenesService;
            _consultaService = consultaService;
            _driveImagenes = _consultaServices.ObtieneUnidadImagenes();
            /*
            _archivoExpedientesConsultaCsv = _configuration.GetValue<string>("archivoExpedientesConsulta") ?? "";
            _archivoExpedienteJuridicoCsv = _configuration.GetValue<string>("archivoExpedientesJuridicosCsv") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\ExpedientesJuridicos\\BITACORA GENERAL AL 31 DE MARZO 2023.csv";
            _archivoDeExpedientesCanceladosCsv = _configuration.GetValue<string>("archivosCreditosCancelados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\HistoricoCancelados.csv";
            _archivoColocacionConPagosCsv = _configuration.GetValue<string>("archivoLiquidaciones") ?? "C:\\202302 Digitalizacion\\1. AB Saldos\\Liquidados\\historico colocacion con pagos final.csv";
            */
            _archivoImagenCorta = _configuration.GetValue<string>("archivoImagenCorta") ?? "";
            _archivosImagenesExpedientesJuridico = _configuration.GetValue<string>("archivosImagenesExpedientesJuridico") ?? "C:\\202302 Digitalizacion\\6. Resultado de las imagenes\\TodosLosTurnos.xlsx";
            // _archivosImagenesExpedientesJuridicoCorta = _configuration.GetValue<string>("archivosImagenesExpedientesJuridicoCorta") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\TodosLosTurnos.csv";

        }
        public async Task<IEnumerable<ArchivoImagenCorta>> CargaImagenesLiquidadosAsync(string directorioOrigen, IProgress<ReporteProgresoProceso> progreso, string directorioTemporal = "")
        {
            _logger.LogInformation("Inicia el proceso de carga de imagenes");
            int numeroDeImagenes = 0;
            int cantidadDePasos = 12;
            int etapa = 1;
            IList<ArchivosImagenes> imagenesPorCargar = new List<ArchivosImagenes>();
            #region Cargo las imagenes procesadas
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes ya procesadas", NumeroProgresoSubProceso = 1, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Obteniendo la unidad de descarga" });
            string driveImagenes = await Task.Run(() => _consultaServices.ObtieneUnidadImagenes());
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes ya procesadas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Cargando las imagenes" });
            IEnumerable<ArchivosImagenes> imagenesPorcesadas = await _servicioImagenes.CargaImagenesTratadasAsync();
            numeroDeImagenes = imagenesPorcesadas.Count();
            #endregion

            #region Obtengo los archivos que voy a procesar
            etapa++;
            IList<ArchivosImagenes> listaImagenesAProcesar = new List<ArchivosImagenes>();
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Obteniendo imagenes de la carpeta", NumeroProgresoSubProceso = 1, TotalProgresoSubProceso = 1, EtiquetaSubProceso = "Obteniendo lista de archivos a descomprimir" });
            IList<string> listaArchivosADescomprimir = await ObtieneListaDirectorios(directorioOrigen);
            int cantidadDeArchivos = listaArchivosADescomprimir.Count;
            #endregion

            #region Descomprimo los archivos
            string directorioDestino = !String.IsNullOrEmpty(directorioTemporal)? directorioTemporal : Path.GetTempPath();
            int archivoProcesado = 0;
            etapa++;
            foreach (var nombreArchivo in listaArchivosADescomprimir)
            {
                archivoProcesado++;
                progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Descomprimiendo imagenes", NumeroProgresoSubProceso = archivoProcesado, TotalProgresoSubProceso = cantidadDeArchivos, EtiquetaSubProceso = string.Format("Descomprimiendo archivo {0}", nombreArchivo) });
                Progress<ReporteProgresoDescompresionArchivos> progress = new();
                progress.ProgressChanged += (sender,  progressModel) => { 
                    progreso.Report(new ReporteProgresoProceso { 
                        NumeroPasoActual = etapa, 
                        EtiquetaDePaso = "Descomprimiendo imagenes", 
                        NumeroProgresoSubProceso = archivoProcesado, 
                        TotalProgresoSubProceso = cantidadDeArchivos, 
                        EtiquetaSubProceso = string.Format("Descomprimiendo archivo {0}", nombreArchivo),
                        ArchivoProcesado = progressModel.ArchivoProcesado,
                        CantidadArchivos = progressModel.CantidadArchivos,
                        InformacionArchivo = progressModel.InformacionArchivo
                    });
                };
                
                IDescomprimeArchivo descomprimeArchivo = ObtieneDescompresor(Path.GetExtension(nombreArchivo));
                progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Descomprimiendo imagenes", NumeroProgresoSubProceso = archivoProcesado, TotalProgresoSubProceso = cantidadDeArchivos, EtiquetaSubProceso = "Termino la descompresión, cargo informacion basica a memoria" });

                string rutaDestino = Path.Combine(directorioDestino, Path.GetFileNameWithoutExtension(nombreArchivo));
                var resultadoDescompreso = await descomprimeArchivo.DescomprimeArchivoAsync(nombreArchivo, rutaDestino, progress);
                foreach (var nombreImagen in resultadoDescompreso)
                {
                    numeroDeImagenes++;

                    listaImagenesAProcesar.Add(new() {
                        Id = numeroDeImagenes,
                        ArchivoOrigen = nombreArchivo,
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = Path.GetFileName(nombreImagen).Trim(),
                        RutaDeGuardado = Path.Combine("LQ2\\" + (Path.GetFileNameWithoutExtension(nombreArchivo)) ?? "").Trim(),
                        UsuarioDeModificacion = "",
                        NumCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreImagen))),
                        NumContrato = Condiciones.ObtieneNumContrato(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreImagen))),
                        NumeroCreditoActivo = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreImagen))),
                        UrlArchivo = (Path.Combine(rutaDestino, nombreImagen) ?? "").Trim(),
                        Extension = new FileInfo(Path.GetFileName(nombreImagen).Trim()).Extension,
                        CarpetaDestino = _driveImagenes + "\\11\\LQ2\\" + (Path.GetFileName(nombreArchivo) ?? "").Trim(),
                        ThreadId = -1,
                        Job = 1
                    });
                }

                // ((List<string>)listaImagenesAProcesar).AddRange(resultadoDescompreso);
            }
            #endregion

            etapa++;
            long cantidadImagenesAProcesar = listaImagenesAProcesar.Count;
            Progress<ReporteProgresoDescompresionArchivos> progresa = new();
            progresa.ProgressChanged += (sender, progressModel) => {
                progreso.Report(new ReporteProgresoProceso
                {
                    NumeroPasoActual = etapa,
                    CantidadDePasos = 10,
                    EtiquetaDePaso = "Copiando imagenes",
                    NumeroProgresoSubProceso = progressModel.ArchivoProcesado,
                    TotalProgresoSubProceso = cantidadImagenesAProcesar,
                    EtiquetaSubProceso = string.Format("Copiando el archivo {0}", progressModel.InformacionArchivo),
                    ArchivoProcesado = 0,
                    CantidadArchivos = 0,
                    InformacionArchivo = string.Empty
                });
            };

            await _administraImagenesService.ProcesaImagenesCargadasAsync(imagenesPorCargar, listaImagenesAProcesar, progresa);
            // Aqui elimino los duplicados
            var cruceArchivosNuevos = await Task.Run(() =>
            {
                return (from otros in imagenesPorCargar.Where(x => (x.CarpetaDestino ?? "").Contains(@":\11\LQ2\", StringComparison.InvariantCultureIgnoreCase))
                        join totales in imagenesPorcesadas.Where(x => (x.CarpetaDestino ?? "").Contains(@":\11\LQ2\", StringComparison.InvariantCultureIgnoreCase))
                           on EliminarUnidadDisco(otros.ArchivoOrigen) equals EliminarUnidadDisco(totales.ArchivoOrigen) into joinedFiles
                        from leftJoinResult in joinedFiles.DefaultIfEmpty()
                        where leftJoinResult == null
                        select otros).ToList();
            });


            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Empatando las imagenes previas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            ((List<ArchivosImagenes>)imagenesPorcesadas).AddRange(cruceArchivosNuevos);

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Carga imagenes de expedientes jurídicos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionImagenesExpedientesJuridicosAsync();

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Guardando todas las imagenes procesadas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _servicioImagenes.GuardarImagenesAsync(imagenesPorcesadas);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Creando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            etapa++;
            if (File.Exists(_archivoImagenCorta))
            {
                File.Delete(_archivoImagenCorta);
            }
            
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            var imagenes = await _consultaService.CargaInformacionImagenesAsync();
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando los expedientes activos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionAsync(true);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando información de liquidados, cancelados y expedientes jurídicos ", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            Task[] listaTareas = new Task[3];
            listaTareas[0] = _consultaService.CargaCreditosCanceladosAsync(true);
            listaTareas[1] = _consultaService.CargaColocacionConPagosAsync(true);
            listaTareas[2] = _consultaService.CargaExpedientesJuridicosAsync(true);
            await Task.WhenAll(listaTareas);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Terminó", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            _logger.LogInformation("Finaliza el proceso de carga de imagenes");
            return imagenes;
        }

        #region Helpers
        private IDescomprimeArchivo ObtieneDescompresor(string extension)
        {
            if (extension.Contains("7z",StringComparison.InvariantCultureIgnoreCase))
            {
                return _descompresor7Z;

            }
            if (extension.Contains("rar", StringComparison.InvariantCultureIgnoreCase))
            {
                return _descompresorRar;

            }
            return _descompresorZip;
        }
        public async Task<IList<string>> ObtieneListaDirectorios(string directorioOrigen)
        {
            string[] files;
            try
            {
                DirectoryInfo directoryInfo = new(directorioOrigen);

                files = await Task.Run(() =>
                    directoryInfo.EnumerateFiles("*.zip")
                        .Concat(directoryInfo.EnumerateFiles("*.rar"))
                        .Concat(directoryInfo.EnumerateFiles("*.7z"))
                        .Select(file => file.FullName)
                        .ToArray());

                return new List<string>(files);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción aquí
                Console.WriteLine($"Ocurrió un error al buscar los archivos de archivo: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private static string ObtenerRutaCompleta(string cadena)
        {
            cadena = Path.GetFileNameWithoutExtension(cadena);
            /*
            if (cadena.Length < 13)
                throw new ArgumentException("La cadena debe tener al menos 15 caracteres.");
            */
            string primerValor = cadena[..1].PadRight(3, '0');
            string segundoValor = cadena[..3].PadRight(3, '0');
            string tercerValor = cadena[..14].PadRight(14, '0');
            string ruta = $@"\GV\{primerValor}\{segundoValor}\{tercerValor}\{cadena}.PDF";

            return ruta;
        }
        private static string ObtenerRutaCompletaSinArchivo(string cadena)
        {
            cadena = Path.GetFileNameWithoutExtension(cadena);
            if (cadena.Length < 14)
                throw new ArgumentException("La cadena debe tener al menos 15 caracteres.");

            string primerValor = cadena[..1].PadRight(3, '0');
            string segundoValor = cadena[..3].PadRight(3, '0');
            string tercerValor = cadena[..14].PadRight(14, '0');
            string ruta = $@"\GV\{primerValor}\{segundoValor}\{tercerValor}";

            return ruta;
        }

        private static string EliminarUnidadDisco(string? rutaCompleta)
        {
            if (string.IsNullOrEmpty(rutaCompleta))
                return string.Empty;
            // Eliminar la unidad de disco (por ejemplo, "C:\") del nombre del archivo
            int index = rutaCompleta.IndexOf('\\');
            return rutaCompleta[(index + 1)..];
        }
        #endregion

        public async Task<IEnumerable<ArchivoImagenCorta>> CargaImagenesGuardaValoresAsync(string directorioOrigen, IProgress<ReporteProgresoProceso> progreso, string unidadTemporal = "")
        {
            _logger.LogInformation("Inicia el proceso de carga de imagenes de Guarda Valores");
            #region Inicializacion
            int numeroDeImagenes = 0;
            int cantidadDePasos = 9;
            int etapa = 1;
            IList<ArchivosImagenes> imagenesPorCargar = new List<ArchivosImagenes>();
            IList<ArchivosImagenes> listaImagenesAProcesar = new List<ArchivosImagenes>();
            string directorioCargaGuardaValores = directorioOrigen;
            if (!string.IsNullOrEmpty(unidadTemporal))
            {
                unidadTemporal = @"z:";
            }
            string mapeaUnidadGuardaValores = $@"subst ""{unidadTemporal}"" ""{directorioCargaGuardaValores}""";
            string sacaInformacionArchivos = $@"dir ""{unidadTemporal}\*.pdf"" /b /a-d /s";
            string eliminaMapeoUnidadGuardaValores = $@"subst ""{unidadTemporal}"" /d";
            #endregion
            #region Cargo las imagenes procesadas
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes ya procesadas", NumeroProgresoSubProceso = 1, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Obteniendo la unidad de descarga" });
            string driveImagenes = await Task.Run(() => _consultaServices.ObtieneUnidadImagenes());
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes ya procesadas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Cargando las imagenes" });
            IEnumerable<ArchivosImagenes> imagenesPorcesadas = await _servicioImagenes.CargaImagenesTratadasAsync();
            //IEnumerable<ArchivosImagenes> imagenesPorcesadas = new List<ArchivosImagenes>();
            numeroDeImagenes = imagenesPorcesadas.Count();
            #endregion

            // Elimino el monton de carpetas, para evitar problema con los nombres largos
            var salida = await EjecutaProceso.EjecutaProcesoAsyc(mapeaUnidadGuardaValores);
            try
            {
                salida = await EjecutaProceso.EjecutaProcesoAsyc(sacaInformacionArchivos);
                List<string> listaArchivos = salida.Split("\r\n").ToList();

                #region Procesos los archivos a Copiar
                foreach (var nombreArchivo in listaArchivos)
                {
                    numeroDeImagenes++;
                    // string rutaDestino = Path.Combine(unidadTemporal, ObtenerRutaCompleta(Path.GetFileNameWithoutExtension(nombreArchivo)));
                    string numeroCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreArchivo)));
                    string numeroContrato = Condiciones.ObtieneNumContrato(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreArchivo)));
                    string soloPDF = Path.GetFileName(nombreArchivo);
                    string soloExtension = new FileInfo(ObtenerRutaCompleta(numeroCredito).Trim()).Extension;
                    string carpetaDestino = driveImagenes + "11" + ObtenerRutaCompletaSinArchivo(numeroCredito);
                    if (numeroCredito.Equals("000000000000000000"))
                    {
                        // Ya no lo descargo
                        continue;
                    }
                    numeroContrato = numeroCredito;
                    listaImagenesAProcesar.Add(new()
                    {
                        Id = numeroDeImagenes,
                        ArchivoOrigen = nombreArchivo,
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = soloPDF,
                        RutaDeGuardado = ObtenerRutaCompletaSinArchivo(numeroCredito),
                        UsuarioDeModificacion = "",
                        NumCredito = numeroCredito,
                        NumContrato = numeroContrato,
                        NumeroCreditoActivo = numeroCredito,
                        UrlArchivo = nombreArchivo.Trim(),
                        Extension = soloExtension,
                        CarpetaDestino = carpetaDestino,
                        ThreadId = -1,
                        Job = 1
                    });
                }
                #endregion
                #region Comienza la copia
                etapa++;
                long cantidadImagenesAProcesar = listaImagenesAProcesar.Count;
                Progress<ReporteProgresoDescompresionArchivos> progresa = new();
                progresa.ProgressChanged += (sender, progressModel) => {
                    progreso.Report(new ReporteProgresoProceso
                    {
                        NumeroPasoActual = etapa,
                        CantidadDePasos = 10,
                        EtiquetaDePaso = "Copiando imagenes",
                        NumeroProgresoSubProceso = progressModel.ArchivoProcesado,
                        TotalProgresoSubProceso = cantidadImagenesAProcesar,
                        EtiquetaSubProceso = string.Format("Copiando el archivo {0}", progressModel.InformacionArchivo),
                        ArchivoProcesado = 0,
                        CantidadArchivos = 0,
                        InformacionArchivo = string.Empty
                    });
                };

                await _administraImagenesService.ProcesaImagenesCargadasAsync(imagenesPorCargar, listaImagenesAProcesar, progresa);
                #endregion

            }
            finally
            {
                // Elimino el mapeo temporal, es un try finally para asegurar que se elimine
                salida = await EjecutaProceso.EjecutaProcesoAsyc(eliminaMapeoUnidadGuardaValores);
                Console.WriteLine(salida);
            }

            #region Aqui es donde debo quitar los duplicados
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Empatando las imagenes previas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });

            /*
             Necesito un left join entre imagenesPorCargar e imagenesPorcesadas, la igualdad sería por el nombre del archivo origen sin la unidad de disco en la ruta
             Si el imagenesProcesadas es null, entonces lo agrego a la lista de imagenesPorCargar
            */
            var cruceArchivosNuevos = await Task.Run(() =>
            {
               return (from otros in imagenesPorCargar.Where(x => (x.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase))
                 join totales in imagenesPorcesadas.Where(x => (x.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase))
                    on EliminarUnidadDisco(otros.ArchivoOrigen) equals EliminarUnidadDisco(totales.ArchivoOrigen) into joinedFiles
                 from leftJoinResult in joinedFiles.DefaultIfEmpty()
                 where leftJoinResult == null
                 select otros).ToList();
            });

             ((List<ArchivosImagenes>)imagenesPorcesadas).AddRange(cruceArchivosNuevos); 
            #endregion

            #region Empato los expedientes
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Carga imagenes de expedientes jurídicos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionImagenesExpedientesJuridicosAsync();

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Guardando todas las imagenes procesadas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _servicioImagenes.GuardarImagenesAsync(imagenesPorcesadas);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Creando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            etapa++;
            if (File.Exists(_archivoImagenCorta))
            {
                File.Delete(_archivoImagenCorta);
            }

            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            var imagenes = await _consultaService.CargaInformacionImagenesAsync();
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando los expedientes activos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionAsync(true);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando información de liquidados, cancelados y expedientes jurídicos ", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            Task[] listaTareas = new Task[3];
            listaTareas[0] = _consultaService.CargaCreditosCanceladosAsync(true);
            listaTareas[1] = _consultaService.CargaColocacionConPagosAsync(true);
            listaTareas[2] = _consultaService.CargaExpedientesJuridicosAsync(true);
            await Task.WhenAll(listaTareas);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Terminó", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            #endregion
            _logger.LogInformation("Finaliza el proceso de carga de imagenes de guarda valores");
            return imagenes;
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

        public async Task<IEnumerable<ArchivoImagenCorta>> CargaImagenesExpedientesAsync(string directorioOrigen, IProgress<ReporteProgresoProceso> progreso, string unidadTemporal = "")
        {
            _logger.LogInformation("Inicia el proceso de carga de imagenes de Expedientes");
            #region Inicializacion
            int numeroDeImagenes = 0;
            int cantidadDePasos = 11;
            int etapa = 1;
            IList<ArchivosImagenes> imagenesPorCargar = new List<ArchivosImagenes>();
            IList<ArchivosImagenes> listaImagenesAProcesar = new List<ArchivosImagenes>();
            string directorioCargaExpedientes = directorioOrigen;
            if (!string.IsNullOrEmpty(unidadTemporal))
            {
                unidadTemporal = @"z:";
            }
            string mapeaUnidadExpedientes = $@"subst ""{unidadTemporal}"" ""{directorioCargaExpedientes}""";
            string sacaInformacionArchivos = $@"dir ""{unidadTemporal}\*.pdf"" /b /a-d /s";
            string eliminaMapeoUnidadExpedientes = $@"subst ""{unidadTemporal}"" /d";
            #endregion
            #region Cargo las imagenes procesadas
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes de expedientes", NumeroProgresoSubProceso = 1, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Obteniendo la unidad de descarga" });
            string driveImagenes = await Task.Run(() => _consultaServices.ObtieneUnidadImagenes());
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando las imagenes de expedientes", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 3, EtiquetaSubProceso = "Cargando el registro de todas las imagenes registradas" });
            IEnumerable<ArchivosImagenes> imagenesPorcesadas = await _servicioImagenes.CargaImagenesTratadasAsync();
            numeroDeImagenes = imagenesPorcesadas.Count();
            #endregion

            // Elimino el monton de carpetas, para evitar problema con los nombres largos
            var salida = await EjecutaProceso.EjecutaProcesoAsyc(mapeaUnidadExpedientes);
            try
            {
                salida = await EjecutaProceso.EjecutaProcesoAsyc(sacaInformacionArchivos);
                List<string> listaArchivos = salida.Split("\r\n").ToList();

                #region Procesos los archivos a Copiar
                foreach (var nombreArchivo in listaArchivos)
                {
                    numeroDeImagenes++;
                    string numeroCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreArchivo)));
                    string numeroContrato = Condiciones.ObtieneNumContrato(Condiciones.LimpiaNoNumeros(Path.GetFileName(nombreArchivo)));
                    string soloPDF = Path.GetFileName(nombreArchivo);
                    string soloExtension = new FileInfo(ObtenerRutaCompleta(numeroCredito).Trim()).Extension;
                    string carpetaDestino = driveImagenes + "11" + ObtenerRutaCompletaSinArchivo(numeroCredito);
                    if (numeroCredito.Equals("000000000000000000"))
                    {
                        // Ya no lo descargo
                        continue;
                    }
                    numeroContrato = numeroCredito;
                    listaImagenesAProcesar.Add(new()
                    {
                        Id = numeroDeImagenes,
                        ArchivoOrigen = nombreArchivo,
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = soloPDF,
                        RutaDeGuardado = ObtenerRutaCompletaSinArchivo(numeroCredito),
                        UsuarioDeModificacion = "",
                        NumCredito = numeroCredito,
                        NumContrato = numeroContrato,
                        NumeroCreditoActivo = numeroCredito,
                        UrlArchivo = nombreArchivo.Trim(),
                        Extension = soloExtension,
                        CarpetaDestino = carpetaDestino,
                        ThreadId = -1,
                        Job = 1
                    });
                }
                #endregion
                #region Comienza la copia
                etapa++;
                long cantidadImagenesAProcesar = listaImagenesAProcesar.Count;
                Progress<ReporteProgresoDescompresionArchivos> progresa = new();
                progresa.ProgressChanged += (sender, progressModel) => {
                    progreso.Report(new ReporteProgresoProceso
                    {
                        NumeroPasoActual = etapa,
                        CantidadDePasos = 10,
                        EtiquetaDePaso = "Copiando imagenes",
                        NumeroProgresoSubProceso = progressModel.ArchivoProcesado,
                        TotalProgresoSubProceso = cantidadImagenesAProcesar,
                        EtiquetaSubProceso = string.Format("Copiando el archivo {0}", progressModel.InformacionArchivo),
                        ArchivoProcesado = 0,
                        CantidadArchivos = 0,
                        InformacionArchivo = string.Empty
                    });
                };

                await _administraImagenesService.ProcesaImagenesCargadasAsync(imagenesPorCargar, listaImagenesAProcesar, progresa);
                #endregion

            }
            finally
            {
                // Elimino el mapeo temporal, es un try finally para asegurar que se elimine
                salida = await EjecutaProceso.EjecutaProcesoAsyc(eliminaMapeoUnidadExpedientes);
                Console.WriteLine(salida);
            }

            #region Aqui es donde debo quitar los duplicados
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Empatando las imagenes previas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });

            /*
             Necesito un left join entre imagenesPorCargar e imagenesPorcesadas, la igualdad sería por el nombre del archivo origen sin la unidad de disco en la ruta
             Si el imagenesProcesadas es null, entonces lo agrego a la lista de imagenesPorCargar
            */
            var cruceArchivosNuevos = await Task.Run(() =>
            {
                return (from otros in imagenesPorCargar.Where(x => (x.CarpetaDestino ?? "").Contains(@":\11\UE\", StringComparison.InvariantCultureIgnoreCase))
                        join totales in imagenesPorcesadas.Where(x => (x.CarpetaDestino ?? "").Contains(@":\11\UE\", StringComparison.InvariantCultureIgnoreCase))
                           on EliminarUnidadDisco(otros.ArchivoOrigen) equals EliminarUnidadDisco(totales.ArchivoOrigen) into joinedFiles
                        from leftJoinResult in joinedFiles.DefaultIfEmpty()
                        where leftJoinResult == null
                        select otros).ToList();
            });

            ((List<ArchivosImagenes>)imagenesPorcesadas).AddRange(cruceArchivosNuevos);

            /*
             Configuro los turnos de todas las imagenes procesadas
             */
            await Task.Run(() =>
            {
                Parallel.ForEach(imagenesPorcesadas, (imagen) =>
                {
                    imagen.UsuarioDeModificacion = (imagen.UrlArchivo ?? "").Contains("Cobranza", StringComparison.InvariantCultureIgnoreCase) ? "Si" : "";
                    imagen.ClasifMesa = TieneTurno(imagen.UrlArchivo, imagen.UrlArchivo) ? "Si" : "";
                });
            });


            #endregion

            #region Empato los expedientes
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Carga imagenes de expedientes jurídicos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionImagenesExpedientesJuridicosAsync();

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Guardando todas las imagenes procesadas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _servicioImagenes.GuardarImagenesAsync(imagenesPorcesadas);

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Guardando las imagenes de los turnos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            var soloImagenesTurnos = imagenesPorcesadas.Where(x => (x.UsuarioDeModificacion ??"").Equals("Si", StringComparison.InvariantCultureIgnoreCase) ||
            (x.ClasifMesa ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)).ToList();
            await _servicioImagenes.GuardarImagenesAsync(soloImagenesTurnos, _archivosImagenesExpedientesJuridico);

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Guardando las imagenes cortas de los turnos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            var expedientesJuridicoCorta = await _consultaService.GuardaImagenesCortasJuridico(soloImagenesTurnos);

            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Creando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            etapa++;
            if (File.Exists(_archivoImagenCorta))
            {
                File.Delete(_archivoImagenCorta);
            }

            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Cargando imagenes cortas", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            var imagenes = await _consultaService.CargaInformacionImagenesAsync();
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando los expedientes activos", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            await _consultaService.CargaInformacionAsync(true);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Actualizando información de liquidados, cancelados y expedientes jurídicos ", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            Task[] listaTareas = new Task[3];
            listaTareas[0] = _consultaService.CargaCreditosCanceladosAsync(true);
            listaTareas[1] = _consultaService.CargaColocacionConPagosAsync(true);
            listaTareas[2] = _consultaService.CargaExpedientesJuridicosAsync(true);
            await Task.WhenAll(listaTareas);
            etapa++;
            progreso.Report(new ReporteProgresoProceso { NumeroPasoActual = etapa, CantidadDePasos = cantidadDePasos, EtiquetaDePaso = "Terminó", NumeroProgresoSubProceso = 2, TotalProgresoSubProceso = 0, EtiquetaSubProceso = "" });
            #endregion
            _logger.LogInformation("Finaliza el proceso de carga de imagenes de guarda valores");
            return imagenes;
        }

    }
}
