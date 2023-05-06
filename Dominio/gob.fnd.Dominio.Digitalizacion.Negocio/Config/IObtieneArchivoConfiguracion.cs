using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Config;

/// <summary>
/// Informacion de Archivo de Configuración
/// </summary>
public interface IObtieneArchivoConfiguracion
{
    /// <summary>
    /// Revisa si el Archivo de Configuración tiene una configuración coeherente
    /// </summary>
    /// <returns></returns>
    bool ExisteConfiguracion();
    /// <summary>
    /// Obtiene la carpeta en donde se obtendrá el archivo AB Saldos
    /// </summary>
    /// <returns>El nombre de la carpeta donde viene el AB Saldos</returns>
    string ObtieneCarpetaOrigen();
    /// <summary>
    /// Obtiene la carpeta donde quedará el ABSaldos
    /// </summary>
    /// <returns>El nombre de la carpeta donde quedará el ABSaldos</returns>
    string ObtieneCarpetaDestino();
    /// <summary>
    /// Valida si existe un archivo ZIP de AB Saldos en la carpeta
    /// </summary>
    /// <returns>Verdadero si existe el archivo, falso si no existe el archivo</returns>
    bool ExisteABSaldos();
    /// <summary>
    /// Obtiene el nombre del AB saldos inicial
    /// </summary>
    /// <returns>El nombre del archivo AB saldos que será procesado</returns>
    string ObtieneNombreArchivoABSaldos();
    /// <summary>
    /// Copia el archivo de la carpeta original a la carpeta de inicio del proceso
    /// </summary>
    /// <param name="archivoOrigen">Lugar de donde se tomará el archivo de orgien</param>
    /// <returns>Verdadero si pudo hacer el cambio de carpeta, falso en caso de no haber hecho la copia</returns>
    bool CopiaArchivoABSaldos(string archivoOrigen);
    /// <summary>
    /// Obtiene la Carpeta donde se guardarán las agencias
    /// </summary>
    /// <returns>El nombre de la carpeta de donde se obtendrán las agencias</returns>
    string ObtieneCarpetaDestinoAgencias();
    /// <summary>
    /// Crea el arbol del archivo de configuración a partir de donde se originaron las carpetas y 
    /// ajusta el archivo de configuración
    /// </summary>
    /// <returns>Verdadero si pudo crear las carpetas y si pudo hacer el cambio de configuracion, falso si hay un error durante este proceso</returns>
    bool CreaCarpetasConfiguracion(string directorioInicial);

}
