using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public static class Cadena
    {
        public static string[] ConvertirEnArreglo(string texto, char separador)
        {
            List<string> resultado = new List<string>();
            StringBuilder sb = new StringBuilder();
            char ultimaLetra = ' ';
            foreach (char letra in texto)
            {
                if (letra != separador)
                {
                    sb.Append(letra);
                }
                else
                {
                    AgregarResultado(resultado, sb);
                    sb.Remove(0, sb.Length);
                }
            }
            if (ultimaLetra != separador)
            {
                AgregarResultado(resultado, sb);
            }
            resultado.Sort();
            return resultado.ToArray();
        }

        public static void AgregarResultado(List<string> resultado, StringBuilder sb)
        {
            if (!String.IsNullOrEmpty(sb.ToString()))
            {
                resultado.Add(sb.ToString().Replace("\r", ""));
            }
        }

        public static string EliminarDuplicado(string cadena, char duplicado)
        {
            StringBuilder sb = new StringBuilder();
            char ultimoCaracter = duplicado;
            foreach (char letra in cadena)
            {
                if (letra != ultimoCaracter)
                {
                    sb.Append(letra);
                    ultimoCaracter = letra;
                }
                else
                {
                    if (ultimoCaracter != duplicado)
                    {
                        sb.Append(letra);
                        ultimoCaracter = letra;
                    }
                }
            }
            return sb.ToString();
        }

        public static string QuitarComentarios(string cadena)
        {
            string resultado = cadena;
            while (true)
            {
                int inicio = resultado.IndexOf("/*");
                if (inicio >= 0)
                {
                    int fin = resultado.IndexOf("*/");
                    if (fin >= 0)
                    {
                        if (fin + 2 > resultado.Length)
                            resultado = resultado.Substring(0, inicio);
                        else
                            resultado = resultado.Substring(0, inicio) + resultado.Substring(fin + 2);
                    }
                }
                if (resultado.IndexOf("/*") < 0)
                {
                    break;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Encuentra un texto en un arreglo y devuelve el valor encontrado en el arreglo.
        /// La comparación es case-insensitive
        /// </summary>
        /// <param name="texto">Texto a buscar</param>
        /// <param name="arreglo">Lista de valores donde se ha de buscar.</param>
        /// <returns>Devuelve el valor encontrado. Si no lo encuentra, devuelve cadena vacía.</returns>
        public static string EncontrarTextoEnArreglo(string texto, string[] arreglo)
        {
            string resultado = "";
            foreach (string valor in arreglo)
            {
                if (texto.ToUpper() == valor.ToUpper())
                {
                    resultado = valor;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Devuelve la ubicación de la enésima coincidencia de un caracter sobre una cadena
        /// </summary>
        /// <param name="caracter">Caracter a buscar</param>
        /// <param name="cadena">Cadena donde se ha de buscar</param>
        /// <param name="enesimaCoincidencia">Número de coincidencia</param>
        /// <returns>Retorna la ubicación de la coincidencia, si no lo encuentra retorna -1.</returns>
        public static int EncontrarEnesimaCoincidencia(char caracter, string cadena, int enesimaCoincidencia)
        {
            int count = 0;
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] == caracter)
                {
                    count++;
                    if (count == enesimaCoincidencia)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }


    }
}
