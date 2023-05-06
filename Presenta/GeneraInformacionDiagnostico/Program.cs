using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Jaec.Helper.IoC;
using System;

namespace GeneraInformacionDiagnostico;
// Note: actual namespace depends on the project name.
internal class Program
{
    static void Main() // string[] args
    {
        var builder = new ConfigurationBuilder();
        IConfiguration _config = builder.BuildConfig();

        #region Inicializo el Logger
        // Uso el configurator por cualquier nueva inicialización
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_config)
            .Enrich.FromLogContext()
            .WriteTo.File("D:\\202302 Digitalizacion\\InfoTona.log")
            .WriteTo.Console()
            .CreateLogger();
        #endregion

        /// Aviso que comenzó la aplicación
        var logger = new CustomSerilogger(Log.Logger);
        logger.LogInformation("Application starting!!!");

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
                services.LoadAllDinamicAssemblies(_config, logger);
            })
            .UseSerilog()
            .Build();

        /// Se inicializa el servicio que arranca los servicios, en este caso la clase se llama
        /// MainApp y se ejecuta (run)
        IMainControlApp? svc = host.Services.SvcRun(_config);
    }

}