using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.ExcelHelper;
using gob.fnd.Infraestructura.Digitalizacion.Excel.Config;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.ABSaldosC
{
    public class ServicioABSaldosConCastigo : IServicioABSaldosConCastigo
    {
        private readonly DateTime _periodoDelDoctor;
        private readonly ILogger<ServicioABSaldosConCastigo> _logger;
        private readonly IConfiguration _config;
        private readonly string _archivoSaldosConCastigoProcesado;
        #region Campos iniciales
        //private readonly IObtieneCatalogoProductos _obtieneCatalogoProductos;
        private int _CatalogoRegion = 2 + 1;/// B = 1 (segunda columna)
        private int _Sucursal = 5 + 1; /// C
        private int _CatalogoSucursal = 6 + 1; /// C
        private int _NumeroCliente = 7 + 1; /// C
        // private int _ApellidoPaterno = 6 + 1; /// C
        // private int _ApellidoMaterno = 7 + 1; /// C
        // private int _Nombre1 = 8 + 1; /// C
        // private int _Nombre2 = 9 + 1; /// C
        // private int _RazonSocial = 10 + 1; /// C
        private int _NombreCliente = 8 + 1; /// C
        private int _NumeroCredito = 9 + 1; /// C
        private int _FechaMinistracion = 14 + 1; /// C
        private int _FechaVencimiento = 15 + 1; /// C
        private int _NumProducto = 10 + 1;
        // private int _NombreEjecutivo = 88 + 1; /// C
        // private int _CatalogoProducto = 11 + 1; // U
        private int _TipoCredito = 11 + 1; // T
        private int _MontoMinistracion = 18 + 1; // CF - 2
        private int _InteresCont = 19 + 1;
        private int _Castigo = 26 + 1;
        #endregion

        public ServicioABSaldosConCastigo(ILogger<ServicioABSaldosConCastigo> logger, IConfiguration config) //IObtieneCatalogoProductos obtieneCatalogoProductos
        {
            _logger = logger;
            _config = config;
            //_obtieneCatalogoProductos = obtieneCatalogoProductos;
            string periodoDelDoctor = _config.GetValue<string>("periodoDelDoctor") ?? "01/07/2020";
            _periodoDelDoctor = DateTime.ParseExact(periodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture); //??new DateTime(2020,7,1)
            _archivoSaldosConCastigoProcesado = _config.GetValue<string>("archivoSaldosConCastigoProcesados") ?? "";
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
            _CatalogoRegion = BuscaCelda(hoja, _CatalogoRegion, "REGION");
            _logger.LogInformation("Datos del catalogo de region {region}", _CatalogoRegion);
            _Sucursal = BuscaCelda(hoja, _Sucursal, "AGENCIA");
            _CatalogoSucursal = BuscaCelda(hoja, _CatalogoSucursal, "NOM_AGENCIA");
            _NumeroCliente = BuscaCelda(hoja, _NumeroCliente, "NUM_CTE");
            // _ApellidoPaterno = BuscaCelda(hoja, _ApellidoPaterno, "NOMBRE_CLIENTE");
            // _ApellidoMaterno = BuscaCelda(hoja, _ApellidoMaterno, "NOMBRE_CLIENTE");
            // _Nombre1 = BuscaCelda(hoja, _Nombre1, "NOMBRE_CLIENTE");
            // _Nombre2 = BuscaCelda(hoja, _Nombre2, "NOMBRE_CLIENTE");
            // _RazonSocial = BuscaCelda(hoja, _RazonSocial, "NOMBRE_CLIENTE");
            _NombreCliente = BuscaCelda(hoja, _NombreCliente, "NOMBRE_CLIENTE");
            _NumeroCredito = BuscaCelda(hoja, _NumeroCredito, "NUM_CREDITO");
            _FechaMinistracion = BuscaCelda(hoja, _FechaMinistracion, "FECHA_APERTURA");
            _FechaVencimiento = BuscaCelda(hoja, _FechaVencimiento, "FECHA_VENCIMIENTO");
            // _NombreEjecutivo = BuscaCelda(hoja, _NombreEjecutivo, "NOMBRE_EJECUTIVO");
            _NumProducto = BuscaCelda(hoja, _NumProducto, "NUM_PRODUCTO");
            // _CatalogoProducto = BuscaCelda(hoja, _CatalogoProducto, "");
            _TipoCredito = BuscaCelda(hoja, _TipoCredito, "TIPO_CREDITO");
            _MontoMinistracion = BuscaCelda(hoja, _MontoMinistracion, "CAPITAL_CONT");
            _InteresCont = BuscaCelda(hoja, _InteresCont, "INTERES_CONT");
            _Castigo = BuscaCelda(hoja, _Castigo, "Castigo");
        }
        public IEnumerable<ABSaldosConCastigo> AgregaAgentesYGuardaValores(IEnumerable<ABSaldosConCastigo> origen, IEnumerable<CorreosAgencia> agentes)
        {
            foreach (var registro in origen)
            {
                registro.Agente = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.NombreAgente).FirstOrDefault<string?>() ?? "";
                registro.GuardaValores = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.NombreGuardaValores).FirstOrDefault<string?>() ?? "";
                // registro.LigaAgencia = agentes.Where(x => x.NoAgencia == registro.Sucursal).Select(x => x.LigaAgencia).FirstOrDefault<string?>() ?? "";
            }
            return origen;
        }

        public IEnumerable<ABSaldosConCastigo> CargaProcesaYLimpia(string archivoOrigen) // , string archivoDestino
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //DateTime fechaFiltro = _obtieneCatalogoProductos.ObtieneFechaFiltro() ?? (new DateTime(2020, 7, 1));

            bool resultado = false;
            if (!File.Exists(archivoOrigen))
            {
                _logger.LogError("No se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldos} con Castigos", archivoOrigen);
                return new List<ABSaldosConCastigo>();
            }
            _logger.LogWarning("Procesando el archivo ABSaldos\n{archivoABSaldos} con Castigos", archivoOrigen);
            IList<ABSaldosConCastigo> lista = new List<ABSaldosConCastigo>();
            //var filtro = _obtieneCatalogoProductos.ObtieneListaAFiltrar();
            //IEnumerable<string> numerosDeProducto = ObtieneArregloDeNumerosDeProducto(filtro);
            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoOrigen, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista;
                var hoja = package.Workbook.Worksheets[0];
                ObtieneValoresCampos(hoja);
                int row = 2;
                while (!resultado)
                {
                    var obj = new ABSaldosConCastigo();
                    var celda = hoja.Cells[row, _CatalogoRegion];
                    obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                    if (string.IsNullOrEmpty(obj.CatRegion))
                    {
                        resultado = true;
                        break;
                    }
                    obj.Sucursal = FNDExcelHelper.GetCellDouble(hoja.Cells[row, _Sucursal]);
                    obj.CatSucursal = FNDExcelHelper.GetCellString(hoja.Cells[row, _CatalogoSucursal]);
                    obj.NumeroCliente = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumeroCliente]);
                    // obj.ApellidoPaterno = FNDExcelHelper.GetCellString(hoja.Cells[row, _ApellidoPaterno]);
                    // obj.ApellidoMaterno = FNDExcelHelper.GetCellString(hoja.Cells[row, _ApellidoMaterno]);
                    // obj.PrimerNombre = FNDExcelHelper.GetCellString(hoja.Cells[row, _Nombre1]);
                    // obj.SegundoNombre = FNDExcelHelper.GetCellString(hoja.Cells[row, _Nombre2]);
                    // obj.RazonSocial = FNDExcelHelper.GetCellString(hoja.Cells[row, _RazonSocial]);
                    obj.Acreditado = FNDExcelHelper.GetCellString(hoja.Cells[row, _NombreCliente]);
                    obj.Acreditado = obj.Acreditado.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("Ð", "Ñ");
                    obj.NumCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumeroCredito]);
                    if (!string.IsNullOrEmpty(obj.NumCredito) && obj.NumCredito.Length>17)
                    {
                        string tratamiento = obj.NumCredito.Substring(3, 1);
                        obj.EsTratamiento = tratamiento.Equals("8");
                    }

                    obj.NumeroAperturaCredito = Condiciones.QuitaCastigo(obj.NumCredito[..14] + "0001");
                    obj.FechaApertura = FNDExcelHelper.GetCellDateTime(hoja.Cells[row, _FechaMinistracion]);
                    obj.FechaVencimiento = FNDExcelHelper.GetCellDateTime(hoja.Cells[row, _FechaVencimiento]);
                    // obj.NombreEjecutivo = FNDExcelHelper.GetCellString(hoja.Cells[row, _NombreEjecutivo]);
                    //obj.CatProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, _CatalogoProducto]);
                    obj.TipoDeCredito = FNDExcelHelper.GetCellString(hoja.Cells[row, _TipoCredito]);
                    obj.MontoCredito = Convert.ToDecimal(hoja.Cells[row, _MontoMinistracion].Value);
                    obj.InteresCont = Convert.ToDecimal(hoja.Cells[row, _InteresCont].Value);
                    obj.NumProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, _NumProducto]);
                    // filtro.Where(x => (x.NumProducto ?? "").Contains(obj, StringComparison.CurrentCulture)).Select(y => y.CatalogoProducto).FirstOrDefault() ?? "";
                    obj.Castigo = FNDExcelHelper.GetCellString(hoja.Cells[row, _Castigo]);
                    obj.EsOrigenDelDoctor = (obj.FechaApertura >= _periodoDelDoctor);
                    if (!resultado)
                    {
                        //if (obj.FechaApertura.HasValue && (obj.FechaApertura >= fechaFiltro )) // || obj.FechaVencimiento >= fechaFiltro
                            //if ((numerosDeProducto.Any(x => x == obj.NoTipoDeCredito.Trim().ToUpper())))
                                lista.Add(obj);
                    }
                    row++;
                }
            }
            #endregion

            // throw new NotImplementedException();

            return lista;
        }

        /*
        private static IEnumerable<string> ObtieneArregloDeNumerosDeProducto(IEnumerable<FiltraCatalogoProductos> filtro)
        {
            IList<string> listaDeNumerosDeProducto = new List<string>();
            IEnumerable<string> numerosDeProducto;
            foreach (var item in filtro)
            {
                if ((item.NumProducto ?? "").Split('|').Length > 1)
                {
                    foreach (string numProducto in (item.NumProducto ?? "").Split('|'))
                    {
                        listaDeNumerosDeProducto.Add(numProducto);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.NumProducto))
                    {
                        listaDeNumerosDeProducto.Add(item.NumProducto);
                    }
                }
            }
            numerosDeProducto = listaDeNumerosDeProducto.ToArray();
            return numerosDeProducto;
        }
        */

        public bool GuardaInformacionABSaldos(string archivoDestino, IEnumerable<ABSaldosConCastigo> listadoCreditos)
        {
            if (File.Exists(archivoDestino))
            {
                try
                {
                    File.Delete(archivoDestino);
                }
                catch
                {
                    _logger.LogError("Se encontró el Archivo de AB Saldos a procesar\n{archivoABSaldos}", archivoDestino);
                    return false;
                }
            }


            #region Actualización de Datos
            if (File.Exists(archivoDestino))
            {
                _logger.LogError("Ya existía el archivo de los nuevos creditos\n{archivoDestino}\n se procede a eliinarlo", archivoDestino);
                File.Delete(archivoDestino);
            }

            FileStream fsO = new(archivoDestino, FileMode.CreateNew, FileAccess.Write);
            using var package = new ExcelPackage(fsO);
            _logger.LogWarning(
                "Se comienzan a vaciar los nuevos creditos en el archivo destino\n{archivoDestino}\n con un total de {totalCreditosNuevos}",
                archivoDestino,
                listadoCreditos.Count());
            FileInfo fi = new(archivoDestino);
            string nombreHoja = "ABSaldos_Diarios_"; // = fi.Name[9..].Replace("202", "2").Replace(" Norte.xlsx", "");
            ExcelWorksheet hoja = package.Workbook.Worksheets.Add(nombreHoja);
            hoja.Name = nombreHoja;

            int row = 1;
            int col = 1;
            hoja.Cells[row, col].Value = "CAT_REGION";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 14.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "sucursal";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 6.78 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CAT_SUCURSAL";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 20.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "numcte";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 12.22 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NOMBRE_CLIENTES";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 47 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "num_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ES_TRATAMIENTO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "origina_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "fecha_apertura";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 12.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "fecha_vencim";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 12.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "fecha_ministra";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 12.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "monto_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "interes_cont";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Agente";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 47 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "GuardaValores";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 47 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NOMBRE_EJECUTIVO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 47 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "num_producto";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 6.78 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CAT_TIPO_CREDITO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 20.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CAT_PRODUCTO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 39.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CASTIGO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 6.78 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ES_ACTIVO_PAGADO";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ES_ORIGEN_DEL_DOCTOR";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "ES_CANCELACION_DEL_DOCTOR";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EXPEDIENTE_DIGITAL";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NO_ARCHIVOS";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "TIENE_GV";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "TIPO_CARTERA";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 28.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "MONTO_MINISTRA_CAP";
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
            hoja.Cells[row, col].Value = "NO_MINISTRACION";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;

            col++;
            hoja.Cells[row, col].Value = "FECHA_INICIO_MINISTRACION"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;

            col++;
            hoja.Cells[row, col].Value = "Clasificacion Mesa";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 47 + 0.78;

            col++;
            hoja.Cells[row, col].Value = "FECHA_SOLICITUD"; // Id
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;

            col++;
            hoja.Cells[row, col].Value = "monto_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;

            row++;
            foreach (var ab in listadoCreditos)
            {
                int celda = 1;
                hoja.Cells[row, celda].Value = ab.CatRegion;
                celda++;
                hoja.Cells[row, celda].Value = ab.Sucursal;
                celda++;
                hoja.Cells[row, celda].Value = ab.CatSucursal;
                celda++;
                hoja.Cells[row, celda].Value = ab.NumeroCliente;
                celda++;

                hoja.Cells[row, celda].Value = ab.Acreditado;
                celda++;
                hoja.Cells[row, celda].Value = ab.NumCredito;
                celda++;
                hoja.Cells[row, celda].Value = Condiciones.EsTratamiento(ab.EsTratamiento);
                celda++;

                hoja.Cells[row, celda].Value = ab.NumeroAperturaCredito;
                celda++;
                if (ab.FechaApertura.HasValue)
                    FNDExcelHelper.SetCellDate(hoja.Cells[row, celda], ab.FechaApertura.Value);
                celda++;
                if (ab.FechaVencimiento.HasValue)
                    FNDExcelHelper.SetCellDate(hoja.Cells[row, celda], ab.FechaVencimiento.Value);
                celda++;
                if (ab.FechaMinistra.HasValue)
                    FNDExcelHelper.SetCellDate(hoja.Cells[row, celda], ab.FechaMinistra.Value);
                celda++;


                hoja.Cells[row, celda].Value = ab.MontoCredito;
                hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                celda++;

                hoja.Cells[row, celda].Value = ab.InteresCont;
                hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                celda++;

                hoja.Cells[row, celda].Value = ab.Agente;
                celda++;
                hoja.Cells[row, celda].Value = ab.GuardaValores;
                celda++;
                hoja.Cells[row, celda].Value = ab.NombreEjecutivo;
                celda++;

                hoja.Cells[row, celda].Value = ab.NumProducto; 
                celda++;
                hoja.Cells[row, celda].Value = ab.TipoDeCredito;
                celda++;

                hoja.Cells[row, celda].Value = ab.CatProducto;
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.ObtieneCastigo(ab.Castigo);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EstaEnABSaldosActivo(ab.EsCarteraActiva);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsOrigenDelDoctor);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsCancelacionDelDoctor);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.TieneExpedienteDigital(ab.ExpedienteDigital);
                celda++;

                hoja.Cells[row, celda].Value = ab.NoPDF;
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.TieneGuardaValores(ab.CuentaConGuardaValores);
                celda++;

                hoja.Cells[row, celda].Value = ab.TipoCartera;
                celda++;

                hoja.Cells[row, celda].Value = ab.MontoMinistraCap;
                hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                celda++;

                hoja.Cells[row, celda].Value = ab.NumCreditoOriginal;
                celda++;

                hoja.Cells[row, celda].Value = ab.SesionDeAutorizacion;
                celda++;

                hoja.Cells[row, celda].Value = ab.MesCastigo;
                celda++;

                hoja.Cells[row, celda].Value = ab.AnioCastigo;
                celda++;

                hoja.Cells[row, celda].Value = ab.Generacion;
                celda++;

                if (ab.FechaCancelacion is not null)
                    hoja.Cells[row, celda].SetCellDate(ab.FechaCancelacion.Value);
                celda++;

                hoja.Cells[row, celda].Value = ab.AnioOriginacion;
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EstaEnCancelado(ab.EstaEnCreditosCancelados);
                celda++;

                hoja.Cells[row, celda].Value = ab.NumeroMinistracion;
                celda++;

                if (ab.FechaInicioMinistracion is not null)
                    hoja.Cells[row, celda].SetCellDate(ab.FechaInicioMinistracion.Value);
                celda++;

                hoja.Cells[row, celda].Value = ab.ClasificacionMesa;
                celda++;

                if (ab.FechaDeSolicitud is not null)
                    hoja.Cells[row, celda].SetCellDate(ab.FechaDeSolicitud.Value);
                celda++;

                hoja.Cells[row, celda].Value = ab.MontoOtorgado;
                //celda++;

                row++;

            }

            #region autofiltro
            hoja.Cells[String.Format("A1:AN{0}", row - 1)].AutoFilter = true;  // ABCDEFGHIJKLMNO
            hoja.View.FreezePanes(2, 1);
            #endregion

            package.Save();
            _logger.LogWarning("Se terminaron de procesar los nuevos creditos en:\n{archivoDestino}\n", archivoDestino);

            #endregion
            return true;
        }

        public IEnumerable<ABSaldosConCastigo> ObtieneABSaldosConCastigoProcesados()
        {
            string archivoOrigen = _archivoSaldosConCastigoProcesado;

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //DateTime fechaFiltro = _obtieneCatalogoProductos.ObtieneFechaFiltro() ?? (new DateTime(2020, 7, 1));

            bool resultado = false;
            if (!File.Exists(archivoOrigen))
            {
                _logger.LogError("No se encontró el Archivo de AB Saldos ya procesados\n{archivoABSaldos} con Castigos", archivoOrigen);
                return new List<ABSaldosConCastigo>();
            }
            _logger.LogWarning("Cargando el archivo ABSaldos\n{archivoABSaldos} con Castigos", archivoOrigen);
            IList<ABSaldosConCastigo> lista = new List<ABSaldosConCastigo>();
            //var filtro = _obtieneCatalogoProductos.ObtieneListaAFiltrar();
            //IEnumerable<string> numerosDeProducto = ObtieneArregloDeNumerosDeProducto(filtro);
            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoOrigen, FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista;
                var hoja = package.Workbook.Worksheets[0];
                //ObtieneValoresCampos(hoja);
                int row = 2;
                while (!resultado)
                {
                    var obj = new ABSaldosConCastigo();

                    int col = 1;
                    var celda = hoja.Cells[row, col];
                    obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                    col++;
                    if (string.IsNullOrEmpty(obj.CatRegion))
                        break;

                    celda = hoja.Cells[row, col];
                    obj.Sucursal = FNDExcelHelper.GetCellDouble(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CatSucursal = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumeroCliente = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsTratamiento = Condiciones.ObtieneEsTratamiento(FNDExcelHelper.GetCellString(celda))??false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumeroAperturaCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaVencimiento = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaMinistra = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MontoCredito = FNDExcelHelper.GetCellDecimal(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.InteresCont = FNDExcelHelper.GetCellDecimal(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Agente = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.GuardaValores = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NombreEjecutivo = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumProducto = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TipoDeCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CatProducto = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsCarteraActiva = Condiciones.ObtieneEstaEnABSaldosActivo(FNDExcelHelper.GetCellString(celda))??false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsOrigenDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsCancelacionDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ExpedienteDigital = Condiciones.ObtieneTieneExpedienteDigital(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NoPDF = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CuentaConGuardaValores = Condiciones.ObtieneTieneGuardaValores(FNDExcelHelper.GetCellString(celda))??false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TipoCartera = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MontoMinistraCap = FNDExcelHelper.GetCellDecimal(celda);
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
                    obj.FechaInicioMinistracion = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.ClasificacionMesa = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaDeSolicitud = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                    // col++;

                    lista.Add(obj);
                    row++;

                }
            }
            #endregion

            // throw new NotImplementedException();

            return lista;
        }

        public async Task<IEnumerable<ABSaldosConCastigo>> ObtieneABSaldosConCastigoProcesadosAsync()
        {
            string archivoOrigen = _archivoSaldosConCastigoProcesado;

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //DateTime fechaFiltro = _obtieneCatalogoProductos.ObtieneFechaFiltro() ?? (new DateTime(2020, 7, 1));

            bool resultado = false;
            if (!File.Exists(archivoOrigen))
            {
                _logger.LogError("No se encontró el Archivo de AB Saldos ya procesados\n{archivoABSaldos} con Castigos", archivoOrigen);
                return new List<ABSaldosConCastigo>();
            }
            _logger.LogWarning("Cargando el archivo ABSaldos\n{archivoABSaldos} con Castigos", archivoOrigen);
            IList<ABSaldosConCastigo> lista = new List<ABSaldosConCastigo>();
            //var filtro = _obtieneCatalogoProductos.ObtieneListaAFiltrar();
            //IEnumerable<string> numerosDeProducto = ObtieneArregloDeNumerosDeProducto(filtro);
            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoOrigen, FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                await package.LoadAsync(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista;
                var hoja = package.Workbook.Worksheets[0];
                //ObtieneValoresCampos(hoja);
                int row = 2;
                await Task.Run(() => { 
                    while (!resultado)
                    {
                        var obj = new ABSaldosConCastigo();

                        int col = 1;
                        var celda = hoja.Cells[row, col];
                        obj.CatRegion = FNDExcelHelper.GetCellString(celda);
                        col++;
                        if (string.IsNullOrEmpty(obj.CatRegion))
                            break;

                        celda = hoja.Cells[row, col];
                        obj.Sucursal = FNDExcelHelper.GetCellDouble(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.CatSucursal = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.NumeroCliente = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.EsTratamiento = Condiciones.ObtieneEsTratamiento(FNDExcelHelper.GetCellString(celda)) ?? false;
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.NumeroAperturaCredito = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.FechaApertura = FNDExcelHelper.GetCellDateTime(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.FechaVencimiento = FNDExcelHelper.GetCellDateTime(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.FechaMinistra = FNDExcelHelper.GetCellDateTime(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.MontoCredito = FNDExcelHelper.GetCellDecimal(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.InteresCont = FNDExcelHelper.GetCellDecimal(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.Agente = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.GuardaValores = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.NombreEjecutivo = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.NumProducto = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.TipoDeCredito = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.CatProducto = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(celda));
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.EsCarteraActiva = Condiciones.ObtieneEstaEnABSaldosActivo(FNDExcelHelper.GetCellString(celda)) ?? false;
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.EsOrigenDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.EsCancelacionDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
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
                        obj.TipoCartera = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.MontoMinistraCap = FNDExcelHelper.GetCellDecimal(celda);
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
                        obj.FechaInicioMinistracion = FNDExcelHelper.GetCellDateTime(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.ClasificacionMesa = FNDExcelHelper.GetCellString(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.FechaDeSolicitud = FNDExcelHelper.GetCellDateTime(celda);
                        col++;

                        celda = hoja.Cells[row, col];
                        obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                        // col++;

                        lista.Add(obj);
                        row++;

                    }
                });
            }
            #endregion

            // throw new NotImplementedException();

            return lista;
        }
    }
}
