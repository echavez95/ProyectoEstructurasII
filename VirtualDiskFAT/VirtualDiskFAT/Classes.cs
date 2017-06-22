using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDiskFAT
{
    [Serializable]
    public class MBR
    {
        public byte[] jumpInstruction = new byte[3];
        public byte[] oemID = new byte[8];
        ///BPB
        public short bytesxSector = new short();
        public byte sectorxCluster = new byte();
        public short reservedSectors = new short();
        public byte numberOfFATs = new byte();
        public short rootEntries = new short();
        public short smallSectors = new short();
        public byte mediaDescriptor = new byte();
        public short sectorxFATs = new short();
        public short sectorxTrack = new short();
        public short numberOfHeads = new short();
        public int hiddenSectors = new int();
        public int largeSectors = new int();
        ///BPB
        ///Extended BPB
        public byte physicalDriveNo = new byte();
        public byte reserved = new byte();
        public byte extBootSignature = new byte();
        public int serialNo = new int();
        public byte[] volumeLabel = new byte[11];
        public byte[] fileSystemType = new byte[8];
        ///Extended BPB
        public byte[] bootstrapCode = new byte[448];
        public short endOfSector = new short();

        public MBR()
        {

        }
        public void llenarDatosMBR()
        {
            oemID = Encoding.ASCII.GetBytes("MSWIN4.1");
            //BPB
            bytesxSector = 512;
            sectorxCluster = 32;
            reservedSectors = 512;
            numberOfFATs = 1;
            rootEntries = 512;
            smallSectors = 0;
            mediaDescriptor = 0xf8;
            sectorxFATs = 256;
            sectorxTrack = 0;
            numberOfHeads = 0;
            hiddenSectors = 0;
            largeSectors = 2097152;
            ///BPB
            ///Extended BPB
            physicalDriveNo = 1;
            reserved = 0;
            extBootSignature = 29;
            serialNo = 1995123;
            byte[] vlabel = Encoding.ASCII.GetBytes("DiscoLocal");
            Array.Resize<byte>(ref vlabel, 11);
            volumeLabel = vlabel;
            byte[] fstype = Encoding.ASCII.GetBytes("FAT16");
            Array.Resize<byte>(ref fstype, 8);
            fileSystemType = fstype;
            endOfSector = 1;
        }
    }

    public class decodedMBR
    {
        public byte[] jumpInstruction { get; set; }
        public string oemID { get; set; }
        ///BPB
        public short bytesxSector { get; set; }
        public byte sectorxCluster { get; set; }
        public short reservedSectors { get; set; }
        public byte numberOfFATs { get; set; }
        public short rootEntries { get; set; }
        public short smallSectors { get; set; }
        public byte mediaDescriptor { get; set; }
        public short sectorxFATs { get; set; }
        public short sectorxTrack { get; set; }
        public short numberOfHeads { get; set; }
        public int hiddenSectors { get; set; }
        public int largeSectors { get; set; }
        ///BPB
        ///Extended BPB
        public byte physicalDriveNo { get; set; }
        public byte reserved { get; set; }
        public byte extBootSignature { get; set; }
        public int serialNo { get; set; }
        public string volumeLabel { get; set; }
        public string fileSystemType { get; set; }
        public byte[] bootstrapCode { get; set; }
        public short endOfSector { get; set; }
        public decodedMBR()
        {

        }

    }

    [Serializable]
    public class FAT16
    {
        public ushort entradaFAT { get; set; }

        public FAT16()
        {

            clusterLibre();
        }

        public void clusterLibre()
        {
            entradaFAT = new ushort();
        }

        public void clusterOcupado(Nullable<ushort> siguienteCluster) //si el parametro es nulo el cluster se marca como ocupado y fin del alchivo
        {
            if (siguienteCluster != null)
            {
                entradaFAT = (ushort)siguienteCluster;
            }
            else
            {
                entradaFAT = 1;
            }
        }

        public void clusterReservado()
        {
            entradaFAT = 2;
        }
        public void clusterMalo()
        {
            entradaFAT = 3;
        }
        
    }

    [Serializable]
    public class Directory
    {
        public Directory Padre { get; set; }
        public byte[] filename { get; set; }
        public byte[] filenameExt { get; set; } //3
        public byte fileAttributes { get; set; }
        public byte NT { get; set; }
        public byte millisegundos_Creado { get; set; }
        public ushort hora_Creado { get; set; }
        public ushort fecha_Creado { get; set; }
        public ushort fecha_ultimoAcceso { get; set; }
        public ushort reservedFAT32 { get; set; }
        public ushort hora_ultimaEscritura { get; set; }
        public ushort fecha_ultimaEscritura { get; set; }
        public ushort startingCluster { get; set; }
        public uint fileSize { get; set; }

        /// <summary>
        /// Posicion en Bytes del directorio
        /// </summary>
        public long posicionByte { get; set; }
        /// <summary>
        /// Lista de apuntadores de cluster de los archivos dentro de la carpeta
        /// </summary>
        public List<ushort> subDirectorio { get; set; } 

        public Directory()
        {
            filename = new byte[8];
            filenameExt = new byte[3];
            fileAttributes = new byte();
            NT = new byte();
            millisegundos_Creado = new byte();
            hora_Creado = new ushort();
            fecha_Creado = new ushort();
            fecha_ultimoAcceso = new ushort();
            reservedFAT32 = new ushort();
            hora_ultimaEscritura = new ushort();
            fecha_ultimaEscritura = new ushort();
            startingCluster = new ushort();
            fileSize = new uint();

            subDirectorio = new List<ushort>();
        }

        public void nuevoArchivo(string nombreArchivo, char atributo, DateTime creado, ushort clusterInicio, uint tamanio)
        {
            string[] FileAndExtension = nombreArchivo.Split('.');
            byte[] temp = Encoding.ASCII.GetBytes(FileAndExtension[0]);
            Array.Resize<byte>(ref temp, 8);
            filename = temp;
            temp = Encoding.ASCII.GetBytes(FileAndExtension[1]);
            Array.Resize<byte>(ref temp, 3);
            filenameExt = temp;
            fileAttributes = Convert.ToByte(atributo);
            NT = 0;
            millisegundos_Creado = Convert.ToByte(creado.Millisecond / 10);

            hora_Creado = setHoras(creado);
            fecha_Creado = setDias(creado);
            fecha_ultimoAcceso = setDias(creado);
            reservedFAT32 = 0;
            hora_ultimaEscritura = setHoras(creado);
            fecha_ultimaEscritura = setDias(creado);
            startingCluster = clusterInicio;
            fileSize = tamanio;
        }
        public void nuevaCarpeta(Directory padre, string nombreCarpeta, DateTime creado)
        {
            Padre = padre;
            byte[] temp = Encoding.ASCII.GetBytes(nombreCarpeta);
            Array.Resize<byte>(ref temp, 8);
            filename = temp;
            filenameExt = new byte[3];
            fileAttributes = Convert.ToByte('D');
            NT = 0;
            millisegundos_Creado = Convert.ToByte(creado.Millisecond / 10);
            hora_Creado = setHoras(creado);
            fecha_Creado = setDias(creado);
            fecha_ultimoAcceso = setDias(creado);
            reservedFAT32 = 0;
            hora_ultimaEscritura = setHoras(creado);
            fecha_ultimaEscritura = setDias(creado);
            startingCluster = 0;
            fileSize = 0;

            subDirectorio = new List<ushort>();
        }

        public void cambiarNombreCarpeta(string nombreCarpeta)
        {
            byte[] temp = Encoding.ASCII.GetBytes(nombreCarpeta);
            Array.Resize<byte>(ref temp, 8);
            filename = temp;
        }
        public void setclusterSubdirectorio(ushort nCluster)
        {
            startingCluster = nCluster;
        }
        public ushort setDias(DateTime fecha)
        {
            DateTime fechabase = DateTime.Parse("30/01/2017");
            ushort result = (ushort)(fecha - fechabase).TotalDays;
            return result;
        }
        public ushort setHoras(DateTime fecha)
        {
            DateTime fechabase = DateTime.Parse("00:00AM");
            TimeSpan TSpan = fecha.Subtract(fechabase);
            return (ushort)TSpan.TotalMinutes; ;
        }

        public DateTime getFecha(ushort valor)
        {
            DateTime fechabase = DateTime.Parse("30/01/2017");
            DateTime fecha =  fechabase.AddDays(valor);
            return fecha;
        }

        public DateTime getHora(ushort valor)
        {
            DateTime fechabase = DateTime.Parse("00:00AM");
            DateTime fecha = fechabase.AddMinutes(valor);
            return fecha;
        }
    }
    //    public class decodedDirectory
    //    {
    //        public string filename { get; set; }
    //        public string fileAttributes { get; set; }
    //        DateTime fecha_creacion { get; set; }
    //        DateTime fecha_ultimoAcceso { get; set; }
    //        DateTime fecha_ultimaEscritura { get; set; }
    //        public uint fileSize { get; set; }

    //        public void decodedRootDir(Directory directory)
    //        {

    //        }
    //    }
}

