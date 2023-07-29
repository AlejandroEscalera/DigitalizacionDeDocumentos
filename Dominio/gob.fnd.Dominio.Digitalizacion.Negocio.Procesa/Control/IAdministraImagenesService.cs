using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control.Descarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Procesa.Control
{
    public interface IAdministraImagenesService
    {
        bool RecolectaArchivosYCarpetasDeControl(int maxThreadId, int maxJobId, string control = "CTL");

        bool CreaArchivosYCarpetasDeControlInicial(IEnumerable<ArchivosImagenes> imagenes);
        bool RecolectaArchivosYCarpetasDeControl(IEnumerable<ArchivosImagenes> imagenes, string control="CTL");

        /// <summary>
        /// Procesa un archivo dependiendo el tipo de archivo
        /// 
        /// Se puede llamar recursivamente cuando es un archivo .zip o .zip
        /// 
        /// Al final debe regresar uno o mas archivos resultados del proceso, cuyo numero de id es el mismo y el subarchivo con tantos subarchivos que sea necesario
        /// </summary>
        /// <param name="archivoAProcesar">Información del registro del archivo a procesar</param>
        /// <param name="subArchivo">La subvariante del archivo</param>
        /// <returns>Arreglo de uno o mas archivos procesados</returns>
        IEnumerable<ArchivosImagenes> ProcesaArchivo(ArchivosImagenes archivoAProcesar, ref int subArchivo);

        bool ProcesaHiloYTrabajo(int threadId, int job);

        IEnumerable<ArchivosImagenes> CargaSoloPdf(string archivo ="");


        bool CreaArchivosYCarpetasDeControlOcr(IEnumerable<ArchivosImagenes> imagenes);

        void ProcesaImagenesCargadas(IList<ArchivosImagenes> imagenesDescargadasYProcesadas, IList<ArchivosImagenes> recuperaImagenes);
        
        Task ProcesaImagenesCargadasAsync(IList<ArchivosImagenes> imagenesDescargadasYProcesadas, IList<ArchivosImagenes> recuperaImagenes, IProgress<ReporteProgresoDescompresionArchivos> progresa);
    }
}
