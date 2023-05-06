using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal
{
    public class CreditosCanceladosDetalle
    {
        public int Region { get; set; }
        public int Agencia { get; set; }
        public string? NumCreditoCancelado { get; set; }
        public string? NumCliente { get; set; }
        public string? Acreditado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public DateTime? FechaPrimeraDispersion { get; set; }
        public int PersonaFisica { get; set; }
        public int PersonaMoral { get; set; }
        public int Anio2020 { get; set; }
        public int Anio2021 { get; set; }
        public int Anio2022 { get; set; }
        public int Anio2023 { get; set; }
        public int PrimerPiso { get; set; }
        public int SegundoPiso { get; set; }
        public int Fira { get; set; }
        public int FondosMutuales { get; set; }
        public int ReservasPreventivas { get; set; }
        public bool TieneImagenDirecta { get; set; }
        public bool TieneImagenIndirecta { get; set; }
    }
}
