namespace VirtualDiskFAT
{
    partial class frmBusqueda
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBusqueda));
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.viewBusqueda = new System.Windows.Forms.ListView();
            this.btnExtraerArchivo = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre del Archivo:";
            // 
            // txtNombre
            // 
            this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(121, 9);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(192, 21);
            this.txtNombre.TabIndex = 1;
            this.txtNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNombre_KeyPress);
            // 
            // viewBusqueda
            // 
            this.viewBusqueda.LargeImageList = this.imageList1;
            this.viewBusqueda.Location = new System.Drawing.Point(15, 36);
            this.viewBusqueda.MultiSelect = false;
            this.viewBusqueda.Name = "viewBusqueda";
            this.viewBusqueda.Size = new System.Drawing.Size(298, 186);
            this.viewBusqueda.TabIndex = 4;
            this.viewBusqueda.UseCompatibleStateImageBehavior = false;
            this.viewBusqueda.Click += new System.EventHandler(this.viewBusqueda_Click);
            // 
            // btnExtraerArchivo
            // 
            this.btnExtraerArchivo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExtraerArchivo.Image = global::VirtualDiskFAT.Properties.Resources.file_download;
            this.btnExtraerArchivo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExtraerArchivo.Location = new System.Drawing.Point(319, 9);
            this.btnExtraerArchivo.Name = "btnExtraerArchivo";
            this.btnExtraerArchivo.Size = new System.Drawing.Size(71, 52);
            this.btnExtraerArchivo.TabIndex = 3;
            this.btnExtraerArchivo.Text = "Guardar";
            this.btnExtraerArchivo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExtraerArchivo.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Image = global::VirtualDiskFAT.Properties.Resources.cancel;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelar.Location = new System.Drawing.Point(319, 67);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(71, 49);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "document.png");
            // 
            // frmBusqueda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 227);
            this.ControlBox = false;
            this.Controls.Add(this.viewBusqueda);
            this.Controls.Add(this.btnExtraerArchivo);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmBusqueda";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBusqueda_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnExtraerArchivo;
        private System.Windows.Forms.ListView viewBusqueda;
        private System.Windows.Forms.ImageList imageList1;
    }
}