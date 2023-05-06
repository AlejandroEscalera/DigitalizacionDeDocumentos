using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas
{
    public class ExpedienteDeConsultaCarga
    {
        
        /// <summary>
        /// Número de Crédito
        /// </summary>
        public string? NumCredito { get; set; }
        
        /// <summary>
        /// Número de Crédito en Cancelación
        /// </summary>
        public string? NumCreditoCancelado { get; set; }
        /// <summary>
        /// Si está en el archivo de saldos activos
        /// </summary>
        public bool EsSaldosActivo { get; set; }
        /// <summary>
        /// Si está en la lista de créditos cancelados
        /// </summary>
        public bool EsCancelado { get; set; }
        /// <summary>
        /// Si se originó el crédito en el período del Dr
        /// </summary>
        public bool EsOrigenDelDr { get; set; }
        /// <summary>
        /// Si el crédito se canceló en el período del Dr
        /// </summary>
        public bool EsCanceladoDelDr { get; set; }
        /// <summary>
        /// Si el crédito está castigado
        /// </summary>
        public bool EsCastigado { get; set; }
        /// <summary>
        /// Si tiene un arqueo de auditoría
        /// </summary>
        public bool TieneArqueo { get; set; }
        /// <summary>
        /// Nombre del acreditado
        /// </summary>
        public string? Acreditado { get; set; }
        /// <summary>
        /// Fecha de Apertura del crédito
        /// </summary>
        public string? FechaApertura { get; set; }
        /// <summary>
        /// Fecha de la Cancelacion del crédito
        /// </summary>
        public string? FechaCancelacion { get; set; }
        /// <summary>
        /// La evaluación de castigo de la cartera
        /// </summary>
        public string? Castigo { get; set; }
        /// <summary>
        /// El número de producto de crédito
        /// </summary>
        public string? NumProducto { get; set; }
        /// <summary>
        /// Descripción del producto de crédito
        /// </summary>
        public string? CatProducto { get; set; }
        /// <summary>
        /// Clasificación del crédito
        /// </summary>
        public string? TipoDeCredito { get; set; }
        /// <summary>
        /// El nombre del ejecutivo que aperturó el crédito
        /// </summary>
        public string? Ejecutivo { get; set; }
        /// <summary>
        /// El nombre del analista de la mesa de control
        /// </summary>
        public string? Analista { get; set; }
        /// <summary>
        /// Fecha en que se inició la primera ministración del crédito
        /// </summary>
        public string? FechaInicioMinistracion { get; set; }
        /// <summary>
        /// Fecha en que ingresó a la mesa de control
        /// </summary>
        public string? FechaSolicitud { get; set; }

        public Decimal MontoCredito { get; set; }
        public Decimal InterCont { get; set; }
        public int Region { get; set; }
        public int Agencia { get; set; }
        public string? CatRegion { get; set; }
        public string? CatAgencia { get; set; }
        public bool EsCreditoAReportar { get; set; }
        public bool StatusImpago { get; set; }
        public bool StatusCarteraVencida { get; set; }
        public bool StatusCarteraVigente { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
        public decimal SldoTotContval { get; set; }
        public string? NumCliente { get; set; }
    }
}
