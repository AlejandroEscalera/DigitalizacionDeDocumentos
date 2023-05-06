using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados
{
    public class CreditosCancelados
    {
        public int Id { get; set; }
        public string? NumCreditoActual { get; set; }
        public string? NumCreditoNvo { get; set; }
        public string? NumCreditoOrigen { get; set; }
        public string? NumCliente { get; set; }
        public string? Genero { get; set; }
        public string? TipoPersona { get; set; }
        public string? Portafolio { get; set; }
        public string? SesionAutorizacion { get; set; }
        public string? MesCastigo { get; set; }
        public int AnioCastigo { get; set; }
        public string? Generacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? Entidad { get; set; }
        public string? CoordinacionRegional { get; set; }
        public string? Agencia { get; set; }
        public string? Acreditado { get; set; }
        public string? Cartera { get; set; }
        public string? Concepto { get; set; }
        public string? CuentaCredito { get; set; }
        public decimal ImporteCancelado { get; set; }
        public decimal SaldoContable { get; set; }
        public DateTime? PrimeraDispersion { get; set; }
        public int AnioOrignacion { get; set; }
        public string? Observacion { get; set; }
        public string? TipoDeOperacion { get; set; }
        public int Piso { get; set; }
        public bool EsOrigenDelDoctor { get; set; }
        public bool EsCancelacionDelDoctor { get; set; }
        public bool EstaSaldosCancelados { get; set; }
        public bool EstaEnCreditosCastigados { get; set; }
        public bool EstaEnCreditosActivos { get; set; }
        public bool TieneImagen { get; set; }
        public string? Castigo { get; set; }
        public bool EsTratamiento { get; set; }
    }
}
