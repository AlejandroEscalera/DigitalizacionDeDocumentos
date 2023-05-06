using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Excel.Arqueos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using gob.fnd.ExcelHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.Arqueos
{
    public class AnalisisCamposGuardaValores : IAnalisisCamposGuardaValores
    {
        private readonly ILogger<AnalisisCamposGuardaValores> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _archivoAnalisisCamposGuardaValores;

        public AnalisisCamposGuardaValores(ILogger<AnalisisCamposGuardaValores> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _archivoAnalisisCamposGuardaValores = _configuration.GetValue<string>("archivoAnalisisCamposGuardaValor") ?? "";
        }
        public IEnumerable<ArchivoAnalisisCamposArqueos> CargaInformacion()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            FileStream fs = new(_archivoAnalisisCamposGuardaValores, FileMode.Open, FileAccess.Read);
            IList<ArchivoAnalisisCamposArqueos> lista = new List<ArchivoAnalisisCamposArqueos>();
            using (var package = new ExcelPackage())
            {
                package.Load(fs);
                if (package.Workbook.Worksheets.Count == 0)
                    return lista;
                var hojaResumenArchivos = package.Workbook.Worksheets["ResumenArchivos"];
                var hojaCamposGV = package.Workbook.Worksheets["Campos GV"];
                if (hojaResumenArchivos is null || hojaCamposGV is null) {
                    _logger.LogError("Archivo invalido {archivo}", _archivoAnalisisCamposGuardaValores);
                    return lista;
                }
                int renglon = 1;
                int columna = 2;
                var valor = hojaResumenArchivos.Cells[renglon, columna].Value;
                string rutaCompleta = Convert.ToString(valor)??"";
                renglon = 3;
                columna = 1;
                valor = hojaResumenArchivos.Cells[renglon, columna].Value;
                while (valor is not null) {
                    ArchivoAnalisisCamposArqueos obj = new()
                    {
                        Id = hojaResumenArchivos.Cells[renglon, columna].GetCellInt(),
                        RutaCompleta = rutaCompleta
                    };
                    columna++;
                    obj.Carpeta = hojaResumenArchivos.Cells[renglon, columna].GetCellString();
                    columna++;
                    obj.NombreArchivo = hojaResumenArchivos.Cells[renglon, columna].GetCellString();
                    columna++;
                    obj.RutaCompleta = Path.Combine(rutaCompleta, obj.Carpeta, obj.NombreArchivo);

                    obj.TieneHojaPT = hojaResumenArchivos.Cells[renglon, columna].GetCellString() == "Si";
                    columna++;
                    obj.TieneHojaBaseABSaldos = hojaResumenArchivos.Cells[renglon, columna].GetCellString() == "Si";
                    columna++;
                    obj.TieneHojaRgOpeGva005_010 = hojaResumenArchivos.Cells[renglon, columna].GetCellString() == "Si";
                    columna= 11; 
                    obj.PrimerColumnaDetalleGuardaValores = hojaResumenArchivos.Cells[renglon, columna].GetCellInt();
                    columna++;
                    obj.CamposGuardaValores = hojaResumenArchivos.Cells[renglon, columna].GetCellString();
                    columna++;
                    obj.PrimerRenglonGuardaValores = hojaResumenArchivos.Cells[renglon, columna].GetCellInt();
                    columna++;
                    obj.NumeroGuardaValores = hojaResumenArchivos.Cells[renglon, columna].GetCellInt();
                    columna++;
                    obj.NoCreditoEjemplo = hojaResumenArchivos.Cells[renglon, columna].GetCellString();
                    columna++;
                    obj.NoAgencia = hojaResumenArchivos.Cells[renglon, columna].GetCellInt();
                    columna++;
                    obj.Coordinacion = hojaResumenArchivos.Cells[renglon, columna].GetCellString();
                    columna = 8;
                    obj.NumeroCamposGuardaValor = hojaCamposGV.Cells[renglon, columna].GetCellInt();
                    obj.CamposDeInformacionGuardaValor = new string[16];
                    for (columna = 10; columna < 26; columna++)
                    {
                        obj.CamposDeInformacionGuardaValor[columna-10] = hojaCamposGV.Cells[renglon, columna].GetCellString();
                    }
                    renglon++;
                    columna = 1;
                    lista.Add(obj);
                    valor = hojaResumenArchivos.Cells[renglon, columna].Value;
                }
            }

            return lista;
        }
    }
}
