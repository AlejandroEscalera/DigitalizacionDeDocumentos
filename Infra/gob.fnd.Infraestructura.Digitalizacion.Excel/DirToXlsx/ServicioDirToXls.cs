using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Excel.DirToXlsx;
using gob.fnd.Infraestructura.Digitalizacion.Excel.HelperInterno;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Infraestructura.Digitalizacion.Excel.DirToXlsx;
public class ServicioDirToXls : IServicioDirToXls
{
    private readonly ILogger<ServicioDirToXls> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _archivoExcelImagenesRespaldo;
    private readonly string _carpetaRaiz;
    private readonly string _carpetaRaizPF1;
    private readonly string _carpetaRaizComplementos;
    private readonly bool _registraTodasLasImagenes;
    private readonly string _archivoComplementoImagenes;
    private readonly string _archivoComplementoImagenesSegundaParte;
    private readonly string _archivoOrigenImagenesBienesAdjudicados;

    public ServicioDirToXls(ILogger<ServicioDirToXls> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _archivoExcelImagenesRespaldo = _configuration.GetValue<string>("archivoRespaldoImagenes") ??"";
        _archivoComplementoImagenes = _configuration.GetValue<string>("archivoComplementoImagenes") ?? "";
        _archivoComplementoImagenesSegundaParte = _configuration.GetValue<string>("archivoComplementoImagenesSegundaParte") ?? "";
        _carpetaRaiz = "D:\\OD\\OneDrive - FND\\Respaldo 3";
        _carpetaRaizPF1 = "D:\\PF1";
        _carpetaRaizComplementos = "D:\\OD\\OneDrive - FND\\Documentos - Expedientes de Cartera de Crédito";
        _registraTodasLasImagenes = true;  (_configuration.GetValue<string>("registraTodasLasImagenes") ?? "").Equals("Si");
        _archivoOrigenImagenesBienesAdjudicados = _configuration.GetValue<string>("archivoOrigenImagenesBienesAdjudicados") ??"";
    }

