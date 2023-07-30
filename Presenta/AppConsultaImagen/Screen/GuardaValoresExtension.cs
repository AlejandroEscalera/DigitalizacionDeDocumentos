using System;
using System.Collections.Generic;
using System.Linq;
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