using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen
{
    public partial class MainFRM
    {
        private WindowsFormsGlobalInformation? _windowsFormsGloablInformation;
        private IEnumerable<ExpedienteDeConsulta>? _infoExpedientes;
        private IEnumerable<ArchivoImagenCorta>? _archivosImagenes;
        //private IEnumerable<ArchivoImagenBienesAdjudicadosCorta>? _archivoImagenBienesAdjudicados;
        private IEnumerable<ExpCasRegiones>? _expCasRegiones;
        private IEnumerable<ExpCasAgencias>? _expCasAgencias;
        private IEnumerable<ExpCasExpedientes>? _expCasExpedientes;
        private Microsoft.Extensions.Logging.ILogger? _logger;

        public void CargaInformacion() {
            _windowsFormsGloablInformation = WindowsFormsGlobalInformation.Instance;
            _windowsFormsGloablInformation.LlenaInformacionGlobal();
            _logger = _windowsFormsGloablInformation.Logger;
            try
            {
                _logger.LogInformation("Comienzo a cargar la información de los saldos");
                _infoExpedientes = _windowsFormsGloablInformation.ActivaConsultasServices().CargaInformacion(); // Carga información
                _unidadDeDiscoImagenes = _windowsFormsGloablInformation.ActivaConsultasServices().ObtieneUnidadImagenes();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al Cargar la informacion de los saldos {mensaje}", ex.Message);
                throw;
            }
            try
            {
                _logger.LogInformation("Comienzo a cargar la información de imágenes");
                _archivosImagenes = _windowsFormsGloablInformation.ActivaConsultasServices().CargaInformacionImagenes(); // Carga información de imágenes
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al Cargar la informacion de imagenes {mensaje}", ex.Message);
                throw;
            }
            try
            {
                _logger.LogInformation("Comienzo a hacer el cálculo de las regiones");
                _expCasRegiones = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaRegiones(_infoExpedientes);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al calcular las regiones {mensaje}", ex.Message);
                throw;
            }
            try
            {
                _logger.LogInformation("Comienzo a cargar las imágnees de los Bienes Adjudicados");
                _ = _windowsFormsGloablInformation.ActivaConsultasServices().CargaInformacionImagenesBienesAdjudicados();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al cargar las imágenes de los Bienes adjudicados", ex.Message);
                throw;
            }
        }

        public static string QuitaCastigo(string? origen)
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
        public IEnumerable<ExpedienteDeConsulta> BuscaPorNumeroDeCredito(string numeroDeCredito)
        {
            if (_infoExpedientes is null) {
                Close();
                throw new Exception("Tienen que existir expedientes");
            }
            var resultado = _infoExpedientes.Where(x => (x.NumCredito??"").Equals(numeroDeCredito));
            #region Busco por cancelados
            if (!resultado.Any())
            {
                resultado = _infoExpedientes.Where(x => (x.NumCreditoCancelado ?? "").Equals(numeroDeCredito));
            }
            if (!resultado.Any())
            {
                resultado = _infoExpedientes.Where(x => (x.NumCredito ?? "").Equals(QuitaCastigo(numeroDeCredito)));
            }

            #endregion
            if (!resultado.Any()) {
                numeroDeCredito = numeroDeCredito.TrimEnd('0').PadRight(14,'0');
                if (!string.IsNullOrEmpty(numeroDeCredito))
                {
                    resultado = _infoExpedientes.Where(x => (x.NumCredito ?? "").StartsWith(numeroDeCredito));
                }
                // Comienza búsqueda parcial
            }

            return resultado;
        }

        public IEnumerable<ExpedienteDeConsulta> BuscaPorAcreditado(string nombreDeAcreditado)
        {
            if (_infoExpedientes is null)
            {
                Close();
                throw new Exception("Tienen que existir expedientes");
            }
            string[] datosBusquedaAcreditado = nombreDeAcreditado.Split(' ');
            var resultado = _infoExpedientes;
            foreach (var datoBusqueda in datosBusquedaAcreditado)
            { 
                resultado = resultado.Where(x=>(x.Acreditado??"").Contains(datoBusqueda,StringComparison.InvariantCultureIgnoreCase));
            }
            return resultado;
        }

    }
}
