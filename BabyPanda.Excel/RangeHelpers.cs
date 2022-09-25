using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Excel
{
    public static class RangeHelpers
    {

        /// <summary>
        /// Writes an array to excel based on the row/column position of the top corner cell
        /// Ignores the rest of the range. Cells outside the range can be overwritten based on the size of the array.
        /// Returns the final range.
        /// </summary>
        public static Range WriteGridValues<T>(this Range startCell, T[,] values)
        {
            var worksheet = startCell.Worksheet;
            int row = startCell.Row;
            int col = startCell.Column;
            var cell1 = (Range)worksheet.Cells[row, col];
            var cell2 = (Range)worksheet.Cells[row + values.GetLength(0) - 1, col + values.GetLength(1) - 1];
            Range rng = worksheet.get_Range(cell1, cell2);
            rng.Value = values;
            return rng;
        }

        /// <summary>
        /// Writes a list to excel as a column of values based on the row/column position of the top corner cell
        /// Ignores the rest of the range. Cells outside the range can be overwritten based on the size of the array.
        /// </summary>
        public static Range WriteValuesAsColumn<T>(this Range startCell, IList<T> values)
        {
            var worksheet = startCell.Worksheet;
            int row = startCell.Row;
            int col = startCell.Column;
            var valueArray = new T[values.Count, 1];
            for (int i = 0; i < values.Count; i++)
            {
                valueArray[i, 0] = values[i];
            }
            return WriteGridValues(startCell, valueArray);
        }

        /// <summary>
        /// Writes a list to excel as a row of values based on the row/column position of the top corner cell
        /// Ignores the rest of the range. Cells outside the range can be overwritten based on the size of the array.
        /// </summary>
        public static void WriteValuesToRow<T>(this Range range, IEnumerable<T> values) => WriteValuesToRow(range, values.ToArray());

        /// <summary>
        /// Writes a list to excel as a row of values based on the row/column position of the top corner cell
        /// Ignores the rest of the range. Cells outside the range can be overwritten based on the size of the array.
        /// </summary>
        public static Range WriteValuesToRow<T>(this Range startCell, T[] values)
        {
            var worksheet = startCell.Worksheet;
            int row = startCell.Row;
            int col = startCell.Column;
            var cell1 = (Range)worksheet.Cells[row, col];
            var cell2 = (Range)worksheet.Cells[row, col + values.Length - 1];
            Range rng = worksheet.get_Range(cell1, cell2);
            rng.Value = values;
            return rng;
        }

        public static object[] GetValues(this Range range)
        {
            var val = range.Value2;

            if (val is Array array)
                return array.Cast<object>().ToArray();
            else
                return new object[] { val };
        }

        public static string[] GetValuesAsString(this Range range)
        {
            var val = range.Value2;

            if(val is Array array)
            {
                string[] strArray = new string[array.Length];
                int i = 0;
                foreach (var item in array)
                {
                    if (item == null) 
                        strArray[i] = "";
                    else
                        strArray[i] = item.ToString();
                    i++;
                }
                return strArray;
            }
            else
            {
                if (val == null)
                    return new string[] { "" };
                else
                    return new string[] { val.ToString() };
            }
        }

        /// <summary>
        /// 
        /// Replaces strings, boolean and error types with the errorValue.
        /// </summary>
        public static double[] GetValuesAsDoubles(this Range range, double errorValue = double.NaN)
        {
            var val = range.Value2;

            if (val is Array array)
            {
                //Is this better or worse??
                double[] dArray = new double[array.Length];
                int i = 0;
                foreach (var item in array)
                {
                    if (item is double d)
                        dArray[i] = d;
                    else
                        dArray[i] = errorValue;
                    i++;
                }
                return dArray;
            }
            else
            {
                if (val is double d)
                    return new double[] { d };
                else
                    return new double[] { errorValue };
            }
        }

        public static bool IsRangeEmpty(this Range range)
        {
            var values = range.Value;
            if (values == null) return true;

            if (values is object[,] array)
            {
                foreach (var cell in array)
                {
                    if (cell != null) return false;
                }
                return true;
            }

            return (object)values == null;

        }

    }
}
