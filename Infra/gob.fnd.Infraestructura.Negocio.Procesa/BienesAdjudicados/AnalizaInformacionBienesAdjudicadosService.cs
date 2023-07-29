using gob.fnd.Dominio.Digitalizacion.Excel.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.OtrasOperaciones.ResultadoOCRBA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.BienesAdjudicados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System.Globalization;
using System.Security;

namespace gob.fnd.Infraestructura.Negocio.Procesa.BienesAdjudicados;

public class AnalizaInformacionBienesAdjudicadosService : IAnalizaInformacionBienesAdjudicados
{
    private readonly ILogger<AnalizaInformacionBienesAdjudicadosService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAdministraCargaBienesAdjudicados _bienesAdjudicadosService;
    private readonly IAdministraCargaConsulta _administraCargaConsultaService;
    private readonly ICreaArchivoCsv _creaArchivoCsvService;
    private readonly IBienesAdjudicadosIdentificados _bienesAdjudicadosIdentificadosService;
    private readonly string _archivoBienesAdjudicados;
    private readonly string _archivosImagenesBienesAdjudicadosCorta;

    public AnalizaInformacionBienesAdjudicadosService(ILogger<AnalizaInformacionBienesAdjudicadosService> logger, 
        IConfiguration configuration, IAdministraCargaBienesAdjudicados bienesAdjudicadosService,
        IAdministraCargaConsulta administraCargaConsultaService,
        ICreaArchivoCsv creaArchivoCsvService,
        IBienesAdjudicadosIdentificados bienesAdjudicadosIdentificadosService
        )
    {
        _logger = logger;
        _configuration = configuration;
        _bienesAdjudicadosService = bienesAdjudicadosService;
        _administraCargaConsultaService = administraCargaConsultaService;
        _creaArchivoCsvService = creaArchivoCsvService;
        _bienesAdjudicadosIdentificadosService = bienesAdjudicadosIdentificadosService;
        _archivoBienesAdjudicados = _configuration.GetValue<string>("archivoBienesAdjudicados") ?? "";
        _archivosImagenesBienesAdjudicadosCorta = _configuration.GetValue<string>("archivosImagenesBienesAdjudicadosCorta") ?? "";
    }

    /// <summary>
    /// Aqui sería el proceso principal que orquesta todos los cruces para obtener
    /// el consolidado de toda la información de los bienes adjudicados
    /// </summary>
    public void RealizaAnalisisBienesAdjudicados()
    {
        _logger.LogInformation("Comienzo con el cruce entre los expedientes y las carpetas");
        var cruceFuenteBAvsExpedientes = RealizaCruceEntreBaseBienesAdjudicadosYExpedientes();
        _logger.LogInformation("Terminó el cruce entre los expedientes y las carpetas");

        _logger.LogInformation("Cargo la información de los bienes Adjudicados Service y comienzo el cruce con los expedientes");
        var bienesIdentificadosAdjudicados = RealizaCruceEntreBienesIdentificadosYExpedientes(cruceFuenteBAvsExpedientes);
        _logger.LogInformation("Se cruzaron {numeroExpedientesIdentificados}", bienesIdentificadosAdjudicados.Count(x => !string.IsNullOrEmpty(x.CveBienI)));
        _logger.LogInformation("Termino el cruce entre los expedientes y los bienes adjudicados identificados");

    }
    /// <summary>
    /// Aqui se realiza el cruce entre la base de datos de Recursos Materiales
    /// y las carpetas físicas con todos los archivos de los expedientes
    /// 
    /// La idea es asignar una carpeta de expediente por cada registro 
    /// </summary>
    /// <param name="archivoBienesAdjudicados">Archivo base de donde sale la información de los bienes adjudicados</param>
    /// <param name="archivoImagenesBienesAdjudicados">Archivo donde se vaciaron los pdf's de las carpetas de los expedientes</param>
    /// <returns>Lista de cruce entre expedientes y bienes adjudicados</returns>

