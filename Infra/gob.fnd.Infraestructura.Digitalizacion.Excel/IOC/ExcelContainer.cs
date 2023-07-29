using gob.fnd.Dominio.Digitalizacion.Excel;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Excel.Arqueos;
using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Excel.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Dominio.Digitalizacion.Excel.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Juridico;
using gob.fnd.Dominio.Digitalizacion.Excel.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Tratamientos;
using gob.fnd.Infraestructura.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Arqueos;
using gob.fnd.Infraestructura.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Cancelados;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Config;
using gob.fnd.Infraestructura.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Excel;
using gob.fnd.Infraestructura.Digitalizacion.Excel.GuardaValores;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Imagenes;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Juridico;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Liquidaciones;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Ministraciones;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.IOC
{
    public class ExcelContainer : IJaecContainer
    {
        public void Load(IServiceCollection services)
        {
            services.AddScoped<IObtieneCatalogoProductos, ObtieneCatalogoProductos>();
            services.AddScoped<IServicioABSaldosConCastigo, ServicioABSaldosConCastigo>();
            services.AddScoped<IObtieneCorreosAgentes, ObtieneCorreosAgentes>();
            services.AddScoped<IAnalisisArqueos, AnalisisArqueos>();
            services.AddScoped<IAnalisisCamposGuardaValores, AnalisisCamposGuardaValores>();
            services.AddScoped<IAdministraGuardaValores, AdministraGuardaValores>();
            services.AddScoped<IServicioImagenes, ServicioImagenes>();
            services.AddScoped<IServicioABSaldosActivos, ServicioABSaldosActivos>();
            services.AddScoped<IServicioDirToXls, ServicioDirToXls>();
            services.AddScoped<IServicioMinistracionesMesa, ServicioMinistracionesMesa>();
            services.AddScoped<IServicioCancelados, ServicioCancelados>();
            services.AddScoped<IServicioABSaldosRegionales, ServicioABSaldosRegionales>();
            services.AddScoped<IBienesAdjudicados, ServicioBienesAdjudicados>();
            services.AddScoped<IBienesAdjudicadosIdentificados, ServicioBienesAdjudicadosIdentificados>();
            services.AddScoped < ILiquidaciones, LiquidacionesService>();
            services.AddScoped<ITratamientos, Tratamientos.TratamientosService>();
            services.AddScoped<IJuridico, JuridicoService>();
        }
    }
}
