using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes
{
    public class ArchivoImagenBienesAdjudicadosCorta
    {
        public int Id { get; set; }
        public string? NumExpediente { get; set; }
        public string? Acreditados { get; set; }
        public string? NombreArchivo { get; set; }
        public string? CarpetaDestino { get; set; }
        public int NumPaginas { get; set; }
        public string? Hash { get; set; }
    }
}
