namespace AppCargaImagenes
{
    partial class MainFRM
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tbcCargaDeInformacion = new TabControl();
            tbpCargaLiquidados = new TabPage();
            pnlSeleccionTop = new Panel();
            btnSeleccionaTemporal = new Button();
            txtDirectorioTemporal = new TextBox();
            label2 = new Label();
            btnCargaInformacion = new Button();
            btnSeleccionaCarpeta = new Button();
            txtCarpetaArchivo = new TextBox();
            lblSeleccionCarpeta = new Label();
            tabPage2 = new TabPage();
            panel1 = new Panel();
            btnSeleccionaCarpetaGuardaValores = new Button();
            txtDirectorioCargaGuardaValores = new TextBox();
            label1 = new Label();
            btnCargaInformacionGuardaValores = new Button();
            txtUnidadTemporal = new TextBox();
            label3 = new Label();
            pnlInferior = new Panel();
            pnlDerecho = new Panel();
            btnSalir = new Button();
            fbdSeleccionaCarpetaImagenes = new FolderBrowserDialog();
            fbdSeleccionaTemporal = new FolderBrowserDialog();
            pnlReportaAvances = new Panel();
            prgArchivo = new ProgressBar();
            prgSubProceso = new ProgressBar();
            lblArchivo = new Label();
            lblArchivoEtiqueta = new Label();
            lblSubProceso = new Label();
            lblSubProcesoEtiqueta = new Label();
            prgEtapa = new ProgressBar();
            lblEtapa = new Label();
            lblEtapaEtiqueta = new Label();
            tbpCargaExpedientes = new TabPage();
            panel2 = new Panel();
            btnSekeccionDirectorioCargaExpedientes = new Button();
            txtDirectorioCargaExpedientes = new TextBox();
            label4 = new Label();
            btnCargaInformacionExpedientes = new Button();
            txtUnidadTemporalExpedientes = new TextBox();
            label5 = new Label();
            tbcCargaDeInformacion.SuspendLayout();
            tbpCargaLiquidados.SuspendLayout();
            pnlSeleccionTop.SuspendLayout();
            tabPage2.SuspendLayout();
            panel1.SuspendLayout();
            pnlInferior.SuspendLayout();
            pnlDerecho.SuspendLayout();
            pnlReportaAvances.SuspendLayout();
            tbpCargaExpedientes.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tbcCargaDeInformacion
            // 
            tbcCargaDeInformacion.Appearance = TabAppearance.Buttons;
            tbcCargaDeInformacion.Controls.Add(tbpCargaLiquidados);
            tbcCargaDeInformacion.Controls.Add(tabPage2);
            tbcCargaDeInformacion.Controls.Add(tbpCargaExpedientes);
            tbcCargaDeInformacion.Dock = DockStyle.Top;
            tbcCargaDeInformacion.Location = new Point(0, 0);
            tbcCargaDeInformacion.Name = "tbcCargaDeInformacion";
            tbcCargaDeInformacion.SelectedIndex = 0;
            tbcCargaDeInformacion.Size = new Size(1180, 113);
            tbcCargaDeInformacion.TabIndex = 0;
            // 
            // tbpCargaLiquidados
            // 
            tbpCargaLiquidados.Controls.Add(pnlSeleccionTop);
            tbpCargaLiquidados.Location = new Point(4, 32);
            tbpCargaLiquidados.Name = "tbpCargaLiquidados";
            tbpCargaLiquidados.Padding = new Padding(3);
            tbpCargaLiquidados.Size = new Size(1172, 77);
            tbpCargaLiquidados.TabIndex = 0;
            tbpCargaLiquidados.Text = "Liquidados";
            tbpCargaLiquidados.UseVisualStyleBackColor = true;
            // 
            // pnlSeleccionTop
            // 
            pnlSeleccionTop.Controls.Add(btnSeleccionaTemporal);
            pnlSeleccionTop.Controls.Add(txtDirectorioTemporal);
            pnlSeleccionTop.Controls.Add(label2);
            pnlSeleccionTop.Controls.Add(btnCargaInformacion);
            pnlSeleccionTop.Controls.Add(btnSeleccionaCarpeta);
            pnlSeleccionTop.Controls.Add(txtCarpetaArchivo);
            pnlSeleccionTop.Controls.Add(lblSeleccionCarpeta);
            pnlSeleccionTop.Dock = DockStyle.Top;
            pnlSeleccionTop.Location = new Point(3, 3);
            pnlSeleccionTop.Name = "pnlSeleccionTop";
            pnlSeleccionTop.Size = new Size(1166, 80);
            pnlSeleccionTop.TabIndex = 0;
            // 
            // btnSeleccionaTemporal
            // 
            btnSeleccionaTemporal.Location = new Point(904, 42);
            btnSeleccionaTemporal.Name = "btnSeleccionaTemporal";
            btnSeleccionaTemporal.Size = new Size(31, 29);
            btnSeleccionaTemporal.TabIndex = 6;
            btnSeleccionaTemporal.Text = "...";
            btnSeleccionaTemporal.UseVisualStyleBackColor = true;
            btnSeleccionaTemporal.Click += BtnSeleccionaTemporalClick;
            // 
            // txtDirectorioTemporal
            // 
            txtDirectorioTemporal.Location = new Point(254, 42);
            txtDirectorioTemporal.Name = "txtDirectorioTemporal";
            txtDirectorioTemporal.Size = new Size(644, 27);
            txtDirectorioTemporal.TabIndex = 5;
            txtDirectorioTemporal.Text = "C:\\OD\\OneDrive - FND\\Documentos - PF1 CANCELADOS\\LIQUIDADOS";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(100, 46);
            label2.Name = "label2";
            label2.Size = new Size(148, 20);
            label2.TabIndex = 4;
            label2.Text = "Directorio temporal :";
            // 
            // btnCargaInformacion
            // 
            btnCargaInformacion.Location = new Point(941, 42);
            btnCargaInformacion.Name = "btnCargaInformacion";
            btnCargaInformacion.Size = new Size(166, 29);
            btnCargaInformacion.TabIndex = 1;
            btnCargaInformacion.Text = "Carga Imagenes";
            btnCargaInformacion.UseVisualStyleBackColor = true;
            btnCargaInformacion.Visible = false;
            btnCargaInformacion.Click += BtnCargaImagenesClick;
            // 
            // btnSeleccionaCarpeta
            // 
            btnSeleccionaCarpeta.Location = new Point(1076, 7);
            btnSeleccionaCarpeta.Name = "btnSeleccionaCarpeta";
            btnSeleccionaCarpeta.Size = new Size(31, 29);
            btnSeleccionaCarpeta.TabIndex = 3;
            btnSeleccionaCarpeta.Text = "...";
            btnSeleccionaCarpeta.UseVisualStyleBackColor = true;
            btnSeleccionaCarpeta.Click += BtnSeleccionaCarpeta;
            // 
            // txtCarpetaArchivo
            // 
            txtCarpetaArchivo.Location = new Point(254, 7);
            txtCarpetaArchivo.Name = "txtCarpetaArchivo";
            txtCarpetaArchivo.Size = new Size(816, 27);
            txtCarpetaArchivo.TabIndex = 1;
            txtCarpetaArchivo.Text = "C:\\OD\\OneDrive - FND\\Documentos - PF1 CANCELADOS\\LIQUIDADOS";
            // 
            // lblSeleccionCarpeta
            // 
            lblSeleccionCarpeta.AutoSize = true;
            lblSeleccionCarpeta.Location = new Point(15, 10);
            lblSeleccionCarpeta.Name = "lblSeleccionCarpeta";
            lblSeleccionCarpeta.Size = new Size(233, 20);
            lblSeleccionCarpeta.TabIndex = 0;
            lblSeleccionCarpeta.Text = "Selección de carpeta de archivos :";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(panel1);
            tabPage2.Location = new Point(4, 32);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1172, 77);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Carga de Guarda Valores";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnSeleccionaCarpetaGuardaValores);
            panel1.Controls.Add(txtDirectorioCargaGuardaValores);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnCargaInformacionGuardaValores);
            panel1.Controls.Add(txtUnidadTemporal);
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1166, 80);
            panel1.TabIndex = 1;
            // 
            // button1
            // 
            btnSeleccionaCarpetaGuardaValores.Location = new Point(904, 42);
            btnSeleccionaCarpetaGuardaValores.Name = "button1";
            btnSeleccionaCarpetaGuardaValores.Size = new Size(31, 29);
            btnSeleccionaCarpetaGuardaValores.TabIndex = 6;
            btnSeleccionaCarpetaGuardaValores.Text = "...";
            btnSeleccionaCarpetaGuardaValores.UseVisualStyleBackColor = true;            
            // 
            // txtDirectorioCargaGuardaValores
            // 
            txtDirectorioCargaGuardaValores.Location = new Point(143, 42);
            txtDirectorioCargaGuardaValores.Name = "txtDirectorioCargaGuardaValores";
            txtDirectorioCargaGuardaValores.Size = new Size(754, 27);
            txtDirectorioCargaGuardaValores.TabIndex = 5;
            txtDirectorioCargaGuardaValores.Text = "C:\\OD\\FND\\Documentos Valor - Noroeste";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 46);
            label1.Name = "label1";
            label1.Size = new Size(124, 20);
            label1.TabIndex = 4;
            label1.Text = "Directorio carga :";
            // 
            // btnCargaInformacionGuardaValores
            // 
            btnCargaInformacionGuardaValores.Location = new Point(988, 42);
            btnCargaInformacionGuardaValores.Name = "btnCargaInformacionGuardaValores";
            btnCargaInformacionGuardaValores.Size = new Size(166, 29);
            btnCargaInformacionGuardaValores.TabIndex = 1;
            btnCargaInformacionGuardaValores.Text = "Carga Imagenes";
            btnCargaInformacionGuardaValores.UseVisualStyleBackColor = true;
            btnCargaInformacionGuardaValores.Click += BtnCargaImagenesGuardaValoresClick;
            // 
            // txtUnidadTemporal
            // 
            txtUnidadTemporal.Location = new Point(143, 7);
            txtUnidadTemporal.Name = "txtUnidadTemporal";
            txtUnidadTemporal.Size = new Size(34, 27);
            txtUnidadTemporal.TabIndex = 1;
            txtUnidadTemporal.Text = "Z:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 10);
            label3.Name = "label3";
            label3.Size = new Size(129, 20);
            label3.TabIndex = 0;
            label3.Text = "Unidad temporal :";
            // 
            // pnlInferior
            // 
            pnlInferior.Controls.Add(pnlDerecho);
            pnlInferior.Dock = DockStyle.Bottom;
            pnlInferior.Location = new Point(0, 311);
            pnlInferior.Name = "pnlInferior";
            pnlInferior.Size = new Size(1180, 54);
            pnlInferior.TabIndex = 1;
            // 
            // pnlDerecho
            // 
            pnlDerecho.Controls.Add(btnSalir);
            pnlDerecho.Dock = DockStyle.Right;
            pnlDerecho.Location = new Point(930, 0);
            pnlDerecho.Name = "pnlDerecho";
            pnlDerecho.Size = new Size(250, 54);
            pnlDerecho.TabIndex = 1;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(144, 15);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(94, 29);
            btnSalir.TabIndex = 0;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += BtnSalirClick;
            // 
            // fbdSeleccionaCarpetaImagenes
            // 
            fbdSeleccionaCarpetaImagenes.Description = "Selecciona la carpeta de donde se cargarán las imágenes";
            fbdSeleccionaCarpetaImagenes.InitialDirectory = "C:\\OD\\OneDrive - FND\\Documentos - PF1 CANCELADOS\\LIQUIDADOS";
            fbdSeleccionaCarpetaImagenes.SelectedPath = "C:\\OD\\OneDrive - FND\\Documentos - PF1 CANCELADOS\\LIQUIDADOS";
            // 
            // fbdSeleccionaTemporal
            // 
            fbdSeleccionaTemporal.Description = "Selecciona Carpeta Temporal";
            // 
            // pnlReportaAvances
            // 
            pnlReportaAvances.Controls.Add(prgArchivo);
            pnlReportaAvances.Controls.Add(prgSubProceso);
            pnlReportaAvances.Controls.Add(lblArchivo);
            pnlReportaAvances.Controls.Add(lblArchivoEtiqueta);
            pnlReportaAvances.Controls.Add(lblSubProceso);
            pnlReportaAvances.Controls.Add(lblSubProcesoEtiqueta);
            pnlReportaAvances.Controls.Add(prgEtapa);
            pnlReportaAvances.Controls.Add(lblEtapa);
            pnlReportaAvances.Controls.Add(lblEtapaEtiqueta);
            pnlReportaAvances.Dock = DockStyle.Fill;
            pnlReportaAvances.Location = new Point(0, 113);
            pnlReportaAvances.Name = "pnlReportaAvances";
            pnlReportaAvances.Size = new Size(1180, 198);
            pnlReportaAvances.TabIndex = 2;
            pnlReportaAvances.Visible = false;
            // 
            // prgArchivo
            // 
            prgArchivo.Location = new Point(15, 159);
            prgArchivo.Name = "prgArchivo";
            prgArchivo.Size = new Size(1146, 29);
            prgArchivo.Step = 1;
            prgArchivo.TabIndex = 8;
            // 
            // prgSubProceso
            // 
            prgSubProceso.Location = new Point(15, 104);
            prgSubProceso.Name = "prgSubProceso";
            prgSubProceso.Size = new Size(1146, 29);
            prgSubProceso.Step = 1;
            prgSubProceso.TabIndex = 7;
            // 
            // lblArchivo
            // 
            lblArchivo.AutoSize = true;
            lblArchivo.Location = new Point(80, 136);
            lblArchivo.Name = "lblArchivo";
            lblArchivo.Size = new Size(0, 20);
            lblArchivo.TabIndex = 6;
            // 
            // lblArchivoEtiqueta
            // 
            lblArchivoEtiqueta.AutoSize = true;
            lblArchivoEtiqueta.Location = new Point(15, 136);
            lblArchivoEtiqueta.Name = "lblArchivoEtiqueta";
            lblArchivoEtiqueta.Size = new Size(62, 20);
            lblArchivoEtiqueta.TabIndex = 5;
            lblArchivoEtiqueta.Text = "Archivo:";
            // 
            // lblSubProceso
            // 
            lblSubProceso.AutoSize = true;
            lblSubProceso.Location = new Point(150, 81);
            lblSubProceso.Name = "lblSubProceso";
            lblSubProceso.Size = new Size(0, 20);
            lblSubProceso.TabIndex = 4;
            // 
            // lblSubProcesoEtiqueta
            // 
            lblSubProcesoEtiqueta.AutoSize = true;
            lblSubProcesoEtiqueta.Location = new Point(15, 81);
            lblSubProcesoEtiqueta.Name = "lblSubProcesoEtiqueta";
            lblSubProcesoEtiqueta.Size = new Size(130, 20);
            lblSubProcesoEtiqueta.TabIndex = 3;
            lblSubProcesoEtiqueta.Text = "Etapa subproceso:";
            // 
            // prgEtapa
            // 
            prgEtapa.Location = new Point(15, 49);
            prgEtapa.Name = "prgEtapa";
            prgEtapa.Size = new Size(1146, 29);
            prgEtapa.Step = 1;
            prgEtapa.TabIndex = 2;
            // 
            // lblEtapa
            // 
            lblEtapa.AutoSize = true;
            lblEtapa.Location = new Point(71, 14);
            lblEtapa.Name = "lblEtapa";
            lblEtapa.Size = new Size(0, 20);
            lblEtapa.TabIndex = 1;
            // 
            // lblEtapaEtiqueta
            // 
            lblEtapaEtiqueta.AutoSize = true;
            lblEtapaEtiqueta.Location = new Point(15, 14);
            lblEtapaEtiqueta.Name = "lblEtapaEtiqueta";
            lblEtapaEtiqueta.Size = new Size(50, 20);
            lblEtapaEtiqueta.TabIndex = 0;
            lblEtapaEtiqueta.Text = "Etapa:";
            // 
            // tbpCargaExpedientes
            // 
            tbpCargaExpedientes.Controls.Add(panel2);
            tbpCargaExpedientes.Location = new Point(4, 32);
            tbpCargaExpedientes.Name = "tbpCargaExpedientes";
            tbpCargaExpedientes.Padding = new Padding(3);
            tbpCargaExpedientes.Size = new Size(1172, 77);
            tbpCargaExpedientes.TabIndex = 2;
            tbpCargaExpedientes.Text = "Carga expedientes";
            tbpCargaExpedientes.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnSekeccionDirectorioCargaExpedientes);
            panel2.Controls.Add(txtDirectorioCargaExpedientes);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(btnCargaInformacionExpedientes);
            panel2.Controls.Add(txtUnidadTemporalExpedientes);
            panel2.Controls.Add(label5);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(1166, 80);
            panel2.TabIndex = 2;
            // 
            // btnSekeccionDirectorioCargaExpedientes
            // 
            btnSekeccionDirectorioCargaExpedientes.Location = new Point(904, 42);
            btnSekeccionDirectorioCargaExpedientes.Name = "btnSekeccionDirectorioCargaExpedientes";
            btnSekeccionDirectorioCargaExpedientes.Size = new Size(31, 29);
            btnSekeccionDirectorioCargaExpedientes.TabIndex = 6;
            btnSekeccionDirectorioCargaExpedientes.Text = "...";
            btnSekeccionDirectorioCargaExpedientes.UseVisualStyleBackColor = true;
            // 
            // txtDirectorioCargaExpedientes
            // 
            txtDirectorioCargaExpedientes.Location = new Point(143, 42);
            txtDirectorioCargaExpedientes.Name = "txtDirectorioCargaExpedientes";
            txtDirectorioCargaExpedientes.Size = new Size(754, 27);
            txtDirectorioCargaExpedientes.TabIndex = 5;
            txtDirectorioCargaExpedientes.Text = "C:\\OD\\FND\\Documentos Valor - Noroeste";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 46);
            label4.Name = "label4";
            label4.Size = new Size(124, 20);
            label4.TabIndex = 4;
            label4.Text = "Directorio carga :";
            // 
            // btnCargaInformacionExpedientes
            // 
            btnCargaInformacionExpedientes.Location = new Point(988, 42);
            btnCargaInformacionExpedientes.Name = "btnCargaInformacionExpedientes";
            btnCargaInformacionExpedientes.Size = new Size(166, 29);
            btnCargaInformacionExpedientes.TabIndex = 1;
            btnCargaInformacionExpedientes.Text = "Carga Imagenes";
            btnCargaInformacionExpedientes.UseVisualStyleBackColor = true;
            // 
            // txtUnidadTemporalExpedientes
            // 
            txtUnidadTemporalExpedientes.Location = new Point(143, 7);
            txtUnidadTemporalExpedientes.Name = "txtUnidadTemporalExpedientes";
            txtUnidadTemporalExpedientes.Size = new Size(34, 27);
            txtUnidadTemporalExpedientes.TabIndex = 1;
            txtUnidadTemporalExpedientes.Text = "Z:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(15, 10);
            label5.Name = "label5";
            label5.Size = new Size(129, 20);
            label5.TabIndex = 0;
            label5.Text = "Unidad temporal :";
            // 
            // MainFRM
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1180, 365);
            Controls.Add(pnlReportaAvances);
            Controls.Add(pnlInferior);
            Controls.Add(tbcCargaDeInformacion);
            Name = "MainFRM";
            Text = "Carga de Información";
            Load += MainFRM_Load;
            tbcCargaDeInformacion.ResumeLayout(false);
            tbpCargaLiquidados.ResumeLayout(false);
            pnlSeleccionTop.ResumeLayout(false);
            pnlSeleccionTop.PerformLayout();
            tabPage2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            pnlInferior.ResumeLayout(false);
            pnlDerecho.ResumeLayout(false);
            pnlReportaAvances.ResumeLayout(false);
            pnlReportaAvances.PerformLayout();
            tbpCargaExpedientes.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tbcCargaDeInformacion;
        private TabPage tbpCargaLiquidados;
        private TabPage tabPage2;
        private Panel pnlInferior;
        private Panel pnlDerecho;
        private Button btnSalir;
        private Panel pnlSeleccionTop;
        private Label lblSeleccionCarpeta;
        private Button btnSeleccionaCarpeta;
        private TextBox txtCarpetaArchivo;
        private FolderBrowserDialog fbdSeleccionaCarpetaImagenes;
        private Button btnCargaInformacion;
        private Label label2;
        private Button btnSeleccionaTemporal;
        private TextBox txtDirectorioTemporal;
        private FolderBrowserDialog fbdSeleccionaTemporal;
        private Panel pnlReportaAvances;
        private ProgressBar prgArchivo;
        private ProgressBar prgSubProceso;
        private Label lblArchivo;
        private Label lblArchivoEtiqueta;
        private Label lblSubProceso;
        private Label lblSubProcesoEtiqueta;
        private ProgressBar prgEtapa;
        private Label lblEtapa;
        private Label lblEtapaEtiqueta;
        private Panel panel1;
        private Button btnSeleccionaCarpetaGuardaValores;
        private TextBox txtDirectorioCargaGuardaValores;
        private Label label1;
        private Button btnCargaInformacionGuardaValores;
        private TextBox txtUnidadTemporal;
        private Label label3;
        private TabPage tbpCargaExpedientes;
        private Panel panel2;
        private Button btnSekeccionDirectorioCargaExpedientes;
        private TextBox txtDirectorioCargaExpedientes;
        private Label label4;
        private Button btnCargaInformacionExpedientes;
        private TextBox txtUnidadTemporalExpedientes;
        private Label label5;
    }
}