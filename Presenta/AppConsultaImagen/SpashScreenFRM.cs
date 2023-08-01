using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppConsultaImagen
{
    public partial class SpashScreenFRM : Form
    {
        const string C_STR_CTL_SPLASH = "ControlSplash.txt";
        public SpashScreenFRM()
        {
            InitializeComponent();
            Text = "Splash Screen";
            timer1.Start();
        }

        public void StartTimer()
        {
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //StartPosition = FormStartPosition.CenterScreen;
            prgProgressBar.Increment(1);
            if (prgProgressBar.Value == 101)
                prgProgressBar.Value = 0;

            if (!File.Exists(C_STR_CTL_SPLASH))
            {
                TopMost = false;
                Close();
            }
        }
    }
}
