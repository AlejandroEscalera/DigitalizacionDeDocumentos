using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos
{
    public class ABSaldosCuentaCarga
    {
        public int Coordinacion { get; set; }
        public int Agencia { get; set; }
        public string? Moneda { get; set; }
        public string? NumCredito { get; set; }
        public string? NumProducto { get; set; }
        public string? StatusCred { get; set; }
        public string? StatusCont { get; set; }
        public decimal CapVig { get; set; }
        public string? CtaCapVig { get; set; }
        public decimal CapVen { get; set; }
        public string? CtaCapVen { get; set; }
        public decimal IntFinVig { get; set; }
        public string? CtaIntFinVig { get; set; }
        public decimal IntFinVigNp { get; set; }
        public string? CtaIntFinVigNp { get; set; }
        public decimal IntFinVen { get; set; }
        public string? CtaIntFinVen { get; set; }
        public decimal IntFinVenNp { get; set; }
        public string? CtaIntFinVenNp { get; set; }
        public decimal IntNorVig { get; set; }
        public string? CtaIntNorVig { get; set; }
        public decimal IntNorVigNp { get; set; }
        public string? CtaIntNorVigNp { get; set; }
        public decimal IntNorVen { get; set; }
        public string? CtaIntNorVen { get; set; }
        public decimal IntNorVenNp { get; set; }
        public string? CtaIntNorVenNp { get; set; }
        public decimal IntDesVen { get; set; }
        public string? CtaIntDesVen { get; set; }
        public decimal IntPen { get; set; }
        public string? CtaIntPen { get; set; }
        public int? Sector { get; set; }

    }
}
