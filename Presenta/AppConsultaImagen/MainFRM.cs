using CefSharp;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Web;
//using Microsoft.Windows.Sdk;

namespace AppConsultaImagen;

public partial class MainFRM
{
    #region Parche para que funcione el publish de Microsoft
    const int I_BUFFER_SIZE = 4096;
    public static IList<string> DescomprimeArchivo(MemoryStream fs, string carpetaDestino)
    {
        IList<string> lista = new List<string>();
        ZipFile file;
        // FileStream? fs;
        try
        {
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            //fs = File.OpenRead(fileName);
            file = new ZipFile(fs);
            // Se recorren todos los archivos contenidos en el ZIP
            foreach (ZipEntry archivoCompreso in file)
            {
                // Valido que el archivo compreso sea un archivo, si es una carpeta, lo ignoro!!!!
                if (!archivoCompreso.IsFile)
                {
                    continue;
                }
                // Obtengo el nombre del archivo y le agrego la fecha de descompresión
                String nombreArchivoADescomprimir = archivoCompreso.Name;

                // un buffer de 4k es optimo para esta operación, cuando se manipulen archivos grandes
                byte[] buffer = new byte[I_BUFFER_SIZE];

                // Cargo el encabezado del archivo en turno por descomprimir
                Stream zipStream = file.GetInputStream(archivoCompreso);

                // Manipulate the output filename here as desired.
                #region Ajustes sobre el archivo destino, como agregar el directorio de salida, y si no existe el directorio, lo creamos
                /// FIX: Se eliminó nombres con "," o con ";", reemplazandose con "_"
                String rutaCompletaDelArchivoADescomprimir = Path.Combine(carpetaDestino, nombreArchivoADescomprimir).Replace(",", "_").Replace(";", "_");
                string? nombreDirectorioDestino = Path.GetDirectoryName(rutaCompletaDelArchivoADescomprimir);

                #region Se crea el directorio destino, en caso de no existir
                if (nombreDirectorioDestino != null && nombreDirectorioDestino.Length > 0)
                {
                    Directory.CreateDirectory(nombreDirectorioDestino);
                }
                #endregion
                #endregion

                #region Se vacía el ZIP en el directorio
                using (FileStream streamWriter = File.Create(rutaCompletaDelArchivoADescomprimir))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
                #endregion
                lista.Add(rutaCompletaDelArchivoADescomprimir);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return lista.ToList();

    }

    static async Task DescargaZip()
    {
        string url = "https://infofin.parquefundidora.org/ParqueFundidora/lib/lib1.4.0.1.zip";
        string destino = "lib.zip";
        string carpetaDestino = ".\\lib";

        try
        {
            if (Directory.Exists(carpetaDestino))
            {
                // Directory.Delete(carpetaDestino, true);
            }
        }
        catch (Exception)
        {

            throw;
        }


        if (File.Exists(destino))
            File.Delete(destino);

        using HttpClient httpClient = new();
        using HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            using (HttpContent content = response.Content)
            {
                await using var stream = await content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream(); //new System.IO.FileStream(destino, System.IO.FileMode.Create))
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                DescomprimeArchivo(memoryStream, ".\\");

            }

            Console.WriteLine("Descarga completada.");
        }
        else
        {
            Console.WriteLine($"Error al descargar el archivo. Código de estado: {response.StatusCode}");
        }
    }
    #endregion

    public MainFRM()
    {
        #region Comienza Splash Screen
        const string C_STR_CTL_SPLASH = "ControlSplash.txt";
        Thread t = new(new ThreadStart(() => { Application.Run(new SpashScreenFRM()); }));
        // Crear un objeto StreamWriter para escribir el contenido en el archivo de texto
        using (StreamWriter sw = new(C_STR_CTL_SPLASH, true))
        {
            // Escribir el contenido del StringBuilder en el archivo de texto
            sw.Write("Si");
        }

        t.Start();
        #endregion


        Task.Run(async () =>
        {
            try
            {
                await DescargaZip();
            }
            catch (Exception ex)
            {
                _logger?.LogError("Mensaje de error {error}", ex.Message);
            }
            // El código aquí se ejecutará después de que el método asíncrono termine su ejecución
        }).Wait();
        /*
                Thread t2 = new(new ThreadStart(()=> {  }));
                t2.Start();
        */

        if (!File.Exists("appsettings.json") || !File.Exists("appsettings.Debug.json") || !File.Exists("appsettings.Production.json"))
        {
            MessageBox.Show("No se encontraron los archivos de configuración, favor de verificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
        this.SendToBack();

        InitializeComponent();
        chromiumWebBrowser1.LoadingStateChanged += ChromiumWebBrowser1_LoadingStateChanged;
        chromiumWebBrowser2.LoadingStateChanged += ChromiumWebBrowser2_LoadingStateChanged;

        this.pnlTodoTabNavegacion.Resize += PnlTodoTabNavegacion_Resize;
        this.pnlTodoTabReporteFinal.Resize += PnlTodoTabReporteFinal_Resize;

        CargaInformacion();
        ActivaMenus();
        ActivaNavegacion();

        InicializaBusquedaPorExpediente();
        InicializaBusquedaPorAcreditado();
        InicializaVisorDeImagenes();
        InicializaResumen();
        InicializaReporteExpCastigos();
        InicializaReporteFinalInicializacionAgencias();
        InicializaCreditosCancelados();
        InicializaBusquedaBienesAdjudicados();
        InicializaVisorDeImagenesBienesAdjudicados();
        InicializaCreditosLiquidados();
        InicializaTratamientos();
        InicializaExpedienteJuridico();
        InicializaVisorDeImagenesExpendedientesCobranza();
        InicializaGuardaValores();

        #region Termina Splash Screen
        if (File.Exists(C_STR_CTL_SPLASH))
            File.Delete(C_STR_CTL_SPLASH);

        TmrBringToFront.Enabled = true;
        TmrBringToFront.Start();
        #endregion
    }

    private void PnlTodoTabReporteFinal_Resize(object? sender, EventArgs e)
    {
        tabReportesFinales.Location = new System.Drawing.Point(0, -29);
        tabReportesFinales.Dock = (global::System.Windows.Forms.DockStyle.None);
        tabReportesFinales.Size = new System.Drawing.Size(pnlTodoTabReporteFinal.Width, pnlTodoTabReporteFinal.Height + 29);
    }

    private void PnlTodoTabNavegacion_Resize(object? sender, EventArgs e)
    {
        tabNavegacion.Location = new System.Drawing.Point(0, -29);
        tabNavegacion.Dock = (global::System.Windows.Forms.DockStyle.None);
        tabNavegacion.Size = new System.Drawing.Size(pnlTodoTabNavegacion.Width, pnlTodoTabNavegacion.Height + 29);
    }

    private async void TmrBringToFront_Tick(object sender, EventArgs e)
    {
        this.BringToFront();
        await Task.Run(() => RevisaElTemporalParaEvitarProblemasDeDesempenioConImagenes());
    }

    private void RevisaElTemporalParaEvitarProblemasDeDesempenioConImagenes()
    {
        // Mantiene vivo la unidad de las imagenes para que su carga sea rápida
        if (_windowsFormsGloablInformation is null)
            return;
        _windowsFormsGloablInformation.ActivaConsultasServices().CreaTemporal();
    }
}