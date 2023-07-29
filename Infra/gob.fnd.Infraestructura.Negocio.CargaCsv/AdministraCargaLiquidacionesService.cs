using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Liquidaciones;
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
    public class AdministraCargaLiquidacionesService : IAdministraCargaLiquidaciones
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdministraCargaLiquidacionesService> _logger;
        private readonly string _archivoLiquidaciones;
        private readonly string _formatDateTime;

        public AdministraCargaLiquidacionesService(IConfiguration configuration, ILogger<AdministraCargaLiquidacionesService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _archivoLiquidaciones = _configuration.GetValue<string>("archivoLiquidaciones") ?? "";
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

        public IEnumerable<ColocacionConPagos> CargaLiquidacionesCompleta(string archivoLiquidaciones = "")
        {
            if (string.IsNullOrEmpty(archivoLiquidaciones))
                archivoLiquidaciones = _archivoLiquidaciones;
            IList<ColocacionConPagos> resultado = new List<ColocacionConPagos>();
            if (!File.Exists(archivoLiquidaciones))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoLiquidaciones);
            IEnumerable<ColocacionConPagosCarga> expedienteDeConsulta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ColocacionConPagosCarga>(options, new ColocacionConPagosCsvMapping());
                expedienteDeConsulta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = expedienteDeConsulta.Select(x => new ColocacionConPagos()
            {
                Agencia = x.Agencia,
                CatAgencia = x.CatAgencia,
                NumCte = x.NumCte,
                Acreditado = x.Acreditado,
                NumCredito = x.NumCredito,
                FechaApertura = GetDateTimeFromString(x.FechaApertura),
                FechaVencim = GetDateTimeFromString(x.FechaVencim),
                MontoOtorgado = x.MontoOtorgado,
                CatRegionMigrado = x.CatRegionMigrado,
                CatEstadoMigrado = x.CatEstadoMigrado,
                CatAgenciaMigrado = x.CatAgenciaMigrado,
                Ministraciones  = x.Ministraciones,
                FecPrimMinistra = GetDateTimeFromString(x.FecPrimMinistra),
                FecUltimaMinistra = GetDateTimeFromString(x.FecUltimaMinistra),
                MontoMinistrado = x.MontoMinistrado,
                Reestructura = x.Reestructura,
                Cancelacion = x.Cancelacion,
                PagoCapital = x.PagoCapital??0M,
                PagoInteres = x.PagoInteres ?? 0M,
                PagoMoratorios = x.PagoMoratorios ?? 0M,
                PagoTotal = x.PagoTotal ?? 0M,
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta
            }).ToList();
            #endregion

            // ((List<ExpedienteDeConsulta>)resultado).AddRange(expedienteDeConsulta);
            _logger.LogInformation("Termino la carga de los expedientes liquidados.");
            return resultado;
        }
    }
}
