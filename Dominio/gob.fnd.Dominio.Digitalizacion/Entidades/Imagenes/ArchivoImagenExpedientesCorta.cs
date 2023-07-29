using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes
{
    public class ArchivoImagenExpedientesCorta
    {
        public int Id { get; set; }
        public string? NumContrato { get; set; }
        public string? NumCredito { get; set; }
        public string? NombreArchivo { get; set; }
        public string? CarpetaDestino { get; set; }
        public int NumPaginas { get; set; }
        public string? Hash { get; set; }
        public bool EsTurno { get; set; }
        public bool EsCobranza { get; set; }
    }
}
