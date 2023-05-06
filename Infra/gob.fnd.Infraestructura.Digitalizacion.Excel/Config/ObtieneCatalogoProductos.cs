using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Config
{
    public class ObtieneCatalogoProductos : IObtieneCatalogoProductos
    {
        const string C_STR_HOJA_CATALOGO_PRODUCTOS = "CAT_PRODUCTO";
        const string C_STR_HOJA_CATALOGO_PROD2 = "CAT_PROD2";
        const int C_INT_CAT_PRODUCTO = 1;
        const int C_INT_GRUPO_PRODUCTO = 2;
        const int C_INT_VENCIMIENTO = 3;
        const int C_INT_NUM_PRODUCTO = 4;
        const string C_STR_HOJA_OTROS = "OTROS";
        const int C_INT_COL_FECHA_FILTRO = 1;

        const int C_INT_NUM_PRODUCTO2 = 1;
        const int C_INT_CAT_PRODUCTO2 = 2;

        private readonly ILogger<ObtieneCatalogoProductos> _logger;
        private readonly IConfiguration _config;
        private readonly string? _archivoConfiguracion;

        public ObtieneCatalogoProductos(ILogger<ObtieneCatalogoProductos> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _archivoConfiguracion = _config.GetValue<string>("archivoFiltro") ?? "";
        }
        
        public DateTime? ObtieneFechaFiltro()
        {
            if (!ExisteArchivoConfiguracion())
                return null;
            #region Abrimos el excel de Filtros, y obtengo la fecha de los créditos a filtrar
            DateTime? resultado = null;
            FileStream fs = new(_archivoConfiguracion??"", FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return null;
                var hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == C_STR_HOJA_OTROS);
                if (hoja == null)
                {
                    return null;
                }
                int row = 2;
                resultado = FNDExcelHelper.GetCellDateTime(hoja.Cells[row, C_INT_COL_FECHA_FILTRO]);
            }
            #endregion
            return resultado;
        }

        private bool ExisteArchivoConfiguracion() 
        {
            if (!File.Exists(_archivoConfiguracion))
            {
                _logger.LogError("No se puede procesar el archivo de configuracion:\n{archivoConfiguracion}", _archivoConfiguracion);
                return false;
            }
            return true;
        }
        public IEnumerable<FiltraCatalogoProductos> ObtieneListaAFiltrar()
        {
            IList<FiltraCatalogoProductos> lista = new List<FiltraCatalogoProductos>();
            if (!ExisteArchivoConfiguracion())
                return lista.ToArray();

            #region Abrimos el excel de Filtors, y obtengo el Catalogo de Productos
            FileStream fs = new(_archivoConfiguracion??"", FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista.ToArray();
                var hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == C_STR_HOJA_CATALOGO_PRODUCTOS);
                if (hoja == null)
                {
                    return lista.ToArray();
                }
                int row = 2;
                bool resultado = false;
                while (!resultado)
                {
                    var obj = new FiltraCatalogoProductos
                    {
                        CatalogoProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CAT_PRODUCTO]),
                        GrupoProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_GRUPO_PRODUCTO]),
                        DiasDeVencimiento = FNDExcelHelper.GetCellInt(hoja.Cells[row, C_INT_VENCIMIENTO]),
                        NumProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NUM_PRODUCTO])
                    };
                    if (string.IsNullOrEmpty(obj.CatalogoProducto))
                        resultado = true;
                    if (!resultado)
                        lista.Add(obj);
                    row++;
                }
            }
            #endregion


            return lista.ToArray();
        }

        public IEnumerable<CatalogoProductos> ObtieneElCatalogoDelProducto()
        {
            IList<CatalogoProductos> lista = new List<CatalogoProductos>();
            if (!ExisteArchivoConfiguracion())
                return lista.ToArray();

            #region Abrimos el excel de Filtors, y obtengo el Catalogo de Productos
            FileStream fs = new(_archivoConfiguracion ?? "", FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista.ToArray();
                var hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == C_STR_HOJA_CATALOGO_PROD2);
                if (hoja == null)
                {
                    return lista.ToArray();
                }
                int row = 2;
                bool resultado = false;
                while (!resultado)
                {
                    var obj = new CatalogoProductos
                    {
                        NumProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NUM_PRODUCTO2]),
                        CatProducto = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CAT_PRODUCTO2])
                    };
                    if (string.IsNullOrEmpty(obj.CatProducto))
                        resultado = true;
                    if (!resultado)
                        lista.Add(obj);
                    row++;
                }
            }
            #endregion


            return lista.ToArray();
        }
    }
}
