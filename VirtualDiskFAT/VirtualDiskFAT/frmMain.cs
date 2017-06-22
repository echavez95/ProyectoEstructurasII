using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualDiskFAT
{
    public partial class frmMain : Form
    {
        public static string discoDefault { get; set; }
        public frmMain()
        {
            InitializeComponent();

        }

        private void crearDiscoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgNuevoArchivo = new SaveFileDialog();
            dlgNuevoArchivo.RestoreDirectory = true;
            dlgNuevoArchivo.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (dlgNuevoArchivo.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(dlgNuevoArchivo.FileName))
                {
                    FileStream fs = new FileStream(dlgNuevoArchivo.FileName, FileMode.CreateNew);
                    fs.Seek(1024 * 1024 * 1024, SeekOrigin.Begin);
                    fs.WriteByte(0);
                    fs.Close();

                    MBR tablaMBR = new MBR();
                    tablaMBR.llenarDatosMBR();
                    using (BinaryWriter stream = new BinaryWriter(File.Open(dlgNuevoArchivo.FileName, FileMode.Open)))
                    {
                        //Escribir master boot record
                        stream.BaseStream.Position = 0;
                        stream.Write(tablaMBR.jumpInstruction,0, tablaMBR.jumpInstruction.Length);
                        stream.Write(tablaMBR.oemID, 0, tablaMBR.oemID.Length);

                        stream.Write(tablaMBR.bytesxSector);
                        stream.Write(tablaMBR.sectorxCluster);
                        stream.Write(tablaMBR.reservedSectors);
                        stream.Write(tablaMBR.numberOfFATs);
                        stream.Write(tablaMBR.rootEntries);
                        stream.Write(tablaMBR.smallSectors);
                        stream.Write(tablaMBR.mediaDescriptor);
                        stream.Write(tablaMBR.sectorxFATs);
                        stream.Write(tablaMBR.sectorxTrack);
                        stream.Write(tablaMBR.numberOfHeads);
                        stream.Write(tablaMBR.hiddenSectors);
                        stream.Write(tablaMBR.largeSectors);

                        stream.Write(tablaMBR.physicalDriveNo);
                        stream.Write(tablaMBR.reserved);
                        stream.Write(tablaMBR.extBootSignature);
                        stream.Write(tablaMBR.serialNo);
                        stream.Write(tablaMBR.volumeLabel);
                        stream.Write(tablaMBR.fileSystemType);

                        stream.Write(tablaMBR.bootstrapCode, 0, tablaMBR.bootstrapCode.Length);
                        stream.Write(tablaMBR.endOfSector);

                        //escribir tabla fat 2 veces
                        FAT16[] tablaFAT = new FAT16[65536];

                        for (int i = 0; i < 65536; i++)
                        {
                            tablaFAT[i] = new FAT16();
                            if (i <= 16)
                            {
                                tablaFAT[i].clusterReservado();
                            }
                            else
                            {
                                tablaFAT[i].clusterLibre();
                            }
                        }

                        for (int a = 0; a < 2; a++)
                        {
                            foreach (FAT16 fentry in tablaFAT)
                            {
                                stream.Write(fentry.entradaFAT);
                            }
                        }

                        //inicializar root directory
                        for(int i = 0; i < 512;i++)
                        {
                            Directory directorioVacio = new Directory();
                            stream.Write(directorioVacio.filename);
                            stream.Write(directorioVacio.filenameExt);
                            stream.Write(directorioVacio.fileAttributes);
                            stream.Write(directorioVacio.NT);
                            stream.Write(directorioVacio.millisegundos_Creado);
                            stream.Write(directorioVacio.hora_Creado);
                            stream.Write(directorioVacio.fecha_Creado);
                            stream.Write(directorioVacio.fecha_ultimoAcceso);
                            stream.Write(directorioVacio.reservedFAT32);
                            stream.Write(directorioVacio.hora_ultimaEscritura);
                            stream.Write(directorioVacio.fecha_ultimaEscritura);
                            stream.Write(directorioVacio.startingCluster);
                            stream.Write(directorioVacio.fileSize);
                        }
                    }
                    
                    frmMain.discoDefault= Path.GetFullPath(dlgNuevoArchivo.FileName);

                    MessageBox.Show("Creado Con Exito! Abra el explorador para usar el Disco",
                                    "Informacion",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("No se puede reemplazar el archivo!",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

            }

        }

        private void exploradorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (discoDefault != null)
            {
                if (!Application.OpenForms.OfType<frmExplorer>().Any())
                {
                    frmExplorer explorador = new frmExplorer();
                    explorador.MdiParent = this;
                    explorador.WindowState = FormWindowState.Maximized;
                    explorador.Show();
                }

            }
            else
            {
                MessageBox.Show("Cargue un disco desde el menu Archivo->Abrir Disco",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }


        }

        private void abrirDiscoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgAbrirArchivo = new OpenFileDialog();
            dlgAbrirArchivo.RestoreDirectory = true;
            dlgAbrirArchivo.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dlgAbrirArchivo.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dlgAbrirArchivo.FileName))
                {
                    discoDefault = Path.GetFullPath(dlgAbrirArchivo.FileName);
                }
                else
                {
                    MessageBox.Show("No se puede Abrir el archivo!",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

            }
        }
        public MBR FromByteArraytoObject(byte[] data)
        {
            if (data == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (MBR)obj;
            }
        }
        public byte[] objectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

       
    }
}
