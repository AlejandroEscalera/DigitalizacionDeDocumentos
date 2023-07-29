using gob.fnd.Dominio.Digitalizacion.Excel.Tratamientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using gob.fnd.ExcelHelper;
using OfficeOpenXml;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Tratamientos
{
    public class TratamientosService : ITratamientos
    {
        private int iCatRegion = 1;
        private int iCatEstado = 2;
        private int iCatAgencia = 3;
        private int iNumCte = 4;
        private int iNombreCliente = 5;
        private int iNumCredito = 6;
        private int iTipoTratamiento = 7;
        private int iCreditoOrigen = 8;
        private int iPagosRecibidos = 9;
        private int iFecPrimPago = 10;
        private int iFecUltimoPago = 11;
        private int iPagoCapital = 12;
        private int iPagoInteres = 13;
        private int iPagoMoratorios = 14;
        private int iPagoTotal = 15;

        const string C_CAT_REGION = "CAT_REGION";
        const string C_CAT_ESTADO = "CAT_ESTADO";
        const string C_CAT_AGENCIA = "CAT_AGENCIA";
        const string C_NUM_CTE = "numcte";
        const string C_NOMBRE_CLIENTE = "Nombre_Cliente";
        const string C_NUM_CREDITO = "num_credito";
        const string C_TIPO_TRATAMIENTO = "tipo_tratamiento";
        const string C_CREDITO_ORIGEN = "credito_origen";
        const string C_PAGOS_RECIBIDOS = "Pagos Recibidos";
        const string C_FEC_PRIM_PAGO = "Fec_prim_pago";
        const string C_FEC_ULTIMO_PAGO = "Fec_ultimo_pago";
        const string C_PAGO_CAPITAL = "Pago_Capital";
        const string C_PAGO_INTERES = "Pago_Interes";
        const string C_PAGO_MORATORIOS = "Pago_Moratorios";
        const string C_PAGO_TOTAL = "Pago_Total";
        private readonly ILogger<TratamientosService> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _archivoTratamientoYCancelados;

        public TratamientosService(ILogger<TratamientosService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoTratamientoYCancelados = _configuration.GetValue<string>("archivoTratamientoYCancelados") ?? "";
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
            iCatRegion = BuscaCelda(hoja, iCatRegion, C_CAT_REGION);
            iCatEstado = BuscaCelda(hoja, iCatEstado, C_CAT_ESTADO);
            iCatAgencia = BuscaCelda(hoja, iCatAgencia, C_CAT_AGENCIA);
            iNumCte = BuscaCelda(hoja, iNumCte, C_NUM_CTE);
            iNombreCliente = BuscaCelda(hoja, iNombreCliente, C_NOMBRE_CLIENTE);
            iNumCredito = BuscaCelda(hoja, iNumCredito, C_NUM_CREDITO);
            iTipoTratamiento = BuscaCelda(hoja, iTipoTratamiento, C_TIPO_TRATAMIENTO);
            iCreditoOrigen = BuscaCelda(hoja, iCreditoOrigen, C_CREDITO_ORIGEN);
            iPagosRecibidos = BuscaCelda(hoja, iPagosRecibidos, C_PAGOS_RECIBIDOS);
            iFecPrimPago = BuscaCelda(hoja, iFecPrimPago, C_FEC_PRIM_PAGO);
            iFecUltimoPago = BuscaCelda(hoja, iFecUltimoPago, C_FEC_ULTIMO_PAGO);
            iPagoCapital = BuscaCelda(hoja, iPagoCapital, C_PAGO_CAPITAL);
            iPagoInteres = BuscaCelda(hoja, iPagoInteres, C_PAGO_INTERES);
            iPagoMoratorios = BuscaCelda(hoja, iPagoMoratorios, C_PAGO_MORATORIOS);
            iPagoTotal = BuscaCelda(hoja, iPagoTotal, C_PAGO_TOTAL);
        }

        public async Task<IEnumerable<gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado.Tratamientos>> GetTratamientos(string archivoTratamientos)
        {
            IList<gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado.Tratamientos> resultado = new List<gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado.Tratamientos>();
            _logger.LogInformation("Comenzamos a leer la colocación con pagos (Liquidados)");

            if (string.IsNullOrWhiteSpace(archivoTratamientos))
            {
                archivoTratamientos = _archivoTratamientoYCancelados;
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoTratamientos))
            {
                _logger.LogError("No se encontró el Archivo de Tratamientos y Cancelados\n{archivoTratamientosYCancelados}", archivoTratamientos);
                return resultado;
            }
            // TODO: Hacer el trabajo

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoTratamientos, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets["Pag Tratamiento y Cancelado"];
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            while (!salDelCiclo)
            {
                var obj = new gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado.Tratamientos();
                var celda = hoja.Cells[row, iCatRegion];
                obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                if (string.IsNullOrEmpty(obj.CatRegion))
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    celda = hoja.Cells[row, iCatRegion];
                    obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCatEstado];
                    obj.CatEstado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCatAgencia];
                    obj.CatAgencia = FNDExcelHelper.GetCellString(celda);

                    celda = hoja.Cells[row, iNumCte];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNombreCliente];
                    obj.NombreCliente = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCredito];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTipoTratamiento];
                    obj.TipoTratamiento = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCreditoOrigen];
                    obj.CreditoOrigen = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iPagosRecibidos];
                    obj.PagosRecibidos = FNDExcelHelper.GetCellInt(celda);
                    celda = hoja.Cells[row, iFecPrimPago];
                    obj.FecPrimPago = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iFecUltimoPago];
                    obj.FecUltimoPago = FNDExcelHelper.GetCellDateTime(celda);

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

        public IEnumerable<ConversionTratamientos> GetConversionTratamientos(string archivoConversionTratamiento)
        {
            IList<ConversionTratamientos> resultado = new List<ConversionTratamientos>();
            _logger.LogInformation("Comenzamos a leer la colocación con pagos (Liquidados)");

            if (string.IsNullOrWhiteSpace(archivoConversionTratamiento))
            {
                // No hay default
                return resultado;
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoConversionTratamiento))
            {
                _logger.LogError("No se encontró el Archivo de Conversion de Tratamientos \n{archivoTratamientos}", archivoConversionTratamiento);
                return resultado;
            }
            // TODO: Hacer el trabajo

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoConversionTratamiento, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets.FirstOrDefault();
            if (hoja == null) {
                return resultado;
            }
            // ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            while (!salDelCiclo)
            {
                var obj = new ConversionTratamientos();
                var celda = hoja.Cells[row, 1];
                obj.Id = FNDExcelHelper.GetCellInt(celda);
                if (obj.Id == 0)
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    celda = hoja.Cells[row, 2];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, 3];
                    obj.Origen = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, 4];
                    obj.Destino = FNDExcelHelper.GetCellString(celda);

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
}
