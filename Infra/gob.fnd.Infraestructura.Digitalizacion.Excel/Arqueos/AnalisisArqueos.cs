using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Excel.Arqueos;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Arqueos
{
    public class AnalisisArqueos : IAnalisisArqueos
    {
        private readonly ILogger<AnalisisArqueos> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _carpetaArqueos;

        public AnalisisArqueos(ILogger<AnalisisArqueos> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _carpetaArqueos = _configuration.GetValue<string>("carpetaArqueos") ??"";
        }

        public bool AnalisaArchivosArqueos(IEnumerable<ArchivosArqueos> informacion)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            foreach (var archivoArqueo in informacion)
            {
                #region Abrimos el excel de cara archivo de arqueo y lo analisamos
                FileStream fs = new(archivoArqueo.RutaCompleta??"", FileMode.Open, FileAccess.Read);
                using (var package = new ExcelPackage())
                {
                    package.Load(fs);
                    if (package.Workbook.Worksheets.Count == 0)
                        break;
                    var hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name.Contains("BASE ABSALDOS", StringComparison.OrdinalIgnoreCase));
                    if (hoja != null)
                    {
                        archivoArqueo.TieneHojaBaseABSaldos= true;
                    }
                    hoja = package.Workbook.Worksheets.FirstOrDefault(x=>(x.Name.Contains("PT Arqueo (VF)", StringComparison.OrdinalIgnoreCase)||
                    x.Name.Contains("PT Arqueo", StringComparison.OrdinalIgnoreCase)
                    ));
                    if (hoja != null)
                    {
                        archivoArqueo.TieneHojaPT = true;
                        if (string.Equals(archivoArqueo.NombreArchivo, "2.-Reporte de Arqueo Físico y Control de Resultados (RG-OPE-GVA-005-010).xlsx", StringComparison.InvariantCultureIgnoreCase))
                            _logger.LogInformation("Este es el que hay que revisar");
                        SeObtieneInformacionResultados(archivoArqueo, hoja);
                        SeObtieneInformacionGuardaValores(archivoArqueo, hoja);
                        if ((archivoArqueo.PrimerRenglonGuardaValores != 0) && (string.IsNullOrEmpty(archivoArqueo.CamposGuardaValores) || string.IsNullOrWhiteSpace(archivoArqueo.CamposGuardaValores)))
                        {
                            SeObtieneInformacionGuardaValores(archivoArqueo, hoja, desplazamiento: 4);
                        }
                    }
                    hoja = package.Workbook.Worksheets.FirstOrDefault(x => (x.Name.Contains("RG-OPE-GVA-005-010", StringComparison.OrdinalIgnoreCase)||
                    x.Name.Contains("RG OPE GVA 005 010", StringComparison.OrdinalIgnoreCase)
                    ));
                    if (hoja != null)
                    {
                        archivoArqueo.TieneHojaRgOpeGva005_010 = true;
                        if (string.Equals(archivoArqueo.NombreArchivo, "2.-Reporte de Arqueo Físico y Control de Resultados (RG-OPE-GVA-005-010).xlsx", StringComparison.InvariantCultureIgnoreCase))
                            _logger.LogInformation("Este es el que hay que revisar");
                        SeObtieneInformacionResultados(archivoArqueo, hoja);
                        int anteriorResultado = archivoArqueo.PrimerRenglonResultado;
                        int numeroResultados = archivoArqueo.NumeroResultados;
                        try
                        {
                            archivoArqueo.PrimerRenglonResultado = 1;
                            archivoArqueo.NumeroResultados = 0;
                            SeObtieneInformacionGuardaValores(archivoArqueo, hoja);
                            if ((archivoArqueo.PrimerRenglonGuardaValores != 0) && (string.IsNullOrEmpty(archivoArqueo.CamposGuardaValores) || string.IsNullOrWhiteSpace(archivoArqueo.CamposGuardaValores)))
                            {
                                SeObtieneInformacionGuardaValores(archivoArqueo, hoja, desplazamiento: 4);
                            }

                        }
                        finally
                        {
                            archivoArqueo.PrimerRenglonResultado = anteriorResultado;
                            archivoArqueo.NumeroResultados = numeroResultados;
                        }
                    }

                    if (!(archivoArqueo.TieneHojaPT || archivoArqueo.TieneHojaBaseABSaldos || archivoArqueo.TieneHojaRgOpeGva005_010))
                    {
                        hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name.Contains("Hoja1", StringComparison.OrdinalIgnoreCase));
                        if (hoja != null)
                        {
                            archivoArqueo.TieneHojaPT = true;
                            if (string.Equals(archivoArqueo.NombreArchivo, "2.-Reporte de Arqueo Físico y Control de Resultados (RG-OPE-GVA-005-010).xlsx", StringComparison.InvariantCultureIgnoreCase))
                                _logger.LogInformation("Este es el que hay que revisar");
                            SeObtieneInformacionResultados(archivoArqueo, hoja);
                            int anteriorResultado = archivoArqueo.PrimerRenglonResultado;
                            int numeroResultados = archivoArqueo.NumeroResultados;
                            try
                            {
                                archivoArqueo.PrimerRenglonResultado = 1;
                                archivoArqueo.NumeroResultados = 0;
                                SeObtieneInformacionGuardaValores(archivoArqueo, hoja);
                                if ((archivoArqueo.PrimerRenglonGuardaValores != 0) && (string.IsNullOrEmpty(archivoArqueo.CamposGuardaValores) || string.IsNullOrWhiteSpace(archivoArqueo.CamposGuardaValores)))
                                {
                                    SeObtieneInformacionGuardaValores(archivoArqueo, hoja, desplazamiento: 4);
                                }

                            }
                            finally
                            {
                                archivoArqueo.PrimerRenglonResultado = anteriorResultado;
                                archivoArqueo.NumeroResultados = numeroResultados;
                            }
                        }
                    }
                    
                }
                #endregion
            }
            return true;
        }

        private void SeObtieneInformacionGuardaValores(ArchivosArqueos archivoArqueo, ExcelWorksheet? hoja, int desplazamiento = 1)
        {            
            _logger.LogInformation("Comienza a obtener información de los Guarda Valores desde el archivo {carpetaArchivo} {archivoArqueo}", archivoArqueo.Carpeta, archivoArqueo.NombreArchivo);
            if (hoja == null)
                return;
            int renglonInicialBusqueda = archivoArqueo.PrimerRenglonResultado + archivoArqueo.NumeroResultados + 1;

            // Localizo el renglon de los encabezados de los Guarda Valores, para ello busco la palabra acreditado en la hoja de excel
            var encuentraInformacionAcreditados = hoja.SearchText("Acreditado", renglonInicialBusqueda, 1, renglonInicialBusqueda + 60, 30);
            if (encuentraInformacionAcreditados != null)
            {
                ExcelAddress direccion = new(encuentraInformacionAcreditados.Address);
                // Si existe acreditado en el resultado
                while (encuentraInformacionAcreditados.Text.Length > 20) {
                    
                    encuentraInformacionAcreditados = hoja.SearchText("Acreditado", direccion.Start.Row +1, 1, renglonInicialBusqueda + 60, 30);
                    if (encuentraInformacionAcreditados == null)
                        return;
                    direccion = new(encuentraInformacionAcreditados.Address);
                }
                // Ahora en el siguiente renglon, se busca la columna de inicio
                bool tieneDosRenglones = false;
                int columnaInicialGuardaValores = 1;
                int columnaEncuentro = direccion.Start.Column;
                int renglonGuardaValores = direccion.Start.Row;
                if (string.Equals(hoja.Cells[renglonGuardaValores, columnaEncuentro].Text, hoja.Cells[renglonGuardaValores + 1, columnaEncuentro].Text, StringComparison.InvariantCultureIgnoreCase))
                    tieneDosRenglones = true;
                archivoArqueo.PrimerRenglonGuardaValores = renglonGuardaValores + 1;
                string valorABuscar;
                if (!tieneDosRenglones)
                    valorABuscar = hoja.Cells[renglonGuardaValores + 1, columnaInicialGuardaValores].Text ?? "";
                else
                    valorABuscar = hoja.Cells[renglonGuardaValores + 2, columnaInicialGuardaValores].Text ?? "";

                //
                if (string.Equals(archivoArqueo.RutaCompleta, "D:\\OD\\OneDrive - FND\\Arqueos de Agencias\\Sur\\Córdoba\\PT 04 2022.xlsx", StringComparison.InvariantCultureIgnoreCase))
                {
                    tieneDosRenglones = true;
                    columnaInicialGuardaValores = 1;
                    desplazamiento = 4;
                    valorABuscar = "SI";
                    archivoArqueo.PrimerRenglonGuardaValores = renglonGuardaValores + desplazamiento;
                    archivoArqueo.PrimerColumnaDetalleGuardaValores = 1;
                    archivoArqueo.CamposGuardaValores = "Tipo de Documento|#|No. de Acreditado|Acreditado|No. de Credito|Tipo de producto|fecha de ministración|Importe Ministrado|Ejecutivo de Financamiento Rural|Estatus|Salida de guardavalores (Fecha)|Coordinador de Guardavalor (firmado)|Ejecutivo de Cobranza|Fecha de sello Supervisión y Cobranza|/ Incorrecto /|El contrato de crédito|Fecha del Contrato|Escritura publica|aportación acreditado|Observaciones|Monto|Observaciones|Hallazgos";
                    archivoArqueo.NumeroGuardaValores = 1404;
                }

                while (string.IsNullOrEmpty(valorABuscar))
                {
                    columnaInicialGuardaValores++;
                    if (columnaInicialGuardaValores > 120)
                    {
                        _logger.LogError("Existe un error al buscar los valores del guadavalores del archivo {archivo}", archivoArqueo.NombreArchivo);
                        return;
                    }
                    // reviso si son 2 renglones de detalle
                    if ((columnaInicialGuardaValores > columnaEncuentro) && (!tieneDosRenglones))
                    {
                        columnaInicialGuardaValores = 1;
                        tieneDosRenglones = true;
                    }
                    if (!tieneDosRenglones)
                        valorABuscar = hoja.Cells[renglonGuardaValores + 1, columnaInicialGuardaValores].Text ?? "";
                    else
                        valorABuscar = hoja.Cells[renglonGuardaValores + 1 + desplazamiento, columnaInicialGuardaValores].Text ?? "";
                }
                if (tieneDosRenglones)
                    archivoArqueo.PrimerRenglonGuardaValores = renglonGuardaValores + desplazamiento;
                archivoArqueo.PrimerColumnaDetalleGuardaValores = columnaInicialGuardaValores;
                // Ahora en el renglon de los encabezados de los campos de los Guarda Valores, busco hasta encontrar la columna "Detalle" que es el Detalle del hallazgo
                valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                string cierreEncabezado = "Detalle";
                var existeDetalle = hoja.SearchText(cierreEncabezado, renglonGuardaValores, columnaInicialGuardaValores, renglonGuardaValores + desplazamiento, 50);
                if (existeDetalle == null) {
                    cierreEncabezado = "Observaciones  Auditoría";
                    existeDetalle = hoja.SearchText(cierreEncabezado, renglonGuardaValores, columnaInicialGuardaValores, renglonGuardaValores + desplazamiento, 50);
                }
                if (existeDetalle != null) // por si no existe la columna llamada detalle
                {
                    ExcelAddress direccionDetalle = new(existeDetalle.Address);
                    string campos = "";
                    while (!valorABuscar.Contains(cierreEncabezado, StringComparison.InvariantCultureIgnoreCase) &&
                        (columnaInicialGuardaValores <= direccionDetalle.Start.Column) // Valido no pasarme de la columna detalle
                        )
                    {
                        if (!string.IsNullOrEmpty(campos))
                            campos += "|";
                        // Si está en blanco en el primer renglon y la columna es superior a la columna de acreditado, me paso al segundo renglon 
                        if ((string.IsNullOrEmpty(valorABuscar) || string.IsNullOrWhiteSpace(valorABuscar)) && (columnaInicialGuardaValores > columnaEncuentro) )
                        {
                            if (tieneDosRenglones) // valido que el encabezado tenga 2 renglones
                            {
                                if (renglonGuardaValores < archivoArqueo.PrimerRenglonGuardaValores)
                                {
                                    // me paso al segundo renglon
                                    renglonGuardaValores += desplazamiento;
                                    // me regreso una columna para conocer el detalle de los campos que voy a revisar
                                    columnaInicialGuardaValores--;
                                    if (columnaInicialGuardaValores < archivoArqueo.PrimerColumnaDetalleGuardaValores)
                                        break;
                                    valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                                }
                                else  // Puede estar vacío el segundo renglon
                                {
                                    campos += "(vacia)";
                                    columnaInicialGuardaValores++;
                                    valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                                }
                            }
                            else 
                            {
                                if ((columnaEncuentro > columnaInicialGuardaValores))
                                {
                                    campos += "(vacia)";
                                    columnaInicialGuardaValores++;
                                    valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                                }
                                else
                                    break;
                            }

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(valorABuscar) || string.IsNullOrWhiteSpace(valorABuscar))
                            {
                                // Si la columna es anterior a la columna de Acreditado, puede venir vacía, eso pasa cuando no describen el # de registro
                                if ((columnaEncuentro > columnaInicialGuardaValores))
                                {
                                    // La columna viene en blanco
                                    campos += "(vacia)";
                                    columnaInicialGuardaValores++;
                                    valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                                }
                                else
                                    // Error                                
                                    break;
                            }
                            else
                            {
                                campos += valorABuscar;
                                columnaInicialGuardaValores++;
                                valorABuscar = hoja.Cells[renglonGuardaValores, columnaInicialGuardaValores].Text ?? "";
                            }
                        }
                    }
                    if (!(columnaInicialGuardaValores >= 50))
                        campos += "|Detalle";
                    archivoArqueo.CamposGuardaValores = campos;
                }
            }
        }

        private  void SeObtieneInformacionResultados(ArchivosArqueos archivoArqueo, ExcelWorksheet? hoja)
        {
            _logger.LogInformation("Comienza a obtener información de los resultados desde el archivo {carpetaArchivo} {archivoArqueo}", archivoArqueo.Carpeta,archivoArqueo.NombreArchivo);
            #region Se obtiene el resultado
            if (hoja == null)
                return;
            var primerCeldaResultado = hoja.SearchText("Resultado", 1, 1, 21, 30);
            if (primerCeldaResultado != null)
            {
                ExcelAddress direccion = new(primerCeldaResultado.Address);
                // Si existe acreditado en el resultado
                while (primerCeldaResultado.Text.Length > 15)
                {

                    primerCeldaResultado = hoja.SearchText("Resultado", direccion.Start.Row + 1, 1, direccion.Start.Row + 60, 30);
                    if (primerCeldaResultado == null)
                        return;
                    direccion = new(primerCeldaResultado.Address);
                }

                if (primerCeldaResultado.GetCellString().Contains("Resultado", StringComparison.InvariantCultureIgnoreCase))
                {
                    direccion = new(primerCeldaResultado.Address);

                    int renglonResultado = direccion.Start.Row;
                    int columnaResultado = direccion.Start.Column;

                    archivoArqueo.PrimerColumnaResultado = columnaResultado;
                    string columnas = primerCeldaResultado.GetCellString();
                    bool dosRenglonsEncabezado = false;

                    int renglonValores = renglonResultado + 1;
                    string valorResultado = hoja.Cells[renglonValores, columnaResultado].Text ?? "";
                    if (string.IsNullOrEmpty(valorResultado))
                    {
                        renglonValores++;
                        valorResultado = hoja.Cells[renglonValores, columnaResultado].Text ?? "";
                    }
                    if (!string.IsNullOrEmpty(valorResultado))
                    {
                        dosRenglonsEncabezado = true;
                        archivoArqueo.PrimerRenglonResultado = renglonValores;
                        // obtengo cuantos resultados tengo
                        int cantidadResultados = 0;
                        while (!string.IsNullOrEmpty(valorResultado))
                        {
                            valorResultado = hoja.Cells[renglonValores + cantidadResultados, columnaResultado].Text ?? "";
                            cantidadResultados++;
                        }
                        archivoArqueo.NumeroResultados = cantidadResultados - 1;
                    }

                    columnaResultado += 2; // El resultado pone 2 columnas, el # de resultado y su descripción
                    string valorColumna = hoja.Cells[renglonResultado, columnaResultado].GetCellString();
                    while (!string.IsNullOrEmpty(valorColumna))
                    {
                        columnas += "|" + valorColumna;
                        columnaResultado++;
                        valorColumna = hoja.Cells[renglonResultado, columnaResultado].GetCellString();
                        if (string.IsNullOrEmpty(valorColumna) && dosRenglonsEncabezado)
                        {
                            renglonResultado++;
                            valorColumna = hoja.Cells[renglonResultado, columnaResultado].GetCellString();
                        }
                    }
                    archivoArqueo.CamposResultado = columnas;
                }

            }
            #endregion
        }

        public bool GuardaInformacionArqueos(IEnumerable<ArchivosArqueos> informacion, string archivoDestino)
        {
            if (File.Exists(archivoDestino))
            {
                _logger.LogError("Se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldos}", archivoDestino);
                return false;
            }


            #region Actualización de Datos
            if (File.Exists(archivoDestino))
            {
                _logger.LogError("Ya existía el archivo de los nuevos creditos\n{archivoDestino}\n se procede a eliinarlo", archivoDestino);
                File.Delete(archivoDestino);
            }

            FileStream fsO = new(archivoDestino, FileMode.CreateNew, FileAccess.Write);
            using (var package = new ExcelPackage(fsO))
            {
                _logger.LogWarning(
                    "Se comienzan a vaciar los nuevos creditos en el archivo destino\n{archivoDestino}\n con un total de {totalCreditosNuevos}",
                    archivoDestino,
                    informacion.Count());
                FileInfo fi = new(archivoDestino);
                string nombreHoja = "ResumenArchivos"; // = fi.Name[9..].Replace("202", "2").Replace(" Norte.xlsx", "");
                ExcelWorksheet hoja = package.Workbook.Worksheets.Add(nombreHoja);
                hoja.Name = nombreHoja;

                int row = 1;
                int col = 1;
                hoja.Cells[row, col].Value = "Carpeta origen";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                col++;
                hoja.Cells[row, col].Value = _carpetaArqueos;
                col = 1;
                row++;
                hoja.Cells[row, col].Value = "#"; // Id
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 12.22 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Carpeta";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 36.67 + 0.78;                
                col++;
                hoja.Cells[row, col].Value = "Archivo";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 69.33 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Hoja PT";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Base AB";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "RG-OPE-GVA-005-010";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Rslt Col";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9.11 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Campos Resultado";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 117.22 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "1er ren rslt";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "# rslt";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Guarda Valores";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9.11 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Campos GuardaValores";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 255;
                col++;
                hoja.Cells[row, col].Value = "1er ren GV";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "# GV";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "num_credito";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 18.33 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "sucursal";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.78 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Coordinacion";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 36.67 + 0.78;
                col = 1;
                row++;


                foreach (var archivo in informacion)
                {
                    int celda = 1;
                    hoja.Cells[row, celda].Value = archivo.Id;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.Carpeta;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NombreArchivo;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaPT?"Si":"No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaBaseABSaldos ? "Si" : "No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaRgOpeGva005_010 ? "Si" : "No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.PrimerColumnaResultado;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.CamposResultado;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.PrimerRenglonResultado;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NumeroResultados;
                    celda++;

                    hoja.Cells[row, celda].Value = archivo.PrimerColumnaDetalleGuardaValores;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.CamposGuardaValores;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.PrimerRenglonGuardaValores;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NumeroGuardaValores;
                    celda++;

                    hoja.Cells[row, celda].Value = archivo.NoCreditoEjemplo;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NoAgencia;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.Coordinacion;
                    celda++;

                    row++;
                }

                #region Vacía los campos de Guarda Valor
                nombreHoja = "Campos GV"; // = fi.Name[9..].Replace("202", "2").Replace(" Norte.xlsx", "");
                hoja = package.Workbook.Worksheets.Add(nombreHoja);
                hoja.Name = nombreHoja;

                row = 1;
                col = 1;

                hoja.Cells[row, col].Value = "#"; // Id
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 12.22 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Carpeta";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 36.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Archivo";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 69.33 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Hoja PT";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Base AB";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "RG-OPE-GVA-005-010";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "# GV";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 9 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "# Campos GV";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;
                hoja.Cells[row, col].Value = "Campos";
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
                hoja.Column(col).Width = 6.67 + 0.78;
                col++;

                row = row + 1;
                foreach (var archivo in informacion)
                {
                    int celda = 1;
                    hoja.Cells[row, celda].Value = row - 1;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.Carpeta;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NombreArchivo;
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaPT ? "Si" : "No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaBaseABSaldos ? "Si" : "No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.TieneHojaRgOpeGva005_010 ? "Si" : "No";
                    celda++;
                    hoja.Cells[row, celda].Value = archivo.NumeroGuardaValores;
                    celda++;

                    string[] camposGV = (archivo.CamposGuardaValores??"").Split('|');
                    hoja.Cells[row, celda].Value = camposGV.Length;
                    celda++;

                    foreach (string campo in camposGV)
                    {
                        hoja.Cells[row, celda].Value = campo;
                        celda++;
                    }

                    row++;
                }
                #endregion

                package.Save();
                _logger.LogWarning("Se terminó de procesar los archivos de arqueo en:\n{archivoDestino}\n", archivoDestino);
            }

            #endregion
            return true;
        }
    }
}
