using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Style;
using System.Drawing;
using OfficeOpenXml.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using gob.fnd.ExcelHelper;
using System.Globalization;
using gob.fnd.Dominio.Digitalizacion.Excel;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Excel;
public class ServicioABSaldosRegionales : IServicioABSaldosRegionales
{
    private int _CatalogoRegion = 1 + 1;/// B = 1 (segunda columna)
    private int _Sucursal = 2 + 1; /// C
    private int _CatalogoSucursal = 3 + 1; /// C
    private int _NumeroCliente = 5 + 1; /// C
    private int _ApellidoPaterno = 6 + 1; /// C
    private int _ApellidoMaterno = 7 + 1; /// C
    private int _Nombre1 = 8 + 1; /// C
    private int _Nombre2 = 9 + 1; /// C
    private int _RazonSocial = 10 + 1; /// C
    private int _NumeroCredito = 18 + 1; /// C
    private int _FechaMinistracion = 32 + 1; /// C
    private int _NombreEjecutivo = 88 + 1; /// C
    private int _NumeroProducto = 19 + 1;
    private int _CatalogoProducto = 20 + 1; // U
    private int _TipoCredito = 23 + 1; // T
    private int _CatTipoCartera = 29 + 1;
    private int _MontoMinistracion = 51 + 1; // CF - 2
    private readonly ILogger<ServicioABSaldosRegionales> _logger;
    private readonly string _archivoCreditos;
    private readonly string _archivoCreditosProcesado;
// #pragma warning disable IDE0052 // Quitar miembros privados no leídos
    private readonly IConfiguration _config;
// #pragma warning restore IDE0052 // Quitar miembros privados no leídos

    public ServicioABSaldosRegionales(ILogger<ServicioABSaldosRegionales> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _archivoCreditos = _config.GetValue<string>("archivoCreditos") ??"";
        _archivoCreditosProcesado = _config.GetValue<string>("archivoCreditosProcesado") ?? "";
    }

    private void ObtieneValoresCampos(ExcelWorksheet hoja)
    {
        _CatalogoRegion = BuscaCelda(hoja, _CatalogoRegion, "CAT_REGION");
        _Sucursal = BuscaCelda(hoja, _Sucursal, "SUCURSAL");
        _CatalogoSucursal = BuscaCelda(hoja, _CatalogoSucursal, "CAT_SUCURSAL");
        _NumeroCliente = BuscaCelda(hoja, _NumeroCliente, "NUMCTE");
        _ApellidoPaterno = BuscaCelda(hoja, _ApellidoPaterno, "APELL_PATERNO");
        _ApellidoMaterno = BuscaCelda(hoja, _ApellidoMaterno, "APELL_MATERNO");
        _Nombre1 = BuscaCelda(hoja, _Nombre1, "NOMBRE1");
        _Nombre2 = BuscaCelda(hoja, _Nombre2, "NOMBRE2");
        _RazonSocial = BuscaCelda(hoja, _RazonSocial, "RAZON_SOCIAL");
        _NumeroCredito = BuscaCelda(hoja, _NumeroCredito, "NUM_CREDITO");
        _FechaMinistracion = BuscaCelda(hoja, _FechaMinistracion, "FECHA_MINISTRA");
        _NombreEjecutivo = BuscaCelda(hoja, _NombreEjecutivo, "NOMBRE_EJECUTIVO");
        _NumeroProducto = BuscaCelda(hoja, _NumeroProducto, "num_producto");
        _CatalogoProducto = BuscaCelda(hoja, _CatalogoProducto, "CAT_PRODUCTO");
        _CatTipoCartera = BuscaCelda(hoja, _CatalogoProducto, "CAT_TIPOCARTERA");
        _TipoCredito = BuscaCelda(hoja, _TipoCredito, "CAT_TIPO_CREDITO");
        _MontoMinistracion = BuscaCelda(hoja, _MontoMinistracion, "MTO_MINISTRA_CAP");
    }

