using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas
{
    public class ExpCasAgencias
    {
        public int NumRegion { get; set; }
        public int NumAgencia { get; set; }
        public string? CatAgencia { get; set; }
        public Decimal MontoCredito { get; set; }
        public Decimal InteresCont { get; set; }
        public int TotalExpCastigados { get; set; }
        public int TotalReportos { get; set; }
        public int TotalConArqueos { get; set; }
        public int TotalCancelados { get; set; }
        public int TotalActivos { get; set; }

    }
}
