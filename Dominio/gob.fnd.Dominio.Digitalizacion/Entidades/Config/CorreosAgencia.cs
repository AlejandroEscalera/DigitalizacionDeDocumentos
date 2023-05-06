using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Config
{
    /// <summary>
    /// Los correos que son utilizados en una agencia
    /// </summary>
    public class CorreosAgencia
    {
        /// <summary>
        /// No de agencia
        /// </summary>
        public int NoAgencia { get; set; }
        /// <summary>
        /// Nombre de la agencia
        /// </summary>
        public string? Agencia { get; set; }
        /// <summary>
        /// Nombre del agente al que se le entregará el reporte
        /// </summary>
        public string? NombreAgente { get; set; }
        /// <summary>
        /// Correo del agente al que se le entregará el reporte
        /// </summary>
        public string? CorreoAgente { get; set; }
        /// <summary>
        /// Nombre del guarda valores de la agencia
        /// </summary>
        public string? NombreGuardaValores { get; set; }
        /// <summary>
        /// Correo electrónico del guarda valores
        /// </summary>
        public string? CorreoGuardaValores { get; set; }

        public string? LigaAgencia { get; set; }
        public string? Region { get; set; }
    }
}
