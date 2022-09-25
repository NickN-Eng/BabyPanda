using System;
using System.Collections.Generic;
using Microsoft.Data.Analysis;

namespace BabyPanda
{
    [Serializable]
    public class IO_DataFrame
    {
        public List<IO_DataFrameColumn> Columns;

        public IO_DataFrame(DataFrame dataFrame)
        {
            Columns = new List<IO_DataFrameColumn>();
            foreach (var col in dataFrame.Columns)
            {
                Columns.Add(IO_DataFrameColumn.FromDataFrameColumn(col));
            }
        }

        public DataFrame ToDataFrame()
        {
            var cols = new List<DataFrameColumn>();
            foreach (var iocol in Columns)
            {
                cols.Add(iocol.ToDataFrameColumn());
            }
            return new DataFrame(cols);
        }

        
    }

}
