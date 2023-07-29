using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Juridico
{
    public class ExpedientesJuridicoRegion
    {
        public int Region { get; set; }
        public string? CatRegion { get; set; }
        public int CantContratos { get; set; }
        public int CantClientes { get; set; }
        public int CantCreditosHomologados { get; set; }
        public int CantDesfavorable { get; set; }
        public int CantFavorable { get; set; }
        public int CantNoFavorable { get; set; }
        public int CantReservado { get; set; }
        public decimal TotalCapitalDesfavorable { get; set; }
        public decimal TotalCapitalFavorable { get; set; }
        public decimal TotalCapitalNoFavorable { get; set; }
        public decimal TotalCapitalReservado { get; set; }
        public decimal TotalCapitalDemandado { get; set; }
    }
}
