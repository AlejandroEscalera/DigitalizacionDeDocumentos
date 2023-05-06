using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos
{
    public class ABSaldosFiltroCreditoDiario
    {
        public int Regional { get; set; }
        public string? NumCte { get; set; }
        public int Sucursal { get; set; }
        public string? ApellPaterno { get; set; }
        public string? ApellMaterno { get; set; }
        public string? Nombre1 { get; set; }
        public string? Nombre2 { get; set; }
        public string? RazonSocial { get; set; }
        public string? EstadoInegi { get; set; }
        public string? NumCredito { get; set; }
        public string? NumProducto { get; set; }
        public string? TipoCartera { get; set; }
        public DateTime FechaApertura { get; set; } // 
        public DateTime FechaVencim { get; set; } //
        public string? StatusContable { get; set; }
        public decimal SldoTotContval { get; set; }
        public string? NumContrato { get; set; }
        public DateTime? FechaSuscripcion { get; set; } //
        public DateTime? FechaVencCont { get; set; } //
        public string? TipoCreditoCont { get; set; }
        public bool StatusImpago { get; set; }
        public bool StatusCarteraVencida { get; set; }
        public bool StatusCarteraVigente { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
    }
}
