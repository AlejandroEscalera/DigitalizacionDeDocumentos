using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using Jaec.Helper.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace gob.fnd.Infaestructura.Negocio.Ocr.Main
{
    public class ThreadData
    {
        public int ThreadId { get; set; }
        public IAdministraOperacionesOCRService? AdministraOperacionesOCRService { get; set; }
    }

    public class MainApp : IMainControlApp
    {
        private readonly ILogger<MainApp> _logger;
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private readonly IServiceProvider _services;

        public MainApp(ILogger<MainApp> logger, IConfiguration configuration, IServiceProvider services)
        {
            _logger = logger;
            _configuration = configuration;
            _services = services;
        }


        public static async Task DoOcr(ThreadData data) 
        { 
            IAdministraOperacionesOCRService? administraOperacionesOCRService = data.AdministraOperacionesOCRService;

            if (administraOperacionesOCRService is not null)
            for (int i = 2; i < 3; i++) // 140
            {
                await Task.Delay(1);
                administraOperacionesOCRService.ProcesaHiloYTrabajo(data.ThreadId, i);
                await Task.Delay(1);
            }

        }
        public async void Run()
        {
            int numeroDeThreads = 1;
            Task[] arregloDeHilos = new Task[numeroDeThreads];
            for (int i = 0; i < numeroDeThreads; i++)
            {
                var administraOperacionesOCRService = _services.GetService<IAdministraOperacionesOCRService>();
                ThreadData data = new()
                {
                    ThreadId = 111 + 1, // i + 1,
                    AdministraOperacionesOCRService = administraOperacionesOCRService
                };
                arregloDeHilos[i] = DoOcr(data);
            }
            _logger.LogInformation("Se crearon todos los threads, se procede a su ejecución");
            await Task.WhenAll(arregloDeHilos);
            _logger.LogInformation("Terminó la ejecución");

        }
    }
}
