using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Directorios;

public interface IExploraCarpetas
{
    IEnumerable<ArchivosArqueos> ObtieneListaArchivosDeArqueos();
}