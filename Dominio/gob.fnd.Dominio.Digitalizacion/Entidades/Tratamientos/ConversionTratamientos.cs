using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos
{
    public class ConversionTratamientos
    {
        public int Id { get; set; }
        public string? NumCte { get; set; }
        public string? Origen { get; set; }
        public string? Destino { get; set; }
    }
}
