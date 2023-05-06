using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv.IOC
{
    public class OrquestaDI : IJaecContainer
    {

        public void Load(IServiceCollection services)
        {
            // services.AddScoped<IMainControlApp, MainApp>();
            services.AddScoped<IAdministraCargaCorteDiario, AdministraCargaCorteDiarioService>();
            services.AddScoped<IAdministraCartera, AdministraCarteraService>();
            services.AddScoped<IAdministraCargaConsulta, AdministraCargaConsultaService>();
            services.AddScoped<ICreaArchivoCsv, CreaArchivoCsvService>();
            services.AddScoped<IAdministraABSaldosDiario, AdministraABSaldosDiarioService>();
            services.AddScoped<IAdministraCargaCreditosCancelados, AdministraCreditosCanceladosService>();
            services.AddScoped<IAdministraCargaBienesAdjudicados, AdministraBienesAdjudicadosService>();
        }
    }
}