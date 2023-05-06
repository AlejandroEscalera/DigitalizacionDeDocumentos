using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno
{
    public static class Condiciones
    {
        public const string C_STR_NULLO = "000000000000000000";
        public static string ObtieneCastigo(string? clasificacion)
        {
            if (string.IsNullOrEmpty(clasificacion) || string.IsNullOrWhiteSpace(clasificacion))
                return "Sin clasi";
            return clasificacion switch
            {
                "N" => "Normal",
                "S" => "Castigada",
                "R" => "Rpto",
                _ => "Sin clasi",
            };
        }

        public static string? RegresaObtieneCastigo(string? clasificacion)
        {
            if (string.IsNullOrEmpty(clasificacion) || string.IsNullOrWhiteSpace(clasificacion))
                return null;

            if (clasificacion.Contains("Sin clas", StringComparison.InvariantCultureIgnoreCase))
                return null;
            if (clasificacion.Contains("Normal", StringComparison.InvariantCultureIgnoreCase))
                return "N";
            if (clasificacion.Contains("Castigada", StringComparison.InvariantCultureIgnoreCase))
                return "S";
            if (clasificacion.Contains("Rpto", StringComparison.InvariantCultureIgnoreCase))
                return "R";
            return null;
        }

        public static string EsPeriodoDelDr(bool? esPeriodoDelDr) 
        {
            if (esPeriodoDelDr is null || !esPeriodoDelDr.HasValue) {
                return "Sin clas";
            }
            if (esPeriodoDelDr.Value)
            {
                return "Dr";
            }
            else
                return "No";
        
        }

        public static bool? RegresaEsPeriodoDelDr(string? esPeriodoDelDr)
        {
            if (string.IsNullOrEmpty(esPeriodoDelDr) || string.IsNullOrWhiteSpace(esPeriodoDelDr))
                return null;

            if (esPeriodoDelDr.Contains("Sin clas", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            if (esPeriodoDelDr.Contains("Dr", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            if (esPeriodoDelDr.Contains("No", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// Si está en el arqueo con ABSaldos
        /// </summary>
        /// <param name="esABSaldos">Para conocer el resultado del Arqueo</param>
        /// <returns></returns>
        public static string EsCarteraABSaldos(bool? esABSaldos)
        {
            if (esABSaldos is null || !esABSaldos.HasValue)
            {
                return "Sin clas";
            }
            if (esABSaldos.Value)
            {
                return "ABS Cast";
            }
            else
                return "Nvo reg.";
        }

        public static bool? ObtenerEsCarteraABSaldos(string? esABSaldos)
        {
            if (string.IsNullOrEmpty(esABSaldos) || string.IsNullOrWhiteSpace(esABSaldos))
                return null;

            if (esABSaldos.Contains("Sin clas", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (esABSaldos.Contains("ABS Cast", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (esABSaldos.Contains("Nvo reg.", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }


        public static string EstaEnArqueosSinABSaldo(bool? esSoloArqueo) 
        {
            if (esSoloArqueo is null || !esSoloArqueo.HasValue)
            {
                return "Sin clas";
            }
            if (esSoloArqueo.Value)
            {
                return "Activo";
            }
            else
                return "No Cartera";
        }

        public static bool? ObtieneEstaEnArqueosSinABSaldo(string? esSoloArqueo)
        {
            if (string.IsNullOrEmpty(esSoloArqueo) || string.IsNullOrWhiteSpace(esSoloArqueo))
                return null;

            if (esSoloArqueo.Contains("Sin clas", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (esSoloArqueo.Contains("Activo", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (esSoloArqueo.Contains("No Cartera", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        public static string EstaEnABSaldosActivo(bool? esABSaldosActivo)
        {
            if (esABSaldosActivo is null || !esABSaldosActivo.HasValue)
            {
                return "Sin clas";
            }
            if (esABSaldosActivo.Value)
            {
                return "ABS Act";
            }
            else
                return "P. Pago";
        }

        public static bool? ObtieneEstaEnABSaldosActivo(string? esABSaldosActivo)
        {
            if (string.IsNullOrEmpty(esABSaldosActivo) || string.IsNullOrWhiteSpace(esABSaldosActivo))
                return null;

            if (esABSaldosActivo.Contains("Sin clas", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (esABSaldosActivo.Contains("ABS Act", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (esABSaldosActivo.Contains("P. Pago", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        /// <summary>
        /// Si está en el arqueo con ABSaldos
        /// </summary>
        /// <param name="tieneExpediente">Para conocer el resultado del Arqueo</param>
        /// <returns></returns>
        public static string TieneExpedienteDigital(bool? tieneExpediente)
        {
            if (tieneExpediente is null || !tieneExpediente.HasValue)
            {
                return "Sn info";
            }
            if (tieneExpediente.Value)
            {
                return "Con Img";
            }
            else
                return "Por dig";
        }

        public static bool?  ObtieneTieneExpedienteDigital(string? tieneExpediente)
        {
            if (string.IsNullOrEmpty(tieneExpediente) || string.IsNullOrWhiteSpace(tieneExpediente))
                return null;

            if (tieneExpediente.Contains("Sn info", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (tieneExpediente.Contains("Con Img", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (tieneExpediente.Contains("Por dig", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return null;
        }

        public static string TieneGuardaValores(bool? seRealizoArqueo)
        {
            if (seRealizoArqueo is null || !seRealizoArqueo.HasValue)
            {
                return "Sn info";
            }
            if (seRealizoArqueo.Value)
            {
                return "Arq GV";
            }
            else
                return "No Arq";
        }
        public static bool? ObtieneTieneGuardaValores(string? seRealizoArqueo)
        {
            if (string.IsNullOrEmpty(seRealizoArqueo) || string.IsNullOrWhiteSpace(seRealizoArqueo))
                return null;

            if (seRealizoArqueo.Contains("Sn info", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (seRealizoArqueo.Contains("Arq GV", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (seRealizoArqueo.Contains("No Arq", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        public static string QuitaCastigo(this string? origen)
        {
            if (origen is null)
                return "000000000000000000";

            string digito = origen.Substring(3, 1);
            string nuevoOrigen = origen;
            if (digito == "1" || digito == "9")
            {
                nuevoOrigen = string.Concat(origen.AsSpan(0,3), origen.AsSpan(4, 1), "0", origen.AsSpan(5, 13));
            }
            return nuevoOrigen;
        }

        static string RemoveLetters(string input)
        {
            // La siguiente línea crea un objeto Regex con una expresión regular que coincide con todas las letras.
            // Regex regex = new ("[a-zA-Z]");
            Regex regex = new("[^0-9]");

            // La función Replace reemplaza todas las letras encontradas con una cadena vacía.
            string output = regex.Replace(input, string.Empty);

            return output;
        }

        public static string ObtieneNumCredito(string expediente) // , string url
        {
            /// TODO: Quitar palabras en el nombre del directorio o archivo
            char[] separadores = { ' ', '.', '|', '-', '{', '[', 'F', 'S', 'f', 's', '(', '_' };
            string[] expdientes = expediente.Split(separadores);
            // Regex regex = new("[^0-9]+");
            foreach (var expresionConEspacios in expdientes)
            {
                string expresion = RemoveLetters(expresionConEspacios).Trim();
                string separoElExpediente = expresion.Length > 17 ? expresion[..18] : ((expresion.Length < 18) && (expresion.Length > 13) ? expresion : "0"); //  
                separoElExpediente = separoElExpediente.Trim();
                #region convierto el expediente a numérico
                if (separoElExpediente.Length == 18)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente);
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                if (separoElExpediente.Length == 17)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente + '0');
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                if (separoElExpediente.Length == 16)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente + "00");
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                if (separoElExpediente.Length == 15)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente + "000");
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                if (separoElExpediente.Length == 14)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente + "0000");
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                #endregion
            }
//            _logger.LogError("Otro tipo de error en el nombre del archivo {nombreArchivo} con url {url}", expediente, url);
            return C_STR_NULLO;
        }


        public static string ObtieneExpedienteBienesAdjudicados(string bienesAdjudicados) // , string url
        {
            /// TODO: Quitar palabras en el nombre del directorio o archivo
            char[] separadores = { ' ', '.', '|', '-', '{', '[', 'F', 'S', 'f', 's', '(', '_' };
            string[] expdientes = bienesAdjudicados.Split(separadores);
            // Regex regex = new("[^0-9]+");
            foreach (var expresionConEspacios in expdientes)
            {
                string expresion = RemoveLetters(expresionConEspacios).Trim();
                string separoElExpediente = expresion.Length > 7 ? expresion[..8] : expresion; //  
                separoElExpediente = separoElExpediente.Trim();
                #region convierto el expediente a numérico
                if (separoElExpediente.Length == 8)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente);
                        return string.Format("{0:00000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                #endregion
            }
            //            _logger.LogError("Otro tipo de error en el nombre del archivo {nombreArchivo} con url {url}", expediente, url);
            return "00000000";
        }

        public static string ObtieneOtrosNumeros(string expediente, int longitud) // , string url
        {
            char[] separadores = { ' ', '.', '|', '-', '{', '[', 'F', 'S', 'f', 's', '(', '_' };
            string[] expdientes = expediente.Split(separadores);
            foreach (var expresionCompletaConEspacio in expdientes)
            {
                string expresion = RemoveLetters(expresionCompletaConEspacio).Trim();
                string separoElExpediente = expresion.Length > 17 ? expresion[..18] : ((expresion.Length < 18) && (expresion.Length > 13) ? expresion : "0"); //  
                separoElExpediente = separoElExpediente.Trim();
                #region convierto el expediente a numérico
                if (separoElExpediente.Length == longitud)
                {
                    try
                    {
                        Int64 numeroCredito = Convert.ToInt64(separoElExpediente);
                        return string.Format("{0:000000000000000000}", numeroCredito);
                    }
                    catch
                    {
                    }
                }
                #endregion
            }
            //            _logger.LogError("Otro tipo de error en el nombre del archivo {nombreArchivo} con url {url}", expediente, url);
            return C_STR_NULLO;
        }

        public static string LimpiaNoNumeros(string? nombreArchivo)
        {
            string sinCaracteresNoNumeros = "";
            if (string.IsNullOrEmpty(nombreArchivo))
                return Condiciones.C_STR_NULLO; //"000000000000000000"
            foreach (char caracter in nombreArchivo)
            {
                sinCaracteresNoNumeros += caracter switch
                {
                    '0' => '0',
                    '1' => '1',
                    '2' => '2',
                    '3' => '3',
                    '4' => '4',
                    '5' => '5',
                    '6' => '6',
                    '7' => '7',
                    '8' => '8',
                    '9' => '9',
                    _ => (object)' ',
                };
            }
            return sinCaracteresNoNumeros.Trim();

        }

        public static string EstaEnCancelado(bool? estaEnCancelados)
        {
            if (estaEnCancelados is null || !estaEnCancelados.HasValue)
            {
                return "(Sin clas)";
            }
            if (estaEnCancelados.Value)
            {
                return "CANCELADO";
            }
            else
                return "NO";
        }

        public static bool? ObtieneEstaEnCancelado(string? estaEnCancelados)
        {
            if (string.IsNullOrEmpty(estaEnCancelados) || string.IsNullOrWhiteSpace(estaEnCancelados))
                return null;

            if (estaEnCancelados.Contains("(Sin clas)", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (estaEnCancelados.Contains("CANCELADO", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (estaEnCancelados.Contains("NO", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        public static string EstaEnMesa(bool? estaEnMesa)
        {
            if (estaEnMesa is null || !estaEnMesa.HasValue)
            {
                return "(SC)";
            }
            if (estaEnMesa.Value)
            {
                return "MESA";
            }
            else
                return "NO";

        }

        public static bool? ObtieneEstaEnMesa(string? estaEnMesa)
        {
            if (string.IsNullOrEmpty(estaEnMesa) || string.IsNullOrWhiteSpace(estaEnMesa))
                return null;

            if (estaEnMesa.Contains("(SC)", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (estaEnMesa.Contains("MESA", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (estaEnMesa.Contains("NO", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        public static string EsTratamiento(bool? esTratamiento)
        {
            if (esTratamiento is null || !esTratamiento.HasValue)
            {
                return "";
            }
            if (esTratamiento.Value)
            {
                return "Tratamiento";
            }
            else
                return "";

        }

        public static bool? ObtieneEsTratamiento(string? esTratamiento)
        {
            if (string.IsNullOrEmpty(esTratamiento) || string.IsNullOrWhiteSpace(esTratamiento))
                return false;

            if (esTratamiento.Contains("(SC)", StringComparison.InvariantCultureIgnoreCase))
                return null;

            if (esTratamiento.Contains("Tratamiento", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public static string EsVerdaderoFalso(bool? esVerdadero)
        {
            if (esVerdadero is null)
                return "N";
            if (esVerdadero.Value)
                return "S";
            return "N";
        }

        public static bool ObtieneEsVerdaderoFalso(string? esVerdadero)
        {
            if (string.IsNullOrEmpty(esVerdadero))
                return false;
            return esVerdadero.Equals("S", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static string ObtieneNumCreditoCarpeta(string urlArchivo)
        {
            string resultado = Condiciones.C_STR_NULLO.ToString();
            #region Reemplazo el número de credito por el de la carpeta (si aplica)
            string[] directorios = (urlArchivo ?? "").Split('/');
            if (directorios is not null && (directorios.Length > 0))
            {
                var soloDirectoriosConPosibleNumeroDeCredito = from dir in directorios
                                                               where dir.Length> 17
                                                               select dir;

                foreach (var directorio in soloDirectoriosConPosibleNumeroDeCredito)
                {
                    if (!string.IsNullOrEmpty(directorio))
                    {
                        resultado = Condiciones.ObtieneNumCredito(directorio);
                        if (!resultado.Equals(C_STR_NULLO))
                        {
                            return resultado;
                        }
                    }
                }
            }
            #endregion            //

            return resultado;
        }
    }
}
