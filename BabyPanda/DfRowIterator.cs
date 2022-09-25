using Microsoft.Data.Analysis;

namespace BabyPanda
{
    /// <summary>
    /// An object used to access one row of the dataframe, controlled by a method such as "ExecuteByRow".
    /// Provides access to row data on each column through column name indexer.
    /// New columns can be declared through the indexer, according to the type of the first object supplied.
    /// 
    /// </summary>
    public class DfRowIterator
    {
        public DataFrame DataFrame;

        internal int Row = 0;

        private int NoOfRows;

        public DfRowIterator(DataFrame dataFrame)
        {
            DataFrame = dataFrame;
            NoOfRows = (int)dataFrame.Rows.Count;
        }

        public DfRowIterator(DataFrame dataFrame, int noOfRows)
        {
            DataFrame = dataFrame;
            NoOfRows = noOfRows;
        }

        public object this[string columnName]
        {
            get => DataFrame[columnName][Row];
            set
            {
                var index = DataFrame.Columns.IndexOf(columnName);
                if (index == -1)
                {
                    if (value == null) return;
                    var tf = TypeMaster.GetTypeFunction(value.GetType());
                    var newColumn = tf.Create_Column(columnName, NoOfRows);
                    DataFrame.Columns.Add(newColumn);
                    index = DataFrame.Columns.IndexOf(columnName);
                }
                DataFrame.Columns[index][Row] = value;
            }
        }

    }

}
