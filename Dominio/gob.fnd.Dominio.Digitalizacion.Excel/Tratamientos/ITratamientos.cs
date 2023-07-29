using gob.fnd.Dominio.Digitalizacion.Entidades.Tratamientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Tratamientos
{
    /// <summary>
    /// Servicio de Tratamientos
    /// </summary>
    public interface ITratamientos
    {
        /// <summary>
        /// Obtiene la lista de los tratamientos y los cancelados
        /// </summary>
        /// <returns>Lista de tratamientos y cancelados</returns>
        Task<IEnumerable<gob.fnd.Dominio.Digitalizacion.Entidades.TratamientoYCancelado.Tratamientos>> GetTratamientos(string archivoTratamientos);

        /// <summary>
        /// Carga la base para hacer la conversión de un tratamiento a su número de crédito origen
        /// </summary>
        /// <param name="archivoConversionTratamiento">Archivo que se cargará de excel</param>
        /// <returns>Arreglo con las conversiones de Tratamientos</returns>
        IEnumerable<ConversionTratamientos> GetConversionTratamientos(string archivoConversionTratamiento);
    }
}
