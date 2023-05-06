using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class FuenteBienesAdjudicados
    {
        public int Id { get; set; }
        public string? CveBien { get; set; }
        public string? ExpedienteElectronico { get; set; }
        public string? CoordinacionRegional { get; set; }
        public string? Acreditado { get; set; }
        public string? TipoDeAdjudicacion { get; set; }
        public string? TipoDeBien { get; set; }
        public string? DescripcionReducidaBien { get; set; }
        public string? DescripcionCompletaBien { get; set; }
        public string? EntidadFederativa { get; set; }
        public string? Municipio { get; set; }
        public string? NumeroCliente { get; set; }
        public string? OficioNotificaiconGCRJ { get; set; }
        public string? NumContrato { get; set; }
        public string? NumCredito { get; set; }
        public DateTime? FechaFirmezaAdjudicacion { get; set; }
        public decimal ImporteIndivAdjudicacion { get; set; }
        public decimal ImporteIndivImplicado { get; set; }
        public DateTime? FechaComunicadoGOT { get; set; }
        public string? CuentaConEscritura { get; set; }
        public string? CuentaConEscrituraFisica { get; set; }
        public string? CuentaConLibertadGravamen { get; set; }
        public string? DocumentoDeJuridicoDeInexistencia { get; set; }
        public string? CuentaConProcesosJudiciales { get; set; }
        public string? Factura { get; set; }
        public string? TomaMaterial { get; set; }
        public string? Fotografias { get; set; }
        public string? NoAdeudo { get; set; }
        public string? DocumentoJuridicoInexistenciaProcesosJudiciales { get; set; }
        public string? CuentaConProcesosJudiciales2 { get; set; }
        public DateTime? FechaOficioSolicitudTransferenciaAlIndep { get; set; }
        public DateTime? FechaVenta { get; set; }
        public Decimal ImporteDeVenta { get; set; }
        public DateTime? FechaPolizaDeBajaContable { get; set; }
        public string? AreaResponsable { get; set; }
        public string? Estatus { get; set; }
        public string? ComentarioDelEstatusDelBien { get; set; }


    }
}
