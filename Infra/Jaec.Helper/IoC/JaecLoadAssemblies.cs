using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jaec.Helper.Section;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Jaec.Helper.Assemblies;

namespace Jaec.Helper.IoC;

public static class JaecLoadAssemblies
{
    public static void LoadAllDinamicAssemblies(this IServiceCollection services, IConfiguration config, ILogger logger)
    {
        #region Obtengo la información de que librerías cargar desde el archivo de configuración
        logger.LogTrace("Obteniendo una sección de configuración");        
        var assembliesToLoad = config.GetSection("Jaec:IoC").Get<List<JaecIoCConfigElement>>();  
        logger.LogTrace("Se obtuvo una lista de objetos");
        #endregion        

        if (assembliesToLoad!= null)
            foreach (JaecIoCConfigElement assemblyPath in assembliesToLoad)
            {
                #region Cargo la libreria indicada en la configuración
                logger.LogTrace("Assembly a cargar {assemblyName}", assemblyPath.AssemblyName);
                string logDirectory = string.Empty;
                Assembly assemblyMain = Assembly.GetExecutingAssembly();
                if (assemblyMain is not null)
                    logDirectory = Path.GetDirectoryName(assemblyMain.Location) ?? "";
                //if (!File.Exists(assemblyPath.FileName))
                assemblyPath.FileName = assemblyPath.FileName.Replace(".\\", logDirectory + "\\");
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath.FileName);
                logger.LogTrace("Ensamblado cargado {fileName}", assemblyPath.FileName);            
                #endregion

                #region Si no hay un contenedor, no le solicito el registro de clases
                if (string.IsNullOrEmpty(assemblyPath.ModuleClass))
                {
                    continue;
                }
                #endregion

                CargaOtrasDependencias(logger, assembly);

                CreaContenedorCuandoAplique(services, logger, assemblyPath, assembly);

            }
    }

    private static void CreaContenedorCuandoAplique(IServiceCollection services, ILogger logger, JaecIoCConfigElement assemblyPath, Assembly assembly) {
        #region Cargo el tipo del Contaniner
        Type? tipoCarga = assembly.GetTypes().Select(x => x).FirstOrDefault(t => (t.FullName ?? "").Contains(assemblyPath.ModuleClass)) ?? throw new Exception(String.Format("No se encontró el Módulo {0}", assemblyPath.ModuleClass));
        #endregion

        #region creo el contenedor
        if (Activator.CreateInstance(tipoCarga, false) is IJaecContainer obj)
        {
            // Se cargan las nuevas dependencias (DI)
            obj.Load(services);
            logger.LogTrace("Módulo cargado {modulo}", assemblyPath.ModuleClass);
        }
        else
            throw new Exception(string.Format("No se pudo instanciar el Módulo {0}", assemblyPath.ModuleClass));
        #endregion
    }

    private static void CargaOtrasDependencias(ILogger logger, Assembly ultimaLibreriaCargada)
    {
        logger.LogTrace("Cargando otras dependencias...");
        var libreriasCargadas = System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies;      
        string directorioLibreria = Path.GetDirectoryName(ultimaLibreriaCargada.Location ?? "")??"";
        var otrasLibrerias = Directory.GetFiles(directorioLibreria, "*"+Path.GetExtension(ultimaLibreriaCargada.Location));
        var nombresLibreriasCargadas = libreriasCargadas.Select(x => (x.GetRealAssemblyName()));
        foreach (var otraLibreria in otrasLibrerias)
        {
            if (nombresLibreriasCargadas != null)
            {
                string nombreLibreria = Path.GetFileName(otraLibreria).Replace(Path.GetExtension(otraLibreria), "")??"";
                if (!nombresLibreriasCargadas.Any(x => (x ?? "").Equals(nombreLibreria, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(directorioLibreria, otraLibreria));
                        logger.LogTrace("Se cargo la librería {library}", otraLibreria);
                    }
                    catch {
                        logger.LogError("No se pudo cargar la librería {library}", otraLibreria);
                    }
                }
            }
        }
    }
}