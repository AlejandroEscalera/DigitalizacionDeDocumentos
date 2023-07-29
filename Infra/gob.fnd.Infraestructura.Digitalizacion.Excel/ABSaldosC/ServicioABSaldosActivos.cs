using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.ExcelHelper;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.ABSaldosC;

public class ServicioABSaldosActivos : IServicioABSaldosActivos
{
    private readonly ILogger<ServicioABSaldosActivos> _logger;
    private readonly IConfiguration _configuration;
    private readonly DateTime _periodoDelDoctor;

    private int iRegional = 1;
    private int iNumCte = 2;
    private int iSucursal = 3;
    private int iApell_Paterno = 4;
    private int iApell_Materno = 5;
    private int iNombre1 = 6;
    private int iNombre2 = 7;
    private int iRazonSocial = 9;
    private int iNumCredito = 16;
    private int iNumProducto = 17;
    private int iFechaApertura = 22;
    private int iFechaVencim = 23;
    private int iMontoOtorgado = 41;
    private int iMontoMinistraCap = 42;
    private int iSldoCapCont = 43;
    private int iSldoIntCont = 44;
    private int iSldoTotCont = 45;
    private int iSldoTotContVal = 46;
    private int iNumContrato = 47;
    private int iCapVig = 69;
    private int iIntFinVig = 70;
    private int iIntFinVigNp = 71;
    private int iCapVen = 72;
    private int iIntFinVen = 73;
    private int iIntFinVenNp = 74;
    private int iIntNorVig = 75;
    private int iIntNorVigNp = 76;
    private int iIntNorVen = 77;
    private int iIntNorVenNP = 78;
    private int iIntDesVen = 79;
    private int iIntPen = 80;
    private int iFechaIngUsGaAp = 88;

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
    private void ObtieneValoresCampos(ExcelWorksheet hoja)
    {
        
        iRegional = BuscaCelda(hoja, iRegional, "regional");
        iNumCte = BuscaCelda(hoja, iNumCte, "numcte");
        iSucursal = BuscaCelda(hoja, iSucursal, "sucursal");
        iApell_Paterno = BuscaCelda(hoja, iApell_Paterno, "apell_paterno");
        iApell_Materno = BuscaCelda(hoja, iApell_Materno, "apell_materno");
        iNombre1 = BuscaCelda(hoja, iNombre1, "nombre1");
        iNombre2 = BuscaCelda(hoja, iNombre2, "nombre2");
        iRazonSocial = BuscaCelda(hoja, iRazonSocial, "razon_social");
        iNumCredito = BuscaCelda(hoja, iNumCredito, "num_credito");
        iNumProducto = BuscaCelda(hoja, iNumProducto, "num_producto");
        iFechaApertura = BuscaCelda(hoja, iFechaApertura, "fecha_apertura");

        iFechaVencim = BuscaCelda(hoja, iFechaVencim, "fecha_vencim");
        iMontoOtorgado = BuscaCelda(hoja, iMontoOtorgado, "monto_otorgado");
        iMontoMinistraCap = BuscaCelda(hoja, iMontoMinistraCap, "mto_ministra_cap");
        iSldoCapCont = BuscaCelda(hoja, iSldoCapCont, "sldo_cap_cont");
        iSldoIntCont = BuscaCelda(hoja, iSldoIntCont, "sldo_int_cont");
        iSldoTotCont = BuscaCelda(hoja, iSldoTotCont, "sldo_tot_cont");
        iSldoTotContVal = BuscaCelda(hoja, iSldoTotContVal, "sldo_tot_contval");
        iNumContrato = BuscaCelda(hoja, iNumContrato, "num_contrato");
        iCapVig = BuscaCelda(hoja, iCapVig, "cap_vig");
        iIntFinVig = BuscaCelda(hoja, iIntFinVig, "int_fin_vig");
        iIntFinVigNp = BuscaCelda(hoja, iIntFinVigNp, "int_fin_vig_np");
        iCapVen = BuscaCelda(hoja, iCapVen, "cap_ven");
        iIntFinVen = BuscaCelda(hoja, iIntFinVen, "int_fin_ven");
        iIntFinVenNp = BuscaCelda(hoja, iIntFinVenNp, "int_fin_ven_np");
        iIntNorVig = BuscaCelda(hoja, iIntNorVig, "int_nor_vig");
        iIntNorVigNp = BuscaCelda(hoja, iIntNorVigNp, "int_nor_vig_np");

        iIntNorVen = BuscaCelda(hoja, iIntNorVen, "int_nor_ven");
        iIntNorVenNP = BuscaCelda(hoja, iIntNorVenNP, "int_nor_ven_np");
        iIntDesVen = BuscaCelda(hoja, iIntDesVen, "int_des_ven");
        iIntPen = BuscaCelda(hoja, iIntPen, "int_pen");
        iFechaIngUsGaAp = BuscaCelda(hoja, iFechaIngUsGaAp, "fecha_ingusgaap");
    }

