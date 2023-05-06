using GemBox.Document;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Doc2Pdf
{
    public class Doc2PdfService : IDoc2Pdf
    {
        public IList<string> ConvierteArchivo(string archivo, string directorioDestino)
        {
            IList<string> list = new List<string>();
            FileInfo fi = new(archivo);
            // directorio destino + nombre del archivo con su extensión, solo que en lugar de "." se reemplaza por "_" + el nombre del archivo
            // Revisar si la conversión del directorio es necesario ponerlo aqui o solo el nombre del archivo
            //string documentoDestino = directorioDestino + System.IO.Path.DirectorySeparatorChar + (fi.Name ?? "").Replace(fi.Extension, "_"+ fi.Extension.Replace(".","")) +
            //    System.IO.Path.DirectorySeparatorChar + (fi.Name ?? "").Replace(fi.Extension, "") + ".pdf";

            string documentoDestino = System.IO.Path.Combine(directorioDestino, (fi.Name ?? "").Replace(fi.Extension, "_" + fi.Extension.Replace(".", "")) + ".pdf");
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(documentoDestino) ?? ""))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(documentoDestino) ?? "");
            }


            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            // Cargar el documento DOCX
            DocumentModel document = DocumentModel.Load(archivo);

            // Crear un objeto PdfSaveOptions para guardar el documento como PDF
            PdfSaveOptions options = new();

            // Guardar el documento como PDF
            document.Save(documentoDestino, options);
            // Regreso el documento destino
            list.Add(documentoDestino);
            return list.ToList();
        }
    }
}
