using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Descarga.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IDescargaInformacionSharePoint,DescargaInformacionSharePointService>();
        }
    }
}
