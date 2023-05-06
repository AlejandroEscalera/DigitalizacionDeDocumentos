using AppConsultaImagen.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace AppConsultaImagen.Pinta
{
    public class PintaReporte
    {
        #region Distribucion por Monto de Saldos
        public static void PintaDistribucionDeSaldos(ReporteDiario datosReporte, Chart chartReporteDistribucionSaldos)
        {
            // Configuramos el Chart
            #region Limpio la información
            chartReporteDistribucionSaldos.Series.Clear();
            chartReporteDistribucionSaldos.Legends.Clear();
            chartReporteDistribucionSaldos.ChartAreas.Clear();
            #endregion

            #region Configuro
            chartReporteDistribucionSaldos.Titles.Add("");
            ChartArea chartArea1 = new();
            chartReporteDistribucionSaldos.ChartAreas.Add(chartArea1);
            chartReporteDistribucionSaldos.Series.Add("Ventas por Producto");
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].ChartType = SeriesChartType.Pie;
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].IsValueShownAsLabel = true;
            #endregion

            // Agregamos los datos
            for (int i = 0; i < 5; i++)
            {
                if (datosReporte.DistribucionPorMonto is not null && datosReporte.DistribucionPorMonto[i] is not null)
                {
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, datosReporte.DistribucionPorMonto[i].Color,
                        datosReporte.DistribucionPorMonto[i].NombreDeLaRegion ?? "",
                        datosReporte.DistribucionPorMonto[i].PorcentajeDeLaRegion,
                        datosReporte.DistribucionPorMonto[i].ObtienePorcentajeDeLaRegion(),
                        i);
                }
                #region Valores de default por cualquier cosa
                else
                {
                    int orden = 0;
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, Color.FromArgb(212, 193, 156), "C-O", 28.5M, "C-O\n28.5%", orden++);
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, Color.FromArgb(98, 17, 50), "NO", 22.6M, "NO\n22.6%", orden++);
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, Color.FromArgb(40, 92, 77), "N", 23.2M, "N\n23.2%", orden++);
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, Color.FromArgb(19, 50, 43), "S", 14.4M, "S\n14.4%", orden++);
                    AgregaEjePintDistribucionDeSaldos(chartReporteDistribucionSaldos, Color.FromArgb(179, 142, 93), "SE", 11.3M, "SE\n11.3%", orden++);
                }
                #endregion
            }
            chartReporteDistribucionSaldos.ChartAreas[0].Position = new ElementPosition(2, 2, 98, 98);
        }

        private static void AgregaEjePintDistribucionDeSaldos(Chart chartReporteDistribucionSaldos, Color colorEje, string region, decimal valor, string valorEnFormato, int orden)
        {
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points.AddXY(region, valor);
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].Label = valorEnFormato;
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].Color = colorEje;
            #region Formato fijo
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].LabelForeColor = Color.White;
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].Font = new Font("Montserrat", 15, FontStyle.Bold);
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].BorderWidth = 5;
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].BorderDashStyle = ChartDashStyle.Solid;
            chartReporteDistribucionSaldos.Series["Ventas por Producto"].Points[orden].BorderColor = Color.White;
            #endregion
        }
        #endregion

        #region Distibucion por Numeros de Credito
        public static void PintaDistribucionDeNumerosDeCredito(ReporteDiario datosReporte, Chart chrtPorNumCreditos)
        {
            #region Limpio el Chart
            chrtPorNumCreditos.Series.Clear();
            chrtPorNumCreditos.Legends.Clear();
            chrtPorNumCreditos.ChartAreas.Clear();
            #endregion
            #region Comienzo la configuración
            ChartArea chartArea1 = new ();
            chrtPorNumCreditos.ChartAreas.Add(chartArea1);
            chrtPorNumCreditos.ChartAreas[0].AxisY.Title = "Eje Y";
            chrtPorNumCreditos.ChartAreas[0].AxisY.TitleForeColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosBarra;
            chrtPorNumCreditos.ChartAreas[0].AxisY.LabelStyle.ForeColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosBarra;
            chrtPorNumCreditos.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
            chrtPorNumCreditos.ChartAreas[0].AxisY.Minimum = 0;
            #endregion

            #region Configuro las Barras
            chrtPorNumCreditos.Series.Add("Columnas");
            chrtPorNumCreditos.Series["Columnas"].ChartType = SeriesChartType.Column;
            chrtPorNumCreditos.Series["Columnas"].Color = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosBarra;
            chrtPorNumCreditos.Series["Columnas"].IsValueShownAsLabel = true;
            chrtPorNumCreditos.Series["Columnas"].IsXValueIndexed = true;
            chrtPorNumCreditos.Series["Columnas"].MarkerBorderWidth = 2;
            chrtPorNumCreditos.Series["Columnas"].LabelForeColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosBarra;
            chrtPorNumCreditos.Series["Columnas"].LabelFormat = "N0";
            #endregion

            #region Configuro las lineas
            chrtPorNumCreditos.Series.Add("Linea");
            chrtPorNumCreditos.Series["Linea"].ChartType = SeriesChartType.Line;
            chrtPorNumCreditos.Series["Linea"].Color = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosLinea;
            chrtPorNumCreditos.Series["Linea"].MarkerStyle = MarkerStyle.Diamond;
            chrtPorNumCreditos.Series["Linea"].MarkerSize = 10;
            chrtPorNumCreditos.Series["Linea"].MarkerColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosLinea;
            chrtPorNumCreditos.Series["Linea"].IsValueShownAsLabel = true;
            chrtPorNumCreditos.Series["Linea"].LabelForeColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosLinea;
            chrtPorNumCreditos.Series["Linea"].LabelFormat = "N0";
            chrtPorNumCreditos.Series["Linea"].SmartLabelStyle.MovingDirection = LabelAlignmentStyles.BottomLeft;
            #endregion

            // Agrego valores
            for (int i = 0; i < 5; i++)
            {
                AgregaBarraDistribucionDeNumerosDeCredito(chrtPorNumCreditos, datosReporte.BarrasPorNumeroDeCredito[i].NombreDeLaRegion ?? "", datosReporte.BarrasPorNumeroDeCredito[i].CantidadDeCreditos, i);
                AgregaLineaDistribucionDeNumerosDeClientes(chrtPorNumCreditos, datosReporte.BarrasPorNumeroDeCredito[i].NombreDeLaRegion ?? "", datosReporte.BarrasPorNumeroDeCredito[i].NumeroDeClientes, i);
            }

            #region Ocultar los ejes al final
            chrtPorNumCreditos.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrtPorNumCreditos.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Montserrat", 9, FontStyle.Bold);
            chrtPorNumCreditos.ChartAreas[0].AxisX.LabelStyle.ForeColor = datosReporte.ColorLineaDistribucionPorNumeroDeCreditosBarra;
            chrtPorNumCreditos.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chrtPorNumCreditos.ChartAreas[0].AxisX.LineWidth = 0;
            chrtPorNumCreditos.ChartAreas[0].AxisX.InterlacedColor = Color.Transparent;
            chrtPorNumCreditos.ChartAreas[0].AxisX.IsInterlaced = false;
            chrtPorNumCreditos.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            chrtPorNumCreditos.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            #endregion
        }

        private static void AgregaBarraDistribucionDeNumerosDeCredito(Chart chrtPorNumCreditos, string region, decimal valor, int orden)
        {
            chrtPorNumCreditos.Series["Columnas"].Points.AddXY(region, valor);
            chrtPorNumCreditos.Series["Columnas"].Points[orden].AxisLabel = region;
            chrtPorNumCreditos.Series["Columnas"].Points[orden].IsValueShownAsLabel = true;
            chrtPorNumCreditos.Series["Columnas"].Points[orden].Font = new Font("Montserrat", 9, FontStyle.Bold);
        }

        private static void AgregaLineaDistribucionDeNumerosDeClientes(Chart chrtPorNumCreditos, string region, decimal valor, int orden)
        {
            chrtPorNumCreditos.Series["Linea"].Points.AddXY(region, valor);
            chrtPorNumCreditos.Series["Linea"].Points[orden].Font = new Font("Montserrat", 9, FontStyle.Bold);
        }
        #endregion

        #region Saldos de todos los creditos
        public static void PintaInformacionSaldoTodosLosCreditos(ReporteDiario datosReporte, Label lblSaldosMDP, Label lblNoClientes, Label lblNoDeCreditos, Label lblSaldoVigente, Label lblSaldoVencida, Label lblImpago, Label lblICV)
        {
            lblSaldosMDP.Text = datosReporte.SaldosCreditos.ObtieneSaldoTotalCartera();
            lblNoClientes.Text = datosReporte.SaldosCreditos.ObtieneNoDeClientes();
            lblNoDeCreditos.Text = datosReporte.SaldosCreditos.ObtieneNoDeCreditos();
            lblSaldoVigente.Text = datosReporte.SaldosCreditos.ObtieneSaldoTotalCarteraVigente();
            lblSaldoVencida.Text = datosReporte.SaldosCreditos.ObtieneSaldoTotalCarteraVencida();
            lblImpago.Text = datosReporte.SaldosCreditos.ObtieneSaldoImpago();
            lblICV.Text = datosReporte.SaldosCreditos.ObtieneICV();
        }
        #endregion

        #region Comportamientos Diarios
        public static void PintaInformacionComportamientosDiarios(ReporteDiario datosReporte, Chart chrtSaldo, Chart chrtCV, Chart chrtICV)
        {
            #region Limpio el Chart
            chrtSaldo.Series.Clear();
            chrtSaldo.Legends.Clear();
            chrtSaldo.ChartAreas.Clear();

            chrtCV.Series.Clear();
            chrtCV.Legends.Clear();
            chrtCV.ChartAreas.Clear();

            chrtICV.Series.Clear();
            chrtICV.Legends.Clear();
            chrtICV.ChartAreas.Clear();
            #endregion

            #region Comienzo la Configuracion
            ChartArea chartArea1 = new();
            chrtSaldo.ChartAreas.Add(chartArea1);
            chrtSaldo.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            chrtSaldo.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);
            chrtSaldo.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chrtSaldo.ChartAreas[0].AxisX.LineWidth = 0;
            chrtSaldo.ChartAreas[0].AxisX.InterlacedColor = Color.Transparent;
            chrtSaldo.ChartAreas[0].AxisX.IsInterlaced = false;
            chrtSaldo.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;


            chrtSaldo.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            chrtSaldo.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);

            ChartArea chartArea2 = new();
            chrtCV.ChartAreas.Add(chartArea2);
            chrtCV.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            chrtCV.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);
            chrtCV.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chrtCV.ChartAreas[0].AxisX.LineWidth = 0;
            chrtCV.ChartAreas[0].AxisX.InterlacedColor = Color.Transparent;
            chrtCV.ChartAreas[0].AxisX.IsInterlaced = false;
            chrtCV.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            chrtCV.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            chrtCV.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);

            ChartArea chartArea23 = new ();
            chrtICV.ChartAreas.Add(chartArea23);
            chrtICV.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrtICV.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);
            chrtICV.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chrtICV.ChartAreas[0].AxisX.LineWidth = 0;
            chrtICV.ChartAreas[0].AxisX.InterlacedColor = Color.Transparent;
            chrtICV.ChartAreas[0].AxisX.IsInterlaced = false;
            chrtICV.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;


            chrtICV.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            chrtICV.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Montserrat", 8, FontStyle.Bold);
            #endregion

            #region Configuro las Series
            Series seriesSaldo = new("Saldo")
            {
                ChartType = SeriesChartType.Line,
                Color = datosReporte.ComportamientosDiarios.ColorSaldo,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10,
                IsValueShownAsLabel = true,
                LabelFormat = "N0"
            };
            chrtSaldo.Series.Add(seriesSaldo);

            Series seriesCV = new("CV")
            {
                ChartType = SeriesChartType.Line,
                Color = datosReporte.ComportamientosDiarios.ColorCarteraVencida,
                MarkerStyle = MarkerStyle.Diamond,
                MarkerSize = 10,
                IsValueShownAsLabel = true,
                LabelFormat = "N0"
            };
            chrtCV.Series.Add(seriesCV);

            Series seriesICV = new ("ICV")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(179, 142, 93),
                MarkerStyle = MarkerStyle.Diamond,
                MarkerSize = 10,
                IsValueShownAsLabel = true
            };
            chrtICV.Series.Add(seriesICV);
            #endregion

            // Agrego valores
            for (int i = 0; i < 6; i++)
            {
                seriesSaldo.Points.AddXY(datosReporte.ComportamientosDiarios.DiaHabil[i], datosReporte.ComportamientosDiarios.Saldo[i]);
                seriesCV.Points.AddXY(datosReporte.ComportamientosDiarios.DiaHabil[i], datosReporte.ComportamientosDiarios.CarteraVencida[i]);
                seriesICV.Points.AddXY(datosReporte.ComportamientosDiarios.DiaHabil[i], datosReporte.ComportamientosDiarios.IndiceCarteraVencida[i]);
                seriesICV.Points[i].Label = datosReporte.ComportamientosDiarios.GetIndiceCarteraVencida(i); // Obicv[i].ToString("0.0") + "%";
            }
        }
        #endregion

        #region Pagos Recibidos
        public static void PintaInformacionPagosRecibidos(ReporteDiario datosReporte, Label lblPagosRecibidos, Label lblPrPagoPuntual, Label lblPrPagoAnticipado, Label lblPrPagoDeIncumplimiento)
        {
            lblPagosRecibidos.Text = datosReporte.PagosRecibidos.ObtieneMontoPagosRecibidos();
            lblPrPagoPuntual.Text = datosReporte.PagosRecibidos.ObtieneMontoPagoPuntual();
            lblPrPagoAnticipado.Text = datosReporte.PagosRecibidos.ObtieneMontoPagoAnticipado();
            lblPrPagoDeIncumplimiento.Text = datosReporte.PagosRecibidos.ObtieneMontoPagoIncumplimiento();
        }
        #endregion

        #region Comportamientos Diarios Pagos Recibidos
        public static void PintaInformacionComportamientosDiarios(ReporteDiario datosReporte, Chart chrtComportamientoDiario)
        {
            #region Limpio el Chart
            chrtComportamientoDiario.Series.Clear();
            chrtComportamientoDiario.Legends.Clear();
            chrtComportamientoDiario.ChartAreas.Clear();
            #endregion

            #region Comienzo la Configuracion
            ChartArea chartArea1 = new();
            chrtComportamientoDiario.ChartAreas.Add(chartArea1);

            chrtComportamientoDiario.ChartAreas[0].AxisY.Title = "Eje Y";
            chrtComportamientoDiario.ChartAreas[0].AxisY.TitleForeColor = datosReporte.ColorComportamientosPagosRecibidos;
            chrtComportamientoDiario.ChartAreas[0].AxisY.LabelStyle.ForeColor = datosReporte.ColorComportamientosPagosRecibidos;
            chrtComportamientoDiario.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
            chrtComportamientoDiario.ChartAreas[0].AxisY.Minimum = 0;

            chrtComportamientoDiario.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrtComportamientoDiario.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Montserrat", 9, FontStyle.Regular);
            chrtComportamientoDiario.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
            chrtComportamientoDiario.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chrtComportamientoDiario.ChartAreas[0].AxisX.LineWidth = 0;
            chrtComportamientoDiario.ChartAreas[0].AxisX.InterlacedColor = Color.Transparent;
            chrtComportamientoDiario.ChartAreas[0].AxisX.IsInterlaced = false;
            chrtComportamientoDiario.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            chrtComportamientoDiario.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

            #endregion

            #region Configuro las Series
            chrtComportamientoDiario.Series.Add("Columnas");
            chrtComportamientoDiario.Series["Columnas"].ChartType = SeriesChartType.Column;
            chrtComportamientoDiario.Series["Columnas"].Color = datosReporte.ColorComportamientosPagosRecibidos;
            chrtComportamientoDiario.Series["Columnas"].IsValueShownAsLabel = true;
            chrtComportamientoDiario.Series["Columnas"].IsXValueIndexed = true;
            chrtComportamientoDiario.Series["Columnas"].MarkerBorderWidth = 2;
            chrtComportamientoDiario.Series["Columnas"].LabelForeColor = datosReporte.ColorComportamientosPagosRecibidos;
            chrtComportamientoDiario.Series["Columnas"].LabelFormat = "N0";

            #endregion

            // Agrego valores
            for (int i = 0; i < 6; i++)
            {
                chrtComportamientoDiario.Series["Columnas"].Points.AddXY(datosReporte.ComportamientosPagosRecibidos.DiaRecibido[i], datosReporte.ComportamientosPagosRecibidos.PagosRecibidos[i]);
                chrtComportamientoDiario.Series["Columnas"].Points[i].AxisLabel = datosReporte.ComportamientosPagosRecibidos.DiaRecibido[i];
                chrtComportamientoDiario.Series["Columnas"].Points[i].IsValueShownAsLabel = true;
                chrtComportamientoDiario.Series["Columnas"].Points[i].Font = new Font("Montserrat", 9, FontStyle.Bold);
            }
        }
        #endregion

        public static void PrintaRegionMapa(RegionMapa regionMapa, Label lblPagosRecibidos, Label lblSaldos, Label lblSaldosVencidos, Label lblSaldosVigente, Label lblICV)
        {
            if (lblPagosRecibidos is not null)
                lblPagosRecibidos.Text = regionMapa.ObtienePagosRecibidos();
            if (lblSaldos is not null)
                lblSaldos.Text = regionMapa.ObtieneSaldo();
            if (lblSaldosVencidos is not null)
                lblSaldosVencidos.Text = regionMapa.ObtieneSaldoVencido();
            if (lblSaldosVigente is not null)
                lblSaldosVigente.Text = regionMapa.ObtieneSaldoVigente();
            if (lblICV is not null)
                lblICV.Text = regionMapa.ObtieneIndiceCarteraVigente();
        }
        public static void PintaRegiones(ReporteDiario datosReporte, RegionMapaLabel[] etiquetas)
        {
            if (etiquetas.Length != 5)
            {
                return;
            }
            for (int i = 0; i < 5; i++)
            {
                PrintaRegionMapa(datosReporte.RegionesMapa[i], etiquetas[i].LblPagosRecibidos, etiquetas[i].LblSaldos, etiquetas[i].LblSaldosVencidos, etiquetas[i].LblSaldosVigente, etiquetas[i].LblICV);
            }
        }

    }

    public class RegionMapaLabel
    {
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public Label LblPagosRecibidos { get; set; }
        public Label LblSaldos { get; set; }
        public Label LblSaldosVencidos { get; set; }
        public Label LblSaldosVigente { get; set; }
        public Label LblICV { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.

    }
}
