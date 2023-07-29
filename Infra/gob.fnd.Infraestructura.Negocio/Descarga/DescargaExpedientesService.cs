using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using gob.fnd.Dominio.Digitalizacion.Negocio.Descarga;
using gob.fnd.Infraestructura.Negocio.Consultas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Descarga
{
    public class DescargaExpedientesService : IDescargaExpedientes
    {
        private readonly ILogger _logger;
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private readonly IConsultaServices _consultaServices;


        public DescargaExpedientesService(ILogger<DescargaExpedientesService> logger, IConfiguration configuration, IConsultaServices consultaServices)
        {
            _logger = logger;
            _configuration = configuration;
            _consultaServices = consultaServices;
        }

        public bool DescargaListaDeExpedientes(string nombreArchivoListaContratos, string carpetaDestino)
        {
            if (!File.Exists(nombreArchivoListaContratos))
            {
                _logger.LogError("No se encontró el archivo con la lista de los contratos! {nombreArchivo}", nombreArchivoListaContratos);
                return false;
            }
            // Cargo la lista de contratos a revisar
            IList<string> listaDeContratos = LeeNumerosDeContrato(nombreArchivoListaContratos);
            // Creo la lista de contratos encontrados
            IList<string> expedientesEncontrados= new List<string>();
            IList<ExpedienteDeConsultaGv> expedientesEncontradosDetalle = new List<ExpedienteDeConsultaGv>();
            IList<int> noExpedientesEncontrados = new List<int>();

            // Cargo la lista de expedientes para encontrar de ahi los contratos            
            IEnumerable<ExpedienteDeConsultaGv> expedientes = _consultaServices.CargaInformacion();
            // Carga la lista de imagenes 
            _ = _consultaServices.CargaInformacionImagenes();
            //IEnumerable<ArchivoImagenCorta> imagenesCortas = _consultaServices.CargaInformacionImagenes();
            foreach (var expediente in listaDeContratos )
            {
                var encuentraExpediente = expedientes.Where(x => ComparaExpedientes(x.NumCredito, expediente));
                if (encuentraExpediente.Any())
                {
                    // Se agrega a la lista de expedientes encontrados
                    expedientesEncontrados.Add(expediente);
                    // Se revisan las imagenes por expediente
                    var ultimoExpediente = encuentraExpediente.OrderBy(x => x.NumCredito).LastOrDefault();
                    var primerExpediente = encuentraExpediente.OrderBy(x => x.NumCredito).FirstOrDefault();
                    int numExpedientes = 1;
                    if (ultimoExpediente != primerExpediente && (ultimoExpediente is not null && primerExpediente is not null))
                    {
                        numExpedientes = encuentraExpediente.Count();
                        ExpedienteDeConsultaGv encontrado = new()
                        {
                            NumCredito = primerExpediente.NumCredito,
                            NumCreditoCancelado = ultimoExpediente.NumCreditoCancelado,
                            EsSaldosActivo = ultimoExpediente.EsSaldosActivo,
                            EsCancelado = ultimoExpediente.EsCancelado,
                            EsOrigenDelDr = ultimoExpediente.EsOrigenDelDr,
                            EsCanceladoDelDr = ultimoExpediente.EsCanceladoDelDr,
                            EsCastigado = ultimoExpediente.EsCastigado,
                            TieneArqueo = ultimoExpediente.TieneArqueo,
                            Acreditado = ultimoExpediente.Acreditado,
                            FechaApertura = primerExpediente.FechaApertura,
                            FechaCancelacion = ultimoExpediente.FechaCancelacion,
                            Castigo = ultimoExpediente.Castigo,
                            NumProducto = ultimoExpediente.NumProducto,
                            CatProducto = ultimoExpediente.CatProducto,
                            TipoDeCredito = ultimoExpediente.TipoDeCredito,
                            Ejecutivo = ultimoExpediente.Ejecutivo,
                            Analista = ultimoExpediente.Analista,
                            FechaInicioMinistracion = primerExpediente.FechaInicioMinistracion,
                            FechaSolicitud = primerExpediente.FechaSolicitud,
                            MontoCredito = ultimoExpediente.MontoCredito,
                            InterCont = ultimoExpediente.InterCont,
                            Region = ultimoExpediente.Region,
                            Agencia = ultimoExpediente.Agencia,
                            CatRegion = ultimoExpediente.CatRegion,
                            CatAgencia = ultimoExpediente.CatAgencia,
                            EsCreditoAReportar = ultimoExpediente.EsCreditoAReportar,
                            StatusImpago = ultimoExpediente.StatusImpago,
                            StatusCarteraVencida = ultimoExpediente.StatusCarteraVencida,
                            StatusCarteraVigente = ultimoExpediente.StatusCarteraVigente,
                            TieneImagenDirecta = ultimoExpediente.TieneImagenDirecta,
                            TieneImagenIndirecta = ultimoExpediente.TieneImagenIndirecta,
                            SldoTotContval = ultimoExpediente.SldoTotContval,
                            NumCliente = ultimoExpediente.NumCliente
                        };
                    }
                    noExpedientesEncontrados.Add(numExpedientes);

                    IEnumerable<ArchivoImagenCorta> imagenesCortasEncontradas = _consultaServices.BuscaImagenes(QuitaCastigoContrato(expediente), true);
                    int numImagenes = 0;
                    if (imagenesCortasEncontradas.Any())
                    {
                        numImagenes = imagenesCortasEncontradas.Count();
                    }
                    string numContrato = QuitaCastigoContrato(expediente)[..14];
                    string rutaContrato = Path.Combine(carpetaDestino, numContrato);
                    if (!Directory.Exists(rutaContrato))
                    {
                        Directory.CreateDirectory(rutaContrato);
                    }
                    if (numImagenes != 0) 
                    {
                        foreach (var img in imagenesCortasEncontradas.OrderBy(x=>x.NumCredito))
                        { 
                            string rutaCredito = Path.Combine(rutaContrato, img.NumCredito?? "000000000000000000");
                            if (!Directory.Exists(rutaCredito))
                            { 
                                Directory.CreateDirectory(rutaCredito);
                            }
                            string nombreArchivoOrignal = img.NombreArchivo ?? "";
                            string nuevoNombreArchivo = nombreArchivoOrignal;
                            int cantidadDuplicado = 1;
                            while (File.Exists(Path.Combine(rutaCredito, nuevoNombreArchivo)))
                            { 
                                string nombreArchivoSinExtension = Path.GetFileNameWithoutExtension(nombreArchivoOrignal);
                                string extension = Path.GetExtension(nombreArchivoOrignal);
                                nuevoNombreArchivo = $"{nombreArchivoSinExtension} ({cantidadDuplicado}){extension}";
                                cantidadDuplicado++;
                            }
                            string archivoDestino = Path.Combine(rutaCredito, nuevoNombreArchivo);
                            string archivoImagenACopiar = Path.Combine(img.CarpetaDestino ?? "", img.NombreArchivo ?? "").Replace(@"G:\",@"F:\");
                            if (File.Exists(archivoImagenACopiar))
                            {
                                File.Copy(archivoImagenACopiar, archivoDestino, false);
                            }
                            else { 
                                _logger.LogError("No se encontró el archivo {archivo}", archivoImagenACopiar);
                            }
                        }
                    }
                }

            }

            return expedientesEncontrados.Any();
        }

        public static string QuitaCastigo(string? origen)
        {
            if (origen is null)
                return "000000000000000000";

            if (origen.Length < 18)
            {
                origen = (origen + "000000000000000000")[..18];
            }
            string digito = origen.Substring(3, 1);
            string digito2 = origen.Substring(4, 1);
            string nuevoOrigen = origen;
            if ((digito == "1" || digito == "9") && (digito2 != "0"))
            {
                nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
            }
            return nuevoOrigen;
        }

        public static string QuitaCastigoContrato(string? origen)
        {
            if (origen is null)
                return "000000000000000000";

            if (origen.Length < 18)
            {
                origen = (origen + "000000000000000000")[..18];
            }
            string digito = origen.Substring(3, 1);
            string digito2 = origen.Substring(4, 1);
            string nuevoOrigen = origen;
            if ((digito == "1" || digito == "9") && (digito2 != "0"))
            {
                nuevoOrigen = string.Concat(origen.AsSpan(0, 3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 9)) + "0000";
            }
            else {
                nuevoOrigen = nuevoOrigen[..14] + "0000";
            }
            return nuevoOrigen;
        }

        private static bool ComparaExpedientes(string? numCredito, string? numCreditoExpedienteABuscar) 
        {
            if ((string.IsNullOrEmpty(numCredito) || string.IsNullOrEmpty(numCreditoExpedienteABuscar)))
            {
                return false;
            }
            if (QuitaCastigo(numCredito).Equals(QuitaCastigo(numCreditoExpedienteABuscar)))
            {
                return true;
            }
            if (QuitaCastigoContrato(numCredito).Equals(QuitaCastigoContrato(numCreditoExpedienteABuscar)))
            {
                return true;
            }
            return false;
        }

        private static IList<string> LeeNumerosDeContrato(string nombreArchivoListaContratos)
        {
            return File.ReadAllLines(nombreArchivoListaContratos).ToList<string>();
        }
    }
}
