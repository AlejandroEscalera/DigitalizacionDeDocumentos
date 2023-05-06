using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos
{
    public class ABSaldosRegionales
    {
        /// <summary>
        /// Catálogo de la región
        /// </summary>
        public string? CatalogoRegion { get; set; }
        /// <summary>
        /// Sucursal
        /// </summary>
        public double Sucursal { get; set; }
        /// <summary>
        /// Catálogo de Sucursales
        /// </summary>
        public string? CatalogoSucursales { get; set; }
        /// <summary>
        /// Número de cliente
        /// </summary>
        public string? NumeroCliente { get; set; }
        /// <summary>
        /// Apellido Paterno
        /// </summary>
        public string? ApellidoPaterno { get; set; }
        /// <summary>
        /// Apellido Materno
        /// </summary>
        public string? ApellidoMaterno { get; set; }
        /// <summary>
        /// Primer Nombre
        /// </summary>
        public string? PrimerNombre { get; set; }
        /// <summary>
        /// Segundo Nombre
        /// </summary>
        public string? SegundoNombre { get; set; }
        /// <summary>
        ///  Razon Social
        /// </summary>
        public string? RazonSocial { get; set; }
        /// <summary>
        /// Numero de Credito
        /// </summary>
        public string? NumCredito { get; set; }
        /// <summary>
        /// Fecha de Ministración
        /// </summary>
        public DateTime? FechaMinistra { get; set; }
        /// <summary>
        /// Nombre del Ejecutivo
        /// </summary>
        public string? NombreEjecutivo { get; set; }
        /// <summary>
        /// Número del producto del crédito activo
        /// </summary>
        public string? NumProducto { get; set; }
        /// <summary>
        /// Para poder hacer el filtro de los catálogos de productos
        /// </summary>
        public string? CatProducto { get; set; }
        /// <summary>
        /// Clave del Tipo de Crédito
        /// </summary>
        public string? CatTipoDeCredito { get; set; }
        /// <summary>
        /// Tipo de cartera
        /// </summary>
        public string? TipoCartera { get; set; }
        /// <summary>
        /// Monto del credito directamente de ABSaldos
        /// </summary>
        public decimal MontoMinistracion { get; set; }
        /// <summary>
        /// Nombre del agente encargado del crédito
        /// </summary>
        public string? Agente { get; set; }
        /// <summary>
        /// Nombre del responsable del guardavalores del crédito
        /// </summary>
        public string? GuardaValores { get; set; }
    }
}
