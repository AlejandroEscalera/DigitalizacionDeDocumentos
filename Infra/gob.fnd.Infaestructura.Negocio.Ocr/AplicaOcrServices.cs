using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace gob.fnd.Infaestructura.Negocio.Ocr
{
    public class AplicaOcrServices : IAplicaOcr
    {
        private readonly ILogger<AplicaOcrServices> _logger;
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private readonly IronTesseract _Ocr;

        public AplicaOcrServices(ILogger<AplicaOcrServices> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
#pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
            _Ocr = new() { Language = OcrLanguage.Spanish  };
#pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public bool ObtieneOcrDesdeArchivo(string nombreArchivo, int numeroDePaginas, int paginaInicial = 1)
        {
            try
            {
                
                FileInfo fi = new(nombreArchivo);
                string textFile = fi.FullName.Replace(fi.Extension, ".txt");
                // Si se está lanzando por segunda vez, desde el inicio, se elimina lo anterior
                if (File.Exists(textFile) && (paginaInicial == 1))
                {
                    // File.Delete(textFile);
                    paginaInicial = ObtieneUltimaPaginaProcesada(textFile) + 1;
                    if (paginaInicial > numeroDePaginas)
                    {
                        return true;
                    }
                }

                #region Cargo el PDF en memoria para hacerlo más rápido
                byte[] pdfToMemory;

                using (var archivoDeMemoria = new MemoryStream())
                {
                    using (var lecturaDeArchivo = new FileStream(nombreArchivo, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {

                        lecturaDeArchivo.CopyTo(archivoDeMemoria);
                    }
                    pdfToMemory = archivoDeMemoria.ToArray();
                }
                #endregion

                for (int i = (paginaInicial - 1); i < numeroDePaginas; i++)
                {
                    IEnumerable<int> numbers = new List<int> { i }; 

                    StringBuilder resultadoOcr = new();
                    // Escribo el encabezado de la página
                    resultadoOcr.AppendLine(string.Format("============= Inicio : Pagina {0}/{1} =================", i + 1, numeroDePaginas));
                    _logger.LogInformation("Voy en la página {pagNumber} de {pagTotal} del archivo {nombreArchivo}", i + 1, numeroDePaginas, nombreArchivo);
                    #region realizo el OCR del arreglo de bytes de memoria
#pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                    using OcrInput documentoAnalizar = new ();
                    documentoAnalizar.TargetDPI = 300;
                    documentoAnalizar.AddPdfPages(pdfToMemory, numbers); // nombreArchivo
                    var resultadoDelReconocimiento = _Ocr.Read(documentoAnalizar);
                    resultadoOcr.AppendLine(resultadoDelReconocimiento.Text);
#pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
                    #endregion

                    // Escribo el fin de la página
                    resultadoOcr.AppendLine(string.Format("============= Fin : Pagina {0}/{1} =================", i + 1, numeroDePaginas));
                    // Escribo un separador para el análisis
                    resultadoOcr.AppendLine("============= Separador =================");

                    // Crear un objeto StreamWriter para escribir el contenido en el archivo de texto
                    using StreamWriter sw = new(textFile, true);
                    // Escribir el contenido del StringBuilder en el archivo de texto
                    sw.Write(resultadoOcr.ToString());

                }
                _logger.LogInformation("Terminó el archivo {nombreArchivo}", textFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("No se puede continuar con el OCR : {error}", ex.Message);
                return false;
            }
            return true;
        }

        private static int ObtieneUltimaPaginaProcesada(string fileName)
        {
            FileInfo fi = new(fileName);
            if (!fi.Exists)
                return 0;
            string contenidoArchivo = File.ReadAllText(fi.FullName);

            string separador = "============= Separador =================";
            string[] arregloCadenas = contenidoArchivo.Split(new string[] { separador }, StringSplitOptions.None);
            int numPagina = 0;
            if (arregloCadenas.Length >= 2)
            {
                string ultimaPagina = arregloCadenas[^2] ?? "";
                numPagina = ObtieneNumeroPágina(ultimaPagina);
            }
            return numPagina;
        }

        private static int ObtieneNumeroPágina(string paginaCompleta)
        {
            int numeroPagina = 0;
            string subcadena = "============= Fin : Pagina ";
            int indice = paginaCompleta.IndexOf(subcadena);
            string palabra;
            if (indice != -1)
            {
                indice += subcadena.Length;
                int finPalabra = paginaCompleta.IndexOf(" ", indice);
                if (finPalabra == -1)
                {
                    finPalabra = paginaCompleta.Length;
                }
                palabra = paginaCompleta[indice..finPalabra];
                if (palabra.Contains('/'))
                {
                    string[] numeroPaginas = palabra.Split('/');
                    if (numeroPaginas.Length == 2)
                    {
                        numeroPagina = Convert.ToInt32(numeroPaginas[0]);
                    }
                }
            }
            return numeroPagina;
        }
    }
}
