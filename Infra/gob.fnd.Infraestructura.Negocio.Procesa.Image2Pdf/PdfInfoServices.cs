using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Image2Pdf
{
    public class PdfInfoServices : IPdfInfo
    {
        public int DameElNumeroDePaginas(string fileName)
        {
            using var pdfDocument = new PdfDocument(new PdfReader(fileName));
            return pdfDocument.GetNumberOfPages();
        }
    }
}
