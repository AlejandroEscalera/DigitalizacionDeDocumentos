using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Mancomunados
{
    public class MancomunadosActivos
    {
        public string? NumCredito { get; set; }
        public bool EsActivo { get; set; }
        public bool EsImpago { get; set; }
        public bool EsVencido { get; set; }
    }
}
