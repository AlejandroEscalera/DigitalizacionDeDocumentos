using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

/// <summary>
/// Aqui se concentran las rutinas de navegación de la página
/// </summary>
public partial class MainFRM
{
    private bool canNavigate = false;

    protected void NavegaMenu2(bool principal)
    {
        canNavigate = true;
        if (principal)
        {
            pnlRegresaAnteriorBxE.Visible = datosRegreso.Count() != 0;
            tabNavegacion.SelectedIndex = 0;
        }
        else
            tabNavegacion.SelectedIndex = 1;
        canNavigate = false;
        pnlDetalleBusquedaPorExpediente.Visible = false;
        pnlDetalleBusquedaxA.Visible = false;
    }

    protected void NavegaMenu3(bool principal)
    {
        canNavigate = true;
        if (principal)
        {
            tabNavegacion.SelectedIndex = 2;
            tabReportesFinales.SelectedIndex = 0;
            CalculaExpedientesAMostrar();
        }
        else
        {
            tabNavegacion.SelectedIndex = 3;
            tabExpedientesConCastigo.SelectedIndex = 0;
            Close();
        }
        canNavigate = false;
        pnlDetalleBusquedaPorExpediente.Visible = false;
        pnlDetalleBusquedaxA.Visible = false;
    }

    protected void NavegaVisorImagenes()
    {
        canNavigate = true;
        tabNavegacion.SelectedIndex = 4;
        canNavigate = false;
        imagenActual = 1;
        ImagenAnterior();
    }



    protected void ActivaNavegacion()
    {
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        tabNavegacion.Selecting += TabNavegacion_Selecting;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        LblMenuClick(lblReportes, new EventArgs());
        LblSubMenu2Click(lblReportesPorExpedienteActivo, new EventArgs());
        NavegaMenu3(true);
    }
    protected void TabNavegacion_Selecting(object sender, TabControlCancelEventArgs e)
    {        
        if (!canNavigate)
            e.Cancel = true;
    }
}
