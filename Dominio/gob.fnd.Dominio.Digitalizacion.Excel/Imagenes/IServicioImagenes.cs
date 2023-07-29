using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Imagenes
{
    public interface IServicioImagenes
    {
        /// <summary>
        /// Obtiene una lista de nombres de los archivos de expedientes que se van a procesar
        /// 
        /// Existen dos carpetas, una llamada imgs y otra llamada Exp2023
        /// La carpeta img contiene archivos de excel con las imagenes pasadas
        /// La carpeta Exp2023 contiene archivos de excel con los expedientes digitales actuales
        /// </summary>
        /// <param name="expedientes">Si son imagenes o son expedientes</param>
        /// <returns>lista con el nombre de los archivos, dependiendo la carpeta</returns>
        IEnumerable<string> ObtieneLosArchivosDeImagenes(bool expedientes = false);
        /// <summary>
        /// Obtengo tods los archivos de imagenes, si ya tengo una carga previa, le sumo la información anterior
        /// </summary>
        /// <param name="expedientes">Si lo que voy a cargar es un archivo de expedientes</param>
        /// <returns>Lista de archivos de imagenes</returns>
        IEnumerable<ArchivosImagenes> ObtieneInformacionDeTodosLosArchivosDeImagen(IEnumerable<ArchivosImagenes>? anteriores = null, bool expedientes = false);
        /// <summary>
        /// Obtengo la lista de los archivos de imagenes desde un archivo de excel en particular
        /// en caso de no existir el archivo de excel, no regreso registros
        /// </summary>
        /// <param name="archivoExcelOrigen">Nombre del archivo de excel</param>
        /// <param name="esExpediente">Para conocer si el tratamiento es el nuevo formato de digitalización de expedientes</param>
        /// <returns></returns>
        IEnumerable<ArchivosImagenes> GetArchivosImagenes(string archivoExcelOrigen, bool esExpediente = false);
        /// <summary>
        /// Comparo los expedientes, y los asocio con las imagenes, y pongo las banderas para saber si hay un castigo, 
        /// o si pertenecen al período del doctor, en AB saldos pongo la bandera de que fue encontrado el expediente digital
        /// </summary>
        /// <param name="saldosCorporativo">Información sobre la cual voy a buscar la información de expedientes</param>
        /// <param name="imagenesARevisar">Lista de imagenes con las que se realizará la asociación</param>
        /// <returns></returns>
        IEnumerable<ArchivosImagenes> ObtengoFiltrosABSaldosCorporativo(IEnumerable<ABSaldosConCastigo> saldosCorporativo, IEnumerable<ArchivosImagenes> imagenesARevisar);
        /// <summary>
        /// Guardo las imagenes en un excel para su revisión
        /// </summary>
        /// <param name="imagenes">listado de imagenes por guardar en un archivo de excel</param>
        /// <param name="archivoSalida">Nombre del archivo de salida a guardar, se le agrego para guardar los controles de las imagenes en los procesos de tratamiento</param>
        /// <returns>si pudo o no guardar la información</returns>
        bool GuardarImagenes(IEnumerable<ArchivosImagenes> imagenes, string archivoSalida = "");
        /// <summary>
        /// Se renumeran los archivos para poder llevar un control de avance correctamente
        /// </summary>
        /// <param name="archivos">lista de archivos de imágenes</param>
        /// <returns>Archivo de imágenes renumerado</returns>
        IEnumerable<ArchivosImagenes> RenumeraImagenes(IEnumerable<ArchivosImagenes> archivos);

        /// <summary>
        /// Permite cargar los registros de las imagenes que hayan sido procesadas
        /// </summary>
        /// <param name="archivo">NombreDelArchivoProcesado</param>
        /// <returns>listado de imágenes</returns>
        IEnumerable<ArchivosImagenes> CargaImagenesTratadas(string archivo = "");
        /// <summary>
        /// Obtiene el nombre ya codificado de un archivo
        /// </summary>
        /// <param name="nombreOrigen">Nombre de origen</param>
        /// <param name="nombreActual">Nombre de origen</param>
        /// <returns>Informacion del Nombre del Archivo Analizado</returns>
        AnalisisNombreImagen ObtieneNombreCalificado(string nombreOrigen);

        /// <summary>
        /// Permite cargar los registros de las imagenes que hayan sido procesadas
        /// </summary>
        /// <param name="archivo">NombreDelArchivoProcesado</param>
        /// <returns>listado de imágenes</returns>

        Task<IEnumerable<ArchivosImagenes>> CargaImagenesTratadasAsync(string archivo = "");
        /// <summary>
        /// Guardo las imagenes en un excel para su revisión
        /// </summary>
        /// <param name="imagenes">listado de imagenes por guardar en un archivo de excel</param>
        /// <param name="archivoSalida">Nombre del archivo de salida a guardar, se le agrego para guardar los controles de las imagenes en los procesos de tratamiento</param>
        /// <returns>si pudo o no guardar la información</returns>
        Task<bool> GuardarImagenesAsync(IEnumerable<ArchivosImagenes> imagenes, string archivoSalida = "");
    }
}
