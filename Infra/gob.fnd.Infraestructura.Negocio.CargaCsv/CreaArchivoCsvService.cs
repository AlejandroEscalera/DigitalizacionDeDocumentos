using gob.fnd.Dominio.Digitalizacion.Negocio.CargaCsv;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Negocio.CargaCsv
{
    public class CreaArchivoCsvService : ICreaArchivoCsv
    {
        public bool CreaArchivoCsv(string archivoDestino, IEnumerable<Object> detalleAGuardar, bool otroEncoding = true)
        {
            Encoding encoding = Encoding.UTF8;
            if (otroEncoding)
            {
                encoding = Encoding.GetEncoding("ISO-8859-1");
            }
            using var writer = new StreamWriter(archivoDestino, false, encoding);
            bool encabezado = false;
            PropertyInfo[]? propiedades = null;
            // Escribir los objetos en el archivo
           // int i = 0;
            foreach (var objeto in detalleAGuardar)
            {
                //i++;
                // Escribir la cabecera del archivo CSV
                if (!encabezado)
                {
                    if (objeto is not null)
                    {
                        Type tipo = objeto.GetType();
                        propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        writer.WriteLine(string.Join("|", propiedades.Select(p => p.Name)));
                        encabezado = true;
                    }
                }
                // Construir la línea de texto para el objeto utilizando reflexión
                StringBuilder linea = new();
                if (propiedades is not null)
                {
                    object? valor = null;
                    foreach (var propiedad in propiedades)
                    {
                        valor = propiedad.GetValue(objeto);
                        if (valor is not null && valor.GetType() == typeof(DateTime))
                        {
                            DateTime fecha = Convert.ToDateTime(valor);
                            linea.Append(fecha.ToShortDateString() ?? "").Append('|');
                        }
                        else
                        {
                            if (valor is not null)
                                linea.Append(valor?.ToString() ?? "").Append('|');
                            else
                                linea.Append('|');
                        }
                    }
                    // Escribir la línea de texto en el archivo
                    if (!string.IsNullOrEmpty(valor?.ToString() ?? ""))
                        writer.WriteLine(linea.ToString().TrimEnd('|'));
                    else
                    {
                        writer.WriteLine(linea.ToString()[..^1]);
                    }
                }
            }
            return true;
        }
    }
}
