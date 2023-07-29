using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Cancelados;
using gob.fnd.ExcelHelper;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Cancelados;

public class ServicioCancelados : IServicioCancelados
{
    private readonly DateTime _periodoDelDoctor;
    private readonly string _archivoCancelaciones;
    private readonly string _archivoCancelacionesProcesado;
    private readonly ILogger<CreditosCancelados> _logger;
    private readonly IConfiguration _configuration;

    private int iNumCreditoActual = 1;
    private int iNumCreditoNvo = 2;
    private int iNumCreditoOrigen = 3;
    private int iNumCliente = 4;
    private int iGenero = 5;
    private int iTipoPersona = 6;
    private int iPortafolio = 7;
    private int iSesionAutorizacion = 8;
    private int iMesCastigo = 9;
    private int iAnioCastigo = 10;
    private int iGeneracion = 11;
    private int iFechaCancelacion = 12;
    private int iEntidad = 13;
    private int iCoordinacionRegional = 14;
    private int iAgencia = 15;
    private int iAcreditado = 16;
    private int iCartera = 17;
    private int iConcepto = 18;
    private int iCuentaCredito = 19;
    private int iImporteCancelado = 20;
    private int iSaldoContable = 21;
    private int iPrimeraDispersion = 22;
    private int iAnioOrignacion = 23;
    private int iObservacion = 24;
    private int iTipoDeOperacion = 25;
    private int iPiso = 26;

    const string C_STR_NUM_CREDITO_ACTUAL = "num_credito_actual 31enero23";
    const string C_STR_NUM_CREDITO_NVO = "num_credito_nvo";
    const string C_STR_NUM_CREDITO_ORIGEN = "num_credito_orig";
    const string C_STR_NUM_CLIENTE = "numcte";
    const string C_STR_GENERO = "género";
    const string C_STR_TIPO_PERSONA = "tipo_persona";
    const string C_STR_PORTAFOLIO = "Portafolio";
    const string C_STR_SESION_AUTORIZACION = "Sesión de Autorización\nConsejo Directivo/Comité de Crédito";
    const string C_STR_MES_CASTIGO = "Mes_castigo";
    const string C_STR_ANIO_CASTIGO = "año_castigo";
    const string C_STR_GENERACION = "Generacion";
    const string C_STR_FECHA_CANCELACION = "Fecha cancelación";
    const string C_STR_ENTIDAD = "Estado";
    const string C_STR_COORDINACION_REGIONAL = "Coordinación Regional";
    const string C_STR_AGENCIA = "Agencia";
    const string C_STR_ACREDITADO = "Nombre";
    const string C_STR_CARTERA = "Cartera";
    const string C_STR_CONCEPTO = "Concepto";
    const string C_STR_CUENTA_CREDITO = "Cuenta Credito";
    const string C_STR_IMPORTE_CANCELADO = "Importe Cancelado";
    const string C_STR_SALDO_CONTABLE = "Saldo Contable al cierre de enero 2023";
    const string C_STR_PRIMERA_DISPERSION = "Primera Dispersión";
    const string C_STR_ANIO_ORIGINACION = "Año originación";
    const string C_STR_OBSERVACION = "Observacion";
    const string C_STR_TIPO_DE_OPERACION = "Tipo_Operacion";
    const string C_STR_PISO = "Piso Crédito";

    public ServicioCancelados(ILogger<CreditosCancelados> logger, IConfiguration configuration)
    {
        
        _logger = logger;
        _configuration = configuration;
        _archivoCancelaciones = _configuration.GetValue<string>("archivoCancelaciones")??"";
        _archivoCancelacionesProcesado = _configuration.GetValue<string>("archivoCancelacionesProcesado") ?? "";
        string periodoDelDoctor = _configuration.GetValue<string>("periodoDelDoctor") ?? "01/07/2020";
        _periodoDelDoctor = DateTime.ParseExact(periodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture); //??new DateTime(2020,7,1)
    }

