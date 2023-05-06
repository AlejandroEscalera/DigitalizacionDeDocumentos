using gob.fnd.Dominio.Digitalizacion.Entidades.BaseCredito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC
{
    public class ABSaldosActivos : CreditoBase
    {
        public int Id { get; set; }
        /// <summary>
        /// Numero de la regional
        /// </summary>
        public int Regional { get; set; }
        /// <summary>
        /// Obtengo en string la información de la región, importante para los cruces
        /// </summary>
        public string? CatRegion { get; set; }
        /// <summary>
        /// Número de client
        /// </summary>
        public string? NumCte { get; set; }
        /// <summary>
        /// Sucursal del cliente
        /// </summary>
        public int Sucursal { get; set; }
        /// <summary>
        /// Información de la sucursal donde se llevó a cabo el crédito
        /// </summary>
        public string? CatSucursal { get; set; }
        /// <summary>
        /// Apellido Paterno
        /// </summary>
        public string? Apell_Paterno { get; set; }
        /// <summary>
        /// Apellido Materno
        /// </summary>
        public string? Apell_Materno { get; set; }
        /// <summary>
        /// Nombre 1
        /// </summary>
        public string? Nombre1 { get; set; }
        /// <summary>
        ///  Nombre 2
        /// </summary>
        public string? Nombre2 { get; set; }
        /// <summary>
        /// Razon Social
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// Si es tratamiento de cartera
        /// </summary>
        public bool EsTratamiento { get; set; }
        /// <summary>
        /// Numero de producto
        /// </summary>
        public string? NumProducto { get; set; }
        /// <summary>
        /// Fecha de apertura
        /// </summary>
        public DateTime? FechaApertura { get; set; }
        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? FechaVencim { get; set; }
        /// <summary>
        /// Monto otorgado
        /// </summary>
        public decimal MontoOtorgado { get; set; }
        /// <summary>
        /// Monto Ministrado de Capital
        /// </summary>
        public decimal MontoMinistraCap { get; set; }
        /// <summary>
        /// Saldo de Capital Contable
        /// </summary>
        public decimal SldoCapCont { get; set; }
        /// <summary>
        /// Saldo de intereses de Capital Contable
        /// </summary>
        public decimal SldoIntCont { get; set; }
        /// <summary>
        /// Saldo total de Capital Contable
        /// </summary>
        public decimal SldoTotCont { get; set; }
        /// <summary>
        /// Saldo Total de Contabilidad Validado
        /// </summary>
        public decimal SldoTotContVal { get; set; }
        /// <summary>
        /// Número de Contrato
        /// </summary>
        public string? NumContrato { get; set; }
        /// <summary>
        /// Capital Vigente
        /// </summary>
        public decimal CapVig { get; set; }
        /// <summary>
        /// Intereses Finales Vigentes
        /// </summary>
        public decimal IntFinVig { get; set; }
        /// <summary>
        /// Intereses Finales Vigentes NP
        /// </summary>
        public decimal IntFinVigNp { get; set; }
        /// <summary>
        /// Capital Vencido
        /// </summary>
        public decimal CapVen { get; set; }
        /// <summary>
        /// Intereses Finales del Capital Vencidos
        /// </summary>
        public decimal IntFinVen { get; set; }
        /// <summary>
        /// Intereses Finales del Capital Vencidos NP
        /// </summary>
        public decimal IntFinVenNp { get; set; }
        /// <summary>
        /// Intereses Normales Vigentes
        /// </summary>
        public decimal IntNorVig { get; set; }
        /// <summary>
        /// Intereses Normales Vigentes NP
        /// </summary>
        public decimal IntNorVigNp { get; set; }
        /// <summary>
        /// Intereses Normales Vencidos
        /// </summary>
        public decimal IntNorVen { get; set; }
        /// <summary>
        /// Intereses Normales Vencidos NP
        /// </summary>
        public decimal IntNorVenNP { get; set; }
        /// <summary>
        /// Intereses Des Vencidos
        /// </summary>
        public decimal IntDesVen { get; set; }
        /// <summary>
        /// Intereses Pendientes
        /// </summary>
        public decimal IntPen { get; set; }
        /// <summary>
        /// Fecha de Ingreso Usuario GaAp
        /// </summary>
        public DateTime? FechaIngUsGaAp { get; set; }
        /// <summary>
        /// El nivel de castigo de la cartera
        /// </summary>
        public string? Castigo { get; set; }
        /// <summary>
        /// Agrego si el archivo pertenece al período de doctor
        /// </summary>
        public bool EsOrigenDelDoctor { get; set; }
        public bool EsCancelacionDelDoctor { get; set; }
        /// <summary>
        /// Señalo si el archivo pertenece a una cartera activa
        /// </summary>
        public bool EsCarteraActiva { get; set; }
        /// <summary>
        /// Nombre del Acreditado
        /// </summary>
        public string? Acreditado { get; set; }
        /// <summary>
        /// Si cuenta con la digitalizacion
        /// </summary>
        public bool ExpedienteDigital { get; set; }
        /// <summary>
        /// Numero de PDF's con los que cuenta
        /// </summary>
        public int NoPDF { get; set; }
        /// <summary>
        /// Si se tiene el desgloce de los GuardaValores
        /// </summary>
        public bool CuentaConGuardaValores { get; set; }
        public string? TipoCartera { get; set; }
        public string? CatProducto { get; set; }
        public string? NombreAgente { get; set; }
        public string? GuardaValores { get; set; }
        public string? NombreEjecutivo { get; set; }
        public string? NumCreditoOriginal { get; set; }
        public string? NumCreditoCancelado { get; set; }
        public string? SesionDeAutorizacion { get; set; }
        public string? MesCastigo { get; set; }
        public int AnioCastigo { get; set; }
        public string? Generacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public DateTime? FechaInicioMinistracion { get; set; }
        public DateTime? FechaDeSolicitud { get; set; }
        public bool EstaEnCreditosCancelados { get; set; }
        public int AnioOriginacion { get; set; }
        public int Piso { get; set; }
        public int NumeroMinistracion { get; set; }
        public string? ClasificacionMesa { get; set; }
        public DateTime? FechaAsignacionMesa { get; set; }
        public bool EstaEnMesa { get; set; }


    }
}
