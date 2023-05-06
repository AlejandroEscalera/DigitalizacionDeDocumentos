using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC
{
    public class ABSaldosConCastigo : BaseCredito.CreditoBase
    {
        /// <summary>
        /// Catálogo de la región
        /// </summary>
        public string? CatRegion { get; set; }  // Region
        /// <summary>
        /// Sucursal
        /// </summary>
        public double Sucursal { get; set; }  // Agencia
        /// <summary>
        /// Catálogo de Sucursales
        /// </summary>
        public string? CatSucursal { get; set; } // NOM_AGENCIA
        /// <summary>
        /// Número de cliente
        /// </summary>
        public string? NumeroCliente { get; set; }  // num_cte
        /// <summary>
        /// Apellido Paterno
        /// </summary>
        public string? ApellidoPaterno { get; set; } // No aplica
        /// <summary>
        /// Apellido Materno
        /// </summary>
        public string? ApellidoMaterno { get; set; } // No aplica
        /// <summary>
        /// Primer Nombre
        /// </summary>
        public string? PrimerNombre { get; set; } // No aplica
        /// <summary>
        /// Segundo Nombre
        /// </summary>
        public string? SegundoNombre { get; set; } // No aplica
        /// <summary>
        ///  Razon Social
        /// </summary>
        public string? RazonSocial { get; set; } // No aplica
        /// <summary>
        /// Nombre del cliente ya concatenado
        /// </summary>
        public string? Acreditado { get; set; }  // Nombre_Cliente
        /*
        /// <summary>
        /// Numero de Credito
        /// </summary>
        public string? NumCredito { get; set; }  //  num_credito (actual)
        */
        public bool EsTratamiento { get; set; }
        /// <summary>
        /// Número de crédito cuando no estaba castigado
        /// </summary>
        public string? NumCreditoOriginal { get; set; }
        /// <summary>
        /// Apertura de Credito, es el nombre que tendrá el archivo del expediente
        /// </summary>
        public string? NumeroAperturaCredito { get; set; }  //  num_credito poniendo los últimos 4 digitos en cero.
        /// <summary>
        /// Fecha de Apertura
        /// </summary>
        public DateTime? FechaApertura { get; set; } // fecha_apertura 
        /// <summary>
        /// Fecha en que se vence el compromiso del crédito
        /// </summary>
        public DateTime? FechaVencimiento { get; set; } // fecha_vencimiento
        /// <summary>
        /// Fecha de Ministración
        /// </summary>
        public DateTime? FechaMinistra { get; set; } // fecha_apertura, aqui se debe calcular como la fecha de dispersión
        /// <summary>
        /// Nombre del Ejecutivo
        /// </summary>
        public string? NombreEjecutivo { get; set; }  // No aplica
        /// <summary>
        /// Para poder hacer el filtro de los catálogos de productos
        /// </summary>
        public string? CatProducto { get; set; }  
        /// <summary>
        /// Tipo de Crédito
        /// </summary>
        public string? TipoDeCredito { get; set; }  // Tipo_Credito
        /// <summary>
        /// Num Producto
        /// </summary>
        public string? NumProducto { get; set; } // num_producto
        /// <summary>
        /// Monto del credito directamente de ABSaldos
        /// </summary>
        public Decimal? MontoCredito { get; set; }  // Capital_Cont
        /// <summary>
        /// Interes del crédito directamente de ABSaldos
        /// </summary>
        public Decimal? InteresCont { get; set; }  // Interes_Cont
        /// <summary>
        /// Nombre del agente encargado del crédito
        /// </summary>
        public string? Agente { get; set; }  // Se calcula
        /// <summary>
        /// Nombre del responsable del guardavalores del crédito
        /// </summary>
        public string? GuardaValores { get; set; }  // Se calcula
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
        /// Si se encuentra la imagen
        /// </summary>
        public bool ExpedienteDigital { get; set; }
        /// <summary>
        /// Número de archivos de imagen
        /// </summary>
        public int NoPDF { get; set; }
        /// <summary>
        /// Está en Guarda Valores
        /// </summary>        
        public bool CuentaConGuardaValores { get; set; }
        /// <summary>
        /// Cuanto se le entregó la ultima vez al usuario
        /// </summary>
        public Decimal? MontoMinistraCap { get; set; }  // Capital_Cont
        public string? TipoCartera { get; set; }
        public string? SesionDeAutorizacion { get; set; }
        public string? MesCastigo { get; set; }
        public int AnioCastigo { get; set; }
        public string? Generacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public DateTime? FechaDeSolicitud { get; set; }
        public int AnioOriginacion { get; set; }
        public int Piso { get; set; }
        public bool EstaEnCreditosCancelados { get; set; }
        public int NumeroMinistracion { get; set; }
        public DateTime? FechaInicioMinistracion { get; set; }
        public string? ClasificacionMesa { get; set; }
        public DateTime? FechaInicioSolicitud { get; set; }
        public Decimal MontoOtorgado { get; set; }
        public bool EstaEnMesa { get; set; }

    }
}
