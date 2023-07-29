using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Rar;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Rar.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IDescomprimeArchivoRar, DescomprimeArchivoRarService>();
        }
    }
}
