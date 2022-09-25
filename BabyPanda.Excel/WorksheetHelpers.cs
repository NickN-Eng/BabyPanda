using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Excel
{
    public static class WorksheetHelpers
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">Index, starting at 1 for row 1</param>
        /// <param name="col">Index starting at 1 for column A</param>
        /// <param name="value">Single value to be written</param>
        /// <param name="worksheet">Optional worksheet</param>
        public static void WriteValueToCell(this Worksheet worksheet, int row, int col, object value)
        {
            var rng = (Range)worksheet.Cells[row, col];
            rng.Value = value;
        }

        public static void WriteValuesToColumn(this Worksheet worksheet, int row, int col, IList<object> values)
        {
            var valueArray = new object[values.Count, 1];
            for (int i = 0; i < values.Count; i++)
            {
                valueArray[i, 0] = values[i];
            }
            WriteGridValues(worksheet, row, col, valueArray);
        }

        public static void WriteValuesToRow(Worksheet worksheet, int row, int col, IEnumerable<object> values) => WriteValuesToRow(worksheet, row, col, values.ToArray());

        public static void WriteValuesToRow(Worksheet worksheet, int row, int col, object[] values)
        {
            var cell1 = (Range)worksheet.Cells[row, col];
            var cell2 = (Range)worksheet.Cells[row, col + values.Length - 1];
            Range rng = worksheet.get_Range(cell1, cell2);
            rng.Value = values;
        }


        /// <summary>
        /// Writes an array to excel based on the row/column position of the top corner cell
        /// </summary>
        public static void WriteGridValues(this Worksheet worksheet, int row, int col, object[,] values)
        {
            var cell1 = (Range)worksheet.Cells[row, col];
            var cell2 = (Range)worksheet.Cells[row + values.GetLength(0) - 1, col + values.GetLength(1) - 1];
            Range rng = worksheet.get_Range(cell1, cell2);
            rng.Value = values;
        }


        public static Range GetRange(this Worksheet worksheet, int row, int col, int height = 1, int width = 1)
        {
            var cell1 = (Range)worksheet.Cells[row, col];
            var cell2 = (Range)worksheet.Cells[row + height - 1, col + width - 1];
            return worksheet.get_Range(cell1, cell2);
        }
    }
}
