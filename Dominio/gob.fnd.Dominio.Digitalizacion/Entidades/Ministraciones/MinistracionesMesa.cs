using gob.fnd.Dominio.Digitalizacion.Entidades.BaseCredito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones
{
    public class MinistracionesMesa : CreditoBase
    {
        /// <summary>
        /// Llave del regisro a cargar
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Analista que realizó la revisión de la documentación
        /// </summary>
        public string? Analista { get; set; }
        /// <summary>
        /// Numero de ministración
        /// </summary>
        public int NumMinistracion { get; set; }
        /// <summary>
        /// Numero de solicitud
        /// </summary>
        public int NumSolicitud { get; set; }
        /// <summary>
        /// Estado de la solicitud
        /// </summary>
        public string? StaSolicitud { get; set; }
        /// <summary>
        /// Fecha de inicio de solicitud
        /// </summary>
        public DateTime? FechaInicio { get; set; }
        /// <summary>
        /// Descripción del producto solicitado
        /// </summary>
        public string? Descripcion { get; set; }
        /// <summary>
        /// Monto otorgado
        /// </summary>
        public Decimal MontoOtorgado { get; set; }
        /// <summary>
        /// Fecha de asignación
        /// </summary>
        public DateTime? FechaAsignacion { get; set; }
        /// <summary>
        /// Nombre del acreditado
        /// </summary>
        public string? Acreditado { get; set; }
        /// <summary>
        /// Regional
        /// </summary>
        public int Regional { get; set; }
        /// <summary>
        /// # de agencia
        /// </summary>
        public int Sucursal { get; set; }
        /// <summary>
        /// Agencia
        /// </summary>
        public string? CatAgencia { get; set; }
        public bool EsOrigenDelDoctor { get; set; }
        public bool EsSolicitudDoctor { get; set; }
        public bool EstaCancelado { get; set; }
        public bool EstaEnSaldosCastigos { get; set; }
        public bool EstaEnSaldosActivos { get; set; }
        public string? Castigo { get; set; }
        public bool TieneImagen { get; set; }
        public bool TieneImagenIndirecta { get; set; }
        public string? NumCreditoActual { get; set; }
        public bool EsTratamiento { get; set; }
        public bool CuentaConGuardaValores { get; set; }
    }
}
