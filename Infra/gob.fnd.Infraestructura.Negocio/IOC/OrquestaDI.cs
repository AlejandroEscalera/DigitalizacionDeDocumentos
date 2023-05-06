using gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion;
using gob.fnd.Dominio.Digitalizacion.Negocio.Directorios;
using gob.fnd.Infraestructura.Negocio.Directorios;
using gob.fnd.Infraestructura.Negocio.Main;
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
        // services.AddScoped<IProcesaParametrosCarta, ProcesaParametrosCarta>();

        // throw new NotImplementedException();
    }
}
