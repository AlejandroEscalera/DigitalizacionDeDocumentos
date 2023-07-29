using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Excel.Juridico;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Juridico
{
    public class JuridicoService : IJuridico
    {
        private readonly ILogger<JuridicoService> _logger;
        private readonly IConfiguration _configuration;
        private int iRegion = 2;
        private int iAgencia = 3;
        private int iEstado = 4;
        private int iNombreDemandado = 5;
        private int iTipoCredito = 6;
        private int iNumContrato = 7;
        private int iNumCte = 8;
        private int iNumCredito = 9;
        private int iNumCreditos = 10;
        private int iFechaTranspasoJuridico = 11;
        private int iFechaTranspasoExterno = 12;
        private int iFechaDemanda = 13;
        private int iJuicio = 14;
        private int iCapitalDemandado = 15;
        private int iDlls = 16;
        private int iJuzgadoUbicacion = 17;
        private int iExpediente = 18;
        private int iClaveProcesal = 19;
        private int iEtapaProcesal = 20;
        private int iRppc = 21;
        private int iRan = 22;
        private int iRedCredAgricola = 23;
        private int iFechaRegistro = 24;
        private int iTipo = 25;
        private int iTipoGarantias = 26;
        private int iDescripcion = 27;
        private int iValorRegistro = 28;
        private int iFechaAvaluo = 29;
        private int iGradoPrelacion = 30;
        private int iCAval = 31;
        private int iSAval = 32;
        private int iInscripcion = 33;
        private int iClave = 34;
        private int iNumFolio = 35;
        private int iAbogadoResponsable = 36;
        private int iObservaciones = 37;
        private int iFechaUltimaActuacion = 38;
        private int iDescripcionActuacion = 39;
        private int iPiso = 40;
        private int iFonaga = 41;
        private int iPequenioProductor = 42;
        private int iFondoMutual = 43;
        private int iExpectativasRecuperacion = 45;

        const string C_REGION = "CR";
        const string C_AGENCIA = "ESTADO";
        const string C_ESTADO = "AGENCIA";
        const string C_NOMBRE_DEMANDADO = "NOMBRE DEL DEMANDADO";
        const string C_TIPO_CREDITO = "TIPO CRÉDITO";
        const string C_NUM_CONTRATO = "NÚMERO DE CONTRATO";
        const string C_NUM_CTE = "NÚMERO DE CLIENTE HOMOLOGADO";
        const string C_NUM_CREDITO = "NÚMERO DE CRÉDITO HOMOLOGADO";
        const string C_NUM_CREDITOS = "NÚMERO DE CRÉDITO";
        const string C_FECHA_TRANSPASO_JURIDICO = "FECHA DE TRASPASO A JURÍDICO";
        const string C_FECHA_TRANSPASO_EXTERNO = "FECHA DE TRASPASO A EXTERNO";
        const string C_FECHA_DEMANDA = "FECHA DEMANDA";
        const string C_JUICIO = "JUICIO";
        const string C_CAPITAL_DEMANDADO = "CAPITAL DEMANDADO (Suerte Principal)";
        const string C_DLLS = "DLLS";
        const string C_JUZGADO_UBICACION = "JUZGADO Y UBICACIÓN";
        const string C_EXPEDIENTE = "EXPEDIENTE";
        const string C_CLAVE_PROCESAL = "CLAVE  PROCESAL";
        const string C_ETAPA_PROCESAL = "ETAPA PROCESAL (PRIMERA-SEGUNDA-TERCERA)";
        const string C_RPPC = "R.P.P.C";
        const string C_RAN = "R.A.N.";
        const string C_RED_CRED_AGRICOLA = "Reg.Cred. Agrícola";
        const string C_FECHA_REGISTRO = "Fecha de Reg.";
        const string C_TIPO = "TIPO";
        const string C_TIPO_GARANTIAS = "TIPO DE GARANTIAS";
        const string C_DESCRIPCION = "DESCRIPCION";
        const string C_VALOR_REGISTRO = "Valor de Reg.";
        const string C_FECHA_AVALUO = "Fecha Avaluó";
        const string C_GRADO_PRELACION = "Grado de Prelación";
        const string C_C_AVAL = "C / AVAL";
        const string C_S_AVAL = "S / AVAL";
        const string C_INSCRIPCION = "INSCRIPCIÓN";
        const string C_CLAVE = "CLAVE";
        const string C_NUM_FOLIO = "NUM FOLIO";
        const string C_ABOGADO_RESPONSABLE = "ABOGADO/DESPACHO RESPONSABLE";
        const string C_OBSERVACIONES = "OBSERVACIONES";
        const string C_FECHA_ULTIMA_ACTUACION = "FECHA DE ÚLTIMA ACTUACIÓN";
        const string C_DESCRIPCION_ACTUACION = "DESCRIPCIÓN DE LA ACTUACIÓN";
        const string C_PISO = "PISO";
        const string C_FONAGA = "FONAGA";
        const string C_PEQUENIO_PRODUCTOR = "PEQUEÑO PRODUCTOR";
        const string C_FONDO_MUTUAL = "FONDO MUTUAL";
        const string C_EXPECTATIVAS_RECUPERACION = "EXPECTATIVAS DE RECUPERACION";

        private readonly string _archivoDeExpedientes;

        public JuridicoService(ILogger<JuridicoService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoDeExpedientes = _configuration.GetValue<string>("archivoExpedientesJuridicos") ?? "";
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
            iRegion = BuscaCelda(hoja, iRegion, C_REGION);
            iAgencia = BuscaCelda(hoja, iAgencia, C_AGENCIA);
            iEstado = BuscaCelda(hoja, iEstado, C_ESTADO);
            iNombreDemandado = BuscaCelda(hoja, iNombreDemandado, C_NOMBRE_DEMANDADO);
            iTipoCredito = BuscaCelda(hoja, iTipoCredito, C_TIPO_CREDITO);
            iNumContrato = BuscaCelda(hoja, iNumContrato, C_NUM_CONTRATO);
            iNumCte = BuscaCelda(hoja, iNumCte, C_NUM_CTE);
            iNumCredito = BuscaCelda(hoja, iNumCredito, C_NUM_CREDITO);
            iNumCreditos = BuscaCelda(hoja, iNumCreditos, C_NUM_CREDITOS);
            iFechaTranspasoJuridico = BuscaCelda(hoja, iFechaTranspasoJuridico, C_FECHA_TRANSPASO_JURIDICO);
            iFechaTranspasoExterno = BuscaCelda(hoja, iFechaTranspasoExterno, C_FECHA_TRANSPASO_EXTERNO);
            iFechaDemanda = BuscaCelda(hoja, iFechaDemanda, C_FECHA_DEMANDA);
            iJuicio = BuscaCelda(hoja, iJuicio, C_JUICIO);
            iCapitalDemandado = BuscaCelda(hoja, iCapitalDemandado, C_CAPITAL_DEMANDADO);
            iDlls = BuscaCelda(hoja, iDlls, C_DLLS);
            iJuzgadoUbicacion = BuscaCelda(hoja, iJuzgadoUbicacion, C_JUZGADO_UBICACION);
            iExpediente = BuscaCelda(hoja, iExpediente, C_EXPEDIENTE);
            iClaveProcesal = BuscaCelda(hoja, iClaveProcesal, C_CLAVE_PROCESAL);
            iEtapaProcesal = BuscaCelda(hoja, iEtapaProcesal, C_ETAPA_PROCESAL);
            iRppc = BuscaCelda(hoja, iRppc, C_RPPC);
            iRan = BuscaCelda(hoja, iRan, C_RAN);
            iRedCredAgricola = BuscaCelda(hoja, iRedCredAgricola, C_RED_CRED_AGRICOLA);
            iFechaRegistro = BuscaCelda(hoja, iFechaRegistro, C_FECHA_REGISTRO);
            iTipo = BuscaCelda(hoja, iTipo, C_TIPO);
            iTipoGarantias = BuscaCelda(hoja, iTipoGarantias, C_TIPO_GARANTIAS);
            iDescripcion = BuscaCelda(hoja, iDescripcion, C_DESCRIPCION);
            iValorRegistro = BuscaCelda(hoja, iValorRegistro, C_VALOR_REGISTRO);
            iFechaAvaluo = BuscaCelda(hoja, iFechaAvaluo, C_FECHA_AVALUO);
            iGradoPrelacion = BuscaCelda(hoja, iGradoPrelacion, C_GRADO_PRELACION);
            iCAval = BuscaCelda(hoja, iCAval, C_C_AVAL);
            iSAval = BuscaCelda(hoja, iSAval, C_S_AVAL);
            iInscripcion = BuscaCelda(hoja, iInscripcion, C_INSCRIPCION);
            iClave = BuscaCelda(hoja, iClave, C_CLAVE);
            iNumFolio = BuscaCelda(hoja, iNumFolio, C_NUM_FOLIO);
            iAbogadoResponsable = BuscaCelda(hoja, iAbogadoResponsable, C_ABOGADO_RESPONSABLE);
            iObservaciones = BuscaCelda(hoja, iObservaciones, C_OBSERVACIONES);
            iFechaUltimaActuacion = BuscaCelda(hoja, iFechaUltimaActuacion, C_FECHA_ULTIMA_ACTUACION);
            iDescripcionActuacion = BuscaCelda(hoja, iDescripcionActuacion, C_DESCRIPCION_ACTUACION);
            iPiso = BuscaCelda(hoja, iPiso, C_PISO);
            iFonaga = BuscaCelda(hoja, iFonaga, C_FONAGA);
            iPequenioProductor = BuscaCelda(hoja, iPequenioProductor, C_PEQUENIO_PRODUCTOR);
            iFondoMutual = BuscaCelda(hoja, iFondoMutual, C_FONDO_MUTUAL);
            iExpectativasRecuperacion = BuscaCelda(hoja, iExpectativasRecuperacion, C_EXPECTATIVAS_RECUPERACION);
        }

        public IEnumerable<ExpedienteJuridico> GetExpedientesJuridicos(string archivoDeExpedientes = "")
        {
            IList<ExpedienteJuridico> resultado = new List<ExpedienteJuridico>();
            _logger.LogInformation("Comenzamos a leer los expedientes jurídicos");

            if (string.IsNullOrWhiteSpace(archivoDeExpedientes))
            {
                archivoDeExpedientes = _archivoDeExpedientes;
            }
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoDeExpedientes))
            {
                _logger.LogError("No se encontró el Archivo de Expedientes Jurídicos\n{archivoDeExpedientes}", archivoDeExpedientes);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoDeExpedientes, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            // await package.LoadAsync(fs);
            package.Load(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = package.Workbook.Worksheets.FirstOrDefault();
            if (hoja == null)
                return resultado;
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            while (!salDelCiclo)
            {
                var obj = new ExpedienteJuridico();
                var celda = hoja.Cells[row,iRegion];
                obj.CatRegion = FNDExcelHelper.GetCellString(celda);

                if (string.IsNullOrEmpty(obj.CatRegion))
                {
                    salDelCiclo = true;
                    //break;
                }
                else
                {
                    celda = hoja.Cells[row,iAgencia];
                    obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iEstado];
                    obj.Estado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNombreDemandado];
                    obj.NombreDemandado = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iTipoCredito];
                    obj.TipoCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNumContrato];
                    obj.NumContrato = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNumCte];
                    obj.NumCte = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNumCredito];
                    obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNumCreditos];
                    obj.NumCreditos = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iFechaTranspasoJuridico];
                    obj.FechaTranspasoJuridico = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iFechaTranspasoExterno];
                    obj.FechaTranspasoExterno = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iFechaDemanda];
                    obj.FechaDemanda = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iJuicio];
                    obj.Juicio = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iCapitalDemandado];
                    obj.CapitalDemandado = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row,iDlls];
                    obj.Dlls = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row,iJuzgadoUbicacion];
                    obj.JuzgadoUbicacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iExpediente];
                    obj.Expediente = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iClaveProcesal];
                    obj.ClaveProcesal = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iEtapaProcesal];
                    obj.EtapaProcesal = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iRppc];
                    obj.Rppc = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iRan];
                    obj.Ran = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iRedCredAgricola];
                    obj.RedCredAgricola = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iFechaRegistro];
                    obj.FechaRegistro = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iTipo];
                    obj.Tipo = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iTipoGarantias];
                    obj.TipoGarantias = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iDescripcion];
                    obj.Descripcion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iValorRegistro];
                    obj.ValorRegistro = FNDExcelHelper.GetCellDecimal(celda);
                    celda = hoja.Cells[row,iFechaAvaluo];
                    obj.FechaAvaluo = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iGradoPrelacion];
                    obj.GradoPrelacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iCAval];
                    obj.CAval = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iSAval];
                    obj.SAval = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iInscripcion];
                    obj.Inscripcion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iClave];
                    obj.Clave = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iNumFolio];
                    obj.NumFolio = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iAbogadoResponsable];
                    obj.AbogadoResponsable = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iObservaciones];
                    obj.Observaciones = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iFechaUltimaActuacion];
                    obj.FechaUltimaActuacion = FNDExcelHelper.GetCellDateTime(celda);
                    celda = hoja.Cells[row,iDescripcionActuacion];
                    obj.DescripcionActuacion = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iPiso];
                    obj.Piso = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iFonaga];
                    obj.Fonaga = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iPequenioProductor];
                    obj.PequenioProductor = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iFondoMutual];
                    obj.FondoMutual = FNDExcelHelper.GetCellString(celda);
                    celda = hoja.Cells[row,iExpectativasRecuperacion];
                    obj.ExpectativasRecuperacion = FNDExcelHelper.GetCellString(celda); 
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

        public async Task<IEnumerable<ExpedienteJuridico>> GetExpedientesJuridicosAsync(string archivoDeExpedientes = "")
        {
            IList<ExpedienteJuridico> resultado = new List<ExpedienteJuridico>();
            _logger.LogInformation("Comenzamos a leer los expedientes jurídicos");

            if (string.IsNullOrWhiteSpace(archivoDeExpedientes))
            {
                archivoDeExpedientes = _archivoDeExpedientes;
            }
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (!File.Exists(archivoDeExpedientes))
            {
                _logger.LogError("No se encontró el Archivo de Expedientes Jurídicos\n{archivoDeExpedientes}", archivoDeExpedientes);
                return resultado;
            }

            #region Abrimos el excel de ABSaldos y Extraemos la información de todos los campos
            FileStream fs = new(archivoDeExpedientes, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var package = new ExcelPackage();
            // await package.LoadAsync(fs);
            await package.LoadAsync(fs);
            if (package.Workbook.Worksheets.Count == 0)
                return resultado;
            var hoja = await Task.Run(() => { return package.Workbook.Worksheets.FirstOrDefault(); });
            if (hoja == null)
                return resultado;
            ObtieneValoresCampos(hoja);
            int row = 2;
            bool salDelCiclo = false;
            await Task.Run(() => {
                while (!salDelCiclo)
                {
                    var obj = new ExpedienteJuridico();
                    var celda = hoja.Cells[row, iRegion];
                    obj.CatRegion = FNDExcelHelper.GetCellString(celda);

                    if (string.IsNullOrEmpty(obj.CatRegion))
                    {
                        salDelCiclo = true;
                        //break;
                    }
                    else
                    {
                        celda = hoja.Cells[row, iAgencia];
                        obj.CatAgencia = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iEstado];
                        obj.Estado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNombreDemandado];
                        obj.NombreDemandado = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iTipoCredito];
                        obj.TipoCredito = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumContrato];
                        obj.NumContrato = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCte];
                        obj.NumCte = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCredito];
                        obj.NumCredito = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumCreditos];
                        obj.NumCreditos = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaTranspasoJuridico];
                        obj.FechaTranspasoJuridico = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iFechaTranspasoExterno];
                        obj.FechaTranspasoExterno = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iFechaDemanda];
                        obj.FechaDemanda = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iJuicio];
                        obj.Juicio = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCapitalDemandado];
                        obj.CapitalDemandado = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iDlls];
                        obj.Dlls = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iJuzgadoUbicacion];
                        obj.JuzgadoUbicacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iExpediente];
                        obj.Expediente = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iClaveProcesal];
                        obj.ClaveProcesal = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iEtapaProcesal];
                        obj.EtapaProcesal = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iRppc];
                        obj.Rppc = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iRan];
                        obj.Ran = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iRedCredAgricola];
                        obj.RedCredAgricola = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaRegistro];
                        obj.FechaRegistro = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iTipo];
                        obj.Tipo = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iTipoGarantias];
                        obj.TipoGarantias = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iDescripcion];
                        obj.Descripcion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iValorRegistro];
                        obj.ValorRegistro = FNDExcelHelper.GetCellDecimal(celda);
                        celda = hoja.Cells[row, iFechaAvaluo];
                        obj.FechaAvaluo = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iGradoPrelacion];
                        obj.GradoPrelacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iCAval];
                        obj.CAval = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iSAval];
                        obj.SAval = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iInscripcion];
                        obj.Inscripcion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iClave];
                        obj.Clave = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iNumFolio];
                        obj.NumFolio = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iAbogadoResponsable];
                        obj.AbogadoResponsable = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iObservaciones];
                        obj.Observaciones = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFechaUltimaActuacion];
                        obj.FechaUltimaActuacion = FNDExcelHelper.GetCellDateTime(celda);
                        celda = hoja.Cells[row, iDescripcionActuacion];
                        obj.DescripcionActuacion = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iPiso];
                        obj.Piso = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFonaga];
                        obj.Fonaga = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iPequenioProductor];
                        obj.PequenioProductor = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iFondoMutual];
                        obj.FondoMutual = FNDExcelHelper.GetCellString(celda);
                        celda = hoja.Cells[row, iExpectativasRecuperacion];
                        obj.ExpectativasRecuperacion = FNDExcelHelper.GetCellString(celda);
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
