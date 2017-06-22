namespace VirtualDiskFAT
{
    partial class frmMain
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
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crearDiscoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirDiscoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploradorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.exploradorToolStripMenuItem});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(927, 24);
            this.menuPrincipal.TabIndex = 0;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.crearDiscoToolStripMenuItem,
            this.abrirDiscoToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.archivoToolStripMenuItem.Text = "Archivo";
            // 
            // crearDiscoToolStripMenuItem
            // 
            this.crearDiscoToolStripMenuItem.Name = "crearDiscoToolStripMenuItem";
            this.crearDiscoToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.crearDiscoToolStripMenuItem.Text = "Crear Disco";
            this.crearDiscoToolStripMenuItem.Click += new System.EventHandler(this.crearDiscoToolStripMenuItem_Click);
            // 
            // abrirDiscoToolStripMenuItem
            // 
            this.abrirDiscoToolStripMenuItem.Name = "abrirDiscoToolStripMenuItem";
            this.abrirDiscoToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.abrirDiscoToolStripMenuItem.Text = "Abrir Disco";
            this.abrirDiscoToolStripMenuItem.Click += new System.EventHandler(this.abrirDiscoToolStripMenuItem_Click);
            // 
            // exploradorToolStripMenuItem
            // 
            this.exploradorToolStripMenuItem.Name = "exploradorToolStripMenuItem";
            this.exploradorToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.exploradorToolStripMenuItem.Text = "Explorador";
            this.exploradorToolStripMenuItem.Click += new System.EventHandler(this.exploradorToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 411);
            this.Controls.Add(this.menuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Virtual Disk";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crearDiscoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirDiscoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploradorToolStripMenuItem;
    }
}

