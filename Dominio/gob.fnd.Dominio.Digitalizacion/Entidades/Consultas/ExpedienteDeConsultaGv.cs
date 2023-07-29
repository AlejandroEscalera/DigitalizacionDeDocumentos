using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas
{
    public class ExpedienteDeConsultaGv : ExpedienteDeConsulta
    {
        public bool TieneGuardaValor { get; set; }
        public ExpedienteDeConsultaGv(ExpedienteDeConsulta origen)
        {
            this.NumCreditoCancelado = origen.NumCreditoCancelado;
            this.EsSaldosActivo = origen.EsSaldosActivo;
            this.EsCancelado = origen.EsCancelado;
            this.EsOrigenDelDr = origen.EsOrigenDelDr;
            this.EsCanceladoDelDr = origen.EsCanceladoDelDr;
            this.EsCastigado = origen.EsCastigado;
            this.TieneArqueo = origen.TieneArqueo;
            this.Acreditado = origen.Acreditado;
            this.FechaApertura = origen.FechaApertura;
            this.FechaCancelacion = origen.FechaCancelacion;
            this.Castigo = origen.Castigo;
            this.NumProducto = origen.NumProducto;
            this.CatProducto = origen.CatProducto;
            this.TipoDeCredito = origen.TipoDeCredito;
            this.Ejecutivo = origen.Ejecutivo;
            this.Analista = origen.Analista;
            this.FechaInicioMinistracion = origen.FechaInicioMinistracion;
            this.FechaSolicitud = origen.FechaSolicitud;
            this.MontoCredito = origen.MontoCredito;
            this.InterCont = origen.InterCont;
            this.Region = origen.Region;
            this.Agencia = origen.Agencia;
            this.CatRegion = origen.CatRegion;
            this.CatAgencia = origen.CatAgencia;
            this.EsCreditoAReportar = origen.EsCreditoAReportar;
            this.StatusImpago = origen.StatusImpago;
            this.StatusCarteraVencida = origen.StatusCarteraVencida;
            this.StatusCarteraVigente = origen.StatusCarteraVigente;
            this.TieneImagenDirecta = origen.TieneImagenDirecta;
            this.TieneImagenIndirecta = origen.TieneImagenIndirecta;
            this.SldoTotContval = origen.SldoTotContval;
            this.NumCliente = origen.NumCliente;
            this.NumCredito = origen.NumCredito;
    }
        public ExpedienteDeConsultaGv()
        { 
        
        }
    }
}
