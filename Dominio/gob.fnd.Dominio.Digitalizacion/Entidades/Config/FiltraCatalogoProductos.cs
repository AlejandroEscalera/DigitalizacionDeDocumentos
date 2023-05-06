using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Config
{
    public class FiltraCatalogoProductos
    {
        /// <summary>
        /// Información del catálogo de producto a filtrar
        /// </summary>
        public string? CatalogoProducto { get; set; }  // CAT_PRODUCTO
        /// <summary>
        /// Grupo del producto
        /// </summary>
        public string? GrupoProducto { get; set; } // GRUPO_PRODUCTO
        /// <summary>
        /// Días de vencimiento
        /// </summary>
        public int? DiasDeVencimiento { get; set; } // VENCIMIENTO
        /// <summary>
        /// Los productos
        /// </summary>
        public string? NumProducto { get; set; }
    }
}
