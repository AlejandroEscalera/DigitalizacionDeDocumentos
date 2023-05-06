using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal
{
    public class CreditosCanceladosResumen
    {
        public int Region { get; set; }
        public string? CatRegion { get; set; }
        public int CantidadCreditos { get; set; }
        public int CantidadClientes { get; set; }
        public int CantidadPersonasFisicas { get; set; }
        public int CantidadPersonasMorales { get; set; }
        public int CantidadAnio2020 { get; set; }
        public int CantidadAnio2021 { get; set; }
        public int CantidadAnio2022 { get; set; }
        public int CantidadAnio2023 { get; set; }
        public int CantidadPrimerPiso { get; set; }
        public int CantidadSegundoPiso { get; set; }
        public int CantidadFira { get; set; }
        public int CantidadFondosMutuales { get; set; }
        public int CantidadReservasPreventivas { get; set; }
    }
}
