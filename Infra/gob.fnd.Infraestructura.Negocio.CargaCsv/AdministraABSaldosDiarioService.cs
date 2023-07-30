using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.ZIP;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraABSaldosDiarioService : IAdministraABSaldosDiario
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdministraABSaldosDiarioService> _logger;
        private readonly IComprimeArchivoZIP _comprimeArchivoZIPService;
        private readonly IDescomprimeArchivoZip _descomprimeArchivoZipService;
        private readonly string _archivoABSaldosDiarioOrigen;
        private readonly string _archivoABSaldosDiarioDestino;
        private readonly string _archivoSaldosDiarioProcesados;
        private readonly string _rutaDeDescarga;

        public AdministraABSaldosDiarioService(IConfiguration configuration, ILogger<AdministraABSaldosDiarioService> logger,
            IComprimeArchivoZIP comprimeArchivoZIPService, IDescomprimeArchivoZip descomprimeArchivoZipService)
        {
            _configuration = configuration;
            _logger = logger;
            _comprimeArchivoZIPService = comprimeArchivoZIPService;
            _descomprimeArchivoZipService = descomprimeArchivoZipService;
            _archivoABSaldosDiarioOrigen = _configuration.GetValue<string>("archivoABSaldosDiarioOrigen")??"";
            _archivoABSaldosDiarioOrigen = String.Format(_archivoABSaldosDiarioOrigen, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);            
            _archivoABSaldosDiarioDestino = _configuration.GetValue<string>("archivoABSaldosDiarioDestino") ?? "";
            _archivoABSaldosDiarioDestino = String.Format(_archivoABSaldosDiarioDestino, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _archivoSaldosDiarioProcesados = _configuration.GetValue<string>("archivoSaldosDiarioProcesados") ?? "";
            _archivoSaldosDiarioProcesados = String.Format(_archivoSaldosDiarioProcesados, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _rutaDeDescarga = _configuration.GetValue<string>("rutaDeDescarga") ?? "";
            // _rutaDeDescarga = String.Format(_rutaDeDescarga, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        private static bool ExisteServidor(string ruta)         
        {
            Uri uri = new (ruta);
            string nombreServidor = uri.Host;
            Ping pingSender = new ();
            //PingOptions options = new ();
            try {
                PingReply reply = pingSender.Send(nombreServidor, 5000);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch // (PingException ex) 
            {

            }
            return false;
        }

        public bool CopiaABSaldosDiario(string origen = "", string destino = "")
        {
            if (string.IsNullOrEmpty(origen))
            {
                origen = _archivoABSaldosDiarioOrigen;
            }
            if (string.IsNullOrEmpty(destino)) { 
                destino = _archivoABSaldosDiarioDestino;
            }
            _logger.LogInformation("Comienza la copia el AB Saldos diario desde {origen} a {destino}", origen, destino);
            if (File.Exists(destino))
                return false;
            try
            {
                if (ExisteServidor(_archivoABSaldosDiarioOrigen))
                {
                    // Copia el archivo origen del servidor de cierres diarios a la carpeta local
                    File.Copy(origen, destino, true);
                    // Archivo publicado en página web, para evitar que requieran el permiso a la carpeta de los cortes diarios
                    string destinoInterno = String.Format("\\\\192.168.20.4\\ParqueFundidora\\Saldos\\ABSDO{0:0000}{1:00}{2:00}.zip", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    string infoDestinoIntero = "\\\\192.168.20.4\\ParqueFundidora\\Saldos\\LastABSDO.txt";
                    
                    if (ExisteServidor(destinoInterno))
                    {
                        _comprimeArchivoZIPService.ComprimirArchivo(destino, destinoInterno, "1234");
                        if (File.Exists(infoDestinoIntero))
                        { 
                            File.Delete(infoDestinoIntero);
                        }
                        File.WriteAllText(infoDestinoIntero, Path.GetFileName(destinoInterno));
                        // File.Copy(destino, destinoInterno, true);
                    }
                    return true;
                }
            }
            catch
            {
                _logger.LogInformation("Falló la copia el AB Saldos diario desde {origen} a {destino}", origen, destino);
            }
            return false;
        }

        public bool DescargaABSaldosDiario(string origen = "", string destino = "")
        {

            string url = _rutaDeDescarga+ "LastABSDO.txt";
            string archivoABSaldosDiarioDestinoZip = Path.ChangeExtension(_archivoABSaldosDiarioDestino, "ZIP");            

            // Crea una nueva instancia de WebClient
            using HttpClient cliente = new();
            try
            {
                // Descarga el archivo como un array de bytes.
                byte[] archivo = cliente.GetByteArrayAsync(url).Result;
                string nombreArchivoDescarga = Encoding.UTF8.GetString(archivo);
                archivoABSaldosDiarioDestinoZip = Path.Combine(Path.GetDirectoryName(archivoABSaldosDiarioDestinoZip)?? "", nombreArchivoDescarga);
                url = _rutaDeDescarga + nombreArchivoDescarga;

                archivo = cliente.GetByteArrayAsync(url).Result;
                if (File.Exists(archivoABSaldosDiarioDestinoZip))
                    File.Delete(archivoABSaldosDiarioDestinoZip);
                File.WriteAllBytes(archivoABSaldosDiarioDestinoZip, archivo);
                _logger.LogInformation("Archivo descargado con éxito en {ruta} ", Path.GetDirectoryName(archivoABSaldosDiarioDestinoZip));
                _descomprimeArchivoZipService.DescomprimeArchivo(archivoABSaldosDiarioDestinoZip, Path.GetDirectoryName(_archivoABSaldosDiarioDestino)??"");
                return true;
            }
            catch (Exception e)
            {
                // Si hay un error durante la descarga, imprime el mensaje de error.
                _logger.LogError("Error al descargar el archivo:  {archivo} ", e.Message);
            }
            //_archivoABSaldosDiarioDestino
            // _rutaDeDescarga
            // _descomprimeArchivoZipService
            return false;

        }

        public bool MueveABSaldosDiario(string origen = "", string destino = "")
        {
            if (string.IsNullOrEmpty(origen))
            {
                origen = _archivoABSaldosDiarioDestino;
            }
            if (string.IsNullOrEmpty(destino))
            {
                destino = _archivoSaldosDiarioProcesados;
            }
            _logger.LogInformation("Comienza mover la información del AB Saldos diario desde {origen} a {destino}", origen, destino);
            if (File.Exists(destino))
                return true;
            try
            {
                File.Move(origen, destino, true);
            }
            catch
            {
                _logger.LogInformation("Falló mover la información del AB Saldos diario desde {origen} a {destino}", origen, destino);
                return false;
            }
            return true;
        }


        public async Task<bool> CopiaABSaldosDiarioAync(string origen = "", string destino = "")
        {
            if (string.IsNullOrEmpty(origen))
            {
                origen = _archivoABSaldosDiarioOrigen;
            }
            if (string.IsNullOrEmpty(destino))
            {
                destino = _archivoABSaldosDiarioDestino;
            }
            _logger.LogInformation("Comienza la copia el AB Saldos diario desde {origen} a {destino}", origen, destino);
            // Si ya existe el archivo, no lo copia
            if (File.Exists(destino))
                return false;
            try
            {
                if (ExisteServidor(_archivoABSaldosDiarioOrigen))
                {
                    // Copia el archivo origen del servidor de cierres diarios a la carpeta local
                    await Task.Run(()=>File.Copy(origen, destino, true));
                    // Archivo publicado en página web, para evitar que requieran el permiso a la carpeta de los cortes diarios
                    string destinoInterno = String.Format("\\\\192.168.20.4\\ParqueFundidora\\Saldos\\ABSDO{0:0000}{1:00}{2:00}.zip", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    // Archivo de control que dice que día se puede descargar
                    string infoDestinoIntero = "\\\\192.168.20.4\\ParqueFundidora\\Saldos\\LastABSDO.txt";

                    if (ExisteServidor(destinoInterno))
                    {
                        // Se comprime el archivo para que la descarga sea menos pesada
                        await Task.Run(() => _comprimeArchivoZIPService.ComprimirArchivo(destino, destinoInterno, "1234") );
                        // Elimino el archivo de control
                        if (File.Exists(infoDestinoIntero))
                        {
                            File.Delete(infoDestinoIntero);
                        }
                        // Creo el archivo de control
                        await Task.Run(() => File.WriteAllText(infoDestinoIntero, Path.GetFileName(destinoInterno)));
                        // File.Copy(destino, destinoInterno, true);
                    }
                    return true;
                }
            }
            catch
            {
                _logger.LogInformation("Falló la copia el AB Saldos diario desde {origen} a {destino}", origen, destino);
            }
            return false;
        }


        public async Task<bool> DescargaABSaldosDiarioAync(string origen = "", string destino = "")
        {

            string url = _rutaDeDescarga + "LastABSDO.txt";
            string archivoABSaldosDiarioDestinoZip = Path.ChangeExtension(_archivoABSaldosDiarioDestino, "ZIP");

            // Crea una nueva instancia de WebClient
            using HttpClient cliente = new();
            try
            {
                // Descarga el archivo como un array de bytes.
                byte[] archivo = await cliente.GetByteArrayAsync(url);
                string nombreArchivoDescarga = Encoding.UTF8.GetString(archivo);
                url = _rutaDeDescarga + nombreArchivoDescarga;

                archivo = await cliente.GetByteArrayAsync(url);

                File.WriteAllBytes(archivoABSaldosDiarioDestinoZip, archivo);
                _logger.LogInformation("Archivo descargado con éxito en {ruta} ", Path.GetDirectoryName(archivoABSaldosDiarioDestinoZip));
                await Task.Run(() =>  _descomprimeArchivoZipService.DescomprimeArchivo(archivoABSaldosDiarioDestinoZip, Path.GetDirectoryName(_archivoABSaldosDiarioDestino) ?? ""));
                return true;
            }
            catch (Exception e)
            {
                // Si hay un error durante la descarga, imprime el mensaje de error.
                _logger.LogError("Error al descargar el archivo:  {archivo} ", e.Message);
            }
            return false;
        }
    }
}
