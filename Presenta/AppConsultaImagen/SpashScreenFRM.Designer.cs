namespace AppConsultaImagen
{
    partial class SpashScreenFRM
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
            components = new System.ComponentModel.Container();
            imgLogo = new PictureBox();
            prgProgressBar = new ProgressBar();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)imgLogo).BeginInit();
            SuspendLayout();
            // 
            // imgLogo
            // 
            imgLogo.Dock = DockStyle.Fill;
            imgLogo.Image = Properties.Resources.LogoFND;
            imgLogo.Location = new Point(0, 0);
            imgLogo.Name = "imgLogo";
            imgLogo.Size = new Size(1777, 749);
            imgLogo.TabIndex = 0;
            imgLogo.TabStop = false;
            // 
            // prgProgressBar
            // 
            prgProgressBar.Dock = DockStyle.Bottom;
            prgProgressBar.Location = new Point(0, 720);
            prgProgressBar.Maximum = 101;
            prgProgressBar.Name = "prgProgressBar";
            prgProgressBar.Size = new Size(1777, 29);
            prgProgressBar.Step = 1;
            prgProgressBar.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            // 
            // SpashScreenFRM
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1777, 749);
            Controls.Add(prgProgressBar);
            Controls.Add(imgLogo);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SpashScreenFRM";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "SpashScreen";
            ((System.ComponentModel.ISupportInitialize)imgLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox imgLogo;
        private ProgressBar prgProgressBar;
        private System.Windows.Forms.Timer timer1;
    }
}