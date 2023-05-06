using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsultaImagen;

public partial class MainFRM
{
    private int _expCasRegionSeleccionada;
    private int _expCasAgenciaSeleccionada;
    private string? _expCasExpedienteNumCreditoSeleccionado;
    protected void InicializaReporteExpCastigos()
    {
        #region Tabla de Reporte de Expedientes Castigos por Region
        dgvExpCastigoRegion.AutoGenerateColumns = false;
        dgvExpCastigoRegion.DataSource = _expCasRegiones;
        dgvExpCastigoRegion.AllowUserToAddRows = false;
        dgvExpCastigoRegion.AllowUserToDeleteRows = false;
        dgvExpCastigoRegion.AllowUserToResizeRows = false;
        dgvExpCastigoRegion.EditMode = DataGridViewEditMode.EditProgrammatically;
        dgvExpCastigoRegion.MultiSelect = false;
        dgvExpCastigoRegion.ReadOnly = true;
        dgvExpCastigoRegion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        #endregion

        #region Tabla de Reporte de Expedientes Castigos por Agencia
        dgvECAgencia.AutoGenerateColumns = false;
        dgvECAgencia.DataSource = _expCasAgencias;
        dgvECAgencia.AllowUserToAddRows = false;
        dgvECAgencia.AllowUserToDeleteRows = false;
        dgvECAgencia.AllowUserToResizeRows = false;
        dgvECAgencia.EditMode = DataGridViewEditMode.EditProgrammatically;
        dgvECAgencia.MultiSelect = false;
        dgvECAgencia.ReadOnly = true;
        dgvECAgencia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        #endregion

        #region Tabla de Reporte de Expedientes Castigos por Expediente
        dgvECExpediente.AutoGenerateColumns = false;
        dgvECExpediente.DataSource = _expCasAgencias;
        dgvECExpediente.AllowUserToAddRows = false;
        dgvECExpediente.AllowUserToDeleteRows = false;
        dgvECExpediente.AllowUserToResizeRows = false;
        dgvECExpediente.EditMode = DataGridViewEditMode.EditProgrammatically;
        dgvECExpediente.MultiSelect = false;
        dgvECExpediente.ReadOnly = true;
        dgvECExpediente.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        #endregion

        #region Formateo de columnas de Expedientes Castigo por Region
        colECRRegion.Width = 71;
        colECRCatRegion.Width = 185;
        colECRMontoCredito.Width = 162;
        colECRMontoCredito.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRMontoCredito.DefaultCellStyle.Format = "C2";
        colECRMontoCredito.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECRInteresCont.Width = 162;
        colECRInteresCont.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRInteresCont.DefaultCellStyle.Format = "C2";
        colECRInteresCont.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECRTotalExpCastgado.Width =  125;
        colECRTotalExpCastgado.DefaultCellStyle.Format = "N0";
        colECRTotalExpCastgado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRTotalReportos.Width = 108;
        colECRTotalReportos.DefaultCellStyle.Format = "N0";
        colECRTotalReportos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRTotalArqueo.Width = 87;
        colECRTotalArqueo.DefaultCellStyle.Format = "N0";
        colECRTotalArqueo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRTotalCancelados.Width = 99;
        colECRTotalCancelados.DefaultCellStyle.Format = "N0";
        colECRTotalCancelados.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECRTotalActivos.Width = 125;
        colECRTotalActivos.DefaultCellStyle.Format = "N0";
        colECRTotalActivos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        #endregion

        #region Formateo de columnas de Expedientes Castigo por Agencia
        colECANumAgecia.Width = 71;
        colECANumAgecia.DataPropertyName = "NumAgencia";
        colECACatAgencia.Width = 200;
        colECACatAgencia.DataPropertyName = "CatAgencia";
        colECAMontoCredito.Width = 162;
        colECAMontoCredito.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECAMontoCredito.DefaultCellStyle.Format = "C2";
        colECAMontoCredito.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECAMontoCredito.DataPropertyName = "MontoCredito";
        colECAInteresCont.Width = 162;
        colECAInteresCont.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECAInteresCont.DefaultCellStyle.Format = "C2";
        colECAInteresCont.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECAInteresCont.DataPropertyName = "InteresCont";
        colECATotalExpCastigados.Width = 125;
        colECATotalExpCastigados.DefaultCellStyle.Format = "N0";
        colECATotalExpCastigados.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECATotalExpCastigados.DataPropertyName = "TotalExpCastigados";
        colECATotalReportos.Width = 108;
        colECATotalReportos.DefaultCellStyle.Format = "N0";
        colECATotalReportos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECATotalReportos.DataPropertyName = "TotalReportos";
        colECATotalArqueos.Width = 87;
        colECATotalArqueos.DefaultCellStyle.Format = "N0";
        colECATotalArqueos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECATotalArqueos.DataPropertyName = "TotalConArqueos";
        colECATotalCancelados.Width = 99;
        colECATotalCancelados.DefaultCellStyle.Format = "N0";
        colECATotalCancelados.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECATotalCancelados.DataPropertyName = "TotalCancelados";
        colECATotalActivos.Width = 125;
        colECATotalActivos.DefaultCellStyle.Format = "N0";
        colECATotalActivos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECATotalActivos.DataPropertyName = "TotalActivos";
        #endregion

        #region Formateo de columnas de Expedientes Castigo por Expediente
        colECENumCredito.Width = 185;
        colECENumCredito.DataPropertyName = "NumCredito";
        colECEAcreditado.Width = 500;
        colECEAcreditado.DataPropertyName = "Acreditado";
        colECEMontoCredito.Width = 162;
        colECEMontoCredito.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECEMontoCredito.DefaultCellStyle.Format = "C2";
        colECEMontoCredito.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECEMontoCredito.DataPropertyName = "MontoCredito";
        colECEInteresCont.Width = 162;
        colECEInteresCont.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colECEInteresCont.DefaultCellStyle.Format = "C2";
        colECEInteresCont.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-MX");
        colECEInteresCont.DataPropertyName = "InteresCont";
        colECECastigo.Width = 125;
        colECECastigo.DataPropertyName = "Castigo";
        colECEReporto.Width = 108;
        colECEReporto.DataPropertyName = "Reporto";
        colECECuentaConArqueo.Width = 87;
        colECECuentaConArqueo.DataPropertyName = "CuentaConArqueo";
        colECEEstaCancelado.Width = 99;
        colECEEstaCancelado.DataPropertyName = "EstaCancelado";
        colECEEstaActivo.Width = 125;
        colECEEstaActivo.DataPropertyName = "EstaActivo";
        #endregion

#pragma warning disable CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
        tabExpedientesConCastigo.Selecting += TabNavegacionReporteExpCas_Selecting;
        dgvExpCastigoRegion.SelectionChanged += DgvExpCasRegion_SelectionChange;
        btnECRSiguiente.Click += BtnECRSiguiente;
        btnECASiguiente.Click += BtnECASiguiente;
        btnECAAnterior.Click += BtnECAAnterior;
        btnECEAnterior.Click += BtnECEAnterior;
        btnECEVerImagen.Click += BtnMuestraImagenECE;
        dgvECAgencia.SelectionChanged += DgvExpCasAgencia_SelectionChange;
        dgvECExpediente.SelectionChanged += DgvExpCasExpediente_SelectionChange;
#pragma warning restore CS8622 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
    }
    
