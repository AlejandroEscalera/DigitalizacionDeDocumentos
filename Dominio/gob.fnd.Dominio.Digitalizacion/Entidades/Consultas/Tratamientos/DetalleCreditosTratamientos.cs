using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.Tratamientos
{
    public class DetalleCreditosTratamientos : DetalleCreditos
    {
        public string? CreditoOrigen { get; set; }
    }
}
