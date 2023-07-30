using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.GuardaValores;
using gob.fnd.ExcelHelper;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Arqueos;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.GuardaValores;

public class AdministraGuardaValores : IAdministraGuardaValores
{
    const int C_INT_AUDITOR = 2 - 1;
    const int C_INT_ACREDITADO = 4 - 1;
    const int C_INT_NUMCTE = 3 - 1;
    const int C_INT_NUMERO_CREDITO = 5 - 1;
    const int C_INT_CAT_TIPO_CREDITO = 6 - 1;
    const int C_INT_CAT_TIPO_DOCUMENTO = 7 - 1;
    const int C_INT_NUMERO_DE_DOCUMENTO = 9 - 1;
    const int C_INT_IMPORTE = 8 - 1;
    const int C_INT_NUMERO_DE_HOJAS = 11 - 1;
    const int C_INT_ESTATUS = 10 - 1;
    const int C_INT_TIPO_DE_HALLAZGO = 12 - 1;
    const int C_INT_DETALLE = 13 - 1;
    const int C_INT_CORRECTO_INCORRECTO = 16 - 1;

    private readonly ILogger<AdministraGuardaValores> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _archivoGuardaValor;

    public AdministraGuardaValores(ILogger<AdministraGuardaValores> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _archivoGuardaValor = _configuration.GetValue<string>("archivoConcentradoGV") ?? "";
    }
    public IEnumerable<InformacionGuardaValor> CargaInformacionGuardaValores(IEnumerable<ArchivoAnalisisCamposArqueos> archivosYCampos)
    {
        IList<InformacionGuardaValor> lista = new List<InformacionGuardaValor>();
        foreach (var archivo in archivosYCampos)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivo.RutaCompleta))
            {
                _logger.LogError("No encontró el archivo {archivoGuardaValor}", archivo.NombreArchivo);
                break;
            }
            int id = 1;
            FileStream fs = new(archivo.RutaCompleta ?? "", FileMode.Open, FileAccess.Read);
            using var package = new ExcelPackage();
            ExcelWorksheet? hoja = null;
            package.Load(fs);
            if (archivo.TieneHojaPT)
            {
                hoja = package.Workbook.Worksheets.FirstOrDefault(x => (x.Name.Contains("PT Arqueo (VF)", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Contains("PT Arqueo", StringComparison.OrdinalIgnoreCase)
                    ));

            }
            if (archivo.TieneHojaRgOpeGva005_010 && (hoja is null))
            {
                hoja = package.Workbook.Worksheets.FirstOrDefault(x => (x.Name.Contains("RG-OPE-GVA-005-010", StringComparison.OrdinalIgnoreCase) ||
                x.Name.Contains("RG OPE GVA 005 010", StringComparison.OrdinalIgnoreCase)
                ));
            }
            if (hoja is null)
                hoja ??= package.Workbook.Worksheets.FirstOrDefault(x => x.Name.Contains("Hoja1", StringComparison.OrdinalIgnoreCase));
            int renglon = archivo.PrimerRenglonGuardaValores;
            if (hoja is not null)
            {
                // Primero obtengo los campos
                int[] celdaCampos = ObtieneValoresCampos(hoja, archivo);
                // luego obtengo los valores de cada renglon, basado en los campos encontrados
                bool renglonVacio = false;
                renglon = archivo.PrimerRenglonGuardaValores - 1;

                string numeroCredito = "000000000000000000";

                while (!renglonVacio)
                {
                    renglon++;
                    InformacionGuardaValor? info = ObtieneInformacionRenglon(celdaCampos, renglon, hoja, archivo);

                    if (info is null && (renglon == archivo.PrimerRenglonGuardaValores))
                        renglonVacio = false;
                    else
                    {
                        // Se considera que no encontró información si el número de crédito y el tipo de documento están en blanco
                        if (info is not null)
                        {
                            if ((info.Acreditado is not null)
                                &&
                                !info.Acreditado.Trim().Equals("Acreditado", StringComparison.InvariantCultureIgnoreCase)) // Valido que no se llame acreditado el acreditado
                            {
                                info.Id = id;
                                renglonVacio = string.IsNullOrEmpty(info.NumeroCredito) && string.IsNullOrEmpty(info.TipoDeDocumento);
                                // Si hay información, la agrego a la lista
                                if (!renglonVacio)
                                {
                                    if (string.IsNullOrEmpty(info.NumeroCredito))
                                        info.NumeroCredito = numeroCredito;
                                    else
                                        numeroCredito = info.NumeroCredito;

                                    if (!string.IsNullOrEmpty(info.CorrectoIncorrecto) && "INCORRECTO".Contains(info.CorrectoIncorrecto, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (!string.IsNullOrEmpty(info.TipoDeHallazgo))
                                            info.MontoDelHallazgo = info.Importe;
                                    }

                                    lista.Add(info);
                                    id++;
                                }
                            }
                        }
                        else
                            renglonVacio = true;
                    }
                }

            }
        }
        return lista;
    }


    public bool GuardaInformacionGuardaValores(IEnumerable<InformacionGuardaValor> guardaValores, string archivoGuardaValor = "")
    {
        if (string.IsNullOrEmpty(archivoGuardaValor))
            archivoGuardaValor = _archivoGuardaValor;
        if (string.IsNullOrEmpty(archivoGuardaValor))
            return false;
        if (File.Exists(archivoGuardaValor))
            File.Delete(archivoGuardaValor);

        FileStream fs = new(archivoGuardaValor ?? "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var package = new ExcelPackage(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.Add("GuardaValores");
        hoja.Name = "GuardaValores";
        #region Escribo los encabezados
        int row = 1;
        int col = 1;
        hoja.Cells[row, col].Value = "#"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 8.89 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "REGIONAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 3 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "SUCURSAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 3.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Archivo que origina la información"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; // 88.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Auditor"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 6.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Acreditado"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 85.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUM_CTE"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 12.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUMERO_CREDITO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 19.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUM_PRODUCTO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "CAT_TIPO_CREDITO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 28.56 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "TIPO_DE_DOCUMENTO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 21.44 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUMERO_DE_DOCUMENTO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 26.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "IMPORTE"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.67 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUMERO_DE_HOJAS"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 17.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "FECHA_DE_VENCIMIENTO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "ESTADO_FISICO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 13.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "UBICACION_FISICA"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "ESTATUS"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18.44 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "CORRECTO / INCORRECTO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 22.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "TIPO_DE_HALLAZGO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 41.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "MONTO_DEL_HALLAZGO"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.67 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "DETALLE"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 51.56 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Tiene expediente Digital"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 9.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Castigo"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 6.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "En GV y sin ABSaldos"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 7.78 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Apertura en tiempo del Dr"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 11.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Esta en AB Saldos Castigo"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 8.44 + 0.78;
        row++;
        #endregion

        #region Vacio los campos
        foreach (var guardaValor in guardaValores)
        {
            col = 1;
            hoja.Cells[row, col].Value = guardaValor.Id;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Region;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Sucursal;
            col++;
            hoja.Cells[row, col].Value = guardaValor.ArchivoOrigenInformacion;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Auditor;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Acreditado;
            col++;
            hoja.Cells[row, col].Value = guardaValor.NumCte;
            col++;
            hoja.Cells[row, col].Value = guardaValor.NumeroCredito;

            col++;
            hoja.Cells[row, col].Value = guardaValor.NumProducto;
            col++;
            hoja.Cells[row, col].Value = guardaValor.CatTipoCredito;
            col++;
            hoja.Cells[row, col].Value = guardaValor.TipoDeDocumento;
            col++;
            hoja.Cells[row, col].Value = guardaValor.NumeroDeDocumento;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Importe;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";


            col++;
            hoja.Cells[row, col].Value = guardaValor.NumeroDeHojas;
            col++;
            if (guardaValor.FechaDeVencimiento is not null)
            {
                hoja.Cells[row, col].SetCellDate(guardaValor.FechaDeVencimiento.Value);
                //                        .Value = guardaValor.FechaDeVencimiento;
            }
            col++;
            hoja.Cells[row, col].Value = guardaValor.EstadoFisico;
            col++;
            hoja.Cells[row, col].Value = guardaValor.UbicacionFisica;
            col++;
            hoja.Cells[row, col].Value = guardaValor.Estatus;
            col++;
            hoja.Cells[row, col].Value = guardaValor.CorrectoIncorrecto;
            col++;
            hoja.Cells[row, col].Value = guardaValor.TipoDeHallazgo;
            col++;
            hoja.Cells[row, col].Value = guardaValor.MontoDelHallazgo;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";


            col++;
            hoja.Cells[row, col].Value = guardaValor.Detalle;
            col++;
            hoja.Cells[row, col].Value = Condiciones.TieneExpedienteDigital(guardaValor.TieneExpedienteDigital);
            col++;
            hoja.Cells[row, col].Value = Condiciones.ObtieneCastigo(guardaValor.Castigo);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EstaEnArqueosSinABSaldo(guardaValor.EsNuevo);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsPeriodoDelDr(guardaValor.AperturaTiempoDoctor);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsCarteraABSaldos(guardaValor.AunEnABSaldos);
            row++;
        }
        #endregion
        #region autofiltro
        hoja.Cells[String.Format("A1:AA{0}", row - 1)].AutoFilter = true;
        hoja.View.FreezePanes(2, 1);
        #endregion

        package.Save();
        return true;
    }

    private InformacionGuardaValor? ObtieneInformacionRenglon(int[] celdaCampos, int renglon, ExcelWorksheet hoja, ArchivoAnalisisCamposArqueos archivo)
    {
        InformacionGuardaValor resultado = new()
        {
            // id = 0

            ArchivoOrigenInformacion = Path.Combine(archivo.Carpeta ?? "", archivo.NombreArchivo ?? "")
        };

        #region Numero de Credito
        if (celdaCampos[C_INT_NUMERO_CREDITO] != -1)
        {
            try
            {
                resultado.NumeroCredito =
                    hoja.Cells[renglon, celdaCampos[C_INT_NUMERO_CREDITO]].Text;
                if (resultado.NumeroCredito is not null && resultado.NumeroCredito.Contains('´', StringComparison.InvariantCultureIgnoreCase))
                {
                    resultado.NumeroCredito = resultado.NumeroCredito.Replace("´", "");
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Tipo de Documento
        if (celdaCampos[C_INT_CAT_TIPO_DOCUMENTO] != -1)
        {
            try
            {
                resultado.TipoDeDocumento =
                    hoja.Cells[renglon, celdaCampos[C_INT_CAT_TIPO_DOCUMENTO]].Text;
            }
            catch
            {
            }
        }
        #endregion

        if (string.IsNullOrWhiteSpace(resultado.NumeroCredito) && string.IsNullOrWhiteSpace(resultado.TipoDeDocumento))
        {
            _logger.LogInformation("Numero total de renglones: {noRenglones} del archivo: {nombreArchivo}", renglon, archivo.NombreArchivo);
            return null;
        }

        #region Auditor
        if (celdaCampos[C_INT_AUDITOR] != -1)
        {
            try
            {
                resultado.Auditor =
                    hoja.Cells[renglon, celdaCampos[C_INT_AUDITOR]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Acreditado
        if (celdaCampos[C_INT_ACREDITADO] != -1)
        {
            try
            {
                resultado.Acreditado =
                    hoja.Cells[renglon, celdaCampos[C_INT_ACREDITADO]].Text;

                if (resultado.Acreditado is not null)
                {
                    resultado.Acreditado = resultado.Acreditado.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("Ð", "Ñ");
                }



            }
            catch
            {
            }
        }
        #endregion

        #region Numero de Cliente
        if (celdaCampos[C_INT_NUMCTE] != -1)
        {
            try
            {
                resultado.NumCte =
                    hoja.Cells[renglon, celdaCampos[C_INT_NUMCTE]].Text;
                if (resultado.NumCte is not null && resultado.NumCte.Contains('´', StringComparison.InvariantCultureIgnoreCase))
                {
                    resultado.NumCte = resultado.NumCte.Replace("´", "");
                }

            }
            catch
            {
            }
        }
        #endregion

        #region Tipo de Credito
        if (celdaCampos[C_INT_CAT_TIPO_CREDITO] != -1)
        {
            try
            {
                resultado.CatTipoCredito =
                    hoja.Cells[renglon, celdaCampos[C_INT_CAT_TIPO_CREDITO]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Numero de Documento
        if (celdaCampos[C_INT_NUMERO_DE_DOCUMENTO] != -1)
        {
            try
            {
                resultado.NumeroDeDocumento =
                    hoja.Cells[renglon, celdaCampos[C_INT_NUMERO_DE_DOCUMENTO]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Importe
        if (celdaCampos[C_INT_IMPORTE] != -1)
        {
            try
            {
                resultado.Importe = Convert.ToDecimal(
                    hoja.Cells[renglon, celdaCampos[C_INT_IMPORTE]].Text);
            }
            catch
            {
                try
                {
                    resultado.Importe = hoja.Cells[renglon, celdaCampos[C_INT_IMPORTE]].GetCellValue<Decimal>();
                }
                catch { }
            }
        }
        #endregion

        #region Numero de hojas
        if (celdaCampos[C_INT_NUMERO_DE_HOJAS] != -1)
        {
            try
            {
                resultado.NumeroDeHojas = Convert.ToInt32(
                    hoja.Cells[renglon, celdaCampos[C_INT_NUMERO_DE_HOJAS]].Text);
            }
            catch
            {
            }
        }
        #endregion

        #region Estatus
        if (celdaCampos[C_INT_ESTATUS] != -1)
        {
            try
            {
                resultado.Estatus =
                    hoja.Cells[renglon, celdaCampos[C_INT_ESTATUS]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Tipo de Hallazgo
        if (celdaCampos[C_INT_TIPO_DE_HALLAZGO] != -1)
        {
            try
            {
                resultado.TipoDeHallazgo =
                    hoja.Cells[renglon, celdaCampos[C_INT_TIPO_DE_HALLAZGO]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Detalle
        if (celdaCampos[C_INT_DETALLE] != -1)
        {
            try
            {
                resultado.Detalle =
                    hoja.Cells[renglon, celdaCampos[C_INT_DETALLE]].Text;
            }
            catch
            {
            }
        }
        #endregion

        #region Correcto / Incorrecto
        if (celdaCampos[C_INT_CORRECTO_INCORRECTO] != -1)
        {
            try
            {
                resultado.CorrectoIncorrecto =
                    hoja.Cells[renglon, celdaCampos[C_INT_CORRECTO_INCORRECTO]].Text;
            }
            catch
            {
            }
        }
        #endregion            
        return resultado;
    }

    private int[] ObtieneValoresCampos(ExcelWorksheet hoja, ArchivoAnalisisCamposArqueos archivo)
    {
        int primeraColumnaGuardaValores;
        int primerRenglonGuardaValores; // el primer renglon es +1
        // Por default todos los campos no son encontrados
        int[] resultado = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        int campoBusqueda = 0;
        // Si no hay informacion de los campos, ni entro
        if (archivo.CamposDeInformacionGuardaValor != null)
        {
            // recorro cada nombre de campo
            foreach (var campo in archivo.CamposDeInformacionGuardaValor)
            {
                if (campo.Contains("Correct", StringComparison.InvariantCultureIgnoreCase))
                    _logger.LogTrace("Revisar correcto e incorrecto");
                primerRenglonGuardaValores = archivo.PrimerRenglonGuardaValores; // el primer renglon es +1
                primeraColumnaGuardaValores = archivo.PrimerColumnaDetalleGuardaValores;
                if (!string.IsNullOrEmpty(campo) && !string.IsNullOrWhiteSpace(campo))
                {
                    ExcelRange? celda = null;
                    while (celda is null && primerRenglonGuardaValores > 0)
                    {
                        celda = hoja.SearchText(campo, primerRenglonGuardaValores, primeraColumnaGuardaValores, primerRenglonGuardaValores, primeraColumnaGuardaValores + archivo.NumeroCamposGuardaValor, true);
                        if (celda is null)
                        {
                            primerRenglonGuardaValores--;
                        }
                    }
                    // Solo si lo encuentra coloco el número de campo
                    if (celda is not null && primerRenglonGuardaValores > 0)
                    {
                        ExcelAddress direccionCelda = new(celda.Address);
                        resultado[campoBusqueda] = direccionCelda.Start.Column;
                        _logger.LogTrace("Encontró valor {campoBusqueda} {archivo} {columna}", campo, archivo.NombreArchivo, resultado[campoBusqueda]);
                    }
                    else
                        _logger.LogTrace("No encontró valor {campoBusqueda} {archivo} {columna}", campo, archivo.NombreArchivo, resultado[campoBusqueda]);
                }
                campoBusqueda++;
            }
        }
        return resultado;
    }

    public IEnumerable<InformacionGuardaValor> CargaInformacionGuardaValoresPrevia(string archivoGuardaValor = "")
    {
        if (string.IsNullOrEmpty(archivoGuardaValor))
            archivoGuardaValor = _archivoGuardaValor;
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<InformacionGuardaValor> resultado = new List<InformacionGuardaValor>();
        if (!File.Exists(_archivoGuardaValor))
        {
            _logger.LogError("No se encontró el Archivo de Guarda Valores a Procesar\n{archivoMinistracionesMesa}", _archivoGuardaValor);
            return resultado;
        }

        #region Abrimos el excel de GuardaValores y Extraemos la información de todos los registros previos
        FileStream fs = new(archivoGuardaValor, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (var package = new ExcelPackage())
        {
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets[0];
            int row = 2;

            bool aunInformacion = true;
            while (aunInformacion)
            {
                int col = 1;
                InformacionGuardaValor guardaValor = new();
                int id = FNDExcelHelper.GetCellInt(hoja.Cells[row, col]);
                if (id == 0)
                {
                    //aunInformacion = false;
                    break;
                }
                guardaValor.Id = id;

                col++;
                guardaValor.Region = FNDExcelHelper.GetCellInt(hoja.Cells[row, col]);
                col++;
                guardaValor.Sucursal = FNDExcelHelper.GetCellInt(hoja.Cells[row, col]);
                col++;
                guardaValor.ArchivoOrigenInformacion = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.Auditor = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.Acreditado = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.NumCte = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.NumeroCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.NumProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.CatTipoCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.TipoDeDocumento = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.NumeroDeDocumento = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.Importe = FNDExcelHelper.GetCellDecimal(hoja.Cells[row, col]);
                col++;
                guardaValor.NumeroDeHojas = FNDExcelHelper.GetCellInt(hoja.Cells[row, col]);
                col++;
                guardaValor.FechaDeVencimiento = FNDExcelHelper.GetCellDateTime(hoja.Cells[row, col]);
                col++;
                guardaValor.EstadoFisico = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.UbicacionFisica = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.Estatus = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.CorrectoIncorrecto = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.TipoDeHallazgo = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.MontoDelHallazgo = FNDExcelHelper.GetCellDecimal(hoja.Cells[row, col]);
                col++;
                guardaValor.Detalle = FNDExcelHelper.GetCellString(hoja.Cells[row, col]);
                col++;
                guardaValor.TieneExpedienteDigital = Condiciones.ObtieneTieneExpedienteDigital(FNDExcelHelper.GetCellString(hoja.Cells[row, col])) ?? false;
                col++;
                guardaValor.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(hoja.Cells[row, col]));
                col++;
                guardaValor.EsNuevo = Condiciones.ObtieneEstaEnArqueosSinABSaldo(FNDExcelHelper.GetCellString(hoja.Cells[row, col])) ?? false;
                col++;
                guardaValor.AperturaTiempoDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(hoja.Cells[row, col])) ?? false;
                col++;
                guardaValor.AunEnABSaldos = Condiciones.ObtenerEsCarteraABSaldos(FNDExcelHelper.GetCellString(hoja.Cells[row, col])) ?? false;
                row++;

                resultado.Add(guardaValor);
            }

        }
        #endregion
        return resultado;

    }

    public bool GuardaReporteGuardaValores(IEnumerable<Dominio.Digitalizacion.Entidades.GuardaValores.GuardaValores> guardaValores, string archivoGuardaValor = "")
    {
        if (string.IsNullOrEmpty(archivoGuardaValor))
            archivoGuardaValor = _archivoGuardaValor;
        if (string.IsNullOrEmpty(archivoGuardaValor))
            return false;
        if (File.Exists(archivoGuardaValor))
            File.Delete(archivoGuardaValor);

#pragma warning disable IDE0063 // Use la instrucción "using" simple
        using (FileStream fs = new(archivoGuardaValor ?? "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite)) 
        {
            using (var package = new ExcelPackage(fs))
            {
                ExcelWorksheet? hoja = package.Workbook.Worksheets.Add("GuardaValores");
                hoja.Name = "Documentos Valor";
                #region Escribo los encabezados
                int row = 1;
                string logDirectory = AppDomain.CurrentDomain.BaseDirectory ?? "";
                FNDExcelHelper.AddLogo(hoja, 0, 0, string.Format(@"{0}\Img\LogoFND2.png", logDirectory));
                _logger.LogInformation("Logo agregado {archivoLogo}", string.Format(@"{0}\Img\LogoFND2.png", logDirectory));
                hoja.Row(row).Height = 16.8 + 0.78;
                row++;
                hoja.Row(row).Height = 16.8 + 0.78;
                row++;
                hoja.Row(row).Height = 16.8 + 0.78;
                row++;
                hoja.Row(row).Height = 16.8 + 0.78;
                row++;
                hoja.Row(row).Height = 16.8 + 0.78;
                hoja.Cells[row, 1, row, 9].Merge = true;
                hoja.Cells[row, 1, row, 9].Value = "Documentos Valor Cartera Administrada";
                hoja.Cells[row, 1, row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                hoja.Cells[row, 1, row, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                hoja.Cells[row, 1, row, 9].Style.Font.Name = "Montserrat";
                hoja.Cells[row, 1, row, 9].Style.Font.Size = 11;
                hoja.Cells[row, 1, row, 9].Style.Font.Bold = true;
                hoja.Cells[row, 1, row, 9].Style.Font.Color.SetColor(Color.White);
                FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 1, row, 9], Color.FromArgb(98, 17, 50));
                row++;
                int col = 1;
                hoja.Row(row).Height = 33.6 + 0.78;
                Color titleColor = Color.FromArgb(40, 92, 77);
                PrintTitleCell(hoja, row, col, 9, "Region", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 29.22, "Estado", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 22.44, "Agencia", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 12.22, "Numero de Cliente", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 43.22, "Nombre de Cliente", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 15.33, "Número de Contrato", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 40.44, "Producto", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 11.22, "Estatus de Cartera", titleColor);
                col++;
                PrintTitleCell(hoja, row, col, 17, "Monto", titleColor);
                col++;
                row++;
                #endregion

                #region Vacio los campos
                foreach (var guardaValor in guardaValores)
                {
                    col = 1;
                    hoja.Cells[row, 1, row, 9].Style.Font.Name = "Montserrat";
                    hoja.Cells[row, 1, row, 9].Style.Font.Size = 9;
                    hoja.Cells[row, 1, row, 9].Style.Font.Color.SetColor(Color.Black);

                    hoja.Cells[row, col].Value = guardaValor.CatRegional;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.EstadoInegi;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.CatAgencia;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.NumCte;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.Acreditado;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.NumContrato;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.CatProducto;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.EstadoDeLaCartera;
                    col++;
                    hoja.Cells[row, col].Value = guardaValor.SldoTotContval;
                    hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                    if (row % 2 == 1)
                    {
                        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 1, row, 9], Color.FromArgb(243, 203, 215));
                    }
                    row++;
                }
                #endregion
                #region autofiltro
                hoja.Cells[String.Format("A6:I{0}", row - 1)].AutoFilter = true;

                hoja.Cells[row, 9].Formula = string.Format("=SUBTOTAL(9,I6:I{0})", row - 1);
                hoja.Cells[row, 9].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                row++;
                hoja.View.FreezePanes(7, 1);
                #endregion
                #region Printer Settings
                hoja.PrinterSettings.RepeatRows = hoja.Cells["1:6"];
                hoja.PrinterSettings.PrintArea = hoja.Cells[String.Format("A1:I{0}", row - 1)];
                var footer01 = hoja.HeaderFooter.EvenFooter;
                var footer02 = hoja.HeaderFooter.OddFooter;
                footer01.CenteredText = footer02.CenteredText = "Fecha de Impresión: &D";
                footer01.RightAlignedText = footer02.RightAlignedText = "Página &P de &N";
                hoja.PrinterSettings.Orientation = eOrientation.Landscape;
                
                hoja.PrinterSettings.LeftMargin = 0.6m;
                hoja.PrinterSettings.RightMargin = 0.6m;
                hoja.PrinterSettings.TopMargin = 0.6m;
                hoja.PrinterSettings.BottomMargin = 1.9m;
                hoja.PrinterSettings.FitToWidth = 1;
                #endregion 

                package.Save();
            }
        }
#pragma warning restore IDE0063 // Use la instrucción "using" simple
        return true;
    }

    private static void PrintTitleCell(ExcelWorksheet hoja, int row, int col, double colWidth, string title, Color titleColor)
    {
        hoja.Cells[row, col].Value = title; // Id
        hoja.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        hoja.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
        hoja.Cells[row, col].Style.Font.Bold = true;
        hoja.Cells[row, col].Style.Font.Name = "Montserrat";
        hoja.Cells[row, col].Style.Font.Size = 11;
        hoja.Cells[row, col].Style.Font.Color.SetColor(Color.White);
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], titleColor);
        hoja.Column(col).Width = colWidth + 0.78;
    }
}
