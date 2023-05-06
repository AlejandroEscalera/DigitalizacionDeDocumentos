using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using gob.fnd.Infaestructura.Negocio.Ocr.Main;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infaestructura.Negocio.Ocr.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IMainControlApp, MainApp>();
            services.AddScoped<IAdministraAcomodaExpedientes, AdministraAcomodaExpedientesService>();
            services.AddTransient<IAdministraOperacionesOCRService, AdministraOperacionesOCRService>();
            services.AddScoped<IAnalisisOcr, AnalisisOcrService>();
            services.AddScoped<IAplicaOcr, AplicaOcrServices>();

        }
    }
}
