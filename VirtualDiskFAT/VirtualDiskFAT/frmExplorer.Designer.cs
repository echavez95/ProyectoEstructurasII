namespace VirtualDiskFAT
{
    partial class frmExplorer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExplorer));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFolder = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNuevaCarpeta = new System.Windows.Forms.ToolStripButton();
            this.btnAgregar = new System.Windows.Forms.ToolStripButton();
            this.btnExtraer = new System.Windows.Forms.ToolStripButton();
            this.btnBorrar = new System.Windows.Forms.ToolStripButton();
            this.btnBuscar = new System.Windows.Forms.ToolStripButton();
            this.viewGeneral = new System.Windows.Forms.ListView();
            this.Nombre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Fecha_Hora = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tamano = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listaIconos = new System.Windows.Forms.ImageList(this.components);
            this.lstPropiedadesDisco = new System.Windows.Forms.ListBox();
            this.lblDiscoDefault = new System.Windows.Forms.Label();
            this.btnBorrarCarpeta = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblFolder);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lstPropiedadesDisco);
            this.panel1.Controls.Add(this.lblDiscoDefault);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(846, 428);
            this.panel1.TabIndex = 1;
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(213, 29);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(0, 13);
            this.lblFolder.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.toolStrip1);
            this.panel2.Controls.Add(this.viewGeneral);
            this.panel2.Location = new System.Drawing.Point(213, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(630, 375);
            this.panel2.TabIndex = 6;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNuevaCarpeta,
            this.btnBorrarCarpeta,
            this.btnAgregar,
            this.btnExtraer,
            this.btnBorrar,
            this.btnBuscar});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(630, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNuevaCarpeta
            // 
            this.btnNuevaCarpeta.Image = global::VirtualDiskFAT.Properties.Resources.folder;
            this.btnNuevaCarpeta.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnNuevaCarpeta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNuevaCarpeta.Name = "btnNuevaCarpeta";
            this.btnNuevaCarpeta.Size = new System.Drawing.Size(105, 22);
            this.btnNuevaCarpeta.Text = "Nueva Carpeta";
            this.btnNuevaCarpeta.Click += new System.EventHandler(this.btnNuevaCarpeta_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Image = global::VirtualDiskFAT.Properties.Resources.file_add;
            this.btnAgregar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(113, 22);
            this.btnAgregar.Text = "Agregar Archivo";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnExtraer
            // 
            this.btnExtraer.Image = global::VirtualDiskFAT.Properties.Resources.file_download;
            this.btnExtraer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExtraer.Name = "btnExtraer";
            this.btnExtraer.Size = new System.Drawing.Size(106, 22);
            this.btnExtraer.Text = "Extraer Archivo";
            this.btnExtraer.Click += new System.EventHandler(this.btnExtraer_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.Image = global::VirtualDiskFAT.Properties.Resources.file_delete;
            this.btnBorrar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(103, 22);
            this.btnBorrar.Text = "Borrar Archivo";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Image = global::VirtualDiskFAT.Properties.Resources.glass2;
            this.btnBuscar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(62, 22);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // viewGeneral
            // 
            this.viewGeneral.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Nombre,
            this.Fecha_Hora,
            this.Tamano});
            this.viewGeneral.Location = new System.Drawing.Point(3, 28);
            this.viewGeneral.MultiSelect = false;
            this.viewGeneral.Name = "viewGeneral";
            this.viewGeneral.Size = new System.Drawing.Size(624, 344);
            this.viewGeneral.SmallImageList = this.listaIconos;
            this.viewGeneral.TabIndex = 8;
            this.viewGeneral.UseCompatibleStateImageBehavior = false;
            this.viewGeneral.View = System.Windows.Forms.View.Details;
            this.viewGeneral.DoubleClick += new System.EventHandler(this.viewGeneral_DoubleClick);
            // 
            // Nombre
            // 
            this.Nombre.Text = "Nombre";
            this.Nombre.Width = 134;
            // 
            // Fecha_Hora
            // 
            this.Fecha_Hora.Text = "Fecha Hora Creacion";
            this.Fecha_Hora.Width = 138;
            // 
            // Tamano
            // 
            this.Tamano.Text = "Tamaño";
            this.Tamano.Width = 104;
            // 
            // listaIconos
            // 
            this.listaIconos.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listaIconos.ImageStream")));
            this.listaIconos.TransparentColor = System.Drawing.Color.Transparent;
            this.listaIconos.Images.SetKeyName(0, "folder.png");
            this.listaIconos.Images.SetKeyName(1, "file.png");
            this.listaIconos.Images.SetKeyName(2, "harddisk.png");
            // 
            // lstPropiedadesDisco
            // 
            this.lstPropiedadesDisco.FormattingEnabled = true;
            this.lstPropiedadesDisco.Location = new System.Drawing.Point(12, 29);
            this.lstPropiedadesDisco.Name = "lstPropiedadesDisco";
            this.lstPropiedadesDisco.Size = new System.Drawing.Size(195, 342);
            this.lstPropiedadesDisco.TabIndex = 4;
            // 
            // lblDiscoDefault
            // 
            this.lblDiscoDefault.AutoSize = true;
            this.lblDiscoDefault.Location = new System.Drawing.Point(12, 9);
            this.lblDiscoDefault.Name = "lblDiscoDefault";
            this.lblDiscoDefault.Size = new System.Drawing.Size(37, 13);
            this.lblDiscoDefault.TabIndex = 2;
            this.lblDiscoDefault.Text = "Disco:";
            // 
            // btnBorrarCarpeta
            // 
            this.btnBorrarCarpeta.Image = global::VirtualDiskFAT.Properties.Resources.delete_folder;
            this.btnBorrarCarpeta.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorrarCarpeta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBorrarCarpeta.Name = "btnBorrarCarpeta";
            this.btnBorrarCarpeta.Size = new System.Drawing.Size(103, 22);
            this.btnBorrarCarpeta.Text = "Borrar Carpeta";
            this.btnBorrarCarpeta.Click += new System.EventHandler(this.btnBorrarCarpeta_Click);
            // 
            // frmExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 428);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "frmExplorer";
            this.ShowInTaskbar = false;
            this.Text = "Explorador de Disco";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmExplorer_FormClosing);
            this.Load += new System.EventHandler(this.frmExplorer_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDiscoDefault;
        private System.Windows.Forms.ListBox lstPropiedadesDisco;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ImageList listaIconos;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNuevaCarpeta;
        private System.Windows.Forms.ListView viewGeneral;
        private System.Windows.Forms.ColumnHeader Nombre;
        private System.Windows.Forms.ColumnHeader Fecha_Hora;
        private System.Windows.Forms.ColumnHeader Tamano;
        private System.Windows.Forms.ToolStripButton btnAgregar;
        private System.Windows.Forms.ToolStripButton btnExtraer;
        private System.Windows.Forms.ToolStripButton btnBorrar;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.ToolStripButton btnBuscar;
        private System.Windows.Forms.ToolStripButton btnBorrarCarpeta;
    }
}