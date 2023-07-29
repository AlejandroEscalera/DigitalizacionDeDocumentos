using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class CruceFuenteVsIdentificacionClaveBien : CruceFuenteBAvsExpedientes
    {
        public string? CveBienI { get; set; }
        public string? CrI { get; set; }
        public string? AcreditadoI { get; set; }
        public string? TipoBienI { get; set; }
        public string? NumCreditoI { get; set; }
        public string? ObservacionesI { get; set; }
    }
}
