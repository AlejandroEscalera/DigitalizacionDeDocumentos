using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.File2Pdf;
using iText.Forms;
using iText.Html2pdf.Resolver.Font;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Media;
using MsgReader.Mime.Header;
using MsgReader.Outlook;
using System.Text;
using System.Text.RegularExpressions;
using static MsgReader.Outlook.Storage;
using iText.Layout;
using Microsoft.Extensions.Logging;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Msg2Pdf;

public class Msg2PdfService : IMsg2Pdf
{
    private readonly ILogger<Msg2PdfService> _logger;

    public Msg2PdfService(ILogger<Msg2PdfService> logger)
    {
        // Registrar el proveedor de codificación CodePages
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _logger = logger;
    }

    public IList<string> ConvierteArchivo(string archivo, string directorioDestino)
    {
        IList<string> list = new List<string>();
        //FileInfo fi = new(archivo);
        string fileNameSpecial = Path.GetFileName(directorioDestino + ".texto").Replace(".texto","");

        //string documentoDestino = System.IO.Path.Combine(directorioDestino, (fi.Name ?? "").Replace(fi.Extension, "_" + fi.Extension.Replace(".", "")) + ".pdf");
        string documentoDestino = System.IO.Path.Combine(directorioDestino, fileNameSpecial + ".pdf");
        if (!Directory.Exists(System.IO.Path.GetDirectoryName(documentoDestino) ?? ""))
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(documentoDestino) ?? "");
        }
        try
        {
            ConvertMsgToPdfUsingMsgReaderAndPdfHtml(archivo, documentoDestino);
            list.Add(documentoDestino);
        }
        catch (OpenMcdf.CFCorruptedFileException ex)
        {
            _logger.LogError("Es error real del archivo {mensaje}", ex.Message);
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("Marcó error {mensaje}", ex.Message);
            throw new Exception(ex.Message);
        }
        return list;
    }


    public static void ConvertMsgToPdfUsingMsgReaderAndPdfHtml(string msgFilePath, string pdfOutputPath)
    {

        // Cargar el archivo .msg usando MsgReader.Storage.Message
        using var msgFile = new Storage.Message(msgFilePath);

        // Obtener el cuerpo del correo electrónico en formato HTML
        string bodyHtml = msgFile.BodyHtml;

        // Crear un directorio temporal para guardar las imágenes incrustadas
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        // Nombre el archivo encabezado
        string archivoEncabezado = Path.Combine(tempDirectory, "encabezado.pdf");
        // Nombre el archivo detalle
        string archivoDetalle = Path.Combine(tempDirectory, "detalle.pdf");





        Dictionary<string, Attachment> inlineImageMap = new();
        // Extraer y guardar las imágenes incrustadas en el directorio temporal
        foreach (var objeto in msgFile.Attachments) // .Cast<Attachment>()
        {
            if (objeto.GetType() == typeof(Attachment)) {
                Attachment attachment = (Attachment)objeto;
                if (attachment.IsInline && (attachment.MimeType is not null) && attachment.MimeType.Contains("Image", StringComparison.InvariantCultureIgnoreCase)) // && attachment.IsImage
                {
                    if (!string.IsNullOrEmpty(attachment.ContentId))
                    {
                        inlineImageMap[attachment.ContentId] = attachment;
                    }
                    string imagePath = Path.Combine(tempDirectory, attachment.FileName);
                    File.WriteAllBytes(imagePath, attachment.Data);
                }
            }
        }

        // Crear un objeto ConverterProperties para manejar imágenes y otros elementos
        ConverterProperties converterProperties = new();

        // Configurar el manejador de imágenes y recursos
        converterProperties.SetMediaDeviceDescription(MediaDeviceDescription.GetDefault());
        converterProperties.SetBaseUri(new Uri(tempDirectory).AbsoluteUri); // Utilizar el directorio temporal como base URI
        converterProperties.SetFontProvider(new DefaultFontProvider(true, true, true));

        using (FileStream pdfStream = new(archivoEncabezado, FileMode.Create))
        {

            using PdfWriter pdfWriter = new(pdfStream);
            PdfDocument pdfDoc = new(pdfWriter);
            Document doc = new(pdfDoc);

            // Cambia la rotación de la página
            pdfDoc.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LEGAL.Rotate());

            doc.Add(new Paragraph("Asunto: " + msgFile.Subject));
            if (msgFile.Headers is not null)
            {
                doc.Add(new Paragraph("De: " + msgFile.Headers.From));
                doc.Add(new Paragraph("Para: " + ObtieneCorreos(msgFile.Headers.To)));
                doc.Add(new Paragraph("Fecha: " + msgFile.Headers.Date));
            }
            else
            {
                doc.Add(new Paragraph("De: " + ObtieneCorreos(msgFile.Sender)));
                doc.Add(new Paragraph("Para: " + ObtieneCorreos(msgFile.Recipients)));
                doc.Add(new Paragraph("Fecha: " + msgFile.SentOn));

            }
            doc.Add(new Paragraph("-----------------------------------------------------------------"));
            doc.Add(new Paragraph("Contenido del mensaje:"));
            //doc.Add(new AreaBreak(AreaBreakType.LAST_PAGE)); // Agrega un salto de página
            doc.Add(new Paragraph(""));
            if (string.IsNullOrEmpty(msgFile.BodyHtml))
            {
                doc.Add(new Paragraph(msgFile.BodyText));
            }
            doc.Close();
        }

        if (bodyHtml is not null)
        {
            foreach (Match match in Regex.Matches(bodyHtml, "src=\"cid:(.*?)\"").Cast<Match>())
            {
                string cid = match.Groups[1].Value;
                if (inlineImageMap.TryGetValue(cid, out Attachment? inlineImage))
                {
                    if (inlineImage is not null)
                    {
                        string imagePath = Path.Combine(tempDirectory, inlineImage.FileName);
                        File.WriteAllBytes(imagePath, inlineImage.Data);
                        bodyHtml = bodyHtml.Replace($"cid:{cid}", imagePath).Replace(Convert.ToChar(65533) + "", "");
                    }
                }
            }
            // Convertir el contenido HTML a PDF usando iText7 y PdfHtml
            using (MemoryStream htmlStream = new(System.Text.Encoding.UTF8.GetBytes(bodyHtml)))
            {
                using FileStream pdfStream = new(archivoDetalle, FileMode.Create);

                using PdfWriter pdfWriter = new(pdfStream);
                PdfDocument pdfDoc = new(pdfWriter);
                // Cambia la rotación de la página
                pdfDoc.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LEGAL.Rotate());
                // Verificar si el mensaje contiene contenido HTML
                if (!string.IsNullOrEmpty(msgFile.BodyHtml))
                {
                    HtmlConverter.ConvertToPdf(htmlStream, pdfDoc, converterProperties);

                }
            }
            if (!File.Exists(pdfOutputPath))
            {
                UnirPDFs(archivoEncabezado, archivoDetalle, pdfOutputPath);
            }
        }
        else
        {
            if (!File.Exists(pdfOutputPath))
            {
                File.Move(archivoEncabezado, pdfOutputPath);
            }
        }
        // Eliminar el directorio temporal y sus contenidos
        Directory.Delete(tempDirectory, true);
    }

    static void UnirPDFs(string pdf1Path, string pdf2Path, string outputPath)
    {
        PdfDocument pdf1 = new(new PdfReader(pdf1Path));
        PdfDocument pdf2 = new(new PdfReader(pdf2Path));

        PdfWriter writer = new(outputPath);
        PdfDocument mergedPdf = new(writer);

        pdf1.CopyPagesTo(1, pdf1.GetNumberOfPages(), mergedPdf, new PdfPageFormCopier());
        pdf2.CopyPagesTo(1, pdf2.GetNumberOfPages(), mergedPdf, new PdfPageFormCopier());

        pdf1.Close();
        pdf2.Close();
        mergedPdf.Close();
    }

    static string ObtieneCorreos(IEnumerable<RfcMailAddress> listaCorreos)
    {
        StringBuilder stringBuilder = new();
        foreach (var correo in listaCorreos)
        {
            stringBuilder.Append('"');
            stringBuilder.Append(correo.DisplayName);
            stringBuilder.Append("\" <");
            stringBuilder.Append(correo.Address);
            stringBuilder.Append(">;");
        }
        return stringBuilder.ToString();
    }

    static string ObtieneCorreos(IEnumerable<Recipient> listaCorreos)
    {
        StringBuilder stringBuilder = new();
        foreach (var correo in listaCorreos)
        {
            stringBuilder.Append('"');
            stringBuilder.Append(correo.DisplayName);
            stringBuilder.Append("\" <");
            stringBuilder.Append(correo.Email);
            stringBuilder.Append(">;");
        }
        return stringBuilder.ToString();
    }

    static string ObtieneCorreos(Sender correo)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append('"');
        stringBuilder.Append(correo.DisplayName);
        stringBuilder.Append("\" <");
        stringBuilder.Append(correo.Email);
        stringBuilder.Append(">;");
        return stringBuilder.ToString();
    }

}
