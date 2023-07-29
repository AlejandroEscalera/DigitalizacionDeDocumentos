using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado
{
    public class Tratamientos
    {
        public string? CatRegion { get; set; }
        public string? CatEstado { get; set; }
        public string? CatAgencia { get; set; }
        public string? NumCte { get; set; }
        public string? NombreCliente { get; set; }
        public string? NumCredito { get; set; }
        public string? TipoTratamiento { get; set; }
        public string? CreditoOrigen { get; set; }
        public int PagosRecibidos { get; set; }
        public DateTime? FecPrimPago { get; set; }
        public DateTime? FecUltimoPago { get; set; }
        public decimal PagoCapital { get; set; }
        public decimal PagoInteres { get; set; }
        public decimal PagoMoratorios { get; set; }
        public decimal PagoTotal { get; set; }
    }
}