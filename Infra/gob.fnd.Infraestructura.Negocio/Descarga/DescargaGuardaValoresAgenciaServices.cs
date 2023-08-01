using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Negocio.Descarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Descarga;

public class DescargaGuardaValoresAgenciaServices : IDescargaGuardaValoresAgencia
{
    public async Task<bool> DescargaGuardaValoresAgencia(IEnumerable<GuardaValores> guardaValores, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> avance)
    {
        try
        {
            int cantidadArchivos = guardaValores.Count();
            int noArchivo = 1;
            foreach (var archivo in guardaValores)
            {
                if (!string.IsNullOrEmpty(archivo.Imagen))
                {
                    string nombreArchivoACopiar = archivo.Imagen ?? "";
                    string soloNombreArchivoACopiar = Path.GetFileName(nombreArchivoACopiar);
                    string archivoDestino = carpetaDestino + string.Format(@"{0:000}/{1:000}/{2:000}/{4}/{3}", archivo.Regional, archivo.Sucursal, archivo.NumContrato, soloNombreArchivoACopiar, (archivo.TieneTurnoCobranza || archivo.TieneTurnoJuridico)?"T":"GV");
                    string directorioDestino = Path.GetDirectoryName(archivoDestino) ?? "";
                    if (!Directory.Exists(directorioDestino))
                    {
                        Directory.CreateDirectory(directorioDestino);
                    }
                    if (File.Exists(archivoDestino))
                    {
                        File.Delete(archivoDestino);
                    }
                    var reporteProgresoDescompresionArchivos = new ReporteProgresoDescompresionArchivos
                    {
                        ArchivoProcesado = noArchivo,
                        CantidadArchivos = cantidadArchivos,
                        InformacionArchivo = soloNombreArchivoACopiar
                    };
                    await Task.Run(() => File.Copy(nombreArchivoACopiar, archivoDestino, true));
                    noArchivo++;
                    avance.Report(reporteProgresoDescompresionArchivos);
                    await Task.Delay(1);
                }
            }
            return true;
        }
        catch 
        {
            return false;
        }
    }
}
