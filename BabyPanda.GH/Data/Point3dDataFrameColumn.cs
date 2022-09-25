using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Getters;

namespace BabyPanda
{
    public class Point3dDataFrameColumn : DataFrameColumnByArray<Point3d?>
    {
        public Point3dDataFrameColumn(string name, long length) : base(name, length)
        {
        }

        public Point3dDataFrameColumn(string name, Point3d?[] array) : base(name, array)
        {
        }

        public Point3dDataFrameColumn(string name, IEnumerable<Point3d?> values) : base(name, values.ToArray())
        {
        }
    }

}
