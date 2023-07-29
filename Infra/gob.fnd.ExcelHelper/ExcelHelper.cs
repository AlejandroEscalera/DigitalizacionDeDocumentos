using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace gob.fnd.ExcelHelper
{
    public static class FNDExcelHelper
    {
        public static string GetCellString(this ExcelRange origen)
        {
            if (origen == null)
            {
                return string.Empty;
            }
            if (origen.Value == null)
            {
                return string.Empty;
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("STRING"))
            {
                return (string)origen.Value;
            }

            if (origen.Text.GetType().Name.ToUpper().Contains("STRING"))
            {
                return origen.Text;
            }

            return string.Empty;
        }

        public static DateTime? GetCellDateTime(this ExcelRange origen)
        {
            if (origen == null)
            {
                return null;
            }
            if (origen.Value == null)
            {
                return null;
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DOUBLE"))
            {
                double fechaExcel = Convert.ToDouble(origen.Value);
                try
                {
                    DateTime date = new DateTime(1900, 1, 1).AddDays(fechaExcel - 2);
                    return date;
                }
                catch { return null; }
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DATETIME"))
            {
                // double fechaExcel = Convert.ToDouble(origen.Value);

                DateTime date = (DateTime)origen.Value;
                // new DateTime(1900, 1, 1).AddDays(fechaExcel - 2);
                return date;
            }

            return null;
        }
        public static double GetCellDouble(ExcelRange origen)
        {
            if (origen == null)
            {
                return 0;
            }
            if (origen.Value == null)
            {
                return 0;
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DOUBLE"))
            {
                return Convert.ToDouble(origen.Value);
            }
            try
            {
                return Convert.ToDouble(origen.Text.Replace(",", ""));
            }
            catch { return 0; }
        }

        public static int GetCellInt(this ExcelRange origen)
        {
            if (origen == null)
            {
                return 0;
            }
            if (origen.Value == null)
            {
                return 0;
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DOUBLE"))
            {
                return Convert.ToInt32(origen.Value);
            }
            try
            {
                return Convert.ToInt32(origen.Text.Replace(",",""));
            }
            catch { return 0; }
        }
        public static Decimal GetCellDecimal(this ExcelRange origen)
        {
            if (origen == null)
            {
                return 0;
            }
            if (origen.Value == null)
            {
                return 0;
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DOUBLE"))
            {
                return Convert.ToDecimal(origen.Value);
            }
            if (origen.Value.GetType().Name.ToUpper().Contains("DECIMAL"))
            {
                return Convert.ToDecimal(origen.Value);
            }
            try
            {
                return Convert.ToDecimal(origen.Text.Replace(",", ""));
            }
            catch { return 0; }
        }
        public static void SetCellDate(this ExcelRange destino, DateTime value)
        {
            destino.Style.Numberformat.Format = "dd/mm/yyyy";
            destino.Value = GetExcelDecimalValueForDate(value);
        }
        private static decimal GetExcelDecimalValueForDate(DateTime date)
        {
            DateTime start = new(1900, 1, 1);
            TimeSpan diff = date - start;
            return diff.Days + 2;
        }

        /// <summary>
        /// Cambia el color del fondo
        /// </summary>
        /// <param name="cell">Celda</param>
        /// <param name="color">Color</param>
        /// <returns><value>Verdadero</value>si lo cambio. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool ChangeBackgroundColor(this ExcelRange cell, Color color)
        {
            bool result = true;
            try
            {
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(color);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        public const int MTU_PER_PIXEL = 9525;

        public static int Pixel2MTU(int pixels)
        {
            //convert pixel to MTU
            int mtus = pixels * MTU_PER_PIXEL;

            return mtus;
        }

        public static int ColumnWidth2Pixel(this ExcelWorksheet ws, double excelColumnWidth, int column = 0)
        {
            //The correct method to convert width to pixel is:
            //Pixel =Truncate(((256 * {width} + Truncate(128/{Maximum DigitWidth}))/256)*{Maximum Digit Width})

            //get the maximum digit width
            decimal mdw = ws.Workbook.MaxFontWidth;

            //convert width to pixel
            decimal pixels = decimal.Truncate(((256 * (decimal)excelColumnWidth + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
            //double columnWidthInTwips = (double)(pixels * (1440f / 96f));
            ws.Column(column).Width = Convert.ToInt32(pixels);
            return Convert.ToInt32(pixels);

        }

        /// <summary>
        /// Añade un logotipo a la hoja de trabajo
        /// </summary>
        /// <param name="workSheet">Hoja de trabajo</param>
        /// <param name="rowIndex">Renglon</param>
        /// <param name="columnIndex">Columna</param>
        /// <param name="filePath">Ruta del archivo del cual cargar el logotipo</param>
        public static void AddLogo(this ExcelWorksheet workSheet, int columnIndex, int rowIndex, string filePath)
        {
            if (File.Exists(filePath))
            {
                //Se carga la imagen a un BMP
                //Bitmap image = new Bitmap(filePath);
                /// Se agrega la imagen en una celda y renglon en particular
                ExcelPicture picture = workSheet.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), filePath);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = Pixel2MTU(4); //Two pixel space for better alignment
                //picture.From.ColumnOff = 4;
                picture.From.RowOff = Pixel2MTU(5);//Two pixel space for better alignment
                //picture.From.RowOff = 5;
                picture.SetSize(123);
            }
        }
        /// <summary>
        /// Se busca un texto dentro de una hoja
        /// </summary>
        /// <param name="hojaExcel">Hoja donde se buscará la información</param>
        /// <param name="valorABuscar">Valor que buscaremos</param>
        /// <param name="renglonInical">A partir de que renglon</param>
        /// <param name="columnaIncial">A partir de que columna</param>
        /// <param name="maxRenglonFinal">El máximo renglon donde pretendo buscar la expresión</param>
        /// <param name="maxColumnaFinal">La máxima columna donde pretendo buscar la expresión</param>
        /// <returns></returns>
        public static ExcelRange? SearchText(this ExcelWorksheet hojaExcel, string valorABuscar, int renglonInical = 1, int columnaIncial = 1, int maxRenglonFinal = -1, int maxColumnaFinal = -1, bool exacto = false) 
        {
            // En caso de que se especifique un máximo renglon a buscar
            maxRenglonFinal = maxRenglonFinal != -1 ? maxRenglonFinal : Math.Min(hojaExcel.Dimension.End.Row, maxRenglonFinal);
            // En caso de que se especifique una máxima columna a buscar
            maxColumnaFinal = maxColumnaFinal != -1 ? maxColumnaFinal : Math.Min(hojaExcel.Dimension.End.Column, maxColumnaFinal);

            valorABuscar = valorABuscar.Trim().Replace("\n", "");

            for (int row = renglonInical; row <= maxRenglonFinal; row++)
            {
                for (int col = columnaIncial; col <= maxColumnaFinal; col++)
                {
                    object valor = hojaExcel.Cells[row, col].Value;
                    if ((valor != null) && 
                        (valor.GetType() == typeof(string))) 
                    {
                        //Se obtiene el valor de la celda        
                        string valorCelda = Convert.ToString(valor)??"";
                        valorCelda = valorCelda.Trim().Replace("\n", "");
                        //Se compara con el valor "Buscado"
                        if (!string.IsNullOrWhiteSpace(valorCelda) && valorCelda.Contains(valorABuscar, StringComparison.InvariantCultureIgnoreCase))
                            if (!exacto || (exacto && (valorCelda.Length == valorABuscar.Length)))
                            // Regresamos la primera celda donde se encuentra
                                return hojaExcel.Cells[row,col];
                    }
                }
            }
            return null;
        }
    }

}
