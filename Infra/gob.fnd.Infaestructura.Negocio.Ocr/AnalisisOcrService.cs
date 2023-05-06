using gob.fnd.Dominio.Digitalizacion.Entidades.Ocr;
using gob.fnd.Dominio.Digitalizacion.Negocio.Ocr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gob.fnd.Infaestructura.Negocio.Ocr
{
    public class AnalisisOcrService : IAnalisisOcr
    {
        static readonly string[] C_STRA_CRITERIO_CREDITO = new string[] { "CREDITO", "APERTURA", "Exig.", "RECUPERACIÓN", "APERTURA :", "No.APERTURA :" };
        static readonly string[] C_STRA_CRITERIO_CLIENTE = new string[] { "CLIENTE" };
        private readonly ILogger<AnalisisOcrService> _logger;
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private readonly IConfiguration _configuration;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        const int C_INT_LONGITUD_NUMERO_CREDITO = 18;
        const int C_INT_LONGITUD_NUMERO_CLIENTE = 12;
        const char C_STR_SEPARADOR = '|';

        public AnalisisOcrService(ILogger<AnalisisOcrService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene la información del número de crédito y número de cliente de un archivo
        /// </summary>
        /// <param name="archivoOrigen">Nombre del archivo que se analizará</param>
        /// <param name="cadenaCredito">Cadena resultante del análisis de los números de 18 dígitos</param>
        /// <param name="cadenaCliente">Cadena resultante del análisis de los números parecidos al número de cliente</param>
        /// <param name="numeroCliente">Evaluación del número de cliente</param>
        /// <param name="numeroCredito">Evaluación del número de crédito</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ObtieneInformacionOCR(string archivoOrigen, ref string cadenaCredito, ref string cadenaCliente, ref string numeroCliente, ref string numeroCredito)
        {
            try
            {
                numeroCliente = string.Empty;
                numeroCredito = string.Empty;
                cadenaCredito = string.Empty;
                cadenaCliente = string.Empty;

                StringBuilder sbNumeroDeCredito = new();
                StringBuilder sbNumeroDeCliente = new();

                string archivoCompleto = File.ReadAllText(archivoOrigen);
                string separador = "============= Separador =================";
                string[] arregloCadenas = archivoCompleto.Split(new string[] { separador }, StringSplitOptions.None);
                // Consideración de los separados
                bool considerarPipeNumeroCredito = false;
                bool considerarPipeNumeroCliente = false;
                foreach (string paginaCompleta in arregloCadenas)
                {
                    int numeroDePagina = ObtieneNumeroPágina(paginaCompleta);
                    string cadena = ObtengoCadenaNumero(numeroDePagina, paginaCompleta, C_INT_LONGITUD_NUMERO_CREDITO);
                    if (!string.IsNullOrEmpty(cadena) && !string.IsNullOrWhiteSpace(cadena))
                    {
                        if (considerarPipeNumeroCredito)
                            sbNumeroDeCredito.Append(C_STR_SEPARADOR);
                        sbNumeroDeCredito.Append(cadena);
                        considerarPipeNumeroCredito = true;
                    }
                    cadena = ObtengoCadenaNumero(numeroDePagina, paginaCompleta, C_INT_LONGITUD_NUMERO_CLIENTE);
                    if (!string.IsNullOrEmpty(cadena) && !string.IsNullOrWhiteSpace(cadena))
                    {
                        if (considerarPipeNumeroCliente)
                            sbNumeroDeCliente.Append(C_STR_SEPARADOR);
                        sbNumeroDeCliente.Append(cadena);
                        considerarPipeNumeroCliente = true;
                    }
                }
                cadenaCredito = RecortaOriginal(sbNumeroDeCredito.ToString(), 29696);
                cadenaCliente = RecortaOriginal(sbNumeroDeCliente.ToString(), 29696);

                numeroCredito = ObtieneCriterioBusqueda(cadenaCredito, C_STRA_CRITERIO_CREDITO);
                numeroCliente = ObtieneCriterioBusqueda(cadenaCliente, C_STRA_CRITERIO_CLIENTE);
            }
            catch (Exception ex)
            {
                _logger.LogError("No se pudo analizar el archivo de texto {archivoTexto} con el mensaje de error {mensaje}", archivoOrigen, ex.Message);
                return false;
            }
            return true;
        }

        static string RecortaOriginal(string original, int maximaLongitud = 2048)
        {
            string[] elementos = original.Split('|'); // dividimos el string original por el carácter '|'

            IList<string> nuevosElementos = new List<string>(); // creamos una lista para almacenar los nuevos elementos
            string nuevoElemento = string.Empty; // creamos un nuevo elemento vacío

            foreach (string elemento in elementos)
            {
                if (nuevoElemento.Length + elemento.Length + 1 <= maximaLongitud) // si la longitud del nuevo elemento es menor a 32765 caracteres, agregamos el elemento al nuevo elemento
                {
                    nuevoElemento += elemento + "|";
                }
            }

            if (!string.IsNullOrEmpty(nuevoElemento)) // agregamos el último nuevo elemento si es que existe
            {
                nuevosElementos.Add(nuevoElemento.TrimEnd('|'));
            }

            string nuevoString = string.Join("|", nuevosElementos); // unimos los nuevos elementos con el carácter '|' para crear el nuevo string
            return nuevoString;
        }

        private static string ObtieneCriterioBusqueda(string cadenaCredito, string[] arregloCriterio)
        {
            string resultado = string.Empty;
            string[] elementos = cadenaCredito.Split('|');
            IList<FiltroBusquedaOcr> lista = new List<FiltroBusquedaOcr>();
            if (elementos.Length >= 1)
            {
                foreach (string elemento in elementos)
                {
                    string[] campos = elemento.Split(",");
                    if (campos.Length == 3)
                    {
                        FiltroBusquedaOcr obj = new()
                        {
                            Pagina = Convert.ToInt32(campos[0]),
                            Busqueda = campos[1],
                            Valor = campos[2]
                        };
                        lista.Add(obj);
                    }
                }
                var listaFiltrada = lista.Where(x => arregloCriterio.Any(y => RemoveAccents(x.Busqueda ?? "").Contains(RemoveAccents(y), StringComparison.InvariantCultureIgnoreCase))).Select(z => z).ToList();
                // Obtengo el de mayor coincidencia
                var listaAgrupadaCantidad = (from lf in listaFiltrada
                                             group lf by lf.Valor into agr
                                             orderby agr.Count() descending
                                             select new { Valor = agr.Key, Cantidad = agr.Count() }).ToList();
                // Obtengo el primer valor de coincidencia
                var valorGanador = listaAgrupadaCantidad.FirstOrDefault();
                // Si existe un valor ganador, lo asigno al resultado
                if (valorGanador is not null)
                {
                    resultado = valorGanador.Valor;
                }
            }

            return resultado;
        }

        private static string RemoveAccents(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private string ObtengoCadenaNumero(int numeroDePagina, string paginaCompleta, int longitudABuscar)
        {
            StringBuilder sb = new();
            string palabraAnteriorUno = string.Empty;
            string palabraAnteriorDos = string.Empty;
            // Dividir la cadena en un arreglo de strings utilizando el separador espacio
            string[] palabrasConBlancos = paginaCompleta.Replace('\r', ' ').Replace('\n', ' ').Split(' ');
            // Elimino las palabras vacías o sin sentido
            string[] palabras = palabrasConBlancos.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).Select(y => y).ToArray();
            // Encontrar los elementos del arreglo que tienen una longitud de 18 caracteres
            string[] resultadosPalabras = palabras.Where(x => x.Length == longitudABuscar).ToArray();
            // Reemplazo las que tengan letas y las quito de los resultados
            Regex regex = new("[^0-9]+");
            string[] resultados = resultadosPalabras.Where(x => regex.Replace(x, " ").Split(' ').Length == 1).Select(y => regex.Replace(y, " ")).Distinct().ToArray();
            bool separador = false;
            if (resultados.Length != 0)
            {
                if (resultados.Length >= 2)
                    System.Diagnostics.Debug.WriteLine("Comienza");
                foreach (string resultado in resultados)
                {

                    // Encontrar los dos elementos anteriores a cada cadena de 18 dígitos
                    for (int i = 0; i < palabras.Length; i++)
                    {
                        // Si existe el resultado dentro de las palabras
                        if (palabras[i].Equals(resultado))
                        {
                            if (i >= 2)
                            {
                                // Fix: Elimina la coma porque es un separador
                                palabraAnteriorDos = palabras[i - 2].Replace(",","");
                                palabraAnteriorUno = palabras[i - 1].Replace(",", "");
                                _logger.LogTrace("Elementos anteriores a la cadena de 18 dígitos: {2PalabrasAnterior}, {palabraAnterior}", palabras[i - 2], palabras[i - 1]);
                            }
                            else if (i == 1)
                            {
                                // Fix: Elimina la coma porque es un separador
                                palabraAnteriorUno = palabras[i - 1].Replace(",", "");
                                _logger.LogTrace("Elemento anterior a la cadena de 18 dígitos: {palabraAnterior}", palabras[i - 1]);
                            }
                        }
                    }
                    string palabraAnterior = palabraAnteriorDos + ' ' + palabraAnteriorUno;

                    if (separador)
                        sb.Append('|');
                    sb.Append(string.Format("{0},'{1}',{2}", numeroDePagina, palabraAnterior, resultado));
                    separador = true;
                }
            }
            return sb.ToString();
        }

        private static int ObtieneNumeroPágina(string paginaCompleta)
        {
            int numeroPagina = 0;
            string subcadena = "============= Fin : Pagina ";
            int indice = paginaCompleta.IndexOf(subcadena);
            string palabra;
            if (indice != -1)
            {
                indice += subcadena.Length;
                int finPalabra = paginaCompleta.IndexOf(" ", indice);
                if (finPalabra == -1)
                {
                    finPalabra = paginaCompleta.Length;
                }
                palabra = paginaCompleta[indice..finPalabra];
                if (palabra.Contains('/'))
                {
                    string[] numeroPaginas = palabra.Split('/');
                    if (numeroPaginas.Length == 2)
                    {
                        numeroPagina = Convert.ToInt32(numeroPaginas[0]);
                    }
                }
            }
            return numeroPagina;
        }
    }
}
