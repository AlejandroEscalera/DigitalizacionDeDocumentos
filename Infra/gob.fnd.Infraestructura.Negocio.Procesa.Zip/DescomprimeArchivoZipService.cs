using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Zip;

public class DescomprimeArchivoZipService : IDescomprimeArchivoZip
{
    private readonly ILogger<DescomprimeArchivoZipService> _logger;
    const int I_BUFFER_SIZE = 4096;

    public DescomprimeArchivoZipService(ILogger<DescomprimeArchivoZipService> logger)
    {
        _logger = logger;
    }
    public IList<string> DescomprimeArchivo(string fileName, string carpetaDestino)
    {
        IList<string> lista = new List<string>();
        ZipFile file;
        FileStream? fs ;
        try
        {
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            fs = File.OpenRead(fileName);
            file = new ZipFile(fs);
            // Se recorren todos los archivos contenidos en el ZIP
            foreach (ZipEntry archivoCompreso in file)
            {
                // Valido que el archivo compreso sea un archivo, si es una carpeta, lo ignoro!!!!
                if (!archivoCompreso.IsFile)
                {
                    continue;
                }
                // Obtengo el nombre del archivo y le agrego la fecha de descompresión
                String nombreArchivoADescomprimir = archivoCompreso.Name;

                // un buffer de 4k es optimo para esta operación, cuando se manipulen archivos grandes
                byte[] buffer = new byte[I_BUFFER_SIZE];

                // Cargo el encabezado del archivo en turno por descomprimir
                Stream zipStream = file.GetInputStream(archivoCompreso);

                // Manipulate the output filename here as desired.
                #region Ajustes sobre el archivo destino, como agregar el directorio de salida, y si no existe el directorio, lo creamos
                /// FIX: Se eliminó nombres con "," o con ";", reemplazandose con "_"
                String rutaCompletaDelArchivoADescomprimir = Path.Combine(carpetaDestino, nombreArchivoADescomprimir).Replace(",","_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaCompletaDelArchivoADescomprimir);

                #region Se crea el directorio destino, en caso de no existir
                if (nombreDirectorioDestino != null && nombreDirectorioDestino.Length > 0)
                {
                    Directory.CreateDirectory(nombreDirectorioDestino);
                }
                #endregion
                #endregion

                #region Se vacía el ZIP en el directorio
                using (FileStream streamWriter = File.Create(rutaCompletaDelArchivoADescomprimir))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
                #endregion
                lista.Add(rutaCompletaDelArchivoADescomprimir);
            }
        }
        catch (Exception ex) {
            _logger.LogError("{error}", ex.Message);
        }

        return lista.ToList();

    }

    public async Task<IList<string>> DescomprimeArchivoAsync(string fileName, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> progress)
    {
        IList<string> lista = new List<string>();
        ZipFile file;
        FileStream? fs;
        try
        {
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            fs = File.OpenRead(fileName);
            file = new ZipFile(fs);
            long totalArchivos = file.Count;
            int archivosCompletados = 0;

            // Se recorren todos los archivos contenidos en el ZIP
            foreach (ZipEntry archivoCompreso in file)
            {
                // Valido que el archivo compreso sea un archivo, si es una carpeta, lo ignoro!!!!
                if (!archivoCompreso.IsFile)
                {
                    continue;
                }
                // Obtengo el nombre del archivo y le agrego la fecha de descompresión
                String nombreArchivoADescomprimir = archivoCompreso.Name;

                // un buffer de 4k es optimo para esta operación, cuando se manipulen archivos grandes
                byte[] buffer = new byte[I_BUFFER_SIZE];

                // Cargo el encabezado del archivo en turno por descomprimir
                Stream zipStream = file.GetInputStream(archivoCompreso);

                // Manipulate the output filename here as desired.
                #region Ajustes sobre el archivo destino, como agregar el directorio de salida, y si no existe el directorio, lo creamos
                /// FIX: Se eliminó nombres con "," o con ";", reemplazandose con "_"
                String rutaCompletaDelArchivoADescomprimir = Path.Combine(carpetaDestino, nombreArchivoADescomprimir).Replace(",", "_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaCompletaDelArchivoADescomprimir);

                #region Se crea el directorio destino, en caso de no existir
                if (nombreDirectorioDestino != null && nombreDirectorioDestino.Length > 0)
                {
                    Directory.CreateDirectory(nombreDirectorioDestino);
                }
                #endregion
                #endregion

                await Task.Run(() =>
                {
                    #region Se vacía el ZIP en el directorio
                    using (FileStream streamWriter = File.Create(rutaCompletaDelArchivoADescomprimir))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                    #endregion
                });
                archivosCompletados++;
                ReporteProgresoDescompresionArchivos reporte = new()
                {
                    ArchivoProcesado = archivosCompletados,
                    CantidadArchivos = totalArchivos,
                    InformacionArchivo = nombreArchivoADescomprimir
                };
                progress.Report(reporte);

                lista.Add(rutaCompletaDelArchivoADescomprimir);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("{error}", ex.Message);
        }

        return lista.ToList();
    }
}
