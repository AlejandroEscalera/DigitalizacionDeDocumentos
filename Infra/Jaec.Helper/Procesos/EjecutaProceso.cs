using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaec.Helper.Procesos
{
    public static class EjecutaProceso
    {
        /// <summary>
        /// Ejecuta un proceso asincrono
        /// </summary>
        /// <param name="comando">Comando directo de DOS a ejecutar</param>
        /// <returns>El resultado de la ejecución</returns>
        public static async Task<string> EjecutaProcesoAsyc(string comando)
        {
            string command = "cmd";
            Process process = new();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = "/c "+ comando;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850); // Encoding.GetEncoding("Windows-1252");// Encoding.GetEncoding("ISO-8859-1");
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }

    }
}
