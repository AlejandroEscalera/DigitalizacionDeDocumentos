using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC
{

    /// <summary>
    /// Obtiene información del concentrado de información del corporativo
    /// </summary>
    public interface IServicioABSaldosConCastigo
    {

        /// <summary>
        /// Abre el archivo de excel y lo vacía a un arreglo de información relacionada con los saldos de los créditos activos en
        /// el período del Dr.
        /// </summary>
        /// <param name="archivoOrigen">Nombre del archivo de origen</param>
        /// <returns>Arreglo con la información de ABSaldosCorporativo</returns>
        IEnumerable<ABSaldosConCastigo> CargaProcesaYLimpia(string archivoOrigen); // , string archivoDestino
        /// <summary>
        /// Agrega los datos faltantes de los Agentes y los Guarda Valores a los nuevos registros de AB Saldos
        /// </summary>
        /// <param name="origen">Información de AB Saldos de forma recortada a los nuevos créditos</param>
        /// <param name="agentes">Información de los correos, agentes y guarda valores</param>
        /// <returns>Información subsanada</returns>
        IEnumerable<ABSaldosConCastigo> AgregaAgentesYGuardaValores(IEnumerable<ABSaldosConCastigo> origen, IEnumerable<CorreosAgencia> agentes);

        /// <summary>
        /// Guarda temporalmente la información de ABSaldos ya filtrada, complementada y depurada en un archivo de destino
        /// </summary>
        /// <param name="archivoDestino">Nombre del archivo donde se guardará la información</param>
        /// <param name="listadoCreditos">Arreglo con la información de AB Saldos que se guardará</param>
        /// <returns>Verdadero si pudo guardar la información</returns>
        bool GuardaInformacionABSaldos(string archivoDestino, IEnumerable<ABSaldosConCastigo> listadoCreditos); // , string archivoDestino

        IEnumerable<ABSaldosConCastigo> ObtieneABSaldosConCastigoProcesados();

        Task<IEnumerable<ABSaldosConCastigo>> ObtieneABSaldosConCastigoProcesadosAsync();
    }
}
