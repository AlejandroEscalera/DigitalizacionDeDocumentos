using AppConsultaImagen;
using gob.fnd.Dominio.Digitalizacion.Negocio.Config;
using InformacionCartera;
using Jaec.Helper.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reactive.Linq;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using System.ComponentModel;

namespace AppConsultaImagen;

public class WindowsFormsGlobalInformation
{
    public EventoActualizaLog AlActualizaLog { get; set; }

    private static WindowsFormsGlobalInformation? _instance;

    /// <summary>  
    /// Instance will set only once initially when _insatnce is null  
    /// </summary>  
    public static WindowsFormsGlobalInformation Instance => _instance ??= new WindowsFormsGlobalInformation();

    private IMainControlApp? _mainApp;
    private IHost? _host;
    private IConfiguration _config;
    private Microsoft.Extensions.Logging.ILogger _logger;
    private ICollection<LogEvent>? _logItems;
    private IObtieneArchivoConfiguracion? _obtieneArchivoConfiguracion;
    private IConsultaServices _consultaServices;

    public IMainControlApp? MainApp { get { return _mainApp; } }
    public IHost? HostApp { get { return _host; } }
    public IConfiguration Config { get { return _config; } }
    public Microsoft.Extensions.Logging.ILogger Logger { get { return _logger; } }

    public IObtieneArchivoConfiguracion? ObtieneArchivoConfiguracion { get { return ActivaInformacionInterna(); } }

    public ICollection<LogEvent>? LogItems { get { return _logItems; } }

    private void EventoAlActualizaLog(LogEvent evt)
    {
        // Solo para quitar la advertencia, no hay código, salvo que ponga ya un logger
        _logItems?.Add(evt);
    }
    public WindowsFormsGlobalInformation LlenaInformacionGlobal()
    {

        // if (AlActualizaLog == null)
        //{
        AlActualizaLog += EventoAlActualizaLog;
        //}


        #region Aqui es donde hay que incluir el builder para el DI/IOC
        var builder = new ConfigurationBuilder();
        //IConfiguration 
        _config = builder.BuildConfig();
        _logItems = new ObservableCollection<LogEvent>();

        #region Inicializo el Logger
        string logDirectory = string.Empty;
        Assembly assembly = Assembly.GetExecutingAssembly();
        if (assembly is not null)
            logDirectory = Path.GetDirectoryName(assembly.Location) ?? "";
        // Uso el configurator por cualquier nueva inicialización
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_config)
            .Enrich.FromLogContext()
            .WriteTo.File(string.Format("{0}\\MensajeError.log", logDirectory))
            .WriteTo.Observers(events => events
             .Do(evt => AlActualizaLog?.Invoke(evt)) // Cada vez que se registra un evento, se registra dentro del listado y se llama en cascada las suscripciones
             .Subscribe())
            .CreateLogger();
        #endregion

        /// Aviso que comenzó la aplicación
        var logger = new CustomSerilogger(Log.Logger);

        /// Agrego el Servicio principal (tiene el código de ejecución) MainApp
        /// 
        /// Nota: Se hace así para que pueda usarse la Inversión de Control, 
        /// y así mandar los parámetros del Logger, de las configuraciones 
        /// y otros temas que se requieran
        /// 
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                /// Esta rutina hace la carga dinámica de las librerías, 
                /// busca los containers de DI, y los manda llamar
                /// De esta manera las implementaciones son independientes
                /// Del resto de las definiciones y se mantiene
                /// Un aislamento completo
                logger.LogInformation("Comienza la carga de las librerías");
                try
                {
                    services.LoadAllDinamicAssemblies(_config, logger);
                    logger.LogInformation("Termino con la carga de las librerías");
                }
                catch (Exception ex)
                {
                    logger.LogError("No se cargaron las librerias {mensaje}", ex.Message);
                }
                
            })
            .UseSerilog()
            .Build();

        // IMainControlApp? svc = host.Services.SvcRun(_config);
        #endregion

        // _mainApp = mainApp;
        _host = host;
        _logger = logger;
        return Instance;
    }

    /// <summary>
    /// Permite ejecutar el proceso, es el mismo código de la consola con la diferencia de que el logger apuntará a un gridview
    /// para mostrar los avances en pantalla
    /// </summary>
    /// <returns></returns>
    public IMainControlApp? RunApp()
    {
        if (_host != null)
        {
            _logger.LogInformation("Comienza el proceso de ABSaldos!!!");
            _mainApp = _host.Services.SvcRun(_config);
            _logger.LogInformation("Terminó el procedimiento!!!");
        }
        return _mainApp;
    }

    public IObtieneArchivoConfiguracion? ActivaInformacionInterna()
    {
        try
        {
            if (_obtieneArchivoConfiguracion != null)
                return _obtieneArchivoConfiguracion;
            if (_host != null)
            {
                Assembly? mainAssembly = _config.GetMainAssembly();
                if (mainAssembly != null)
                {

                    Type? tipoCarga = mainAssembly.GetAuxiliarType(_config);
                    if (tipoCarga != null)
                    {
                        _obtieneArchivoConfiguracion = (IObtieneArchivoConfiguracion)ActivatorUtilities.CreateInstance(_host.Services, tipoCarga);
                    }
                }
            }
            return _obtieneArchivoConfiguracion;
        }
        catch   (Exception ex)
        {
            _logger.LogError("Error durante la activación de Informacion Interna {mensaje}", ex.Message);
            return null;

        }
    }

    public IConsultaServices ActivaConsultasServices()
    {
        try
        {
            if (_consultaServices != null)
                return _consultaServices;
            if (_host != null)
            {
                Assembly? mainAssembly = _config.GetMainAssembly();

                if (mainAssembly != null)
                {

                    Type? tipoCarga = mainAssembly.GetAuxiliarType(_config);
                    if (tipoCarga != null)
                    {
                        _consultaServices = (IConsultaServices)ActivatorUtilities.CreateInstance(_host.Services, tipoCarga);
                    }
                }
            }
            if (_consultaServices is null)
            {
                _logger.LogError("Error, no se pudo cargar la librería gob.fnd.Infraestructura.Negocio");
                throw new Exception("Error, no se pudo cargar la librería gob.fnd.Infraestructura.Negocio");
            }
            return _consultaServices;
        }
        catch (System.Exception ex) {
            _logger.LogError("Error {mensaje}", ex.Message);
            throw;
        }
    }

#pragma warning disable CS8618 // Es un singlweton, Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
    private WindowsFormsGlobalInformation()
#pragma warning restore CS8618 // Es un singlweton, Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
    {

    }
}
