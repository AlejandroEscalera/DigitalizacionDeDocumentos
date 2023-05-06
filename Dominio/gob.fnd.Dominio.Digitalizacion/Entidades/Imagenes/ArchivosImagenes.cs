using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes
{
    public class ArchivosImagenes
    {
        /// <summary>
        /// Renglon de carga de información
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Llave de búsqueda
        /// </summary>
        public string? IdSearch { get; set; }
        /// <summary>
        /// Nombre del archivo del expediente digital
        /// </summary>
        public string? NombreArchivo { get; set; }
        /// <summary>
        /// Numero de crédito o de apertura
        /// </summary>
        public string? NumCredito { get; set; }
        /// <summary>
        /// Fecha de Modificación
        /// </summary>
        public DateTime? FechaDeModificacion { get; set; }
        /// <summary>
        /// Usuario de Modificación
        /// </summary>
        public string? UsuarioDeModificacion { get; set; }
        /// <summary>
        /// Tamaño del archivo
        /// </summary>
        public long TamanioArchivo { get; set; }
        /// <summary>
        /// Ruta de guardado (PATH) que aparece en el archivo.
        /// </summary>
        public string? RutaDeGuardado { get; set; }
        /// <summary>
        /// El url para poder encontrar el archivo en el sharepoint
        /// </summary>
        public string? UrlArchivo { get; set; }
        /// <summary>
        /// Número de minisración
        /// </summary>
        public int Ministracion { get; set; }
        /// <summary>
        /// Si el contrato se encuentra vigente o vencido
        /// </summary>
        public string? VigenciaDelContato { get; set; }
        /// <summary>
        /// Fecha de vigencia del contrato
        /// </summary>
        public DateTime? FechaVigenciaDelaDispOContrato { get; set; }
        /// <summary>
        /// Si es banca de Primer o segundo piso
        /// </summary>
        public string? CheckList { get; set; }
        /// <summary>
        /// Si ya fue validad por calidad
        /// </summary>
        public bool ValidadoPorCalidad { get; set; }
        /// <summary>
        /// Obtengo el archivo de donde salió la información del credito
        /// </summary>
        public string? ArchivoOrigen { get; set; }
        /// <summary>
        /// Pongo la información del número de crédito activo
        /// </summary>
        public string? NumeroCreditoActivo { get; set; }
        /// <summary>
        /// Agrego el tipo de castigo de la cartera
        /// </summary>
        public string? Castigo { get; set; }
        /// <summary>
        /// Agrego si el archivo pertenece al período de doctor
        /// </summary>
        public bool EsOrigenDelDoctor { get; set; }
        /// <summary>
        /// Señalo si el archivo pertenece a una cartera activa
        /// </summary>
        public bool EsCarteraActiva { get; set; }
        /// <summary>
        /// Número de cliente
        /// </summary>
        public string? NumCte { get; set; }
        /// <summary>
        /// Nombre del acreditado
        /// </summary>
        public string? Acreditado { get; set; }
        /// <summary>
        /// Numero de Contrato
        /// </summary>
        public string? NumContrato { get; set; }
        /// <summary>
        /// Fecha de Apertura
        /// </summary>
        public DateTime? FechaApertura { get; set; }
        public bool EsCancelacionDelDoctor { get; set; }
        public int NumeroMinistracion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public string? ClasifMesa { get; set; }
        public DateTime? FechaAsignacionEntrada { get; set; }
        public Decimal MontoOtorgado { get; set; }
        public string? Analista { get; set; }
        public int Regional { get; set; }
        public int Sucursal { get; set; }
        public string? CatAgencia { get; set; }
        public string? Extension { get; set; }
        public string? CarpetaDestino { get; set; }
        public int ThreadId { get; set; }
        public int Job { get; set; }
        public bool PorDescargar { get; set; }
        public bool ErrorAlDescargar { get; set; }
        public string? MensajeDeErrorAlDescargar { get; set; }
        public bool PorOcr { get; set; }
        public bool ErrorOcr { get; set; }
        public string? MensajeDeErrorOCR { get; set; }
        public bool PorAOcr { get; set; }
        public bool ErrorAOcr { get; set; }
        public string? MensajeDeErrorAOCR { get; set; }
        public bool PorAcomodar { get; set; }
        public bool ErrorAcomodar { get; set; }
        public string? MensajeDeErrorAcomodar { get; set; }
        public bool Termino { get; set; }
        public string? Hash { get; set; }
        public string? CadenaCredito { get; set; }
        public string? CadenaCliente { get; set; }
        public string? OcrNumCredito { get; set; }
        public string? OcrNumCliente { get; set; }
        public bool IgualNumCredito { get; set; }
        public bool TieneNumCredito { get; set; }
        public bool ChecarTona { get; set; }
        public int NumPaginas { get; set; }
        public int SubArchivo { get; set; }
    }
}
