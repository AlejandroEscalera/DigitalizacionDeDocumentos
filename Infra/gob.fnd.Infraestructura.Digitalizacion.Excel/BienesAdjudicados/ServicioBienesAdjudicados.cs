using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.BienesAdjudicados
{
    public class ServicioBienesAdjudicados : IBienesAdjudicados
    {
        private int iCveBien = 1;
        private int iExpedienteElectronico = 2;
        private int iCoordinacionRegional = 3;
        private int iAcreditado = 4;
        private int iTipoDeAdjudicacion = 5;
        private int iTipoDeBien = 6;
        private int iDescripcionReducidaBien = 7;
        private int iDescripcionCompletaBien = 8;
        private int iEntidadFederativa = 9;
        private int iMunicipio = 10;
        private int iNumeroCliente = 11;
        private int iOficioNotificaiconGCRJ = 12;
        private int iNumContrato = 13;
        private int iNumCredito = 14;
        private int iFechaFirmezaAdjudicacion = 15;
        private int iImporteIndivAdjudicacion = 16;
        private int iImporteIndivImplicado = 17;
        private int iFechaComunicadoGOT = 18;
        private int iCuentaConEscritura = 19;
        private int iCuentaConEscrituraFisica = 20;
        private int iCuentaConLibertadGravamen = 21;
        private int iDocumentoDeJuridicoDeInexistencia = 22;
        private int iCuentaConProcesosJudiciales = 23;
        private int iFactura = 24;
        private int iTomaMaterial = 25;
        private int iFotografias = 26;
        private int iNoAdeudo = 27;
        private int iDocumentoJuridicoInexistenciaProcesosJudiciales = 28;
        private int iCuentaConProcesosJudiciales2 = 29;
        private int iFechaOficioSolicitudTransferenciaAlIndep = 30;
        private int iFechaVenta = 31;
        private int iImporteDeVenta = 32;
        private int iFechaPolizaDeBajaContable = 33;
        private int iAreaResponsable = 34;
        private int iEstatus = 35;
        private int iComentarioDelEstatusDelBien = 36;

        const string C_CVEBIEN = "Clave del Bien";
        const string C_EXPEDIENTEELECTRONICO = "Expediente Electrónico";
        const string C_COORDINACIONREGIONAL = "CR";
        const string C_ACREDITADO = "Acreditado(s)";
        const string C_TIPODEADJUDICACION = "Tipo de Adjudicación";
        const string C_TIPODEBIEN = "Tipo de bien";
        const string C_DESCRIPCIONREDUCIDABIEN = "Descripción reducida del bien";
        const string C_DESCRIPCIONCOMPLETABIEN = "Descripción completa del bien";
        const string C_ENTIDADFEDERATIVA = "Entidad Federativa";
        const string C_MUNICIPIO = "Municipio";
        const string C_NUMEROCLIENTE = "Número de cliente";
        const string C_OFICIONOTIFICAICONGCRJ = "Oficio de notificación de la GCRJ";
        const string C_NUMCONTRATO = "No. De Contrato";
        const string C_NUMCREDITO = "Número de Crédito";
        const string C_FECHAFIRMEZAADJUDICACION = "Fecha de la Firmeza de Adjudicación";
        const string C_IMPORTEINDIVADJUDICACION = "Importe indiv de adjudicación";
        const string C_IMPORTEINDIVIMPLICADO = "Importe indiv aplicado";
        const string C_FECHACOMUNICADOGOT = "Fecha comunidado GOT";
        const string C_CUENTACONESCRITURA = "Con Escritura";
        const string C_CUENTACONESCRITURAFISICA = "Con Escritura física";
        const string C_CUENTACONLIBERTADGRAVAMEN = "¿Certif Libertad de Gravamen?";
        const string C_DOCUMENTODEJURIDICODEINEXISTENCIA = "Documento de Jurídico de inexistencia de procesos judiciales";
        const string C_CUENTACONPROCESOSJUDICIALES = "Cuenta con procesos Judiciales";
        const string C_FACTURA = "Factura";
        const string C_TOMAMATERIAL = "Toma Material";
        const string C_FOTOGRAFIAS = "Fotografías";
        const string C_NOADEUDO = "No adeudo";
        const string C_DOCUMENTOJURIDICOINEXISTENCIAPROCESOSJUDICIALES = "Documento de Jurídico de inexistencia de procesos judiciales";
        const string C_CUENTACONPROCESOSJUDICIALES2 = "Cuenta con procesos Judiciales";
        const string C_FECHAOFICIOSOLICITUDTRANSFERENCIAALINDEP = "Fecha de oficio de solicitud de transferencia al INDEP";
        const string C_FECHAVENTA = "Fecha de venta";
        const string C_IMPORTEDEVENTA = "Importe de venta";
        const string C_FECHAPOLIZADEBAJACONTABLE = "Fecha PÓLIZA de baja contable como Adj";
        const string C_AREARESPONSABLE = "Área responsable";
        const string C_ESTATUS = "Estatus";
        const string C_COMENTARIODELESTATUSDELBIEN = "Comentarios del estatus del bien";
        private readonly ILogger<ServicioBienesAdjudicados> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _archivoFuenteBienesAdjudicados;

        public ServicioBienesAdjudicados(ILogger<ServicioBienesAdjudicados> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoFuenteBienesAdjudicados = _configuration.GetValue<string>("archivoFuenteBienesAdjudicados")?? "";
        }

        private static int BuscaCelda(ExcelWorksheet hoja, int columnaIncial, string nombre)
        {
            string nuevoNombre = FNDExcelHelper.GetCellString(hoja.Cells[3, columnaIncial]).Trim();
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
            iCveBien = BuscaCelda(hoja, iCveBien, C_CVEBIEN);
            iExpedienteElectronico = BuscaCelda(hoja, iExpedienteElectronico, C_EXPEDIENTEELECTRONICO);
            iCoordinacionRegional = BuscaCelda(hoja, iCoordinacionRegional, C_COORDINACIONREGIONAL);
            iAcreditado = BuscaCelda(hoja, iAcreditado, C_ACREDITADO);
            iTipoDeAdjudicacion = BuscaCelda(hoja, iTipoDeAdjudicacion, C_TIPODEADJUDICACION);
            iTipoDeBien = BuscaCelda(hoja, iTipoDeBien, C_TIPODEBIEN);
            iDescripcionReducidaBien = BuscaCelda(hoja, iDescripcionReducidaBien, C_DESCRIPCIONREDUCIDABIEN);
            iDescripcionCompletaBien = BuscaCelda(hoja, iDescripcionCompletaBien, C_DESCRIPCIONCOMPLETABIEN);
            iEntidadFederativa = BuscaCelda(hoja, iEntidadFederativa, C_ENTIDADFEDERATIVA);
            iMunicipio = BuscaCelda(hoja, iMunicipio, C_MUNICIPIO);
            iNumeroCliente = BuscaCelda(hoja, iNumeroCliente, C_NUMEROCLIENTE);
            iOficioNotificaiconGCRJ = BuscaCelda(hoja, iOficioNotificaiconGCRJ, C_OFICIONOTIFICAICONGCRJ);
            iNumContrato = BuscaCelda(hoja, iNumContrato, C_NUMCONTRATO);
            iNumCredito = BuscaCelda(hoja, iNumCredito, C_NUMCREDITO);
            iFechaFirmezaAdjudicacion = BuscaCelda(hoja, iFechaFirmezaAdjudicacion, C_FECHAFIRMEZAADJUDICACION);
            iImporteIndivAdjudicacion = BuscaCelda(hoja, iImporteIndivAdjudicacion, C_IMPORTEINDIVADJUDICACION);
            iImporteIndivImplicado = BuscaCelda(hoja, iImporteIndivImplicado, C_IMPORTEINDIVIMPLICADO);
            iFechaComunicadoGOT = BuscaCelda(hoja, iFechaComunicadoGOT, C_FECHACOMUNICADOGOT);
            iCuentaConEscritura = BuscaCelda(hoja, iCuentaConEscritura, C_CUENTACONESCRITURA);
            iCuentaConEscrituraFisica = BuscaCelda(hoja, iCuentaConEscrituraFisica, C_CUENTACONESCRITURAFISICA);
            iCuentaConLibertadGravamen = BuscaCelda(hoja, iCuentaConLibertadGravamen, C_CUENTACONLIBERTADGRAVAMEN);
            iDocumentoDeJuridicoDeInexistencia = BuscaCelda(hoja, iDocumentoDeJuridicoDeInexistencia, C_DOCUMENTODEJURIDICODEINEXISTENCIA);
            iCuentaConProcesosJudiciales = BuscaCelda(hoja, iCuentaConProcesosJudiciales, C_CUENTACONPROCESOSJUDICIALES);
            iFactura = BuscaCelda(hoja, iFactura, C_FACTURA);
            iTomaMaterial = BuscaCelda(hoja, iTomaMaterial, C_TOMAMATERIAL);
            iFotografias = BuscaCelda(hoja, iFotografias, C_FOTOGRAFIAS);
            iNoAdeudo = BuscaCelda(hoja, iNoAdeudo, C_NOADEUDO);
            iDocumentoJuridicoInexistenciaProcesosJudiciales = BuscaCelda(hoja, iDocumentoJuridicoInexistenciaProcesosJudiciales, C_DOCUMENTOJURIDICOINEXISTENCIAPROCESOSJUDICIALES);
            iCuentaConProcesosJudiciales2 = BuscaCelda(hoja, iCuentaConProcesosJudiciales2, C_CUENTACONPROCESOSJUDICIALES2);
            iFechaOficioSolicitudTransferenciaAlIndep = BuscaCelda(hoja, iFechaOficioSolicitudTransferenciaAlIndep, C_FECHAOFICIOSOLICITUDTRANSFERENCIAALINDEP);
            iFechaVenta = BuscaCelda(hoja, iFechaVenta, C_FECHAVENTA);
            iImporteDeVenta = BuscaCelda(hoja, iImporteDeVenta, C_IMPORTEDEVENTA);
            iFechaPolizaDeBajaContable = BuscaCelda(hoja, iFechaPolizaDeBajaContable, C_FECHAPOLIZADEBAJACONTABLE);
            iAreaResponsable = BuscaCelda(hoja, iAreaResponsable, C_AREARESPONSABLE);
            iEstatus = BuscaCelda(hoja, iEstatus, C_ESTATUS);
            iComentarioDelEstatusDelBien = BuscaCelda(hoja, iComentarioDelEstatusDelBien, C_COMENTARIODELESTATUSDELBIEN);
        }

        public IEnumerable<FuenteBienesAdjudicados> ObtieneFuenteBienesAdjudicados(string archivo = "")
        {
            if (string.IsNullOrWhiteSpace(archivo))
            {
                archivo = _archivoFuenteBienesAdjudicados;
            }
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            IList<FuenteBienesAdjudicados> resultado = new List<FuenteBienesAdjudicados>();
            if (!File.Exists(archivo))
            {
                _logger.LogError("No se encontró el Archivo de Ministraciones de la Mesa a procesar\n{archivoMinistracionesMesa}", archivo);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivo, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets[0];
            ObtieneValoresCampos(hoja);
            int row = 4;
            bool salDelCiclo = false;
            while (!salDelCiclo)
            {
                var obj = new FuenteBienesAdjudicados();
                var celda = hoja.Cells[row, iCveBien];
                obj.CveBien = FNDExcelHelper.GetCellString(celda);
                if (string.IsNullOrEmpty(obj.CveBien))
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    obj.Id = row - 1;
                    celda = hoja.Cells[row, iCveBien];
                    obj.CveBien = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iExpedienteElectronico];
                    obj.ExpedienteElectronico = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCoordinacionRegional];
                    obj.CoordinacionRegional = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iAcreditado];
                    obj.Acreditado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTipoDeAdjudicacion];
                    obj.TipoDeAdjudicacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTipoDeBien];
                    obj.TipoDeBien = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iDescripcionReducidaBien];
                    obj.DescripcionReducidaBien = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iDescripcionCompletaBien];
                    obj.DescripcionCompletaBien = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iEntidadFederativa];
                    obj.EntidadFederativa = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iMunicipio];
                    obj.Municipio = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumeroCliente];
                    obj.NumeroCliente = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iOficioNotificaiconGCRJ];
                    obj.OficioNotificaiconGCRJ = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumContrato];
                    obj.NumContrato = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNumCredito];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFechaFirmezaAdjudicacion];
                    obj.FechaFirmezaAdjudicacion = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iImporteIndivAdjudicacion];
                    obj.ImporteIndivAdjudicacion = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iImporteIndivImplicado];
                    obj.ImporteIndivImplicado = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iFechaComunicadoGOT];
                    obj.FechaComunicadoGOT = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iCuentaConEscritura];
                    obj.CuentaConEscritura = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCuentaConEscrituraFisica];
                    obj.CuentaConEscrituraFisica = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCuentaConLibertadGravamen];
                    obj.CuentaConLibertadGravamen = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iDocumentoDeJuridicoDeInexistencia];
                    obj.DocumentoDeJuridicoDeInexistencia = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCuentaConProcesosJudiciales];
                    obj.CuentaConProcesosJudiciales = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFactura];
                    obj.Factura = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iTomaMaterial];
                    obj.TomaMaterial = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFotografias];
                    obj.Fotografias = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iNoAdeudo];
                    obj.NoAdeudo = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iDocumentoJuridicoInexistenciaProcesosJudiciales];
                    obj.DocumentoJuridicoInexistenciaProcesosJudiciales = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iCuentaConProcesosJudiciales2];
                    obj.CuentaConProcesosJudiciales2 = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iFechaOficioSolicitudTransferenciaAlIndep];
                    obj.FechaOficioSolicitudTransferenciaAlIndep = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iFechaVenta];
                    obj.FechaVenta = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iImporteDeVenta];
                    obj.ImporteDeVenta = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row, iFechaPolizaDeBajaContable];
                    obj.FechaPolizaDeBajaContable = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row, iAreaResponsable];
                    obj.AreaResponsable = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iEstatus];
                    obj.Estatus = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row, iComentarioDelEstatusDelBien];
                    obj.ComentarioDelEstatusDelBien = FNDExcelHelper.GetCellString(celda);
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
