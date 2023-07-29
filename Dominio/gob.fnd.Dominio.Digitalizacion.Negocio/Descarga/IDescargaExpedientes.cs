using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Descarga
{
    public interface IDescargaExpedientes
    {
        /// <summary>
        /// Descarga los expedientes encontratodos en el archivo de lista de contratos 
        /// y los incluye en una estructura dentro de la carpeta de destino
        /// </summary>
        /// <param name="nombreArchivoListaContratos">Lista de números de crédito o de contratos</param>
        /// <param name="carpetaDestino">Carpeta destino donde se gardará la información</param>
        /// <returns>falso en caso de que no se pudo descargar o encontrar al menos un expediente</returns>
        bool DescargaListaDeExpedientes(string nombreArchivoListaContratos, string carpetaDestino);
    }
}
