using gob.fnd.Dominio.Digitalizacion.Negocio.Carga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion;
using gob.fnd.Dominio.Digitalizacion.Negocio.Descarga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Directorios;
using gob.fnd.Dominio.Digitalizacion.Negocio.Tratamientos;
using gob.fnd.Infraestructura.Negocio.Carga;
using gob.fnd.Infraestructura.Negocio.Consultas;
using gob.fnd.Infraestructura.Negocio.Descarga;
using gob.fnd.Infraestructura.Negocio.Directorios;
using gob.fnd.Infraestructura.Negocio.Main;
using gob.fnd.Infraestructura.Negocio.Tratamientos;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.IOC;          
public class OrquestaDI : IJaecContainer
{

    public void Load(IServiceCollection services)
    {
        services.AddScoped<IExploraCarpetas, ExploraCarpetas>();
        services.AddScoped<IMainControlApp, MainApp>();
        services.AddScoped<ICruceInformacion, CruceInformacion.CruceInformacion>();
        services.AddScoped<IOperacionesTratamientos, OperacionesTratamientosService>();
        services.AddScoped<IDescargaExpedientes, DescargaExpedientesService>();
        services.AddScoped<IConsultaServices, ConsultaServices>();
        services.AddScoped<ICargaImagenes, CargaImagenService>();
        // services.AddScoped<IProcesaParametrosCarta, ProcesaParametrosCarta>();

        // throw new NotImplementedException();
    }
}
