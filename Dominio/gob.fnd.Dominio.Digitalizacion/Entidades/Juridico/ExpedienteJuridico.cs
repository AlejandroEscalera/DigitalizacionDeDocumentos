using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Juridico
{
    public class ExpedienteJuridico
    {
        public int Region { get; set; }
        public string? CatRegion { get; set; }
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public string? Estado { get; set; }
        public string? NombreDemandado { get; set; }
        public string? TipoCredito { get; set; }
        public string? NumContrato { get; set; }
        public string? NumCte { get; set; }
        public string? NumCredito { get; set; }
        public string? NumCreditos { get; set; }
        public DateTime? FechaTranspasoJuridico { get; set; }
        public DateTime? FechaTranspasoExterno { get; set; }
        public DateTime? FechaDemanda { get; set; }
        public string? Juicio { get; set; }
        public decimal CapitalDemandado { get; set; }
        public decimal Dlls { get; set; }
        public string? JuzgadoUbicacion { get; set; }
        public string? Expediente { get; set; }
        public string? ClaveProcesal { get; set; }
        public string? EtapaProcesal { get; set; }
        public string? Rppc { get; set; }
        public string? Ran { get; set; }
        public string? RedCredAgricola { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? Tipo { get; set; }
        public string? TipoGarantias { get; set; }
        public string? Descripcion { get; set; }
        public decimal ValorRegistro { get; set; }
        public DateTime? FechaAvaluo { get; set; }
        public string? GradoPrelacion { get; set; }
        public string? CAval { get; set; }
        public string? SAval { get; set; }
        public string? Inscripcion { get; set; }
        public string? Clave { get; set; }
        public string? NumFolio { get; set; }
        public string? AbogadoResponsable { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaUltimaActuacion { get; set; }
        public string? DescripcionActuacion { get; set; }
        public string? Piso { get; set; }
        public string? Fonaga { get; set; }
        public string? PequenioProductor { get; set; }
        public string? FondoMutual { get; set; }
        public string? ExpectativasRecuperacion { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
        public bool TieneImagenExpediente { get; set; }

    }
}
