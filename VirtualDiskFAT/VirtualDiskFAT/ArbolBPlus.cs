using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDiskFAT
{
    
    [Serializable]
    public class ArbolBPlus
    {
        public Hoja raiz { get; set; }

        public ArbolBPlus()
        {
            raiz = new Hoja();
            raiz.nuevaHoja();
        }
        public void insertarNodo(Nodo nuevoNodo)
        {
            Nodo resultado = OperacionesArbol.insertarNodo(nuevoNodo, raiz);

            if (resultado != null)
            {
                if (raiz.indice == false)
                {
                    raiz = new Hoja();
                    raiz.nuevoIndice();
                    raiz.Nodos.Add(resultado);
                }
                else
                {
                    raiz.Nodos.Add(resultado);
                    raiz.Nodos = raiz.Nodos.OrderBy(x => x.nombre).ToList();

                    if (raiz.Nodos.Count() > Constants.ordenArbol)
                    {
                        Nodo promovido = OperacionesArbol.Promover(raiz);
                        raiz = new Hoja();
                        raiz.nuevoIndice();
                        raiz.Nodos.Add(promovido);
                    }
                }
            }

        }



    }

    [Serializable]
    public class Nodo
    {
        public string nombre { get; set; }
        public ushort cluster { get; set; }
        public byte ubicacion { get; set; }
        public Hoja hijosDer { get; set; }
        public Hoja hijosIzq { get; set; }

        public Nodo()
        {

        }
        public void entradaIndice(string nombreArchivo, Hoja izquierdo, Hoja derecho)
        {
            nombre = nombreArchivo;
            cluster = 0;
            hijosIzq = izquierdo;
            hijosDer = derecho;
        }

        public void entradaHoja(string nombreArchivo, ushort clusterArchivo, byte esRoot)
        {
            nombre = nombreArchivo;
            cluster = clusterArchivo;
            ubicacion = esRoot;
            hijosIzq = null;
            hijosDer = null;
        }
    }

    [Serializable]
    public class Hoja
    {
        public bool indice { get; set; }
        public List<Nodo> Nodos { get; set; }
        public Hoja hojaSiguiente { get; set; }

        public Hoja()
        {

        }
        public void nuevoIndice()
        {
            indice = true;
            Nodos = new List<Nodo>();
            hojaSiguiente = null;
        }

        public void nuevaHoja()
        {
            indice = false;
            Nodos = new List<Nodo>();
            hojaSiguiente = null;
        }
    }


    public static class OperacionesArbol
    {
        public static List<Nodo> buscarNombre(string nombre,Hoja hoja)
        {
            if (hoja.indice == true)
            {
                int tamanioHoja = hoja.Nodos.Count();
                for (int i = 0; i < tamanioHoja; i++)
                {
                    Nodo nodoActual = hoja.Nodos.ElementAt(i);

                    int c = string.Compare(nombre, nodoActual.nombre);
                    if (c == -1)
                    {
                        List<Nodo> encontrado = buscarNombre(nombre, nodoActual.hijosIzq);
                        return encontrado;
                    }
                    else
                    {
                        if (i != tamanioHoja - 1)
                        {
                            Nodo nodoSiguiente = hoja.Nodos.ElementAt(i + 1);
                            c = string.Compare(nombre, nodoSiguiente.nombre);
                            if (c == -1)
                            {
                                List<Nodo> encontrado = buscarNombre(nombre, nodoActual.hijosDer);
                                return encontrado;
                            }
                            else
                            {
                                List<Nodo> encontrado = buscarNombre(nombre, nodoSiguiente.hijosDer);
                                return encontrado;
                            }
                        }
                        else
                        {
                            List<Nodo> encontrado = buscarNombre(nombre, nodoActual.hijosDer);
                            return encontrado;
                        }
                    }
                }
            }
            else
            {
                List<Nodo> encontrado = hoja.Nodos.Where(x=>x.nombre.Contains(nombre)).ToList();
                return encontrado;
            }
            return null;
        }

        public static Nodo insertarNodo(Nodo nuevoNodo, Hoja hoja)
        {
            if (hoja.indice == false)
            {
                hoja.Nodos.Add(nuevoNodo);
                hoja.Nodos = hoja.Nodos.OrderBy(x => x.nombre).ToList();
                if (hoja.Nodos.Count() > Constants.ordenArbol)
                {
                    Nodo promovido = Promover(hoja);
                    return promovido;
                }
                else
                {
                    return null;
                }
            }
            else //si la raiz actual es un indice
            {
                int tamanioHoja = hoja.Nodos.Count();
                for (int i = 0; i < tamanioHoja; i++)
                {
                    Nodo nodoActual = hoja.Nodos.ElementAt(i);

                    int c = string.Compare(nuevoNodo.nombre, nodoActual.nombre);
                    if (c == -1)
                    {
                        Nodo promovido = insertarNodo(nuevoNodo, nodoActual.hijosIzq);
                        if (promovido != null)
                        {
                            nodoActual.hijosIzq = promovido.hijosDer;
                        }
                        return promovido;
                    }
                    else
                    {
                        if (i != tamanioHoja - 1)
                        {
                            Nodo nodoSiguiente = hoja.Nodos.ElementAt(i + 1);
                            c = string.Compare(nuevoNodo.nombre, nodoSiguiente.nombre);
                            if (c == -1)
                            {
                                Nodo promovido = insertarNodo(nuevoNodo, nodoActual.hijosDer);
                                if (promovido != null)
                                {
                                    nodoActual.hijosDer = new Hoja();
                                    nodoActual.hijosDer = promovido.hijosIzq;
                                    nodoSiguiente.hijosIzq = promovido.hijosDer;
                                }
                                return promovido;
                            }
                            else
                            {
                                Nodo promovido = insertarNodo(nuevoNodo, nodoSiguiente.hijosDer);
                                if (promovido != null)
                                {
                                    nodoSiguiente.hijosDer = new Hoja();
                                    nodoSiguiente.hijosDer = promovido.hijosIzq;
                                }
                                return promovido;
                            }
                        }
                        else
                        {
                            Nodo promovido = insertarNodo(nuevoNodo, nodoActual.hijosDer);
                            if (promovido != null)
                            {
                                nodoActual.hijosDer = new Hoja();
                                nodoActual.hijosDer = promovido.hijosIzq;
                            }
                            return promovido;
                        }
                    }
                }
                return null;
            }
        }
        public static Nodo Promover(Hoja hoja)
        {
            Nodo nodoaPromover;
            int posicionMedio = Constants.ordenArbol / 2;
            nodoaPromover = hoja.Nodos.ElementAt(posicionMedio);
            Hoja hijosIzq = new Hoja();
            Hoja hijosDer = new Hoja();

            if (hoja.indice)
            {
                hijosIzq.nuevoIndice();
                hijosDer.nuevoIndice();
                hoja.Nodos.RemoveAt(posicionMedio);

            }
            else
            {
                hijosIzq.nuevaHoja();
                hijosDer.nuevaHoja();
            }

            hijosIzq.Nodos = hoja.Nodos.Take(posicionMedio).ToList();
            hijosDer.Nodos = hoja.Nodos.Skip(posicionMedio).ToList();

            if (hoja.indice)
            {
                hijosIzq.hojaSiguiente = null;
            }
            else
            {
                hijosIzq.hojaSiguiente = hijosDer;
            }
            nodoaPromover.hijosIzq = hijosIzq;
            nodoaPromover.hijosDer = hijosDer;
            return nodoaPromover;
        }

        public static void imprimirArbol(Hoja raiz)
        {
            recorridoRecursivo(raiz);
            Console.Write("\n");
        }

        public static void recorridoRecursivo(Hoja hoja)
        {

            if (hoja.indice == true)
            {
                foreach (Nodo n in hoja.Nodos)
                {
                    Console.Write("\n");

                    string nombre = n.nombre;

                    Console.WriteLine(nombre);


                    Console.Write("\tIzquierda\n");

                    recorridoRecursivo(n.hijosIzq);


                    Console.Write("\tDerecha\n");
                    recorridoRecursivo(n.hijosDer);

                    Console.WriteLine("----------------------------");
                }
            }
            else
            {
                Console.Write("\t");
                foreach (Nodo n in hoja.Nodos)
                {
                    string nombre = n.nombre;
                    Console.Write(nombre + " ");
                }
                Console.Write("\n");

            }
        }
        public static void guardarArbol(ArbolBPlus arbol)
        {
            string serializationFile = Constants.discoIndice;
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, arbol);
            }
        }

        public static ArbolBPlus cargarArbol()
        {
            string serializationFile = Constants.discoIndice;
            ArbolBPlus result = new ArbolBPlus();
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                result = (ArbolBPlus)bformatter.Deserialize(stream);
            }
            return result;
        }
    }

}
