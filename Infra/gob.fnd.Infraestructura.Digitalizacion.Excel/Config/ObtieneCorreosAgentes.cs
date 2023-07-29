using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.ExcelHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Config
{
    public class ObtieneCorreosAgentes : IObtieneCorreosAgentes
    {
        const int C_INT_NO_AGENCIA = 1;            // A
        const int C_INT_AGENCIA = 2;               // B
        const int C_INT_NOMBRE_AGENTE = 3;         // C
        const int C_INT_CORREO_AGENTE = 4;         // D
        const int C_INT_NOMBRE_GUARDA_VALORES = 5; // E
        const int C_INT_CORREO_GUARDA_VALORES = 6; // F
        const int C_INT_LIGA_AGENCIA = 7; // G
        const int C_INT_REGION = 8; // H
        private readonly ILogger<ObtieneCorreosAgentes> _logger;
        private readonly IConfiguration _config;
        private readonly string _archivoConfiguracion;

        public ObtieneCorreosAgentes(ILogger<ObtieneCorreosAgentes> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _archivoConfiguracion = _config.GetValue<string>("archivoFiltro") ?? "";
        }
        public IEnumerable<CorreosAgencia> ObtieneTodosLosCorreosYAgentes()
        {
            IList<CorreosAgencia> lista = new List<CorreosAgencia>();
            if (!File.Exists(_archivoConfiguracion))
            {
                _logger.LogError("No se puede procesar el archivo de configuracion:\n{archivoConfiguracion}", _archivoConfiguracion);
                return lista.ToArray();
            }

            #region Abrimos el excel de Filtors, y obtengo el Catalogo de Productos
            FileStream fs = new(_archivoConfiguracion, FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista.ToArray();
                var hoja = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Agencias");
                if (hoja == null)
                {
                    return lista.ToArray();
                }
                int row = 2;
                bool resultado = false;
                while (!resultado)
                {
                    CorreosAgencia obj = new()
                    {
                        NoAgencia = Convert.ToInt32(FNDExcelHelper.GetCellDouble(hoja.Cells[row, C_INT_NO_AGENCIA])),
                        Agencia = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_AGENCIA]),
                        NombreAgente = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NOMBRE_AGENTE]),
                        CorreoAgente = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CORREO_AGENTE]),
                        NombreGuardaValores = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NOMBRE_GUARDA_VALORES]),
                        CorreoGuardaValores = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CORREO_GUARDA_VALORES]),
                        LigaAgencia = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_LIGA_AGENCIA]),
                        Region = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_REGION])
                    };

                    if (string.IsNullOrEmpty(obj.Agencia) || string.IsNullOrWhiteSpace(obj.Agencia))
                        resultado = true;
                    if (!resultado)
                        lista.Add(obj);
                    row++;
                }
            }
            #endregion

            return lista.ToArray();

        }

        public async Task<IEnumerable<CorreosAgencia>> ObtieneTodosLosCorreosYAgentesAsync()
        {
            IList<CorreosAgencia> lista = new List<CorreosAgencia>();
            if (!File.Exists(_archivoConfiguracion))
            {
                _logger.LogError("No se puede procesar el archivo de configuracion:\n{archivoConfiguracion}", _archivoConfiguracion);
                return lista.ToArray();
            }

            #region Abrimos el excel de Filtors, y obtengo el Catalogo de Productos
            FileStream fs = new(_archivoConfiguracion, FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage())
            {
                await package.LoadAsync(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista.ToArray();
                var hoja = await Task.Run(() =>{ return package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Agencias"); });
                if (hoja == null)
                {
                    return lista.ToArray();
                }
                int row = 2;
                bool resultado = false;
                await Task.Run(() => {
                    while (!resultado)
                    {
                        CorreosAgencia obj = new()
                        {
                            NoAgencia = Convert.ToInt32(FNDExcelHelper.GetCellDouble(hoja.Cells[row, C_INT_NO_AGENCIA])),
                            Agencia = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_AGENCIA]),
                            NombreAgente = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NOMBRE_AGENTE]),
                            CorreoAgente = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CORREO_AGENTE]),
                            NombreGuardaValores = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_NOMBRE_GUARDA_VALORES]),
                            CorreoGuardaValores = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_CORREO_GUARDA_VALORES]),
                            LigaAgencia = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_LIGA_AGENCIA]),
                            Region = FNDExcelHelper.GetCellString(hoja.Cells[row, C_INT_REGION])
                        };

                        if (string.IsNullOrEmpty(obj.Agencia) || string.IsNullOrWhiteSpace(obj.Agencia))
                            resultado = true;
                        if (!resultado)
                            lista.Add(obj);
                        row++;
                    }
                });                
            }
            #endregion

            return lista.ToArray();
        }
    }
}
