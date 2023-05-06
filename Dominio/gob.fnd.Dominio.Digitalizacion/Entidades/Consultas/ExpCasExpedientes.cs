using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas
{
    public class ExpCasExpedientes
    {
        public int NumRegion { get; set; }
        public int NumAgencia { get; set; }
        public string? NumCredito { get; set; }
        public string? Acreditado { get; set; }
        public Decimal MontoCredito { get; set; }
        public Decimal InteresCont { get; set; }
        public string? Castigo { get; set; }
        public string? Reporto { get; set; }
        public bool CuentaConArqueo { get; set; }
        public bool EstaCancelado { get; set; }
        public bool EstaActivo { get; set; }
    }
}