    public IEnumerable<CreditosCancelados> GetCreditosCancelados(string archivo = "")
    {
        if (string.IsNullOrWhiteSpace(archivo))
        {
            archivo = _archivoCancelaciones;
        }
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<CreditosCancelados> resultado = new List<CreditosCancelados>();
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
                var obj = new CreditosCancelados();
                var celda = hoja.Cells[row, iNumCreditoActual];
                obj.NumCreditoActual = FNDExcelHelper.GetCellString(celda);
                if (string.IsNullOrEmpty(obj.NumCreditoActual))
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    obj.Id = row - 1;
                    celda = hoja.Cells[row, iNumCreditoNvo];
                    obj.NumCreditoNvo = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCreditoOrigen];
                    obj.NumCreditoOrigen = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCliente];
                    obj.NumCliente = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iGenero];
                    obj.Genero = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTipoPersona];
                    obj.TipoPersona = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iPortafolio];
                    obj.Portafolio = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iSesionAutorizacion];
                    obj.SesionAutorizacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iMesCastigo];
                    obj.MesCastigo = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iAnioCastigo];
                    obj.AnioCastigo = FNDExcelHelper.GetCellInt(celda);
                    celda = hoja.Cells[row, iGeneracion];
                    obj.Generacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFechaCancelacion];
                    obj.FechaCancelacion = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iEntidad];
                    obj.Entidad = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCoordinacionRegional];
                    obj.CoordinacionRegional = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iAgencia];
                    obj.Agencia = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iAcreditado];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCartera];
                    obj.Cartera = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iConcepto];
                    obj.Concepto = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCuentaCredito];
                    obj.CuentaCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iImporteCancelado];
                    obj.ImporteCancelado = Convert.ToDecimal(FNDExcelHelper.GetCellDouble(celda));
                    celda = hoja.Cells[row, iSaldoContable];
                    obj.SaldoContable = Convert.ToDecimal(FNDExcelHelper.GetCellDouble(celda));
                    celda = hoja.Cells[row, iPrimeraDispersion];
                    obj.PrimeraDispersion = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iAnioOrignacion];
                    obj.AnioOrignacion = FNDExcelHelper.GetCellInt(celda);
                    celda = hoja.Cells[row, iObservacion];
                    obj.Observacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTipoDeOperacion];
                    obj.TipoDeOperacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iPiso];
                    obj.Piso = FNDExcelHelper.GetCellInt(celda);

                    // Calculados
                    obj.EsOrigenDelDoctor = (obj.PrimeraDispersion >= _periodoDelDoctor);
                    obj.EsCancelacionDelDoctor = (obj.FechaCancelacion >= _periodoDelDoctor);
                    if (!string.IsNullOrEmpty(obj.NumCreditoOrigen) && obj.NumCreditoOrigen.Length > 17)
                    {
                        string tratamiento = obj.NumCreditoOrigen.Substring(3, 1);
                        obj.EsTratamiento = tratamiento.Equals("8");
                    }

                }

                if (!salDelCiclo)
                {
                    resultado.Add(obj);
                }
                row++;
            }
            
        }
        #endregion

        return resultado.OrderBy(x => x.NumCreditoActual).Select(y => y).ToList();
    }

    private void ObtieneValoresCampos(ExcelWorksheet hoja)
    {
        iNumCreditoActual = BuscaCelda(hoja, iNumCreditoActual, C_STR_NUM_CREDITO_ACTUAL);
        if (iNumCreditoActual <= 0)
        {
            iNumCreditoActual = 1;
            iNumCreditoActual = BuscaCelda(hoja, iNumCreditoActual, "num_credito_actual 31marzo23");
            if (iNumCreditoActual <= 0)
                iNumCreditoActual = 1;
        }
        iNumCreditoNvo = BuscaCelda(hoja, iNumCreditoNvo, C_STR_NUM_CREDITO_NVO);
        iNumCreditoOrigen = BuscaCelda(hoja, iNumCreditoOrigen, C_STR_NUM_CREDITO_ORIGEN);
        iNumCliente = BuscaCelda(hoja, iNumCliente, C_STR_NUM_CLIENTE);
        iGenero = BuscaCelda(hoja, iGenero, C_STR_GENERO);
        iTipoPersona = BuscaCelda(hoja, iTipoPersona, C_STR_TIPO_PERSONA);
        iPortafolio = BuscaCelda(hoja, iPortafolio, C_STR_PORTAFOLIO);
        iSesionAutorizacion = BuscaCelda(hoja, iSesionAutorizacion, C_STR_SESION_AUTORIZACION);
        iMesCastigo = BuscaCelda(hoja, iMesCastigo, C_STR_MES_CASTIGO);
        iAnioCastigo = BuscaCelda(hoja, iAnioCastigo, C_STR_ANIO_CASTIGO);
        iGeneracion = BuscaCelda(hoja, iGeneracion, C_STR_GENERACION);
        iFechaCancelacion = BuscaCelda(hoja, iFechaCancelacion, C_STR_FECHA_CANCELACION);
        iEntidad = BuscaCelda(hoja, iEntidad, C_STR_ENTIDAD);
        iCoordinacionRegional = BuscaCelda(hoja, iCoordinacionRegional, C_STR_COORDINACION_REGIONAL);
        iAgencia = BuscaCelda(hoja, iAgencia, C_STR_AGENCIA);
        iAcreditado = BuscaCelda(hoja, iAcreditado, C_STR_ACREDITADO);
        iCartera = BuscaCelda(hoja, iCartera, C_STR_CARTERA);
        iConcepto = BuscaCelda(hoja, iConcepto, C_STR_CONCEPTO);
        iCuentaCredito = BuscaCelda(hoja, iCuentaCredito, C_STR_CUENTA_CREDITO);
        if (iCuentaCredito <= 0)
        {
            iCuentaCredito = 19;
        }
        iImporteCancelado = BuscaCelda(hoja, iImporteCancelado, C_STR_IMPORTE_CANCELADO);
        iSaldoContable = BuscaCelda(hoja, iSaldoContable, C_STR_SALDO_CONTABLE);
        if (iSaldoContable <= 0)
        {
            iSaldoContable = 21;
            iSaldoContable = BuscaCelda(hoja, iNumCreditoActual, "Saldo Contable al cierre de marzo 2023");
            if (iNumCreditoActual <= 0)
                iNumCreditoActual = 21;
        }
        iPrimeraDispersion = BuscaCelda(hoja, iPrimeraDispersion, C_STR_PRIMERA_DISPERSION);
        iAnioOrignacion = BuscaCelda(hoja, iAnioOrignacion, C_STR_ANIO_ORIGINACION);
        iObservacion = BuscaCelda(hoja, iObservacion, C_STR_OBSERVACION);
        iTipoDeOperacion = BuscaCelda(hoja, iTipoDeOperacion, C_STR_TIPO_DE_OPERACION);
        iPiso = BuscaCelda(hoja, iPiso, C_STR_PISO);
    }

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

    public bool GuardaCreditosCancelados(IEnumerable<CreditosCancelados> listaCreditosCancelados)
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        #region Guarda información
        if (File.Exists(_archivoCancelacionesProcesado))
        {
            _logger.LogError("Ya existía el archivo de los créditos cancelados\n{archivoDestinoCancelados}\n se procede a eliminarlo", _archivoCancelacionesProcesado);
            File.Delete(_archivoCancelacionesProcesado);
        }
        FileStream fsO = new(_archivoCancelacionesProcesado, FileMode.CreateNew, FileAccess.Write);
        using var package = new ExcelPackage(fsO);
        _logger.LogWarning(
            "Se comienzan a vaciar los créditos cancelados en el archivo destino\n{archivoDestinoCancelados}\n con un total de {totalCreditosCancelados}",
            _archivoCancelacionesProcesado,
            listaCreditosCancelados.Count());
        FileInfo fi = new(_archivoCancelacionesProcesado);
        string nombreHoja = "creditos";
        ExcelWorksheet hoja = package.Workbook.Worksheets.Add(nombreHoja);
        hoja.Name = nombreHoja;

        int row = 1;
        int col = 1;

        hoja.Cells[row, col].Value = "num_credito_actual 31enero23";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 33.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "num_credito_nvo";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 21.67 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "num_credito_orig";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 21.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "numcte";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 14.56 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "género";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 11.67 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "tipo_persona";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.56 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Portafolio";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 22.89 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Sesión de Autorización\nConsejo Directivo/Comité de Crédito";
        hoja.Cells[row, col].Style.WrapText = true;
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 47.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Mes_castigo";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "año_castigo";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Generacion";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 15.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Fecha cancelación";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 13.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Estado";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 21.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Coordinación Regional";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 26.67 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Agencia";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 23.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Nombre";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 102.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Cartera";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 17.22 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Concepto";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 19.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Cuenta Credito";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 19.22 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Importe Cancelado";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 23.67 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Saldo Contable al cierre de enero 2023";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 42.11 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Primera Dispersión";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 23.22 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Año originación";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 19.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Observacion";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 25.78 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Tipo_Operacion";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 19.67 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Piso Crédito";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 16.56 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Es Origen Dr";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Es Cancelacion Dr";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Esta en Saldos Castigados";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 9.89 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Esta en Saldos Activos";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Expediente digital";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 10.33 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Castigo";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 9.89 + 0.78;
        col++;

        hoja.Cells[row, col].Value = "Es Tratamiento";
        FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
        hoja.Column(col).Width = 9.89 + 0.78;
        // col++;

        row++;
        foreach (var ab in listaCreditosCancelados)
        {
            int celda = 1;
            hoja.Cells[row, celda].Value = ab.NumCreditoActual;
            celda++;

            hoja.Cells[row, celda].Value = ab.NumCreditoNvo;
            celda++;

            hoja.Cells[row, celda].Value = ab.NumCreditoOrigen;
            celda++;

            hoja.Cells[row, celda].Value = ab.NumCliente;
            celda++;

            hoja.Cells[row, celda].Value = ab.Genero;
            celda++;

            hoja.Cells[row, celda].Value = ab.TipoPersona;
            celda++;

            hoja.Cells[row, celda].Value = ab.Portafolio;
            celda++;

            hoja.Cells[row, celda].Value = ab.SesionAutorizacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.MesCastigo;
            celda++;

            hoja.Cells[row, celda].Value = ab.AnioCastigo;
            celda++;

            hoja.Cells[row, celda].Value = ab.Generacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.FechaCancelacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.Entidad;
            celda++;

            hoja.Cells[row, celda].Value = ab.CoordinacionRegional;
            celda++;

            hoja.Cells[row, celda].Value = ab.Agencia;
            celda++;

            hoja.Cells[row, celda].Value = ab.Acreditado;
            celda++;

            hoja.Cells[row, celda].Value = ab.Cartera;
            celda++;

            hoja.Cells[row, celda].Value = ab.Concepto;
            celda++;

            hoja.Cells[row, celda].Value = ab.CuentaCredito;
            celda++;

            hoja.Cells[row, celda].Value = ab.ImporteCancelado;
            hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            celda++;

            hoja.Cells[row, celda].Value = ab.SaldoContable;
            hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
            celda++;
            if (ab.PrimeraDispersion.HasValue)
                hoja.Cells[row, celda].SetCellDate(ab.PrimeraDispersion.Value);
            celda++;

            hoja.Cells[row, celda].Value = ab.AnioOrignacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.Observacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.TipoDeOperacion;
            celda++;

            hoja.Cells[row, celda].Value = ab.Piso;
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsOrigenDelDoctor);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsCancelacionDelDoctor);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.EsCarteraABSaldos(ab.EstaEnCreditosCastigados);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.EstaEnABSaldosActivo(ab.EstaEnCreditosActivos);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.TieneExpedienteDigital(ab.TieneImagen);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.ObtieneCastigo(ab.Castigo);
            celda++;

            hoja.Cells[row, celda].Value = Condiciones.EsTratamiento(ab.EsTratamiento);
            celda++;
            row++;
        }
        #endregion

        #region autofiltro
        hoja.Cells[String.Format("A1:AG{0}", row - 1)].AutoFilter = true;  // ABCDEFGHIJKLMNO
        hoja.View.FreezePanes(2, 1);
        #endregion

        package.Save();
        return true;
    }

    public async Task<IEnumerable<CreditosCancelados>> GetCreditosCanceladosAsync(string archivo = "")
    {
        if (string.IsNullOrWhiteSpace(archivo))
        {
            archivo = _archivoCancelaciones;
        }
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<CreditosCancelados> resultado = new List<CreditosCancelados>();
        if (!File.Exists(archivo))
        {
            _logger.LogError("No se encontró el Archivo de Ministraciones de la Mesa a procesar\n{archivoMinistracionesMesa}", archivo);
            return resultado;
        }

        #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
        FileStream fs = new(archivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (var package = new ExcelPackage())
        {
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = await Task.Run(() => { return package.Workbook.Worksheets[0]; });
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            await Task.Run(() => {
                while (!salDelCiclo)
                {
                    var obj = new CreditosCancelados();
                    var celda = hoja.Cells[row, iNumCreditoActual];
                    obj.NumCreditoActual = FNDExcelHelper.GetCellString(celda);
                    if (string.IsNullOrEmpty(obj.NumCreditoActual))
                    {
                        salDelCiclo = true;
                        //break;
                    }
                    else
                    {
                        obj.Id = row - 1;
                        celda = hoja.Cells[row, iNumCreditoNvo];
                        obj.NumCreditoNvo = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCreditoOrigen];
                        obj.NumCreditoOrigen = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCliente];
                        obj.NumCliente = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iGenero];
                        obj.Genero = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iTipoPersona];
                        obj.TipoPersona = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iPortafolio];
                        obj.Portafolio = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iSesionAutorizacion];
                        obj.SesionAutorizacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iMesCastigo];
                        obj.MesCastigo = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iAnioCastigo];
                        obj.AnioCastigo = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iGeneracion];
                        obj.Generacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaCancelacion];
                        obj.FechaCancelacion = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iEntidad];
                        obj.Entidad = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCoordinacionRegional];
                        obj.CoordinacionRegional = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iAgencia];
                        obj.Agencia = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iAcreditado];
                        obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCartera];
                        obj.Cartera = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iConcepto];
                        obj.Concepto = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCuentaCredito];
                        obj.CuentaCredito = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iImporteCancelado];
                        obj.ImporteCancelado = Convert.ToDecimal(FNDExcelHelper.GetCellDouble(celda));
                        celda = hoja.Cells[row, iSaldoContable];
                        obj.SaldoContable = Convert.ToDecimal(FNDExcelHelper.GetCellDouble(celda));
                        celda = hoja.Cells[row, iPrimeraDispersion];
                        obj.PrimeraDispersion = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iAnioOrignacion];
                        obj.AnioOrignacion = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iObservacion];
                        obj.Observacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iTipoDeOperacion];
                        obj.TipoDeOperacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iPiso];
                        obj.Piso = FNDExcelHelper.GetCellInt(celda);

                        // Calculados
                        obj.EsOrigenDelDoctor = (obj.PrimeraDispersion >= _periodoDelDoctor);
                        obj.EsCancelacionDelDoctor = (obj.FechaCancelacion >= _periodoDelDoctor);
                        if (!string.IsNullOrEmpty(obj.NumCreditoOrigen) && obj.NumCreditoOrigen.Length > 17)
                        {
                            string tratamiento = obj.NumCreditoOrigen.Substring(3, 1);
                            obj.EsTratamiento = tratamiento.Equals("8");
                        }

                    }

                    if (!salDelCiclo)
                    {
                        resultado.Add(obj);
                    }
                    row++;
                }
            });

        }
        #endregion

        return resultado.OrderBy(x => x.NumCreditoActual).Select(y => y).ToList();
    }
}
