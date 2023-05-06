using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.ExcelHelper
{
    public class EPPlusHelper
    {
        /// <summary>
        /// Crea una hoja en el archivo de excel, en este caso, la primera hoja
        /// </summary>
        /// <param name="excelFile">El objeto package del archivo donde voy a guardar la información</param>
        /// <param name="sheetName">El nombre de la hoja</param>
        /// <returns>La hoja a trabajar</returns>
        public static ExcelWorksheet CreateSheet(ExcelPackage excelFile, string sheetName)
        {
            excelFile.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet workSheet = excelFile.Workbook.Worksheets[sheetName];
            workSheet.Name = sheetName; //Setting Sheet's name
            // workSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            // workSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            return workSheet;
        }

        /// <summary>
        /// Combina y centra un texto de Excel
        /// </summary>
        /// <param name="workSheet">La hoja de trabajo</param>
        /// <param name="textValue">El texto a escribir en las celdas</param>
        /// <param name="rowIni">Renglon inicial</param>
        /// <param name="celIni">Celda Inicial</param>
        /// <param name="rowEnd">Renglon final</param>
        /// <param name="celEnd">Celda Final</param>
        /// <param name="textAlign">Alineación, por default si se manda nulo, va centrado</param>
        /// <returns><value>True</value>si pude Combinar<value>False</value>Si no pudo</returns>
        public static bool MergeCell(ExcelWorksheet workSheet, string textValue, int rowIni, int celIni, int rowEnd, int celEnd, ExcelHorizontalAlignment textAlign = ExcelHorizontalAlignment.Center)
        {
            bool result = true;
            try
            {
                //Merging cells and create a center heading for out table
                workSheet.Cells[rowIni, celIni].Value = textValue; // Heading Name
                workSheet.Cells[rowIni, celIni, rowEnd, celEnd].Merge = true; //Merge columns start and end range
                // ws.Cells[rowIni, celIni, rowEnd, celEnd].Style.Font.Bold = true; //Font should be bold
                workSheet.Cells[rowIni, celIni, rowEnd, celEnd].Style.HorizontalAlignment = textAlign; // Aligmnet is center        
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Cambia el color del fondo
        /// </summary>
        /// <param name="cell">Celda</param>
        /// <param name="color">Color</param>
        /// <returns><value>Verdadero</value>si lo cambio. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool ChangeBackgroundColor(ExcelRange cell, Color color)
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

        /// <summary>
        /// Pone los bordes en una celda (o en un conjunto de celdas)
        /// </summary>
        /// <param name="cell">Celda a poner el borde</param>
        /// <returns><value>Verdadero</value>si lo puso. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool SetBorder(ExcelRange cell)
        {
            bool result = true;
            try
            {
                //Setting Top/left,right/bottom borders.
                var border = cell.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Realiza una suma de un rango de celdas.
        /// </summary>
        /// <param name="cell">Celda destino</param>
        /// <param name="workSheet">Hoja de trabajo</param>
        /// <param name="rowIni">Renglon inicial</param>
        /// <param name="celIni">Celda Inicial</param>
        /// <param name="rowEnd">Renglon final</param>
        /// <param name="celEnd">Celda final</param>
        /// <returns><value>Verdadero</value>si lo puso. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool SetSum(ExcelRange cell, ExcelWorksheet workSheet, int rowIni, int celIni, int rowEnd, int celEnd)
        {
            bool result = true;
            try
            {
                //Setting Top/left,right/bottom borders.
                cell.Formula = "Sum(" + workSheet.Cells[rowIni, celIni].Address + ":" + workSheet.Cells[rowEnd, celEnd].Address + ")";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Contar si, de un rango de celdas, se cumple cierto criterio
        /// </summary>
        /// <param name="cell">Celda destino</param>
        /// <param name="workSheet">Hoja de trabajo</param>
        /// <param name="rowIni">Renglon inicial</param>
        /// <param name="celIni">Celda Inicial</param>
        /// <param name="rowEnd">Renglon final</param>
        /// <param name="celEnd">Celda final</param>
        /// <param name="criterio"></param>
        /// <returns><value>Verdadero</value>si lo puso. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool SetCountIf(ExcelRange cell, ExcelWorksheet workSheet, int rowIni, int celIni, int rowEnd, int celEnd, int criterio)
        {
            bool result = true;
            try
            {
                //Setting Top/left,right/bottom borders.
                cell.Formula = "Count.If(" + workSheet.Cells[rowIni, celIni].Address + ":" + workSheet.Cells[rowEnd, celEnd].Address + "," + criterio + ")";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Añade un comentario
        /// </summary>
        /// <param name="workSheet">Hoja de trabajo</param>
        /// <param name="rowIndex">Renglon</param>
        /// <param name="colIndex">Columna</param>
        /// <param name="comment">Comentario</param>
        /// <param name="author">Autor</param>
        public static bool AddComment(ExcelWorksheet workSheet, int rowIndex, int colIndex, string comment, string author)
        {
            bool result = true;
            try
            {
                //Adding a comment to a Cell
                var commentCell = workSheet.Cells[rowIndex, colIndex];
                commentCell.AddComment(comment, author);
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

        /// <summary>
        /// Añade un logotipo a la hoja de trabajo
        /// </summary>
        /// <param name="workSheet">Hoja de trabajo</param>
        /// <param name="rowIndex">Renglon</param>
        /// <param name="columnIndex">Columna</param>
        /// <param name="filePath">Ruta del archivo del cual cargar el logotipo</param>
        public static void AddLogo(ExcelWorksheet workSheet, int columnIndex, int rowIndex, string filePath)
        {
            //How to Add a Image using EP Plus
            FileInfo image = new(filePath); // Bitmap
            ExcelPicture picture;
            if (image != null)
            {
                picture = workSheet.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = Pixel2MTU(2); //Two pixel space for better alignment
                picture.From.RowOff = Pixel2MTU(2);//Two pixel space for better alignment
                picture.SetSize(100, 100);
            }
        }

        /// <summary>
        /// Orientación de una celda
        /// </summary>
        /// <param name="cell">Celda</param>
        /// <param name="color">Orientacion</param>
        /// <returns><value>Verdadero</value>si lo cambio. <value>False</value> en caso de que haya marcado error.</returns>
        public static bool RotacionVertical(ExcelRange cell, int textRotation = -90)
        {
            bool result = true;
            try
            {
                cell.Style.TextRotation = textRotation;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        public static int ColumnWidth2Pixel(ExcelWorksheet ws, double excelColumnWidth)
        {
            //The correct method to convert width to pixel is:
            //Pixel =Truncate(((256 * {width} + Truncate(128/{Maximum DigitWidth}))/256)*{Maximum Digit Width})

            //get the maximum digit width
            decimal mdw = ws.Workbook.MaxFontWidth;

            //convert width to pixel
            decimal pixels = decimal.Truncate(((256 * (decimal)excelColumnWidth + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
            //double columnWidthInTwips = (double)(pixels * (1440f / 96f));

            return Convert.ToInt32(pixels);

        }

        public static double Pixel2ColumnWidth(ExcelWorksheet ws, int pixels)
        {
            //The correct method to convert pixel to width is:
            //1. use the formula =Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100 
            //    to convert pixel to character number.
            //2. use the formula width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256 
            //    to convert the character number to width.

            //get the maximum digit width
            decimal mdw = ws.Workbook.MaxFontWidth;

            //convert pixel to character number
            decimal numChars = decimal.Truncate(decimal.Add((decimal)(pixels - 5) / mdw * 100, (decimal)0.5)) / 100;
            //convert the character number to width
            decimal excelColumnWidth = decimal.Truncate((decimal.Add(numChars * mdw, (decimal)5)) / mdw * 256) / 256;

            return Convert.ToDouble(excelColumnWidth);
        }

        public static int RowHeight2Pixel(double excelRowHeight)
        {
            //convert height to pixel
            decimal pixels = decimal.Truncate((decimal)(excelRowHeight / 0.75));

            return Convert.ToInt32(pixels);
        }

        public static double Pixel2RowHeight(int pixels)
        {
            //convert height to pixel
            double excelRowHeight = pixels * (double)0.75;

            return excelRowHeight;
        }

        public static int MTU2Pixel(int mtus)
        {
            //convert MTU to pixel
            decimal pixels = decimal.Truncate((decimal)(mtus / MTU_PER_PIXEL));

            return Convert.ToInt32(pixels);
        }

        /// <summary>
        /// Genera las propiedades del Archivo de Excel (Workbook)
        /// </summary>
        /// <param name="excelPackage">El archivo que estamos trabajando</param>
        public static void SetWorkbookProperties(ExcelPackage excelPackage, string author, string title)
        {
            //Here setting some document properties
            excelPackage.Workbook.Properties.Author = author;
            excelPackage.Workbook.Properties.Title = title;
        }

    }

}
