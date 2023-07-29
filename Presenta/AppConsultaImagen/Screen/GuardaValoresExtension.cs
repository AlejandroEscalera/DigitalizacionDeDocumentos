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
            var guardaValores = _windowsFormsGloablInformation?.ActivaConsultasServices().ObtieneGuardaValores(0);
            if (guardaValores != null)
            {
                foreach (var gv in guardaValores)
                {
                    Console.WriteLine($"Regional: {gv.Regional}");
                }
            }
        }
    }
}