using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo
{
    public class ArchivoAnalisisCamposArqueos
    {
        /// <summary>
        /// Número de archivo localizado
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// La ruta completa localizada
        /// </summary>
        public string? RutaCompleta { get; set; }
        /// <summary>
        /// Carpeta de la coordinación / agencia del archivo
        /// </summary>
        public string? Carpeta { get; set; }
        /// <summary>
        /// Nombre del archivo de Excel
        /// </summary>
        public string? NombreArchivo { get; set; }
        /// <summary>
        /// Tiene la hoja de PT Arqueo (VF)
        /// </summary>
        public bool TieneHojaPT { get; set; }
        /// <summary>
        /// Tiene la hoja BASE ABSaldos
        /// </summary>
        public bool TieneHojaBaseABSaldos { get; set; }
        /// <summary>
        /// Tiene la hoja RG-OPE-GVA-005-010
        /// </summary>
        public bool TieneHojaRgOpeGva005_010 { get; set; }
        /// <summary>
        /// Primer columna que hable del detalle de los guardas Valores
        /// </summary>
        public int PrimerColumnaDetalleGuardaValores { get; set; }
        /// <summary>
        /// Listado de campos del renglon de encabezados
        /// </summary>
        public string? CamposGuardaValores { get; set; }
        /// <summary>
        /// Primer renglon de la información del detalle de los expedientes
        /// </summary>
        public int PrimerRenglonGuardaValores { get; set; }
        /// <summary>
        /// Número de documentos que están registrados en el guarda valores
        /// </summary>
        public int NumeroGuardaValores { get; set; }
        /// <summary>
        /// Primer número de crédito que sirva de ejemplo y que se encuentre en el concentrado del ABSaldos
        /// </summary>
        public string? NoCreditoEjemplo { get; set; }
        /// <summary>
        /// Agencia del ejemplo del número de crédito encontrado en el AB Saldos, sirve para crear un nuevo arbol de carpetas
        /// </summary>
        public int NoAgencia { get; set; }
        /// <summary>
        /// Coordinación regional a la que pertenezca la agencia
        /// </summary>
        public string? Coordinacion { get; set; }
        /// <summary>
        /// Es para poder obtener los campos de destino de los Guardavalores
        /// </summary>
        public string[]? CamposDeInformacionGuardaValor { get; set; }
        /// <summary>
        /// Es el numero de Campos que tiene la información de guarda valores, es para poder encontrar las columnas de los campos en el detalle
        /// </summary>
        public int NumeroCamposGuardaValor { get; set; }
    }
}
