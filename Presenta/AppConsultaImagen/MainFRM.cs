using CefSharp;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using System.Threading;
using System.Web;
//using Microsoft.Windows.Sdk;

namespace AppConsultaImagen
{
    public partial class MainFRM
    {
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

        private void TmrBringToFront_Tick(object sender, EventArgs e)
        {
            this.BringToFront();
            TmrBringToFront.Enabled = false;
            // User32.SetWindowPos(this.Handle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_SHOWWINDOW);
            TmrBringToFront.Stop();
        }
    }
}