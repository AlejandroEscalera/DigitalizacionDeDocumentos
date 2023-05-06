using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jaec.Helper.Assemblies;

public static class AssembliesExtensions
{
    public static Assembly? GetAssembly(this IEnumerable<Assembly> assemblies, string name)
    {
        return assemblies.Where(x => x.GetRealAssemblyName().Contains(name)).FirstOrDefault();
    }

    public static string GetRealAssemblyName(this Assembly? assembly) {
        if (assembly == null)
            return string.Empty;
        string name = assembly.GetName().Name??"";
        return name;
    }
}
