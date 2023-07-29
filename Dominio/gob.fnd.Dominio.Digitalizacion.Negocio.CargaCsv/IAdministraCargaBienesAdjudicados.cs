using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv
{
    public interface IAdministraCargaBienesAdjudicados
    {
        /// <summary>
        /// Cargo desde un archivo csv los bienes adjudicados
        /// </summary>
        /// <param name="archivoBienesAdjudicados">Nombre del archivo de Bienes adjudicados, en caso de blanco sacar
        /// del archivo de configuración</param>
        /// <returns></returns>
        IEnumerable<DetalleBienesAdjudicados> CargaBienesAdjudicados(string archivoBienesAdjudicados = "");
    }
}
