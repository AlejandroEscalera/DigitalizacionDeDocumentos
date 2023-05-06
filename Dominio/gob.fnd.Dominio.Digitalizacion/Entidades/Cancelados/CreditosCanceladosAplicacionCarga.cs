using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados
{
    public class CreditosCanceladosAplicacionCarga
    {
        public string? NumCreditoCancelado { get; set; }
        public string? NumCreditoOrigen { get; set; }
        public int NumRegion { get; set; }
        public string? CatRegion { get; set; }
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public string? NumCliente { get; set; }
        public string? Acreditado { get; set; }
        public string? TipoPersona { get; set; }
        public int AnioOriginacion { get; set; }
        public int PisoCredito { get; set; }
        public string? Portafolio { get; set; }
        public string? Concepto { get; set; }
        public string? FechaCancelacion { get; set; }
        public string? FechaPrimeraDispersion { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }

    }
}
