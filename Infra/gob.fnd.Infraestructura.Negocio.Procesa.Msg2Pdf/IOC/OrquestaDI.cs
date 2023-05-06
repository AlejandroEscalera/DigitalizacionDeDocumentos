using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Msg2Pdf.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IMsg2Pdf, Msg2PdfService>();
        }
    }
}


