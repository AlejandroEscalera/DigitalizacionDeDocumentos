using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class CruceLiquidadosBienesAdjudicados
    {
        public string? NumExpediente { get; set; }
        public string? Adjudicado { get; set; }
        public string? NumCredito { get; set; }
        public bool TieneInfo { get; set; }
        public bool TieneImg { get; set; }
        public bool SeSolicitoLiquidado { get; set; }
        public bool SeTieneLiquidado { get; set; }

    }
}
