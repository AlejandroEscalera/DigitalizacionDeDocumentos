using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen.Entidades
{
    public class ReporteDiario
    {
        public RegionDistribucionPorMonto[] DistribucionPorMonto { get { return _distribucionPorMonto; } set { if (_distribucionPorMonto is not null) { _distribucionPorMonto = InicializaDistribucionSaldosPorMonto(); } } }
        public Color ColorLineaDistribucionPorNumeroDeCreditosBarra { get; set; }
        public Color ColorLineaDistribucionPorNumeroDeCreditosLinea { get; set; }

        public RegionPorNumeroDeCreditos[] BarrasPorNumeroDeCredito { get { return _barrasPorNumeroDeCredito; } set { if (_barrasPorNumeroDeCredito is not null) { _barrasPorNumeroDeCredito = InicializaBarrasPorNumeroDeCredito(); } } }

        public SaldosTodosLosCreditos SaldosCreditos { get; set; }
        public ComportamientoDiario ComportamientosDiarios { get; set; }

        public PagosRecibidos PagosRecibidos { get; set; }

        public ComportamientoDiarioPagosRecibidos ComportamientosPagosRecibidos { get; set; }
        public Color ColorComportamientosPagosRecibidos { get; set; }

        public RegionMapa[] RegionesMapa { get; set; }
        public ReporteDiario()
        {
            _distribucionPorMonto = InicializaDistribucionSaldosPorMonto();
            _barrasPorNumeroDeCredito = InicializaBarrasPorNumeroDeCredito();
            SaldosCreditos = new SaldosTodosLosCreditos();
            ComportamientosDiarios = new ComportamientoDiario();
            PagosRecibidos = new PagosRecibidos();
            ComportamientosPagosRecibidos = new ComportamientoDiarioPagosRecibidos();
            ColorComportamientosPagosRecibidos = Color.FromArgb(40, 92, 77);
            RegionesMapa = InicializaRegionesMapa();
        }

        private static RegionMapa[] InicializaRegionesMapa()
        {
            RegionMapa[] resultado = new RegionMapa[5];
            for (int i = 0; i < 5; i++)
            {
                resultado[i] = new RegionMapa();
            }
            resultado[0].NombreRegion = "Centro Occidente";
            resultado[1].NombreRegion = "Nor Oeste";
            resultado[2].NombreRegion = "Norte";
            resultado[3].NombreRegion = "Sur";
            resultado[4].NombreRegion = "Sur Este";
            return resultado;
        }

        #region DistribucionSaldosPorMonto
        private RegionDistribucionPorMonto[] _distribucionPorMonto;

        private static RegionDistribucionPorMonto[] InicializaDistribucionSaldosPorMonto()
        {
            RegionDistribucionPorMonto[] resultado = new RegionDistribucionPorMonto[5];
            for (int i = 0; i < 5; i++)
            {
                resultado[i] = new RegionDistribucionPorMonto();
            }
            resultado[0].Sobresale = false;
            resultado[0].NombreDeLaRegion = "C-O";
            resultado[0].PorcentajeDeLaRegion = 28.6M;
            resultado[0].Color = Color.FromArgb(212, 193, 156);

            resultado[1].Sobresale = false;
            resultado[1].NombreDeLaRegion = "NO";
            resultado[1].PorcentajeDeLaRegion = 22.6M;
            resultado[1].Color = Color.FromArgb(98, 17, 50);

            resultado[2].Sobresale = false;
            resultado[2].NombreDeLaRegion = "N";
            resultado[2].PorcentajeDeLaRegion = 23.1M;
            resultado[2].Color = Color.FromArgb(40, 92, 77);

            resultado[3].Sobresale = false;
            resultado[3].NombreDeLaRegion = "S";
            resultado[3].PorcentajeDeLaRegion = 14.4M;
            resultado[3].Color = Color.FromArgb(19, 50, 43);

            resultado[4].Sobresale = false;
            resultado[4].NombreDeLaRegion = "SE";
            resultado[4].PorcentajeDeLaRegion = 11.3M;
            resultado[4].Color = Color.FromArgb(179, 142, 93);
            return resultado;
        }
        #endregion

        #region Distribucion por Num Creditos
        private RegionPorNumeroDeCreditos[] _barrasPorNumeroDeCredito;
        private RegionPorNumeroDeCreditos[] InicializaBarrasPorNumeroDeCredito()
        {
            ColorLineaDistribucionPorNumeroDeCreditosBarra = Color.FromArgb(156, 35, 72);
            ColorLineaDistribucionPorNumeroDeCreditosLinea = Color.FromArgb(212, 193, 156);
            RegionPorNumeroDeCreditos[] resultado = new RegionPorNumeroDeCreditos[5];

            for (int i = 0; i < 5; i++)
            {
                resultado[i] = new RegionPorNumeroDeCreditos();
            }
            resultado[0].NombreDeLaRegion = "C-O";
            resultado[0].NumeroDeClientes = 10051;
            resultado[0].CantidadDeCreditos = 5374;

            resultado[1].NombreDeLaRegion = "NO";
            resultado[1].NumeroDeClientes = 5693;
            resultado[1].CantidadDeCreditos = 3434;

            resultado[2].NombreDeLaRegion = "N";
            resultado[2].NumeroDeClientes = 5090;
            resultado[2].CantidadDeCreditos = 1647;

            resultado[3].NombreDeLaRegion = "S";
            resultado[3].NumeroDeClientes = 4830;
            resultado[3].CantidadDeCreditos = 2649;

            resultado[4].NombreDeLaRegion = "SE";
            resultado[4].NumeroDeClientes = 2936;
            resultado[4].CantidadDeCreditos = 1763;
            return resultado;
        }

        #endregion
    }

    public class RegionDistribucionPorMonto
    {
        public Color Color { get; set; }
        public string? NombreDeLaRegion { get; set; }
        public decimal PorcentajeDeLaRegion { get; set; }
        public bool Sobresale { get; set; }
        public string ObtienePorcentajeDeLaRegion()
        {
            return string.Format("{0}\n{1:#0.0}%", NombreDeLaRegion, PorcentajeDeLaRegion);
        }
    }

    public class RegionPorNumeroDeCreditos
    {
        public decimal CantidadDeCreditos { get; set; }
        public int NumeroDeClientes { get; set; }
        public string? NombreDeLaRegion { get; set; }

        public string ObtieneCantidadDeCreditos()
        {
            return String.Format("{0:##,##0}%", CantidadDeCreditos);
        }
        public string ObtieneNumeroDeClientes()
        {
            return String.Format("{0:##,##0}%", NumeroDeClientes);
        }
    }

    public class SaldosTodosLosCreditos
    {
        public decimal SaldoTotalCartera { get; set; }
        public int NoDeClientes { get; set; }
        public int NoDeCreditos { get; set; }
        public decimal SaldoTotalCarteraVigente { get; set; }
        public decimal SaldoTotalCarteraVencida { get; set; }
        public decimal Impago { get; set; }
        public decimal ICV { get; set; }
        public string ObtieneSaldoTotalCartera()
        {
            return String.Format("{0:##,##0.0} mdp", SaldoTotalCartera);
        }
        public string ObtieneNoDeClientes()
        {
            return String.Format("{0:##,##0}", NoDeClientes);
        }
        public string ObtieneNoDeCreditos()
        {
            return String.Format("{0:##,##0}", NoDeCreditos);
        }
        public string ObtieneSaldoTotalCarteraVigente()
        {
            return String.Format("{0:##,##0.0} mdp", SaldoTotalCarteraVigente);
        }
        public string ObtieneSaldoTotalCarteraVencida()
        {
            return String.Format("{0:##,##0.0} mdp", SaldoTotalCarteraVencida);
        }
        public string ObtieneSaldoImpago()
        {
            return String.Format("{0:##,##0.0} mdp", Impago);
        }
        public string ObtieneICV()
        {
            return String.Format("{0:#0.00}%", ICV);
        }
    }

    public class ComportamientoDiario
    {
        public string[] DiaHabil { get; set; }
        public Color ColorDiaHabil { get; set; }

        public decimal[] Saldo { get; set; }
        public Color ColorSaldo { get; set; }

        public decimal[] CarteraVencida { get; set; }
        public Color ColorCarteraVencida { get; set; }

        public decimal[] IndiceCarteraVencida { get; set; }
        public Color ColorIndiceCarteravencida { get; set; }

        public ComportamientoDiario()
        {
            DiaHabil = new string[6];
            Saldo = new decimal[6];
            CarteraVencida = new decimal[6];
            IndiceCarteraVencida = new decimal[6];
            ColorSaldo = Color.FromArgb(156, 35, 72);
            ColorCarteraVencida = Color.FromArgb(98, 17, 50);
            ColorIndiceCarteravencida = Color.FromArgb(179, 142, 93);
        }

        public string GetDiaHabil(int indice)
        {
            if (indice >= 0 && indice <= 5)
            {
                return DiaHabil[indice];
            }
            return string.Empty;
        }

        public string GetSaldo(int indice)
        {
            if (indice >= 0 && indice <= 5)
            {
                return string.Format("{0:##,##0}", Saldo[indice]);
            }
            return string.Empty;
        }
        public string GetCarteraVencida(int indice)
        {
            if (indice >= 0 && indice <= 5)
            {
                return string.Format("{0:##,##0}", CarteraVencida[indice]);
            }
            return string.Empty;
        }
        public string GetIndiceCarteraVencida(int indice)
        {
            if (indice >= 0 && indice <= 5)
            {
                return string.Format("{0:##0.0}%", IndiceCarteraVencida[indice]);
            }
            return string.Empty;
        }

    }

    public class PagosRecibidos
    {
        public decimal MontoPagosRecibidos { get; set; }
        public decimal MontoPagoPuntual { get; set; }
        public decimal MontoPagoAnticipado { get; set; }
        public decimal MontoPagoIncumplimiento { get; set; }

        public string ObtieneMontoPagosRecibidos()
        {
            return string.Format("{0:##,##0.0} mdp", MontoPagosRecibidos);
        }

        public string ObtieneMontoPagoPuntual()
        {
            return string.Format("{0:##,##0.0} mdp", MontoPagoPuntual);
        }
        public string ObtieneMontoPagoAnticipado()
        {
            return string.Format("{0:##,##0.0} mdp", MontoPagoAnticipado);
        }
        public string ObtieneMontoPagoIncumplimiento()
        {
            return string.Format("{0:##,##0.0} mdp", MontoPagoIncumplimiento);
        }
    }

    public class ComportamientoDiarioPagosRecibidos
    {
        public decimal[] PagosRecibidos { get; set; }
        public string[] DiaRecibido { get; set; }

        public ComportamientoDiarioPagosRecibidos()
        {
            PagosRecibidos = new decimal[6];
            DiaRecibido = new string[6];
        }

        public string ObtienePagoRecibido(int dia)
        {
            if (dia >= 0 && dia <= 5)
            {
                return string.Format("{0:##0}", PagosRecibidos[dia]);
            }
            return string.Empty;
        }
    }

    public class RegionMapa
    {
        public string NombreRegion { get; set; }
        public decimal PagosRecibidos { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoVigente { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal IndiceCarteraVigente { get; set; }

        public RegionMapa()
        {
            NombreRegion = string.Empty;
            /*
        a) Pagos recibidos    ##,##0.0 mdp
       b) Saldo              ##,##0.0 mdp 
       c) Saldo vigente      ##,##0.0 mdp 
       d) Saldo vencido      ##,##0.0 mdp
       e) Indice de Cartera Viguente  #0.0%
            */
        }
        public string ObtienePagosRecibidos()
        {
            return string.Format("{0:#,##0.0} mdp", PagosRecibidos);
        }

        public string ObtieneSaldo()
        {
            return string.Format("{0:#,##0.0} mdp", Saldo);
        }
        public string ObtieneSaldoVigente()
        {
            return string.Format("{0:#,##0.0} mdp", SaldoVigente);
        }
        public string ObtieneSaldoVencido()
        {
            return string.Format("{0:#,##0.0} mdp", SaldoVencido);
        }
        public string ObtieneIndiceCarteraVigente()
        {
            return string.Format("{0:#0.0} %", IndiceCarteraVigente);
        }

    }
}
