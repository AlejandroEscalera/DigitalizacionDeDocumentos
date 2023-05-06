using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.GuardaValores
{
    public interface IAdministraGuardaValores
    {
        /// <summary>
        /// Obtiene la información de los GuardaValores de todos los archivos de arqueos que nos compartieron
        /// </summary>
        /// <param name="archivosYCampos">Información resultante del análisis de los campos de los GV</param>
        /// <returns></returns>
        IEnumerable<InformacionGuardaValor> CargaInformacionGuardaValores(IEnumerable<ArchivoAnalisisCamposArqueos> archivosYCampos);
        /// <summary>
        /// Guarda la Información de los Guarda Valores
        /// </summary>
        /// <param name="guardaValores">Arreglo de Guarda Valores, puede estar concentrado o solo una agencia</param>
        /// <param name="fileName">Nombre de la agencia a Guardar o el archivo concentrado global</param>
        /// <returns>Si lo pudo guardar o no</returns>
        bool GuardaInformacionGuardaValores(IEnumerable<InformacionGuardaValor> guardaValores, string archivoGuardaValor = "");
        /// <summary>
        /// Carga información de los guardavalores desde un archivo
        /// </summary>
        /// <param name="archivoGuardaValor">Nombre del archivo</param>
        /// <returns>Los guardavalores previos</returns>
        IEnumerable<InformacionGuardaValor> CargaInformacionGuardaValoresPrevia(string archivoGuardaValor = "");
    }
}
