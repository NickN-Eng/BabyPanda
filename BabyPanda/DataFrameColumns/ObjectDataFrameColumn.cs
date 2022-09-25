using System;
using System.Collections.Generic;
using System.Text;

namespace BabyPanda
{
    public class ObjectDataFrameColumn : DataFrameColumnByArray<object>
    {
        public ObjectDataFrameColumn(string name, long length) : base(name, length)
        {
        }

        public ObjectDataFrameColumn(string name, object[] array) : base(name, array)
        {
        }
    }
}