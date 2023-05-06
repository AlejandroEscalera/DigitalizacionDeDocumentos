using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.ABSaldos;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Imagenes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraCargaConsultaService : IAdministraCargaConsulta
    {
        private readonly string _archivoDeExpedienteDeConsulta;
        private readonly string _archivoArchivoImagenCorta;
        private readonly string _archivosImagenesBienesAdjudicadosCorta;
        private readonly string _formatDateTime;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdministraCargaConsultaService> _logger;

        public AdministraCargaConsultaService(IConfiguration configuration, ILogger<AdministraCargaConsultaService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _archivoDeExpedienteDeConsulta = _configuration.GetValue<string>("archivoExpedientesConsulta") ?? "";
            _archivoArchivoImagenCorta = _configuration.GetValue<string>("archivoImagenCorta") ?? "";
            _archivosImagenesBienesAdjudicadosCorta = _configuration.GetValue<string>("archivosImagenesBienesAdjudicadosCorta") ?? "";
            _formatDateTime = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        private static string EliminaInconsistencias(string archivoABSaldosCompleta, bool otroEncoding = true)
        {
            Encoding cualVoyAUsar = Encoding.UTF8;
            if (otroEncoding)
                cualVoyAUsar = Encoding.GetEncoding("ISO-8859-1");
            string contenidoArchivo = File.ReadAllText(archivoABSaldosCompleta, cualVoyAUsar);
            contenidoArchivo = contenidoArchivo.Replace("\"", "~");
            string nuevoContenidoArchivo = contenidoArchivo.Replace("| |", "||");
            return nuevoContenidoArchivo;
        }

        public IEnumerable<ArchivoImagenCorta> CargaArchivoImagenCorta(string archivoArchivoImagenCorta = "")
        {
            if (string.IsNullOrEmpty(archivoArchivoImagenCorta))
                archivoArchivoImagenCorta = _archivoArchivoImagenCorta;
            IList<ArchivoImagenCorta> resultado = new List<ArchivoImagenCorta>();
            if (!File.Exists(archivoArchivoImagenCorta))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoArchivoImagenCorta);
            IEnumerable<ArchivoImagenCorta> imagenesEnCorto;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ArchivoImagenCorta>(options, new ArchivoImagenCortaCsvMapping());
                imagenesEnCorto = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            ((List<ArchivoImagenCorta>)resultado).AddRange(imagenesEnCorto);
            _logger.LogInformation("Termino la carga de los expedientes de consulta.");
            return resultado;
        }

        private DateTime? GetDateTimeFromString(string? fechaAConvertir)
        {
            if (string.IsNullOrEmpty(fechaAConvertir) || string.IsNullOrWhiteSpace(fechaAConvertir))
            {
                return null;
            }
            fechaAConvertir = fechaAConvertir.Trim();
            if (!DateTime.TryParseExact(fechaAConvertir, _formatDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado))
            {
                return null;
            }
            return resultado;
        }
        public IEnumerable<ExpedienteDeConsulta> CargaExpedienteDeConsulta(string archivoDeExpedienteDeConsulta = "")
        {
            if (string.IsNullOrEmpty(archivoDeExpedienteDeConsulta))
                archivoDeExpedienteDeConsulta = _archivoDeExpedienteDeConsulta;
            IList<ExpedienteDeConsulta> resultado = new List<ExpedienteDeConsulta>();
            if (!File.Exists(archivoDeExpedienteDeConsulta))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoDeExpedienteDeConsulta);
            IEnumerable<ExpedienteDeConsultaCarga> expedienteDeConsulta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ExpedienteDeConsultaCarga>(options, new ExpedienteConsultaCsvMapping());
                expedienteDeConsulta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = expedienteDeConsulta.Select(x => new ExpedienteDeConsulta()
            {
                NumCredito = x.NumCredito,
                NumCreditoCancelado = x.NumCreditoCancelado,
                EsSaldosActivo = x.EsSaldosActivo,
                EsCancelado = x.EsCancelado,
                EsOrigenDelDr = x.EsOrigenDelDr,
                EsCanceladoDelDr = x.EsCanceladoDelDr,
                EsCastigado = x.EsCastigado,
                TieneArqueo  = x.TieneArqueo,
                Acreditado = x.Acreditado,
                FechaApertura = GetDateTimeFromString(x.FechaApertura),
                FechaCancelacion = GetDateTimeFromString(x.FechaCancelacion),
                Castigo = x.Castigo,
                NumProducto = x.NumProducto,
                CatProducto = x.CatProducto,
                TipoDeCredito = x.TipoDeCredito,
                Ejecutivo = x.Ejecutivo,
                Analista = x.Analista,
                FechaInicioMinistracion = GetDateTimeFromString(x.FechaInicioMinistracion),
                FechaSolicitud = GetDateTimeFromString(x.FechaSolicitud),
                MontoCredito = x.MontoCredito,
                InterCont = x.InterCont,
                Region = x.Region,
                Agencia = x.Agencia,
                CatRegion = x.CatRegion,
                CatAgencia = x.CatAgencia,
                EsCreditoAReportar = x.EsCreditoAReportar,
                StatusImpago = x.StatusImpago,
                StatusCarteraVencida = x.StatusCarteraVencida,
                StatusCarteraVigente = x.StatusCarteraVigente,
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta,
                SldoTotContval = x.SldoTotContval,
                NumCliente = x.NumCliente
            }).ToList();
            #endregion

            // ((List<ExpedienteDeConsulta>)resultado).AddRange(expedienteDeConsulta);
            _logger.LogInformation("Termino la carga de los expedientes de consulta.");
            return resultado;
        }

        public IEnumerable<ArchivoImagenBienesAdjudicadosCorta> CargaArchivoImagenBienesAdjudicadosCorta(string archivoImagenBienesAdjudicadosCorta = "")
        {
            if (string.IsNullOrEmpty(archivoImagenBienesAdjudicadosCorta))
                archivoImagenBienesAdjudicadosCorta = _archivosImagenesBienesAdjudicadosCorta;
            IList<ArchivoImagenBienesAdjudicadosCorta> resultado = new List<ArchivoImagenBienesAdjudicadosCorta>();
            if (!File.Exists(archivoImagenBienesAdjudicadosCorta))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoImagenBienesAdjudicadosCorta);
            IEnumerable<ArchivoImagenBienesAdjudicadosCorta> imagenesEnCorto;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ArchivoImagenBienesAdjudicadosCorta>(options, new ArchivoImagenBienesAdjudicadosCortaCsvMapping());
                imagenesEnCorto = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            ((List<ArchivoImagenBienesAdjudicadosCorta>)resultado).AddRange(imagenesEnCorto);
            _logger.LogInformation("Termino la carga de los expedientes de consulta.");
            return resultado;
        }
    }
}
