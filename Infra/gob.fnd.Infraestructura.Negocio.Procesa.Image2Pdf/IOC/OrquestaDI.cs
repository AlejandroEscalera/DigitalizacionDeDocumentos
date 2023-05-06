using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Image2Pdf.IOC
{
    public class OrquestaDI : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IImagen2Pdf, Image2PdfService>();
            services.AddScoped<IPdfInfo, PdfInfoServices>();
            services.AddScoped<ITiff2Pdf, Tiff2PdfService>();

        }
    }
}
