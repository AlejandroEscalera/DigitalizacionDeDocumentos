using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo
{
    public class InformacionGuardaValor
    {
        /// <summary>
        /// El número de registro del archivo
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Se guarda la información adicional de que archivo provino la información
        /// </summary>
        public string? ArchivoOrigenInformacion { get; set; }
        /// <summary>
        /// Quien elaboró el arqueo
        /// </summary>
        public string? Auditor { get; set; } // campo 2 / 11
        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        public string? Acreditado { get; set; } // campo 4 /13
        /// <summary>
        /// Número de cliente
        /// </summary>
        public string? NumCte { get; set; } // campo 3 / 12
        /// Número de credito ministrado
        /// </summary>
        public string? NumeroCredito { get; set; } // campo 5 / 14
        /// <summary>
        /// Número de producto
        /// </summary>
        public string? NumProducto { get; set; } // Se tiene que sacar del numero de credito del ab saldos
        /// <summary>
        /// Tipo de credito
        /// </summary>
        public string? CatTipoCredito { get; set; } // Se obtiene del campo 6 / 15, pero hay que validar con absaldos
        /// <summary>
        /// Tipo de documento del guardavalor
        /// </summary>
        public string? TipoDeDocumento { get; set; } // campo 7/16
        /// <summary>
        /// Número de documento
        /// </summary>
        public string? NumeroDeDocumento { get; set; } // campo 9/18
        /// <summary>
        /// Importe o monto ministrado
        /// </summary>
        public Decimal Importe { get; set; } // campo 8 /17, pero hay que validar con absaldos
        /// <summary>
        /// Número de hojas
        /// </summary>
        public int NumeroDeHojas { get; set; } //  campo 11/20
        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? FechaDeVencimiento { get; set; } // hay que sacarlo del absaldos
        /// <summary>
        /// Calidad fisica del documento
        /// </summary>
        public string? EstadoFisico { get; set; }
        /// <summary>
        /// Ubicación donde se ubica el evento
        /// </summary>
        public string? UbicacionFisica { get; set; }
        /// <summary>
        /// Si esta vigente del crédito
        /// </summary>
        public string? Estatus { get; set; } // Campo 10/19
        /// <summary>
        /// Si el documento es correcto o incorrecto
        /// </summary>
        public string? CorrectoIncorrecto { get; set; }
        /// <summary>
        /// Si hay un hallazgo sobre el documento
        /// </summary>
        public string? TipoDeHallazgo { get; set; } // Campo 12/21
        /// <summary>
        /// Monto del hallazgo
        /// </summary>
        public decimal MontoDelHallazgo { get; set; } // hay que sacarlo del absaldos
        /// <summary>
        /// Detalle que describe el hallazgo
        /// </summary>
        public string? Detalle { get; set; } // Campo 13/22
        /// <summary>
        /// Indica si en la mesa 
        /// </summary>
        public bool TieneExpedienteDigital { get; set; }
        /// <summary>
        /// El estado del castigo que puede tener un crédito
        /// </summary>
        public string? Castigo { get; set; }
        /// <summary>
        /// Si el expediente es nuevo y requiere información detalle del guardavalor
        /// </summary>
        public bool EsNuevo { get; set; }
        /// <summary>
        /// Si se aperturó (originó) el crédito a partir del 1 de julio del 2020
        /// </summary>
        public bool AperturaTiempoDoctor { get; set; }
        /// <summary>
        /// Indica si ya fue baja en AB Saldos
        /// </summary>
        public bool AunEnABSaldos { get; set; }
        /// <summary>
        /// La region a la que pertenece el GV
        /// </summary>
        public int Region { get; set; }
        /// <summary>
        /// La agencia a la que pertenece el GV
        /// </summary>
        public int Sucursal { get; set; }
    }
}
