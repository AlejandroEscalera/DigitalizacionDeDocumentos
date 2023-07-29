using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Image2Pdf
{
    public class Image2PdfService : IImagen2Pdf
    {
        public IList<string> ConvierteArchivo(string archivo, string directorioDestino)
        {
            IList<string> list = new List<string>();
            //FileInfo fileInfo = new(archivo);

            if (!Directory.Exists(directorioDestino))
            {
                Directory.CreateDirectory(directorioDestino);
            }

            string fileNameSpecial = System.IO.Path.GetFileName(directorioDestino + ".texto").Replace(".texto", "");

            // string archivoSalida = System.IO.Path.Combine(directorioDestino, (fileInfo.Name ?? "").Replace(fileInfo.Extension, "_" + fileInfo.Extension.Replace(".", "")) + " R.pdf");
            string archivoSalida = System.IO.Path.Combine(directorioDestino, fileNameSpecial + ".pdf");

            // Crear un objeto ImageData a partir de la imagen
            ImageData imagen = ImageDataFactory.Create(archivo);

            // Crear un objeto PageSize con el tamaño de la imagen
            PageSize pageSize = new(imagen.GetWidth(), imagen.GetHeight());

            // Si la altura de la imagen es mayor que la anchura, establecer la orientación del tamaño de página en vertical (retrato)
            if (imagen.GetWidth() > imagen.GetHeight())
            {
                pageSize = pageSize.Rotate();
            }

            // Crear un objeto PdfWriter para escribir el PDF
            using PdfWriter writer = new(archivoSalida);
            // Crear un objeto PdfDocument con el tamaño de página y el escritor
            using PdfDocument pdf = new(writer);
            pdf.SetDefaultPageSize(pageSize);

            // Crear un objeto Document con el PdfDocument
            using iText.Layout.Document document = new(pdf);
            // Insertar la imagen en el Document
            iText.Layout.Element.Image imagenPdf = new(imagen);
            document.Add(imagenPdf);

            // Regreso el documento destino
            list.Add(archivoSalida);
            return list.ToList();
        }
    }
}
