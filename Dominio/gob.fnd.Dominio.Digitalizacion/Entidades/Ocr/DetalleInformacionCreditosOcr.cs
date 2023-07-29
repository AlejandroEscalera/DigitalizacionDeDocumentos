using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Ocr
{
    public class DetalleInformacionCreditosOcr
    {
        public string? NumeroDeExpediente { get; set; }
        public string? RutaDeGuardado { get; set; }
        public int Pagina { get; set; }
        public string? Leyenda { get; set; }
        public string? NumeroDeCredito { get; set; }
        public int Region { get; set; }
        public string? CatRegion { get; set; }
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public string? NumCliente { get; set; }
        public string? Acreditado { get; set; }
    }
}
