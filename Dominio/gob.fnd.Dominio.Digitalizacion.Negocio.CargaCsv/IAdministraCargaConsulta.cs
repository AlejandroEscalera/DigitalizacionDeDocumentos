using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCargaConsulta
    {
        /// <summary>
        /// Carga los expedientes que sirven de consulta en la aplicación de digitalización
        /// </summary>
        /// <param name="archivoDeExpedienteDeConsulta">nombre del archivo ya optimizado para la consulta</param>
        /// <returns>Lista de expedientes para consultar en la aplicación</returns>
        IEnumerable<ExpedienteDeConsulta> CargaExpedienteDeConsulta(string archivoDeExpedienteDeConsulta = "");
        /// <summary>
        /// Carga los expedientes que sirven de consulta en la aplicación de digitalización
        /// </summary>
        /// <param name="archivoDeExpedienteDeConsulta">nombre del archivo ya optimizado para la consulta</param>
        /// <returns>Lista de expedientes para consultar en la aplicación</returns>
        IEnumerable<ExpedienteDeConsultaGv> CargaExpedienteDeConsultaGv(string archivoDeExpedienteDeConsulta = "");
        /// <summary>
        /// Carga un archivo recortado de imagenes de expedientes para su consulta
        /// </summary>
        /// <param name="archivoArchivoImagenCorta">Es opcional, es el nombre del archivo de imagenes, en caso de no seleccionarlo viene del app.json</param>
        /// <returns>Lista de imágenes de los expedientes</returns>
        IEnumerable<ArchivoImagenCorta> CargaArchivoImagenCorta(string archivoArchivoImagenCorta = "");
        /// <summary>
        /// Carga un archivo con las imagenes de los bienes adjudicados para su consulta
        /// </summary>
        /// <param name="archivoImagenBienesAdjudicadosCorta">Es opcional, es el nombre del archivo con las imagenes de bienes adjudicados, ya recortadas</param>
        /// <returns>Lista de imágenes de los bienes adjudicados</returns>
        IEnumerable<ArchivoImagenBienesAdjudicadosCorta> CargaArchivoImagenBienesAdjudicadosCorta(string archivoImagenBienesAdjudicadosCorta = "");

        /// <summary>
        /// Carga un archivo con las imagenes de los expedientes para su consulta
        /// </summary>
        /// <param name="archivoImagenExpedientesCorta">Es opcional, es el nombre del archivo con las imagenes de los expedientes juridicos de cobranza, ya recortadas</param>
        /// <returns>Lista de imágenes de los expedientes</returns>
        IEnumerable<ArchivoImagenExpedientesCorta> CargaArchivoImagenExpedientesCortas(string archivoImagenExpedientesCorta = "");
    }
}
