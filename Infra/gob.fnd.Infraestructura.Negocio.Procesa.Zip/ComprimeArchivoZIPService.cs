using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Zip;

public class ComprimeArchivoZIPService : IComprimeArchivoZIP
{
    private readonly ILogger<ComprimeArchivoZIPService> _logger;

    public ComprimeArchivoZIPService(ILogger<ComprimeArchivoZIPService> logger)
    {
        _logger = logger;
    }
    public bool ComprimirArchivo(string archivoAComprimir, string archivoZipFinal, string contrasenia)
    {
        try
        {
            using (FileStream archivoFuente = new (archivoAComprimir, FileMode.Open))
            {
                using FileStream archivoDestino = File.Create(archivoZipFinal);
                using ZipArchive archivoZip = new(archivoDestino, ZipArchiveMode.Create);
                ZipArchiveEntry entrada = archivoZip.CreateEntry(Path.GetFileName(archivoAComprimir), CompressionLevel.SmallestSize);

                using Stream stream = entrada.Open();
                archivoFuente.CopyTo(stream);
            }

            _logger.LogInformation("Archivo comprimido con éxito.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrió un error al comprimir el archivo: {mensajeError}" , ex.Message);
        }
        return false;
    }
}
