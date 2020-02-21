using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Globalization;
using Microsoft.Office.Core;
using System.Threading;

namespace Utilitarios
{
    public class Excel
    {
        public static string ObtenerValor(Worksheet hoja, int fila, int columna)
        {
            try
            {
                object valorBruto = (hoja.Cells[fila, columna] as Range).Value2;
                string valor = valorBruto == null ? String.Empty : valorBruto.ToString().Trim();
                return valor;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Devuelve en forma de System.Data.DataTable un rango de celdas de Excel
        /// </summary>
        /// <param name="hoja">Nombre de la hoja de Excel donde se busca</param>
        /// <param name="nombreCampo">Nombre del campo a buscar</param>
        /// <param name="numeroColumnas">Número de columnas que contendrá la tabla</param>
        /// <param name="numeroRegistros">Número de registros a obtener, si es 0 se obtienen hasta que el registro se encuentre en blanco</param>
        /// <param name="filaInicial">fila inicial a partir de donde se inicia la búsqueda</param>
        /// <param name="columnaInicial">columna inicial a partir de donde se inicia la búsqueda</param>
        /// <param name="profundidadFila">número de filas a buscar hasta encontrar el nombre del campo</param>
        /// <param name="profundidadColumna">número de columnas a buscar hasta encontrar el nombre del campo</param>
        /// <param name="deltaPrimerRegistro">cuantos filas saltarse para empezar a obtener los datos para la tabla</param>
        /// <param name="caseSensitive">si la búsqueda hace coincidir mayúsculas y minúsculas</param>
        /// <returns></returns>
        public static System.Data.DataTable ObtenerTabla(Worksheet hoja, string nombreCampo, int numeroColumnas, int numeroRegistros = 0, int filaInicial = 1, int columnaInicial = 1, int profundidadFila = 100, int profundidadColumna = 100, int deltaPrimerRegistro = 1, bool caseSensitive = false)
        {
            System.Data.DataTable tabla = new System.Data.DataTable();
            int filaEncontrada = 0;
            int columnaEncontrada = 0;
            bool encontrado = false;
            if (!caseSensitive)
            {
                nombreCampo = nombreCampo.ToUpper();
            }
            try
            {
                for (int fila = filaInicial; fila <= filaInicial + profundidadFila; fila++)
                {
                    for (int columna = columnaInicial; columna <= columnaInicial + profundidadColumna; columna++)
                    {
                        string valor = ObtenerValor(hoja, fila, columna);
                        string valorFinal = !caseSensitive ? valor.ToUpper() : valor;
                        if (String.Compare(nombreCampo, valorFinal) == 0)
                        {
                            filaEncontrada = fila;
                            columnaEncontrada = columna;
                            encontrado = true;
                            break;
                        }
                    }
                    if (encontrado)
                    {
                        break;
                    }
                }
                if (encontrado)
                {
                    int filaDato = filaEncontrada + deltaPrimerRegistro;

                    #region Crear Tabla de Datos
                    for (int i = 1; i <= numeroColumnas; i++)
                    {
                        tabla.Columns.Add(new DataColumn("columna" + i.ToString(), typeof(string)));
                    }
                    #endregion

                    #region Llenar Tabla con Datos
                    if (numeroRegistros > 0)
                    {
                        for (int j = 1; j <= numeroRegistros; j++)
                        {
                            DataRow row = tabla.NewRow();
                            for (int i = 1; i <= numeroColumnas; i++)
                            {
                                row[i - 1] = ObtenerValor(hoja, filaDato + j - 1, columnaEncontrada + i - 1);
                            }
                            tabla.Rows.Add(row);
                        }
                    }
                    else
                    {
                        // Se llena hasta que no se encuentren registros en la primera columna
                        // de la fila procesada
                        int deltaFila = 0;

                        string valorCelda = ObtenerValor(hoja, filaDato, columnaEncontrada);
                        while (!String.IsNullOrEmpty(valorCelda))
                        {
                            DataRow row = tabla.NewRow();
                            for (int i = 1; i <= numeroColumnas; i++)
                            {
                                row[i - 1] = ObtenerValor(hoja, filaDato + deltaFila, columnaEncontrada + i - 1);
                            }
                            tabla.Rows.Add(row);

                            deltaFila++;
                            valorCelda = ObtenerValor(hoja, filaDato + deltaFila, columnaEncontrada);
                        }
                    }
                    #endregion
                }
                return tabla;
            }
            catch
            {
                throw;
            }
        }

        public static System.Data.DataTable ObtenerTabla(string nombreArchivo, string nombreHoja, string nombreCampo, int numeroColumnas, int numeroRegistros = 0, int filaInicial = 1, int columnaInicial = 1, int profundidadFila = 100, int profundidadColumna = 100, int deltaPrimerRegistro = 1, bool caseSensitive = false)
        {
            try
            {
                System.Data.DataTable resultado = null;
                var app = new Application();
                System.Globalization.CultureInfo newCulture = new CultureInfo(app.LanguageSettings.get_LanguageID(MsoAppLanguageID.msoLanguageIDUI));
                Thread.CurrentThread.CurrentCulture = newCulture;
                Workbook wb = app.Workbooks.Open(nombreArchivo, false, true);
                Worksheet hoja = null;
                var numeroHojas = wb.Sheets.Count;
                for (var i = 1; i <= numeroHojas; i++)
                {
                    if (wb.Sheets[i].Name == nombreHoja)
                    {
                        hoja = wb.Sheets[i];
                        break;
                    }
                }
                if (hoja != null)
                {
                    resultado = ObtenerTabla(hoja, nombreCampo, numeroColumnas, numeroRegistros, filaInicial, columnaInicial, profundidadFila, profundidadColumna, deltaPrimerRegistro, caseSensitive);
                }
                wb.Close(false);
                app.Quit();
                return resultado;
            }
            catch
            {
                throw;
            }
        }
    }
}
