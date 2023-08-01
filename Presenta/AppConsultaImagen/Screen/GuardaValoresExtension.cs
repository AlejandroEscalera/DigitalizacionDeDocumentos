using AppConsultaImagen.Dialogo;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Excel.Arqueos;
using gob.fnd.Dominio.Digitalizacion.Negocio.Descarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen
{
    public partial class MainFRM
    {
        public void InicializaGuardaValores() 
        { 
            btnReporteGuardaValor.Click += BtnGuardaValoresClick;
            btnDescargaGuardaValores.Click += BtnDescargaGuardaValoresClick;
        }

        private readonly AvanceArchivosFRM frm = new();

        protected async void BtnDescargaGuardaValoresClick(object? sender, EventArgs e)
        {
            var guardaValores =_windowsFormsGloablInformation?.ActivaConsultasServices().ObtieneGuardaValores(0);
            if (guardaValores is not null)
            {
                fbdGuardaImagenes.Description = "Selecciona la carpeta donde se encuentran los archivos de Guarda Valores";
                fbdGuardaImagenes.RootFolder = Environment.SpecialFolder.MyComputer;
                fbdGuardaImagenes.ShowNewFolderButton = true;
                string carpetaDestino = string.Empty;
                if (fbdGuardaImagenes.ShowDialog() == DialogResult.OK)
                {
                    carpetaDestino = fbdGuardaImagenes.SelectedPath+Path.DirectorySeparatorChar;
                }
                else 
                {
                    return;
                }

                object objValue = dgvrdDetalleAgencias.Rows[0].Cells[1].Value;
                int agencia = Convert.ToInt32(objValue);
                bool descargo = false;
                frm.Titulo = "Descarga de Guarda Valores de la Agencia " + agencia;
                try
                {
                    frm.Owner = this;
                    frm.Show();
                    Progress<ReporteProgresoDescompresionArchivos> avance = new();
                    avance.ProgressChanged += Avance_ProgressChanged;
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                    descargo = await _windowsFormsGloablInformation?
                        .ActivaConsultasServices().
                        DescargaGuardaValoresAgencia(guardaValores, agencia, carpetaDestino, avance);
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
                }
                finally
                {
                    string valor = frm.Titulo;
                    frm.Close();
                }

                if (descargo)
                {
                    MessageBox.Show($"Se descargaron correctamente los Guarda Valores.");
                }
                    else
                {
                    MessageBox.Show($"No se pudieron copiar los Guarda Valores.");
                }
            }
        }

        private void Avance_ProgressChanged(object? sender, ReporteProgresoDescompresionArchivos e)
        {
            frm.InformacionAvance = e.InformacionArchivo??"";
            frm.Porcentaje = Convert.ToInt32(e.ArchivoProcesado * 100 / e.CantidadArchivos);
        }

        public void BtnGuardaValoresClick(object? sender, EventArgs e)
        {
            // sfdGuardaReporte
            // string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfdGuardaReporte.DefaultExt = "xlsx";
            sfdGuardaReporte.Filter = "Archivos de Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*";
            sfdGuardaReporte.FileName = string.Format("{0:yy}{0:MM}{0:dd}_{1}.xlsx", DateTime.Now, lblrdDetalleAgencia.Text.Replace("Creditos de la Agencia ","").Replace(",","").Replace(" ","-").Replace(".",""));

            // Obtiene la ruta de la carpeta "Mis documentos"
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Establece la ruta de inicio en "Mis documentos"
            sfdGuardaReporte.InitialDirectory = myDocumentsPath;

            if (sfdGuardaReporte.ShowDialog() == DialogResult.OK)
            {
                // Obtiene la ruta del archivo seleccionado
                string rutaArchivo = sfdGuardaReporte.FileName;
                object objValue = dgvrdDetalleAgencias.Rows[0].Cells[1].Value;
                int agencia = Convert.ToInt32(objValue);
                // Guarda el archivo
                var guardaValores = _windowsFormsGloablInformation?.ActivaConsultasServices().ObtieneGuardaValores(agencia);
                if (guardaValores != null)
                {
                    if (_windowsFormsGloablInformation?.ActivaConsultasServices().GuardaValores(guardaValores, rutaArchivo) == true)
                    {
                        MessageBox.Show($"Se guardó el archivo con {guardaValores.Count()} contratos.");
                    }
                    else
                    {
                        MessageBox.Show($"Hay un error al intentar guardar los contratos.");
                    }
                }
            }
        }
    }
}