    private void DgvExpCasRegion_SelectionChange(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            _expCasRegionSeleccionada = Convert.ToInt32(dgvExpCastigoRegion.CurrentRow.Cells[0].Value);
        }
    }

    private void DgvExpCasAgencia_SelectionChange(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            _expCasAgenciaSeleccionada = Convert.ToInt32(dgvECAgencia.CurrentRow.Cells[0].Value);            
        }
    }

    private void DgvExpCasExpediente_SelectionChange(object sender, EventArgs e)
    {
        if (!_estoyPintandoDatos)
        {
            _expCasExpedienteNumCreditoSeleccionado = Convert.ToString(dgvECExpediente.CurrentRow.Cells[0].Value);
            if (_archivosImagenes is not null)
            {
                imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(_expCasExpedienteNumCreditoSeleccionado, StringComparison.OrdinalIgnoreCase)).ToArray();
                if (!imagenesEncontradas.Any())
                {
                    // 214960012450000001
                    imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(_expCasExpedienteNumCreditoSeleccionado, StringComparison.OrdinalIgnoreCase)).ToArray();
                }
                imagenActual = 0;
                btnECEVerImagen.Visible = imagenesEncontradas.Length != 0;
            }
        }
    }
    protected void BtnECRSiguiente(object sender, EventArgs e)
    {
        if (_expCasRegionSeleccionada != 0)
        {
            if (_windowsFormsGloablInformation is not null && _infoExpedientes is not null )
            {
                canNavigate = true;
                _estoyPintandoDatos = true;
                try
                {
                    if (_windowsFormsGloablInformation is not null && _infoExpedientes is not null)
                    {
                        _expCasAgencias = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaAgencias(_infoExpedientes, _expCasRegionSeleccionada);
                        dgvECAgencia.DataSource = _expCasAgencias;
                        tabExpedientesConCastigo.SelectedIndex = 1;
                    }
                }
                finally {
                    canNavigate = false;
                    _estoyPintandoDatos = false;
                }
            }
        }
    }

    protected void BtnECASiguiente(object sender, EventArgs e)
    {
        if (_expCasAgenciaSeleccionada != 0)
        {
            if (_windowsFormsGloablInformation is not null && _infoExpedientes is not null)
            {
                canNavigate = true;
                _estoyPintandoDatos = true;
                try
                {
                    if (_windowsFormsGloablInformation is not null && _infoExpedientes is not null)
                    {
                        _expCasExpedientes = _windowsFormsGloablInformation.ActivaConsultasServices().CalculaExpedientes(_infoExpedientes, _expCasAgenciaSeleccionada);
                        dgvECExpediente.DataSource = _expCasExpedientes;
                        tabExpedientesConCastigo.SelectedIndex = 2;
                    }
                }
                finally
                {
                    canNavigate = false;
                    _estoyPintandoDatos = false;
                }
            }
        }
    }

    protected void BtnECAAnterior(object sender, EventArgs e)
    {
        canNavigate = true;
        _estoyPintandoDatos = true;
        try
        {
            tabExpedientesConCastigo.SelectedIndex = 0;
        }
        finally
        {
            canNavigate = false;
            _estoyPintandoDatos = false;
        }
    }

    protected void BtnECEAnterior(object sender, EventArgs e)
    {
        canNavigate = true;
        _estoyPintandoDatos = true;
        try
        {
            tabExpedientesConCastigo.SelectedIndex = 1;
        }
        finally
        {
            canNavigate = false;
            _estoyPintandoDatos = false;
        }
    }

    protected void BtnMuestraImagenECE(object sender, EventArgs e)
    {
        _expCasExpedienteNumCreditoSeleccionado = Convert.ToString(dgvECExpediente.CurrentRow.Cells[0].Value);
        if (_archivosImagenes is not null && (imagenesEncontradas is not null && imagenesEncontradas.Length != 0))
        {
            imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(_expCasExpedienteNumCreditoSeleccionado, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (!imagenesEncontradas.Any())
            {
                imagenesEncontradas = _archivosImagenes.OrderByDescending(y => y.NumPaginas).Where(x => (x.NumCredito ?? "").Equals(_expCasExpedienteNumCreditoSeleccionado, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            imagenActual = 0;
        }
        if (_infoExpedientes is not null)
        {
            var expediente = _infoExpedientes.Where(x => (x.NumCredito ?? "").Equals(_expCasExpedienteNumCreditoSeleccionado)).FirstOrDefault();
            MuestraExpedienteEnImagen(expediente);
            NavegaVisorImagenes();
        }
    }
    protected void TabNavegacionReporteExpCas_Selecting(object sender, TabControlCancelEventArgs e)
    {
        if (!canNavigate)
            e.Cancel = true;
    }
}
