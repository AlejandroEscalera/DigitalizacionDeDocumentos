using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.Saldos
{
    public class SDMovDia
    {
        public string? Secuencia { get; set; }
        public DateTime? FechaMov { get; set; }
        public string? HoraMov { get; set; }
        public string? Sucursal { get; set; }
        public string? NumCredito { get; set; }
        public string? Plaza { get; set; }
        public string? TransaccSuc { get; set; }
        public string? Usuario { get; set; }
        public string? Monto { get; set; }
        public string? CodigoFun { get; set; }
        public string? CodigoRef { get; set; }
        public string? Divisa { get; set; }
        public string? Reversado { get; set; }
        public string? FolioSuc { get; set; }
        public string? NumProducto { get; set; }

    }
}
