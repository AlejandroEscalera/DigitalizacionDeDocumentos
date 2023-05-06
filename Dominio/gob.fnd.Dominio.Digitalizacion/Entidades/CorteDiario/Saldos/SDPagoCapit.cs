using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos
{
    public class SDPagoCapit
    {
        public string? NumCredito { get; set; }
        public DateTime FechaCuota { get; set; }
        public string? CuotaRec { get; set; }
        public int NumCuota { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal SaldoCuota { get; set; }
        public decimal ImpCapitalizado { get; set; }
        public decimal FactorAjuste { get; set; }
        public decimal MontoRealPag { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal FactorMoratorio { get; set; }
        public decimal MontoMoratorio { get; set; }
        public DateTime? FechaMoratorio { get; set; }
        public int DiasMoratorios { get; set; }
        public string? StatusMoratorio { get; set; }
        public int NumPagares { get; set; }
        public decimal PorcPago { get; set; }
        public string? BanderaMinistra { get; set; }
        public string? StatusCuota { get; set; }

    }
}
