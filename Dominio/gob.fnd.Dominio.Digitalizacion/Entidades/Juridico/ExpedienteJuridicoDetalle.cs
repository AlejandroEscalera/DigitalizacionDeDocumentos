using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Juridico
{
    public class ExpedienteJuridicoDetalle
    {
        public int Region { get; set; }
        public int Agencia { get; set; }
        public string? Estado { get; set; }
        public string? NumContrato { get; set; }
        public string? NombreDemandado { get; set; }
        public string? NumCte { get; set; }
        public string? TipoCredito { get; set; }
        public string? NumCredito { get; set; }
        public string? NumCreditos { get; set; }
        public DateTime? FechaTranspasoJuridico { get; set; }
        public DateTime? FechaTranspasoExterno { get; set; }
        public DateTime? FechaDemanda { get; set; }
        public string? Juicio { get; set; }
        public string? TipoGarantias { get; set; }
        public decimal CapitalDemandado { get; set; }
        public string? Fonaga { get; set; }
        public string? PequenioProductor { get; set; }
        public string? FondoMutual { get; set; }
        public string? ExpectativasRecuperacion { get; set; }
        public string? Expediente { get; set; }
        public string? AbogadoResponsable { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
        public bool TieneImagenExpediente { get; set; }
    }
}
