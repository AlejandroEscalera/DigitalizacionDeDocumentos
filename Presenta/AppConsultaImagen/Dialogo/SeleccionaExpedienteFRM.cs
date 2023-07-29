using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppConsultaImagen.Dialogo
{
    public partial class SeleccionaExpedienteFRM : Form
    {
        public SeleccionaExpedienteFRM()
        {
            InitializeComponent();
            dgvListaDeExpedientes.DoubleClick += DoBuscar;
            btnBuscar.Click += DoBuscar;
            btnCancelar.Click += (s, e) => Hide();
        }

        public MainFRM? FormaPrincipal { get; set; }

        public void CargaListaDeExpedientes(string[] listaDeExpedientes)
        {
            dgvListaDeExpedientes.Rows.Clear();
            foreach (string numCredito in listaDeExpedientes)
            {
                dgvListaDeExpedientes.Rows.Add(numCredito);
            }
            ShowDialog();
        }

        private void DoBuscar(object? sender, EventArgs e)
        {
            if (FormaPrincipal is not null)
            {
                if (dgvListaDeExpedientes.CurrentRow is not null)
                {
                    string numCredito = dgvListaDeExpedientes.CurrentRow.Cells[0].Value.ToString() ?? "";
                    FormaPrincipal.BuscaExpedienteDetallePorNumCredito(numCredito);
                    Close();
                }
            }
        }
    }
}
