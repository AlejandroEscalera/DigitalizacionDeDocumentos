using SharpCompress.Archives;
using SharpCompress.Common;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Rar;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using Microsoft.Extensions.Logging;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Rar;

public class DescomprimeArchivoRarService : IDescomprimeArchivoRar
{
    private readonly ILogger<DescomprimeArchivoRarService> _logger;

    public DescomprimeArchivoRarService(ILogger<DescomprimeArchivoRarService> logger)
    {
        _logger = logger;
    }
    public IList<string> DescomprimeArchivo(string fileName, string carpetaDestino)
    {
        IList<string> listaArchivos = new List<string>();
        if (!Directory.Exists(carpetaDestino))
            Directory.CreateDirectory(carpetaDestino);

        using (var archive = ArchiveFactory.Open(fileName))
        {
            foreach (var entry in archive.Entries)
            {
                string rutaArchivoDescomprimido = Path.Combine(carpetaDestino, entry.Key).Replace(",", "_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaArchivoDescomprimido);
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(rutaArchivoDescomprimido ?? "");
                    continue;
                }
                #region Se crea el directorio destino, en caso de no existir
                if (nombreDirectorioDestino != null && nombreDirectorioDestino.Length > 0)
                {
                    Directory.CreateDirectory(nombreDirectorioDestino);
                }
                #endregion

                if (!entry.IsDirectory)
                {
                    try
                    {
                        entry.WriteToDirectory(carpetaDestino, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                        listaArchivos.Add(entry.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{mensaje}", ex.Message);
                    }
                }
            }
        }
        return listaArchivos;
    }

    public async Task<IList<string>> DescomprimeArchivoAsync(string fileName, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> progress)
    {
        IList<string> listaArchivos = new List<string>();
        if (!Directory.Exists(carpetaDestino))
            Directory.CreateDirectory(carpetaDestino);

        using (var archive = ArchiveFactory.Open(fileName))
        {
            int totalArchivos = archive.Entries.Count();
            int archivosCompletados = 0;

            foreach (var entry in archive.Entries)
            {
                string rutaArchivoDescomprimido = Path.Combine(carpetaDestino, entry.Key).Replace(",", "_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaArchivoDescomprimido);
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(rutaArchivoDescomprimido ?? "");
                    continue;
                }
                #region Se crea el directorio destino, en caso de no existir
                if (nombreDirectorioDestino != null && nombreDirectorioDestino.Length > 0)
                {
                    Directory.CreateDirectory(nombreDirectorioDestino);
                }
                #endregion

                if (!entry.IsDirectory)
                {
                    try
                    {
                        await Task.Run(() => entry.WriteToDirectory(carpetaDestino, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        }));
                        archivosCompletados++;
                        ReporteProgresoDescompresionArchivos reporte = new()
                        {
                            ArchivoProcesado = archivosCompletados,
                            CantidadArchivos = totalArchivos,
                            InformacionArchivo = entry.Key
                        };
                        progress.Report(reporte);
                        listaArchivos.Add(entry.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{mensaje}", ex.Message);
                    }
                }
            }
        }
        return listaArchivos;
    }
}
