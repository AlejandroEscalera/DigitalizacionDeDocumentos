using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen
{
    public partial class MainFRM
    {
        public void BuscaExpedienteDetallePorNumCredito(string numCredito)
        {
            canNavigate = true;
            try
            {
                tabNavegacion.SelectedIndex = 0;
                pnlRegresaAnteriorBxE.Visible = datosRegreso.Peek() is not null;
                txtNumeroExpediente.Text = numCredito;
                BuscaPorExpediente();
                // BuscaPorNumeroDeCredito(numCredito);
            }
            catch
            {
                canNavigate = false;
            }
        }
    }
}
