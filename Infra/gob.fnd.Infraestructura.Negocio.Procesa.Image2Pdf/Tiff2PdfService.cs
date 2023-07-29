using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using iText.Kernel.Pdf;
using iText.IO.Image;
using System.Reflection.Metadata;
using iText.IO.Codec;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using System.ComponentModel;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Image2Pdf
{
    public class Tiff2PdfService : ITiff2Pdf
    {
        public IList<string> ConvierteArchivo(string archivo, string directorioDestino)
        {
            IList<string> list = new List<string>();
            //FileInfo fileInfo = new(archivo);
            string fileNameSpecial = System.IO.Path.GetFileName(directorioDestino + ".texto").Replace(".texto", "");

            // string pdfDestino = System.IO.Path.Combine(directorioDestino, (fileInfo.Name ?? "").Replace(fileInfo.Extension, "_" + fileInfo.Extension.Replace(".", "")) + ".pdf");
            string pdfDestino = System.IO.Path.Combine(directorioDestino, fileNameSpecial + ".pdf");

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(pdfDestino) ?? ""))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(pdfDestino) ?? "");
            }

            // Cargar imagen TIFF
            ImageData tiffData = ImageDataFactory.Create(archivo);

            // Crear documento PDF
            PdfDocument pdfDocument = new(new PdfWriter(pdfDestino));

            // Crear página PDF
            PdfPage pdfPage;

            RandomAccessFileOrArray raf = new(new RandomAccessSourceFactory().CreateBestSource(archivo));

            // Obtener el número de páginas de la imagen TIFF
            int numPages = TiffImageData.GetNumberOfPages(raf);
            //int numPages = tiffData.GetNumberOfPages();


            // Iterar sobre todas las páginas de la imagen TIFF
            for (int i = 1; i <= numPages; i++)
            {

                // Cargar la página de la imagen TIFF
                ImageData pageData = ImageDataFactory.CreateTiff(tiffData.GetUrl(), true, i, true);
                iText.Kernel.Geom.Rectangle tiffPageSize = new(pageData.GetWidth(), pageData.GetHeight());

                // Agrega una nueva página
                pdfPage = pdfDocument.AddNewPage(new PageSize(tiffPageSize));
                // Agrego un área para dibujar la imagen en la página
                PdfCanvas canvas = new(pdfPage);
                // Agregar la imagen a la página PDF
                canvas.AddImageAt(pageData, tiffPageSize.GetX(), tiffPageSize.GetY(), false);
            }

            // Cerrar documento PDF
            pdfDocument.Close();

            list.Add(pdfDestino);
            return list.ToList();
        }
    }
}
