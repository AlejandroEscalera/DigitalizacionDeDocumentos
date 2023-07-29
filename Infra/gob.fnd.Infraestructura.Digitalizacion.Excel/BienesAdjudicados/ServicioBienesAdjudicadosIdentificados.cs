using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.BienesAdjudicados;
public class ServicioBienesAdjudicadosIdentificados : IBienesAdjudicadosIdentificados
{
    private int iCveBienI = 1;
    private int iCrI = 2;
    private int iAcreditadoI = 3;
    private int iTipoBienI = 4;
    private int iNumCreditoI = 5;
    private int iObservacionesI = 6;

    const string C_CVEBIEN_I = "Clave del Bien";
    const string C_CR_I = "CR";
    const string C_ACREDITADO_I = "Acreditado(s)";
    const string C_TIPOBIEN_I = "Tipo bien";
    const string C_NUMCREDITO_I = "Número de crédito";
    const string C_OBSERVACIONES_I = "OBSERVACIONES DE CONCILIACIÓN";
    private readonly ILogger<ServicioBienesAdjudicados> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _archivoIdentificacionClaveBien;

    public ServicioBienesAdjudicadosIdentificados(ILogger<ServicioBienesAdjudicados> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _archivoIdentificacionClaveBien = _configuration.GetValue<string>("archivoIdentificacionClaveBien") ?? "";
    }

    private static int BuscaCelda(ExcelWorksheet hoja, int columnaIncial, string nombre)
    {
        string nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[2, columnaIncial]).Trim();
        while (!(string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrWhiteSpace(nuevoNombre)))
        {
            if (string.Equals(nombre, nuevoNombre, StringComparison.InvariantCultureIgnoreCase))
            {
                return columnaIncial;
            }
            columnaIncial++;
            nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[3, columnaIncial]).Trim();
        }
        return -1;
    }

    private void ObtieneValoresCampos(ExcelWorksheet hoja)
    {
        iCveBienI = BuscaCelda(hoja, iCveBienI, C_CVEBIEN_I);
        iCrI = BuscaCelda(hoja, iCrI, C_CR_I);
        iAcreditadoI = BuscaCelda(hoja,iAcreditadoI, C_ACREDITADO_I);
        iTipoBienI = BuscaCelda(hoja, iTipoBienI, C_TIPOBIEN_I);
        iNumCreditoI = BuscaCelda(hoja,iNumCreditoI, C_NUMCREDITO_I);
        iObservacionesI = BuscaCelda(hoja, iObservacionesI, C_OBSERVACIONES_I);
    }

    public IEnumerable<IdentificacionClaveBien> ObtieneFuenteBienesAdjudicadosIdentificacion(string archivo = "")
    {
        if (string.IsNullOrWhiteSpace(archivo))
        {
            archivo = _archivoIdentificacionClaveBien;
        }
        // Al ser un organismo de gobierno, y al ser open source, no tiene uso comercial
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        IList<IdentificacionClaveBien> resultado = new List<IdentificacionClaveBien>();
        if (!File.Exists(archivo))
        {
            _logger.LogInformation("No se encontró el archivo de Identificación de Bienes Adjudicados\n{nombreArchivo}", archivo);
            return resultado;
        }

        #region Abrimos el Fuente de Bienes Adjudicados Identificación y extraemos la información de todos los campos
        FileStream fs = new(archivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var package = new ExcelPackage();
        package.Load(fs);
        if (package.Workbook.Worksheets.Count == 0)
            return resultado;
        var hoja = package.Workbook.Worksheets.Where(x=>x.Name.Contains("Asociación claves - crédito")).FirstOrDefault();
        if (hoja is null) 
            return resultado;
        ObtieneValoresCampos(hoja);
        int row = 3;
        bool salDelCiclo = false;
        while(!salDelCiclo)
        {
            var obj = new IdentificacionClaveBien();
            var celda = hoja.Cells[row, iCveBienI];
            obj.CveBienI = FNDExcelHelper.GetCellString(celda);
            if (string.IsNullOrEmpty(obj.CveBienI))
            {
                salDelCiclo = true;
                break;
            }
            else 
            { 
                celda = hoja.Cells[row, iCveBienI];
                obj.CveBienI = FNDExcelHelper.GetCellString(celda);
                celda = hoja.Cells[row, iCrI];
                obj.CrI = FNDExcelHelper.GetCellString(celda);
                celda = hoja.Cells[row, iAcreditadoI];
                obj.AcreditadoI = FNDExcelHelper.GetCellString(celda);
                celda = hoja.Cells[row, iTipoBienI];
                obj.TipoBienI = FNDExcelHelper.GetCellString(celda);
                celda = hoja.Cells[row, iNumCreditoI];
                obj.NumCreditoI = FNDExcelHelper.GetCellString(celda);
                celda = hoja.Cells[row, iObservacionesI];
                obj.ObservacionesI = FNDExcelHelper.GetCellString(celda);
            }
            if (!salDelCiclo)
            {
                resultado.Add(obj);
            }
            row++;
        }
        #endregion
        return resultado.ToList();
    }
}
