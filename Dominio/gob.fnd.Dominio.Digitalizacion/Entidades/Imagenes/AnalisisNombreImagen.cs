using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes
{
    public class AnalisisNombreImagen
    {
        public string? NombreOriginal { get; set; }
        public string? NombreActual { get; set; }
        public int NoCopia { get; set; }
        public bool EsContrato { get; set; }
        public bool EsFonaga { get; set; }
        public bool EsSuplemento { get; set; }
        public bool EsRiesgo { get; set; }
        public string? Remanente { get; set; }

    }
}