    public IEnumerable<CruceFuenteBAvsExpedientes> RealizaCruceEntreBaseBienesAdjudicadosYExpedientes(string archivoBienesAdjudicados = "", string archivoImagenesBienesAdjudicados = "")
    {
        IList<CruceFuenteBAvsExpedientes> resultado = new List<CruceFuenteBAvsExpedientes>();
        // En caso de que los parámetros esten vacíos se utiliza la información del app.json por default.
        if (string.IsNullOrEmpty(archivoBienesAdjudicados))
            archivoBienesAdjudicados = _archivoBienesAdjudicados;
        if (string.IsNullOrEmpty(archivoImagenesBienesAdjudicados))
            archivoImagenesBienesAdjudicados = _archivosImagenesBienesAdjudicadosCorta;
        // En caso de que no existan los archivos a procesar, se regresa una lista vacía
        if (!File.Exists(archivoBienesAdjudicados) || !File.Exists(archivoImagenesBienesAdjudicados))
        {
            return resultado;
        }

        // Cargo los expedientes a revisar
        var expedientes = _bienesAdjudicadosService.CargaBienesAdjudicados(archivoBienesAdjudicados);
        // Cargo las imagnes a cruzar
        var imagenes = _administraCargaConsultaService.CargaArchivoImagenBienesAdjudicadosCorta(archivoImagenesBienesAdjudicados);

        // Lista de imagenes por expediente
        IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas = new List<ArchivoImagenBienesAdjudicadosCorta>();

        // Recorro cada expediente para buscar su carpeta de imagen basado en la información del acreditado del expediente
        foreach (var expediente in expedientes)
        {
            // Se filtran los elementos de "imagenes" y se asignan a la variable "listaImagenesEncontradas".
            listaImagenesEncontradas = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (expediente.ExpedienteElectronico ?? "").Equals(x.NumExpediente, StringComparison.OrdinalIgnoreCase)).ToList();
            // Solo en caso de no encontrar imagenes intento relacionar expedientes y carpetas con fechas diferentes
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..7].Equals((expediente.ExpedienteElectronico??"")[..7], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
            }
            // Se llama a la función "FiltraMasdeUnAcreditado" con la variable "acreditados" y la variable "listaImagenesEncontradas", y se asigna el resultado a "listaImagenesEncontradas".
            listaImagenesEncontradas = FiltraMasdeUnAcreditado(expediente.Acreditado??"", listaImagenesEncontradas);
            #region Vuelvo a buscar, pero quitando los días
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..6].Equals((expediente.ExpedienteElectronico ?? "")[..6], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(expediente.Acreditado ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Vuelvo a buscar, dejando solo el año
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "")[..4].Equals((expediente.ExpedienteElectronico ?? "")[..4], StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(expediente.Acreditado ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Vuelvo a buscar, quitando el año y dejando mes y día
            if (!listaImagenesEncontradas.Any())
            {
                var imagenesAdicionales = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumExpediente ?? "").Substring(4, 4).Equals((expediente.ExpedienteElectronico ?? "").Substring(4, 4), StringComparison.OrdinalIgnoreCase)).ToArray();
                ((List<ArchivoImagenBienesAdjudicadosCorta>)listaImagenesEncontradas).AddRange(imagenesAdicionales);
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(expediente.Acreditado ?? "", listaImagenesEncontradas);
            }
            #endregion
            #region Si de plano no hay nada, espero que sea una empresa y sea solo una palabra
            if (!listaImagenesEncontradas.Any())
            {
                listaImagenesEncontradas = imagenes.OrderByDescending(y => y.NumPaginas).Where(x => (expediente.ExpedienteElectronico ?? "").Equals(x.NumExpediente, StringComparison.OrdinalIgnoreCase)).ToList();
                // Ya solo se busca por una palabra
                listaImagenesEncontradas = FiltraMasdeUnAcreditado(expediente.Acreditado ?? "", listaImagenesEncontradas, true);
            }
            #endregion

            CruceFuenteBAvsExpedientes cruce = new()
                {
                    NumRegion = expediente.NumRegion,
                    CatRegion = expediente.CatRegion,
                    Entidad = expediente.Entidad,
                    CveBien = expediente.CveBien,
                    ExpedienteElectronico = expediente.ExpedienteElectronico,
                    Acreditado = expediente.Acreditado,
                    TipoDeBien = expediente.TipoDeBien,
                    DescripcionReducidaBien = expediente.DescripcionReducidaBien,
                    DescripcionCompletaBien = expediente.DescripcionCompletaBien,
                    OficioNotificaiconGCRJ = expediente.OficioNotificaiconGCRJ,
                    NumCredito = expediente.NumCredito,
                    TipoAdjudicacion = expediente.TipoAdjudicacion,
                    AreaResponsable = expediente.AreaResponsable,
                    ImporteIndivAdjudicacion = expediente.ImporteIndivAdjudicacion,
                    ImporteIndivImplicado = expediente.ImporteIndivImplicado,
                    Estatus = expediente.Estatus
                };
            // Si hay algun criterio coincidente, se agrega la carpeta, en caso contrario, se va en blanco
            if (listaImagenesEncontradas.Any())
            {
                cruce.DirectorioExpediente = QuitaUltimaCarpeta(listaImagenesEncontradas);
                if ((cruce.DirectorioExpediente ?? "").Contains("20220322_d", StringComparison.InvariantCultureIgnoreCase)
                    && (cruce.Acreditado??"").Contains("Medina", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    _logger.LogInformation("A revisar");
                }
            }
            else
            {
                _logger.LogInformation("No encontró la información en una carpeta para el expediente {numeroExpediente} del bien {cveBien}", cruce.ExpedienteElectronico, cruce.CveBien);
            }
            resultado.Add(cruce);
        }

        // Guardo la información en un archivo csv
        _creaArchivoCsvService.CreaArchivoCsv("C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\CruceExpedientesVsCarpetas.csv",
            resultado);

        return resultado;
    }

    /// <summary>
    /// Quito la carpeta de la clasificación de la documentación
    /// </summary>
    /// <param name="listaImagenesEncontradas">Lista de imágenes encontradas</param>
    /// <returns>Carpeta solo del expediente</returns>
    private static string? QuitaUltimaCarpeta(IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas)
    {
        string? carpetaDestino = listaImagenesEncontradas.Select(x => x.CarpetaDestino).First();
        // necesito quitar la ultima carpeta de un path de la carpeta destino

        if (!string.IsNullOrEmpty(carpetaDestino))
        {
            carpetaDestino = carpetaDestino[..carpetaDestino.LastIndexOf('\\')];
        }

        return carpetaDestino;
    }

    /// <summary>
    /// Como en los expedientes, los números se pueden repetir, pero con otros acreditados, 
    /// se observa el acreditado declarado en el expediente y se busca en las imágenes
    /// Filtrando aquellos que cumplan el criterio de manera parcial
    /// 
    /// Por ejemplo en el expediente, el acreditado aparece como: José Jaime Sánchez Chavez y
    /// en las imáges aparece cmo 20160913 José Jaime Sánchez Chávez
    /// 
    /// Por tanto busco por José en los acreditados, cuando encuentre más de un acreditado,
    /// y si el resultante ya es un registor único, lo regreso, si no, busco por Jaime, 
    /// y así sucesivamente
    /// </summary>
    /// <param name="acreditados">Nombres que aparecen en el expediente</param>
    /// <param name="listaImagenesEncontradas">Lista de imágenes, con su campo de acreditado por expediente</param>
    /// <param name="soloUnaPalabra">Se busca solo una palabra cuando son empresas</param>
    /// <returns>Lista de imágenes que cumplen con pertenecer a lo descrito en acreditados</returns>
    private static IList<ArchivoImagenBienesAdjudicadosCorta> FiltraMasdeUnAcreditado(string acreditados, IList<ArchivoImagenBienesAdjudicadosCorta> listaImagenesEncontradas, bool soloUnaPalabra = false)
    {
        string?[] listaAcreditadosDirectorio = listaImagenesEncontradas.Select(x => x.Acreditados).Distinct().ToArray();
        if (listaAcreditadosDirectorio.Length == 1)
        {
            return listaImagenesEncontradas;
        }

        char[] splitArray = { '_', '-', '(', ')', ' ', ',','.','=','~','|' };
        string[] listaAcreditados = acreditados.Split(splitArray).Where(x=>!string.IsNullOrEmpty(x)).ToList().Where(x=>x.Length > 3).ToArray();
        if (listaAcreditadosDirectorio is not null)
        {
            int encontro = 2;
            IList<string> palabrasPorBuscar = new List<string>();
            int criterio = 0;
            var buscaAcreditado = listaAcreditadosDirectorio;
            bool removioElUltimoCriterio = false;
            while ((encontro != 0) && (criterio< listaAcreditados.Length))
            {
                string palabraActual = RemoveAccentsWithNormalization(listaAcreditados[criterio]);
                if (palabraActual.Equals("Maria", StringComparison.InvariantCultureIgnoreCase))
                {
                    palabraActual = "M";
                }

                palabrasPorBuscar.Add(palabraActual);
                // necesito hacer un query que regrese si hay coincidencias con las palabras por buscar

                var temporalBuscaAcreditado = buscaAcreditado.Where(x => RemoveAccentsWithNormalization(x ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                encontro = temporalBuscaAcreditado.Length;
                if (encontro != 0)
                {
                    buscaAcreditado = temporalBuscaAcreditado;
                    removioElUltimoCriterio = false;
                }
                else
                {
                    // Necesito remover la ultima palabra de la lista de palabras a buscar
                    palabrasPorBuscar.RemoveAt(palabrasPorBuscar.Count - 1);
                    removioElUltimoCriterio = true;
                    /*
                    if (criterio + 1 == listaAcreditados.Length)
                    {
                        break;
                    }
                    */
                    encontro = 2;
                }
                criterio++;
            }
            // Si hay suficientes criterios para dar por buena el cruce, lo regreso, si no, regreso una lista vacía
            if (palabrasPorBuscar.Count > 1 || !removioElUltimoCriterio)
            {
                foreach (var palabraActual in palabrasPorBuscar)
                {
                    listaImagenesEncontradas = listaImagenesEncontradas.Where(x => RemoveAccentsWithNormalization(x.Acreditados ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                }
                return listaImagenesEncontradas;
            }
            else
            {
                if (soloUnaPalabra)
                {
                    foreach (var palabraActual in palabrasPorBuscar)
                    {
                        listaImagenesEncontradas = listaImagenesEncontradas.Where(x => RemoveAccentsWithNormalization(x.Acreditados ?? "").Contains(palabraActual, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                    }
                    return listaImagenesEncontradas;
                }
                return new List<ArchivoImagenBienesAdjudicadosCorta>();
            }
        }
        return new List<ArchivoImagenBienesAdjudicadosCorta>();
    }

    // Necesito una funcion que quite las tildes y las ñ
    public static string RemoveAccentsWithNormalization(string inputString)
    {
        inputString = inputString.Replace("y", "i", StringComparison.InvariantCultureIgnoreCase);
        if (string.IsNullOrEmpty(inputString))
            return inputString;
        inputString = inputString.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in inputString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public IEnumerable<CruceFuenteVsIdentificacionClaveBien> RealizaCruceEntreBienesIdentificadosYExpedientes(IEnumerable<CruceFuenteBAvsExpedientes> expedientes, string archivoBienesAdjudicadosIdentificados = "")
    {
        // new List<CruceFuenteVsIdentificacionClaveBien>();
        _logger.LogInformation("Cargo la información de los bienes Adjudicados Service y comienzo el cruce con los expedientes");
        var bienesIdentificadosAdjudicados = _bienesAdjudicadosIdentificadosService.ObtieneFuenteBienesAdjudicadosIdentificacion();
        IList<CruceFuenteVsIdentificacionClaveBien> listaCruce = expedientes.Select(x => new CruceFuenteVsIdentificacionClaveBien() {
            NumRegion = x.NumRegion,
            CatRegion = x.CatRegion,
            Entidad = x.Entidad,
            CveBien = x.CveBien,
            ExpedienteElectronico = x.ExpedienteElectronico,
            Acreditado = x.Acreditado,
            TipoDeBien = x.TipoDeBien,
            DescripcionReducidaBien = x.DescripcionReducidaBien,
            DescripcionCompletaBien = x.DescripcionCompletaBien,
            OficioNotificaiconGCRJ = x.OficioNotificaiconGCRJ,
            NumCredito = x.NumCredito,
            TipoAdjudicacion = x.TipoAdjudicacion,
            AreaResponsable = x.AreaResponsable,
            ImporteIndivAdjudicacion = x.ImporteIndivAdjudicacion,
            ImporteIndivImplicado = x.ImporteIndivImplicado,
            Estatus = x.Estatus,
            DirectorioExpediente = x.DirectorioExpediente
        } ).ToList();
        var cruceInformacion = from expediente in listaCruce
                               join bienIdentificado in bienesIdentificadosAdjudicados on expediente.CveBien equals bienIdentificado.CveBienI
                               select new { expediente, bienIdentificado };
        foreach (var item in cruceInformacion)
        { 
            item.expediente.CveBienI = item.bienIdentificado.CveBienI;
            item.expediente.CrI = item.bienIdentificado.CrI;
            item.expediente.AcreditadoI = item.bienIdentificado.AcreditadoI;
            item.expediente.TipoBienI = item.bienIdentificado.TipoBienI;            
            item.expediente.NumCreditoI = AgregarCaracter(item.bienIdentificado.NumCreditoI);
            item.expediente.ObservacionesI = item.bienIdentificado.ObservacionesI;
        }

        // Guardo la información en un archivo csv
        _creaArchivoCsvService.CreaArchivoCsv("C:\\202302 Digitalizacion\\2. Saldos procesados\\BienesAdjudicados\\CruceExpedientesVsIdentificados.csv",
            listaCruce);

        _logger.LogInformation("Termino el cruce entre los expedientes y los bienes adjudicados identificados");
        return listaCruce;
    }

    public static string AgregarCaracter(string? inputString)
    {
        if (string.IsNullOrEmpty(inputString))
            return "";
        inputString = inputString.Replace(" ", "", StringComparison.InvariantCultureIgnoreCase);
        inputString = inputString.Replace("\r\n", "", StringComparison.InvariantCultureIgnoreCase);
        inputString = inputString.Replace("\n", "", StringComparison.InvariantCultureIgnoreCase);
        string output = "";
        int n = inputString.Length;
        for (int i = 0; i < n; i += 18)
        {
            if (i + 18 < n)
            {
                output += string.Concat(inputString.AsSpan(i, 18), "¬");
            }
            else
            {
                output += inputString[i..];
            }
        }
        return output;
    }

}
