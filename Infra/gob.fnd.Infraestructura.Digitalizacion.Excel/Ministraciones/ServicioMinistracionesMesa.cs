using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
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

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Ministraciones
{
    public class ServicioMinistracionesMesa : IServicioMinistracionesMesa
    {
        private readonly DateTime _periodoDelDoctor;
        private readonly ILogger<ServicioMinistracionesMesa> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _archivoMinistracionesMesa;
        private readonly string _archivoMinistracionesMesaProcesado;

        private int iAnalista = 1;
        private int iNumCredito = 2;
        private int iNumMinistracion=3;
        private int iNumSolicitud=4;
        private int iStaSolicitud=6;
        private int iFechaInicio=7;
        private int iDescripcion=11;
        private int iMontoOtorgado=12;
        private int iFechaAsignacion=13;
        private int iAcreditado=14;
        private int iRegional=15;
        private int iSucursal=16;
        private int iAgencia=17;

        const string C_STR_ANALISTA= "nom_nombre";
        const string C_STR_NUM_CREDITO = "pk_num_cred";
        const string C_STR_NUM_MINISTRACION = "pk_num_minist";
        const string C_STR_NUM_SOLICITUD = "pk_solicitud";
        const string C_STR_STA_SOLICITUD = "sta_solicitud";
        const string C_STR_FECHA_INICIO = "fec_inicio";
        const string C_STR_DESCRIPCION = "des_descripcion";
        const string C_STR_MONTO_OTORGADO = "monto_otorgado";
        const string C_STR_FECHA_ASIGNACION = "fec_asignacion";
        const string C_STR_ACREDITADO = "acreditado";
        const string C_STR_REGIONAL = "regional";
        const string C_STR_SURCURSAL = "sucursal";
        const string C_STR_AGENCIA = "nombre";

        public ServicioMinistracionesMesa(ILogger<ServicioMinistracionesMesa> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoMinistracionesMesa = _configuration.GetValue<string>("archivoMinistracionesMesa")??"";
            _archivoMinistracionesMesaProcesado = _configuration.GetValue<string>("archivoMinistracionesMesaProcesado") ?? "";
            string periodoDelDoctor = _configuration.GetValue<string>("periodoDelDoctor") ?? "01/07/2020";
            _periodoDelDoctor = DateTime.ParseExact(periodoDelDoctor, "dd/MM/yyyy", CultureInfo.InvariantCulture); //??new DateTime(2020,7,1)
        }

        public IEnumerable<MinistracionesMesa> GetMinistraciones()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            IList<MinistracionesMesa> resultado = new List<MinistracionesMesa>();
            if (!File.Exists(_archivoMinistracionesMesa)) 
            {
                _logger.LogError("No se encontró el Archivo de Ministraciones de la Mesa a procesar\n{archivoMinistracionesMesa}", _archivoMinistracionesMesa);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(_archivoMinistracionesMesa, FileMode.Open, FileAccess.Read, FileShare.Read);
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
                    var obj = new MinistracionesMesa();
                    var celda = hoja.Cells[row, iAnalista];
                    obj.Analista = FNDExcelHelper.GetCellString(celda);
                    if (string.IsNullOrEmpty(obj.Analista))
                    {
                        salDelCiclo = true;
                        //break;
                    }
                    else
                    {
                        obj.Id = row - 1;
                        celda = hoja.Cells[row, iNumCredito];
                        obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumMinistracion];
                        obj.NumMinistracion = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iNumSolicitud];
                        obj.NumSolicitud = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iStaSolicitud];
                        obj.StaSolicitud = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaInicio];
                        obj.FechaInicio = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iDescripcion];
                        obj.Descripcion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iMontoOtorgado];
                        obj.MontoOtorgado = Convert.ToDecimal(FNDExcelHelper.GetCellDouble(celda));
                        celda = hoja.Cells[row, iFechaAsignacion];
                        obj.FechaAsignacion = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iAcreditado];
                        obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iRegional];
                        obj.Regional = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iSucursal];
                        obj.Sucursal = FNDExcelHelper.GetCellInt(celda);
                        celda = hoja.Cells[row, iAgencia];
                        obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                        obj.EsOrigenDelDoctor = (obj.FechaInicio >= _periodoDelDoctor);
                        obj.EsSolicitudDoctor = (obj.FechaInicio >= _periodoDelDoctor);
                        if (!string.IsNullOrEmpty(obj.NumCredito) && (obj.NumCredito.Length > 17))
                        {
                            string tratamiento = obj.NumCredito.Substring(3, 1);
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

            return resultado.OrderBy(x=>x.NumCredito).Select(y=>y).ToList();
        }

        private void ObtieneValoresCampos(ExcelWorksheet hoja)
        {
            iAnalista = BuscaCelda(hoja, iAnalista, C_STR_ANALISTA);
            iNumCredito = BuscaCelda(hoja, iNumCredito, C_STR_NUM_CREDITO);
            iNumMinistracion = BuscaCelda(hoja, iNumMinistracion, C_STR_NUM_MINISTRACION);
            iNumSolicitud = BuscaCelda(hoja, iNumSolicitud, C_STR_NUM_SOLICITUD);
            iStaSolicitud = BuscaCelda(hoja, iStaSolicitud, C_STR_STA_SOLICITUD);
            iFechaInicio = BuscaCelda(hoja, iFechaInicio, C_STR_FECHA_INICIO);
            iDescripcion = BuscaCelda(hoja, iDescripcion, C_STR_DESCRIPCION);
            iMontoOtorgado = BuscaCelda(hoja, iMontoOtorgado, C_STR_MONTO_OTORGADO);
            iFechaAsignacion = BuscaCelda(hoja, iFechaAsignacion, C_STR_FECHA_ASIGNACION);
            iAcreditado = BuscaCelda(hoja, iAcreditado, C_STR_ACREDITADO);
            iRegional = BuscaCelda(hoja, iRegional, C_STR_REGIONAL);
            iSucursal = BuscaCelda(hoja, iSucursal, C_STR_SURCURSAL);
            iAgencia = BuscaCelda(hoja, iAgencia, C_STR_AGENCIA);
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

        public bool GuardaMinistraciones(IEnumerable<MinistracionesMesa> listaDocumentosMesa)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            #region Guarda información
            if (File.Exists(_archivoMinistracionesMesaProcesado))
            {
                _logger.LogError("Ya existía el archivo de los creditos pagados por la mesa \n{archivoDestinoMesa}\n se procede a eliminarlo", _archivoMinistracionesMesaProcesado);
                File.Delete(_archivoMinistracionesMesaProcesado);
            }
            FileStream fsO = new(_archivoMinistracionesMesaProcesado, FileMode.CreateNew, FileAccess.Write);
            using var package = new ExcelPackage(fsO);
            _logger.LogWarning(
                "Se comienzan a vaciar los créditos cancelados en el archivo destino\n{archivoDestinoCancelados}\n con un total de {totalCreditosCancelados}",
                _archivoMinistracionesMesaProcesado,
                listaDocumentosMesa.Count());
            FileInfo fi = new(_archivoMinistracionesMesaProcesado);
            string nombreHoja = "creditos";
            ExcelWorksheet hoja = package.Workbook.Worksheets.Add(nombreHoja);
            hoja.Name = nombreHoja;

            int row = 1;
            int col = 1;

            hoja.Cells[row, col].Value = "#";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Analista";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 38.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "num_credito";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "num_ministracion";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "num_solicitud";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Sta_Solicitud";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Fecha_Inicio";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Descripción";
            hoja.Cells[row, col].Style.WrapText = true;
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 64.56 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Monto_Otorgado";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 17.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Fecha_Asignacion";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 15 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Acreditado";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 64.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Regional";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Sucursal";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.67 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "CatAgencia";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 27.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EsOrigenDelDoctor";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EsSolicitudDoctor";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EstaCancelado";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EstaEnSaldosCastigos";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 9.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EstaEnSaldosActivos";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Castigo";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 9.89 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Expediente digital";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "Exp dig. Indirecto";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 10.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "NumCreditoActual";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 18.33 + 0.78;
            col++;

            hoja.Cells[row, col].Value = "EsTratamiento";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 9.89 + 0.78;
            col++;
            
            hoja.Cells[row, col].Value = "Tiene GV";
            FNDExcelHelper.ChangeBackgroundColor(hoja.Cells[row, col], Color.FromArgb(179, 142, 93));
            hoja.Column(col).Width = 9.89 + 0.78;
            // col++;

            row++;
            foreach (var ab in listaDocumentosMesa)
            {
                int celda = 1;
                hoja.Cells[row, celda].Value = ab.Id;
                celda++;

                hoja.Cells[row, celda].Value = ab.Analista;
                celda++;

                hoja.Cells[row, celda].Value = ab.NumCredito;
                celda++;

                hoja.Cells[row, celda].Value = ab.NumMinistracion;
                celda++;

                hoja.Cells[row, celda].Value = ab.NumSolicitud;
                celda++;

                hoja.Cells[row, celda].Value = ab.StaSolicitud;
                celda++;

                if (ab.FechaInicio.HasValue)
                    hoja.Cells[row, celda].SetCellDate(ab.FechaInicio.Value);
                celda++;

                hoja.Cells[row, celda].Value = ab.Descripcion;
                celda++;

                hoja.Cells[row, celda].Value = ab.MontoOtorgado;
                hoja.Cells[row, celda].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                celda++;

                if (ab.FechaAsignacion.HasValue)
                    hoja.Cells[row, celda].SetCellDate(ab.FechaAsignacion.Value);
                celda++;

                hoja.Cells[row, celda].Value = ab.Acreditado;
                celda++;

                hoja.Cells[row, celda].Value = ab.Regional;
                celda++;

                hoja.Cells[row, celda].Value = ab.Sucursal;
                celda++;

                hoja.Cells[row, celda].Value = ab.CatAgencia;
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsOrigenDelDoctor);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsPeriodoDelDr(ab.EsSolicitudDoctor);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EstaEnCancelado(ab.EstaCancelado); ///
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsCarteraABSaldos(ab.EstaEnSaldosCastigos);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EstaEnABSaldosActivo(ab.EstaEnSaldosActivos);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.ObtieneCastigo(ab.Castigo);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.TieneExpedienteDigital(ab.TieneImagen);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.TieneExpedienteDigital(ab.TieneImagenIndirecta);
                celda++;

                hoja.Cells[row, celda].Value = ab.NumCreditoActual;
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.EsTratamiento(ab.EsTratamiento);
                celda++;

                hoja.Cells[row, celda].Value = Condiciones.TieneGuardaValores(ab.CuentaConGuardaValores);

                row++;
            }
            #endregion

            #region autofiltro
            hoja.Cells[String.Format("A1:Y{0}", row - 1)].AutoFilter = true;  // ABCDE FGHIJ KLMNO PQRST UVWXY Z
            hoja.View.FreezePanes(2, 1);
            #endregion

            package.Save();
            return true;
        }

        public IEnumerable<MinistracionesMesa> GetMinistracionesProcesadas()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            IList<MinistracionesMesa> resultado = new List<MinistracionesMesa>();
            if (!File.Exists(_archivoMinistracionesMesaProcesado))
            {
                _logger.LogError("No se encontró el Archivo de Ministraciones de la Mesa a procesar\n{archivoMinistracionesMesa}", _archivoMinistracionesMesa);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(_archivoMinistracionesMesaProcesado, FileMode.Open, FileAccess.Read, FileShare.Read);
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
                    var obj = new MinistracionesMesa();

                    int col = 1;

                    var celda = hoja.Cells[row, col];
                    obj.Id = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    if (obj.Id == 0)
                        break;

                    celda = hoja.Cells[row, col];
                    obj.Analista = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumMinistracion = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumSolicitud = FNDExcelHelper.GetCellInt(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.StaSolicitud = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaInicio = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Descripcion = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.MontoOtorgado = FNDExcelHelper.GetCellDecimal(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.FechaAsignacion = FNDExcelHelper.GetCellDateTime(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
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
                    obj.EsOrigenDelDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda))??false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsSolicitudDoctor = Condiciones.RegresaEsPeriodoDelDr(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EstaCancelado = Condiciones.ObtieneEstaEnCancelado(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EstaEnSaldosCastigos = Condiciones.ObtenerEsCarteraABSaldos(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EstaEnSaldosActivos = Condiciones.ObtieneEstaEnABSaldosActivo(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.Castigo = Condiciones.RegresaObtieneCastigo(FNDExcelHelper.GetCellString(celda));
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TieneImagen = Condiciones.ObtieneTieneExpedienteDigital(FNDExcelHelper.GetCellString(celda))?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.TieneImagenIndirecta = Condiciones.ObtieneTieneExpedienteDigital(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.NumCreditoActual = FNDExcelHelper.GetCellString(celda);
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.EsTratamiento = Condiciones.ObtieneEsTratamiento(FNDExcelHelper.GetCellString(celda)) ?? false;
                    col++;

                    celda = hoja.Cells[row, col];
                    obj.CuentaConGuardaValores = Condiciones.ObtieneTieneGuardaValores(FNDExcelHelper.GetCellString(celda)) ?? false;
                    // col++;

                    resultado.Add(obj);

                    row++;
                }
            }
            #endregion

            return resultado.OrderBy(x => x.NumCredito).Select(y => y).ToList();
        }
    }
}

