using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteLiquidados
{
    public class CreditosLiquidadosDetalle
    {
        public int Region { get; set; }
        public int Agencia { get; set; }
        public string? NumCte { get; set; }
        public string? Acreditado { get; set; }
        public string? NumCredito { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaVencim { get; set; }
        public decimal MontoOtorgado { get; set; }
        public int Ministraciones { get; set; }
        public DateTime? FecPrimMinistra { get; set; }
        public DateTime? FecUltimaMinistra { get; set; }
        public decimal MontoMinistrado { get; set; }
        public string? Reestructura { get; set; }
        public string? Cancelacion { get; set; }
        public decimal PagoCapital { get; set; }
        public decimal PagoInteres { get; set; }
        public decimal PagoMoratorios { get; set; }
        public decimal PagoTotal { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
    }
}
