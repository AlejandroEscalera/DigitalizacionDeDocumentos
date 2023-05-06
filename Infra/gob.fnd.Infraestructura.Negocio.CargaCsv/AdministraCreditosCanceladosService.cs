using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Cancelados;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraCreditosCanceladosService : IAdministraCargaCreditosCancelados
    {
        private readonly ILogger<AdministraCreditosCanceladosService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _archivoDeExpedientesCancelados;
        private readonly string _formatDateTime;

        public AdministraCreditosCanceladosService(ILogger<AdministraCreditosCanceladosService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoDeExpedientesCancelados = _configuration.GetValue<string>("archivosCreditosCancelados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\HistoricoCancelados.csv";
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

        public IEnumerable<CreditosCanceladosAplicacion> CargaExpedienteCancelados(string archivoDeExpedientesCancelados = "")
        {
            if (string.IsNullOrEmpty(archivoDeExpedientesCancelados))
                archivoDeExpedientesCancelados = _archivoDeExpedientesCancelados;
            IList<CreditosCanceladosAplicacion> resultado = new List<CreditosCanceladosAplicacion>();
            if (!File.Exists(archivoDeExpedientesCancelados))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoDeExpedientesCancelados);
            IEnumerable<CreditosCanceladosAplicacionCarga> creditosCanceladosAplicacionCarga;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<CreditosCanceladosAplicacionCarga>(options, new CreditosCanceladosAplicacionCsvMapping());
                creditosCanceladosAplicacionCarga = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = creditosCanceladosAplicacionCarga.Select(x => new CreditosCanceladosAplicacion()
            {
                NumCreditoCancelado = x.NumCreditoCancelado,
                NumCreditoOrigen = x.NumCreditoOrigen,
                NumRegion = x.NumRegion,
                CatRegion = x.CatRegion,
                Agencia = x.Agencia,
                CatAgencia = x.CatAgencia,
                NumCliente = x.NumCliente,
                Acreditado = x.Acreditado,
                TipoPersona = x.TipoPersona,
                AnioOriginacion = x.AnioOriginacion,
                PisoCredito = x.PisoCredito,
                Portafolio = x.Portafolio,
                Concepto = x.Concepto,
                FechaCancelacion = GetDateTimeFromString(x.FechaCancelacion),
                FechaPrimeraDispersion = GetDateTimeFromString(x.FechaPrimeraDispersion),
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta
            }).ToList();
            #endregion

            // ((List<ExpedienteDeConsulta>)resultado).AddRange(expedienteDeConsulta);
            _logger.LogInformation("Termino la carga de los expedientes cancelados.");
            return resultado;
        }
    }
}
