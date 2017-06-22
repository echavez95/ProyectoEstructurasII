using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace VirtualDiskFAT
{
    public partial class frmExplorer : Form
    {
        //public FAT16[] tablaFAT = new FAT16[65536];
        public List<FAT16> fat1List = new List<FAT16>();
        public List<FAT16> fat2List = new List<FAT16>();
        public List<Directory> listaRootDirectory = new List<Directory>();
        public List<Directory> listaDirectorioActual = new List<Directory>();
        Directory carpetaActual = new Directory();
        long posicionCarpetaActual = new long();
        public decodedMBR tablaMBR = new decodedMBR();
        public int mbrOffset = 512;
        public long inicioTablaFat1 = new long();
        public long inicioTablaFat2 = new long();
        public bool viewRootDirectory = new bool(); //variable que controla si lo que esta en el viewgeneral es el root directory
        public int tamanioCluster = new int();
        public frmExplorer()
        {
            InitializeComponent();

        }

        private void frmExplorer_Load(object sender, EventArgs e)
        {
            if (frmMain.discoDefault == null)
            {
                MessageBox.Show("Abra un disco o cree un nuevo desde el menu archivo",
                                   "Error",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                lblDiscoDefault.Text = "Disco: " + frmMain.discoDefault;
                leerInfoDisco();
                leerTablasFat();
                leerRootDirectory();
                tamanioCluster = tablaMBR.bytesxSector * tablaMBR.sectorxCluster;
                cargarViewGeneral(true);
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgAbrirArchivo = new OpenFileDialog();
            byte[] archivo;

            if (dlgAbrirArchivo.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dlgAbrirArchivo.FileName))
                {
                    using (FileStream stream = File.OpenRead(dlgAbrirArchivo.FileName))
                    {
                        archivo = new byte[stream.Length];
                        stream.Read(archivo, 0, archivo.Length);
                    }
                    Directory entradaArchivo = new Directory();
                    FileInfo info = new FileInfo(dlgAbrirArchivo.FileName);
                    uint filesize = (uint)info.Length;

                    entradaArchivo.nuevoArchivo(info.Name, 'A', DateTime.Now, 0, filesize);

                    guardarArchivoEnRoot(entradaArchivo, archivo);
                }
            }
        }
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            string seleccionado = viewGeneral.SelectedItems[0].Text;
            if (viewRootDirectory)
            {
                byte[] archivo = sacarArchivoDeRoot(seleccionado);

                SaveFileDialog dlgGuardarArchivo = new SaveFileDialog();
                dlgGuardarArchivo.FileName = seleccionado;
                if (dlgGuardarArchivo.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(dlgGuardarArchivo.FileName))
                    {
                        //File.WriteAllBytes(dlgGuardarArchivo.FileName, archivo);

                        using (Stream file = File.OpenWrite(dlgGuardarArchivo.FileName))
                        {
                            file.Write(archivo, 0, archivo.Length);
                        }
                        
                        MessageBox.Show("Creado Con Exito!",
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
        }

        private void btnSacar_Click(object sender, EventArgs e)
        {


        }

        private void btnNuevaCarpeta_Click(object sender, EventArgs e)
        {
            string nombreCarpeta;
            do
            {
                nombreCarpeta = Interaction.InputBox("Nombre", "Nueva Carpeta");
                if(nombreCarpeta.Length > 0 && nombreCarpeta.Length <= 8)
                {
                    break;
                }
                else
                {
                    MessageBox.Show("El nombre debe ser menor o igual a 8 caracteres"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Exclamation);
                }
            } while (true);

            if (viewRootDirectory)
            {
                int existecarpeta = 0;
                foreach (var v in listaRootDirectory)
                {
                    string a = Encoding.ASCII.GetString(v.filename);
                    string nombre = a.Replace("\0", string.Empty);
                    if (nombre == nombreCarpeta)
                    {
                        existecarpeta = 1;
                    }
                }

                if (existecarpeta > 0)
                {
                    MessageBox.Show("Ya existe una carpeta con ese nombre!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
                }
                else
                {
                    Directory Carpeta = new Directory();
                    Carpeta.nuevaCarpeta(null,nombreCarpeta, DateTime.Now);
                    crearCarpetaEnRoot(Carpeta);

                    leerRootDirectory();
                    cargarViewGeneral(true);
                }
            }else
            {
                int existecarpeta = 0;
                foreach (var v in listaDirectorioActual)
                {
                    string a = Encoding.ASCII.GetString(v.filename);
                    string nombre = a.Replace("\0", string.Empty);
                    if (nombre == nombreCarpeta)
                    {
                        existecarpeta = 1;
                    }
                }

                if (existecarpeta > 0)
                {
                    MessageBox.Show("Ya existe una carpeta con ese nombre!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
                }
                else
                {
                    Directory Carpeta = new Directory();
                    Carpeta.nuevaCarpeta(carpetaActual,nombreCarpeta, DateTime.Now);
                    crearCarpeta(Carpeta);
                    actualizarCarpetaActual();
                    cargarViewGeneral(false);
                }
            }
            
           
        }

        private void btnBorrarCarpeta_Click(object sender, EventArgs e)
        {
            if (viewGeneral.SelectedItems.Count == 0 || viewGeneral.SelectedItems.Count > 1)
            {
                MessageBox.Show("Seleccione una carpeta"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }else
            {
                string nombre = viewGeneral.SelectedItems[0].Text;
                borrarCarpetaEnRoot(nombre);
                MessageBox.Show("Carpeta Borrada"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Exclamation);
            }
            leerRootDirectory();
            cargarViewGeneral(true);
        }
        
        private void viewGeneral_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem seleccionado = viewGeneral.SelectedItems[0];

            string[] prop = seleccionado.Tag.ToString().Split(',');
            if (prop[0] == "D") //obtiene datos del directorio guardados en el listviewitem, si es una carpeta entonces
            {
                if (viewRootDirectory) //si estoy en el root 
                {
                    string nombreCarpeta = seleccionado.SubItems[0].Text;
                    
                    Directory carpeta = listaRootDirectory.Where(x => Encoding.ASCII.GetString(x.filename) == nombreCarpeta).FirstOrDefault();
                   
                    carpetaActual = leerEntradaDirectorio(posicionByteCluster(carpeta.startingCluster));

                    actualizarCarpetaActual();

                    lblFolder.Text = "Carpeta Actual: " + Encoding.ASCII.GetString(carpetaActual.filename);

                    cargarViewGeneral(false);
                }else
                {
                    if (seleccionado == viewGeneral.Items[0])
                    {
                        if (carpetaActual.Padre == null)
                        {
                            carpetaActual = new Directory();
                            lblFolder.Text = "Carpeta Actual: Root";
                            leerRootDirectory();
                            cargarViewGeneral(true);
                        }
                        else
                        {
                            carpetaActual = carpetaActual.Padre;
                            actualizarCarpetaActual();
                            lblFolder.Text = "Carpeta Actual: " + Encoding.ASCII.GetString(carpetaActual.filename);
                            cargarViewGeneral(false);
                        }
                    }
                    else
                    {
                        string nombreCarpeta = seleccionado.SubItems[0].Text;
                        Directory carpeta = listaDirectorioActual.Where(x => Encoding.ASCII.GetString(x.filename) == nombreCarpeta).FirstOrDefault();
                        
                        carpetaActual = leerEntradaDirectorio(posicionByteCluster(carpeta.Padre.startingCluster));
                        actualizarCarpetaActual();
                        lblFolder.Text = "Carpeta Actual: " + Encoding.ASCII.GetString(carpetaActual.filename);
                        cargarViewGeneral(false);
                    }
                }

            }
        }

        #region //-------- CARGAR INFORMACION DE DISCO ---------
        public void leerInfoDisco()
        {
            byte[] temporalArray;
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                temporalArray = reader.ReadBytes(3);
                tablaMBR.jumpInstruction = temporalArray;
                temporalArray = new byte[8];
                temporalArray = reader.ReadBytes(8);
                tablaMBR.oemID = Encoding.ASCII.GetString(temporalArray);
                tablaMBR.bytesxSector = reader.ReadInt16();
                tablaMBR.sectorxCluster = reader.ReadByte();
                tablaMBR.reservedSectors = reader.ReadInt16();
                tablaMBR.numberOfFATs = reader.ReadByte();
                tablaMBR.rootEntries = reader.ReadInt16();
                tablaMBR.smallSectors = reader.ReadInt16();
                tablaMBR.mediaDescriptor = reader.ReadByte();
                tablaMBR.sectorxFATs = reader.ReadInt16();
                tablaMBR.sectorxTrack = reader.ReadInt16();
                tablaMBR.numberOfHeads = reader.ReadInt16();
                tablaMBR.hiddenSectors = reader.ReadInt32();
                tablaMBR.largeSectors = reader.ReadInt32();
                tablaMBR.physicalDriveNo = reader.ReadByte();
                tablaMBR.reserved = reader.ReadByte();
                tablaMBR.extBootSignature = reader.ReadByte();
                tablaMBR.serialNo = reader.ReadInt32();
                temporalArray = new byte[11];
                temporalArray = reader.ReadBytes(11);
                tablaMBR.volumeLabel = Encoding.ASCII.GetString(temporalArray);
                temporalArray = new byte[8];
                temporalArray = reader.ReadBytes(8);
                tablaMBR.fileSystemType = Encoding.ASCII.GetString(temporalArray);
                tablaMBR.bootstrapCode = reader.ReadBytes(448);
                tablaMBR.endOfSector = reader.ReadInt16();
            }

            lstPropiedadesDisco.Items.Add("OEM ID " + tablaMBR.oemID);
            lstPropiedadesDisco.Items.Add("Bytes por Sector: " + tablaMBR.bytesxSector);
            lstPropiedadesDisco.Items.Add("Sectores por Cluster: " + tablaMBR.sectorxCluster);
            lstPropiedadesDisco.Items.Add("Sectores Reservados: " + tablaMBR.reservedSectors);
            lstPropiedadesDisco.Items.Add("Numero de FATs: " + tablaMBR.numberOfFATs);
            lstPropiedadesDisco.Items.Add("Entradas directorio Root: " + tablaMBR.rootEntries);
            lstPropiedadesDisco.Items.Add("Tipo de Disco: " + tablaMBR.mediaDescriptor);
            lstPropiedadesDisco.Items.Add("Sectores por FAT: " + tablaMBR.sectorxFATs);
            lstPropiedadesDisco.Items.Add("Sectores Ocultos: " + tablaMBR.hiddenSectors);
            lstPropiedadesDisco.Items.Add("Total Sectores: " + tablaMBR.largeSectors);
            lstPropiedadesDisco.Items.Add("Numero Serie: " + tablaMBR.serialNo);
            lstPropiedadesDisco.Items.Add("Etiqueta de Volumen: " + tablaMBR.volumeLabel);
            lstPropiedadesDisco.Items.Add("Formato de Volumen: " + tablaMBR.fileSystemType);

        }

        public void leerRootDirectory()
        {
            listaRootDirectory.Clear();
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = (tablaMBR.reservedSectors * tablaMBR.bytesxSector) + mbrOffset; //mas 512 de la tabla mbr

                for (int i = 0; i < 512; i++)
                {
                    Directory directorio = new Directory();
                    directorio.posicionByte = reader.BaseStream.Position;

                    directorio.filename = reader.ReadBytes(8);
                    directorio.filenameExt = reader.ReadBytes(3);
                    directorio.fileAttributes = reader.ReadByte();
                    directorio.NT = reader.ReadByte();
                    directorio.millisegundos_Creado = reader.ReadByte();
                    directorio.hora_Creado = reader.ReadUInt16();
                    directorio.fecha_Creado = reader.ReadUInt16();
                    directorio.fecha_ultimoAcceso = reader.ReadUInt16();
                    directorio.reservedFAT32 = reader.ReadUInt16();
                    directorio.hora_ultimaEscritura = reader.ReadUInt16();
                    directorio.fecha_ultimaEscritura = reader.ReadUInt16();
                    directorio.startingCluster = reader.ReadUInt16();
                    directorio.fileSize = reader.ReadUInt32();
                    
                    listaRootDirectory.Add(directorio);
                }
            }
        }
        public void leerTablasFat()
        {
            fat1List.Clear();
            fat2List.Clear();
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = mbrOffset;
                ////leer 2 tablas FAT
                inicioTablaFat1 = reader.BaseStream.Position;
                for (int i = 0; i < 65536; i++)
                {
                    FAT16 fatentry = new FAT16();
                    fatentry.entradaFAT = reader.ReadUInt16();
                    fat1List.Add(fatentry);
                }
                inicioTablaFat2 = reader.BaseStream.Position;
                for (int i = 0; i < 65536; i++)
                {
                    FAT16 fatentry = new FAT16();
                    fatentry.entradaFAT = reader.ReadUInt16();
                    fat2List.Add(fatentry);
                }
            }
            lstPropiedadesDisco.Items[12]="Espacio Libre: " + calcularEspacioLibre() + " KB";
        }
        public int calcularEspacioLibre()
        {
            int clustersVacios = fat1List.Count(x => x.entradaFAT == 0);
            return 16 * clustersVacios; //multiplicar cantidad de clusters libres por 16KB para saber el espacio libre
        }
        public void cargarViewGeneral(bool isRoot)
        {
            viewGeneral.Items.Clear();
            List<Directory> listaDirectorios = new List<Directory>();

            if (!isRoot) {
                // carpetaActual.Padre.posicionByte.ToString()
                string[] info = { ".."+ Encoding.ASCII.GetString(carpetaActual.filename), "", carpetaActual.posicionByte.ToString() };
                ListViewItem nod = new ListViewItem(info, 0);
                nod.Tag = "D," + carpetaActual.posicionByte;
                viewGeneral.Items.Add(nod);
                viewRootDirectory = false;
                listaDirectorios = listaDirectorioActual;
            }else
            {
                viewRootDirectory = true;
                listaDirectorios = listaRootDirectory;
            }
            //viewRootDirectory = true;
            foreach (var file in listaDirectorios)
            {
                if (file.filename[0] != 0)
                {
                    if (Convert.ToChar(file.fileAttributes) == 'D')
                    {
                        string nombre = Encoding.ASCII.GetString(file.filename);
                        DateTime fechaCreacion = file.getFecha(file.fecha_Creado);
                        DateTime horaCreacion = file.getHora(file.hora_Creado);

                        string[] info = { nombre,
                                          fechaCreacion.ToShortDateString() + ' ' + horaCreacion.ToShortTimeString(),
                                          file.fileSize.ToString()
                                        };

                        ListViewItem nod = new ListViewItem(info, 0);
                        nod.Tag =  "D," + file.posicionByte;
                        viewGeneral.Items.Add(nod);
                    }
                    else if (Convert.ToChar(file.fileAttributes) == 'A')
                    {
                        string a = Encoding.ASCII.GetString(file.filename);
                        a = a + "." + Encoding.ASCII.GetString(file.filenameExt);
                        string nombre = a.Replace("\0", string.Empty);

                        DateTime fechaCreacion = file.getFecha(file.fecha_Creado);
                        DateTime horaCreacion = file.getHora(file.hora_Creado);

                        string[] info = { nombre,
                                          fechaCreacion.ToShortDateString() + ' ' + horaCreacion.ToShortTimeString(),
                                          file.fileSize.ToString()
                                        };

                        ListViewItem nod = new ListViewItem(info, 1);
                        nod.Tag = "A," + file.posicionByte;
                        viewGeneral.Items.Add(nod);
                    }
                }
            }
        }
        public long posicionByteLibreRootDirectory()
        {
            long posicionByteLibre=-1;
            for (int i = 0; i < listaRootDirectory.Count(); i++)
            {
                if (listaRootDirectory.ElementAt(i).filename[0] == 0)
                {
                    posicionByteLibre = (tablaMBR.reservedSectors * tablaMBR.bytesxSector) + (i * 32) + mbrOffset;
                    break;
                }
            }
            return posicionByteLibre;
        }

        public void actualizarCarpetaActual()
        {
            carpetaActual.subDirectorio.Clear();
            long posicion = carpetaActual.posicionByte;
            carpetaActual.Padre = new Directory();
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = posicion + 26;
                carpetaActual.Padre.startingCluster = reader.ReadUInt16();
                reader.BaseStream.Position = reader.BaseStream.Position + 2;
                int limiteCluster = (tamanioCluster - 64) / 2;
                for (int j = 0; j < limiteCluster; j++)
                {
                    ushort apuntadorFAT = reader.ReadUInt16();
                    carpetaActual.subDirectorio.Add(apuntadorFAT);
                }
            }
            listaDirectorioActual.Clear();
            listaDirectorioActual = getSubdirectorio(carpetaActual.subDirectorio);
        }
        #endregion


        #region  //-------- CONTROL DE ARCHIVOS ---------
        /// <summary>
        /// Crea una carpeta en el root directory
        /// </summary>
        /// <param name="nuevaCarpeta">nombre de la carpeta</param>
        public void crearCarpetaEnRoot(Directory nuevaCarpeta)
        {
            long posicionByteLibre = posicionByteLibreRootDirectory();
            
            //int posicion = 0;
            if (posicionByteLibre >= 0)
            {
                ushort clusterSubCarpeta = buscarClusterVacio();
                nuevaCarpeta.startingCluster = clusterSubCarpeta;
                using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                {
                    stream.BaseStream.Position = posicionByteLibre;
                    stream.Write(nuevaCarpeta.filename);
                    stream.Write(nuevaCarpeta.filenameExt);
                    stream.Write(nuevaCarpeta.fileAttributes);
                    stream.Write(nuevaCarpeta.NT);
                    stream.Write(nuevaCarpeta.millisegundos_Creado);
                    stream.Write(nuevaCarpeta.hora_Creado);
                    stream.Write(nuevaCarpeta.fecha_Creado);
                    stream.Write(nuevaCarpeta.fecha_ultimoAcceso);
                    stream.Write(nuevaCarpeta.reservedFAT32);
                    stream.Write(nuevaCarpeta.hora_ultimaEscritura);
                    stream.Write(nuevaCarpeta.fecha_ultimaEscritura);
                    stream.Write(nuevaCarpeta.startingCluster);
                    stream.Write(nuevaCarpeta.fileSize);

                    long posicionBytesSubCarpeta = posicionByteCluster(clusterSubCarpeta);
                    stream.BaseStream.Position = posicionBytesSubCarpeta;
                    stream.Write(nuevaCarpeta.filename);
                    stream.Write(nuevaCarpeta.filenameExt);
                    stream.Write(nuevaCarpeta.fileAttributes);
                    stream.Write(nuevaCarpeta.NT);
                    stream.Write(nuevaCarpeta.millisegundos_Creado);
                    stream.Write(nuevaCarpeta.hora_Creado);
                    stream.Write(nuevaCarpeta.fecha_Creado);
                    stream.Write(nuevaCarpeta.fecha_ultimoAcceso);
                    stream.Write(nuevaCarpeta.reservedFAT32);
                    stream.Write(nuevaCarpeta.hora_ultimaEscritura);
                    stream.Write(nuevaCarpeta.fecha_ultimaEscritura);
                    ushort clusterDirectorio = new ushort();
                    stream.Write(clusterDirectorio);
                    stream.Write(nuevaCarpeta.fileSize);

                    stream.Write(nuevaCarpeta.filename);
                    stream.Write(nuevaCarpeta.filenameExt);
                    stream.Write(nuevaCarpeta.fileAttributes);
                    stream.Write(nuevaCarpeta.NT);
                    stream.Write(nuevaCarpeta.millisegundos_Creado);
                    stream.Write(nuevaCarpeta.hora_Creado);
                    stream.Write(nuevaCarpeta.fecha_Creado);
                    stream.Write(nuevaCarpeta.fecha_ultimoAcceso);
                    stream.Write(nuevaCarpeta.reservedFAT32);
                    stream.Write(nuevaCarpeta.hora_ultimaEscritura);
                    stream.Write(nuevaCarpeta.fecha_ultimaEscritura);
                    stream.Write(clusterSubCarpeta);
                    stream.Write(nuevaCarpeta.fileSize);

                    int limiteCluster = (tamanioCluster - 64) / 2;
                    for(int i = 0; i < limiteCluster; i++)
                    {
                        ushort apuntadorFAT = 0;
                        stream.Write(apuntadorFAT);
                    }
                }

                setmarcadorCluster(clusterSubCarpeta, 1);
                //ushort clusterSubCarpeta = buscarClusterVacio();
                
            }else
            {
                MessageBox.Show("No se puede crear la carpeta!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Borra una carpeta creada en el root directory
        /// </summary>
        /// <param name="nombreCarpeta">nombre de la carpeta</param>
        public void borrarCarpetaEnRoot(string nombreCarpeta)
        {
            Directory carpeta = listaRootDirectory.Where(x => Encoding.ASCII.GetString(x.filename) == nombreCarpeta).FirstOrDefault();

            long posicion = carpeta.posicionByte;
            Directory carpetaenblanco = new Directory();
            using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = posicion;
                stream.Write(carpetaenblanco.filename);
                stream.Write(carpetaenblanco.filenameExt);
                stream.Write(carpetaenblanco.fileAttributes);
                stream.Write(carpetaenblanco.NT);
                stream.Write(carpetaenblanco.millisegundos_Creado);
                stream.Write(carpetaenblanco.hora_Creado);
                stream.Write(carpetaenblanco.fecha_Creado);
                stream.Write(carpetaenblanco.fecha_ultimoAcceso);
                stream.Write(carpetaenblanco.reservedFAT32);
                stream.Write(carpetaenblanco.hora_ultimaEscritura);
                stream.Write(carpetaenblanco.fecha_ultimaEscritura);
                stream.Write(carpetaenblanco.startingCluster);
                stream.Write(carpetaenblanco.fileSize);
            }
            setmarcadorCluster(carpeta.startingCluster, 0);
        }

        /// <summary>
        /// Crear una Carpeta en el disco
        /// </summary>
        /// <param name="Padre"></param>
        /// <param name="Hija"></param>
        public void crearCarpeta(Directory nuevaCarpeta)
        {
            ushort cluster = buscarClusterVacio();
            
            //int posicion = 0;
            if (cluster > 0)
            {
                long posicionByte = posicionByteCluster(cluster);
                nuevaCarpeta.startingCluster = cluster;
                using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                {
                    stream.BaseStream.Position = posicionByte;

                    stream.Write(carpetaActual.filename);
                    stream.Write(carpetaActual.filenameExt);
                    stream.Write(carpetaActual.fileAttributes);
                    stream.Write(carpetaActual.NT);
                    stream.Write(carpetaActual.millisegundos_Creado);
                    stream.Write(carpetaActual.hora_Creado);
                    stream.Write(carpetaActual.fecha_Creado);
                    stream.Write(carpetaActual.fecha_ultimoAcceso);
                    stream.Write(carpetaActual.reservedFAT32);
                    stream.Write(carpetaActual.hora_ultimaEscritura);
                    stream.Write(carpetaActual.fecha_ultimaEscritura);
                    stream.Write(carpetaActual.startingCluster);
                    stream.Write(carpetaActual.fileSize);

                    stream.Write(nuevaCarpeta.filename);
                    stream.Write(nuevaCarpeta.filenameExt);
                    stream.Write(nuevaCarpeta.fileAttributes);
                    stream.Write(nuevaCarpeta.NT);
                    stream.Write(nuevaCarpeta.millisegundos_Creado);
                    stream.Write(nuevaCarpeta.hora_Creado);
                    stream.Write(nuevaCarpeta.fecha_Creado);
                    stream.Write(nuevaCarpeta.fecha_ultimoAcceso);
                    stream.Write(nuevaCarpeta.reservedFAT32);
                    stream.Write(nuevaCarpeta.hora_ultimaEscritura);
                    stream.Write(nuevaCarpeta.fecha_ultimaEscritura);
                    stream.Write(nuevaCarpeta.startingCluster);
                    stream.Write(nuevaCarpeta.fileSize);

                    int limiteCluster = (tamanioCluster - 64) / 2;
                    for (int i = 0; i < limiteCluster; i++)
                    {
                        ushort apuntadorFAT = 0;
                        stream.Write(apuntadorFAT);
                    }
                }
                setmarcadorCluster(cluster, 1);
                agregarSubDirectorioCarpetaActual(cluster);
                //ushort clusterSubCarpeta = buscarClusterVacio();
            }
            else
            {
                MessageBox.Show("No se puede crear la carpeta!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
            
        }
        
        /// <summary>
        /// Guardar un archivo en el root directory
        /// </summary>
        /// <param name="Archivo">Objeto Directory que contiene la informacion del archivo</param>
        /// <param name="archivo">Arreglo de bytes que contiene el archivo a guardar</param>
        public void guardarArchivoEnRoot(Directory Archivo, byte[] archivo)
        {
            
            long posicionByteLibre = posicionByteLibreRootDirectory();
            if (posicionByteLibre >= 0)
            {
                ushort clusterInicioArchivo = buscarClusterVacio();
                Archivo.startingCluster = clusterInicioArchivo;

                long posicionClusterArchivo = posicionByteCluster(clusterInicioArchivo);

                using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                {
                    stream.BaseStream.Position = posicionByteLibre;
                    stream.Write(Archivo.filename);
                    stream.Write(Archivo.filenameExt);
                    stream.Write(Archivo.fileAttributes);
                    stream.Write(Archivo.NT);
                    stream.Write(Archivo.millisegundos_Creado);
                    stream.Write(Archivo.hora_Creado);
                    stream.Write(Archivo.fecha_Creado);
                    stream.Write(Archivo.fecha_ultimoAcceso);
                    stream.Write(Archivo.reservedFAT32);
                    stream.Write(Archivo.hora_ultimaEscritura);
                    stream.Write(Archivo.fecha_ultimaEscritura);
                    stream.Write(Archivo.startingCluster);
                    stream.Write(Archivo.fileSize);
                }
                
                if (Archivo.fileSize < tamanioCluster)
                {
                    
                    using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                    {
                        stream.BaseStream.Position = posicionClusterArchivo;
                        stream.Write(archivo, 0, archivo.Length);
                    }
                    setmarcadorCluster(clusterInicioArchivo, 1);
                }
                else
                {
                    int cantidadClusters = (int) Math.Ceiling((double)Archivo.fileSize / tamanioCluster);
                    ushort []clusterAsignado = new ushort[cantidadClusters];
                    clusterAsignado[0] = clusterInicioArchivo;
                    setmarcadorCluster(clusterInicioArchivo, 1);
                    
                    for (int j = 1; j < cantidadClusters; j++)
                    {
                        clusterAsignado[j] = buscarClusterVacio();
                        //marcar como ocupado temporalmente para que no lo devuelva como vacio
                        setmarcadorCluster(clusterAsignado[j], 1);
                    }

                    for (int i = 0; i < cantidadClusters; i++)
                    {
                        posicionClusterArchivo = posicionByteCluster(clusterAsignado[i]);
                        using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                        {
                            stream.BaseStream.Position = posicionClusterArchivo;
                            int offset=0;
                            if (i > 0) offset = (i * tamanioCluster) ;

                            if (i == cantidadClusters - 1)
                            {
                                long bytesSobrantes = Archivo.fileSize-(tamanioCluster * (cantidadClusters-1));
                                stream.Write(archivo, offset,(int)bytesSobrantes);
                            }
                            else
                            {
                                stream.Write(archivo, offset, tamanioCluster);
                            }
                        }

                        if (i != cantidadClusters - 1)
                        {
                            setmarcadorCluster(clusterAsignado[i], clusterAsignado[i+1]);
                        }else
                        {
                            setmarcadorCluster(clusterAsignado[i], 1);
                        }
                    }
                }
                leerRootDirectory();
            }
            else
            {
                MessageBox.Show("No se puede guardar el archivo!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Sacar un archivo del root directory
        /// </summary>
        /// <param name="nombreArchivo">nombre del archivo</param>
        /// <returns></returns>
        public byte[] sacarArchivoDeRoot(string nombreArchivo)
        {
            var archivo = listaRootDirectory.Where(x => (Encoding.ASCII.GetString(x.filename)+"."+Encoding.ASCII.GetString(x.filenameExt)) == nombreArchivo).FirstOrDefault();

            byte[] result = new byte[archivo.fileSize];
            long posicionInicioCluster=0;
            ushort[] clusters =  clustersArchivo(archivo);
            if (clusters.Length == 1)
            {
                posicionInicioCluster = posicionByteCluster(archivo.startingCluster);
                using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
                {
                    reader.BaseStream.Position = posicionInicioCluster;
                    reader.Read(result, 0,(int)archivo.fileSize);
                }
            }else
            {
                for(int i=0;i<clusters.Length;i++)
                {
                    posicionInicioCluster = posicionByteCluster(clusters[i]);
                    using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
                    {
                        reader.BaseStream.Position = posicionInicioCluster;
                        int offset = 0;
                        if (i > 0) offset = (i * tamanioCluster) ;

                        if (i == clusters.Length - 1)
                        {
                            long bytesSobrantes = archivo.fileSize-(tamanioCluster * (clusters.Length - 1));
                            reader.Read(result, offset,(int)bytesSobrantes);
                        }else
                        {
                            reader.Read(result, offset, tamanioCluster);
                        }
                        
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Guarda un archivo en el disco
        /// </summary>
        /// <param name="Archivo">Objeto Directory que contiene la informacion del archivo</param>
        /// <param name="archivo">Arreglo de bytes que contiene el archivo a guardar</param>
        /// <returns></returns>
        public ushort guardarArchivo(Directory Archivo, byte[] archivo)
        {
            ushort clusterDirectoryEntry = buscarClusterVacio();
            long posicionByteDirectoryEntry = posicionByteCluster(clusterDirectoryEntry);
            if (posicionByteDirectoryEntry >= 0)
            {
                if (Archivo.fileSize < tamanioCluster-32)
                {
                    Archivo.startingCluster = 0;
                    using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                    {
                        stream.BaseStream.Position = posicionByteDirectoryEntry;
                        stream.Write(Archivo.filename);
                        stream.Write(Archivo.filenameExt);
                        stream.Write(Archivo.fileAttributes);
                        stream.Write(Archivo.NT);
                        stream.Write(Archivo.millisegundos_Creado);
                        stream.Write(Archivo.hora_Creado);
                        stream.Write(Archivo.fecha_Creado);
                        stream.Write(Archivo.fecha_ultimoAcceso);
                        stream.Write(Archivo.reservedFAT32);
                        stream.Write(Archivo.hora_ultimaEscritura);
                        stream.Write(Archivo.fecha_ultimaEscritura);
                        stream.Write(Archivo.startingCluster);
                        stream.Write(Archivo.fileSize);
                        stream.Write(archivo);
                    }
                    setmarcadorCluster(clusterDirectoryEntry, 1);
                }
                else
                {
                    ushort[] clustersAsignados = asignarClustersArchivo(Archivo.fileSize);

                    setmarcadorCluster(clusterDirectoryEntry, clustersAsignados[0]);
                    Archivo.startingCluster = clustersAsignados[0];
                    long posicionClusterArchivo = posicionByteCluster(clusterDirectoryEntry);
                    using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                    {
                        stream.BaseStream.Position = posicionByteDirectoryEntry;
                        stream.Write(Archivo.filename);
                        stream.Write(Archivo.filenameExt);
                        stream.Write(Archivo.fileAttributes);
                        stream.Write(Archivo.NT);
                        stream.Write(Archivo.millisegundos_Creado);
                        stream.Write(Archivo.hora_Creado);
                        stream.Write(Archivo.fecha_Creado);
                        stream.Write(Archivo.fecha_ultimoAcceso);
                        stream.Write(Archivo.reservedFAT32);
                        stream.Write(Archivo.hora_ultimaEscritura);
                        stream.Write(Archivo.fecha_ultimaEscritura);
                        stream.Write(Archivo.startingCluster);
                        stream.Write(Archivo.fileSize);
                    }

                    for (int i = 0; i < clustersAsignados.Length; i++)
                    {
                        posicionClusterArchivo = posicionByteCluster(clustersAsignados[i]);
                        using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
                        {
                            stream.BaseStream.Position = posicionClusterArchivo;
                            int offset = 0;
                            if (i > 0) offset = (i * tamanioCluster);

                            if (i == clustersAsignados.Length - 1)
                            {
                                long bytesSobrantes = Archivo.fileSize - (tamanioCluster * (clustersAsignados.Length - 1));
                                stream.Write(archivo, offset, (int)bytesSobrantes);
                            }
                            else
                            {
                                stream.Write(archivo, offset, tamanioCluster);
                            }
                        }

                        if (i != clustersAsignados.Length - 1)
                        {
                            setmarcadorCluster(clustersAsignados[i], clustersAsignados[i + 1]);
                        }
                        else
                        {
                            setmarcadorCluster(clustersAsignados[i], 1);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se puede crear la carpeta!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
            return clusterDirectoryEntry;
        }

        /// <summary>
        /// Sacar un archivo del disco
        /// </summary>
        /// <returns>byte array con el archivo</returns>
        public byte[] sacarArchivo()
        {
            //var archivo = listaRootDirectory.Where(x => (Encoding.ASCII.GetString(x.filename) + "." + Encoding.ASCII.GetString(x.filenameExt)) == nombreArchivo).FirstOrDefault();

            byte[] result = new byte[2];
            //long posicionInicioCluster = 0;
            //ushort[] clusters = clustersArchivo(archivo);
            //if (clusters.Length == 1)
            //{
            //    posicionInicioCluster = posicionByteCluster(archivo.startingCluster);
            //    using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            //    {
            //        reader.BaseStream.Position = posicionInicioCluster;
            //        reader.Read(result, 0, (int)archivo.fileSize);
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < clusters.Length; i++)
            //    {
            //        posicionInicioCluster = posicionByteCluster(clusters[i]);
            //        using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            //        {
            //            reader.BaseStream.Position = posicionInicioCluster;
            //            int offset = 0;
            //            if (i > 0) offset = (i * tamanioCluster);

            //            if (i == clusters.Length - 1)
            //            {
            //                long bytesSobrantes = archivo.fileSize - (tamanioCluster * (clusters.Length - 1));
            //                reader.Read(result, offset, (int)bytesSobrantes);
            //            }
            //            else
            //            {
            //                reader.Read(result, offset, tamanioCluster);
            //            }

            //        }
            //    }
            //}
           return result;
        } 

        /// <summary>
        /// Leer una entrada de directorio directamente por la posicion en bytes de la entrada
        /// </summary>
        /// <returns>Un objeto Directorio con la entrada de directorio</returns>
        public Directory leerEntradaDirectorio(long poscionBytes)
        {
            Directory result = new Directory();
            result.Padre = new Directory(); 
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = poscionBytes;
                result.Padre.posicionByte = reader.BaseStream.Position;
                ///reader.BaseStream.Position = poscionBytes+32; 
                result.Padre.filename = reader.ReadBytes(8);
                result.Padre.filenameExt = reader.ReadBytes(3);
                result.Padre.fileAttributes = reader.ReadByte();
                result.Padre.NT = reader.ReadByte();
                result.Padre.millisegundos_Creado = reader.ReadByte();
                result.Padre.hora_Creado = reader.ReadUInt16();
                result.Padre.fecha_Creado = reader.ReadUInt16();
                result.Padre.fecha_ultimoAcceso = reader.ReadUInt16();
                result.Padre.reservedFAT32 = reader.ReadUInt16();
                result.Padre.hora_ultimaEscritura = reader.ReadUInt16();
                result.Padre.fecha_ultimaEscritura = reader.ReadUInt16();
                result.Padre.startingCluster = reader.ReadUInt16();
                result.Padre.fileSize = reader.ReadUInt32();

                result.posicionByte = poscionBytes;
                result.filename = reader.ReadBytes(8);
                result.filenameExt = reader.ReadBytes(3);
                result.fileAttributes = reader.ReadByte();
                result.NT = reader.ReadByte();
                result.millisegundos_Creado = reader.ReadByte();
                result.hora_Creado = reader.ReadUInt16();
                result.fecha_Creado = reader.ReadUInt16();
                result.fecha_ultimoAcceso = reader.ReadUInt16();
                result.reservedFAT32 = reader.ReadUInt16();
                result.hora_ultimaEscritura = reader.ReadUInt16();
                result.fecha_ultimaEscritura = reader.ReadUInt16();
                result.startingCluster = reader.ReadUInt16();
                result.fileSize = reader.ReadUInt32();
            }
            return result;
        }

        /// <summary>
        /// obtener lista de directorios que contiene la carpeta
        /// </summary>
        /// <param name="clusterSubdirectorio">lista con los clusters que apuntan a los archivos de la carpeta</param>
        /// <returns>lista con las entradas de directorio</returns>
        public List<Directory> getSubdirectorio(List<ushort> clusterSubdirectorio)
        {
            List<Directory> result = new List<Directory>();
            var filtro = clusterSubdirectorio.Where(x => x != 0);
            foreach (ushort c in filtro)
            {
                Directory entradaDirectorio = new Directory();
                entradaDirectorio.Padre = new Directory();
                long posicionBytes = posicionByteCluster(c);
                using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
                {
                    reader.BaseStream.Position = posicionBytes;
                    entradaDirectorio.Padre.posicionByte = reader.BaseStream.Position;
                    ///reader.BaseStream.Position = posicionBytes + 32; 
                    entradaDirectorio.Padre.filename = reader.ReadBytes(8);
                    entradaDirectorio.Padre.filenameExt = reader.ReadBytes(3);
                    entradaDirectorio.Padre.fileAttributes = reader.ReadByte();
                    entradaDirectorio.Padre.NT = reader.ReadByte();
                    entradaDirectorio.Padre.millisegundos_Creado = reader.ReadByte();
                    entradaDirectorio.Padre.hora_Creado = reader.ReadUInt16();
                    entradaDirectorio.Padre.fecha_Creado = reader.ReadUInt16();
                    entradaDirectorio.Padre.fecha_ultimoAcceso = reader.ReadUInt16();
                    entradaDirectorio.Padre.reservedFAT32 = reader.ReadUInt16();
                    entradaDirectorio.Padre.hora_ultimaEscritura = reader.ReadUInt16();
                    entradaDirectorio.Padre.fecha_ultimaEscritura = reader.ReadUInt16();
                    entradaDirectorio.Padre.startingCluster = reader.ReadUInt16();
                    entradaDirectorio.Padre.fileSize = reader.ReadUInt32();

                    entradaDirectorio.posicionByte = posicionBytes;
                    entradaDirectorio.filename = reader.ReadBytes(8);
                    entradaDirectorio.filenameExt = reader.ReadBytes(3);
                    entradaDirectorio.fileAttributes = reader.ReadByte();
                    entradaDirectorio.NT = reader.ReadByte();
                    entradaDirectorio.millisegundos_Creado = reader.ReadByte();
                    entradaDirectorio.hora_Creado = reader.ReadUInt16();
                    entradaDirectorio.fecha_Creado = reader.ReadUInt16();
                    entradaDirectorio.fecha_ultimoAcceso = reader.ReadUInt16();
                    entradaDirectorio.reservedFAT32 = reader.ReadUInt16();
                    entradaDirectorio.hora_ultimaEscritura = reader.ReadUInt16();
                    entradaDirectorio.fecha_ultimaEscritura = reader.ReadUInt16();
                    entradaDirectorio.startingCluster = reader.ReadUInt16();
                    entradaDirectorio.fileSize = reader.ReadUInt32();
                }
                result.Add(entradaDirectorio);
            }


            return result;
        }

        public void agregarSubDirectorioCarpetaActual(ushort cluster)
        {
            int entradaLibre = 0;
            int limiteCluster = (tamanioCluster - 64) / 2;
            long posicion = carpetaActual.posicionByte + 64;
            
            using (BinaryReader reader = new BinaryReader(new FileStream(frmMain.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = posicion;
                
                for (int j = 0; j < limiteCluster; j++)
                {
                    ushort apuntadorFAT = reader.ReadUInt16();
                    if (apuntadorFAT == 0)
                    {
                        entradaLibre = j;
                        break;
                    }
                }
            }

            using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = (posicion) + (entradaLibre*2);
                stream.Write(cluster);
            }
            actualizarCarpetaActual();
        }
        #endregion

        #region //-------- CONTROL DE TABLA FAT ---------
        /// <summary>
        /// Escribir valor en la entrada de la tabla FAT
        /// </summary>
        /// <param name="numeroCluster"> Numero del cluster a editar </param>
        /// <param name="marcador">marcador del cluster(0=libre,1=Ocupado,numerodecluster=Apuntador a cluster siguiente)</param>
        void setmarcadorCluster(ushort numeroCluster, ushort marcador)
        {
            using (BinaryWriter stream = new BinaryWriter(File.Open(frmMain.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = inicioTablaFat1 + (numeroCluster * 2);
                stream.Write(marcador);
            }
            leerTablasFat();
        }

        /// <summary>
        /// Busca un cluster vacio en la tabla FAT
        /// </summary>
        /// <returns>Retorna en numero del cluster, si retorna 0 es que no hay ningun cluster vacio</returns>
        public ushort buscarClusterVacio()
        {
           for(ushort i = 0; i < fat1List.Count(); i++)
            {
                if (fat1List.ElementAt(i).entradaFAT == 0)
                {
                    return i;
                }
            }
            return 0; 
        }

        /// <summary>
        /// Devuelve la posicion en bytes de un cluster
        /// </summary>
        /// <param name="numeroCluster">El numero del cluster</param>
        /// <returns></returns>
        public long posicionByteCluster(ushort numeroCluster)
        {
            long posicion = (numeroCluster * tamanioCluster) + mbrOffset;
            return posicion;
        }

        public ushort clusterPosicionByte(long posicionBytes)
        {
            ushort cluster = (ushort)((posicionBytes - mbrOffset) / tamanioCluster);
            return cluster;
        }

        /// <summary>
        /// Devuelve un arreglo con los clusters de un archivo guardado
        /// </summary>
        /// <param name="archivo">El archivo a leer</param>
        /// <returns></returns>
        public ushort[] clustersArchivo(Directory archivo)
        {
            List<ushort> listaClusters = new List<ushort>();
            ushort clusterInicio = archivo.startingCluster;
            listaClusters.Add(clusterInicio);
            ushort contadorCluster = clusterInicio;
            do {
                if (fat1List.ElementAt(contadorCluster).entradaFAT == 1) //Ocupado, fin del archivo.
                {
                    break;
                }
                else
                {
                    listaClusters.Add(fat1List.ElementAt(contadorCluster).entradaFAT);
                    contadorCluster = fat1List.ElementAt(contadorCluster).entradaFAT;
                }
            } while (contadorCluster < fat1List.Count());
            return listaClusters.ToArray();
        }

        /// <summary>
        /// Asignar clusters vacios de la tabla FAT a un archivo
        /// </summary>
        /// <param name="FileSize">Tamaño del archivo para saber cuantos clusters ocupara en el disco</param>
        /// <returns>Devuelve un arreglo de clusters asignados a un archivo</returns>
        public ushort[] asignarClustersArchivo(uint FileSize)
        {
            int cantidadClusters = (int)Math.Ceiling((double)FileSize / tamanioCluster);
            ushort[] clusterAsignado = new ushort[cantidadClusters];

            for (int j = 1; j < cantidadClusters; j++)
            {
                clusterAsignado[j] = buscarClusterVacio();
                //marcar como ocupado temporalmente para que no lo devuelva como vacio
                setmarcadorCluster(clusterAsignado[j], 1);
            }
            return clusterAsignado;
        }


        #endregion

    }
}
