using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.Data.Analysis;
using Grasshopper.Kernel;

namespace BabyPanda.GH
{
    public class Point3d_TypeFunction : TypeFunction<Point3dDataFrameColumn, Point3d?>
    {
        public override string PrintName() => TypeFunctionConstants_GH.Point3d;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(string), typeof(Point3d), typeof(Point3d?) };

        public override bool TryConvert_Column_Implementation(Point3dDataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(Point3dDataFrameColumn))
            {
                convDFCol = new Point3dDataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(StringDataFrameColumn))
            {
                convDFCol = new StringDataFrameColumn(col.Name, col.Select(c => ToDisplayString(c)));
                return true;
            }
            convDFCol = null;
            return false;
        }

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<Point3d?> values)
        {
            return new Point3dDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_Point3dDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => false;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            throw new NotImplementedException();
            //TBC parse from Guid, Point coordinates etc...

            //var doc = Rhino.RhinoDoc.ActiveDoc;
            //doc.Objects.FindId()
        }

        public override bool IsCollectableFromModalInput() => false;

        public override bool TryCollectFromModalInput(out object result)
        {
            throw new NotImplementedException();
            //var geom = GH_GeometryGetter.GetGeometry();

            //if (geom == null)
            //{
            //    result = null;
            //    return false;
            //}

            //result = geom;
            //return true;
        }



        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(Point3d))
            {
                convObj = default;
                return false;
            }

            var value = (Point3d)obj;

            if (convType == typeof(Point3d))
            {
                convObj = value;
                return true;
            }
            if (convType == typeof(GeometryBase))
            {
                convObj = new Rhino.Geometry.Point(value);
                return true;
            }
            if (convType == typeof(string))
            {
                convObj = ToDisplayString(value);
                return true;
            }
            convObj = default;
            return false;
        }

        public override bool TryConvert_FromObject(object objOfAnotherTyp, out object objOfThisTyp)
        {
            if (objOfAnotherTyp == null)
            {
                objOfThisTyp = null;
                return false;
            }

            Point3d pt = default;
            if (GH_Convert.ToPoint3d(objOfAnotherTyp, ref pt, GH_Conversion.Both))
            {
                objOfThisTyp = pt;
                return true;
            }

            if (objOfAnotherTyp is string s)
            {
                objOfThisTyp = TypeConversionHelpers.ParseStringToBool(s);
                return true;
            }

            objOfThisTyp = null;
            return false;
        }

        public override bool IsConvertibleToColor() => false;

        public override bool TryGetColourRepresentation(object objDT, out Color color)
        {
            throw new NotImplementedException();
        }

        public override DataFrameColumn Create_Column(string name, IList<object> array)
        {
            return new Point3dDataFrameColumn(name, array.Select(x => (Point3d?)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new Point3dDataFrameColumn(name, length);
        }

        public override string ToDisplayString(object item)
        {
            if (item == null) return "";
            if (item is Point3d pt)
            {
                return $"Point [{pt.X},{pt.Y},{pt.Z}]";
            }
            else return "";
        }
    }

}
