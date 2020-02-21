using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;

namespace Utilitarios
{
    public static class SVN
    {
        public static string[] ListarArchivos(string ruta, string usuario, string clave)
        {
            return ListarItemsProxy(ruta, usuario, clave, SvnNodeKind.File);
        }

        public static string[] ListarCarpetas(string ruta, string usuario, string clave)
        {
            return ListarItemsProxy(ruta, usuario, clave, SvnNodeKind.Directory);
        }

        private static string[] ListarItemsProxy(string ruta, string usuario, string clave, SvnNodeKind tipoObjeto)
        {
            SvnClient svnClient = new SvnClient();
            svnClient.Authentication.DefaultCredentials = new NetworkCredential(usuario, clave);
            SvnUriTarget svnUriTarget = new SvnUriTarget(ruta);
            string[] listaItems = getItems(svnClient, svnUriTarget, tipoObjeto);
            return listaItems;
        }

        public static bool TraerObjeto(string origen, string destino, string usuario, string clave, out string respuesta, int revision = -1)
        {
            try
            {
                destino = destino.ToUpper();
                // Obtener la ruta del repositorio
                int index = Cadena.EncontrarEnesimaCoincidencia('/', origen, 4);
                string repositorio = origen.Substring(0, index);
                string instancia = repositorio.Substring(repositorio.LastIndexOf('/') + 1);

                // Verificar si existen subcarpetas
                if (origen.Substring(index + 1).IndexOf('/') > 0)
                {
                    // Existe una subcarpeta
                    // Obtener lista de subcarpetas
                    string[] subCarpetas = ListarCarpetas(repositorio, usuario, clave);
                    string subCarpeta = origen.Substring(index + 1, Cadena.EncontrarEnesimaCoincidencia('/', origen, 5) - index - 1);
                    string subCarpetaCorregida = Cadena.EncontrarTextoEnArreglo(subCarpeta, subCarpetas);
                    if (String.IsNullOrEmpty(subCarpetaCorregida))
                    {
                        respuesta = String.Format("No se encuentra la carpeta {0} en el versionador", subCarpeta);
                        return false;
                    }
                    string urlCorregidaBase = String.Concat(repositorio, "/", subCarpetaCorregida);

                    // Obtener lista de objetos
                    string[] objetos = ListarArchivos(urlCorregidaBase, usuario, clave);
                    string objeto = origen.Substring(Cadena.EncontrarEnesimaCoincidencia('/', origen, 5) + 1);
                    string objetoCorregido = Cadena.EncontrarTextoEnArreglo(objeto, objetos);
                    if (String.IsNullOrEmpty(objetoCorregido))
                    {
                        respuesta = String.Format("No se encuentra el objeto {0} en el versionador", objeto);
                        return false;
                    }
                    string urlCorregido = String.Concat(urlCorregidaBase, "/", objetoCorregido);

                    // Verificar si la carpeta del repositorio=instancia existe
                    if (!Directory.Exists(destino))
                    {
                        // No existe la carpeta, crearla
                        Directory.CreateDirectory(destino);
                    }

                    // Verificar si el archivo existe
                    string rutaArchivoLocal = String.Concat(destino, "\\", objetoCorregido);
                    if (File.Exists(rutaArchivoLocal))
                    {
                        // Si existe el archivo, eliminarlo
                        File.Delete(rutaArchivoLocal);
                    }

                    // Traer el objeto a la carpeta destino
                    SvnClient svnClient = new SvnClient();
                    svnClient.Authentication.ForceCredentials(usuario, clave);
                    //svnClient.Authentication.Clear();
                    //svnClient.Authentication.DefaultCredentials =  //new NetworkCredential(usuario, clave);
                    if (revision == -1)
                    {
                        svnClient.Export(urlCorregido, rutaArchivoLocal);
                    }
                    else
                    {
                        SvnRevision rev = new SvnRevision(revision);
                        svnClient.Export(urlCorregido, rutaArchivoLocal, new SvnExportArgs() { Revision = rev });
                    }
                }
                else
                {

                    // El archivo se encuentra en la raíz del repositorio.
                    string[] objetos = ListarArchivos(repositorio, usuario, clave);
                    string objeto = origen.Substring(Cadena.EncontrarEnesimaCoincidencia('/', origen, 4) + 1);
                    string objetoCorregido = Cadena.EncontrarTextoEnArreglo(objeto, objetos);
                    if (String.IsNullOrEmpty(objetoCorregido))
                    {
                        respuesta = String.Format("No se encuentra el objeto {0} en el versionador", objeto);
                        return false;
                    }
                    string urlCorregido = String.Concat(instancia, "\\", objetoCorregido);

                    // Verificar si el archivo existe
                    string rutaArchivoLocal = String.Concat(instancia, "\\", repositorio, "\\", objetoCorregido);
                    if (File.Exists(rutaArchivoLocal))
                    {
                        // Si existe el archivo, eliminarlo
                        File.Delete(rutaArchivoLocal);
                    }

                    // Traer el objeto a la carpeta destino
                    SvnClient svnClient = new SvnClient();
                    svnClient.Authentication.DefaultCredentials = new NetworkCredential(usuario, clave);
                    svnClient.Export(urlCorregido, destino);
                }
                respuesta = String.Format("{0} Copiado satisfactoriamente", origen);
                return true;
            }
            catch (SvnRepositoryIOException ex)
            {
                respuesta = ex.Message;
                return false;
            }
        }

        private static string[] getItems(SvnClient client, SvnTarget folderTarget, SvnNodeKind tipoObjeto)
        {
            List<String> filesFound = new List<String>();
            Collection<SvnListEventArgs> listResults;

            if (client.GetList(folderTarget, out listResults))
            {
                foreach (SvnListEventArgs item in listResults)
                    if (item.Entry.NodeKind == tipoObjeto)
                    {
                        if (!String.IsNullOrEmpty(item.Path))
                            filesFound.Add(item.Path);
                    }

                return filesFound.ToArray();
            }
            else
                throw new Exception("Failed to retrieve files via SharpSvn");
        }
    }
}
