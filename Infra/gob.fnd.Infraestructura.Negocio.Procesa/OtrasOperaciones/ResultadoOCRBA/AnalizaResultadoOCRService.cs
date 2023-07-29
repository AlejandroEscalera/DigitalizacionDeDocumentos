using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ocr;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA
{
    public class AnalizaResultadoOCRService : IAnalizaResultadoOCR
    {
        private readonly ILogger<AnalizaResultadoOCRService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly ICreaArchivoCsv _creaArchivoCsv;
        // private readonly IServicioMinistracionesMesa _ministracionesMesaService;
        private readonly IAdministraCargaConsulta _administraCargaConsultaService;
        private readonly string _archivoExpedientesConsulta;

        public AnalizaResultadoOCRService(ILogger<AnalizaResultadoOCRService> logger, IConfiguration configuration, IServicioImagenes servicioImagenes, ICreaArchivoCsv creaArchivoCsv,
            // IServicioMinistracionesMesa ministracionesMesaService,
            IAdministraCargaConsulta administraCargaConsultaService
            )
        {
            _logger = logger;
            _configuration = configuration;
            _servicioImagenes = servicioImagenes;
            _creaArchivoCsv = creaArchivoCsv;
            // _ministracionesMesaService = ministracionesMesaService;
            _administraCargaConsultaService = administraCargaConsultaService;
            _archivoExpedientesConsulta = _configuration.GetValue<string>("archivoExpedientesConsulta") ?? "";
        }
        public void AnalizaResultado()
        {
            IList<DetalleInformacionCreditosOcr> resultado = new List<DetalleInformacionCreditosOcr>();
            IList<DetalleInformacionCreditosOcr> resultadoUnico = new List<DetalleInformacionCreditosOcr>();
            IList<ArchivosImagenes> todasLasImagenes = new List<ArchivosImagenes>();
            IEnumerable<ArchivosImagenes> imagenes = _servicioImagenes.CargaImagenesTratadas(@"C:\202302 Digitalizacion\7. Descargas\AO\112\001\IMG-AO-112-001.xlsx");
            
            var imagenesAdicionales = _servicioImagenes.CargaImagenesTratadas(@"C:\202302 Digitalizacion\7. Descargas\AO\112\002\IMG-AO-112-002.xlsx");
             
            ((List<ArchivosImagenes>)todasLasImagenes).AddRange(imagenes);
            ((List < ArchivosImagenes >) todasLasImagenes).AddRange(imagenesAdicionales);

            ObtieneTodasLasCoincidencias(resultado, todasLasImagenes);
            ObtieneRegistrosUnicos(resultado, resultadoUnico);

            var expedientes = _administraCargaConsultaService.CargaExpedienteDeConsulta(_archivoExpedientesConsulta);
            var cruceInformacion = (from ru in resultadoUnico
                                    join mesa in expedientes
                                    on QuitaCastigo(ru.NumeroDeCredito) equals QuitaCastigo(mesa.NumCredito)
                                    orderby mesa.NumCredito descending
                                    select new { Expedientes = ru, Creditos = mesa }).ToList();
            foreach (var item in cruceInformacion)
            {
                if (item.Creditos is not null)
                {
                    item.Expedientes.Region = item.Creditos.Region;
                    item.Expedientes.CatRegion = item.Creditos.CatRegion;
                    item.Expedientes.Agencia = item.Creditos.Agencia;
                    item.Expedientes.CatAgencia = item.Creditos.CatAgencia;
                    item.Expedientes.Acreditado = item.Creditos.Acreditado;
                    item.Expedientes.NumCliente = item.Creditos.NumCliente;
                }
            }

            IEnumerable< DetalleInformacionCreditosOcr> unicosConCruce = cruceInformacion.Select(x=>x.Expedientes).ToList();

            string archivoUnicosConCruce = "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\cruceCreditosExpedientes.csv";
            if (File.Exists(archivoUnicosConCruce))
                File.Delete(archivoUnicosConCruce);
            _creaArchivoCsv.CreaArchivoCsv(archivoUnicosConCruce, unicosConCruce);

        }

        private static string QuitaCastigo(string? origen)
        {
            if (origen is null)
                return "000000000000000000";

            if (origen.Length < 18)
                return "000000000000000000";
            string digito = origen.Substring(3, 1);
            string digito2 = origen.Substring(4, 1);
            string nuevoOrigen = origen;
            if ((digito == "1" || digito == "9") && (digito2 != "0"))
            {
                nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
                // _logger.LogInformation("numero de crédito {numcredito}", nuevoOrigen);
            }
            return nuevoOrigen;
        }
        private void ObtieneTodasLasCoincidencias(IList<DetalleInformacionCreditosOcr> resultado, IList<ArchivosImagenes> todasLasImagenes)
        {
            foreach (var img in todasLasImagenes)
            {
                DetalleInformacionCreditosOcr detalleInformacionCreditosOcr = new();
                string path = img.RutaDeGuardado ?? "";

                detalleInformacionCreditosOcr.NumeroDeExpediente = ObtieneNumeroExpediente(path);
                detalleInformacionCreditosOcr.RutaDeGuardado = path;
                ObtieneCadenaCreditoYDetallle(img, ref detalleInformacionCreditosOcr);
                if (!string.IsNullOrEmpty(detalleInformacionCreditosOcr.NumeroDeCredito))
                    if (CumpleCriterio(detalleInformacionCreditosOcr))
                        resultado.Add(detalleInformacionCreditosOcr);
            }

            // Falta guardar el resultado ahorita como está
            string archivoDetalle = "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\cruceCreditos.csv";
            if (File.Exists(archivoDetalle))
                File.Delete(archivoDetalle);
            _creaArchivoCsv.CreaArchivoCsv(archivoDetalle, resultado);
        }

        private void ObtieneRegistrosUnicos(IList<DetalleInformacionCreditosOcr> resultado, IList<DetalleInformacionCreditosOcr> resultadoUnico)
        {
            // string numExpediente = "00000000";
            int cantidadDeExpedientes = 0;
            foreach (var registro in resultado.OrderBy(x => x.NumeroDeExpediente).Select(z => z.NumeroDeExpediente).Distinct().ToList())
            {
                string nuevoExpediente = registro ?? "00000000";
                if (!nuevoExpediente.Equals("00000000", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Obtengo el detalle
                    var detalleRegistros = resultado.Where(x => (x.NumeroDeExpediente ?? "").Equals(nuevoExpediente, StringComparison.InvariantCultureIgnoreCase)).
                        Select(r => r.NumeroDeCredito).Distinct().ToList().
                        Select(alfa => new DetalleInformacionCreditosOcr()
                        {
                            NumeroDeExpediente = nuevoExpediente,
                            NumeroDeCredito = alfa
                        }
                        ).Distinct().ToList();
                    if (detalleRegistros.Count > 1)
                    {
                        _logger.LogInformation("En el expediente {numExpediente} se encontraron {numCreditos} créditos.", nuevoExpediente, detalleRegistros.Count);
                    }
                    ((List<DetalleInformacionCreditosOcr>)resultadoUnico).AddRange(detalleRegistros);
                    cantidadDeExpedientes++;
                }
                // 
            }

            _logger.LogInformation("Se encontró la información de {numExpedientesEncontrados} expedientes y {numCreditos} créditos", cantidadDeExpedientes, resultadoUnico.Count);

            // Falta guardar el resultado ahorita como está
            string archivoUnico = "C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\cruceCreditosUnicos.csv";
            if (File.Exists(archivoUnico))
                File.Delete(archivoUnico);
            _creaArchivoCsv.CreaArchivoCsv(archivoUnico, resultadoUnico);
        }

        private static bool CumpleCriterio(DetalleInformacionCreditosOcr detalleInformacionCreditosOcr)
        {
            bool incumple = false;
            // Reviso si no es Clabe interbancaria
            incumple = incumple || (detalleInformacionCreditosOcr.Leyenda ?? "").Contains("Clabe", StringComparison.InvariantCultureIgnoreCase) || (detalleInformacionCreditosOcr.Leyenda ?? "").Contains("Cuenta", StringComparison.InvariantCultureIgnoreCase);
            // Reviso si el número de crédito tiene agencia
            incumple = incumple || (detalleInformacionCreditosOcr.NumeroDeCredito ?? "").Substring(1, 2).Equals("00", StringComparison.InvariantCultureIgnoreCase);
            return !incumple;
        }

        private static string ObtieneCadenaCreditoYDetallle(ArchivosImagenes img, ref DetalleInformacionCreditosOcr detalleInformacionCreditosOcr)
        {
            string cadenaCredito = img.CadenaCredito ?? "";
            if (!string.IsNullOrEmpty(cadenaCredito))
            {
                string[] divideCadenaCreditoEnPipes = cadenaCredito.Split('|');
                foreach (var infoCredito in divideCadenaCreditoEnPipes)
                {
                    if (!string.IsNullOrEmpty(infoCredito))
                    {
                        string[] divideEnComasLosCampos = infoCredito.Split(",");
                        if (divideEnComasLosCampos.Length != 3)
                        {
                            // Error
                            if (divideEnComasLosCampos[0].Length < 4)
                            {
                                if (!string.IsNullOrEmpty(divideEnComasLosCampos[0]))
                                {
                                    try
                                    {
                                        detalleInformacionCreditosOcr.Pagina = Convert.ToInt32(divideEnComasLosCampos[0]);
                                    }
                                    catch { }
                                }
                                else
                                {
                                    detalleInformacionCreditosOcr.Pagina = 0;
                                }
                                detalleInformacionCreditosOcr.Leyenda = "Error con pipe";
                                detalleInformacionCreditosOcr.NumeroDeCredito = cadenaCredito.Substring(cadenaCredito.Length - 18, 18);
                            }
                        }
                        else
                        {
                            detalleInformacionCreditosOcr.Pagina = Convert.ToInt32(divideEnComasLosCampos[0]);
                            detalleInformacionCreditosOcr.Leyenda = divideEnComasLosCampos[1];
                            detalleInformacionCreditosOcr.NumeroDeCredito = divideEnComasLosCampos[2];
                        }
                    }
                }
            }

            return cadenaCredito;
        }

        private static string ObtieneNumeroExpediente(string path)
        {
            string numExpediente = "";
            if (!string.IsNullOrEmpty(path))
            {
                string[] directorios = path.Split(Path.DirectorySeparatorChar);
                if (directorios.Length > 2)
                {
                    if (directorios[2].Length > 8)
                    {
                        numExpediente = directorios[2][..8];
                    }
                }
            }

            return numExpediente;
        }
    }
}