    public IEnumerable<ArchivosImagenes> ObtieneImagenesComplementarias(string directorio = "", int consecutivo = 0)
    {
        IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        if (!File.Exists(_archivoComplementoImagenes))
        {
            _logger.LogError("No encontró el archivo maestro de la información del respaldo 3 {archivoRespaldo3}", _archivoComplementoImagenes);
            return resultado;
        }
        int threadId = 103;
        int job = 1;
        int item = 1;
        int id = consecutivo;
        FileStream fs = new(_archivoComplementoImagenes, FileMode.Open, FileAccess.Read);
        using var package = new ExcelPackage();
        package.Load(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.FirstOrDefault();
        if (hoja != null)
        {
            int renglon = 2;
            string celdaFullName = "Comienza";
            while (!string.IsNullOrEmpty(celdaFullName))
            {
                celdaFullName = hoja.Cells[renglon, 1].Text ?? "";
                if (!string.IsNullOrEmpty(celdaFullName))
                {
                    string rutaCorta = string.Empty;
                    if (celdaFullName.Contains(_carpetaRaizComplementos,StringComparison.InvariantCultureIgnoreCase))
                        rutaCorta = celdaFullName.Replace(_carpetaRaizComplementos, "");
                    else
                        rutaCorta = celdaFullName.Replace(_carpetaRaizPF1, "");
                    ArchivosImagenes archivo = new()
                    {
                        Id = id,
                        ArchivoOrigen = "Complementos 001",
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = Path.GetFileName(celdaFullName).Trim(),
                        RutaDeGuardado = (Path.GetDirectoryName(rutaCorta) ?? "").Trim(),
                        UsuarioDeModificacion = "",
                        NumCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(celdaFullName))),
                        UrlArchivo = celdaFullName,
                        Extension = new FileInfo(Path.GetFileName(celdaFullName).Trim()).Extension,
                        CarpetaDestino = "F:\\11\\COMP" + (Path.GetDirectoryName(rutaCorta) ?? "").Trim(),
                        ThreadId = threadId,
                        Job = job
                    };

                    #region Reemplazo el número de credito por el de la carpeta (si aplica)
                    if (Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO))
                    {
                        string[] directorios = (Path.GetDirectoryName(rutaCorta) ?? "").Split(Path.DirectorySeparatorChar);
                        if (directorios is not null && (directorios.Length > 0))
                        {
                            string posibleNumeroContrato = (directorios.LastOrDefault() ?? "");
                            if (!string.IsNullOrEmpty(posibleNumeroContrato))
                            {
                                archivo.NumCredito = Condiciones.ObtieneNumCredito(posibleNumeroContrato);
                                _logger.LogInformation("Expediente : {numCredito} {archivo}", archivo.NumCredito, archivo.NombreArchivo);
                            }
                        }
                    }
                    #endregion
                    /// Si aun así el número de crédito es nulo, no lo guardo
                    if ((!Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO))) // || _registraTodasLasImagenes
                    {
                        resultado.Add(archivo);
                        id++;
                    }
                    else
                        _logger.LogInformation("No se registra!! {ruta} {nombreArchivo}", archivo.UrlArchivo, archivo.NombreArchivo);
                }
                item++;
                if (item > 91)
                {
                    item = 1;
                    job++;
                    if (job > 89)
                    {
                        job = 1;
                        threadId++;
                    }
                }
                renglon++;
            }
        }
        _logger.LogInformation("Se encontraron {cantidadArchivos} en el complemento de imagenes", resultado.Count);
        return resultado.ToList(); // resultado.Where(x => !Condiciones.QuitaCastigo(x.NumCredito).Equals(Condiciones.C_STR_NULLO)).ToList();
            
    }

    public IEnumerable<ArchivosImagenes> ObtieneImagenesComplementariasSegundaParte(string archivoImagenesComplementarias = "", int consecutivo = 0)
    {
        IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        if (archivoImagenesComplementarias == "") {
            archivoImagenesComplementarias = _archivoComplementoImagenesSegundaParte;
        }
        if (!File.Exists(archivoImagenesComplementarias))
        {
            _logger.LogError("No encontró el archivo maestro de la información del respaldo 3 {archivoRespaldo3}", archivoImagenesComplementarias);
            return resultado;
        }
        int threadId = 104;
        int job = 1;
        int item = 1;
        int id = consecutivo;
        FileStream fs = new(archivoImagenesComplementarias, FileMode.Open, FileAccess.Read);
        using var package = new ExcelPackage();
        package.Load(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.FirstOrDefault();
        if (hoja != null)
        {
            int renglon = 2;
            string celdaFullName = "Comienza";
            while (!string.IsNullOrEmpty(celdaFullName))
            {
                celdaFullName = hoja.Cells[renglon, 1].Text ?? "";
                if (!string.IsNullOrEmpty(celdaFullName))
                {
                    string rutaCorta = string.Empty;
                    if (celdaFullName.Contains(_carpetaRaizComplementos, StringComparison.InvariantCultureIgnoreCase))
                        rutaCorta = celdaFullName.Replace(_carpetaRaizComplementos, "");
                    else
                        rutaCorta = celdaFullName.Replace(_carpetaRaizPF1, "");
                    ArchivosImagenes archivo = new()
                    {
                        Id = id,
                        ArchivoOrigen = "Complementos 001",
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = Path.GetFileName(celdaFullName).Trim(),
                        RutaDeGuardado = (Path.GetDirectoryName(rutaCorta) ?? "").Trim(),
                        UsuarioDeModificacion = "",
                        NumCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(celdaFullName))),
                        UrlArchivo = celdaFullName,
                        Extension = new FileInfo(Path.GetFileName(celdaFullName).Trim()).Extension,
                        CarpetaDestino = "F:\\11\\COMP" + (Path.GetDirectoryName(rutaCorta) ?? "").Trim(),
                        ThreadId = threadId,
                        Job = job
                    };

                    #region Reemplazo el número de credito por el de la carpeta (si aplica)
                    if (Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO))
                    {
                        string[] directorios = (Path.GetDirectoryName(rutaCorta) ?? "").Split(Path.DirectorySeparatorChar);
                        if (directorios is not null && (directorios.Length > 0))
                        {
                            string posibleNumeroContrato = (directorios.LastOrDefault() ?? "");
                            if (!string.IsNullOrEmpty(posibleNumeroContrato))
                            {
                                archivo.NumCredito = Condiciones.ObtieneNumCredito(posibleNumeroContrato);
                                _logger.LogInformation("Expediente : {numCredito} {archivo}", archivo.NumCredito, archivo.NombreArchivo);
                            }
                        }
                    }
                    #endregion
                    /// Si aun así el número de crédito es nulo, no lo guardo
                    if ((!Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO)) || _registraTodasLasImagenes)
                    {
                        resultado.Add(archivo);
                        id++;
                    }
                    else
                        _logger.LogInformation("No se registra!! {ruta} {nombreArchivo}", archivo.UrlArchivo, archivo.NombreArchivo);
                }
                item++;
                if (item > 91)
                {
                    item = 1;
                    job++;
                    if (job > 89)
                    {
                        job = 1;
                        threadId++;
                    }
                }
                renglon++;
            }
        }
        _logger.LogInformation("Se encontraron {cantidadArchivos} en el complemento de imagenes", resultado.Count);
        return resultado.ToList(); // resultado.Where(x => !Condiciones.QuitaCastigo(x.NumCredito).Equals(Condiciones.C_STR_NULLO)).ToList();

    }

    public IEnumerable<ArchivosImagenes> ObtieneImagenesDesdeDirectorio(string directorio = "", int consecutivo = 0)
    {
        IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        if (!File.Exists(_archivoExcelImagenesRespaldo))
        {
            _logger.LogError("No encontró el archivo maestro de la información del respaldo 3 {archivoRespaldo3}", _archivoExcelImagenesRespaldo);
            return resultado;
        }
        int id = 1;
        FileStream fs = new(_archivoExcelImagenesRespaldo, FileMode.Open, FileAccess.Read);
        using var package = new ExcelPackage();
        package.Load(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.FirstOrDefault();
        if (hoja != null)
        {
            int renglon = 2;
            string celdaFullName = "Comienza";
            while (!string.IsNullOrEmpty(celdaFullName))
            {
                celdaFullName = hoja.Cells[renglon, 1].Text ?? "";
                if (!string.IsNullOrEmpty(celdaFullName))
                {
                    string rutaCorta = celdaFullName.Replace(_carpetaRaiz, "");
                    ArchivosImagenes archivo = new()
                    {
                        Id = id,
                        ArchivoOrigen = "Directorio de Respaldo 3",
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = Path.GetFileName(celdaFullName).Trim(),
                        RutaDeGuardado = (Path.GetDirectoryName(rutaCorta)??"").Trim(),
                        UsuarioDeModificacion = "",
                        NumCredito = Condiciones.ObtieneNumCredito(Condiciones.LimpiaNoNumeros(Path.GetFileName(celdaFullName))),
                        UrlArchivo = celdaFullName,
                        Extension = new FileInfo(Path.GetFileName(celdaFullName).Trim()).Extension,
                        CarpetaDestino = "F:\\11\\OD"+ (Path.GetDirectoryName(rutaCorta) ?? "").Trim()
                    };

                    #region Reemplazo el número de credito por el de la carpeta (si aplica)
                    if (Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO))
                    {
                        string[] directorios = (Path.GetDirectoryName(rutaCorta)??"").Split(Path.DirectorySeparatorChar);
                        if (directorios is not null && (directorios.Length > 0))
                        {
                            string posibleNumeroContrato = (directorios.LastOrDefault() ?? "");
                            if (!string.IsNullOrEmpty(posibleNumeroContrato))
                            {
                                archivo.NumCredito = Condiciones.ObtieneNumCredito(posibleNumeroContrato);
                                _logger.LogInformation("Expediente : {numCredito} {archivo}", archivo.NumCredito, archivo.NombreArchivo);
                            }
                        }
                    }
                    #endregion
                    /// Si aun así el número de crédito es nulo, no lo guardo
                    if ((!Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO)) || _registraTodasLasImagenes)
                    {
                        resultado.Add(archivo);
                        id++;
                    }
                }
                renglon++;
            }
        }
        _logger.LogInformation("Se encontraron {cantidadArchivos} en el respaldo", resultado.Count);
        //return resultado.Where(x => !Condiciones.QuitaCastigo(x.NumCredito).Equals(Condiciones.C_STR_NULLO)).ToList();
        return resultado.Where(x => Condiciones.QuitaCastigo(x.NumCredito).Equals(Condiciones.C_STR_NULLO)).ToList();
    }

    public IEnumerable<ArchivosImagenes> ObtieneSoloDiferencias(IEnumerable<ArchivosImagenes> inicial, IEnumerable<ArchivosImagenes> nuevo)
    {
        var leftJoinResult2 = (from nvo in nuevo
                               join ini in inicial on nvo.NombreArchivo equals ini.NombreArchivo into t2Group
        from t2Result in t2Group.DefaultIfEmpty()
                               select new
                               {
                                   PorProcesar = nvo,
                                   Procesados = t2Result
                               }
                          ).ToList();

        // Elimino las que tienen ministración, esas ya las atendí
        leftJoinResult2 = leftJoinResult2.Where(x => x.Procesados is null).ToList();
        return leftJoinResult2.Select(x=>x.PorProcesar).ToList();
    }


    public IEnumerable<ArchivosImagenes> ObtieneBienasAdjudicados(string directorio = "", int consecutivo = 0)
    {
        // 
        IList<ArchivosImagenes> resultado = new List<ArchivosImagenes>();
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        if (string.IsNullOrEmpty(directorio))
        {
            directorio = _archivoOrigenImagenesBienesAdjudicados;
        }

        if (!File.Exists(directorio))
        {
            _logger.LogError("No encontró el archivo con los bienes adjudicados {bienesAdjudicados}", directorio);
            return resultado;
        }
        int id = 1;
        FileStream fs = new(directorio, FileMode.Open, FileAccess.Read);
        using var package = new ExcelPackage();
        package.Load(fs);
        ExcelWorksheet? hoja = package.Workbook.Worksheets.FirstOrDefault();
        if (hoja != null)
        {
            int renglon = 2;
            string celdaFullName = "Comienza";
            while (!string.IsNullOrEmpty(celdaFullName))
            {
                celdaFullName = hoja.Cells[renglon, 1].Text ?? "";
                if (!string.IsNullOrEmpty(celdaFullName))
                {
                    string rutaCorta = celdaFullName.Replace("F:\\14\\BA", "", StringComparison.InvariantCultureIgnoreCase);
                    ArchivosImagenes archivo = new()
                    {
                        Id = id,
                        ArchivoOrigen = "Bienes Adjudicados",
                        TamanioArchivo = 0,
                        FechaDeModificacion = DateTime.Now,
                        NombreArchivo = Path.GetFileName(celdaFullName).Trim(),
                        RutaDeGuardado = (Path.GetDirectoryName(rutaCorta) ?? "").Trim(),
                        UsuarioDeModificacion = "",
                        NumCredito = "00000000000000", // Condiciones.ObtieneExpedienteBienesAdjudicados(Condiciones.LimpiaNoNumeros(Path.GetFileName(celdaFullName)))+ "00000000",
                        UrlArchivo = celdaFullName,
                        Extension = new FileInfo(Path.GetFileName(celdaFullName).Trim()).Extension,
                        CarpetaDestino = "F:\\11\\BA" + (Path.GetDirectoryName(rutaCorta) ?? "").Trim()
                    };

                    #region Reemplazo el número de credito por el de la carpeta (si aplica)
                    if ((archivo.NumCredito ?? "").Equals("00000000000000"))
                    {
                        string[] directorios = (Path.GetDirectoryName(rutaCorta) ?? "").Split(Path.DirectorySeparatorChar);
                        if (directorios is not null && (directorios.Length > 3))
                        {
                            string posibleNumeroContrato = (directorios[2] ?? ""); // .LastOrDefault()
                            if (!string.IsNullOrEmpty(posibleNumeroContrato))
                            {
                                archivo.NumCredito = Condiciones.ObtieneExpedienteBienesAdjudicados(posibleNumeroContrato)+ "00000000";
                                _logger.LogInformation("Expediente : {numCredito} {archivo}", archivo.NumCredito, archivo.NombreArchivo);
                            }
                        }
                    }
                    #endregion
                    /// Si aun así el número de crédito es nulo, no lo guardo
                    // if ((!Condiciones.QuitaCastigo(archivo.NumCredito).Equals(Condiciones.C_STR_NULLO)) || _registraTodasLasImagenes)
                    // {
                    if (!string.IsNullOrEmpty(archivo.NumCredito))
                    { 
                        // Obtengo el expediente
                        int numCredito = Convert.ToInt32(archivo.NumCredito[..8]);
                        // Solo guardo los expedientes superiores al año 1995
                        if (numCredito > 19950000 && numCredito < 20250000)
                        {
                            resultado.Add(archivo);
                            id++;
                        }
                    }
                }
                renglon++;
            }
        }
        _logger.LogInformation("Se encontraron {cantidadArchivos} en Bienes Adjudicados", resultado.Count);
        //return resultado.Where(x => !Condiciones.QuitaCastigo(x.NumCredito).Equals(Condiciones.C_STR_NULLO)).ToList();
        return resultado;
    }
}
