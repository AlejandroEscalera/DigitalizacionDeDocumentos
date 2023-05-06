using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.BienesAdministrados;
using gob.fnd.Infraestructura.Negocio.CargaCsv.Mappings.Cancelados;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv;

public class AdministraBienesAdjudicadosService : IAdministraCargaBienesAdjudicados
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdministraABSaldosDiarioService> _logger;
    private readonly string _archivoBienesAdjudicados;

    public AdministraBienesAdjudicadosService(IConfiguration configuration, ILogger<AdministraABSaldosDiarioService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _archivoBienesAdjudicados = _configuration.GetValue<string>("archivoBienesAdjudicados") ?? "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados.csv";
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

    public IEnumerable<DetalleBienesAdjudicados> CargaBiensAdjudicados(string archivoBienesAdjudicados = "")
    {
        if (string.IsNullOrWhiteSpace(archivoBienesAdjudicados))
            archivoBienesAdjudicados = _archivoBienesAdjudicados;
        if (!File.Exists(archivoBienesAdjudicados))
            return new List<DetalleBienesAdjudicados>();

        IEnumerable<DetalleBienesAdjudicados> bienesAdjudicadosCarga;
        string nuevoContenidoArchivo = EliminaInconsistencias(archivoBienesAdjudicados);
        IEnumerable<DetalleBienesAdjudicados> resultado;
        using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(nuevoContenidoArchivo)))
        {
            var options = new CsvParserOptions(true, '|');
            var parser = new CsvParser<DetalleBienesAdjudicados>(options, new DetalleBienesAdjudicadosCsvMapping());
            bienesAdjudicadosCarga = parser.ReadFromStream(memoryStream, Encoding.UTF8)
                                  .Select(r => r.Result)
                                  .ToList();
        }
        #region Parsea el resultado
        resultado = bienesAdjudicadosCarga.Select(x => new DetalleBienesAdjudicados()
        {
            NumRegion = x.NumRegion,
            CatRegion = x.CatRegion,
            Entidad = x.Entidad,
            CveBien = x.CveBien,
            ExpedienteElectronico = x.ExpedienteElectronico,
            Acreditado = x.Acreditado,
            TipoDeBien = x.TipoDeBien,
            DescripcionReducidaBien = x.DescripcionReducidaBien,
            DescripcionCompletaBien = x.DescripcionCompletaBien,
            AreaResponsable = x.AreaResponsable,
            Estatus = x.Estatus,
            ImporteIndivAdjudicacion = x.ImporteIndivAdjudicacion,
            ImporteIndivImplicado = x.ImporteIndivImplicado,
            OficioNotificaiconGCRJ = x.OficioNotificaiconGCRJ,
            NumCredito = x.NumCredito,
            TipoAdjudicacion = x.TipoAdjudicacion
        }).ToList();
        #endregion

        _logger.LogInformation("Termino la carga de los bienes adjudicados.");
        return resultado;
    }

}
