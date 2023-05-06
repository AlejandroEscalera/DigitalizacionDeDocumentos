using Jaec.Helper.Assemblies;
using Jaec.Helper.Section;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jaec.Helper.IoC
{
    public static class RunThisApp
    {

        public static Assembly? GetMainAssembly(this IConfiguration config)
        {
            JaecMainControlAppSetting mainAppName = config.GetMainSettings();
            Assembly? mainAssembly = System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies.GetAssembly(mainAppName.AssemblyName ?? "");
            return mainAssembly;
        }
        public static Type? GetMainType(this Assembly assembly, IConfiguration config)
        {
            JaecMainControlAppSetting mainAppName = config.GetMainSettings();
            Type? tipoCarga = assembly.GetTypes().Select(x => x).FirstOrDefault(t => (t.FullName ?? "").Contains(mainAppName.MainClass ?? ""));
            return tipoCarga;
        }
        public static Type? GetAuxiliarType(this Assembly assembly, IConfiguration config)
        {
            JaecMainControlAppSetting mainAppName = config.GetMainSettings();
            return assembly.GetAuxiliarType(mainAppName.AuxiliarType ?? "");
        }
        public static Type? GetAuxiliarType(this Assembly assembly, string auxiliarName)
        {
            Type? tipoCarga = assembly.GetTypes().Select(x => x).FirstOrDefault(t => (t.FullName ?? "").Contains(auxiliarName));
            return tipoCarga;
        }
        public static IMainControlApp? SvcRun(this IServiceProvider hostServices, IConfiguration config)
        {
            JaecMainControlAppSetting mainAppName = config.GetMainSettings();
            Assembly? mainAssembly = config.GetMainAssembly();
            if (mainAssembly != null)
            {

                Type? tipoCarga = mainAssembly.GetMainType(config);
                if (tipoCarga != null)
                {
                    IMainControlApp svc = (IMainControlApp)ActivatorUtilities.CreateInstance(hostServices, tipoCarga);
                    svc.Run();
                    return svc;
                }
                else
                {
                    throw new Exception(string.Format("No encontró la clase principal {0} para poder ejecutar la aplicación", mainAppName.MainClass));
                }
            }
            else
            {
                throw new Exception(string.Format("No encontró la librería principal {0} para poder crear la clase", mainAppName.AssemblyName));
            }
        }

        public static JaecMainControlAppSetting GetMainSettings(this IConfiguration config)
        {
            var section = config.GetSection("Jaec.IMainControlApp") ?? throw new Exception(string.Format("No existe la directiva Jaec.IMainControlApp dentro del archivo appSettings.json"));
            JaecMainControlAppSetting mainAppName = new()
            {
                AssemblyName = section.GetValue<string>("AssemblyName"),
                MainClass = section.GetValue<string>("MainClass"),
                AuxiliarType = section.GetValue<string>("AuxiliarType")
            };
            return mainAppName;
        }
    }
}