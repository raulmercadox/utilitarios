using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public class Archivo
    {
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static void Descargar(string file, string destino)
        {
            try
            {
                Uri serverUri = new Uri(file);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return;
                }
                if (!Directory.Exists(destino))
                {
                    Directory.CreateDirectory(destino);
                }
                string nombreArchivo = file.Substring(file.LastIndexOf("/") + 1);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(file));
                //reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                FileStream writeStream = new FileStream(destino + "\\" + nombreArchivo, FileMode.Create);
                int Length = 2048;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }
                writeStream.Close();
                response.Close();
            }
            catch
            {
                throw;
            }
        }

        public static string LeerArchivoTexto(string archivo)
        {
            try
            {
                using (StreamReader sr = new StreamReader(archivo,Encoding.Default))
                {
                    string contenido = sr.ReadToEnd();
                    return contenido;
                }
            }
            catch
            {
                throw;
            }
        }

        public static void GrabarEnArchivo(string nombreArchivo, byte[] contenido)
        {
            var nombreCarpeta = Archivo.ObtenerRuta(nombreArchivo);
            if (!Directory.Exists(nombreCarpeta))
            {
                Directory.CreateDirectory(nombreCarpeta);
            }
            FileStream fileStream = new FileStream(nombreArchivo, FileMode.Create, FileAccess.Write);
            fileStream.Write(contenido, 0, contenido.Length);
            fileStream.Flush();
            fileStream.Close();
        }

        public static void GrabarEnArchivo(string nombreArchivo, string contenido)
        {
            using (StreamWriter sw = new StreamWriter(nombreArchivo,false,Encoding.Default))
            {
                sw.Write(contenido);
            }
        }

        public static string LeerParametro(string archivo, string key)
        {
            return LeerParametro(archivo, key, '=');
        }

        public static string LeerParametro(string archivo, string key, char separador)
        {
            string resultado = "";
            string cadena = LeerArchivoTexto(archivo);
            string[] valores = Cadena.ConvertirEnArreglo(cadena, '\n');
            foreach (string valor in valores)
            {
                string[] par = valor.Split(new char[] { separador });
                if (par[0] == key)
                {
                    resultado = par[1];
                    break;
                }
            }
            return resultado;
        }

        public static string[] ObtenerLineas(string texto)
        {
            string[] lines = texto.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }

        public static string ObtenerExtension(string archivo)
        {
            var indice = archivo.LastIndexOf('.');
            if (indice >= 0)
            {
                return archivo.Substring(indice + 1);
            }
            else
            {
                return "";
            }
        }

        public static string ObtenerArchivo(string rutaCompleta)
        {
            var indice = rutaCompleta.LastIndexOf('\\');
            var archivo = rutaCompleta.Substring(indice + 1);
            return archivo;
        }

        public static string ObtenerRuta(string rutaCompleta)
        {
            var indice = rutaCompleta.LastIndexOf('\\');
            var ruta = rutaCompleta.Substring(0, indice);
            return ruta;
        }
    }
}
