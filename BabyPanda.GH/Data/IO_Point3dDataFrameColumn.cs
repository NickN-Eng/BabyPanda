using System;
using System.Linq;
using Microsoft.Data.Analysis;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
namespace BabyPanda
{
    //[Serializable]
    //public class IO_Point3dDataFrameColumn : IO_DataFrameColumn<Point3d?>
    //{
    //    public override Point3d[] DFColumnToIOArray(DataFrameColumn dataFrameColumn)
    //    {
    //        return dataFrameColumn.Cast<GeometryBase>().Select(g => BinaryRhinoObject.FromGeometry(g)).ToArray();
    //    }

    //    public override bool IsApplicableFor(DataFrameColumn dataFrameColumn) => dataFrameColumn is GeometryBaseDataFrameColumn;

    //    public override DataFrameColumn ToDataFrameColumn()
    //    {
    //        return new GeometryBaseDataFrameColumn(Name, Values.Select(bro => bro.ToGeometry()).ToArray());
    //    }
    //}

    [Serializable]
    public class IO_Point3dDataFrameColumn : IO_DataFrameColumn<Point3d?>
    {
        public override DataFrameColumn ToDataFrameColumn()
        {
            return new Point3dDataFrameColumn(Name, Values.ToArray());
        }
    }

    //[Serializable]
    //public class BinaryRhinoObject
    //{
    //    public Type Type;
    //    public byte[] ByteArray;

    //    public BinaryRhinoObject()
    //    {

    //    }

    //    public static BinaryRhinoObject FromGeometry(GeometryBase geometryBase)
    //    {
    //        var bro = new BinaryRhinoObject();
    //        bro.Type = geometryBase.GetType();
    //        bro.ByteArray = GH_Convert.CommonObjectToByteArray(geometryBase);
    //        return bro;
    //    }

    //    public GeometryBase ToGeometry()
    //    {
    //        //This probably doesnt work, espcially converting to curve???
    //        //Is there a point to this? Or can I just convert to GeometryBase?? i.e. GH_Convert.ByteArrayToCommonObject<GeometryBase>(ByteArray);
    //        if (typeof(Curve).IsAssignableFrom(Type)) return GH_Convert.ByteArrayToCommonObject<Curve>(ByteArray);
    //        if (typeof(Brep).IsAssignableFrom(Type)) return GH_Convert.ByteArrayToCommonObject<Brep>(ByteArray);
    //        if (typeof(Mesh).IsAssignableFrom(Type)) return GH_Convert.ByteArrayToCommonObject<Mesh>(ByteArray);
    //        if (typeof(Point).IsAssignableFrom(Type)) return GH_Convert.ByteArrayToCommonObject<Point>(ByteArray);
    //        if (typeof(SubD).IsAssignableFrom(Type)) return GH_Convert.ByteArrayToCommonObject<SubD>(ByteArray);
    //        throw new Exception("Type " + Type.ToString() + " is not supported for deserialisation.");
    //    }
    //}
}
