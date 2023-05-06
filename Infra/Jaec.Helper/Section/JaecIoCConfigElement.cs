using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jaec.Helper.Section;

public class JaecIoCConfigElement
{
    private string _assemblyName;
    private string _moduleClass;
    private string _fileName;

    public JaecIoCConfigElement()
    {
        _assemblyName = string.Empty;
        _moduleClass = string.Empty;
        _fileName = string.Empty;
    }
    public JaecIoCConfigElement(string assemblyName, string moduleClass, string fileName)
    {
        _assemblyName = assemblyName;
        _moduleClass = moduleClass;
        _fileName = fileName;
    }

    // [ConfigurationProperty("AssemblyName", DefaultValue = "Infraestructure.Data", IsRequired = true, IsKey = true)]
    public string AssemblyName
    {
        get { return _assemblyName; }
        set { _assemblyName = value; }
    }

    // [ConfigurationProperty("ModuleClass", DefaultValue = "Infraestructure.Data.IoCModule", IsRequired = true, IsKey = false)]
    public string ModuleClass
    {
        get { return _moduleClass; }
        set { _moduleClass = value; }
    }


    // [ConfigurationProperty("FileName", DefaultValue = "Infraestructure.Data.dll", IsRequired = true, IsKey = false)]
    public string FileName
    {
        get { return _fileName; }
        set { _fileName = value; }
    }

}