    private static int BuscaCelda(ExcelWorksheet hoja, int columnaIncial, string nombre) 
    {
        string nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[1, columnaIncial]).Trim();
        while (!(string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrWhiteSpace(nuevoNombre))) 
        {
            if (string.Equals(nombre, nuevoNombre, StringComparison.InvariantCultureIgnoreCase)) {
                return columnaIncial;
            }
            columnaIncial++;
            nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[1, columnaIncial]).Trim();
        }
        return -1;
    }


    /// <summary>
    /// Carga Procesa y Limpia ABSaldos antes de preparar cualquier otra situación
    /// </summary>
    /// <param name="archivoOrigen">Ubicación donde se localiza el archivo de AB Saldos Original en .ZIP</param>
    /// <param name="archivoDestino">Nuevo archivo ya limpio y depurado</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<ABSaldosRegionales>> CargaProcesaYLimpia(string archivoOrigen="", string archivoDestino="")
    {
        if (string.IsNullOrEmpty(archivoOrigen)) {
            archivoOrigen = _archivoCreditos;
        }
        if (string.IsNullOrEmpty(archivoDestino))
        {
            archivoDestino = _archivoCreditosProcesado;
        }
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        bool resultado = false;
        if (!File.Exists(archivoOrigen))
        {
            _logger.LogError("No se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldos}", archivoOrigen);
            return new List<ABSaldosRegionales>();
        }
        _logger.LogWarning("Procesando el archivo ABSaldos\n{archivoABSaldos}", archivoOrigen);
        IList<ABSaldosRegionales> lista = new List<ABSaldosRegionales>();
        
        #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
        FileStream fs = new(archivoOrigen, FileMode.Open, FileAccess.Read);
        using (var package = new ExcelPackage())
        {
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return lista;
            var hoja = package.Workbook.Worksheets[0];
            ObtieneValoresCampos(hoja);
            int row = 2;
            while (!resultado)
            {
                var obj = new ABSaldosRegionales();
                var celda = hoja.Cells[row, _CatalogoRegion];
                obj.CatalogoRegion = FNDExcelHelper.GetCellString(celda);
                if (string.IsNullOrEmpty(obj.CatalogoRegion))
                    resultado = true;
                obj.Sucursal = FNDExcelHelper.GetCellDouble(hoja.Cells[row, _Sucursal]);
                obj.CatalogoSucursales = FNDExcelHelper.GetCellString(hoja.Cells[row, _CatalogoSucursal]);
                obj.NumeroCliente = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumeroCliente]);
                obj.ApellidoPaterno = FNDExcelHelper.GetCellString(hoja.Cells[row, _ApellidoPaterno]);
                obj.ApellidoMaterno = FNDExcelHelper.GetCellString(hoja.Cells[row, _ApellidoMaterno]);
                obj.PrimerNombre = FNDExcelHelper.GetCellString(hoja.Cells[row, _Nombre1]);
                obj.SegundoNombre = FNDExcelHelper.GetCellString(hoja.Cells[row, _Nombre2]);
                obj.RazonSocial = FNDExcelHelper.GetCellString(hoja.Cells[row, _RazonSocial]);
                obj.NumCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumeroCredito]);
                obj.FechaMinistra = FNDExcelHelper.GetCellDateTime(hoja.Cells[row, _FechaMinistracion]);
                obj.NombreEjecutivo = FNDExcelHelper.GetCellString(hoja.Cells[row, _NombreEjecutivo]);
                obj.NumProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumeroProducto]);
                //obj.CatProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, _CatalogoProducto]);
                obj.TipoCartera = FNDExcelHelper.GetCellString(hoja.Cells[row, _CatTipoCartera]);
                obj.CatTipoDeCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, _TipoCredito]);
                obj.MontoMinistracion = FNDExcelHelper.GetCellDecimal(hoja.Cells[row, _MontoMinistracion]);

                if (!resultado)
                    lista.Add(obj);
                row++;
            }
        }
        #endregion

        #region Actualización de Datos
        if (File.Exists(archivoDestino))
        {
            _logger.LogError("Ya existía el archivo de los nuevos creditos\n{archivoDestino}\n se procede a eliinarlo", archivoDestino);
            File.Delete(archivoDestino);
        }

        FileStream fsO = new(archivoDestino, FileMode.CreateNew, FileAccess.Write);
        using (var package = new ExcelPackage(fsO))
        {
            _logger.LogWarning("Se comienzan a vaciar los nuevos creditos en el archivo destino\n{archivoDestino}\n con un total de {totalCreditosNuevos}", archivoDestino, lista.Count);
            FileInfo fi = new(archivoOrigen);
            string nombreHoja = fi.Name[9..].Replace("202", "2").Replace(" Norte.xlsx", "");
            ExcelWorksheet hoja = package.Workbook.Worksheets.Add(nombreHoja);
            hoja.Name = nombreHoja;

            int row = 1;
            hoja.Cells[row, 1].Value = "CAT_REGION";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 1], Color.FromArgb(179, 142, 93));
            hoja.Column(1).Width = 11 + 0.78;

            hoja.Cells[row, 2].Value = "sucursal";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 2], Color.FromArgb(179, 142, 93));
            hoja.Column(2).Width = 6.78 + 0.78;

            hoja.Cells[row, 3].Value = "CAT_SUCURSAL";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 3], Color.FromArgb(179, 142, 93));
            hoja.Column(3).Width = 20.89 + 0.78;

            hoja.Cells[row, 4].Value = "numcte";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 4], Color.FromArgb(179, 142, 93));
            hoja.Column(4).Width = 12.22 + 0.78;

            hoja.Cells[row, 5].Value = "apell_paterno";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 5], Color.FromArgb(179, 142, 93));
            hoja.Column(5).Width = 18 + 0.78;

            hoja.Cells[row, 6].Value = "apell_materno";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 6], Color.FromArgb(179, 142, 93));
            hoja.Column(6).Width = 18 + 0.78;

            hoja.Cells[row, 7].Value = "nombre1";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 7], Color.FromArgb(179, 142, 93));
            hoja.Column(7).Width = 18 + 0.78;

            hoja.Cells[row, 8].Value = "nombre2";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 8], Color.FromArgb(179, 142, 93));
            hoja.Column(8).Width = 18 + 0.78;

            hoja.Cells[row, 9].Value = "razon_social";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 9], Color.FromArgb(179, 142, 93));
            hoja.Column(9).Width = 47 + 0.78;

            hoja.Cells[row, 10].Value = "num_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 10], Color.FromArgb(179, 142, 93));
            hoja.Column(10).Width = 18.33 + 0.78;

            hoja.Cells[row, 11].Value = "fecha_ministra";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 11], Color.FromArgb(179, 142, 93));
            hoja.Column(11).Width = 12.33 + 0.78;

            hoja.Cells[row, 12].Value = "NOMBRE_EJECUTIVO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 12], Color.FromArgb(179, 142, 93));
            hoja.Column(12).Width = 39 + 0.78;

            hoja.Cells[row, 13].Value = "NOMBRE_CLIENTES";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 13], Color.FromArgb(179, 142, 93));
            hoja.Column(13).Width = 47 + 0.78;

            hoja.Cells[row, 14].Value = "num_producto";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 14], Color.FromArgb(179, 142, 93));
            hoja.Column(14).Width = 12.22 + 0.78;

            hoja.Cells[row, 15].Value = "CAT_TIPOCARTERA";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, 15], Color.FromArgb(179, 142, 93));
            hoja.Column(15).Width = 20.89 + 0.78;


            row++;
            foreach (var ab in lista)
            {
                int celda = 1;
                hoja.Cells[row, celda].Value = ab.CatalogoRegion;
                celda++;
                hoja.Cells[row, celda].Value = ab.Sucursal;
                celda++;
                hoja.Cells[row, celda].Value = ab.CatalogoSucursales;
                celda++;
                hoja.Cells[row, celda].Value = ab.NumeroCliente;
                celda++;
                hoja.Cells[row, celda].Value = ab.ApellidoPaterno;
                celda++;
                hoja.Cells[row, celda].Value = ab.ApellidoMaterno;
                celda++;
                hoja.Cells[row, celda].Value = ab.PrimerNombre;
                celda++;
                hoja.Cells[row, celda].Value = ab.SegundoNombre;
                celda++;
                hoja.Cells[row, celda].Value = ab.RazonSocial;
                celda++;
                hoja.Cells[row, celda].Value = ab.NumCredito;
                celda++;
                if (ab.FechaMinistra.HasValue)
                    FNDExcelHelper.SetCellDate(hoja.Cells[row, celda], ab.FechaMinistra.Value);
                celda++;
                hoja.Cells[row, celda].Value = ab.NombreEjecutivo;
                celda++;
                if (!string.IsNullOrWhiteSpace(ab.RazonSocial))
                    hoja.Cells[row, celda].Value = ab.RazonSocial;
                else
                {
                    string nombreCompleto = ab.PrimerNombre + " " + ab.SegundoNombre + " " + ab.ApellidoPaterno + " " + ab.ApellidoMaterno;
                    hoja.Cells[row, celda].Value = nombreCompleto.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    ab.RazonSocial = nombreCompleto.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                }
                hoja.Cells[row, celda].Value = ab.NumProducto;
                celda++;
                hoja.Cells[row, celda].Value = ab.TipoCartera;
                celda++;
                row++;
            }
            package.Save();
            _logger.LogWarning("Se terminaron de procesar los nuevos creditos en:\n{archivoDestino}\n", archivoDestino);
        }

        #endregion
        // throw new NotImplementedException();

        return lista;
    }
}

