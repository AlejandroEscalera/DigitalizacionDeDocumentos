using ConsultaDeImagenes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaDeImagenes.Services;

public class MyDataService
{
    private readonly List<MyRecord> records;

    public MyDataService()
    {
        // Inicializa los registros con algunos datos de ejemplo
        records = new List<MyRecord>
        {
            new MyRecord { Id = 1, Name = "Registro 1", Description = "Descripción del registro 1." },
            new MyRecord { Id = 2, Name = "Registro 2", Description = "Descripción del registro 2." },
            new MyRecord { Id = 3, Name = "Registro 3", Description = "Descripción del registro 3." },
            new MyRecord { Id = 4, Name = "Registro 4", Description = "Descripción del registro 4." }
        };
    }

    public List<MyRecord> Search(string searchKey)
    {
        // Busca los registros que contengan la clave de búsqueda en su nombre
        return records.Where(r => r.Name.Contains(searchKey)).ToList();
    }

    public MyRecord GetRecordById(int recordId)
    {
        // Busca el registro por su ID
        var resultado = records.FirstOrDefault(r => r.Id == recordId);
        return resultado;
    }
}