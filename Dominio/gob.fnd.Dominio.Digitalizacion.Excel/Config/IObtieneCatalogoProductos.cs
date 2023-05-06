using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Config;

/// <summary>
/// Obtiene información de configuración
/// </summary>
public interface IObtieneCatalogoProductos
{
    /// <summary>
    /// Obtiene la lista de productos que serán incluidos para la digitalización de expedientes
    /// </summary>
    /// <returns>Un arreglo con los productos a filtrar</returns>
    IEnumerable<FiltraCatalogoProductos> ObtieneListaAFiltrar();
    /// <summary>
    /// Obtiene del archivo de configuración la fecha que se usará para filtrar para el tema de la entrega / recepción, es para el filtro de productos y créditos
    /// </summary>
    /// <returns>Fecha a tomar en cuenta en la entrega recepción</returns>
    DateTime? ObtieneFechaFiltro();

    /// <summary>
    /// Obtiene una lista de todos los productos de crédito
    /// </summary>
    /// <returns>Lista de productos de crédito</returns>
    IEnumerable<CatalogoProductos> ObtieneElCatalogoDelProducto();
}
