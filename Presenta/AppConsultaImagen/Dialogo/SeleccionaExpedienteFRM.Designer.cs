namespace AppConsultaImagen.Dialogo
{
    partial class SeleccionaExpedienteFRM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            pnlFooterBusquedaPorExpediente = new Panel();
            panel1 = new Panel();
            btnBuscar = new Button();
            btnCancelar = new Button();
            pnlMenuInicial = new Panel();
            lblSistemaConsultaExpediente = new Label();
            dgvListaDeExpedientes = new DataGridView();
            colNumCredito = new DataGridViewTextBoxColumn();
            pnlFooterBusquedaPorExpediente.SuspendLayout();
            panel1.SuspendLayout();
            pnlMenuInicial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvListaDeExpedientes).BeginInit();
            SuspendLayout();
            // 
            // pnlFooterBusquedaPorExpediente
            // 
            pnlFooterBusquedaPorExpediente.Controls.Add(panel1);
            pnlFooterBusquedaPorExpediente.Controls.Add(btnCancelar);
            pnlFooterBusquedaPorExpediente.Dock = DockStyle.Bottom;
            pnlFooterBusquedaPorExpediente.Location = new Point(0, 466);
            pnlFooterBusquedaPorExpediente.Name = "pnlFooterBusquedaPorExpediente";
            pnlFooterBusquedaPorExpediente.Size = new Size(485, 49);
            pnlFooterBusquedaPorExpediente.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnBuscar);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(235, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 49);
            panel1.TabIndex = 2;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.FromArgb(12, 35, 30);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.Location = new Point(16, 4);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(218, 41);
            btnBuscar.TabIndex = 1;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.FromArgb(12, 35, 30);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(5, 5);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(218, 41);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // pnlMenuInicial
            // 
            pnlMenuInicial.BackColor = Color.FromArgb(12, 35, 30);
            pnlMenuInicial.Controls.Add(lblSistemaConsultaExpediente);
            pnlMenuInicial.Dock = DockStyle.Top;
            pnlMenuInicial.Location = new Point(0, 0);
            pnlMenuInicial.Name = "pnlMenuInicial";
            pnlMenuInicial.Size = new Size(485, 83);
            pnlMenuInicial.TabIndex = 3;
            // 
            // lblSistemaConsultaExpediente
            // 
            lblSistemaConsultaExpediente.Dock = DockStyle.Fill;
            lblSistemaConsultaExpediente.Font = new Font("Montserrat", 13.7999992F, FontStyle.Bold, GraphicsUnit.Point);
            lblSistemaConsultaExpediente.ForeColor = Color.White;
            lblSistemaConsultaExpediente.Location = new Point(0, 0);
            lblSistemaConsultaExpediente.Name = "lblSistemaConsultaExpediente";
            lblSistemaConsultaExpediente.Padding = new Padding(34, 0, 0, 0);
            lblSistemaConsultaExpediente.Size = new Size(485, 83);
            lblSistemaConsultaExpediente.TabIndex = 5;
            lblSistemaConsultaExpediente.Text = "Seleccione el expediente del cual se requiera buscar el detalle";
            lblSistemaConsultaExpediente.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dgvListaDeExpedientes
            // 
            dgvListaDeExpedientes.AllowUserToAddRows = false;
            dgvListaDeExpedientes.AllowUserToDeleteRows = false;
            dgvListaDeExpedientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListaDeExpedientes.Columns.AddRange(new DataGridViewColumn[] { colNumCredito });
            dgvListaDeExpedientes.Dock = DockStyle.Fill;
            dgvListaDeExpedientes.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvListaDeExpedientes.GridColor = Color.White;
            dgvListaDeExpedientes.Location = new Point(0, 83);
            dgvListaDeExpedientes.MultiSelect = false;
            dgvListaDeExpedientes.Name = "dgvListaDeExpedientes";
            dgvListaDeExpedientes.RowHeadersWidth = 51;
            dgvListaDeExpedientes.RowTemplate.Height = 29;
            dgvListaDeExpedientes.Size = new Size(485, 383);
            dgvListaDeExpedientes.TabIndex = 4;
            // 
            // colNumCredito
            // 
            colNumCredito.DataPropertyName = "NumCredito";
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            colNumCredito.DefaultCellStyle = dataGridViewCellStyle1;
            colNumCredito.HeaderText = "Numero de crédito";
            colNumCredito.MinimumWidth = 6;
            colNumCredito.Name = "colNumCredito";
            colNumCredito.ReadOnly = true;
            colNumCredito.Width = 250;
            // 
            // SeleccionaExpedienteFRM
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 515);
            Controls.Add(dgvListaDeExpedientes);
            Controls.Add(pnlMenuInicial);
            Controls.Add(pnlFooterBusquedaPorExpediente);
            Name = "SeleccionaExpedienteFRM";
            Text = "Selecciona un Expediente";
            pnlFooterBusquedaPorExpediente.ResumeLayout(false);
            panel1.ResumeLayout(false);
            pnlMenuInicial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvListaDeExpedientes).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlFooterBusquedaPorExpediente;
        private Panel panel1;
        private Button btnBuscar;
        private Button btnCancelar;
        private Panel pnlMenuInicial;
        private Label lblSistemaConsultaExpediente;
        private DataGridView dgvListaDeExpedientes;
        private DataGridViewTextBoxColumn colNumCredito;
    }
}