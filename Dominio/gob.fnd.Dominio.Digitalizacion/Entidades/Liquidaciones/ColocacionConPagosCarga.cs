using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Liquidaciones
{
    public class ColocacionConPagosCarga
    {
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public string? NumCte { get; set; }
        public string? Acreditado { get; set; }
        public string? NumCredito { get; set; }
        public string? FechaApertura { get; set; }
        public string? FechaVencim { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string? CatRegionMigrado { get; set; }
        public string? CatEstadoMigrado { get; set; }
        public string? CatAgenciaMigrado { get; set; }
        public int Ministraciones { get; set; }
        public string? FecPrimMinistra { get; set; }
        public string? FecUltimaMinistra { get; set; }
        public decimal MontoMinistrado { get; set; }
        public string? Reestructura { get; set; }
        public string? Cancelacion { get; set; }
        public decimal? PagoCapital { get; set; }
        public decimal? PagoInteres { get; set; }
        public decimal? PagoMoratorios { get; set; }
        public decimal? PagoTotal { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }

    }
}