    public IEnumerable<ABSaldosActivos> AgregaAgentesYGuardaValores(IEnumerable<ABSaldosActivos> origen, IEnumerable<CorreosAgencia> agentes)
    {
        foreach (var registro in origen)
        {
            registro.NombreAgente = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.NombreAgente).FirstOrDefault<string?>() ?? "";
            registro.GuardaValores = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.NombreGuardaValores).FirstOrDefault<string?>() ?? "";
            // registro.LigaAgencia = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.LigaAgencia).FirstOrDefault<string?>() ?? "";
        }
        return origen;
    }
    public ServicioABSaldosActivos(ILogger<ServicioABSaldosActivos> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        string periodoDelDoctor = _configuration.GetValue<string>("periodoDelDoctor")??"01/07/2020";
        _periodoDelDoctor = DateTime.ParseExact(periodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture); //??new DateTime(2020,7,1)
    }
    public IEnumerable<ABSaldosActivos> GetABSaldosActivos()
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<ABSaldosActivos> listado = new List<ABSaldosActivos>();
        string archivoABSaldosNatural = _configuration.GetValue<string>("archivoABSaldosNatural") ?? "";
        if (!File.Exists(archivoABSaldosNatural))
        {
            _logger.LogError("No se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldosNatural} Natural", archivoABSaldosNatural);
            return listado;
        }
        FileStream fs = new(archivoABSaldosNatural, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (var package = new ExcelPackage())
        {
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return listado;
            var hoja = package.Workbook.Worksheets[0];
            ObtieneValoresCampos(hoja);

            int row = 2;
            bool resultado = false;
            while (!resultado)
            {
                ABSaldosActivos obj = new()
                {
                    Id = row - 1
                };

                var celda = hoja.Cells[row, iRegional];
                obj.Regional = Convert.ToInt32("0"+celda.Text);
                if (obj.Regional == 0)
                {
                    //resultado = true;
                    break;
                }
                celda = hoja.Cells[row, iNumCte];
                obj.NumCte = FNDExcelHelper.GetCellString(celda);
                
                celda = hoja.Cells[row, iSucursal];
                obj.Sucursal = Convert.ToInt32("0" + celda.Text);

                celda = hoja.Cells[row, iApell_Paterno];
                obj.Apell_Paterno = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iApell_Materno];
                obj.Apell_Materno = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iNombre1];
                obj.Nombre1 = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iNombre2];
                obj.Nombre2 = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iRazonSocial];
                obj.RazonSocial = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iNumCredito];
                obj.NumCredito = FNDExcelHelper.GetCellString(celda);

                if (!string.IsNullOrEmpty(obj.NumCredito) && obj.NumCredito.Length > 17)
                {
                    string tratamiento = obj.NumCredito.Substring(3, 1);
                    obj.EsTratamiento = tratamiento.Equals("8");
                }

                celda = hoja.Cells[row, iNumProducto];
                obj.NumProducto = FNDExcelHelper.GetCellString(celda);

                /// Es un string con formato mes día año                    
                celda = hoja.Cells[row, iFechaApertura];
                string stringTextDate = celda.Text;
                if (!string.IsNullOrEmpty(stringTextDate))
                {
                    try
                    {
                        obj.FechaApertura = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch { }
                }

                celda = hoja.Cells[row, iFechaVencim];
                // obj.FechaVencim = FNDExcelHelper.GetCellDateTime(celda);
                stringTextDate = celda.Text;
                if (!string.IsNullOrEmpty(stringTextDate))
                {
                    try
                    {
                        obj.FechaVencim = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch { }
                }

                celda = hoja.Cells[row, iMontoOtorgado];
                obj.MontoOtorgado = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iMontoMinistraCap];
                obj.MontoMinistraCap = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iSldoCapCont];
                obj.SldoCapCont = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iSldoIntCont];
                obj.SldoIntCont = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iSldoTotCont];
                obj.SldoTotCont = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iSldoTotContVal];
                obj.SldoTotContVal = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iNumContrato];
                obj.NumContrato = FNDExcelHelper.GetCellString(celda);

                celda = hoja.Cells[row, iCapVig];
                obj.CapVig = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntFinVig];
                obj.IntFinVig = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntFinVigNp];
                obj.IntFinVigNp = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iCapVen];
                obj.CapVen = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntFinVen];
                obj.IntFinVen = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntFinVenNp];
                obj.IntFinVenNp = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntNorVig];
                obj.IntNorVig = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntNorVigNp];
                obj.IntNorVigNp = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntNorVen];
                obj.IntNorVen = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntNorVenNP];
                obj.IntNorVenNP = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntDesVen];
                obj.IntDesVen = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iIntPen];
                obj.IntPen = Convert.ToDecimal(celda.Value);

                celda = hoja.Cells[row, iFechaIngUsGaAp];
                stringTextDate = celda.Text;
                if (!string.IsNullOrEmpty(stringTextDate))
                {
                    try
                    {
                        obj.FechaIngUsGaAp = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch { }
                }
                //obj.FechaIngUsGaAp = FNDExcelHelper.GetCellDateTime(celda);

                if (!string.IsNullOrWhiteSpace(obj.RazonSocial))
                    obj.Acreditado = obj.RazonSocial;
                else { 
                    obj.Acreditado = obj.Nombre1 + " "+ obj.Nombre2 + " " + obj.Apell_Paterno + " " + obj.Apell_Materno;
                }
                obj.EsOrigenDelDoctor = (obj.FechaApertura >= _periodoDelDoctor);

                obj.Acreditado = obj.Acreditado.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("Ð", "Ñ");
                if (!string.IsNullOrEmpty(obj.NumCredito))
                    listado.Add(obj);
                row++;
            }
        }
        return listado;
    }

    public bool GuardaABSaldosActivos(IEnumerable<ABSaldosActivos> lista)
    {
        string archivoABSaldosNaturalProcesado = _configuration.GetValue<string>("archivoABSaldosNaturalProcesado") ?? "";
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        if (File.Exists(archivoABSaldosNaturalProcesado))
        {
            try
            { File.Delete(archivoABSaldosNaturalProcesado); }
            catch { return false; }
        }
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        FileStream fs = new(archivoABSaldosNaturalProcesado ?? "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var package = new ExcelPackage(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.Add("ABSaldos");
        hoja.Name = "ABSaldos";

        var listadoOrdenado = lista.OrderBy(x => x.NumCredito).ToList();
        GuardaHojaConInformacion(hoja, listadoOrdenado);

        /* Los duplicados eran los que los datos del credito estaban en blanco
        hoja = package.Workbook.Worksheets.Add("Duplicados");
        hoja.Name = "Duplicados";

        var duplicados = listadoOrdenado.GroupBy(p=> p.NumCredito).Where(g=>g.Count()>1).SelectMany(g=>g).ToList();
        GuardaHojaConInformacion(hoja, duplicados);
        */
        package.Save();
        return true;
    }

    private static void GuardaHojaConInformacion(ExcelWorksheet hoja, List<ABSaldosActivos> listadoOrdenado)
    {
        #region Encabezado del archivo
        int row = 1;
        int col = 1;
        hoja.Cells[row, col].Value = "#"; // Id
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 11.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "REGIONAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 3 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "NUM_CTE"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 12.22 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "SUCURSAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 3.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "ACREDITADO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 50 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "APELL_PATERNO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; // 17.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "APELL_MATERNO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; // 17.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NOMBRE1"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; // 17.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NOMBRE2"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; // 17.11 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "RAZONSOCIAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 0; //  45.78 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUM_CREDITO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "ES_TRATAMIENTO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "NUM_PRODUCTO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 4.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "FECHA_APERTURA"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "FECHA_VENCIM"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "MONTO_OTORGADO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "MONTO_MINISTRA_CAP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "SLDO_CAP_CONT"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "SLDO_INT_CONT"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "SLDO_TOT_CONT"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "SLDO_TOT_CONT_VAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUM_CONTRATO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15.33 + 0.78;
        col++;


        hoja.Cells[row, col].Value = "CAP_VIG"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_FIN_VIG"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_FIN_VIG_NP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "CAP_VEN"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "INT_FIN_VEN"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "INT_FIN_VEN_NP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "INT_NOR_VIG"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_NOR_VIG_NP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_NOR_VEN"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_NOR_VEN_NP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_DES_VEN"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "INT_PEN"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "FECHA_ING_US_GA_AP"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "CASTIGO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 9.89 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "ES_ORIGEN_DEL_DOCTOR"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "ES_CANCELADO_DEL_DOCTOR"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "ESTA_ABSALDOS_CASTIGO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "EXPEDIENTE_DIGITAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NO_PDF"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 5.22 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "CUENTA_CON_GUARDA_VALOR"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "CAT_PRODUCTO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 75.11 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "TIPO_CARTERA"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 28.33 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "FECHA_MINISTRACION"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "AGENTE"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 40.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "GUARDA VALOR"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 40.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "EJECUTIVO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 40.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "CAT_REGION";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "CAT_SUCURSAL";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 23.22 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "NUM_CREDITO_CANCEL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "NUM_CREDITO_ORIGINAL"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18.33 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Sesión de Autorización"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 52.78 + 0.78;
        col++;
        hoja.Cells[row, col].Value = "Mes de castigo"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 18 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "Año de castigo"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 8.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "Generación"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 12.22 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "FECHA_CANCELACION"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "Año de Originación"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 6.56 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "Esta en cancelados"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 11 + 0.78;


        col++;
        hoja.Cells[row, col].Value = "NO_MINISTRA"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 7.44 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "Clasif. Mesa"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 60.22 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "FECHA_ING_MESA"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "MONTO_OTORGADO"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 17.22 + 0.78;

        col++;
        hoja.Cells[row, col].Value = "ESTA_EN_MESA"; // Id
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 5.56 + 0.78;

        row++;
        #endregion

        #region Detalle
        foreach (var saldo in listadoOrdenado)
        {
            col = 1;
            hoja.Cells[row, col].Value = saldo.Id;
            col++;
            hoja.Cells[row, col].Value = saldo.Regional;
            col++;
            hoja.Cells[row, col].Value = saldo.NumCte;
            col++;
            hoja.Cells[row, col].Value = saldo.Sucursal;
            col++;
            hoja.Cells[row, col].Value = saldo.Acreditado;
            col++;
            hoja.Cells[row, col].Value = saldo.Apell_Paterno;
            col++;
            hoja.Cells[row, col].Value = saldo.Apell_Materno;
            col++;
            hoja.Cells[row, col].Value = saldo.Nombre1;
            col++;
            hoja.Cells[row, col].Value = saldo.Nombre2;
            col++;
            hoja.Cells[row, col].Value = saldo.RazonSocial;
            col++;
            hoja.Cells[row, col].Value = saldo.NumCredito;
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsTratamiento(saldo.EsTratamiento);
            col++;
            hoja.Cells[row, col].Value = saldo.NumProducto;
            col++;
            if (saldo.FechaApertura is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaApertura.Value);
            col++;
            if (saldo.FechaVencim is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaVencim.Value);
            col++;
            hoja.Cells[row, col].Value = saldo.MontoOtorgado;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.MontoMinistraCap;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.SldoCapCont;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.SldoIntCont;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.SldoTotCont;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.SldoTotContVal;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.NumContrato;
            col++;
            hoja.Cells[row, col].Value = saldo.CapVig;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntFinVig;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntFinVigNp;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.CapVen;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntFinVen;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntFinVenNp;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntNorVig;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntNorVigNp;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntNorVen;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntNorVenNP;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntDesVen;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = saldo.IntPen;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            if (saldo.FechaIngUsGaAp is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaIngUsGaAp.Value);
            col++;
            hoja.Cells[row, col].Value = Condiciones.ObtieneCastigo(saldo.Castigo);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsPeriodoDelDr(saldo.EsOrigenDelDoctor);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsPeriodoDelDr(saldo.EsCancelacionDelDoctor);
            col++;
            hoja.Cells[row, col].Value = Condiciones.EsCarteraABSaldos(saldo.EsCarteraActiva);
            col++;
            hoja.Cells[row, col].Value = Condiciones.TieneExpedienteDigital(saldo.ExpedienteDigital);
            col++;
            hoja.Cells[row, col].Value = saldo.NoPDF;
            col++;
            hoja.Cells[row, col].Value = Condiciones.TieneGuardaValores(saldo.CuentaConGuardaValores);
            col++;
            hoja.Cells[row, col].Value = saldo.CatProducto;
            col++;
            hoja.Cells[row, col].Value = saldo.TipoCartera;
            col++;
            if (saldo.FechaInicioMinistracion is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaInicioMinistracion.Value);
            col++;
            hoja.Cells[row, col].Value = saldo.NombreAgente;
            col++;
            hoja.Cells[row, col].Value = saldo.GuardaValores;
            col++;
            hoja.Cells[row, col].Value = saldo.NombreEjecutivo;
            col++;
            hoja.Cells[row, col].Value = saldo.CatRegion;
            col++;
            hoja.Cells[row, col].Value = saldo.CatSucursal;
            col++;

            hoja.Cells[row, col].Value = saldo.NumCreditoCancelado;
            col++;
            hoja.Cells[row, col].Value = saldo.NumCreditoOriginal;
            col++;
            hoja.Cells[row, col].Value = saldo.SesionDeAutorizacion;
            col++;
            hoja.Cells[row, col].Value = saldo.MesCastigo;
            col++;
            hoja.Cells[row, col].Value = saldo.AnioCastigo;
            col++;
            hoja.Cells[row, col].Value = saldo.Generacion;
            col++;
            if (saldo.FechaCancelacion is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaCancelacion.Value);
            col++;
            
            hoja.Cells[row, col].Value = saldo.AnioOriginacion;
            col++;
            hoja.Cells[row, col].Value = Condiciones.EstaEnCancelado(saldo.EstaEnCreditosCancelados);
            col++;

            hoja.Cells[row, col].Value = saldo.NumeroMinistracion;
            col++;
            hoja.Cells[row, col].Value = saldo.ClasificacionMesa;
            col++;
            if (saldo.FechaDeSolicitud is not null)
                hoja.Cells[row, col].SetCellDate(saldo.FechaDeSolicitud.Value);
            col++;
            hoja.Cells[row, col].Value = saldo.MontoOtorgado;
            hoja.Cells[row, col].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            col++;
            hoja.Cells[row, col].Value = Condiciones.EstaEnMesa(saldo.EstaEnMesa);
            // col++;
            row++;

        }
        #endregion

        #region autofiltro
        hoja.Cells[String.Format("A1:BL{0}", row - 1)].AutoFilter = true; // NOPQRSTUV WXYZABCDE FGHIJK
        hoja.View.FreezePanes(2, 1);
        #endregion

    }

    public IEnumerable<ABSaldosActivos> ObtieneABSaldosActivosProcesados()
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<ABSaldosActivos> listado = new List<ABSaldosActivos>();
        string archivoABSaldosNatural = _configuration.GetValue<string>("archivoABSaldosNatural") ?? "";
        if (!File.Exists(archivoABSaldosNatural))
        {
            _logger.LogError("No se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldosNatural} Natural", archivoABSaldosNatural);
            return listado;
        }
        FileStream fs = new(archivoABSaldosNatural, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (var package = new ExcelPackage())
        {
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return listado;
            var hoja = package.Workbook.Worksheets[0];
            ObtieneValoresCampos(hoja);

            int row = 2;
            int col;
            bool resultado = false;
            while (!resultado)
            {
                ABSaldosActivos obj = new();

                col = 1;
                var celda = hoja.Cells[row, col];
                obj.Id = FNDExcelHelper.GetCellInt(celda);
                col++;
                if (obj.Id == 0)
                {
                    //resultado = true;
                    break;
                }

                celda = hoja.Cells[row, col];
                obj.Regional = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NumCte = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Sucursal = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Apell_Paterno = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Apell_Materno = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Nombre1 = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Nombre2 = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.RazonSocial = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.EsTratamiento = Condiciones.ObtieneEsTratamiento(FNDExcelHelper.GetCellString(celda))??false;
                col++;

                celda = hoja.Cells[row, col];
                obj.NumProducto = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaVencim = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.MontoMinistraCap = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.SldoCapCont = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.SldoIntCont = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.SldoTotCont = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.SldoTotContVal = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NumContrato = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.CapVig = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntFinVig = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntFinVigNp = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.CapVen = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntFinVen = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntFinVenNp = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntNorVig = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntNorVigNp = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntNorVen = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntNorVenNP = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntDesVen = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.IntPen = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaIngUsGaAp = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(celda));
                col++;

                celda = hoja.Cells[row, col];
                obj.EsOrigenDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda))??false;
                col++;

                celda = hoja.Cells[row, col];
                obj.EsCancelacionDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
                col++;

                celda = hoja.Cells[row, col];
                obj.EsCarteraActiva = Condiciones.ObtenerEsCarteraABSaldos(FNDExcelHelper.GetCellString(celda)) ?? false;
                col++;

                celda = hoja.Cells[row, col];
                obj.ExpedienteDigital = Condiciones.ObtieneTieneExpedienteDigital(FNDExcelHelper.GetCellString(celda)) ?? false;
                col++;

                celda = hoja.Cells[row, col];
                obj.NoPDF = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.CuentaConGuardaValores = Condiciones.ObtieneTieneGuardaValores(FNDExcelHelper.GetCellString(celda)) ?? false;
                col++;

                celda = hoja.Cells[row, col];
                obj.CatProducto = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.TipoCartera = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaInicioMinistracion = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NombreAgente = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.GuardaValores = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NombreEjecutivo = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.CatSucursal = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NumCreditoCancelado = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.NumCreditoOriginal = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.SesionDeAutorizacion = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.MesCastigo = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.AnioCastigo = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.Generacion = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaCancelacion = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.AnioOriginacion = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.EstaEnCreditosCancelados = Condiciones.ObtieneEstaEnCancelado(FNDExcelHelper.GetCellString(celda)) ?? false;
                col++;

                celda = hoja.Cells[row, col];
                obj.NumeroMinistracion = FNDExcelHelper.GetCellInt(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.ClasificacionMesa = FNDExcelHelper.GetCellString(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.FechaDeSolicitud = FNDExcelHelper.GetCellDateTime(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                col++;

                celda = hoja.Cells[row, col];
                obj.EstaEnMesa = Condiciones.ObtieneEstaEnMesa(FNDExcelHelper.GetCellString(celda)) ?? false;
                //col++;

                listado.Add(obj);
                row++;
            }
        }
        return listado;
    }

    public async Task<IEnumerable<ABSaldosActivos>> GetABSaldosActivosAsync()
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<ABSaldosActivos> listado = new List<ABSaldosActivos>();
        string archivoABSaldosNatural = _configuration.GetValue<string>("archivoABSaldosNatural") ?? "";
        if (!File.Exists(archivoABSaldosNatural))
        {
            _logger.LogError("No se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldosNatural} Natural", archivoABSaldosNatural);
            return listado;
        }
        FileStream fs = new(archivoABSaldosNatural, FileMode.Open, FileAccess.Read, FileShare.Read);
#pragma warning disable IDE0063 // Use la instrucción "using" simple
        using (var package = new ExcelPackage())
        {
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return listado;
            var hoja = await Task.Run(() => { return package.Workbook.Worksheets[0]; });

            ObtieneValoresCampos(hoja);

            int row = 2;
            bool resultado = false;
            await Task.Run(() =>
            {
                while (!resultado)
                {
                    ABSaldosActivos obj = new()
                    {
                        Id = row - 1
                    };

                    var celda = hoja.Cells[row, iRegional];
                    obj.Regional = Convert.ToInt32("0" + celda.Text);
                    if (obj.Regional == 0)
                    {
                        //resultado = true;
                        break;
                    }
                    celda = hoja.Cells[row, iNumCte];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iSucursal];
                    obj.Sucursal = Convert.ToInt32("0" + celda.Text);

                    celda = hoja.Cells[row, iApell_Paterno];
                    obj.Apell_Paterno = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iApell_Materno];
                    obj.Apell_Materno = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iNombre1];
                    obj.Nombre1 = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iNombre2];
                    obj.Nombre2 = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iRazonSocial];
                    obj.RazonSocial = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iNumCredito];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);

                    if (!string.IsNullOrEmpty(obj.NumCredito) && obj.NumCredito.Length > 17)
                    {
                        string tratamiento = obj.NumCredito.Substring(3, 1);
                        obj.EsTratamiento = tratamiento.Equals("8");
                    }

                    celda = hoja.Cells[row, iNumProducto];
                    obj.NumProducto = FNDExcelHelper.GetCellString(celda);

                    /// Es un string con formato mes día año                    
                    celda = hoja.Cells[row, iFechaApertura];
                    string stringTextDate = celda.Text;
                    if (!string.IsNullOrEmpty(stringTextDate))
                    {
                        try
                        {
                            obj.FechaApertura = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch { }
                    }

                    celda = hoja.Cells[row, iFechaVencim];
                    // obj.FechaVencim = FNDExcelHelper.GetCellDateTime(celda);
                    stringTextDate = celda.Text;
                    if (!string.IsNullOrEmpty(stringTextDate))
                    {
                        try
                        {
                            obj.FechaVencim = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch { }
                    }

                    celda = hoja.Cells[row, iMontoOtorgado];
                    obj.MontoOtorgado = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iMontoMinistraCap];
                    obj.MontoMinistraCap = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iSldoCapCont];
                    obj.SldoCapCont = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iSldoIntCont];
                    obj.SldoIntCont = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iSldoTotCont];
                    obj.SldoTotCont = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iSldoTotContVal];
                    obj.SldoTotContVal = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iNumContrato];
                    obj.NumContrato = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iCapVig];
                    obj.CapVig = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntFinVig];
                    obj.IntFinVig = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntFinVigNp];
                    obj.IntFinVigNp = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iCapVen];
                    obj.CapVen = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntFinVen];
                    obj.IntFinVen = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntFinVenNp];
                    obj.IntFinVenNp = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntNorVig];
                    obj.IntNorVig = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntNorVigNp];
                    obj.IntNorVigNp = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntNorVen];
                    obj.IntNorVen = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntNorVenNP];
                    obj.IntNorVenNP = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntDesVen];
                    obj.IntDesVen = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iIntPen];
                    obj.IntPen = Convert.ToDecimal(celda.Value);

                    celda = hoja.Cells[row, iFechaIngUsGaAp];
                    stringTextDate = celda.Text;
                    if (!string.IsNullOrEmpty(stringTextDate))
                    {
                        try
                        {
                            obj.FechaIngUsGaAp = DateTime.ParseExact(stringTextDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch { }
                    }
                    //obj.FechaIngUsGaAp = FNDExcelHelper.GetCellDateTime(celda);

                    if (!string.IsNullOrWhiteSpace(obj.RazonSocial))
                        obj.Acreditado = obj.RazonSocial;
                    else
                    {
                        obj.Acreditado = obj.Nombre1 + " " + obj.Nombre2 + " " + obj.Apell_Paterno + " " + obj.Apell_Materno;
                    }
                    obj.EsOrigenDelDoctor = (obj.FechaApertura >= _periodoDelDoctor);

                    obj.Acreditado = obj.Acreditado.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("Ð", "Ñ");
                    if (!string.IsNullOrEmpty(obj.NumCredito))
                        listado.Add(obj);
                    row++;
                };
            }
            );
            return listado;
        }
#pragma warning restore IDE0063 // Use la instrucción "using" simple
    }
}
