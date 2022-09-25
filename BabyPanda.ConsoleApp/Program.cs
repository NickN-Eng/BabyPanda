using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyPanda.WPF;
using BabyPanda;
using BabyPanda;
using System.Windows;
using Microsoft.Data.Analysis;
using System.Data;
using System.Drawing;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using BabyPanda.Excel;
using Microsoft.Office.Interop.Excel;

namespace BabyPanda.ConsoleApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //ExcelCreateSheetDebug();
            //ExcelAreCellsEmptyDebug();
            //ExcelTableWriterDebug();
            //ExcelWriteValuesDebug();
            //ExcelWriteDfDebug();
            //ExcelForceCloseWorkbookDebug();
            //DatagridDialogTest();
            //RegexDebug();
            //TypeMasterDebug();
            //ExcelClipboardDebug();
            //SmartSelectionDialogDebug();
            //TypeFunctionDebug();
            //DataTableNullBehaviour();
            //DataFrameNullBehaviour();
            //DataframeSerialisation();
        }

        private static void DataframeSerialisation()
        {
            var col = new DoubleDataFrameColumn("Col1", new double[] { 1, 122.43, 0 });
            var df = new DataFrame(col);
            var bin = IO_Helpers.Serialise(df);
            var deserDf = IO_Helpers.DeserialiseDataFrame(bin);


        }


        private static T TestDefault<T>()
        {
            return default(T);
        }

        #region Excel workbook testing

        public static string TestWorkbookPath = @"C:\Users\nniem\source\repos\EasyExcel\excelDocs\TestBook1.xlsx";


        private static void ExcelCreateSheetDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);
            //var sht = wb.Sheets["NewSheet"]; //Fails if sheet name does not exist
            Worksheet sht = wb.Sheets.Add(After: wb.Sheets[wb.Sheets.Count]);
            sht.Name = "NewSheet";


        }

        private static void ExcelForceCloseWorkbookDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);
            var wb1 = wb;
            var wb2 = wb;

        }

        private static void ExcelAreCellsEmptyDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);
            Worksheet sheet = wb.Worksheets["AreCellsEmptyTest"];

            var arrayCell1 = sheet.GetRange(1, 1, 1, 3);
            arrayCell1.Value = "hi";
            bool singleEmpty1 = arrayCell1.IsRangeEmpty();

            var arrayCell2 = sheet.GetRange(2, 1, 1,3);
            bool singleEmpt2 = arrayCell2.IsRangeEmpty();

            var arrayCell3 = sheet.GetRange(3, 1, 1,3);
            arrayCell3.Value = " ";
            bool singleEmpty3 = arrayCell3.IsRangeEmpty();
        }

        private static void ExcelWriteValuesDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);
            Worksheet sheet = wb.Worksheets["Sheet1"];

            var rng = sheet.Range["A1"];
            rng.WriteValuesAsColumn(new double?[] { 4, 55.4, null, -34.45 }.Cast<object>().ToArray());
        }

        private static void ExcelWriteDfDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);
            Worksheet sheet = wb.Worksheets["Sheet7"];

            var rng = sheet.Range["B3"];

            var col1 = new StringDataFrameColumn("A", new string[] { "hiiiiii", "biiiiii", null, "hhjjh" });
            var col2 = new DoubleDataFrameColumn("B", new double?[] { 69, null, 69, 007 });
            var col3 = new Int32DataFrameColumn("C", new int?[] { null, 69, 69, 888 });
            var col4 = new BooleanDataFrameColumn("D", new bool?[] { true, null, false, null });
            var dataframe = new DataFrame(col1, col2, col3, col4);


            //var address = DataFrameReadAndWrite.WriteDFToXlRange(dataframe, rng).Address;
            var table = DataFrameReadAndWrite.WriteDFToXlRange_AsNewTable(dataframe, rng, "69696");
        }

        private static void ExcelTableWriterDebug()
        {
            var path = TestWorkbookPath;
            var wbo = new ExcelWbOpener() { Filepath = path };

            var col1 = new StringDataFrameColumn("A", new string[] { "hiiiiii", "biiiiii",null });
            var col2 = new DoubleDataFrameColumn("B", new double?[] { 69, null, 69 });
            var col3 = new Int32DataFrameColumn("C", new int?[] { null, 69, 69 });
            var col4 = new BooleanDataFrameColumn("D", new bool?[] { true, null, false });
            var dataframe = new DataFrame(col1, col2, col3, col4);

            bool success = wbo.Execute(out Workbook wb, out bool isnewwind);

            DataFrameReadAndWrite.WriteDFToXlTable(wb, dataframe, "Table5", append:false);
        }

        private static void ExcelClipboardDebug()
        {
            var result = DataTableHelpers.GetExcelDataFromClipboard(true);
            var result2 = DataTableHelpers.GetExcelDataFromClipboard(false);
        }

        #endregion


        private static void RegexDebug()
        {
            #region Experiment with parsing
            string s = "dsfru 45,45.3e,354";
            string numberOnly = Regex.Replace(s, "[^0-9.,]", "");
            int i = 0;
            #endregion
        }

        private static void TypeMasterDebug()
        {
            var col = TypeMaster.CreateColumn("ni", new double[] { 1, 3, 4, });
            var tfs = TypeMaster.TypeFuctions;
        }

        private static void SmartSelectionDialogDebug()
        {
            var smartSelectionDialog = new SmartSelectionDialog();
            smartSelectionDialog.ShowDialog();
        }


        private static void DataTableNullBehaviour()
        {
            #region Figuring out DataTable null behaviour
            //When setting by cell, use DBNull.Value instead of null.
            //null works for classes, but not value types

            var tab = new System.Data.DataTable();

            tab.Columns.Add("Int", typeof(int));
            tab.Columns.Add("Double", typeof(double));
            tab.Columns.Add("Bool", typeof(bool));
            tab.Columns.Add("String", typeof(string));
            tab.Columns.Add("Color", typeof(Color));
            tab.Columns.Add("DataFrame", typeof(DataFrame));

            tab.Rows.Add(null, null, null, null, null, null);
            //tab.Rows.Add(new DataRow());

            for (int i = 0; i < tab.Columns.Count - 1; i++)
            {
                var col = tab.Columns[i];
                if (col.DataType.IsValueType)
                    tab.Rows[0][i] = DBNull.Value;
                else
                    tab.Rows[0][i] = null;

            }

            object val0 = tab.Rows[0][0];
            object val1 = tab.Rows[0][1];
            object val2 = tab.Rows[0][2];
            object val3 = tab.Rows[0][3];
            object val4 = tab.Rows[0][4];
            object val5 = tab.Rows[0][5];

            bool isVal0Null = val0 == null;
            bool isVal1Null = val1 == null;
            bool isVal2Null = val2 == null;
            bool isVal3Null = val3 == null;
            bool isVal4Null = val4 == null;
            bool isVal5Null = val5 == null;

            bool isVal0DBN = val0 == DBNull.Value;
            bool isVal1DBN = val1 == DBNull.Value;
            bool isVal2DBN = val2 == DBNull.Value;
            bool isVal3DBN = val3 == DBNull.Value;
            bool isVal4DBN = val4 == DBNull.Value;
            bool isVal5DBN = val5 == DBNull.Value;
            #endregion
        }

        private static void DataFrameNullBehaviour()
        {

            #region Testing DataFrame null beheviour
            var intColWithNulls = new Int32DataFrameColumn("col", new int?[] { 2, null, 33, null });
            var intColValue0 = intColWithNulls[0];
            var intColValue1 = intColWithNulls[1];
            var intColValue2 = intColWithNulls[2];
            var intColValue3 = intColWithNulls[3];
            #endregion

            Type nullablePrimitive = typeof(double?);
            var genT = nullablePrimitive.IsGenericType;
            var valT = nullablePrimitive.IsValueType;
            var genArg = nullablePrimitive.GenericTypeArguments;
            var genParam = nullablePrimitive.ContainsGenericParameters;
            var underlying = nullablePrimitive.UnderlyingSystemType;
            var underlyingName = underlying.Name;
            var underlyingAssName = underlying.AssemblyQualifiedName;
            var nullable = typeof(Nullable<>);
            var isNullable = nullable == underlying;
            var isNullableName = nullable.Name == underlying.Name;

            var underlyingDouble = typeof(double).UnderlyingSystemType;
        }

        private static void TypeFunctionDebug()
        {
            var d_type = typeof(double);
            var d1_type = typeof(double?);
            bool samesies = d_type == d1_type;

            //Test type function casting
            var dtypeFunc = (ITypeFunction)(new Double_TypeFunction());
            var doubleCol = new DoubleDataFrameColumn("col", new double?[] { 2, 45, null, 454.43453 });
            var dresult = dtypeFunc.TryConvert_Column(doubleCol, typeof(Int32DataFrameColumn), out   DataFrameColumn dresultCol1);
            var dresult2 = dtypeFunc.TryConvert_Column(doubleCol, typeof(StringDataFrameColumn), out DataFrameColumn dresultCol2);
            var dresult3 = dtypeFunc.TryConvert_Column(doubleCol, typeof(DoubleDataFrameColumn), out DataFrameColumn dresultCol3);
            double? dobjValue = (double?)5.45;
            var nullableTypeDef = dobjValue.GetType();
            var nullableTypeDef2 = typeof(double?);
            var dresult4 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(int), out object dresultValue1);
            var dresult5 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(double), out object dresultValue2);
            var dresult6 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(string), out object dresultValue3);

            var itypeFunc = (ITypeFunction)(new Integer_TypeFunction());
            var intCol = new Int32DataFrameColumn("col", new int?[] { 2, 45, 33, 454 });
            var intIndex0 = intCol[0];
            var iresult = itypeFunc.TryConvert_Column(intCol, typeof(Int32DataFrameColumn), out   DataFrameColumn iresultCol);
            var iresult2 = itypeFunc.TryConvert_Column(intCol, typeof(StringDataFrameColumn), out DataFrameColumn iresultCol2);
            var iresult3 = itypeFunc.TryConvert_Column(intCol, typeof(DoubleDataFrameColumn), out DataFrameColumn iresultCol3);

            var stypeFunc = (ITypeFunction)(new String_TypeFunction());
            var strCol = new StringDataFrameColumn("col", new string[] { "hihi", "45", "345.354", "23", null });
            var sresult = stypeFunc.TryConvert_Column(strCol, typeof(Int32DataFrameColumn), out   DataFrameColumn sresultCol1);
            var sresult2 = stypeFunc.TryConvert_Column(strCol, typeof(StringDataFrameColumn), out DataFrameColumn sresultCol2);
            var sresult3 = stypeFunc.TryConvert_Column(strCol, typeof(DoubleDataFrameColumn), out DataFrameColumn sresultCol3);
        }

        private static void DatagridDialogTest()
        {
            var d = new DatatableEditorDialog();
            var table = new System.Data.DataTable();
            RandomDataTableGenerator.AddSomeColumns(table, 5);
            RandomDataTableGenerator.AddSomeRows(table, 5);
            d.Model.Table = table;
            d.ShowDialog();
        }
    }
}
