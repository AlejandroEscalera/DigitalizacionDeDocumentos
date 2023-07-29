using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores
{
    public class GuardaValores
    {
        /// <summary>
        /// Número de region
        /// </summary>
        public int Regional { get; set; }
        /// <summary>
        /// Nombre de la región
        /// </summary>
        public string? CatRegional { get; set; }
        /// <summary>
        /// Estado donde se otorgó el crédito
        /// </summary>
        public string? EstadoInegi { get; set; }
        /// <summary>
        /// Número de agencia
        /// </summary>
        public int Sucursal { get; set; }
        /// <summary>
        /// Nombre de la agencia
        /// </summary>
        public string? CatAgencia { get; set; }
        /// <summary>
        /// Número de cliente
        /// </summary>
        public string? NumCte { get; set; }
        /// <summary>
        /// Nombre del cliente / Acreditado
        /// </summary>
        public string? Acreditado { get; set; }
        /// <summary>
        /// Número de contrato
        /// </summary>
        public string? NumContrato { get; set; }
        /// <summary>
        /// Número de crédito
        /// </summary>
        public string? NumCredito { get; set; }
        /// <summary>
        /// Número de producto
        /// </summary>
        public string? NumProducto { get; set; }
        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string? CatProducto { get; set; }
        /// <summary>
        /// Tipo de Operación
        /// </summary>
        public string? TipoCreditoCont { get; set; }
        /// <summary>
        /// Estado de la cartera
        /// </summary>
        public string? EstadoDeLaCartera { get; set; }
        /// <summary>
        /// Monto del crédito
        /// </summary>
        public decimal SldoTotContval { get; set; }
        /// <summary>
        /// Si aparece en pantalla o solo se preparó para el reporte de Guarda Valores
        /// </summary>
        public bool EsCreditoAReportar { get; set; }
        public string? Imagen { get; set; }
        public bool TieneTurnoCobranza { get; set; }
        public bool TieneTurnoJuridico { get; set; }

    }
}
