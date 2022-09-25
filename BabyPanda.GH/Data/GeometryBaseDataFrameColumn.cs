using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;

namespace BabyPanda
{
    /// <summary>
    /// The GeometryBaseDataFrameColumn supports the following types (Curve), (Brep), (Mesh), (Point)
    /// If these types are updated, remember to update the TypeFunction conversion methods and IO_GeometryBaseDataFrameColumn.
    /// </summary>
    public class GeometryBaseDataFrameColumn : DataFrameColumnByArray<GeometryBase>
    {
        public GeometryBaseDataFrameColumn(string name, long length) : base(name, length)
        {
        }

        public GeometryBaseDataFrameColumn(string name, GeometryBase[] array) : base(name, array)
        {
        }

        public GeometryBaseDataFrameColumn(string name, IEnumerable<GeometryBase> values) : base(name, values.ToArray())
        {
        }
    }

}
