using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using gob.fnd.Infraestructura.Negocio.Procesa.BienesAdjudicados;
using gob.fnd.Infraestructura.Negocio.Procesa.Control;
using gob.fnd.Infraestructura.Negocio.Procesa.Control.Descarga;
using gob.fnd.Infraestructura.Negocio.Procesa.Control.File2Pdf;
using gob.fnd.Infraestructura.Negocio.Procesa.Control.ZIP;
using gob.fnd.Infraestructura.Negocio.Procesa.Main;
using gob.fnd.Infraestructura.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using Jaec.Helper.IoC;
using Microsoft.Extensions.DependencyInjection;


namespace gob.fnd.Infraestructura.Negocio.Procesa.IOC;
public class OrquestaDI : IJaecContainer
{

    public void Load(IServiceCollection services)
    {
        services.AddScoped<IMainControlApp, MainApp>();
        services.AddScoped<IAdministraImagenesService, AdministraImagenesService>();
        services.AddScoped<IProcesaConversionArchivos, ProcesaConversionArchivosService>();
        services.AddScoped<IProcesaArchivoCompreso, ProcesaArchivoCompreso>();
        services.AddScoped<IDescargaInformacionOneDrive, DescargaInformacionOneDriveService>();
        services.AddScoped<IProcesaDescargaInformacion, ProcesaDescargaInformacionService>();
        services.AddScoped<IAnalizaResultadoOCR, AnalizaResultadoOCRService>();
        services.AddScoped<IAnalizaInformacionBienesAdjudicados, AnalizaInformacionBienesAdjudicadosService>();
    }
}