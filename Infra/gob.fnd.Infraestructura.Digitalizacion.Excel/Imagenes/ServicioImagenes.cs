using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.ExcelHelper;
using gob.fnd.Infraestructura.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Imagenes
{
    public class ServicioImagenes : IServicioImagenes
    {
        static readonly string[] C_STRA_CRITERIO_EXTENSIONES = new string[] { ".pdf", ".zip", ".jpg", ".png", ".7z", ".msg", ".xlsx", ".docx", ".xls", ".txt", ".tif", ".doc", ".jpeg", ".pptx", ".gif", ".jfif" };

        private readonly ILogger<ServicioImagenes> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServicioDirToXls _servicioDirToXls;
        private readonly string _carpetaRaizImagenesDigitales;
        private readonly bool _registraTodasLasImagenes;
        private readonly string _archivoResultadoImagenes;

        // const int C_INT_IDSEARCH = 1;
        #region Imgs
        const string C_STR_NOMBRE_ARCHIVO = "Name";
        const string C_STR_FECHA_DE_MODIFICACION = "Modified";
        const string C_STR_USUARIO_DE_MODIFICACION = "Modified By";
        const string C_STR_TAMANIO_DE_ARCHIVO = "File Size";
        const string C_STR_RUTA_DE_GUARDADO = "Path";
        const string C_STR_ITEM_TYPE = "Item Type";
        const string C_STR_URL_ARCHIVO = "Name"; // Solo que se obtiene el hyperlink
        const string C_STR_NULLO = "000000000000000000";

        private int iNombreArchivo = 1;  // NAME
        private int iFechaDeModificacion = 2;
        private int iUsuarioDeModificacion = 3;
        private int iTamanioDeArchivo = 4;
        private int iRutaDeGuardado = 5;
        private int iUrlArchivo = 1;
        private int iItemType = 5;
        #endregion

        #region exp
        const string C_STR_ID_SEARCH = "ID SEARCH";
        const string C_STR_NOMBRE = "Nombre";
        const string C_STR_MINISTRACION = "Ministración";
        const string C_STR_VIGENCIA_DEL_CONTRATO = "Vigencia del Contrato";
        const string C_STR_FECHA_DE_VIGENCIA_DE_LA_DISP_O_CONTRATO = "Fecha de Vigencia de la Disp. o Contrato";
        const string C_STR_CHECKLIST = "Check List";
        const string C_STR_VALIDADO_POR_CALIDAD = "Validado por Calidad";
        const string C_STR_MODIFICADO = "Modificado";
        const string C_STR_RUTA_DE_ACCESO = "Ruta de acceso";
        const string C_STR_MODIFICADO_POR = "Modificado por";
        const string C_STR_TIPO_DE_ELEMENTO = "Tipo de elemento";

        private int iIdSeach = 1;
        private int iMinistracion = 3;
        private int iVigenciaDelContrato = 4;
        private int iFechaDeVigenciaDeLaDispOContrato = 5;
        private int iCheckList = 6;
        private int iValidadoPorCalidad = 8;
        #endregion

        /// <summary>
        /// Obtengo las columnas de los campos a obtener, por si cambiaran de ubicación en el archivo original
        /// </summary>
        /// <param name="hoja">Hoja de excel donde se va a buscar</param>
        /// <param name="columnaIncial">columna donde comenzare a buscar</param>
        /// <param name="nombre">Nombre de la columna a buscar</param>
        /// <returns>Regresa el número de columna, si es -1 quiere decir que no existe</returns>
        private static int BuscaCelda(ExcelWorksheet hoja, int columnaIncial, string nombre)
        {
            string nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[1, columnaIncial]).Trim();
            while (!(string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrWhiteSpace(nuevoNombre)))
            {
                if (string.Equals(nombre, nuevoNombre, StringComparison.InvariantCultureIgnoreCase))
                {
                    return columnaIncial;
                }
                columnaIncial++;
                nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[1, columnaIncial]).Trim();
            }
            return -1;
        }

        private void ObtieneValoresCampos(ExcelWorksheet hoja, bool expedientes = false)
        {
            #region Imgs
            if (!expedientes)
            {
                iNombreArchivo = BuscaCelda(hoja, 1, C_STR_NOMBRE_ARCHIVO);
                iFechaDeModificacion = BuscaCelda(hoja, 1, C_STR_FECHA_DE_MODIFICACION);
                iUsuarioDeModificacion = BuscaCelda(hoja, 1, C_STR_USUARIO_DE_MODIFICACION);
                iTamanioDeArchivo = BuscaCelda(hoja, 1, C_STR_TAMANIO_DE_ARCHIVO);
                iRutaDeGuardado = BuscaCelda(hoja, 1, C_STR_RUTA_DE_GUARDADO);
                iUrlArchivo = BuscaCelda(hoja, 1, C_STR_URL_ARCHIVO);
                iItemType = BuscaCelda(hoja, 1, C_STR_ITEM_TYPE);
                #region En caso de que venga en español
                if ((iNombreArchivo == -1)
                    && (iFechaDeModificacion == -1)
                    && (iUsuarioDeModificacion == -1)
                    && (iTamanioDeArchivo == -1)
                    && (iRutaDeGuardado == -1)
                    && (iUrlArchivo == -1)
                    && (iItemType == -1))
                {
                    iNombreArchivo = BuscaCelda(hoja, 1, C_STR_NOMBRE);
                    iFechaDeModificacion = BuscaCelda(hoja, 1, C_STR_MODIFICADO);
                    iUsuarioDeModificacion = BuscaCelda(hoja, 1, C_STR_MODIFICADO_POR);
                    iTamanioDeArchivo = BuscaCelda(hoja, 1, "TAMAÑO DEL ARCHIVO");
                    iRutaDeGuardado = BuscaCelda(hoja, 1, C_STR_RUTA_DE_ACCESO);
                    iUrlArchivo = BuscaCelda(hoja, 1, C_STR_NOMBRE);
                    iItemType = BuscaCelda(hoja, 1, C_STR_TIPO_DE_ELEMENTO);
                    if (iUsuarioDeModificacion == -1)
                        iUsuarioDeModificacion = BuscaCelda(hoja, 1, "Creado por");
                }
                #endregion
            }
            #endregion
            #region exps
            else
            {
                iIdSeach = BuscaCelda(hoja, 1, C_STR_ID_SEARCH);
                iNombreArchivo = BuscaCelda(hoja, 1, C_STR_NOMBRE);
                iMinistracion = BuscaCelda(hoja, 1, C_STR_MINISTRACION);
                iVigenciaDelContrato = BuscaCelda(hoja, 1, C_STR_VIGENCIA_DEL_CONTRATO);
                iFechaDeVigenciaDeLaDispOContrato = BuscaCelda(hoja, 1, C_STR_FECHA_DE_VIGENCIA_DE_LA_DISP_O_CONTRATO);
                iCheckList = BuscaCelda(hoja, 1, C_STR_CHECKLIST);
                iFechaDeModificacion = BuscaCelda(hoja, 1, C_STR_MODIFICADO);
                iValidadoPorCalidad = BuscaCelda(hoja, 1, C_STR_VALIDADO_POR_CALIDAD);

                iUsuarioDeModificacion = BuscaCelda(hoja, 1, C_STR_MODIFICADO_POR);
                iTamanioDeArchivo = -1;
                iRutaDeGuardado = BuscaCelda(hoja, 1, C_STR_RUTA_DE_ACCESO);
                iUrlArchivo = BuscaCelda(hoja, 1, C_STR_NOMBRE);
                iItemType = BuscaCelda(hoja, 1, C_STR_TIPO_DE_ELEMENTO);
            }
            #endregion
        }
        public ServicioImagenes(ILogger<ServicioImagenes> logger, IConfiguration configuration, IServicioDirToXls servicioDirToXls)
        {
            _logger = logger;
            _configuration = configuration;
            _servicioDirToXls = servicioDirToXls;
            _carpetaRaizImagenesDigitales = _configuration.GetValue<string>("archivosExpedientesDigitales") ?? "";
            _registraTodasLasImagenes = (_configuration.GetValue<string>("registraTodasLasImagenes") ?? "").Equals("Si");
            _archivoResultadoImagenes = _configuration.GetValue<string>("archivoResultadoImagenes") ?? "";
        }
        public IEnumerable<ArchivosImagenes> GetArchivosImagenes(string archivoExcelOrigen, bool esExpediente = false)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            IList<ArchivosImagenes> resultadoImagenes = new List<ArchivosImagenes>();

            if (!File.Exists(archivoExcelOrigen))
                return resultadoImagenes;

            if (archivoExcelOrigen.Contains("D:\\202302 Digitalizacion\\5. Imagenes sin procesar\\Imgs\\Respaldo Liberados.xlsx", StringComparison.CurrentCultureIgnoreCase))
                _logger.LogInformation("Comienza debug de respaldos liberados");
            #region Abro el archivo de excel y saco la informacion de los campos
            FileStream fs = new(archivoExcelOrigen, FileMode.Open, FileAccess.Read);
            _logger.LogInformation("=== Procesando archivo :{nombreArchivo} ", archivoExcelOrigen);
            using var package = new ExcelPackage();
            ExcelWorksheet? hoja = null;
            package.Load(fs);
            hoja = package.Workbook.Worksheets.FirstOrDefault();
            if (hoja != null)
            {
                ObtieneValoresCampos(hoja, esExpediente);
                int renglon = 2;
                string valorCelda = "Comienza";
                while (!string.IsNullOrEmpty(valorCelda)) {
                    ArchivosImagenes imagen = new();
                    #region imgs
                    if (!esExpediente)
                    {
                        // Solo agrego imagenes, no agrego carpetas!!!!
                        if ((hoja.Cells[renglon, iItemType].GetCellString().Contains("Element") && EsArchivoValido(hoja.Cells[renglon, iNombreArchivo].GetCellString())) || _registraTodasLasImagenes)
                        {
                            imagen.Id = renglon - 1;
                            #region Nombre del archivo
                            string valornombreDelArchivo = hoja.Cells[renglon, iNombreArchivo].GetCellString();
                            // Por si el tipo es double, en vez de string
                            if (string.IsNullOrEmpty(valornombreDelArchivo))
                            {
                                valornombreDelArchivo = hoja.Cells[renglon, iNombreArchivo].Text;
                            }
                            imagen.NombreArchivo = valornombreDelArchivo;
                            FileInfo fi = new(imagen.NombreArchivo);
                            imagen.Extension = fi.Extension;
                            #endregion
                            imagen.FechaDeModificacion = hoja.Cells[renglon, iFechaDeModificacion].GetCellDateTime();
                            imagen.UsuarioDeModificacion = hoja.Cells[renglon, iUsuarioDeModificacion].GetCellString();
                            if (iTamanioDeArchivo != -1)
                            {
                                try
                                {
                                    imagen.TamanioArchivo = Convert.ToInt64(hoja.Cells[renglon, iTamanioDeArchivo].Text);
                                }
                                catch { imagen.TamanioArchivo = 0;  }
                            }
                            imagen.RutaDeGuardado = hoja.Cells[renglon, iRutaDeGuardado].GetCellString();
                            #region Url
                            if (hoja.Cells[renglon, iUrlArchivo].Hyperlink != null)
                                imagen.UrlArchivo = hoja.Cells[renglon, iUrlArchivo].Hyperlink.AbsoluteUri;
                            #region Corrijo el problema de respaldos liberados
                            if (string.IsNullOrEmpty(imagen.UrlArchivo) && !string.IsNullOrEmpty(imagen.NombreArchivo))
                            {
                                string[] nombreConUrl = imagen.NombreArchivo.Split('\n');
                                if (nombreConUrl.Length > 0) {
                                    imagen.UrlArchivo = nombreConUrl.FirstOrDefault();
                                    imagen.NombreArchivo = nombreConUrl.Last();
                                }
                            }
                            imagen.CarpetaDestino = ObtieneDirectorioDestino(imagen.RutaDeGuardado);
                            #endregion
                            #endregion
                            if (!string.IsNullOrEmpty(imagen.NombreArchivo) && !string.IsNullOrEmpty(imagen.UrlArchivo))
                            {
                                if (imagen.NombreArchivo.Equals("201700000710000024rev.anual.pdf",StringComparison.InvariantCultureIgnoreCase))
                                {
                                    _logger.LogTrace("Comienza debug");
                                }
                                imagen.NumCredito = Condiciones.ObtieneNumCredito(imagen.NombreArchivo ?? ""); // , imagen.UrlArchivo ?? ""
                                
                                if (imagen.NumCredito.Equals(Condiciones.C_STR_NULLO))
                                {
                                    /// TODO: Agregar el número de crédito desde la ruta del archivo
                                    imagen.NumCredito = Condiciones.ObtieneNumCreditoCarpeta(imagen.UrlArchivo);
                                }
                                imagen.ArchivoOrigen = archivoExcelOrigen;
                            }
                            if ((imagen.NumCredito != C_STR_NULLO) || _registraTodasLasImagenes)
                                resultadoImagenes.Add(imagen);
                            else
                                _logger.LogInformation("Archivo inválido (pdf) :{nombreArchivo}|{limpioDeCaracteres}", imagen.NombreArchivo, Condiciones.LimpiaNoNumeros(imagen.NombreArchivo));
                        }
                        #region Si es carpeta la reporto en logger
                        else
                        {
                            if (!hoja.Cells[renglon, iItemType].GetCellString().Contains("Element"))
                                _logger.LogInformation("Elemento no válido :{nombreElemento} {archivo}", hoja.Cells[renglon, iItemType].GetCellString(), hoja.Cells[renglon, iNombreArchivo].GetCellString());
                        }
                        #endregion
                    }
                    #endregion
                    #region Exp
                    else
                    {
                        // Solo agrego imagenes, no agrego carpetas!!!!
                        if ((hoja.Cells[renglon, iItemType].GetCellString().Contains("Element") && EsArchivoValido(hoja.Cells[renglon, iNombreArchivo].GetCellString())) || _registraTodasLasImagenes)
                        {
                            imagen.Id = renglon - 1;
                            #region valorIdSearch
                            string valorIdSearch = hoja.Cells[renglon, iIdSeach].GetCellString();
                            // Por si el tipo es double, en vez de string
                            if (string.IsNullOrEmpty(valorIdSearch))
                            {
                                valorIdSearch = hoja.Cells[renglon, iIdSeach].Text;
                            }
                            imagen.IdSearch = valorIdSearch;
                            #endregion
                            #region Nombre del archivo
                            string valornombreDelArchivo = hoja.Cells[renglon, iNombreArchivo].GetCellString();

                            // Por si el tipo es double, en vez de string
                            if (string.IsNullOrEmpty(valornombreDelArchivo))
                            {
                                valornombreDelArchivo = hoja.Cells[renglon, iNombreArchivo].Text;
                            }
                            imagen.NombreArchivo = valornombreDelArchivo;
                            FileInfo fi = new(imagen.NombreArchivo);
                            imagen.Extension = fi.Extension;
                            #endregion
                            imagen.FechaDeModificacion = hoja.Cells[renglon, iFechaDeModificacion].GetCellDateTime();
                            imagen.UsuarioDeModificacion = hoja.Cells[renglon, iUsuarioDeModificacion].GetCellString();
                            //imagen.TamanioArchivo = Convert.ToInt64(hoja.Cells[renglon, iTamanioDeArchivo].Text);
                            imagen.RutaDeGuardado = hoja.Cells[renglon, iRutaDeGuardado].GetCellString();
                            if (hoja.Cells[renglon, iUrlArchivo].Hyperlink != null)
                                imagen.UrlArchivo = hoja.Cells[renglon, iUrlArchivo].Hyperlink.AbsoluteUri;
                            imagen.Ministracion = hoja.Cells[renglon, iMinistracion].GetCellInt();
                            imagen.VigenciaDelContato = hoja.Cells[renglon, iVigenciaDelContrato].GetCellString(); // Si es vigente o vencido
                            imagen.FechaVigenciaDelaDispOContrato = hoja.Cells[renglon, iFechaDeVigenciaDeLaDispOContrato].GetCellDateTime();
                            imagen.CheckList = hoja.Cells[renglon, iCheckList].GetCellString(); // Si es primero o segundo piso
                            imagen.ValidadoPorCalidad = hoja.Cells[renglon, iValidadoPorCalidad].GetCellString().Contains("VERDADERO", StringComparison.InvariantCultureIgnoreCase);
                            if (!string.IsNullOrEmpty(imagen.NombreArchivo) && !string.IsNullOrEmpty(imagen.UrlArchivo))
                            {
                                imagen.NumCredito = Condiciones.ObtieneNumCredito(imagen.NombreArchivo ?? ""); // , imagen.UrlArchivo ?? ""
                                if (imagen.NumCredito.Equals(Condiciones.C_STR_NULLO))
                                {
                                    /// TODO: Agregar el número de crédito desde la ruta del archivo
                                    imagen.NumCredito = Condiciones.ObtieneNumCreditoCarpeta(imagen.UrlArchivo);
                                }
                                imagen.ArchivoOrigen = archivoExcelOrigen;
                            }
                            imagen.CarpetaDestino = ObtieneDirectorioDestino(imagen.RutaDeGuardado);
                            if ((imagen.NumCredito != C_STR_NULLO) || _registraTodasLasImagenes)
                                resultadoImagenes.Add(imagen);
                            else
                                _logger.LogInformation("Archivo inválido (pdf) :{nombreArchivo}|{limpioDeCaracteres}", imagen.NombreArchivo, Condiciones.LimpiaNoNumeros(imagen.NombreArchivo));
                        }
                    }
                    #endregion
                    renglon++;
                    valorCelda = hoja.Cells[renglon, iFechaDeModificacion].Text;
                }
            }
            // 
            _logger.LogInformation("=== Terminó de procesar archivo :{nombreArchivo} ", archivoExcelOrigen);
            #endregion         

            /// Elimino los duplicados
            IList<ArchivosImagenes> imagenesUnicas = resultadoImagenes
                .GroupBy(p => p.UrlArchivo)
                .Select(g => g.First())
                .ToList();

            return imagenesUnicas;
        }
    
        public IEnumerable<ArchivosImagenes> ObtengoFiltrosABSaldosCorporativo(IEnumerable<ABSaldosConCastigo> saldosCorporativo, IEnumerable<ArchivosImagenes> imagenesARevisar)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ArchivosImagenes> ObtieneInformacionDeTodosLosArchivosDeImagen(IEnumerable<ArchivosImagenes>? anteriores = null, bool expedientes = false)
        {
            IList< ArchivosImagenes > resultado = new List< ArchivosImagenes >();
            if (anteriores != null)
                ((List<ArchivosImagenes>)resultado).AddRange(anteriores);
            string rutaABuscar = ObtieneRutaDeBusqueda(expedientes);
            var archivos = ObtieneLosArchivosDeImagenes(expedientes);
            foreach (var archivo in archivos) {                
                string rutaCompleta = Path.Combine(rutaABuscar, archivo);
                IEnumerable<ArchivosImagenes> nuevosArchivos = GetArchivosImagenes(rutaCompleta, expedientes);
                if (nuevosArchivos is not null && nuevosArchivos.Any())
                {
                    _logger.LogInformation("Cargando el archivo de imagenes {archivoImage}", archivo);
                    ((List<ArchivosImagenes>)resultado).AddRange(nuevosArchivos);
                }
            }

            if (expedientes)
                ((List<ArchivosImagenes>)resultado).AddRange(_servicioDirToXls.ObtieneImagenesDesdeDirectorio());

            #region Limpieza
            #region Selecciono aquellas cuyo número de expediente sea de 18 dígitos
            IList<ArchivosImagenes> imagenesConsistentes = 
                resultado.
                Where(x => !string.IsNullOrEmpty(x.NumCredito) 
                && ((x.NumCredito??"").Length > 17)
                ).
                Select(y=>y).ToList();
            #endregion
            #region Elimino los duplicados
            IList<ArchivosImagenes> imagenesUnicas = imagenesConsistentes
                .GroupBy(p => p.UrlArchivo)
                .Select(g => g.First())
                .ToList();
            #endregion
            #endregion

            #region Aqui quito todas las que no tengan las extensiones válidas
            var imagenesSinExtentesiones = (from img in imagenesUnicas
                                            orderby img.NumCredito
                                           where C_STRA_CRITERIO_EXTENSIONES.Any(x=>(img.Extension??"").Contains(x,StringComparison.InvariantCultureIgnoreCase) )
                                           select img).ToList();
            #endregion

            return imagenesSinExtentesiones.ToList(); // .Where(a => (a.NumContrato ?? "").Length > 17)
        }


        public IEnumerable<string> ObtieneLosArchivosDeImagenes(bool expedientes = false)
        {
            string rutaABuscar = ObtieneRutaDeBusqueda(expedientes);
            var archivos = Directory.GetFiles(rutaABuscar, "*.xls*"); // busco los archivos y le elimino de ser el caso, los que no sean archivos de excel
            return archivos;
        }

        private string ObtieneRutaDeBusqueda(bool expedientes)
        {
            string rutaABuscar;
            if (!expedientes)
                rutaABuscar = Path.Combine(_carpetaRaizImagenesDigitales, "Imgs");
            else
                rutaABuscar = Path.Combine(_carpetaRaizImagenesDigitales, "Exp2023");
            return rutaABuscar;
        }

        public bool GuardarImagenes(IEnumerable<ArchivosImagenes> imagenes,string archivoSalida = "")
        {
            string archivoResultadoImagenes;
            if (string.IsNullOrEmpty(archivoSalida))
                archivoResultadoImagenes = _archivoResultadoImagenes;
            else
                archivoResultadoImagenes = archivoSalida;
            if (File.Exists(archivoResultadoImagenes))
                File.Delete(archivoResultadoImagenes);

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            FileStream fs = new(archivoResultadoImagenes ?? "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var package = new ExcelPackage(fs);
            ExcelWorksheet? hoja = package.Workbook.Worksheets.Add("Imagenes");
            hoja.Name = "Imagenes";
            #region Encabezado del archivo
            int row = 1;
            int col = 1;
            hoja.Cells[row, col].Value = "#"; // Id
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 11.11 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Id Search"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 14.22 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ARCHIVO_ORIGEN"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 0; // Oculto
            col++;

            hoja.Cells[row, col].Value = "Nombre del archivo"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 26.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NUM_CTE"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(255, 255, 0));
            hoja.Column(col).Width = 12.22 + 0.78; // Oculto
            col++;

            hoja.Cells[row, col].Value = "ACREDITADO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(255, 255, 0));
            hoja.Column(col).Width = 50 + 0.78; // Oculto
            col++;

            hoja.Cells[row, col].Value = "NUM_CONTRATO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(255, 255, 0));
            hoja.Column(col).Width = 15.55; // Oculto
            col++;

            hoja.Cells[row, col].Value = "NUM_CREDITO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "FECHA_MODIFICACION"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "USUARIO_MODIFICACION"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 35.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "TAMAÑO_ARCHIVO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 17.11 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "PATH"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "URL PDF"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 79.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "Ministracion"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "VIGENCIA_CONTRATO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 19.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "FECHA_VIGENCIA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "CHECKLIST"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 13.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "VALIDADO_POR_CALIDAD"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 13.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "NUM_CREDITO_ACTIVO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "FECHA_APERTURA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CASTIGO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 7.67 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ES_PERIODO_DOCTOR"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 3.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ES_CARTERA_ACTIVA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 3.22 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NO_PDF"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 5.22 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CLASIF_MESA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 60.22 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "FECHA_ASIGNACION_ENTRADA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "MONTO_OTORGADO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ANALISTA"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 38.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "REGIONAL"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "SUCURSAL"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 27.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CAT_SUCURSAL"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 27.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EXTENSION"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 4.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "DESTINO"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 112 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ThreadId"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 5.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "Job"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 5.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "PorDescargar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ErrorAlDescargar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "MensajeDeErrorAlDescargar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "PorOcr"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ErrorOcr"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "MensajeDeErrorOCR"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "PorAOcr"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ErrorAOcr"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "MensajeDeErrorAOCR"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "PorAcomodar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ErrorAcomodar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "MensajeDeErrorAcomodar"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "Termino"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "Hash"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 67.89 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "CadenaCredito"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 15.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "CadenaCliente"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 15.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "OcrNumCredito"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "OcrNumCliente"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 12.22 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "IgualNumCredito"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "TieneNumCredito"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "ChecarTona"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 2.44 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "NumPaginas"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 13.11 + 0.78;
            col++;
            hoja.Cells[row, col].Value = "SubArchivo"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 13.11 + 0.78;

            row++;
            #endregion

            #region Detalle
            foreach (var imagen in imagenes)
            {
                col = 1;
                hoja.Cells[row, col].Value = imagen.Id;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                if (!string.IsNullOrEmpty(imagen.IdSearch))
                    hoja.Cells[row, col].Value = imagen.IdSearch.Trim();
                else                 
                {
                    if (!string.IsNullOrEmpty(imagen.NumCredito))
                    {
                        string catorcePrimeros = imagen.NumCredito[..14];
                        hoja.Cells[row, col].Value = catorcePrimeros;
                    }
                }

                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.ArchivoOrigen??"").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.NombreArchivo ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.NumCte ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.Acreditado ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.NumContrato ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;


                hoja.Cells[row, col].Value = (imagen.NumCredito ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                if (imagen.FechaDeModificacion != null)
                {
                    hoja.Cells[row, col].SetCellDate(imagen.FechaDeModificacion.Value);
                    CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                }
                col++;

                hoja.Cells[row, col].Value = (imagen.UsuarioDeModificacion ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = imagen.TamanioArchivo;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.RutaDeGuardado ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.UrlArchivo ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = imagen.Ministracion;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.VigenciaDelContato ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                if (imagen.FechaVigenciaDelaDispOContrato != null)
                {
                    hoja.Cells[row, col].SetCellDate(imagen.FechaVigenciaDelaDispOContrato.Value);
                    CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                }
                col++;
                hoja.Cells[row, col].Value = (imagen.CheckList ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ValidadoPorCalidad);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = (imagen.NumeroCreditoActivo ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                if (imagen.FechaApertura != null)
                {
                    hoja.Cells[row, col].SetCellDate(imagen.FechaApertura.Value);
                    CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                }
                col++;

                hoja.Cells[row, col].Value = Condiciones.ObtieneCastigo(imagen.Castigo);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = Condiciones.EsPeriodoDelDr(imagen.EsOrigenDelDoctor);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;
                hoja.Cells[row, col].Value = Condiciones.EstaEnABSaldosActivo(imagen.EsCarteraActiva);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.NumeroMinistracion;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.ClasifMesa ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                if (imagen.FechaAsignacionEntrada != null)
                {
                    hoja.Cells[row, col].SetCellDate(imagen.FechaAsignacionEntrada.Value);
                    CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                }
                col++;

                hoja.Cells[row, col].Value = imagen.MontoOtorgado;
                hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.Analista ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.Regional;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.Sucursal;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.CatAgencia ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.Extension ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.CarpetaDestino ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                /**/
                hoja.Cells[row, col].Value = imagen.ThreadId;
                hoja.Cells[row, col].Style.Numberformat.Format = "000";
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.Job;
                hoja.Cells[row, col].Style.Numberformat.Format = "000";
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.PorDescargar);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ErrorAlDescargar);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.MensajeDeErrorAlDescargar ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.PorOcr);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ErrorOcr);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.MensajeDeErrorOCR ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.PorAOcr);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ErrorAOcr);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.MensajeDeErrorAOCR ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.PorAcomodar);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ErrorAcomodar);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.MensajeDeErrorAcomodar ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.Termino);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.Hash ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.CadenaCredito ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.CadenaCliente ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.OcrNumCredito ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = (imagen.OcrNumCliente ?? "").Trim();
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.IgualNumCredito);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.TieneNumCredito);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = Condiciones.EsVerdaderoFalso(imagen.ChecarTona);
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.NumPaginas;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;

                hoja.Cells[row, col].Value = imagen.SubArchivo;
                CambiaColorSiNoEsPdf(hoja, row, col, imagen);
                col++;                

                row++;
            }
            #endregion

            #region autofiltro
            hoja.Cells[String.Format("A1:BF{0}", row - 1)].AutoFilter = true;  /// W XYZABCDEFGHIJKL
            hoja.View.FreezePanes(2, 1);
            #endregion

            package.Save();
            return true;
        }

        private bool EsArchivoValido(string nombreArchivo)         
        {
            if (_registraTodasLasImagenes)
                return true;
            bool esValido = nombreArchivo.ToUpper().Contains(".PDF", StringComparison.InvariantCultureIgnoreCase);
            esValido = esValido || nombreArchivo.ToUpper().Contains(".ZIP", StringComparison.InvariantCultureIgnoreCase);
            esValido = esValido || nombreArchivo.ToUpper().Contains(".7Z", StringComparison.InvariantCultureIgnoreCase);
            if (!esValido)
            {
                _logger.LogInformation("Archivo inválido :{nombreArchivo}", nombreArchivo);

            }
            return esValido;
        }

        private static string RemoveAccents(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string ObtieneDirectorioDestino(string origen) {


            if (origen.Contains("personal/drivemesacontrol_fnd_gob_mx/MesaControl", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\MC" + origen.Replace("personal/drivemesacontrol_fnd_gob_mx/MesaControl", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }

            if (origen.Contains("sites/Paperless/Archivo Digitales", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\AD" + origen.Replace("sites/Paperless/Archivo Digitales", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Biblioteca", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\BB" + origen.Replace("sites/Paperless/Biblioteca", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Cambios de Lineas", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\CL" + origen.Replace("sites/Paperless/Cambios de Lineas", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Crditos Liberados", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\CRL" + origen.Replace("sites/Paperless/Crditos Liberados", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Digitalizacion", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\DI" + origen.Replace("sites/Paperless/Digitalizacion", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Documentos compartidos", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\DC" + origen.Replace("sites/Paperless/Documentos compartidos", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Nivel de Riesgo", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\NR" + origen.Replace("sites/Paperless/Nivel de Riesgo", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/RESPALDO LIBERADOS", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\RL" + origen.Replace("sites/Paperless/RESPALDO LIBERADOS", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            if (origen.Contains("sites/Paperless/Revisin 20", StringComparison.OrdinalIgnoreCase))
            {
                string destino = "F:\\11\\RV" + origen.Replace("sites/Paperless/Revisin 20", "").Replace('/', Path.DirectorySeparatorChar);
                return RemoveAccents(destino).ToLower();
            }
            return "Error";
        }


        private static void CambiaColorSiNoEsPdf(ExcelWorksheet hoja, int row, int col, ArchivosImagenes imagen)
        {
            if (!(imagen.NombreArchivo ?? "").ToUpper().Contains(".PDF", StringComparison.InvariantCultureIgnoreCase))
            {
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(255, 0, 0));
                hoja.Cells[row, col].Style.Font.Color.SetColor(Color.FromArgb(255, 255, 255));
            }
        }

        public IEnumerable<ArchivosImagenes> RenumeraImagenes(IEnumerable<ArchivosImagenes> archivos)
        {
            int i = 1;
            foreach (var archivo in archivos)
            {
                archivo.Id = i;
                archivo.PorDescargar = true;
                archivo.PorOcr = true;
                archivo.PorAOcr = true;
                archivo.PorAcomodar = true;
                archivo.Termino = false;
                i++;
            }
            /// TODO: Poner los threads y los jobs

            var jobNumbersOneDrive = (from img in archivos
                                     where (img.CarpetaDestino ?? "").Contains("F:\\11\\OD")
                                     select img).ToList();
            i = 1;
            decimal numRegistrosPorJob = jobNumbersOneDrive.Count / 100;
            int cnt = 733; // Convert.ToInt32(Math.Truncate(numRegistrosPorJob));
            int job = 1;
            _logger.LogInformation("Se requieren {numRegistros} por Job de OneDrive", cnt);
            foreach (var archivo in jobNumbersOneDrive)
            {
                archivo.ThreadId = 101; // Asignado a One Drive
                archivo.Job = job;
                if (i >= cnt)
                {
                    job++;
                    i = 0;
                }
                i++;
            }

            var jobNumbersResto = (from img in archivos
                                   where !(img.CarpetaDestino ?? "").Contains("F:\\11\\OD")
                                   select img).ToList();
            i = 1;
            numRegistrosPorJob = jobNumbersOneDrive.Count / (100*100); // 100 Threads y 100 Jobs
            cnt = 100; // Convert.ToInt32(Math.Truncate(numRegistrosPorJob));
            int threadNumber = 1;
            int jobNumber = 1;
            _logger.LogInformation("Se requieren {numRegistrosNormales} por Job por Thread", cnt);
            foreach (var archivo in jobNumbersResto)
            {
                archivo.ThreadId = threadNumber; // Asignado a One Drive
                archivo.Job = jobNumber;
                if (i > cnt)
                {
                    threadNumber++;
                    
                    if (threadNumber > 100) {
                        threadNumber = 1;
                        jobNumber++;
                            }
                    i = 1;
                }
                i++;
            }
            return archivos.ToList();
        }

        public IEnumerable<ArchivosImagenes> CargaImagenesTratadas(string archivo = "")
        {
            if (string.IsNullOrEmpty(archivo))
                archivo = _archivoResultadoImagenes;
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
            if (!File.Exists(archivo))
            {
                _logger.LogError("No se encontró el Archivo de Ministraciones de la Mesa a procesar\n{archivoMinistracionesMesa}", archivo);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivo, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return resultado;
                var hoja = package.Workbook.Worksheets[0];
                ObtieneValoresCampos(hoja);
                int row = 2;
                bool salDelCiclo = false;
                while (!salDelCiclo)
                {
                    var obj = new ArchivosImagenes();

                    int col = 1;

                    var celda = hoja.Cells[row, col];
                    obj.Id = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    if (obj.Id == 0)
                        break;

                    celda = hoja.Cells[row, col];
                    obj.IdSearch = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ArchivoOrigen = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NombreArchivo = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumContrato = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    string numCredito = FNDExcelHelper.GetCellString(celda);
                    if (numCredito.Length < 18)
                        obj.NumCredito = numCredito;
                    else
                        obj.NumCredito = Condiciones.QuitaCastigo(numCredito);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaDeModificacion = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.UsuarioDeModificacion = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TamanioArchivo = Convert.ToInt64("0"+FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.RutaDeGuardado = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.UrlArchivo = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Ministracion = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.VigenciaDelContato = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaVigenciaDelaDispOContrato = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CheckList = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ValidadoPorCalidad = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumeroCreditoActivo = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsOrigenDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda))??false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsCarteraActiva = Condiciones.ObtieneEstaEnABSaldosActivo(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumeroMinistracion = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ClasifMesa = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaAsignacionEntrada = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Analista = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Regional = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Sucursal = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Extension = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CarpetaDestino = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ThreadId = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Job = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.PorDescargar = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ErrorAlDescargar = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MensajeDeErrorAlDescargar = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.PorOcr = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ErrorOcr = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MensajeDeErrorOCR = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.PorAOcr = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ErrorAOcr = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MensajeDeErrorAOCR = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.PorAcomodar = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ErrorAcomodar = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MensajeDeErrorAcomodar = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Termino = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Hash = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CadenaCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CadenaCliente = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.OcrNumCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.OcrNumCliente = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.IgualNumCredito = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TieneNumCredito = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ChecarTona = Condiciones.ObtieneEsVerdaderoFalso(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumPaginas = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.SubArchivo = FNDExcelHelper.GetCellInt(celda);
                    
                    resultado.Add(obj);

                    row++;
                }
            }
            #endregion

            return resultado.OrderBy(x => x.NumCredito).Select(y => y).ToList();
        }

        public AnalisisNombreImagen ObtieneNombreCalificado(string nombreOrigen)
        {
            AnalisisNombreImagen resultado = new()
            {
                NombreOriginal = nombreOrigen
            };

            // Elimina comas y punto y coma del nombre del archivo
            bool EsContrato = false;
            bool EsFonaga = false;
            bool EsSuplemento = false;
            bool EsRiesgo = false;
            int numCopia = 0;
            
            FileInfo fileInfo = new (nombreOrigen);
            resultado.NombreActual = nombreOrigen;
            string eliminoDirectorio = nombreOrigen.Replace((fileInfo.DirectoryName??"C:\\"),"");
            bool EsPdf = fileInfo.Extension.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase);
            string eliminoExtension = eliminoDirectorio.Replace(fileInfo.Extension ?? ".pdf", "", StringComparison.InvariantCultureIgnoreCase);
            string eliminoNumeros = eliminoExtension.Replace(Condiciones.ObtieneOtrosNumeros(eliminoExtension, 18), "", StringComparison.InvariantCultureIgnoreCase).Replace(Condiciones.ObtieneOtrosNumeros(eliminoExtension, 16), "", StringComparison.InvariantCultureIgnoreCase).Replace(Condiciones.ObtieneOtrosNumeros(eliminoExtension, 14), "", StringComparison.InvariantCultureIgnoreCase);
            char[] separadores = { ' ', '.', '|', '-', '{', '[', '(', '_', ')',']', '}' };
            string[] palabras = eliminoNumeros.Split(separadores).Where(x=>x.Trim().Length!=0).Select(y=>y).ToArray();
            if (palabras.Length == 0)
                return resultado;
            StringBuilder sb = new ();
            foreach (var palabra in palabras)
            {
                #region Es Numero
                bool EsNumero = false;
                long convierteEnNumero = 0;
                try { 
                    /// Reviso si se puede convertir a número
                    convierteEnNumero = Convert.ToInt64(palabra);
                    EsNumero = true;
                }
                catch { }
                if (EsNumero)
                {
                    if (convierteEnNumero < 10)
                    {
                        // Es un copia
                        numCopia = Convert.ToInt32(convierteEnNumero);
                        EsSuplemento = true;
                    }
                    else
                    {
                        if (convierteEnNumero < 99)
                        {
                            // Es otra ministracion
                            EsSuplemento = true;
                        }
                        else
                        {
                            if (convierteEnNumero < 999)
                            {
                                // Ministración extendida
                                EsSuplemento = true;
                            }
                            else
                            {
                                // Es parte del numero del documento   
                                EsSuplemento = false;
                            }
                        }
                    }
                }
                #endregion
                #region Palabras extra
                else
                {
                    string[] palabrasClave = new[] {"contrato", "conv", "mod", "anual", "modificada" };
                    if (palabrasClave.Any(x => palabra.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        EsContrato = true;
                    }
                    else
                    {
                        if (palabra.Equals("F")|| palabra.Contains("FON",StringComparison.InvariantCultureIgnoreCase))
                        {
                            EsFonaga = true;
                        }
                        else
                        {
                            if (palabra.Equals("S"))
                            {
                                EsSuplemento = true;
                            }
                            else {
                                // Son otras palabras, quizas basura
                                sb.Append(palabra + ' ');
                            }
                        }
                    }
                }
                #endregion
            }
            string[] extensions = new[] { "docx", "doc", "gif", "jfif", "jpeg", "jpg", "png" };
            EsRiesgo = (!EsPdf & (extensions.Any(x => (fileInfo.Extension ?? "").Contains(x, StringComparison.InvariantCultureIgnoreCase)))) && !EsContrato &&!EsFonaga && !EsSuplemento;
            EsContrato = (!EsRiesgo && !EsFonaga && !EsSuplemento);
            resultado.EsContrato = EsContrato;
            resultado.EsFonaga = EsFonaga;
            resultado.EsSuplemento = EsSuplemento;
            resultado.EsRiesgo = EsRiesgo;
            resultado.NoCopia = numCopia;
            resultado.Remanente = sb.ToString();
            return resultado;
        }
    }
}
