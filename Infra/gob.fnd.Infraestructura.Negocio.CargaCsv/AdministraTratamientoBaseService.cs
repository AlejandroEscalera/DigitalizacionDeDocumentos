using gob.fnd.Dominio.Digitalizacion.Entidades.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Liquidaciones;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Tratamientos;
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
    public class AdministraTratamientoBaseService : IAdministraTratamientoBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdministraCargaLiquidacionesService> _logger;
        private readonly string _archivoTratamientoBase;

        public AdministraTratamientoBaseService(IConfiguration configuration, ILogger<AdministraCargaLiquidacionesService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _archivoTratamientoBase = _configuration.GetValue<string>("archivoTratamientoBase") ?? @"C:\202302 Digitalizacion\2. Saldos procesados\Tratamientos\TratamientosBase.csv";
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

        public IEnumerable<ConversionTratamientos> GetConversionTratamientosCsv(string archivoTratamientoBase)
        {
            if (string.IsNullOrEmpty(archivoTratamientoBase))
                archivoTratamientoBase = _archivoTratamientoBase;
            IList<ConversionTratamientos> resultado = new List<ConversionTratamientos>();
            if (!File.Exists(archivoTratamientoBase))
                return resultado;
            string nuevoContenidoArchivo = EliminaInconsistencias(archivoTratamientoBase);
            IEnumerable<ConversionTratamientos> expedienteDeConsulta;
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
            {
                var options = new CsvParserOptions(true, '|');
                var parser = new CsvParser<ConversionTratamientos>(options, new ConversionTratamientosCsvMapping());
                expedienteDeConsulta = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                      .Select(r => r.Result)
                                      .ToList();
            }
            #region Parsea el resultado
            resultado = expedienteDeConsulta.Select(x => new ConversionTratamientos()
            {
                Id = x.Id,
                NumCte = x.NumCte,
                Origen = x.Origen,
                Destino = x.Destino
            }).ToList();
            #endregion

            // ((List<ExpedienteDeConsulta>)resultado).AddRange(expedienteDeConsulta);
            _logger.LogInformation("Termino la carga de los tratamientos base.");
            return resultado;
        }
    }
}
