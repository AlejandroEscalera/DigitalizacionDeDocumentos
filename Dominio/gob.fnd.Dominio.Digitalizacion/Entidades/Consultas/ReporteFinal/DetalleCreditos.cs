using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal
{
    public class DetalleCreditos
    {
        public int Region { get; set; }
        public int Agencia { get; set; }
        public DateTime? FechaApertura { get; set; }
        public string? NumCredito { get; set; }
        public string? Acreditado { get; set; }
        public decimal SaldoCarteraVigente { get; set; }
        public decimal SaldoCarteraImpagos { get; set; }
        public decimal SaldosCarteraVencida { get; set; }
        public decimal SaldosTotal { get; set; }
        public bool EsVigente { get; set; }
        public bool EsImpago { get; set; }
        public bool EsVencida { get; set; }
        public bool EsOrigenDelDr { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
        public bool TieneImagenExpediente { get; set; }
    }
}
