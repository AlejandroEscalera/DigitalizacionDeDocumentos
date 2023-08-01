namespace AppConsultaImagen.Dialogo
{
    partial class AvanceArchivosFRM
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
            lblDetalleEtiquetaAvance = new Label();
            prgAvance = new ProgressBar();
            SuspendLayout();
            // 
            // lblDetalleEtiquetaAvance
            // 
            lblDetalleEtiquetaAvance.AutoSize = true;
            lblDetalleEtiquetaAvance.Location = new Point(12, 9);
            lblDetalleEtiquetaAvance.Name = "lblDetalleEtiquetaAvance";
            lblDetalleEtiquetaAvance.Size = new Size(0, 20);
            lblDetalleEtiquetaAvance.TabIndex = 0;
            // 
            // prgAvance
            // 
            prgAvance.Location = new Point(12, 41);
            prgAvance.Name = "prgAvance";
            prgAvance.Size = new Size(638, 29);
            prgAvance.Step = 1;
            prgAvance.TabIndex = 1;
            // 
            // AvanceArchivosFRM
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(667, 82);
            Controls.Add(prgAvance);
            Controls.Add(lblDetalleEtiquetaAvance);
            Name = "AvanceArchivosFRM";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblDetalleEtiquetaAvance;
        private ProgressBar prgAvance;
    }
}