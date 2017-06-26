using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualDiskFAT
{
    public partial class frmBusqueda : Form
    {
        public frmBusqueda()
        {
            InitializeComponent();
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            viewBusqueda.Items.Clear();
           List<Nodo> resultados = OperacionesArbol.buscarNombre(txtNombre.Text,frmExplorer.arbolIndice.raiz);
            foreach(Nodo n in resultados)
            {
                string[] itemInfo = { n.nombre};
                ListViewItem nod = new ListViewItem(itemInfo,0);
                nod.Tag = n.cluster + "," + n.ubicacion;
                viewBusqueda.Items.Add(nod);
            }
        }

        public  ListViewItem getItemSeleccionadoView()
        {
            ListViewItem seleccionado = viewBusqueda.SelectedItems[0];
            return seleccionado;
        }
        private void frmBusqueda_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewBusqueda.SelectedItems.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un archivo!",
                                       "Informacion",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                e.Cancel=true;
            }
        }
    }
}
