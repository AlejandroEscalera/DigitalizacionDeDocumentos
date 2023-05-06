using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class AdministraABSaldosDiarioService : IAdministraABSaldosDiario
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdministraABSaldosDiarioService> _logger;
        private readonly string _archivoABSaldosDiarioOrigen;
        private readonly string _archivoABSaldosDiarioDestino;
        private readonly string _archivoSaldosDiarioProcesados;

        public AdministraABSaldosDiarioService(IConfiguration configuration, ILogger<AdministraABSaldosDiarioService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _archivoABSaldosDiarioOrigen = _configuration.GetValue<string>("archivoABSaldosDiarioOrigen")??"";
            _archivoABSaldosDiarioOrigen = String.Format(_archivoABSaldosDiarioOrigen, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);            
            _archivoABSaldosDiarioDestino = _configuration.GetValue<string>("archivoABSaldosDiarioDestino") ?? "";
            _archivoABSaldosDiarioDestino = String.Format(_archivoABSaldosDiarioDestino, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _archivoSaldosDiarioProcesados = _configuration.GetValue<string>("archivoSaldosDiarioProcesados") ?? "";
            _archivoSaldosDiarioProcesados = String.Format(_archivoSaldosDiarioProcesados, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
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
                return true;
            try
            {
                File.Copy(origen, destino, true);
            }
            catch
            {
                _logger.LogInformation("Falló la copia el AB Saldos diario desde {origen} a {destino}", origen, destino);
                return false;
            }
            return true;
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

    }
}
