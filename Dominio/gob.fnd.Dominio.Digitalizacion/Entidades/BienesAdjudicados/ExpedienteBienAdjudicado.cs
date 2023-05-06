using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados
{
    public class EtapaBienAdjudicado 
    {
        public int Id { get; set; }
        public string? DescripcionEtapa { get; set; }
        public bool TieneImagenes { get; set; }
        public IEnumerable<ArchivoImagenBienesAdjudicadosCorta> Imagenes { get; set; }
        public EtapaBienAdjudicado()
        {
            Imagenes = new List<ArchivoImagenBienesAdjudicadosCorta>();
        }
    }

    public class ExpedienteBienAdjudicado
    {
        public DetalleBienesAdjudicados Expediente { get; set; }
        public IEnumerable<EtapaBienAdjudicado> Etapas { get; set; }

        public ExpedienteBienAdjudicado()
        {
            Expediente = new();
            IList<EtapaBienAdjudicado> etapas = new List<EtapaBienAdjudicado>
            {
                new EtapaBienAdjudicado()
                {
                    Id = 1,
                    DescripcionEtapa = "1. Identificación del caso",
                    TieneImagenes = false
                },
                new EtapaBienAdjudicado()
                {
                    Id = 2,
                    DescripcionEtapa = "2. Registro contable",
                    TieneImagenes = false
                },
                new EtapaBienAdjudicado()
                {
                    Id = 3,
                    DescripcionEtapa = "3. Administración de solicitud(es) de recursos",
                    TieneImagenes = false
                },
                new EtapaBienAdjudicado()
                {
                    Id = 4,
                    DescripcionEtapa = "4. Documentación para dar destino a los Bienes",
                    TieneImagenes = false
                },
                new EtapaBienAdjudicado()
                {
                    Id = 5,
                    DescripcionEtapa = "5. Consultas a jurídico y toma material del Bien",
                    TieneImagenes = false
                },
                new EtapaBienAdjudicado()
                {
                    Id = 6,
                    DescripcionEtapa = "6. Destino",
                    TieneImagenes = false
                }
            };

            Etapas = etapas;
        }
    }
}
