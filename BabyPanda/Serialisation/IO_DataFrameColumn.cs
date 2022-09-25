using System;
using System.Linq;
using Microsoft.Data.Analysis;

namespace BabyPanda
{
    [Serializable]
    public abstract class IO_DataFrameColumn
    {
        /// <summary>
        /// Returns true if it is possible to serialise this column.
        /// </summary>
        public abstract bool IsApplicableFor(DataFrameColumn dataFrameColumn);

        public abstract DataFrameColumn ToDataFrameColumn();

        public abstract IO_DataFrameColumn ToIOColumn(DataFrameColumn dataFrameColumn);

        public static IO_DataFrameColumn FromDataFrameColumn(DataFrameColumn dataFrameColumn)
        {
            return TypeMaster.ConvertToIOColumn(dataFrameColumn);
        }
    }


    /// <summary>
    /// A column which contains a serialisable type
    /// </summary>
    /// <typeparam name="TSer">A serialisable type</typeparam>
    [Serializable]
    public abstract class IO_DataFrameColumn<TSer> : IO_DataFrameColumn
    {
        public string Name;

        public TSer[] Values;

        public IO_DataFrameColumn() { }

        public override IO_DataFrameColumn ToIOColumn(DataFrameColumn dataFrameColumn)
        {
            var clone = (IO_DataFrameColumn<TSer>)this.MemberwiseClone();
            clone.Name = dataFrameColumn.Name;
            clone.Values = DFColumnToIOArray(dataFrameColumn);
            return clone;
        }

        public virtual TSer[] DFColumnToIOArray(DataFrameColumn dataFrameColumn)
        {
            return dataFrameColumn.Cast<TSer>().ToArray();
        }

        public override bool IsApplicableFor(DataFrameColumn dataFrameColumn)
        {
            return typeof(TSer) == dataFrameColumn.DataType;
        }
    }

    [Serializable]
    public class IO_IntDataFrameColumn : IO_DataFrameColumn<int?>
    {
        public override DataFrameColumn ToDataFrameColumn() => new Int32DataFrameColumn(Name, Values.ToArray());
    }

    [Serializable]
    public class IO_DoubleDataFrameColumn : IO_DataFrameColumn<double?>
    {
        public override DataFrameColumn ToDataFrameColumn() => new DoubleDataFrameColumn(Name, Values.ToArray());
    }

    [Serializable]
    public class IO_StringDataFrameColumn : IO_DataFrameColumn<string>
    {
        public override DataFrameColumn ToDataFrameColumn() => new StringDataFrameColumn(Name, Values.ToArray());
    }

    [Serializable]
    public class IO_BoolDataFrameColumn : IO_DataFrameColumn<bool?>
    {
        public override DataFrameColumn ToDataFrameColumn() => new BooleanDataFrameColumn(Name, Values.ToArray());
    }

}
