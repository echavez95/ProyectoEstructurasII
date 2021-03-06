﻿using System;
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
        #region //---------------Variables Globales--------------------
            public List<FAT16> fat1List = new List<FAT16>();
            public List<FAT16> fat2List = new List<FAT16>();
            public List<Directory> listaRootDirectory = new List<Directory>();
            public List<Directory> listaDirectorioActual = new List<Directory>();
            public Directory carpetaActual2 = new Directory();
            public decodedMBR tablaMBR = new decodedMBR();
            public int mbrOffset = 512;
            public long inicioTablaFat1 = new long();
            public long inicioTablaFat2 = new long();
            public bool viewRootDirectory = new bool(); //variable que controla si lo que esta en el viewgeneral es el root directory
            public int tamanioCluster = new int();
            public static ArbolBPlus arbolIndice = new ArbolBPlus(); //Arbol b+ del indice
        #endregion

        #region //------------------Metodos del Formulario-----------------
        public frmExplorer()
        {
            InitializeComponent();
        }

        private void frmExplorer_Load(object sender, EventArgs e)
        {
                lblDiscoDefault.Text = "Disco: " +  Constants.discoDefault;
                leerInfoDisco();
                leerTablasFat();
                leerRootDirectory();
                tamanioCluster = tablaMBR.bytesxSector * tablaMBR.sectorxCluster;
                cargarIndice();
                cargarViewGeneral(true);
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgAbrirArchivo = new OpenFileDialog();
            dlgAbrirArchivo.Filter = "Todos los Archivos | *.*";
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
                    if (viewRootDirectory) {
                        guardarArchivoEnRoot(entradaArchivo, archivo);
                        leerRootDirectory();
                        cargarViewGeneral(true);
                    }else
                    {
                        guardarArchivo(entradaArchivo, archivo);
                        cargarCarpeta(carpetaActual2.startingCluster);
                        cargarViewGeneral(false);
                    }
                    
                }
            }
        }
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            ListViewItem seleccionado = viewGeneral.SelectedItems[0];
            string[] prop = seleccionado.Tag.ToString().Split(',');
            
            if (prop[0] == "A") //obtiene datos del directorio guardados en el listviewitem, si es un archivo entonces
            {
                byte[] archivo;
                ushort cluster = Convert.ToUInt16(prop[1].ToString());
                if (viewRootDirectory)
                {
                   archivo = sacarArchivoDeRoot(cluster);
                    
                }else
                {
                    archivo = sacarArchivo(cluster);
                }

                SaveFileDialog dlgGuardarArchivo = new SaveFileDialog();
                //string nombre = seleccionado.Text.Substring(0, seleccionado.Text.Length - 4);
                string extension = seleccionado.Text.Substring(seleccionado.Text.Length - 3, 3);
                dlgGuardarArchivo.Filter = "Archivos | *." + extension;
                dlgGuardarArchivo.DefaultExt = extension;

                dlgGuardarArchivo.FileName = seleccionado.Text.ToString();
                if (dlgGuardarArchivo.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(dlgGuardarArchivo.FileName))
                    {
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
        
        private void btnNuevaCarpeta_Click(object sender, EventArgs e)
        {
            string nombreCarpeta = Interaction.InputBox("Nombre", "Nueva Carpeta");
            
            //nombreCarpeta = Interaction.InputBox("Nombre", "Nueva Carpeta");
            if(nombreCarpeta.Length > 0 && nombreCarpeta.Length <= 8)
            {
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
                        Carpeta.nuevaCarpeta(nombreCarpeta, DateTime.Now);
                        crearCarpetaEnRoot(Carpeta);
                        leerRootDirectory();
                        cargarViewGeneral(true);
                    }
                }
                else
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
                        Carpeta.nuevaCarpeta(nombreCarpeta, DateTime.Now);
                        crearCarpeta(Carpeta);
                        cargarCarpeta(carpetaActual2.startingCluster);
                        cargarViewGeneral(false);
                    }
                }
            }
            else
            {
                MessageBox.Show("El nombre debe ser menor o igual a 8 caracteres"
                                , "Informacion"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Exclamation);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            ListViewItem seleccionado = viewGeneral.SelectedItems[0];
            string[] prop = seleccionado.Tag.ToString().Split(',');
            if (prop[0] == "A") //obtiene datos del directorio guardados en el listviewitem, si es un archivo entonces
            {
                ushort cluster = Convert.ToUInt16(prop[1].ToString());
                if (viewRootDirectory)
                {
                    borrarArchivoRootDirectory(cluster);
                    leerRootDirectory();
                    cargarViewGeneral(true);
                }
                else
                {
                    borrarArchivo(cluster);
                    cargarCarpeta(carpetaActual2.startingCluster);
                    cargarViewGeneral(false);
                }
                MessageBox.Show("Borrado Con Exito!",
                                       "Informacion",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Exclamation);
            }
                
        }

        private void viewGeneral_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem seleccionado = viewGeneral.SelectedItems[0];

            string[] prop = seleccionado.Tag.ToString().Split(',');
            if (prop[0] == "D") //obtiene datos del directorio guardados en el listviewitem, si es una carpeta entonces
            {
                if (seleccionado.Text == "..")
                {
                    ushort clusterCarpeta = Convert.ToUInt16(prop[1].ToString());
                    if (clusterCarpeta == 0)
                    {
                        leerRootDirectory();
                        lblFolder.Text = "Carpeta Actual: Root";
                        cargarViewGeneral(true);
                    }else
                    {
                        cargarCarpeta(clusterCarpeta);
                        lblFolder.Text = "Carpeta Actual: " + Encoding.ASCII.GetString(carpetaActual2.filename);
                        cargarViewGeneral(false);
                    }
                }
                else
                {
                    ushort clusterCarpeta = Convert.ToUInt16(prop[1].ToString());
                    cargarCarpeta(clusterCarpeta);
                    lblFolder.Text = "Carpeta Actual: " + Encoding.ASCII.GetString(carpetaActual2.filename);
                    cargarViewGeneral(false);
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            frmBusqueda formBusqueda = new frmBusqueda();
            ListViewItem seleccionado = new ListViewItem();
            DialogResult drformBusqueda = formBusqueda.ShowDialog(this);
            if (drformBusqueda == DialogResult.Cancel)
            {
                formBusqueda.Close();
            }
            else if (drformBusqueda == DialogResult.OK)
            {
                seleccionado = formBusqueda.getItemSeleccionadoView();
                formBusqueda.Close();

                string[] tag = seleccionado.Tag.ToString().Split(',');
                byte[] archivo;
                ushort cluster = Convert.ToUInt16(tag[0].ToString());
                byte isRoot = Convert.ToByte(tag[1].ToString());

                if (isRoot==1)
                {
                    archivo = sacarArchivoDeRoot(cluster);

                }
                else
                {
                    archivo = sacarArchivo(cluster);
                }

                SaveFileDialog dlgGuardarArchivo = new SaveFileDialog();
                //string nombre = seleccionado.Text.Substring(0, seleccionado.Text.Length - 4);
                string extension = seleccionado.Text.Substring(seleccionado.Text.Length - 3, 3);
                dlgGuardarArchivo.Filter = "Archivos | *." + extension;
                dlgGuardarArchivo.DefaultExt = extension;

                dlgGuardarArchivo.FileName = seleccionado.Text.ToString();
                if (dlgGuardarArchivo.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(dlgGuardarArchivo.FileName))
                    {
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

        private void btnBorrarCarpeta_Click(object sender, EventArgs e)
        {
            if (viewGeneral.SelectedItems.Count > 0)
            {
                ListViewItem seleccionado = viewGeneral.SelectedItems[0];
                string[] prop = seleccionado.Tag.ToString().Split(',');
                if (prop[0] == "D") //obtiene datos del directorio guardados en el listviewitem, si es una carpeta entonces
                {
                    if (seleccionado.Text != "..")
                    {
                        ushort clusterCarpeta = Convert.ToUInt16(prop[1].ToString());
                        bool borrado = borrarCarpeta(clusterCarpeta);
                        if (borrado)
                        {
                            MessageBox.Show("Borrado Con Exito!",
                                        "Informacion",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                            if (viewRootDirectory)
                            {
                                leerRootDirectory();
                                cargarViewGeneral(true);
                            }else
                            {
                                Directory actual = listaDirectorioActual.First();
                                cargarCarpeta(actual.startingCluster);
                                cargarViewGeneral(false);
                            }
                        }else
                        {
                            MessageBox.Show("No se puede eliminar la carpeta porque no esta vacia!",
                                        "Informacion",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        #endregion

        #region //-------- CARGAR INFORMACION DE DISCO ---------
        /// <summary>
        /// Lee el MBR
        /// </summary>
        public void leerInfoDisco()
        {
            byte[] temporalArray;
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
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

        /// <summary>
        /// Lee el Directorio Raiz
        /// </summary>
        public void leerRootDirectory()
        {
            listaRootDirectory.Clear();
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
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

        /// <summary>
        /// Lee las tablas FAT
        /// </summary>
        public void leerTablasFat()
        {
            fat1List.Clear();
            fat2List.Clear();
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
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

        /// <summary>
        /// Calcula el espacio libre en el archivo
        /// </summary>
        /// <returns></returns>
        public int calcularEspacioLibre()
        {
            int clustersVacios = fat1List.Count(x => x.entradaFAT == 0);
            return 16 * clustersVacios; //multiplicar cantidad de clusters libres por 16KB para saber el espacio libre
        }

        /// <summary>
        /// Carga el directorio en el view 
        /// </summary>
        /// <param name="isRoot">true=cargar directorio raiz; false = cargar directorio actual</param>
        public void cargarViewGeneral(bool isRoot)
        {
            viewGeneral.Items.Clear();
            List<Directory> listaDirectorios = new List<Directory>();
            int inicioContador = 0;
            if (!isRoot)
            {
                viewRootDirectory = false;
                listaDirectorios = listaDirectorioActual;
                Directory directorioPadre = listaDirectorios.ElementAt(0);
                string[] info = { "..", "", "" };
                carpetaActual2 = directorioPadre;
                inicioContador = 1;
                ListViewItem nod = new ListViewItem(info, 0);
                nod.Tag = "D," + directorioPadre.Padre.startingCluster;
                viewGeneral.Items.Add(nod);
            }
            else
            {
                inicioContador = 0;
                viewRootDirectory = true;
                listaDirectorios = listaRootDirectory;
            }

           for(int i = inicioContador; i < listaDirectorios.Count(); i++)
            {
                var file = listaDirectorios.ElementAt(i);

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
                        nod.Tag = "D," + file.startingCluster;
                        viewGeneral.Items.Add(nod);
                    }
                    else if (Convert.ToChar(file.fileAttributes) == 'A')
                    {
                        string a = Encoding.ASCII.GetString(file.filename);
                        a = a + "." + Encoding.ASCII.GetString(file.filenameExt);
                        string nombre = a.Replace("\0", string.Empty);

                        DateTime fechaCreacion = file.getFecha(file.fecha_Creado);
                        DateTime horaCreacion = file.getHora(file.hora_Creado);

                        //string[] info = { nombre,
                        //                  fechaCreacion.ToShortDateString() + ' ' + horaCreacion.ToShortTimeString(),
                        //                  file.fileSize.ToString()
                        //                };

                        //ListViewItem nod = new ListViewItem(info, 1);
                        ushort clusterDirectorioArchivo;
                        if (viewRootDirectory)
                        {
                            clusterDirectorioArchivo = file.startingCluster;
                        }else
                        {
                            clusterDirectorioArchivo = clusterPosicionByte(file.posicionByte);
                        }
                        ////
                        string[] info = { nombre,
                                          fechaCreacion.ToShortDateString() + ' ' + horaCreacion.ToShortTimeString(),
                                          clusterDirectorioArchivo.ToString()
                                        };

                        ListViewItem nod = new ListViewItem(info, 1);
                        ////

                        nod.Tag = "A," + clusterDirectorioArchivo;
                        viewGeneral.Items.Add(nod);
                    }
                }
            }
        }

        /// <summary>
        /// obtiene una posicion libre en el directorio raiz
        /// </summary>
        /// <returns>posicion en bytes</returns>
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

        /// <summary>
        /// carga una carpeta con sus subdirectorios
        /// </summary>
        /// <param name="cluster">numero de cluster de la carpeta</param>
        public void cargarCarpeta(ushort cluster)
        {
            long posicion = posicionByteCluster(cluster);
            Directory carpeta = new Directory();
            carpeta = leerEntradaDirectorio(posicion);
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = posicion + 64;
                int limiteCluster = (tamanioCluster - 64) / 2;
                for (int j = 0; j < limiteCluster; j++)
                {
                    ushort apuntadorFAT = reader.ReadUInt16();
                    carpeta.subDirectorio.Add(apuntadorFAT);
                }
            }
            carpetaActual2 = carpeta;
            listaDirectorioActual.Clear();
            listaDirectorioActual.Add(carpeta);
            var listaDirectorios = getSubdirectorio(carpeta.subDirectorio);
            listaDirectorioActual.AddRange(listaDirectorios.ToList());
        }

        /// <summary>
        /// Carga el arbol de indice en memoria
        /// </summary>
        public void cargarIndice()
        {
            long tamanio;
            using (BinaryReader reader = new BinaryReader(new FileStream(Constants.discoIndice, FileMode.Open)))
            {
                tamanio = reader.BaseStream.Length;
            }

            if (tamanio == 1)
            {
                arbolIndice = new ArbolBPlus();
            }else
            {
                arbolIndice = OperacionesArbol.cargarArbol();
            }
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
                using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                
            }else
            {
                MessageBox.Show("No se puede crear la carpeta!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
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
                using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
                {
                    stream.BaseStream.Position = posicionByte;

                    stream.Write(carpetaActual2.filename);
                    stream.Write(carpetaActual2.filenameExt);
                    stream.Write(carpetaActual2.fileAttributes);
                    stream.Write(carpetaActual2.NT);
                    stream.Write(carpetaActual2.millisegundos_Creado);
                    stream.Write(carpetaActual2.hora_Creado);
                    stream.Write(carpetaActual2.fecha_Creado);
                    stream.Write(carpetaActual2.fecha_ultimoAcceso);
                    stream.Write(carpetaActual2.reservedFAT32);
                    stream.Write(carpetaActual2.hora_ultimaEscritura);
                    stream.Write(carpetaActual2.fecha_ultimaEscritura);
                    stream.Write(carpetaActual2.startingCluster);
                    stream.Write(carpetaActual2.fileSize);

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

                using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                    
                    using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                        using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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

                string a = Encoding.ASCII.GetString(Archivo.filename);
                string nombreArchivo = a.Replace("\0", string.Empty);
                nombreArchivo = nombreArchivo + "."+ Encoding.ASCII.GetString(Archivo.filenameExt);
                Nodo entradaIndice = new Nodo();
                entradaIndice.entradaHoja(nombreArchivo, clusterInicioArchivo,1);
                arbolIndice.insertarNodo(entradaIndice);
                OperacionesArbol.guardarArbol(arbolIndice);
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
        public byte[] sacarArchivoDeRoot(ushort cluster)
        {
            var archivo = listaRootDirectory.Where(x => x.startingCluster == cluster).FirstOrDefault();
            byte[] result = new byte[archivo.fileSize];
            long posicionInicioCluster = 0;
            ushort[] clusters = clustersArchivo(archivo);
            if (clusters.Length == 1)
            {
                posicionInicioCluster = posicionByteCluster(archivo.startingCluster);
                using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
                {
                    reader.BaseStream.Position = posicionInicioCluster;
                    reader.Read(result, 0, (int)archivo.fileSize);
                }
            }
            else
            {
                for (int i = 0; i < clusters.Length; i++)
                {
                    posicionInicioCluster = posicionByteCluster(clusters[i]);
                    using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
                    {
                        reader.BaseStream.Position = posicionInicioCluster;
                        int offset = 0;
                        if (i > 0) offset = (i * tamanioCluster);

                        if (i == clusters.Length - 1)
                        {
                            long bytesSobrantes = archivo.fileSize - (tamanioCluster * (clusters.Length - 1));
                            reader.Read(result, offset, (int)bytesSobrantes);
                        }
                        else
                        {
                            reader.Read(result, offset, tamanioCluster);
                        }

                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sacar un archivo del disco
        /// </summary>
        /// <returns>byte array con el archivo</returns>
        public byte[] sacarArchivo(ushort cluster)
        {
            //////////arreglar aqui
            long posicion = posicionByteCluster(cluster);
            var archivo = leerEntradaArchivo(posicion);

            byte[] result = new byte[archivo.fileSize];
            long posicionInicioCluster=0;
            ushort[] clusters =  clustersArchivo(archivo);

            if (archivo.fileSize < tamanioCluster - 32)
            {
                posicionInicioCluster = posicion + 32;
                using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
                {
                    reader.BaseStream.Position = posicionInicioCluster;
                    reader.Read(result, 0, (int)archivo.fileSize);
                }
            }
            else
            {
                if (clusters.Length == 1)
                {
                    posicionInicioCluster = posicionByteCluster(archivo.startingCluster);
                    using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
                    {
                        reader.BaseStream.Position = posicionInicioCluster;
                        reader.Read(result, 0, (int)archivo.fileSize);
                    }
                }
                else
                {
                    for (int i = 0; i < clusters.Length; i++)
                    {
                        posicionInicioCluster = posicionByteCluster(clusters[i]);
                        using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
                        {
                            reader.BaseStream.Position = posicionInicioCluster;
                            int offset = 0;
                            if (i > 0) offset = (i * tamanioCluster);

                            if (i == clusters.Length - 1)
                            {
                                long bytesSobrantes = archivo.fileSize - (tamanioCluster * (clusters.Length - 1));
                                reader.Read(result, offset, (int)bytesSobrantes);
                            }
                            else
                            {
                                reader.Read(result, offset, tamanioCluster);
                            }

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
                    using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                    setmarcadorCluster(clusterDirectoryEntry, 1);
                    ushort[] clustersAsignados = asignarClustersArchivo(Archivo.fileSize);
                    
                    setmarcadorCluster(clusterDirectoryEntry, clustersAsignados[0]);
                    Archivo.startingCluster = clustersAsignados[0];
                    long posicionClusterArchivo = posicionByteCluster(clusterDirectoryEntry);
                    using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                        using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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
                agregarSubDirectorioCarpetaActual(clusterDirectoryEntry);
                
                string a = Encoding.ASCII.GetString(Archivo.filename);
                string nombreArchivo = a.Replace("\0", string.Empty);
                nombreArchivo = nombreArchivo + "." + Encoding.ASCII.GetString(Archivo.filenameExt);
                Nodo entradaIndice = new Nodo();
                entradaIndice.entradaHoja(nombreArchivo, clusterDirectoryEntry,0);
                arbolIndice.insertarNodo(entradaIndice);
                OperacionesArbol.guardarArbol(arbolIndice);
            }
            else
            {
                MessageBox.Show("No se puede guardar el archivo!"
                                    , "Informacion"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
            return clusterDirectoryEntry;
        }
        
        /// <summary>
        /// Borrar un Archivo del disco
        /// </summary>
        /// <param name="cluster">cluster del archivo</param>
        public void borrarArchivo(ushort cluster)
        {
            long posicion = posicionByteCluster(cluster);
            var archivo = leerEntradaArchivo(posicion);
            ushort[] clusters = clustersArchivo(archivo);
            foreach(ushort c in clusters)
            {
                setmarcadorCluster(c, 0);
            }
            borrarSubDirectorioCarpetaActual(cluster);
            setmarcadorCluster(cluster, 0);
            leerTablasFat();
        }

        public bool borrarCarpeta(ushort cluster)
        {
            Directory tempCarpetaActual = carpetaActual2;
            List<Directory> templistaDirectorioActual = new List<Directory>();
            templistaDirectorioActual.AddRange(listaDirectorioActual);
            Directory carpetaAborrar = new Directory();
            List<Directory> listaDirectorioCarpetaABorrar = new List<Directory>();

            cargarCarpeta(cluster);

            carpetaAborrar = carpetaActual2;
            carpetaActual2 = tempCarpetaActual;

            listaDirectorioCarpetaABorrar.AddRange(listaDirectorioActual);
            listaDirectorioActual.Clear();
            listaDirectorioActual.AddRange(templistaDirectorioActual);

            int conteo = carpetaAborrar.subDirectorio.Where(x => x != 0).Count();
            if (conteo == 0)
            {
                if (viewRootDirectory)
                {
                    var archivo = listaRootDirectory.Where(x => x.startingCluster == cluster).FirstOrDefault();
                    Directory carpetaenblanco = new Directory();
                    using (BinaryWriter stream = new BinaryWriter(File.Open(Constants.discoDefault, FileMode.Open)))
                    {
                        stream.BaseStream.Position = archivo.posicionByte;
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
                    setmarcadorCluster(cluster, 0);
                }
                else
                {
                    setmarcadorCluster(cluster, 0);
                    borrarSubDirectorioCarpetaActual(cluster);
                    
                }
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// Borrar un Archivo del root directory
        /// </summary>
        /// <param name="cluster">cluster del archivo</param>
        public void borrarArchivoRootDirectory(ushort cluster)
        {
            var archivo = listaRootDirectory.Where(x => x.startingCluster == cluster).FirstOrDefault();
            Directory carpetaenblanco = new Directory();
            using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = archivo.posicionByte;
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
            ushort[] clusters = clustersArchivo(archivo);
            foreach (ushort c in clusters)
            {
                setmarcadorCluster(c, 0);
            }
            leerTablasFat();
        }

        /// <summary>
        /// Leer una entrada de directorio directamente por la posicion en bytes de la entrada
        /// </summary>
        /// <returns>Un objeto Directorio con la entrada de directorio</returns>
        public Directory leerEntradaDirectorio(long poscionBytes)
        {
            Directory result = new Directory();
            result.Padre = new Directory(); 
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
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
        /// Lee una entrada de directorio de un archivo
        /// </summary>
        /// <param name="poscionBytes">la posicion en bytes de la entrada</param>
        /// <returns>clase directorio con la informacion del archivo leido</returns>
        public Directory leerEntradaArchivo(long poscionBytes)
        {
            Directory result = new Directory();
            result.Padre = new Directory();
            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = poscionBytes;
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
                result.posicionByte = poscionBytes;
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
                using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
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
                    char attr = Convert.ToChar(entradaDirectorio.Padre.fileAttributes);
                    if (attr == 'A')
                    {
                        entradaDirectorio.posicionByte = posicionBytes;
                        entradaDirectorio.filename = entradaDirectorio.Padre.filename;
                        entradaDirectorio.filenameExt = entradaDirectorio.Padre.filenameExt;
                        entradaDirectorio.fileAttributes = entradaDirectorio.Padre.fileAttributes;
                        entradaDirectorio.NT = entradaDirectorio.Padre.NT;
                        entradaDirectorio.millisegundos_Creado = entradaDirectorio.Padre.millisegundos_Creado;
                        entradaDirectorio.hora_Creado = entradaDirectorio.Padre.hora_Creado;
                        entradaDirectorio.fecha_Creado = entradaDirectorio.Padre.fecha_Creado;
                        entradaDirectorio.fecha_ultimoAcceso = entradaDirectorio.Padre.fecha_ultimoAcceso;
                        entradaDirectorio.reservedFAT32 = entradaDirectorio.Padre.reservedFAT32;
                        entradaDirectorio.hora_ultimaEscritura = entradaDirectorio.Padre.hora_ultimaEscritura;
                        entradaDirectorio.fecha_ultimaEscritura = entradaDirectorio.Padre.fecha_ultimaEscritura;
                        entradaDirectorio.startingCluster = entradaDirectorio.Padre.startingCluster;
                        entradaDirectorio.fileSize = entradaDirectorio.Padre.fileSize;
                        entradaDirectorio.Padre = null;
                    }
                    else
                    {
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
                }
                result.Add(entradaDirectorio);
            }


            return result;
        }

        /// <summary>
        /// Agrega una entrada al subdirectorio de la carpeta actual
        /// </summary>
        /// <param name="cluster">cluster del subdirectorio o archivo</param>
        public void agregarSubDirectorioCarpetaActual(ushort cluster)
        {
            int entradaLibre = 0;
            int limiteCluster = (tamanioCluster - 64) / 2;
            long posicion = carpetaActual2.posicionByte + 64;
            
            using (BinaryReader reader = new BinaryReader(new FileStream(Constants.discoDefault, FileMode.Open)))
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

            using (BinaryWriter stream = new BinaryWriter(File.Open(Constants.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = (posicion) + (entradaLibre*2);
                stream.Write(cluster);
            }
        }

        /// <summary>
        /// Borra una entrada en el subdirectorio de la carpeta actual
        /// </summary>
        /// <param name="cluster">Numero del cluster a borrar</param>
        public void borrarSubDirectorioCarpetaActual(ushort cluster)
        {
            int posicionCluster = 0;
            int limiteCluster = (tamanioCluster - 64) / 2;
            long posicion = carpetaActual2.posicionByte + 64;

            using (BinaryReader reader = new BinaryReader(new FileStream( Constants.discoDefault, FileMode.Open)))
            {
                reader.BaseStream.Position = posicion;

                for (int j = 0; j < limiteCluster; j++)
                {
                    ushort apuntadorFAT = reader.ReadUInt16();
                    if (apuntadorFAT == cluster)
                    {
                        posicionCluster = j;
                        break;
                    }
                }
            }

            using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
            {
                stream.BaseStream.Position = (posicion) + (posicionCluster * 2);
                ushort vacio = new ushort();
                stream.Write(vacio);
            }
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
            using (BinaryWriter stream = new BinaryWriter(File.Open( Constants.discoDefault, FileMode.Open)))
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

        /// <summary>
        /// Obtiene el numero de cluster a partir de la posicion en bytes
        /// </summary>
        /// <param name="posicionBytes">Posicion en Bytes</param>
        /// <returns>Numero de Cluster</returns>
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
            if (clusterInicio > 0)
            {
                listaClusters.Add(clusterInicio);
                ushort contadorCluster = clusterInicio;
                do
                {
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
            }else
            {
                ushort cluster = clusterPosicionByte(archivo.posicionByte);
                listaClusters.Add(cluster);
            }
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

            for (int j = 0; j < cantidadClusters; j++)
            {
                clusterAsignado[j] = buscarClusterVacio();
                //marcar como ocupado temporalmente para que no lo devuelva como vacio
                setmarcadorCluster(clusterAsignado[j], 1);
            }
            return clusterAsignado;
        }


        #endregion

        private void frmExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperacionesArbol.guardarArbol(arbolIndice);
        }
    }
}
