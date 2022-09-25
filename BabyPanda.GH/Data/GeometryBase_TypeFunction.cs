using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.Data.Analysis;
using Grasshopper.Getters;
using Grasshopper.Kernel;

namespace BabyPanda.GH
{
    /// <summary>
    /// The GeometryBaseDataFrameColumn supports the following types (Curve), (Brep), (Mesh), (Point)
    /// 
    /// </summary>
    public class GeometryBase_TypeFunction : TypeFunction<GeometryBaseDataFrameColumn, GeometryBase>
    {
        public override string PrintName() => TypeFunctionConstants_GH.GeometryBase;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(string), typeof(GeometryBase) };

        public override bool TryConvert_Column_Implementation(GeometryBaseDataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(GeometryBaseDataFrameColumn))
            {
                convDFCol = new GeometryBaseDataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(Point3dDataFrameColumn))
            {
                convDFCol = new Point3dDataFrameColumn(col.Name, col.Select(geom => ConvertIfPoint(geom)));
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

        private static Point3d? ConvertIfPoint(GeometryBase geom) 
        {
            if (geom is Rhino.Geometry.Point pt)
                return pt.Location;
            else
                return null;
        }

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<GeometryBase> values)
        {
            return new GeometryBaseDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_GeometryBaseDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => false;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            throw new NotImplementedException();
            //TBC parse from Guid, Point coordinates etc...

            //var doc = Rhino.RhinoDoc.ActiveDoc;
            //doc.Objects.FindId()
        }

        public override bool IsCollectableFromModalInput() => true;

        public override bool TryCollectFromModalInput(out object result)
        {
            var geom = GH_GeometryGetter.GetGeometry();

            if (geom == null)
            {
                result = null;
                return false;
            }

            result = geom;
            return true;
        }



        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(GeometryBase))
            {
                convObj = default;
                return false;
            }

            var value = (GeometryBase)obj;

            if (convType == typeof(GeometryBase))
            {
                convObj = value;
                return true;
            }
            if (convType == typeof(Point3d))
            {
                if(value is Rhino.Geometry.Point pt)
                {
                    convObj = pt.Location;
                    return true;
                }
                convObj = default;
                return false;
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

            //The GeometryBaseDataFrameColumn supports the following types (Curve), (Brep), (Mesh), (Point)

            if (objOfAnotherTyp is GeometryBase geom)
            {
                objOfThisTyp = geom;
                return true;
            }

            Curve crv = default;
            if (GH_Convert.ToCurve(objOfAnotherTyp, ref crv, GH_Conversion.Both))
            {
                objOfThisTyp = crv;
                return true;
            }

            Point3d pt = default;
            if(GH_Convert.ToPoint3d(objOfAnotherTyp, ref pt, GH_Conversion.Both))
            {
                objOfThisTyp = pt;
                return true;
            }

            Brep brep = default;
            if (GH_Convert.ToBrep(objOfAnotherTyp, ref brep, GH_Conversion.Both))
            {
                objOfThisTyp = brep;
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
            return new GeometryBaseDataFrameColumn(name, array.Select(x => (GeometryBase)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new GeometryBaseDataFrameColumn(name, length);
        }
    }

}
