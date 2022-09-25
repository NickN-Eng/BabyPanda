using Microsoft.Data.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Tests
{
    [TestClass]
    public class DataTableHelpers_Tests
    {
        [TestMethod]
        public void TryCovertColumnType_ChangeToInt()
        {
            var table = new DataTable();
            table.Columns.Add("Col1", typeof(double));
            table.Columns.Add("Col2", typeof(double));
            table.Columns.Add("Col3", typeof(double));
            table.Rows.Add(34.4, 34.4, 34.4);
            table.Rows.Add(34.5, DBNull.Value, 34.5);
            table.Rows.Add(11.6, 11.6, 11.6);

            var result = DataTableHelpers.TryCovertColumnType(table, "Col2", typeof(int));

            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), table.Columns[1].DataType);

            var colArray = DataTableHelpers.GetColumnAsArray(table, "Col2");
            Assert.AreEqual(34, colArray[0]);
            Assert.AreEqual(DBNull.Value, colArray[1]);
            Assert.AreEqual(12, colArray[2]);
        }

        [TestMethod]
        public void TryCovertColumnType_ChangeStringToInt()
        {
            var table = new DataTable();
            table.Columns.Add("Col1", typeof(string));
            table.Columns.Add("Col2", typeof(string));
            table.Columns.Add("Col3", typeof(string));
            table.Rows.Add("34.4", "34.4", "df");
            table.Rows.Add("34.5", DBNull.Value, "34.5");
            table.Rows.Add("11.6", "11.6", "23");
            table.Rows.Add("11.6", "sdff", "23");

            var result = DataTableHelpers.TryCovertColumnType(table, "Col2", typeof(int));

            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), table.Columns[1].DataType);

            var colArray = DataTableHelpers.GetColumnAsArray(table, "Col2");
            Assert.AreEqual(34, colArray[0]);
            Assert.AreEqual(DBNull.Value, colArray[1]);
            Assert.AreEqual(12, colArray[2]);
        }

        [TestMethod]
        public void Convert_ToDataFrame()
        {
            var table = new DataTable();
            table.Columns.Add("Col1", typeof(double));
            table.Columns.Add("Col2", typeof(string));
            table.Columns.Add("Col3", typeof(bool));
            table.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);
            table.Rows.Add(34.5564, "jkj", true);
            table.Rows.Add(344.5, "11.6", false);
            table.Rows.Add(45.3, "sdff", true);

            var result = DataTableHelpers.Convert(table);

            Assert.IsNotNull(result);

            var col1 = result.Columns[0];
            Assert.AreEqual("Col1", col1.Name);
            Assert.AreEqual(typeof(DoubleDataFrameColumn), col1.GetType());
            Assert.AreEqual(null, col1[0]);
            Assert.AreEqual(34.5564, col1[1]);

            var col2 = result.Columns[1];
            Assert.AreEqual("Col2", col2.Name);
            Assert.AreEqual(typeof(StringDataFrameColumn), col2.GetType());
            Assert.AreEqual(null, col2[0]);
            Assert.AreEqual("jkj", col2[1]);

            var col3 = result.Columns[2];
            Assert.AreEqual("Col3", col3.Name);
            Assert.AreEqual(typeof(BooleanDataFrameColumn), col3.GetType());
            Assert.AreEqual(null, col3[0]);
            Assert.AreEqual(true, col3[1]);

        }

        [TestMethod]
        public void Convert_ToDataTable()
        {
            var table = new DataFrame();
            table.Columns.Add(new DoubleDataFrameColumn("Col1", new double?[] { null, 34.5564, 344.5, 45.3 }));
            table.Columns.Add(new StringDataFrameColumn("Col2", new string[] { null, "jkj", "11.6", "sdff" }));
            table.Columns.Add(new BooleanDataFrameColumn("Col3", new bool?[] { null, true, false, true }));

            var result = DataTableHelpers.Convert(table);

            Assert.IsNotNull(result);

            var col1 = result.Columns[0];
            Assert.AreEqual("Col1", col1.ColumnName);
            Assert.AreEqual(typeof(double), col1.DataType);
            Assert.AreEqual(DBNull.Value, result.Rows[0][0]);
            Assert.AreEqual(34.5564, result.Rows[1][0]);

            var col2 = result.Columns[1];
            Assert.AreEqual("Col2", col2.ColumnName);
            Assert.AreEqual(typeof(string), col2.DataType);
            Assert.AreEqual(DBNull.Value, result.Rows[0][0]);
            Assert.AreEqual("jkj", result.Rows[1][1]);

            var col3 = result.Columns[2];
            Assert.AreEqual("Col3", col3.ColumnName);
            Assert.AreEqual(typeof(bool), col3.DataType);
            Assert.AreEqual(DBNull.Value, result.Rows[0][2]);
            Assert.AreEqual(true, result.Rows[1][2]);

        }
    }
}
