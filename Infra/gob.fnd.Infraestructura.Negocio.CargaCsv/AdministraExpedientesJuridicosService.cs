using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Expedientes;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Juridico;
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
    public class AdministraExpedientesJuridicosService : IAdministraExpedientesJuridicos
    {
        private readonly ILogger<AdministraExpedientesJuridicosService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _archivoExpedientesJuridicos;
        private readonly string _formatDateTime;

        public AdministraExpedientesJuridicosService(ILogger<AdministraExpedientesJuridicosService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _archivoExpedientesJuridicos = _configuration.GetValue<string>("archivoExpedientesJuridicosCsv") ?? string.Empty;
            _formatDateTime = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
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

        private static string EliminaInconsistencias(string archivoABSaldosCompleta, bool otroEncoding = true)
        {
            Encoding cualVoyAUsar = Encoding.UTF8;
            if (otroEncoding)
                cualVoyAUsar = Encoding.GetEncoding("ISO-8859-1");
            string contenidoArchivo = File.ReadAllText(archivoABSaldosCompleta, cualVoyAUsar);
            contenidoArchivo = contenidoArchivo.Replace("\"", "~");
            //contenidoArchivo = contenidoArchivo.Replace("\n", "°");
            //contenidoArchivo = contenidoArchivo.Replace(",", "^");
            string nuevoContenidoArchivo = contenidoArchivo.Replace("| |", "||");
            return nuevoContenidoArchivo;
        }

        public IEnumerable<ExpedienteJuridico> CargaExpedientesJuridicos(string archivoExpedientesJuridicos = "")
        {
            if (string.IsNullOrEmpty(archivoExpedientesJuridicos))
                archivoExpedientesJuridicos = _archivoExpedientesJuridicos;
            IList<ExpedienteJuridico> resultado = new List<ExpedienteJuridico>();
            if (!File.Exists(archivoExpedientesJuridicos))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoExpedientesJuridicos);
            IEnumerable<ExpedienteJuridicoCarga> expedientesJuridicos;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ExpedienteJuridicoCarga>(options, new ExpedienteJuridicoCsvMapping());
                expedientesJuridicos = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = expedientesJuridicos.Select(x => new ExpedienteJuridico()
            {
                Region = x.Region,
                CatRegion = x.CatRegion,
                Agencia = x.Agencia,
                CatAgencia = x.CatAgencia,
                Estado = x.Estado,
                NombreDemandado = x.NombreDemandado,
                TipoCredito = x.TipoCredito,
                NumContrato = x.NumContrato,
                NumCte = x.NumCte,
                NumCredito = x.NumCredito,
                NumCreditos = (x.NumCreditos ??"").Replace("¬","\n"),
                FechaTranspasoJuridico = GetDateTimeFromString(x.FechaTranspasoJuridico),
                FechaTranspasoExterno = GetDateTimeFromString(x.FechaTranspasoExterno),
                FechaDemanda = GetDateTimeFromString(x.FechaDemanda),
                Juicio = x.Juicio,
                CapitalDemandado = x.CapitalDemandado,
                Dlls = x.Dlls,
                JuzgadoUbicacion = (x.JuzgadoUbicacion ?? "").Replace("¬", "\n"),
                Expediente = x.Expediente,
                ClaveProcesal = x.ClaveProcesal,
                EtapaProcesal = x.EtapaProcesal,
                Rppc = x.Rppc,
                Ran = x.Ran,
                RedCredAgricola = x.RedCredAgricola,
                FechaRegistro = GetDateTimeFromString(x.FechaRegistro),
                Tipo = x.Tipo,
                TipoGarantias = x.TipoGarantias,
                Descripcion = (x.Descripcion ?? "").Replace("¬", "\n"),
                ValorRegistro = x.ValorRegistro,
                FechaAvaluo = GetDateTimeFromString(x.FechaAvaluo),
                GradoPrelacion = x.GradoPrelacion,
                CAval = x.CAval,
                SAval = x.SAval,
                Inscripcion = x.Inscripcion,
                Clave = x.Clave,
                NumFolio = x.NumFolio,
                AbogadoResponsable = x.AbogadoResponsable,
                Observaciones = (x.Observaciones ?? "").Replace("¬", "\n"),
                FechaUltimaActuacion = GetDateTimeFromString(x.FechaUltimaActuacion),
                DescripcionActuacion = (x.DescripcionActuacion ?? "").Replace("¬", "\n"),
                Piso = x.Piso,
                Fonaga = x.Fonaga,
                PequenioProductor = x.PequenioProductor,
                FondoMutual = x.FondoMutual,
                ExpectativasRecuperacion = x.ExpectativasRecuperacion,
                TieneImagenDirecta = (x.TieneImagenDirecta??"").Equals("True", StringComparison.InvariantCultureIgnoreCase),
                TieneImagenIndirecta = (x.TieneImagenIndirecta ?? "").Equals("True", StringComparison.InvariantCultureIgnoreCase),
                TieneImagenExpediente = (x.TieneImagenExpediente ?? "").Equals("True", StringComparison.InvariantCultureIgnoreCase)
            }).ToList();
            #endregion
            
            _logger.LogInformation("Termino la carga de los expedientes jurídicos.");
            return resultado;
        }
    }
}
