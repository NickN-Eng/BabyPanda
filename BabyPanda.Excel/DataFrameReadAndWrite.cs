using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyPanda;
using Microsoft.Data.Analysis;
using Microsoft.Office.Interop.Excel;

namespace BabyPanda.Excel
{
    public static class DataFrameReadAndWrite
    {
        /// <summary>
        /// Returns null if there is no table by that name.
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataFrame ReadDFFromXlTable(Workbook wb, string tableName)
        {
            var listObject = wb.GetTable(tableName);
            if (listObject == null) return null;

            var columns = new DataFrameColumn[listObject.ListColumns.Count];

            int i = 0;
            foreach (ListColumn col in listObject.ListColumns)
            {
                var range = col.Range;

                var values = range.GetValuesAsString();
                
                var strCol = new StringDataFrameColumn(col.Name, values);

                columns[i] = strCol;
                i++;
            }

            return new DataFrame(columns);
        }

        /// <summary>
        /// Writes a dataframe to a given table name in excel.
        /// Table must already exist by the provided name.
        /// Returns false if the table could not be found.
        /// </summary>
        /// <param name="wb">The workbook to write to</param>
        /// <param name="df">Write this dataframe.</param>
        /// <param name="tableName">The name of the excel table.</param>
        /// <param name="append">Set to true to append rows after the last empty row.</param>
        /// <returns></returns>
        public static bool WriteDFToXlTable(Workbook wb, DataFrame df, string tableName, bool append = true)
        {
            var listObject = wb.GetTable(tableName);
            if (listObject == null) return false;

            ListRows rows = listObject.ListRows;
            if (rows.Count == 0)
            {
                rows.AddEx();
            }

            int appendRow;

            if (append)
            {
                appendRow = FindAppendableEmptyRow(rows);
            }
            else
            {
                appendRow = rows[1].Range.Row;
            }

            ListColumn prevListCol = null;
            foreach (var col in df.Columns)
            {
                var colName = col.Name;
                ListColumn listCol = listObject.GetColumn(colName);
                if (listCol == null)
                {
                    //An attempt to insert consecutive columns together.
                    if (prevListCol != null) listCol = listObject.ListColumns.Add(prevListCol.Index + 1);
                    else listCol = listObject.ListColumns.Add();

                    listCol.Name = col.Name;
                }

                var startCell = listObject.Range.Worksheet.Cells[appendRow, listCol.Range.Column];
                WriteDFColumnToExcelRange(col, startCell);

                prevListCol = listCol;
            }

            return true;
        }

        /// <summary>
        /// Finds the LOWEST empty row, returns the row index.
        /// </summary>
        private static int FindAppendableEmptyRow(ListRows rows)
        {
            for (int i = rows.Count; i > 0; i--)
            {
                var iRow = rows[i];
                if (!iRow.Range.IsRangeEmpty())
                {
                    string addr = iRow.Range.Address;
                    return iRow.Range.Row + 1;
                }
            }

            //If every row is empty, use the first row.
            return rows[1].Range.Row;
        }


        private static void WriteDFColumnToExcelRange(DataFrameColumn col, Range rng)
        {
            if (col is DoubleDataFrameColumn || col is Int32DataFrameColumn || col is BooleanDataFrameColumn || col is StringDataFrameColumn)
            {
                rng.WriteValuesAsColumn(col.Cast<object>().ToArray());
            }
            else
            {
                var tf = TypeMaster.GetTypeFunction(col.DataType);
                var strings = tf.ConvertToDisplayStringArray(col);
                rng.WriteValuesAsColumn(strings);
            }
        }

        /// <summary>
        /// Writes a dataframe to an excel range, by the first cell.
        /// </summary>
        /// <param name="df"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static Range WriteDFToXlRange(DataFrame df, Range rng)
        {
            var arrayRows = df.Rows.Count;
            var arrayCols = df.Columns.Count;
            var array = new object[arrayRows + 1, arrayCols];

            for (int iCol = 0; iCol < df.Columns.Count; iCol++)
            {
                var col = df.Columns[iCol];
                array[0, iCol] = col.Name;

                if (col is DoubleDataFrameColumn || col is Int32DataFrameColumn || col is BooleanDataFrameColumn || col is StringDataFrameColumn)
                {
                    for (int jRow = 0; jRow < arrayRows; jRow++)
                    {
                        array[jRow + 1, iCol] = col[jRow];
                    }
                }
                else
                {
                    var tf = TypeMaster.GetTypeFunction(col.DataType);
                    for (int jRow = 0; jRow < arrayRows; jRow++)
                    {
                        var str = tf.ToDisplayString(col[jRow]);
                        array[jRow + 1, iCol] = str;
                    }
                }
            }

            return rng.WriteGridValues(array);
        }

        public static ListObject WriteDFToXlRange_AsNewTable(DataFrame df, Range rng, string tableName)
        {
            var tableRange = WriteDFToXlRange(df, rng);
            var sheet = rng.Worksheet;
            ListObject table = sheet.ListObjects.AddEx(
                                SourceType: XlListObjectSourceType.xlSrcRange,
                                Source: tableRange,
                                XlListObjectHasHeaders: XlYesNoGuess.xlYes);
            table.Name = tableName;
            return table;
        }

    }
}
