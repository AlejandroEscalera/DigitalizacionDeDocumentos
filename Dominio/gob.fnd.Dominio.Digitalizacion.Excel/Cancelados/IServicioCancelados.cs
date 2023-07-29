using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.Cancelados
{
    public interface IServicioCancelados
    {
        /// <summary>
        /// Obtiene una lista de los creditos Cancelados
        /// </summary>
        /// <returns>Regresa la lista de los créditos cancelados</returns>
        IEnumerable<CreditosCancelados> GetCreditosCancelados(string archivo = "");

        /// <summary>
        /// Guarda la información de los créditos que fueron cancelados (despues del cruce)
        /// </summary>
        /// <returns>regresa si pudo o no guardar la información</returns>
        bool GuardaCreditosCancelados(IEnumerable<CreditosCancelados> listaCreditosCancelados);
        /// <summary>
        /// Obtiene una lista de los creditos Cancelados
        /// </summary>
        /// <returns>Regresa la lista de los créditos cancelados</returns>
        Task<IEnumerable<CreditosCancelados>> GetCreditosCanceladosAsync(string archivo = "");
    }
}
