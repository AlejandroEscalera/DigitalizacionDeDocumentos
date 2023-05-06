using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace System.Security
{
    public static class FirmaArchivo
    {
        public static string ObtieneHashDelArchivo(string rutaArchivo)
        {
            StringBuilder sb = new();

            // Crear un objeto SHA256 para calcular el hash del archivo
            using (SHA256 sha256 = SHA256.Create())
            {
                // Abrir el archivo y calcular el hash
                using FileStream fs = File.OpenRead(rutaArchivo);
                byte[] hash = sha256.ComputeHash(fs);

                // Convertir el hash a una cadena de texto hexadecimal
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
            }
            // Regresa el hash en la consola
            return sb.ToString();
        }

        public static string ObtieneHashDelArchivo(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return string.Empty;
            }
            return ObtieneHashDelArchivo(fileInfo.FullName);
        }
    }
}
