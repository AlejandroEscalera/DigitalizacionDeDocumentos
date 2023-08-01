using AppConsultaImagen.Entidades;
using AppConsultaImagen.Pinta;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

/// <summary>
/// Aqui va la codificación del mapa del resumen regional
/// </summary>
public partial class MainFRM
{
    private string _nombreReporte = "";
    private string _fechaReporte = "";

    private void InicializaResumen()
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        lblResumenCreditosVigentes.DoubleClick += SeleccionaCarterasClick;
        lblResumenCreditosImpagos.DoubleClick += SeleccionaCarterasClick;
        lblResumenCreditosVencida.DoubleClick += SeleccionaCarterasClick;
        label36.DoubleClick += SeleccionaCarterasClick;
        label6.DoubleClick += SeleccionaCarterasClick;
        label11.DoubleClick += SeleccionaCarterasClick;
        label37.DoubleClick += SeleccionaCarterasClick;
        label35.DoubleClick += SeleccionaCarterasClick;
        label38.DoubleClick += SeleccionaCarterasClick;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }

    private void SeleccionaCarterasClick(object sender, EventArgs e)
    {
        PintaTodoElReporte();
    }

    private void BtnRcAnterior(object sender, EventArgs e)
    {
        tabReportesFinales.SelectedIndex = 0;
    }

    private void BtnRcSiguiente(object sender, EventArgs e)
    {
        SeleccionaRegion();
        // Falta seleccionar el renglon
    }

    private void SeleccionaRegion()
    {
        if (dgvResumenRegiones.SelectedRows is not null && dgvResumenRegiones.CurrentRow is not null)
        {
            int region = Convert.ToInt32(dgvResumenRegiones.CurrentRow.Cells[0].Value);
            object objValue = dgvResumenRegiones.CurrentRow.Cells[1].Value;
            string nombreRegion;
            if (objValue == null)
                nombreRegion = string.Empty;
            else
            {
                nombreRegion = Convert.ToString(objValue)??string.Empty;
            }
            if (region != 0)
            {
                MuestraDatosAgencia(region, nombreRegion);
                tabReportesFinales.SelectedIndex = 2;
            }
        }
    }

    private void DgvRcDobleClick(object sender, DataGridViewCellEventArgs e)
    {
        // Falta seleccionar el renglon
        SeleccionaRegion();
    }

    private void CalculaExpedientesAMostrar()
    {
        if (_windowsFormsGloablInformation is not null) {
            CreditosTotales creditosTotales = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosTotales();            
            lblResumenCreditosVigentes.Text = String.Format("{0:#,##0} Créditos Vigentes",creditosTotales.CreditosVigentes);
            lblResumenCreditosImpagos.Text = String.Format("{0:#,##0} Créditos en Impago", creditosTotales.Impagos);
            lblResumenCreditosVencida.Text = String.Format("{0:#,##0} Créditos Vencidos", creditosTotales.CarteraVencida);
        } 
    }

    private static int ConvierteNumeroRegion(int origen) {
        return origen switch
        {
            0 => 100,
            1 => 200,
            2 => 300,
            3 => 500,
            4 => 600,
            _ => 0,
        };
    }

    protected void PintaTodoElReporte()
    {
        ReporteDiario rpt = new();
        int iCentroOccidente = 0;
        int iNorOeste = 1;
        int iNorte = 2;
        int iSur = 3;
        int iSurEste = 4;

        IEnumerable<ResumenDeCreditos>? creditosRegionales = null;

        if (_windowsFormsGloablInformation is not null)
        {
            creditosRegionales = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaCreditosRegiones();
            dgvResumenRegiones.DataSource = creditosRegionales;
        }

        #region Distribucion Por Monto
        if (creditosRegionales is not null)
        {
            var saldoTotal = creditosRegionales.Where(x => x.Region == 0).Select(y => y.SaldosTotal).FirstOrDefault();
            if (saldoTotal == 0)
            { saldoTotal = 1; }

            for (int iRegion = 0; iRegion < 5; iRegion++)
            {
                var saldoRegion = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.SaldosTotal).FirstOrDefault();
                rpt.DistribucionPorMonto[iRegion].PorcentajeDeLaRegion = (saldoRegion / saldoTotal) * 100;

            }
        }
        else
        {
            rpt.DistribucionPorMonto[iCentroOccidente].PorcentajeDeLaRegion = 28.7M;
            rpt.DistribucionPorMonto[iNorOeste].PorcentajeDeLaRegion = 22.7M;
            rpt.DistribucionPorMonto[iNorte].PorcentajeDeLaRegion = 23.2M;
            rpt.DistribucionPorMonto[iSur].PorcentajeDeLaRegion = 14.4M;
            rpt.DistribucionPorMonto[iSurEste].PorcentajeDeLaRegion = 11.1M;
        }
        #endregion

        #region Distribucion Por Creditos y Clientes
        if (creditosRegionales is not null)
        {
            for (int iRegion = 0; iRegion < 5; iRegion++)
            {
                var cantidadDeCreditos = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.TotalDeCreditos).FirstOrDefault();
                var cantidadDeClientes = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.TotalDeClientes).FirstOrDefault();
                rpt.BarrasPorNumeroDeCredito[iRegion].CantidadDeCreditos = cantidadDeCreditos;
                rpt.BarrasPorNumeroDeCredito[iRegion].NumeroDeClientes = cantidadDeClientes;
            }
        }
        else
        {
            rpt.BarrasPorNumeroDeCredito[iCentroOccidente].CantidadDeCreditos = 9895;
            rpt.BarrasPorNumeroDeCredito[iCentroOccidente].NumeroDeClientes = 5276;

            rpt.BarrasPorNumeroDeCredito[iNorOeste].CantidadDeCreditos = 5605;
            rpt.BarrasPorNumeroDeCredito[iNorOeste].NumeroDeClientes = 3393;

            rpt.BarrasPorNumeroDeCredito[iNorte].CantidadDeCreditos = 4976;
            rpt.BarrasPorNumeroDeCredito[iNorte].NumeroDeClientes = 1622;

            rpt.BarrasPorNumeroDeCredito[iSur].CantidadDeCreditos = 4729;
            rpt.BarrasPorNumeroDeCredito[iSur].NumeroDeClientes = 2592;

            rpt.BarrasPorNumeroDeCredito[iSurEste].CantidadDeCreditos = 2859;
            rpt.BarrasPorNumeroDeCredito[iSurEste].NumeroDeClientes = 1713;
        }
        #endregion

        #region Saldos
        if (creditosRegionales is not null)
        {
            rpt.SaldosCreditos.SaldoTotalCartera = Math.Round(creditosRegionales.Where(x => x.Region == 0).Select(y => y.SaldosTotal).FirstOrDefault()/ 1000000,1);
            rpt.SaldosCreditos.NoDeClientes = creditosRegionales.Where(x => x.Region == 0).Select(y => y.TotalDeClientes).FirstOrDefault();
            rpt.SaldosCreditos.NoDeCreditos = creditosRegionales.Where(x => x.Region == 0).Select(y => y.TotalDeCreditos).FirstOrDefault();
            rpt.SaldosCreditos.SaldoTotalCarteraVigente = Math.Round(creditosRegionales.Where(x => x.Region == 0).Select(y => y.SaldosCarteraVigente).FirstOrDefault() / 1000000, 1);
            rpt.SaldosCreditos.SaldoTotalCarteraVencida = Math.Round(creditosRegionales.Where(x => x.Region == 0).Select(y => y.SaldosCarteraVencida).FirstOrDefault() / 1000000, 1);
            rpt.SaldosCreditos.Impago = Math.Round(creditosRegionales.Where(x => x.Region == 0).Select(y => y.SaldosCarteraImpago).FirstOrDefault() / 1000000, 1);
            rpt.SaldosCreditos.ICV = Math.Round(creditosRegionales.Where(x => x.Region == 0).Select(y => y.ICV).FirstOrDefault(), 2);
        }
        else
        {
            rpt.SaldosCreditos.SaldoTotalCartera = 26900.1M;
            rpt.SaldosCreditos.NoDeClientes = 14596;
            rpt.SaldosCreditos.NoDeCreditos = 28064;
            rpt.SaldosCreditos.SaldoTotalCarteraVigente = 16086.7M;
            rpt.SaldosCreditos.SaldoTotalCarteraVencida = 10813.4M;
            rpt.SaldosCreditos.ICV = 40.2M;
        }
        #endregion

        #region Comportamiento diario Saldos
        rpt.ComportamientosDiarios.DiaHabil[0] = "1 mar";
        rpt.ComportamientosDiarios.DiaHabil[1] = "7 mar";
        rpt.ComportamientosDiarios.DiaHabil[2] = "13 mar";
        rpt.ComportamientosDiarios.DiaHabil[3] = "17 mar";
        rpt.ComportamientosDiarios.DiaHabil[4] = "24 mar";
        rpt.ComportamientosDiarios.DiaHabil[5] = "28 mar";

        rpt.ComportamientosDiarios.Saldo[0] = 28536M;
        rpt.ComportamientosDiarios.Saldo[1] = 28160M;
        rpt.ComportamientosDiarios.Saldo[2] = 27737M;
        rpt.ComportamientosDiarios.Saldo[3] = 27421M;
        rpt.ComportamientosDiarios.Saldo[4] = 27050M;
        rpt.ComportamientosDiarios.Saldo[5] = 26900M;

        rpt.ComportamientosDiarios.CarteraVencida[0] = 9568M;
        rpt.ComportamientosDiarios.CarteraVencida[1] = 9782M;
        rpt.ComportamientosDiarios.CarteraVencida[2] = 10262M;
        rpt.ComportamientosDiarios.CarteraVencida[3] = 10440M;
        rpt.ComportamientosDiarios.CarteraVencida[4] = 10660M;
        rpt.ComportamientosDiarios.CarteraVencida[5] = 10813M;

        rpt.ComportamientosDiarios.IndiceCarteraVencida[0] = 33.5M;
        rpt.ComportamientosDiarios.IndiceCarteraVencida[1] = 34.7M;
        rpt.ComportamientosDiarios.IndiceCarteraVencida[2] = 37.0M;
        rpt.ComportamientosDiarios.IndiceCarteraVencida[3] = 38.1M;
        rpt.ComportamientosDiarios.IndiceCarteraVencida[4] = 39.4M;
        rpt.ComportamientosDiarios.IndiceCarteraVencida[5] = 40.2M;
        #endregion

        #region Pagos recibidos
        rpt.PagosRecibidos.MontoPagosRecibidos = 5984.1M;
        rpt.PagosRecibidos.MontoPagoPuntual = 1017.4M;
        rpt.PagosRecibidos.MontoPagoAnticipado = 3342.9M;
        rpt.PagosRecibidos.MontoPagoIncumplimiento = 1623.9M;
        #endregion

        #region Comportamiento Pagos Recibidos
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[0] = 76;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[0] = "1 mar";
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[1] = 97;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[1] = "7 mar";
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[2] = 81;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[2] = "13 mar";
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[3] = 55;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[3] = "17 mar";
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[4] = 129;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[4] = "24 mar";
        rpt.ComportamientosPagosRecibidos.PagosRecibidos[5] = 63;
        rpt.ComportamientosPagosRecibidos.DiaRecibido[5] = "28 mar";
        #endregion

        #region regiones del Mapa
        if (creditosRegionales is not null)
        {
            for (int iRegion = 0; iRegion < 5; iRegion++)
            {
                var saldo = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.SaldosTotal).FirstOrDefault();
                var saldoVigente = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.SaldosCarteraVigente).FirstOrDefault();
                var saldoVencida = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.SaldosCarteraVencida).FirstOrDefault();
                var icv = creditosRegionales.Where(x => x.Region == ConvierteNumeroRegion(iRegion)).Select(y => y.ICV).FirstOrDefault();

                rpt.RegionesMapa[iRegion].PagosRecibidos = 0M;
                rpt.RegionesMapa[iRegion].Saldo = Math.Round(saldo / 1000000,1);
                rpt.RegionesMapa[iRegion].SaldoVigente = Math.Round(saldoVigente / 1000000, 1);
                rpt.RegionesMapa[iRegion].SaldoVencido = Math.Round(saldoVencida / 1000000, 1);
                rpt.RegionesMapa[iRegion].IndiceCarteraVigente = Math.Round(icv,1);
        }

        }
        else
        {
            rpt.RegionesMapa[iCentroOccidente].PagosRecibidos = 2264.3M;
            rpt.RegionesMapa[iCentroOccidente].Saldo = 7714.7M;
            rpt.RegionesMapa[iCentroOccidente].SaldoVigente = 4384M;
            rpt.RegionesMapa[iCentroOccidente].SaldoVencido = 3330M;
            rpt.RegionesMapa[iCentroOccidente].IndiceCarteraVigente = 43.2M;

            rpt.RegionesMapa[iNorOeste].PagosRecibidos = 681.1M;
            rpt.RegionesMapa[iNorOeste].Saldo = 6101.4M;
            rpt.RegionesMapa[iNorOeste].SaldoVigente = 3741M;
            rpt.RegionesMapa[iNorOeste].SaldoVencido = 2360M;
            rpt.RegionesMapa[iNorOeste].IndiceCarteraVigente = 38.7M;

            rpt.RegionesMapa[iNorte].PagosRecibidos = 1672.9M;
            rpt.RegionesMapa[iNorte].Saldo = 6231.2M;
            rpt.RegionesMapa[iNorte].SaldoVigente = 4799M;
            rpt.RegionesMapa[iNorte].SaldoVencido = 1432M;
            rpt.RegionesMapa[iNorte].IndiceCarteraVigente = 23.0M;

            rpt.RegionesMapa[iSur].PagosRecibidos = 829.4M;
            rpt.RegionesMapa[iSur].Saldo = 3874.5M;
            rpt.RegionesMapa[iSur].SaldoVigente = 1544M;
            rpt.RegionesMapa[iSur].SaldoVencido = 2331M;
            rpt.RegionesMapa[iSur].IndiceCarteraVigente = 60.2M;

            rpt.RegionesMapa[iSurEste].PagosRecibidos = 536.5M;
            rpt.RegionesMapa[iSurEste].Saldo = 2978.4M;
            rpt.RegionesMapa[iSurEste].SaldoVigente = 1618M;
            rpt.RegionesMapa[iSurEste].SaldoVencido = 1360M;
            rpt.RegionesMapa[iSurEste].IndiceCarteraVigente = 45.7M;
        }
        #endregion

        #region Etiquetas mapa
        RegionMapaLabel[] etiquetas = new RegionMapaLabel[5];

        etiquetas[iCentroOccidente] = new RegionMapaLabel
        {
            LblPagosRecibidos = lblCentroOccidentePagosRecibidos,
            LblSaldos = lblCentroOccidenteSaldos,
            LblSaldosVencidos = lblCentroOccidenteVencido,
            LblSaldosVigente = lblCentroOccidenteVigente,
            LblICV = lblCentroOccidenteICV
        };

        etiquetas[iNorte] = new RegionMapaLabel
        {
            LblPagosRecibidos = lblNortePagosRecibidos,
            LblSaldos = lblNorteSaldos,
            LblSaldosVencidos = lblNorteVencido,
            LblSaldosVigente = lblNorteVigente,
            LblICV = lblNorteICV
        };

        etiquetas[iNorOeste] = new RegionMapaLabel
        {
            LblPagosRecibidos = lblNorOestePagosRecibidos,
            LblSaldos = lblNorOesteSaldos,
            LblSaldosVencidos = lblNorOesteVencido,
            LblSaldosVigente = lblNorOesteVigente,
            LblICV = lblNorOesteICV
        };

        etiquetas[iSur] = new RegionMapaLabel
        {
            LblPagosRecibidos = lblSurPagosRecibidos,
            LblSaldos = lblSurSaldos,
            LblSaldosVencidos = lblSurVencido,
            LblSaldosVigente = lblSurVigente,
            LblICV = lblSurICV
        };

        etiquetas[iSurEste] = new RegionMapaLabel
        {
            LblPagosRecibidos = lblSurEstePagosRecibidos,
            LblSaldos = lblSurEsteSaldos,
            LblSaldosVencidos = lblSurEsteVencido,
            LblSaldosVigente = lblSurEsteVigente,
            LblICV = lblSurEsteICV
        };
        #endregion

        // PintaReporte pinta = new();
        PintaReporte.PintaDistribucionDeSaldos(rpt, chrtPieDistribucionSaldos);
        PintaReporte.PintaDistribucionDeNumerosDeCredito(rpt, chrtPorNumCreditos);
        PintaReporte.PintaInformacionSaldoTodosLosCreditos(rpt, lblSaldoMDP, lblNoClientes, lblNoDeCreditos, lblSaldoVigente, lblSaldoVencida, lblSaldoImpago,lblICV);
        PintaReporte.PintaInformacionComportamientosDiarios(rpt, chrtSaldo, chrtCV, chrtICV);
        PintaReporte.PintaInformacionPagosRecibidos(rpt, lblPagosRecibidos, lblPrPagoPuntual, lblPrPagoAnticipado, lblPrPagoDeIncumplimiento);
        PintaReporte.PintaInformacionComportamientosDiarios(rpt, chrtComportamientoDiario);
        PintaReporte.PintaRegiones(rpt, etiquetas);
        tabReportesFinales.SelectedIndex = 1;
    }
}
