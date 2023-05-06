using Microsoft.Extensions.Configuration;

namespace Jaec.Helper.IoC
{
    public static class ConfigurationBuilderExtension
    {
        /// <summary>
        /// Es para inicializar la Configuración del Inyector de Dependencias
        /// Donde le decimos que la configuración puede venir del archivo appsettings.json
        /// Tambien del archivo de appsettings que depende del ambiente que estás ejecutando}
        /// y que maneja el visual studio y el visual studio code como una variable de
        /// ambiente de DOS llamada ASPNETCORE_ENVIRONMENT
        /// </summary>
        /// <param name="builder">Patrón que realizará la construcción de objetos dentro del aplicativo</param>
        public static IConfiguration BuildConfig(this ConfigurationBuilder builder) {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true).
                AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
