using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx
{
    public interface IServicioDirToXls
    {
        IEnumerable<ArchivosImagenes> ObtieneImagenesDesdeDirectorio(string directorio = "", int consecutivo = 0);
        IEnumerable<ArchivosImagenes> ObtieneImagenesComplementarias(string directorio = "", int consecutivo = 0);
        IEnumerable<ArchivosImagenes> ObtieneImagenesComplementariasSegundaParte(string directorio = "", int consecutivo = 0);
        IEnumerable<ArchivosImagenes> ObtieneImagenesComplementariasTerceraParte(string directorio = "", int consecutivo = 0);
        IEnumerable<ArchivosImagenes> ObtieneSoloDiferencias(IEnumerable<ArchivosImagenes> inicial, IEnumerable<ArchivosImagenes> nuevo);
        IEnumerable<ArchivosImagenes> ObtieneBienasAdjudicados(string directorio = "", int consecutivo = 0);
        IEnumerable<ArchivosImagenes> ObtieneImagenesCreditosLiquidados(string directorio = "", int consecutivo = 0);
    }
}
