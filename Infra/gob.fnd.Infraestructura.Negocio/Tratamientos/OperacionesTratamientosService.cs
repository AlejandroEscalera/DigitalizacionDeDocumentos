using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado;
using gob.fnd.Dominio.Digitalizacion.Excel.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using gob.fnd.Dominio.Digitalizacion.Negocio.Tratamientos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.Tratamientos
{
    public class OperacionesTratamientosService : IOperacionesTratamientos
    {
        private readonly ILogger<OperacionesTratamientosService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAdministraTratamientoBase _administraTratamientoBaseService;
        private readonly string _archivoTratamientoBase;
        private IEnumerable<ConversionTratamientos>? _tratamientos;

        public OperacionesTratamientosService(ILogger<OperacionesTratamientosService> logger, IConfiguration configuration, IAdministraTratamientoBase administraTratamientoBaseService)
        {
            _logger = logger;
            _configuration = configuration;
            _administraTratamientoBaseService = administraTratamientoBaseService;

            _archivoTratamientoBase = _configuration.GetValue<string>("archivoTratamientoBase") ?? @"C:\202302 Digitalizacion\2. Saldos procesados\Tratamientos\TratamientosBase.csv";
        }
        private IEnumerable<ConversionTratamientos> CargaConversionTratamientos()
        {
            if (_tratamientos == null )
            {
                _tratamientos = _administraTratamientoBaseService.GetConversionTratamientosCsv(_archivoTratamientoBase);
                _logger.LogInformation("Se cargó la lista para la conversión de tratamientos");                
            }
            return _tratamientos;
        }

        public IEnumerable<string> ObtieneTratamientos(string numCredito)
        {
            CargaConversionTratamientos();

            IList<string> listaCreditos = new List<string>();
            if (string.IsNullOrEmpty(numCredito))
                return listaCreditos.ToArray();
            var listaTratamientos = _tratamientos?.Where(x => (x.Destino ?? "").Equals(numCredito)).ToList();
            if (listaTratamientos is not null)
            {
                foreach (var tratamiento in listaTratamientos)
                {
                    string numCreditoDestino = tratamiento.Origen ?? "";
                    listaCreditos.Add(numCreditoDestino);
                    var listaCreditosRecursivo = ObtieneTratamientos(numCreditoDestino).ToList();
                    foreach (var creditoRecursivo in listaCreditosRecursivo)
                    {
                        listaCreditos.Add(creditoRecursivo);
                    }
                }
            }
            return listaCreditos.Distinct().ToArray();
        }

        public IEnumerable<string> ObtieneTratamientosOrigen(string numCredito)
        {
            CargaConversionTratamientos();

            IList<string> listaCreditos = new List<string>();
            var listaTratamientos = _tratamientos?.Where(x => (x.Origen ?? "").Equals(numCredito)).ToList();
            if (listaTratamientos is not null)
            {
                foreach (var tratamiento in listaTratamientos)
                {
                    string numCreditoNuevo = tratamiento.Destino ?? "";
                    listaCreditos.Add(numCreditoNuevo);
                    if (numCreditoNuevo.Length > 3 && numCreditoNuevo.Substring(3, 1).Equals("8"))
                    {
                        var listaCreditosRecursivo = ObtieneTratamientosOrigen(numCreditoNuevo).ToList();
                        foreach (var creditoRecursivo in listaCreditosRecursivo)
                        {
                            listaCreditos.Add(creditoRecursivo);
                        }
                    }
                }
            }
            return listaCreditos.Distinct().ToArray();
        }
    }
}
