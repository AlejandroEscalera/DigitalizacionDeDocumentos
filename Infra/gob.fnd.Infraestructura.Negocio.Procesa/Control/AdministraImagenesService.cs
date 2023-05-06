using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Control;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using gob.fnd.Infraestructura.Negocio.Procesa.Control.ZIP;
using gob.fnd.Infraestructura.Negocio.Procesa.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static gob.fnd.Infraestructura.Negocio.Procesa.Control.AdministraImagenesService;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Control;

public class AdministraImagenesService : IAdministraImagenesService
{
    private readonly ILogger<AdministraImagenesService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServicioImagenes _servicioImagenes;
    private readonly IProcesaArchivoCompreso _procesaArchivoCompreso;
    private readonly IProcesaConversionArchivos _procesaConversionArchivos;
    private readonly IPdfInfo _pdfInfo;
    private readonly IProcesaDescargaInformacion _procesaDescargaInformacion;
    private readonly IServicioMinistracionesMesa _ministracionesMesaService;
    private readonly IServicioABSaldosActivos _abSaldosActivosService;
    private readonly IServicioABSaldosConCastigo _abSaldosConCastigoService;
    private readonly IAdministraGuardaValores _administraGuardaValores;
    private readonly IObtieneCatalogoProductos _obtieneCatalogoProductos;
    private readonly ICruceInformacion _cruceInformacion;
    private readonly string _directorioRaizControl;
    private readonly string _archivoSoloPDF;
    private readonly string _archivoSoloPDFProcesado;
    private readonly string _archivoSaldosConCastigoProcesados;

    private IEnumerable<ArchivosImagenes>? _imagenesPorProcesar;

    public delegate void CrearElDirecrtorio(string directoryName, string fileName, string control, int thread, int job);
    public CrearElDirecrtorio? AlCrearElDirecrtorio { get; set; }
    public AdministraImagenesService(ILogger<AdministraImagenesService> logger, IConfiguration configuration, IServicioImagenes servicioImagenes,
        IProcesaArchivoCompreso procesaArchivoCompreso, IProcesaConversionArchivos procesaConversionArchivos, IPdfInfo pdfInfo, IProcesaDescargaInformacion procesaDescargaInformacion,
        IServicioMinistracionesMesa ministracionesMesaService, IServicioABSaldosActivos abSaldosActivosService, IServicioABSaldosConCastigo abSaldosConCastigoService,
        IAdministraGuardaValores administraGuardaValores, IObtieneCatalogoProductos obtieneCatalogoProductos, ICruceInformacion cruceInformacion)
    {
        _logger = logger;
        _configuration = configuration;
        _servicioImagenes = servicioImagenes;
        _procesaArchivoCompreso = procesaArchivoCompreso;
        _procesaConversionArchivos = procesaConversionArchivos;
        _pdfInfo = pdfInfo;
        _procesaDescargaInformacion = procesaDescargaInformacion;
        _ministracionesMesaService = ministracionesMesaService;
        _abSaldosActivosService = abSaldosActivosService;
        _abSaldosConCastigoService = abSaldosConCastigoService;
        _administraGuardaValores = administraGuardaValores;
        _obtieneCatalogoProductos = obtieneCatalogoProductos;
        _cruceInformacion = cruceInformacion;
        _directorioRaizControl = _configuration.GetValue<string>("directorioRaizControl") ?? "";
        _archivoSoloPDF = _configuration.GetValue<string>("archivoSoloPDF") ?? "";
        _archivoSoloPDFProcesado = _configuration.GetValue<string>("archivoSoloPDFProcesado") ?? "";
        _archivoSaldosConCastigoProcesados = _configuration.GetValue<string>("archivoSaldosConCastigoProcesados") ?? "";
    }

