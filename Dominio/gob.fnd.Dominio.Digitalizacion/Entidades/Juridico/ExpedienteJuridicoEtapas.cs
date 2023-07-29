using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Juridico
{
    public class ExpedienteJuridicoEtapas
    {
        public int Id { get; set; }
        public string? DescripcionEtapa { get; set; }
        public bool TieneImagenes { get; set; }
        public IEnumerable<ArchivoImagenExpedientesCorta> Imagenes { get; set; }

        public ExpedienteJuridicoEtapas()
        {
            Imagenes = new List<ArchivoImagenExpedientesCorta>();
        }
    }

    public class ExpedienteJuridicoParaImagenes
    {
        public ExpedienteJuridicoDetalle Expediente { get; set; }
        public IEnumerable<ExpedienteJuridicoEtapas> Etapas { get; set; }
        public ExpedienteJuridicoParaImagenes()
        {
            Expediente = new();
            IList<ExpedienteJuridicoEtapas> etapas = new List<ExpedienteJuridicoEtapas>
            {
                new ExpedienteJuridicoEtapas()
                {
                    Id = 1,
                    DescripcionEtapa = "Turno Cobranza",
                    TieneImagenes = false
                },
                new ExpedienteJuridicoEtapas()
                {
                    Id = 2,
                    DescripcionEtapa = "Turno Juridico",
                    TieneImagenes = false
                },

                new ExpedienteJuridicoEtapas()
                {
                    Id = 3,
                    DescripcionEtapa = "Asunto Demandado",
                    TieneImagenes = false
                },

                new ExpedienteJuridicoEtapas()
                {
                    Id = 4,
                    DescripcionEtapa = "Expediente",
                    TieneImagenes = false
                }



            };
            Etapas = etapas;
        }
    }
}
