using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Zip.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IDescomprimeArchivoZip, DescomprimeArchivoZipService>();
            services.AddScoped<IComprimeArchivoZIP, ComprimeArchivoZIPService>();
        }
    }
}