    public bool CreaArchivosYCarpetasDeControlInicial(IEnumerable<ArchivosImagenes> imagenes)
    {
        _imagenesPorProcesar = imagenes;
        if (string.IsNullOrEmpty(_directorioRaizControl))
            return false;
        var todosLosthreads = (from img in imagenes
                               group img by img.ThreadId into cnt
                               select new ThreadInfo() { ThreadId = cnt.Key, MaxJob = cnt.Max(x => x.Job) }).ToList();

        foreach (var thread in todosLosthreads)
        {
            int threadId = thread.ThreadId;
            int maxRecords = thread.MaxJob;

            if (!Directory.Exists(_directorioRaizControl))
            {
                Directory.CreateDirectory(_directorioRaizControl);
            }

            AlCrearElDirecrtorio += GuardaInformacionControl;
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "CTL"); // Informacion de Control
            AlCrearElDirecrtorio -= GuardaInformacionControl;
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PD"); // Procesos de Descarga
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PO"); // Procesos de Ocr
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "AO"); // Procesos de Analisis Ocr
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PA"); // Procesos para acomodar las imagenes
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "FIN"); // Imagenes terminadas
        }
        return true;
    }

    private void GuardaInformacionControl(string directoryName, string fileName, string control, int thread, int job)
    {
        if (_imagenesPorProcesar != null)
        {
            if (control.Equals("CTL", StringComparison.InvariantCultureIgnoreCase))
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                var filtraImagenes = (from img in _imagenesPorProcesar
                                      where img.ThreadId == thread && img.Job == job
                                      select img).ToList();
                _servicioImagenes.GuardarImagenes(filtraImagenes, fileName);
                if (job % 10 == 0)
                    _logger.LogInformation("Se guardó el archivo de control de imágenes {archivoControl} con {numeroImagenes}", fileName, filtraImagenes.Count);
            }
        }
    }

    

    private void CreaDirectorios(string directorioRaiz, ThreadInfo thread, int maxJob, string control) // IEnumerable<ArchivosImagenes> imagenes
    {
        string directorioACrear = Path.Combine(directorioRaiz, control); // Proceso de descarga
        if (!Directory.Exists(directorioACrear))
        {
            Directory.CreateDirectory(directorioACrear);
        }
        string directorioThread = Path.Combine(directorioACrear, String.Format("{0:000}", thread.ThreadId));
        if (!Directory.Exists(directorioThread))
        {
            Directory.CreateDirectory(directorioThread);
        }
        for (int i = 1; i <= maxJob; i++)
        {
            string directorioJob = Path.Combine(directorioThread, String.Format("{0:000}", i));
            if (!Directory.Exists(directorioJob))
            {
                Directory.CreateDirectory(directorioJob);
            }
            string fileName = Path.Combine(directorioJob, string.Format("{0}-{1}-{2:000}-{3:000}.xlsx", "IMG", control, thread.ThreadId, i));
            AlCrearElDirecrtorio?.Invoke(directorioJob, fileName, control, thread.ThreadId, i);

        }
    }

    public bool RecolectaArchivosYCarpetasDeControl(IEnumerable<ArchivosImagenes> imagenes, string fase = "CTL")
    {
        _imagenesPorProcesar = imagenes;
        if (string.IsNullOrEmpty(_directorioRaizControl))
            return false;
        var todosLosthreads = (from img in imagenes
                               group img by img.ThreadId into cnt
                               select new ThreadInfo() { ThreadId = cnt.Key, MaxJob = cnt.Max(x => x.Job) }).ToList();
        IList<ArchivosImagenes> controlRecuperado = new List<ArchivosImagenes>();

        foreach (var thread in todosLosthreads)
        {
            int threadId = thread.ThreadId;
            int maxRecords = thread.MaxJob;
            IEnumerable<string> archivos = RecorreDirectorios(_directorioRaizControl, threadId, maxRecords, fase);
            int i = 0;
            foreach (var archivo in archivos)
            {
                string narchivo = archivo.Replace("IMG-PD2-", "IMG-PD-") ;
                FileInfo fi = new(narchivo);
                if (fi.Exists)
                {
                    var recuperaImagenes = _servicioImagenes.CargaImagenesTratadas(narchivo).ToList();
                    ((List<ArchivosImagenes>)controlRecuperado).AddRange(recuperaImagenes);
                }
                i++;
                if (i % 10 == 0)
                    _logger.LogInformation("Se recuperó el archivo de control de imágenes {archivoControl} llevamos {numeroImagenes}", archivo, controlRecuperado.Count);
            }
        }
        _servicioImagenes.GuardarImagenes(controlRecuperado, string.Format(@"D:\202302 Digitalizacion\7. Descargas\Todo el control-{0}.xlsx", fase));
        _logger.LogInformation("Se guardaron las imagenes de control recuperadas {numeroImagenes}!!", controlRecuperado.Count);
        return true;
    }

    /// <summary>
    /// Regresa los archivos que serán recolectados
    /// </summary>
    /// <param name="directorioRaiz">Donde comienza la busqueda de información</param>
    /// <param name="thread">Información del hijo de ejecución a recorrer</param>
    /// <param name="job">Información de los máximos números de trabajo</param>
    /// <param name="control">Datos del archivo de control a obtener</param>
    /// <returns></returns>
    private static IEnumerable<string> RecorreDirectorios(string directorioRaiz, int threadId, int maxJobs, string control, bool exacta = false)
    {
        IList<string> archivo = new List<string>();
        string directorioACrear = Path.Combine(directorioRaiz, control); // Proceso de descarga
        string directorioThread = Path.Combine(directorioACrear, String.Format("{0:000}", threadId));
        int inicial = !exacta ? 1 : maxJobs;
        for (int i = inicial; i <= maxJobs; i++)
        {
            string directorioJob = Path.Combine(directorioThread, String.Format("{0:000}", i));
            string fileName = Path.Combine(directorioJob, string.Format("{0}-{1}-{2:000}-{3:000}.xlsx", "IMG", control, threadId, i));
            archivo.Add(fileName);
        }
        return archivo.ToList();
    }

    public IEnumerable<ArchivosImagenes> ProcesaArchivo(ArchivosImagenes archivoAProcesar, ref int subArchivo)
    {
        IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
        #region Informacion base a procesar
        resultado.Add(archivoAProcesar);

        string primerLimpieza = (archivoAProcesar.NombreArchivo ?? "").Replace(",", "_").Replace(";", "_").Replace(":", "_");
        string baseFileName = Path.Combine(archivoAProcesar.CarpetaDestino ?? "", primerLimpieza);
        FileInfo fileInfo = new(baseFileName);
        // La nueva carpeta tiene como nombre mas la extensión, con "_" en vez de "."
        string nuevaCarpetaDestino = string.Empty;
        try
        {
            nuevaCarpetaDestino = archivoAProcesar.CarpetaDestino + System.IO.Path.DirectorySeparatorChar + (fileInfo.Name ?? "").Replace(fileInfo.Extension, "_" + fileInfo.Extension.Replace(".", ""));
        }
        catch
        {
            nuevaCarpetaDestino = archivoAProcesar.CarpetaDestino + System.IO.Path.DirectorySeparatorChar + fileInfo.Name + "_";
            _logger.LogError("El nombre del archivo es anormal {fileName}", fileInfo.Name);
        }

        if (fileInfo.Exists)
        {
            archivoAProcesar.TamanioArchivo = fileInfo.Length;
            archivoAProcesar.Hash = fileInfo.ObtieneHashDelArchivo();
            if (archivoAProcesar.TamanioArchivo == 0)
            {
                archivoAProcesar.ErrorAlDescargar = true;
                archivoAProcesar.MensajeDeErrorAlDescargar = string.Format("Sin descarga el archivo {0}, tamaño 0", baseFileName);
            }
        }
        else
        {
            archivoAProcesar.ErrorAlDescargar = true;
            archivoAProcesar.MensajeDeErrorAlDescargar = string.Format("No existe el archivo {0}", baseFileName);
            return resultado;
        }
        #endregion
        #region Proceso Archivo zip
        if (fileInfo.Extension.Contains("zip", StringComparison.InvariantCultureIgnoreCase) || fileInfo.Extension.Contains("7z", StringComparison.InvariantCultureIgnoreCase))
        {
            /// Se calcula el directorio de destino
            //
            IList<ArchivosImagenes> nuevosArchivos = _procesaArchivoCompreso.ObtieneArchivosDesde(archivoAProcesar, fileInfo.FullName, nuevaCarpetaDestino, ref subArchivo).ToList();
            //((List<ArchivosImagenes>)resultado).AddRange(nuevosArchivos);
            foreach (ArchivosImagenes archivoNuevo in nuevosArchivos)
            {
                // Se vuelven a procesar en caso de que haya archivos zip o archivos por convertir a pdf
                IEnumerable<ArchivosImagenes> nuevosArchivosDetalle = ProcesaArchivo(archivoNuevo, ref subArchivo);
                ((List<ArchivosImagenes>)resultado).AddRange(nuevosArchivosDetalle);
            }
        }
        #endregion
        #region Procesa Archivos
        string[] extensionesDocumentosPorConvertir = new[] { "docx", "doc", "gif", "jfif", "jpeg", "jpg", "png", "tif", "msg" };
        if (extensionesDocumentosPorConvertir.Any(x => fileInfo.Extension.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
        {
            string directorioDestino = baseFileName.Replace(".", "_");
            ArchivosImagenes archivoConvertido = _procesaConversionArchivos.ConvierteArchivoDesde(archivoAProcesar, fileInfo.FullName, nuevaCarpetaDestino, ref subArchivo);
            // Se agrega el archivo siempre y cuando se pudo convertir
            if (!archivoConvertido.ErrorAlDescargar)
                ProcesaArchivo(archivoConvertido, ref subArchivo);
            ((List<ArchivosImagenes>)resultado).Add(archivoConvertido);
        }
        #endregion

        #region Procesa PDF
        if (fileInfo.Extension.Contains("pdf", StringComparison.InvariantCultureIgnoreCase))
        {
            try
            {
                archivoAProcesar.NumPaginas = _pdfInfo.DameElNumeroDePaginas(baseFileName);
            }
            catch
            {
                archivoAProcesar.ErrorAlDescargar = true;
                archivoAProcesar.MensajeDeErrorAlDescargar = string.Format("El archivo {0} no es un pdf válido", baseFileName);
            }
        }
        #endregion
        return resultado;
    }

    public bool ProcesaHiloYTrabajo(int threadId, int job)
    {
        IEnumerable<string> archivosPorProcesar = RecorreDirectorios(_directorioRaizControl, threadId, job, "PD", true);
        if (archivosPorProcesar.Any())
        {
            string firstFileName = archivosPorProcesar.FirstOrDefault() ?? "";
            if (File.Exists(firstFileName)) {
                _logger.LogInformation("Archivo previamente procesado {nombreArchivo}", firstFileName);
                return false;
            }
        }
        IEnumerable<string> archivos = RecorreDirectorios(_directorioRaizControl, threadId, job, "CTL", true);

        IList<ArchivosImagenes> imagenesDescargadasYProcesadas = new List<ArchivosImagenes>();

        foreach (string archivo in archivos)
        {
            FileInfo fi = new(archivo);
            if (fi.Exists)
            {
                var recuperaImagenes = _servicioImagenes.CargaImagenesTratadas(archivo).ToList();
                /*
                recuperaImagenes = (from arc in recuperaImagenes

                                        // where (new int[] { 2766, 2767, 4511, 4512, 4513 }).Any(x => arc.Id == x)
                                        // 148444,148445,148446,148447,148448,148449,148450,148451,148452,
                                        //9512,9513,40229,94290,94291,94292,94293,94761,94764,
                                        //148340,148341,148342,148343,148344,148345,148346,148347,148348

                                    where (new int[] { 232344 }).Any(x => arc.Id == x)
                                    select arc).ToList();
                */
                foreach (var imagen in recuperaImagenes)
                {
                    _procesaDescargaInformacion.DescargaArchivo(imagen);
                    int subArchivo = 0;
                    var resultado = ProcesaArchivo(imagen, ref subArchivo);
                    if (subArchivo != 0)
                        _logger.LogInformation("Se proceso un archivo!");
                    ((List<ArchivosImagenes>)imagenesDescargadasYProcesadas).AddRange(resultado);
                }
            }

            IEnumerable<string> archivosPorGuardar = RecorreDirectorios(_directorioRaizControl, threadId, job, "PD", true);
            foreach (var item in archivosPorGuardar)
            {
                _servicioImagenes.GuardarImagenes(imagenesDescargadasYProcesadas, item);
                _logger.LogInformation("Se guardo el resultado de la descarga en el archivo {fileDestination}!", item);
            }
        }
        return true;
    }

    public bool RecolectaArchivosYCarpetasDeControl(int maxThreadId, int maxJobId, string control = "CTL")
    {
        if (string.IsNullOrEmpty(_directorioRaizControl))
            return false;
        IList<ArchivosImagenes> controlRecuperado = new List<ArchivosImagenes>();
        int i= 0;
        for (int threadId = 1; threadId <= maxThreadId; threadId++)
        {
            for (int jobId = 1; jobId <= maxJobId; jobId++)
            {
                IEnumerable<string> archivos = RecorreDirectorios(_directorioRaizControl, threadId, jobId, control, true);
                foreach (var archivo in archivos)
                {
                    string narchivo = archivo; //.Replace("IMG-PD2-", "IMG-PD-");
                    FileInfo fi = new(narchivo);
                    if (fi.Exists)
                    {
                        var recuperaImagenes = _servicioImagenes.CargaImagenesTratadas(narchivo).ToList();
                        ((List<ArchivosImagenes>)controlRecuperado).AddRange(recuperaImagenes);
                        i++;
                        if (i%10 == 0)
                            _logger.LogInformation("Se recuperó el archivo de control de imágenes {archivoControl} llevamos {numeroImagenes}", archivo, controlRecuperado.Count);
                    }
                }

            }

        }
        _servicioImagenes.GuardarImagenes(controlRecuperado, string.Format(@"D:\202302 Digitalizacion\7. Descargas\Todo el control-{0}.xlsx", control));
        _logger.LogInformation("Se guardaron las imagenes de control recuperadas {numeroImagenes}!!", controlRecuperado.Count);
        return true;
    }

    public IEnumerable<ArchivosImagenes> CargaSoloPdf(string archivo="")
    {
        _logger.LogInformation("Comienzo la descarga de todas las imagenes");
        IEnumerable<ArchivosImagenes> imagenes;
        if (string.IsNullOrEmpty(archivo))
            imagenes = _servicioImagenes.CargaImagenesTratadas(_archivoSoloPDF);
        else
            imagenes = _servicioImagenes.CargaImagenesTratadas(archivo);

        _logger.LogInformation("Se tenían {numImagenes} por procesar", imagenes.Count());
        imagenes = imagenes.Where(x => (x.Extension ?? "").Contains("pdf", StringComparison.InvariantCultureIgnoreCase)).ToList();
        _logger.LogInformation("Quedaron {numImagenes} ya procesadas", imagenes.Count());

        _logger.LogInformation("Termino de cargar las imagenes");
        foreach (var imagen in imagenes)
        {
            if (!(imagen.Extension??"").Contains("pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            if ((imagen.TamanioArchivo != 0) && (imagen.NumPaginas == 0))
            {
                string baseFileName = Path.Combine(imagen.CarpetaDestino ?? "", imagen.NombreArchivo ?? "");
                try
                {
                    
                    imagen.NumPaginas = _pdfInfo.DameElNumeroDePaginas(baseFileName);
                    if (imagen.NumPaginas != 0)
                    {
                        imagen.ErrorAlDescargar = false;
                        imagen.MensajeDeErrorAlDescargar = string.Empty;
                        _logger.LogInformation("Se reparo el {nombreDelArchivo}, con {numPaginas} páginas", baseFileName, imagen.NumPaginas);
                    }
                }
                catch
                {
                    imagen.ErrorAlDescargar = true;
                    imagen.MensajeDeErrorAlDescargar = string.Format("El archivo {0} no es un pdf válido", baseFileName);
                    _logger.LogError("El archivo {nombreDelArchivo} no es un pdf válido", baseFileName);
                }
            }
            // string fullPath = Path.Combine(imagen.CarpetaDestino??"", imagen.NombreArchivo??"");
            string numCredito = Condiciones.ObtieneNumCredito(imagen.NombreArchivo??"");
            if (numCredito.Equals(Condiciones.C_STR_NULLO))
            {
                /// TODO: Agregar el número de crédito desde la ruta del archivo
                numCredito = Condiciones.ObtieneNumCreditoCarpeta(imagen.CarpetaDestino??"");
                if (numCredito.Equals(Condiciones.C_STR_NULLO))
                {
                    string ultimoRecurso = (imagen.CarpetaDestino ?? "").Replace(Path.DirectorySeparatorChar, ' ');
                    numCredito = Condiciones.ObtieneNumCredito(ultimoRecurso ?? "");
                }
            }
            if (numCredito.Equals(Condiciones.C_STR_NULLO))
            { 
                string ultimoRecurso = (imagen.ArchivoOrigen??"").Replace(Path.DirectorySeparatorChar, ' ');
                numCredito = Condiciones.ObtieneNumCredito(ultimoRecurso ?? "");
            }

            if ((imagen.NumCredito ?? "").Equals(Condiciones.C_STR_NULLO))
            {
                imagen.NumCredito = numCredito;
            }
        }
        var productos = _obtieneCatalogoProductos.ObtieneElCatalogoDelProducto();
        _logger.LogInformation("Se obtivieron {numProductos} productos de crédito", productos.Count());

        _logger.LogInformation("Se carga la información de los GuardaValores Previos");
        var guardaValores = _administraGuardaValores.CargaInformacionGuardaValoresPrevia();
        _logger.LogInformation("Se cargaron {numGuardaValores} Guarda Valores previos", guardaValores.Count());

        _logger.LogInformation("Carga la información para las consultas de imágenes (Información de Expedientes)");
        var ministracionesMesa = _ministracionesMesaService.GetMinistraciones();
        _logger.LogInformation("Se cargaron {numMinistraciones} ministraciones de la mesa", ministracionesMesa.Count());

        _logger.LogInformation("Se carga la información de los saldos activos");
        var saldosActivos = _abSaldosActivosService.GetABSaldosActivos();
        _logger.LogInformation("Se cargaron {numSaldosActivos} expedientes activos", saldosActivos.Count());

        _logger.LogInformation("Se carga la información de los saldos con Castigo");
        var saldosConCastigo = _abSaldosConCastigoService.ObtieneABSaldosConCastigoProcesados(); // _archivoSaldosConCastigoProcesado
        _logger.LogInformation("Se cargaron {numSaldosConCastigo} saldos clasificados con Castigos", saldosConCastigo.Count());

        #region Comienzan los cruces
        _cruceInformacion.CruzaInformacionTipoProductoSaldosActivos(ref saldosActivos, ref productos);
        _cruceInformacion.CruzaInformacionTipoProductoSaldosConCastigo(ref saldosConCastigo, ref productos);
        _cruceInformacion.CruzaInformacionMesaConImagenes(ref ministracionesMesa, ref imagenes);
        _cruceInformacion.CruzaInformacionAbSaldosImagenes(ref saldosActivos, ref imagenes);
        _cruceInformacion.CruzaInformacionAbSaldosCorporativoImagenes(ref saldosConCastigo, ref imagenes);
        _cruceInformacion.CruzaInformacionGvImagenes(ref guardaValores, ref imagenes);

        _servicioImagenes.GuardarImagenes(imagenes, _archivoSoloPDFProcesado);
        _administraGuardaValores.GuardaInformacionGuardaValores(guardaValores);
        _ = _abSaldosActivosService.GuardaABSaldosActivos(saldosActivos);
        _logger.LogInformation("Se escribió la información de los saldos de créditos activos!!!");
        _abSaldosConCastigoService.GuardaInformacionABSaldos(_archivoSaldosConCastigoProcesados, saldosConCastigo);
        _logger.LogInformation("Se escribió la información de los créditos cancelados!!!");
        _ = _ministracionesMesaService.GuardaMinistraciones(ministracionesMesa);
        _logger.LogInformation("Se escribió la información de las ministraciones de la mesa!!!");


        #endregion
        return imagenes;
    }

    /// <summary>
    /// De PD -> Se recolectan imagenes, se limpian las de errores (ver archivo Todos los PDFs procesados Sin Error.xlsx) y aqui se requiere no recorrer los archivos que son iguales (hash duplicados)
    /// </summary>
    /// <param name="imagenes">Imagenes con hash duplicados</param>
    /// <returns>Verdadero si marca error al ejecutar</returns>
    public bool CreaArchivosYCarpetasDeControlOcr(IEnumerable<ArchivosImagenes> imagenes)
    {
        imagenes = imagenes.OrderBy(x => x.Hash).ToList();

        // Corrijo los que su hash sea blanco.
        var imagenesSinHash = imagenes.Where(x => string.IsNullOrWhiteSpace(x.Hash ?? "") || string.IsNullOrEmpty(x.Hash ?? "")).ToList();
        foreach (var imagen in imagenesSinHash)
        {
            FileInfo fi = new(Path.Combine(imagen.CarpetaDestino??"",imagen.NombreArchivo??""));
            if (fi.Exists)
            {
                imagen.Hash = fi.ObtieneHashDelArchivo();
                if (string.IsNullOrWhiteSpace(imagen.Hash)) {
                    throw new Exception("Se pasó!!!");
                }
            }
        }

        /*

        listaDatos.GroupBy(x => x.CampoAgrupado)
                           .Select(grupo => grupo.FirstOrDefault());
        */
        var resultado = imagenes.GroupBy(x => x.Hash).Select(grupo => new { Imagen = grupo.First() }).Select(x=>x.Imagen).ToList();

        _imagenesPorProcesar = resultado;
        if (string.IsNullOrEmpty(_directorioRaizControl))
            return false;
        var todosLosthreads = (from img in _imagenesPorProcesar
                               group img by img.ThreadId into cnt
                               select new ThreadInfo() { ThreadId = cnt.Key, MaxJob = cnt.Max(x => x.Job) }).ToList();

        foreach (var thread in todosLosthreads)
        {
            int threadId = thread.ThreadId;
            int maxRecords = thread.MaxJob;

            if (!Directory.Exists(_directorioRaizControl))
            {
                Directory.CreateDirectory(_directorioRaizControl);
            }

            /*
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "CTL"); // Informacion de Control
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PD"); // Procesos de Descarga
            */
            AlCrearElDirecrtorio += GuardaInformacionControlOcr;
            CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PO"); // Procesos de Ocr
            AlCrearElDirecrtorio -= GuardaInformacionControlOcr;
            // CreaDirectorios(_directorioRaizControl, thread, maxRecords, "AO"); // Procesos de Analisis Ocr
            // CreaDirectorios(_directorioRaizControl, thread, maxRecords, "PA"); // Procesos para acomodar las imagenes
            // CreaDirectorios(_directorioRaizControl, thread, maxRecords, "FIN"); // Imagenes terminadas
        }
        return true;
    }

    private void GuardaInformacionControlOcr(string directoryName, string fileName, string control, int thread, int job)
    {
        if (_imagenesPorProcesar != null)
        {
            if (control.Equals("PO", StringComparison.InvariantCultureIgnoreCase))
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                var filtraImagenes = (from img in _imagenesPorProcesar
                                      where img.ThreadId == thread && img.Job == job
                                      select img).ToList();
                _servicioImagenes.GuardarImagenes(filtraImagenes, fileName);
                if (!filtraImagenes.Any())
                    _logger.LogInformation("Debe crear el archivo {nombreArchivo} y {existe} existe", fileName, File.Exists(fileName) ? "Si" : "No");
                if (job % 10 == 0)
                    _logger.LogInformation("Se guardó el archivo de control de imágenes {archivoControl} con {numeroImagenes}", fileName, filtraImagenes.Count);
            }
        }
    }
}
