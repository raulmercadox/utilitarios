using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public static class Encriptador
    {
        //private byte[] _iv;
        //private byte[] _key;

        //public Encriptador()
        //{
        //_key = new byte[] {220, 20, 134, 246, 194, 240, 189, 191, 253, 148, 224, 123, 226, 150, 17, 177 };
        //_iv = new byte[] {244, 49, 39, 10, 57, 225, 93, 56};
        //}

        public static string Encriptar(string texto)
        {
            byte[] _key = new byte[] { 220, 20, 134, 246, 194, 240, 189, 191, 253, 148, 224, 123, 226, 150, 17, 177 };
            byte[] _iv = new byte[] { 244, 49, 39, 10, 57, 225, 93, 56 };
            return Encriptar(texto, _key, _iv);
        }

        public static string Encriptar(string texto, byte[] _key, byte[] _iv)
        {
            RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
            ICryptoTransform encriptador = rC2.CreateEncryptor(_key, _iv);
            MemoryStream memoria = new MemoryStream();
            CryptoStream cs = new CryptoStream(memoria, encriptador, CryptoStreamMode.Write);

            byte[] encriptar = Encoding.Default.GetBytes(texto);
            cs.Write(encriptar, 0, encriptar.Length);
            cs.FlushFinalBlock();
            byte[] encriptado = memoria.ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in encriptado)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

        public static string Desencriptar(string texto)
        {
            byte[] _key = new byte[] { 220, 20, 134, 246, 194, 240, 189, 191, 253, 148, 224, 123, 226, 150, 17, 177 };
            byte[] _iv = new byte[] { 244, 49, 39, 10, 57, 225, 93, 56 };
            return Desencriptar(texto, _key, _iv);
        }

        public static string Desencriptar(string texto, byte[] _key, byte[] _iv)
        {
            List<byte> codigos = new List<byte>();
            foreach (char letra in texto)
            {
                codigos.Add((byte)letra);
            }

            RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
            ICryptoTransform desencriptador = rC2.CreateDecryptor(_key, _iv);
            MemoryStream memoria = new MemoryStream(codigos.ToArray());
            CryptoStream cs = new CryptoStream(memoria, desencriptador, CryptoStreamMode.Read);

            StringBuilder sb = new StringBuilder();
            int b = 0;
            do
            {
                b = cs.ReadByte();
                if (b != -1)
                {
                    sb.Append((char)b);
                }
            } while (b != -1);

            return sb.ToString();
        }

        public static byte[] ObtenerKey()
        {
            RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
            return rC2.Key;
        }

        public static byte[] ObtenerIV()
        {
            RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
            return rC2.IV;
        }

        public static bool CumplePoliticaClave(string clave)
        {
            if (clave.Trim().Length < 8)
                return false;

            if (!ExisteMayuscula(clave))
                return false;

            if (!ExisteMinuscula(clave))
                return false;

            if (!ExisteDigitos(clave))
                return false;

            if (!ExisteSimbolos(clave))
                return false;

            return true;
        }

        private static bool ExisteSimbolos(string cadena)
        {
            bool existe = false;
            char[] simbolos = GenerarSimbolos();
            foreach (char letra in cadena)
            {
                foreach (char simbolo in simbolos)
                {
                    if (letra == simbolo)
                    {
                        existe = true;
                        break;
                    }
                }
            }
            return existe;
        }

        private static bool ExisteDigitos(string cadena)
        {
            bool existe = false;
            char[] digitos = GenerarDigitos();
            foreach (char letra in cadena)
            {
                foreach (char digito in digitos)
                {
                    if (letra == digito)
                    {
                        existe = true;
                        break;
                    }
                }
            }
            return existe;
        }

        private static bool ExisteMayuscula(string cadena)
        {
            bool existe = false;
            char[] mayusculas = GenerarMayusculas();
            foreach (char letra in cadena)
            {
                foreach (char mayus in mayusculas)
                {
                    if (letra == mayus)
                    {
                        existe = true;
                        break;
                    }
                }
            }
            return existe;
        }

        private static bool ExisteMinuscula(string cadena)
        {
            bool existe = false;
            char[] minuscula = GenerarMinusculas();
            foreach (char letra in cadena)
            {
                foreach (char minus in minuscula)
                {
                    if (letra == minus)
                    {
                        existe = true;
                        break;
                    }
                }
            }
            return existe;
        }

        private static char[] GenerarMayusculas()
        {
            int letraA = Convert.ToInt32('A');
            int letraZ = Convert.ToInt32('Z');
            int ancho = letraZ - (letraA - 1);
            char[] letras = new char[ancho];
            for (int i = letraA; i <= letraZ; i++)
            {
                char letra = Convert.ToChar(i);
                letras[i - letraA] = letra;
            }
            return letras;
        }

        private static char[] GenerarMinusculas()
        {
            int letraA = Convert.ToInt32('a');
            int letraZ = Convert.ToInt32('z');
            int ancho = letraZ - (letraA - 1);
            char[] letras = new char[ancho];
            for (int i = letraA; i <= letraZ; i++)
            {
                char letra = Convert.ToChar(i);
                letras[i - letraA] = letra;
            }
            return letras;
        }

        private static char[] GenerarDigitos()
        {
            char[] letras = new char[10];
            for (int i = 0; i <= 9; i++)
            {
                letras[i] = Convert.ToChar(i.ToString());
            }
            return letras;
        }

        private static char[] GenerarSimbolos()
        {
            string simbolos = "!#$%&()*+-=?[]{}|~@";
            return simbolos.ToCharArray();
        }

        public static string EncriptarMD5(string cadena)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(cadena));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
