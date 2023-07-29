using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Excel.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Liquidaciones
{
    public class LiquidacionesService : ILiquidaciones
    {
        private readonly ILogger<LiquidacionesService> _logger;
        private readonly IConfiguration _configuration;
        private int iSucursal = 1;
        private int iAgencia = 2;
        private int iNumCte = 3;
        private int iNombreCliente = 4;
        private int iNumCredito = 5;
        private int iFechaApertura = 6;
        private int iFechaVencim = 7;
        private int iMontoOtorgado = 8;
        private int iCatRegion = 9;
        private int iCatEstado = 10;
        private int iCatAgencia = 11;
        private int iMinistraciones = 12;
        private int iFecPrimMinistra = 13;
        private int iFecUltimaMinistra = 14;
        private int iMontoMinistrado = 15;
        private int iReestructura = 16;
        private int iCancelacion = 17;
        private int iPagoCapital = 18;
        private int iPagoInteres = 19;
        private int iPagoMoratorios = 20;
        private int iPagoTotal = 21;

        const string C_SUCURSAL = "sucursal";
        const string C_AGENCIA = "agencia";
        const string C_NUM_CTE = "numcte";
        const string C_NOMBRE_CLIENTE = "nombre_cliente";
        const string C_NUM_CREDITO = "num_credito";
        const string C_FECHA_APERTURA = "fecha_apertura";
        const string C_FECHA_VENCIM = "fecha_vencim";
        const string C_MONTO_OTORGADO = "monto_otorgado";
        const string C_CAT_REGION = "CAT_REGION";
        const string C_CAT_ESTADO = "CAT_ESTADO";
        const string C_CAT_AGENCIA = "CAT_AGENCIA";
        const string C_MINISTRACIONES = "Ministraciones";
        const string C_FEC_PRIM_MINISTRA = "Fec_prim_ministra";
        const string C_FEC_ULTIMA_MINISTRA = "Fec_ultima_ministra";
        const string C_MONTO_MINISTRADO = "Monto_Ministrado";
        const string C_REESTRUCTURA = "Reestructura";
        const string C_CANCELACION = "Cancelacion";
        const string C_PAGO_CAPITAL = "Pago_Capital";
        const string C_PAGO_INTERES = "Pago_Interes";
        const string C_PAGO_MORATORIOS = "Pago_Moratorios";
        const string C_PAGO_TOTAL = "Pago_Total";

        private readonly string _archivoLiquidacionesOrigen;

        public LiquidacionesService(ILogger<LiquidacionesService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoLiquidacionesOrigen = _configuration.GetValue<string>("archivoLiquidacionesOrigen") ?? "";
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

        private void ObtieneValoresCampos(ExcelWorksheet hoja)
        {
            iSucursal = BuscaCelda(hoja, iSucursal, C_SUCURSAL);
            iAgencia = BuscaCelda(hoja, iAgencia, C_AGENCIA);
            iNumCte = BuscaCelda(hoja, iNumCte, C_NUM_CTE);
            iNombreCliente = BuscaCelda(hoja, iNombreCliente, C_NOMBRE_CLIENTE);
            iNumCredito = BuscaCelda(hoja, iNumCredito, C_NUM_CREDITO);
            iFechaApertura = BuscaCelda(hoja, iFechaApertura, C_FECHA_APERTURA);
            iFechaVencim = BuscaCelda(hoja, iFechaVencim, C_FECHA_VENCIM);
            iMontoOtorgado = BuscaCelda(hoja, iMontoOtorgado, C_MONTO_OTORGADO);
            iCatRegion = BuscaCelda(hoja, iCatRegion, C_CAT_REGION);
            iCatEstado = BuscaCelda(hoja, iCatEstado, C_CAT_ESTADO);
            iCatAgencia = BuscaCelda(hoja, iCatAgencia, C_CAT_AGENCIA);
            iMinistraciones = BuscaCelda(hoja, iMinistraciones, C_MINISTRACIONES);
            iFecPrimMinistra = BuscaCelda(hoja, iFecPrimMinistra, C_FEC_PRIM_MINISTRA);
            iFecUltimaMinistra = BuscaCelda(hoja, iFecUltimaMinistra, C_FEC_ULTIMA_MINISTRA);
            iMontoMinistrado = BuscaCelda(hoja, iMontoMinistrado, C_MONTO_MINISTRADO);
            iReestructura = BuscaCelda(hoja, iReestructura, C_REESTRUCTURA);
            iCancelacion = BuscaCelda(hoja, iCancelacion, C_CANCELACION);
            iPagoCapital = BuscaCelda(hoja, iPagoCapital, C_PAGO_CAPITAL);
            iPagoInteres = BuscaCelda(hoja, iPagoInteres, C_PAGO_INTERES);
            iPagoMoratorios = BuscaCelda(hoja, iPagoMoratorios, C_PAGO_MORATORIOS);
            iPagoTotal = BuscaCelda(hoja, iPagoTotal, C_PAGO_TOTAL);
        }
        public IEnumerable<ColocacionConPagos> GetColocacionConPagos(string archivoLiquidaciones)
        {
            IList<ColocacionConPagos> resultado = new List<ColocacionConPagos>();
            _logger.LogInformation("Comenzamos a leer la colocación con pagos (Liquidados)");

            if (string.IsNullOrWhiteSpace(archivoLiquidaciones))
            {
                archivoLiquidaciones = _archivoLiquidacionesOrigen;
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoLiquidaciones))
            {
                _logger.LogError("No se encontró el Archivo de Colocación con Pagos\n{archivoColocacionConPagos}", archivoLiquidaciones);
                return resultado;
            }
            // TODO: Hacer el trabajo

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoLiquidaciones, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            // await package.LoadAsync(fs);
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets["historico_colocacion_con_pagos"];
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            while (!salDelCiclo)
            {
                var obj = new ColocacionConPagos();
                var celda = hoja.Cells[row, iSucursal];
                obj.Agencia = FNDExcelHelper.GetCellInt(celda);
                if (obj.Agencia == 0)
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    celda = hoja.Cells[row, iSucursal];
                    obj.Agencia = FNDExcelHelper.GetCellInt(celda);
                    celda = hoja.Cells[row, iAgencia];
                    obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCte];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);
                    
                    celda = hoja.Cells[row, iNombreCliente];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCredito];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFechaApertura];
                    obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iFechaVencim];
                    obj.FechaVencim = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iMontoOtorgado];
                    obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iCatRegion];
                    obj.CatRegionMigrado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCatEstado];
                    obj.CatEstadoMigrado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCatAgencia];
                    obj.CatAgenciaMigrado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iMinistraciones];
                    obj.Ministraciones = FNDExcelHelper.GetCellInt(celda);
                    celda = hoja.Cells[row, iFecPrimMinistra];
                    obj.FecPrimMinistra = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iFecUltimaMinistra];
                    obj.FecUltimaMinistra = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iMontoMinistrado];
                    obj.MontoMinistrado = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iReestructura];
                    obj.Reestructura = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCancelacion];
                    obj.Cancelacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iPagoCapital];
                    obj.PagoCapital = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iPagoInteres];
                    obj.PagoInteres = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iPagoMoratorios];
                    obj.PagoMoratorios = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iPagoTotal];
                    obj.PagoTotal = FNDExcelHelper.GetCellDecimal(celda);
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

        public async Task<IEnumerable<ColocacionConPagos>> GetColocacionConPagosAsync(string archivoLiquidaciones)
        {
            IList<ColocacionConPagos> resultado = new List<ColocacionConPagos>();
            _logger.LogInformation("Comenzamos a leer la colocación con pagos (Liquidados)");

            if (string.IsNullOrWhiteSpace(archivoLiquidaciones))
            {
                archivoLiquidaciones = _archivoLiquidacionesOrigen;
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoLiquidaciones))
            {
                _logger.LogError("No se encontró el Archivo de Colocación con Pagos\n{archivoColocacionConPagos}", archivoLiquidaciones);
                return resultado;
            }
            // TODO: Hacer el trabajo

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoLiquidaciones, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            // await package.LoadAsync(fs);
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = await Task.Run(() => { return package.Workbook.Worksheets["historico_colocacion_con_pagos"]; });
                
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            await Task.Run(() => { 
                while (!salDelCiclo)
                {
                    var obj = new ColocacionConPagos();
                    var celda = hoja.Cells[row, iAgencia];
                    obj.Agencia = FNDExcelHelper.GetCellInt(celda);
                    if (obj.Agencia == 0)
                    {
                        salDelCiclo = true;
                        //break;
                    }
                    else
                    {
                        celda = hoja.Cells[row, iSucursal];
                        obj.Agencia = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iAgencia];
                        obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCte];
                        obj.NumCte = FNDExcelHelper.GetCellString(celda);

                        celda = hoja.Cells[row, iNombreCliente];
                        obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCredito];
                        obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaApertura];
                        obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iFechaVencim];
                        obj.FechaVencim = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iMontoOtorgado];
                        obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iCatRegion];
                        obj.CatRegionMigrado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCatEstado];
                        obj.CatEstadoMigrado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCatAgencia];
                        obj.CatAgenciaMigrado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iMinistraciones];
                        obj.Ministraciones = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iFecPrimMinistra];
                        obj.FecPrimMinistra = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iFecUltimaMinistra];
                        obj.FecUltimaMinistra = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iMontoMinistrado];
                        obj.MontoMinistrado = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iReestructura];
                        obj.Reestructura = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCancelacion];
                        obj.Cancelacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iPagoCapital];
                        obj.PagoCapital = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iPagoInteres];
                        obj.PagoInteres = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iPagoMoratorios];
                        obj.PagoMoratorios = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iPagoTotal];
                        obj.PagoTotal = FNDExcelHelper.GetCellDecimal(celda);
                    }
                    if (!salDelCiclo)
                    {
                        resultado.Add(obj);
                    }
                    row++;
                } 
            });
            #endregion

            return resultado.ToList();
        }
    }
}
