using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sewer56.Update.Packaging.Extractors;
using Microsoft.Extensions.Logging;

namespace gob.fnd.Infraestructura.Negocio.Procesa._7zip
{
    public class DescomprimeArchivo7zService : IDescomprimeArchivo7z
    {
        private readonly ILogger<DescomprimeArchivo7zService> _logger;

        public DescomprimeArchivo7zService(ILogger<DescomprimeArchivo7zService> logger)
        {
            _logger = logger;
        }
        public IList<string> DescomprimeArchivo(string fileName, string carpetaDestino)
        {
            IList<string> list = new List<string>();

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            Stream stream = File.OpenRead(fileName);
            SevenZip.SevenZipExtractor extractor = new(stream);
            var archivos = extractor.ArchiveFileData;
            foreach (var archivo in archivos)
            {
                /// FIX: Se eliminó nombres con "," o con ";", reemplazandose con "_"
                string rutaArchivoDescomprimido = Path.Combine(carpetaDestino, archivo.FileName).Replace(",","_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaArchivoDescomprimido);
                if (archivo.IsDirectory)
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

                Stream outStream = File.Create(rutaArchivoDescomprimido);
                try
                {
                    extractor.ExtractFile(archivo.FileName, outStream);
                    list.Add(rutaArchivoDescomprimido);
                }
                catch (Exception ex) {
                    // Aqui se manejaría la excepción
                    _logger.LogError("{mensaje}",ex.Message);
                }
                finally 
                { 
                    outStream.Close(); 
                }                
            }
            return list.ToList();
        }
    }
}
