using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class DetalleBienesAdjudicados
    {
        public int NumRegion { get; set; }
        public string? CatRegion { get; set; }
        public string? Entidad { get; set; }
        public string? CveBien { get; set; }
        public string? ExpedienteElectronico { get; set; }
        public string? Acreditado { get; set; }
        public string? TipoDeBien { get; set; }
        public string? DescripcionReducidaBien { get; set; }
        public string? DescripcionCompletaBien { get; set; }
        public string? OficioNotificaiconGCRJ { get; set; }
        public string? NumCredito { get; set; }
        public string? TipoAdjudicacion { get; set; }
        public string? AreaResponsable { get; set; }
        public decimal ImporteIndivAdjudicacion { get; set; }
        public decimal ImporteIndivImplicado { get; set; }
        public string? Estatus { get; set; }
    }
}
