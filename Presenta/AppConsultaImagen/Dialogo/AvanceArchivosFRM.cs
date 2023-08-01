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
    public partial class AvanceArchivosFRM : Form
    {
        public AvanceArchivosFRM()
        {
            InitializeComponent();
        }

        public string InformacionAvance { get { return this.lblDetalleEtiquetaAvance.Text; } set { this.lblDetalleEtiquetaAvance.Text = value; } }
        public string Titulo
        {
            get
            {
                TopMost = false;
                return this.Text;
            }
            set
            {
                this.Text = value;
                TopMost = true;
            }
        }
        public int Porcentaje { get { return this.prgAvance.Value; } set { this.prgAvance.Value = value; } }
    }
}
