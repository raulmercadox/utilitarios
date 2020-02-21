using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public class Oracle
    {
        public static bool ValidarNombreObjeto(string texto, string declarador, string nombreArchivo)
        {
            string[] partes = ObtenerPartes(texto, declarador);

            if (partes.Length == 2)
            {
                string nombreObjeto = partes[1].Replace("\"", "");

                return nombreObjeto.ToUpper() == nombreArchivo.ToUpper();
            }
            else
            {
                return false;
            }
        }

        public static bool ValidarComillasEnObjeto(string texto, string declarador)
        {
            bool rsta = true;

            string[] partes = ObtenerPartes(texto, declarador);

            #region Validar las comillas en el esquema
            if (String.Compare(partes[0][0].ToString(), "\"") == 0)
            {
                if (String.Compare(partes[0][partes[0].Length - 1].ToString(), "\"") != 0)
                {
                    return false; // Existe las comillas al comienzo pero no al final
                }
            }

            if (String.Compare(partes[0][partes[0].Length - 1].ToString(), "\"") == 0)
            {
                if (String.Compare(partes[0][0].ToString(), "\"") != 0)
                {
                    return false; // Existe las comillas al final pero no al comienzo
                }
            }
            #endregion


            if (partes.Length == 2)
            {
                #region Validar las comillas en el objeto
                if (String.Compare(partes[1][0].ToString(), "\"") == 0)
                {
                    if (String.Compare(partes[1][partes[1].Length - 1].ToString(), "\"") != 0)
                    {
                        return false; // Existe las comillas al comienzo pero no al final
                    }
                }

                if (String.Compare(partes[1][partes[1].Length - 1].ToString(), "\"") == 0)
                {
                    if (String.Compare(partes[1][0].ToString(), "\"") != 0)
                    {
                        return false; // Existe las comillas al final pero no al comienzo
                    }
                }
                #endregion

                #region Validar si existen las comillas en esquema y no en objeto o viceversa
                if ((String.Compare(partes[0][0].ToString(), "\"") == 0 && String.Compare(partes[1][0].ToString(), "\"") != 0)
                    || (String.Compare(partes[0][0].ToString(), "\"") != 0 && String.Compare(partes[1][0].ToString(), "\"") == 0))
                {
                    return false;
                }
                #endregion
            }

            return rsta;
        }

        private static string[] ObtenerPartes(string texto, string declarador)
        {
            int anchoDeclarador = declarador.Length;

            int indiceDeclarador = texto.IndexOf(declarador);

            int inicioBusqueda = indiceDeclarador + anchoDeclarador + 1;

            string textoNuevo = texto.Substring(inicioBusqueda).Trim();

            int indiceFinal = textoNuevo.IndexOf(" ");
            string nombreObjeto = String.Empty;

            if (indiceFinal >= 0)
                nombreObjeto = textoNuevo.Substring(0, textoNuevo.IndexOf(" "));
            else
                nombreObjeto = textoNuevo;

            string[] partes = nombreObjeto.Split(new char[] { '.' });
            return partes;
        }

        public static bool ExisteEsquemaEnObjeto(string esquema, string texto, string declarador)
        {
            bool rsta = true;

            int anchoDeclarador = declarador.Length;

            int indiceDeclarador = texto.IndexOf(declarador);

            //while(indiceDeclarador >= 0)
            //{
            int inicioBusqueda = indiceDeclarador + anchoDeclarador + 1;

            string textoNuevo = texto.Substring(inicioBusqueda).Trim();

            int indiceFinal = textoNuevo.IndexOf(" ");
            string nombreObjeto = String.Empty;

            if (indiceFinal >= 0)
                nombreObjeto = textoNuevo.Substring(0, indiceFinal).Replace("\"", "");
            else
                nombreObjeto = textoNuevo.Substring(0).Replace("\"", "");

            if (nombreObjeto.IndexOf(".") > 0)
            {
                string nombreEsquema = nombreObjeto.Substring(0, nombreObjeto.IndexOf("."));
                if (nombreEsquema != esquema)
                {
                    rsta = false;
                }
            }
            else
            {
                rsta = false;
            }

            return rsta;
        }

        public static string ObtenerDeclarador(string texto, string nombreTipoObjeto)
        {
            try
            {
                if (texto.IndexOf("CREATE OR REPLACE " + nombreTipoObjeto) >= 0)
                {
                    return "CREATE OR REPLACE " + nombreTipoObjeto;
                }
                else if (texto.IndexOf("CREATE " + nombreTipoObjeto) >= 0)
                {
                    return "CREATE " + nombreTipoObjeto;
                }
                else if (texto.IndexOf("REPLACE " + nombreTipoObjeto) >= 0)
                {
                    return "REPLACE " + nombreTipoObjeto;
                }
                else if (texto.IndexOf("ALTER " + nombreTipoObjeto) >= 0 && (String.Compare(nombreTipoObjeto.Trim(), "TABLE") == 0 || String.Compare(nombreTipoObjeto.Trim(), "TYPE") == 0))
                {
                    return "ALTER " + nombreTipoObjeto;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                throw;
            }
        }


        public static bool EsTipoObjeto(string texto, string nombreTipoObjeto)
        {
            try
            {
                if (texto.IndexOf("CREATE " + nombreTipoObjeto) >= 0 || texto.IndexOf("REPLACE " + nombreTipoObjeto) >= 0 ||
                    texto.IndexOf("CREATE OR REPLACE " + nombreTipoObjeto) >= 0)
                    return true;

                else
                    return false;
            }
            catch
            {
                throw;
            }
        }

        public static string PlancharArchivo(string archivo)
        {
            try
            {
                string texto = Archivo.LeerArchivoTexto(archivo);
                string textoQuitadoComentarioBloque = QuitarComentariodeBloque(texto);
                string textoQuitadoComentarioDeLinea = QuitarComentarioDeLinea(textoQuitadoComentarioBloque);
                string textoEliminadoDuplicado = Cadena.EliminarDuplicado(textoQuitadoComentarioDeLinea, ' ');
                string textoMayuscula = textoEliminadoDuplicado.ToUpper();
                return textoMayuscula;
            }
            catch
            {
                throw;
            }
        }

        public static string QuitarComentariodeBloque(string cadena)
        {
            string resultado = cadena;
            int posicionInicial = 0;
            while (true)
            {
                int inicioComentario = resultado.IndexOf("/*", posicionInicial);
                int finComentario = resultado.IndexOf("*/");
                if (inicioComentario < 0 || finComentario < 0 || finComentario < inicioComentario)
                {
                    break;
                }
                int otroInicioComentario = resultado.IndexOf("/*", inicioComentario + 1);
                if (otroInicioComentario >= 0)
                {
                    if (otroInicioComentario > finComentario)
                    {
                        // Significa que el nuevo inicio de comentario está fuera del primer comentario
                        // Se expulsa a la sección del comentario
                        resultado = resultado.Substring(0, inicioComentario) + resultado.Substring(finComentario + 2);
                    }
                    else
                    {
                        // Significa que el nuevo inicio de comentario está anidado dentro del primer comentario
                        posicionInicial = otroInicioComentario;
                    }
                }
                else
                {
                    resultado = resultado.Substring(0, inicioComentario) + resultado.Substring(finComentario + 2);
                    break;
                }
            }
            return resultado;
        }

        public static string QuitarComentarioDeLinea(string texto)
        {
            StringBuilder sb = new StringBuilder();
            // Quita los comentarios de linea y los comentarios de bloque
            string[] lines = texto.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // Busca comentarios de linea
                int indice = line.IndexOf("--");
                if (indice >= 0)
                    sb.AppendLine(line.Substring(0, indice));
                else
                    sb.AppendLine(line);

            }
            return sb.ToString().Replace("\r\n", " ").Replace("\n", " ");
        }
    }
}
