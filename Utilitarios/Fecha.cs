using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public class Fecha
    {
        /// <summary>
        /// Convierte una cadena del formato dd/mm/aaa al tipo DateTime
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public static DateTime Convertir(string fecha)
        {
            var dia = fecha.Substring(0, 2);
            var mes = fecha.Substring(3, 2);
            var anio = fecha.Substring(6);
            var fechaConvertida = new DateTime(int.Parse(anio), int.Parse(mes), int.Parse(dia));
            return fechaConvertida;
        }
    }
}
