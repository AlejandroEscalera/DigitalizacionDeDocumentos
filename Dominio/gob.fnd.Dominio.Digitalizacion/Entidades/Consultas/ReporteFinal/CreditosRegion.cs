using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal
{
    public class CreditosRegion
    {
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public int CantidadClientesVigente { get; set; }
        public int CantidadClientesImpago { get; set; }
        public int CantidadClientesVencida { get; set; }
        public int TotalDeClientes { get; set; }
        public int CantidadDeCreditosVigente { get; set; }
        public int CantidadDeCreditosImpago { get; set; }
        public int CantidadDeCreditosVencida { get; set; }
        public int TotalDeCreditos { get; set; }
        public decimal SaldosCarteraVigente { get; set; }
        public decimal SaldosCarteraImpago { get; set; }
        public decimal SaldosCarteraVencida { get; set; }
        public decimal ICV { get; set; }
        public decimal SaldosTotal { get; set; }
    }
}
