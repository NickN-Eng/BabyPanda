using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BabyPanda
{
    public class ColorDataFrameColumn : DataFrameColumnByArray<Color?>
    {
        public ColorDataFrameColumn(string name, long length) : base(name, length)
        {
        }

        public ColorDataFrameColumn(string name, Color?[] array) : base(name, array)
        {
        }

        public ColorDataFrameColumn(string name, IEnumerable<Color?> values) : base(name, values.ToArray())
        {
        }
    }
}
