using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM : Form
{
    #region Constantes de nombres de menus
    const string C_STR_MENU_BUSQUEDA = "Búsqueda";
    const string C_STR_MENU_REPORTES = "Reportes";

    const string C_STR_MENU_BUSQUEDA_POR_EXPEDIENTE = "Por expediente";
    const string C_STR_MENU_BUSQUEDA_POR_ACREDITADO = "Por acreditado";

    const string C_STR_MENU_REPORTE_POR_EXPEDIENTE = "Expedientes activos";
    const string C_STR_MENU_REPORTE_POR_CASTIGO = "Expedientes con Castigo";
    #endregion

    #region Constante de colores
    private readonly Color cMenuUnselectedBackground = Color.FromArgb(12, 35, 30);
    private readonly Color cMenuUnselectedForeground = Color.White;

    private readonly Color cMenuSelectedBackground = Color.Olive;
    private readonly Color cMenuSelectedForeground = Color.LightGray;

    private readonly Color cMenuMouseHoverSelectedBackground = Color.LightGray;
    private readonly Color cMenuMouseHoverSelectedForeground = Color.Black;

    private readonly Color cMenuMouseHoverUnselectedBackground = Color.Gray;
    private readonly Color cMenuMouseHoverUnselectedForeground = Color.White;
    #endregion

    #region Inicializa los menus
    /// <summary>
    /// Inicializa los menus
    /// </summary>
    /// <param name="sender">Etiqueta que se va a configurar</param>
    /// <param name="menu">Tipo de Menu</param>
    /// <param name="texto">Texto con el que se va a configurar</param>
    /// <param name="selected">Si está seleccionado</param>
    protected void EscribeEtiqueta(Label sender, int menu, string texto, bool selected = false)
    {
        sender.Text = texto;
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        sender.MouseLeave += MouseLeaveClick;
        sender.MouseHover += MouseHoverClick;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        switch (menu)
        {
            case 1:
#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
                sender.MouseClick += LblMenuClick;
                break;
            case 2:
                sender.MouseClick += LblSubMenu1Click;
                break;
            case 3:
                sender.MouseClick += LblSubMenu2Click;
                break;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        }
        sender.Tag = selected;
    }

    /// <summary>
    /// Activa los valores iniciales
    /// </summary>
    protected void ActivaMenus()
    {
        EscribeEtiqueta(lblBusqueda, 1, C_STR_MENU_BUSQUEDA, true);
        EscribeEtiqueta(lblReportes, 1, C_STR_MENU_REPORTES , false);

        EscribeEtiqueta(lblBusquedaPorExpediente, 2, C_STR_MENU_BUSQUEDA_POR_EXPEDIENTE, false);
        EscribeEtiqueta(lblBusquedaPorAcreditado, 2, C_STR_MENU_BUSQUEDA_POR_ACREDITADO, false);

        EscribeEtiqueta(lblReportesPorExpedienteActivo, 3, C_STR_MENU_REPORTE_POR_EXPEDIENTE, false);
        EscribeEtiqueta(lblReportePorExpedienteConCastigo, 3, C_STR_MENU_REPORTE_POR_CASTIGO, true);

    }
    #endregion

    #region Comportamiento
    /// <summary>
    /// Sucede al pasar el ratón por encima
    /// </summary>
    /// <param name="sender">Etiqueta</param>
    /// <param name="e">Argumento del evento</param>
    protected void MouseHoverClick(object sender, EventArgs e)
    {
        if (sender is Label lblMouseOver)
        {
            bool isSelected = Convert.ToBoolean(lblMouseOver.Tag);
            if (isSelected)
            {
                lblMouseOver.BackColor = cMenuMouseHoverSelectedBackground;
                lblMouseOver.ForeColor = cMenuMouseHoverSelectedForeground;
            }
            else
            {
                lblMouseOver.BackColor = cMenuMouseHoverUnselectedBackground;
                lblMouseOver.ForeColor = cMenuMouseHoverUnselectedForeground;
            }
        }
    }
    /// <summary>
    /// Al salir de pasar el menú por encima
    /// </summary>
    /// <param name="sender">Etiqueta</param>
    /// <param name="e">Argumento del evento</param>
    protected void MouseLeaveClick(object sender, EventArgs e)
    {
        if (sender is Label lblMouseLeave)
        {
            bool isSelected = Convert.ToBoolean(lblMouseLeave.Tag);
            if (isSelected)
            {
                lblMouseLeave.BackColor = cMenuSelectedBackground;
                lblMouseLeave.ForeColor = cMenuSelectedForeground;
            }
            else
            {
                lblMouseLeave.BackColor = cMenuUnselectedBackground;
                lblMouseLeave.ForeColor = cMenuUnselectedForeground;
            }
        }
    }
    #endregion

    #region Primer nivel de menu
    protected void DesHabilitoMenu1() {
        lblBusqueda.Tag = false;
        lblBusqueda.BackColor = cMenuUnselectedBackground;
        lblBusqueda.ForeColor = cMenuUnselectedForeground;
        lblReportes.Tag = false;
        lblReportes.BackColor = cMenuUnselectedBackground;
        lblReportes.ForeColor = cMenuUnselectedForeground;
    }
    protected void HabilitoSubMenu(bool principal) {
        pnlPorBusqueda.Visible = principal;
        pnlPorReportes.Visible = !principal;
    }
    #endregion
    #region Segundo nivel de menu
    protected void DesHabilitoMenu2()
    {
        lblBusquedaPorExpediente.Tag = false;
        lblBusquedaPorExpediente.BackColor = cMenuUnselectedBackground;
        lblBusquedaPorExpediente.ForeColor = cMenuUnselectedForeground;
        lblBusquedaPorAcreditado.Tag = false;
        lblBusquedaPorAcreditado.BackColor = cMenuUnselectedBackground;
        lblBusquedaPorAcreditado.ForeColor = cMenuUnselectedForeground;
    }
    #endregion
    #region Tercer nivel de menu
    protected void DesHabilitoMenu3()
    {
        lblReportesPorExpedienteActivo.Tag = false;
        lblReportesPorExpedienteActivo.BackColor = cMenuUnselectedBackground;
        lblReportesPorExpedienteActivo.ForeColor = cMenuUnselectedForeground;
        lblReportePorExpedienteConCastigo.Tag = false;
        lblReportePorExpedienteConCastigo.BackColor = cMenuUnselectedBackground;
        lblReportePorExpedienteConCastigo.ForeColor = cMenuUnselectedForeground;
    }
    #endregion
    protected void LblMenuClick(object sender, EventArgs e)
    {
        if (sender is Label lblMenuClick)
        {
            bool isSelected = Convert.ToBoolean(lblMenuClick.Tag);
            if (!isSelected)
            {
                DesHabilitoMenu1();

                #region Selecciono el menú al que se le da click
                lblMenuClick.Tag = true;
                lblMenuClick.BackColor = cMenuSelectedBackground;
                lblMenuClick.ForeColor = cMenuSelectedForeground;
                #endregion

                HabilitoSubMenu(lblMenuClick.Text.Equals(C_STR_MENU_BUSQUEDA));
            };
        }
    }

    protected void LblSubMenu1Click(object sender, EventArgs e)
    {
        if (sender is Label lblMenuClick)
        {
            bool isSelected = Convert.ToBoolean(lblMenuClick.Tag);
            if (!isSelected)
            {
                DesHabilitoMenu2();

                #region Selecciono el menú al que se le da click
                lblMenuClick.Tag = true;
                lblMenuClick.BackColor = cMenuSelectedBackground;
                lblMenuClick.ForeColor = cMenuSelectedForeground;
                #endregion
            }
            NavegaMenu2(lblMenuClick.Text.Equals(C_STR_MENU_BUSQUEDA_POR_EXPEDIENTE));
        }
    }

    protected void LblSubMenu2Click(object sender, EventArgs e)
    {
        if (sender is Label lblMenuClick)
        {
            bool isSelected = Convert.ToBoolean(lblMenuClick.Tag);
            if (!isSelected)
            {
                DesHabilitoMenu3();

                #region Selecciono el menú al que se le da click
                lblMenuClick.Tag = true;
                lblMenuClick.BackColor = cMenuSelectedBackground;
                lblMenuClick.ForeColor = cMenuSelectedForeground;
                #endregion
            }
            NavegaMenu3(lblMenuClick.Text.Equals(C_STR_MENU_REPORTE_POR_EXPEDIENTE));
        }
    }
}
