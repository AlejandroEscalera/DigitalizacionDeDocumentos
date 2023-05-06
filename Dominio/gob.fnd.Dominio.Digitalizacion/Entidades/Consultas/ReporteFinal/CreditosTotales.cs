using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal
{
    public class CreditosTotales
    {
        public int CreditosVigentes { get; set; }
        public int Impagos { get; set; }
        public int CarteraVencida { get; set; }
        public int TotalDeCreditos { get; set; }
    }
}
