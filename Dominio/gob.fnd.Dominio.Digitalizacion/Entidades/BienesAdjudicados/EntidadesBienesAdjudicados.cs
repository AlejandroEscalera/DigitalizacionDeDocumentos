using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class EntidadesBienesAdjudicados
    {
        public int NumRegion { get; set; }
        public string? CatEntidad { get; set; }
        public int CantidadDeBienes { get; set; }
        public int CantidadBienesMuebles { get; set; }
        public int CantidadBienesInmuebles { get; set; }
        public int CantidadClientes { get; set; }
        public int TipoAdjudicacionJudicial { get; set; }
        public int TipoDacionDePago { get; set; }
        public int AreaResponsableBaja { get; set; }
        public int AreaResponsableIndep { get; set; }
        public int AreaResponsableIndepVenta { get; set; }
        public int AreaResponsableJuridico { get; set; }
        public int EnIntegracionDeExpediente { get; set; }

    }
}
