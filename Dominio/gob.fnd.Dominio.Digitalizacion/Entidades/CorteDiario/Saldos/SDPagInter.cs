using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos
{
    public class SDPagInter
    {
        public string? NumCredito { get; set; }
        public DateTime? FechaCuota { get; set; }
        public string? CuotaRec { get; set; }
        public string? NumCuota { get; set; }
        public string? MontoCuota { get; set; }
        public string? MontoRealPag { get; set; }
        public DateTime? FechaPag { get; set; }
        public string? FactorMoratorio { get; set; }
        public string? MontoMoratorio { get; set; }
        public DateTime? FechaMoratorio { get; set; }
        public string? DiasMoratorio { get; set; }
        public string? StatusMoratorio { get; set; }
        public string? BonifiIntMora { get; set; }
        public string? PorcPago { get; set; }
        public string? StatusCuota { get; set; }
        public string? MontoFinanciado { get; set; }

    }
}
