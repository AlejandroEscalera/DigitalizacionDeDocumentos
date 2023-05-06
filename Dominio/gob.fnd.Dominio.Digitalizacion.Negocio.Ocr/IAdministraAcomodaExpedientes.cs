using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Ocr
{
    public interface IAdministraAcomodaExpedientes
    {
        bool ProcesaHiloYTrabajo(int threadId, int job);
    }
}
