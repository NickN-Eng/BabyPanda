using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BabyPanda
{
    public static class DataTableHelpers
    {
        public static DataTable GetExcelDataFromClipboard(bool FirstRowAsHeaders)
        {
            var cText = Clipboard.GetText();

            if (string.IsNullOrEmpty(cText)) return null;

            var rowTxt = cText
                .Split(new string[] { "\r\n" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (rowTxt.Count == 0) return null;

            var firstRow = rowTxt[0].Split('\t');

            var table = new DataTable();

            if (FirstRowAsHeaders)
            {
                foreach (var txt in firstRow)
                {
                    table.Columns.Add(GetPermissableColumnName(txt, table.Columns), typeof(string));
                }
            }
            else
            {
                for (int i = 0; i < firstRow.Length; i++)
                    table.Columns.Add("Col "+i.ToString(), typeof(string));

                table.Rows.Add(firstRow);
            }

            for (int i = 1; i < rowTxt.Count; i++)
            {
                var iRowTxt = rowTxt[i].Split('\t');
                //Add more validation??
                table.Rows.Add(iRowTxt);
            }

            return table;
        }

        public static string GetPermissableColumnName(string proposedName, DataColumnCollection columns)
        {
            if (columns.Contains(proposedName))
                return GetPermissableColumnName(proposedName + "#", columns);

            return proposedName;
        }

        /// <summary>
        /// Gets values from a DataTable as an array of objects.
        /// If getEmptyValuesAsNull is true, the DBNull.Value will be converted into null.
        /// </summary>
        public static object[] GetColumnAsArray(DataTable table, string columnName, bool getEmptyValuesAsNull = true)
        {
            var col = table.Columns[columnName];
            return GetColumnAsArray(table, col, getEmptyValuesAsNull);
        }

        /// <summary>
        /// Gets values from a DataTable as an array of objects.
        /// If getEmptyValuesAsNull is true, the DBNull.Value will be converted into null.
        /// </summary>
        public static object[] GetColumnAsArray(DataTable table, DataColumn col, bool getEmptyValuesAsNull = true)
        {
            var colIndex = table.Columns.IndexOf(col);
            return GetColumnAsArray(table, colIndex, getEmptyValuesAsNull);
        }

        /// <summary>
        /// Gets values from a DataTable as an array of objects.
        /// If getEmptyValuesAsNull is true, the DBNull.Value will be converted into null.
        /// </summary>
        public static object[] GetColumnAsArray(DataTable table, int colIndex, bool getEmptyValuesAsNull = true)
        {
            var array = new object[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var value = table.Rows[i][colIndex];
                if (getEmptyValuesAsNull && value == DBNull.Value)
                    value = null;
                array[i] = value;
            }
            return array;
        }


        public static T[] GetColumnAsTypedArray<T>(DataTable table, string columnName)
        {
            var col = table.Columns[columnName];
            var colIndex = table.Columns.IndexOf(col);
            var array = new T[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                array[i] = (T)table.Rows[i][colIndex];
            }
            return array;
        }

        public static DataTable Convert(DataFrame frame)
        {
            var table = new DataTable();
            foreach (var col in frame.Columns)
            {
                //Need to get the non_nullable version since DataTable does not support nullable types like Point3d?.
                var non_nullable = TypeHelpers.GetDataType_NonNullable(col.DataType);
                table.Columns.Add(col.Name, non_nullable);

                //table.Columns.Add(col.Name, col.DataType);
            }
            foreach (var row in frame.Rows)
            {
                table.Rows.Add(row.ToArray());
            }
            return table;
        }

        public static DataFrame Convert(DataTable table)
        {
            var frame = new DataFrame();
            for (int iCol = 0; iCol < table.Columns.Count; iCol++)
            {
                var col = table.Columns[iCol];
                var colArray = GetColumnAsArray(table, iCol);

                var tf = TypeMaster.GetTypeFunction(col.DataType);
                if (tf == null)
                    throw new System.Exception("Type function could not be found for the type " + col.DataType.ToString());


                var dfCol = tf.Create_Column(col.ColumnName, colArray);
                frame.Columns.Add(dfCol);
            }
            return frame;
        }

        public static bool TryCovertColumnType(DataTable dt, string columnName, Type newType)
        {
            var newColName = columnName + Guid.NewGuid().ToString();
            var oldCol = dt.Columns[columnName];
            var oldColType = oldCol.DataType;
            var typeFunction = TypeMaster.GetTypeFunction(oldColType);
            if (typeFunction == null) return false;
            if (!typeFunction.PermittedConversionTypes().Contains(newType)) return false;

            using (DataColumn newCol = new DataColumn(newColName, newType))
            {
                // Add the new column which has the new type, and move it to the ordinal of the old column
                int ordinal = oldCol.Ordinal;
                dt.Columns.Add(newCol);
                newCol.SetOrdinal(ordinal);

                var oldColIndex = dt.Columns.IndexOf(oldCol);
                var newColIndex = dt.Columns.IndexOf(newCol);

                // Get and convert the values of the old column, and insert them into the new
                foreach (DataRow dr in dt.Rows)
                {
                    object value = dr[oldColIndex];
                    bool convResult = typeFunction.TryConvert_ToObject(value, newType, out object convObj);

                    dr[newColIndex] = (convResult && convObj != null) ? convObj : DBNull.Value;
                }

                // Remove the old column
                dt.Columns.RemoveAt(oldColIndex);

                // Give the new column the old column's name
                newCol.ColumnName = columnName;
            }

            return true;
        }
    }
}
