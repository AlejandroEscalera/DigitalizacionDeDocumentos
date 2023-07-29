using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.BienesAdjudicados
{
    /// <summary>
    /// Me permite hacer los cruces de diferentes fuentes para 
    /// consolidar la información de los bienes adjudicados
    /// </summary>
    public interface IAnalizaInformacionBienesAdjudicados
    {
        /// <summary>
        /// Aqui sería el proceso principal que orquesta todos los cruces para obtener
        /// el consolidado de toda la información de los bienes adjudicados
        /// </summary>
        void RealizaAnalisisBienesAdjudicados();

        /// <summary>
        /// Aqui se realiza el cruce entre la base de datos de Recursos Materiales
        /// y las carpetas físicas con todos los archivos de los expedientes
        /// 
        /// La idea es asignar una carpeta de expediente por cada registro 
        /// </summary>
        /// <param name="archivoBienesAdjudicados">Archivo base de donde sale la información de los bienes adjudicados</param>
        /// <param name="archivoImagenesBienesAdjudicados">Archivo donde se vaciaron los pdf's de las carpetas de los expedientes</param>
        /// <returns>Lista de cruce entre expedientes y bienes adjudicados</returns>
        IEnumerable<CruceFuenteBAvsExpedientes> RealizaCruceEntreBaseBienesAdjudicadosYExpedientes(string archivoBienesAdjudicados = "", string archivoImagenesBienesAdjudicados = "");

        /// <summary>
        /// Aqui se realiza el cruce entre riesgos y la base de datos de Recursos Materiales
        /// Se cruza la informción entre las claves de bien, hay menos claves del lado de riesgos
        /// </summary>
        /// <param name="expedientes">Expedientes con los que se realizará el cruce</param>
        /// <param name="archivoBienesAdjudicadosIdentificados">Archivo de donde se obtendrá la información</param>
        /// <returns>Bienes Identificados en cruce con Bienes Identificados vs Expedientes</returns>
        IEnumerable<CruceFuenteVsIdentificacionClaveBien> RealizaCruceEntreBienesIdentificadosYExpedientes(IEnumerable<CruceFuenteBAvsExpedientes> expedientes, string archivoBienesAdjudicadosIdentificados = "");
    }
}
