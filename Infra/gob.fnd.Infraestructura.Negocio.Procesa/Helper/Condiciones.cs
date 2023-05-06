using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Procesa.Helper
{
    public static class Condiciones
    {

        public const string C_STR_NULLO = "000000000000000000";
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

        static string RemoveLetters(string input)
        {
            // La siguiente línea crea un objeto Regex con una expresión regular que coincide con todas las letras.
            // Regex regex = new ("[a-zA-Z]");
            Regex regex = new("[^0-9]");

            // La función Replace reemplaza todas las letras encontradas con una cadena vacía.
            string output = regex.Replace(input, string.Empty);

            return output;
        }

        internal static string ObtieneNumCreditoCarpeta(string urlArchivo)
        {
            string resultado = Condiciones.C_STR_NULLO.ToString();
            #region Reemplazo el número de credito por el de la carpeta (si aplica)
            string[] directorios = (urlArchivo ?? "").Split('/');
            if (directorios is not null && (directorios.Length > 0))
            {
                #region Obtengo el número de contrato
                var soloDirectoriosConPosibleNumeroDeCredito = from dir in directorios
                                                               where dir.Length > 17
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
                #endregion

                #region Incluyo # de contrato de 14 posibles dígitos
                soloDirectoriosConPosibleNumeroDeCredito = from dir in directorios
                                                           where dir.Length > 13
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
                #endregion
            }
            #endregion            //

            return resultado;
        }
    }
}
