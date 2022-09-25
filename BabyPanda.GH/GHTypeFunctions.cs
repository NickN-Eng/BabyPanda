using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using Microsoft.Data.Analysis;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BabyPanda;
using System.Linq;

namespace BabyPanda.GH
{
    public static class GHTypeFunctions
    {
        public const string TypeName_Boolean = "bool";
        public const string TypeName_Double = "double";
        public const string TypeName_Object = "System.Object";
        public const string TypeName_Integer = "int";
        public const string TypeName_String = "string";
        public const string TypeName_GeometryBase = "GeometryBase";
        public const string TypeName_Guid = "Guid";
        public const string TypeName_Colour = "System.Drawing.Color";
        public const string TypeName_Point3d = "Point3d";

        public static IGH_Param ToParameter(Type columnType)
        {
            if (columnType == typeof(BooleanDataFrameColumn)) return new Param_Boolean();
            else if (columnType == typeof(DoubleDataFrameColumn)) return new Param_Number();
            else if (columnType == typeof(Int32DataFrameColumn)) return new Param_Integer();
            else if (columnType == typeof(StringDataFrameColumn)) return new Param_String();
            else if (columnType == typeof(GeometryBaseDataFrameColumn)) return new Param_Geometry();
            else if (columnType == typeof(ColorDataFrameColumn)) return new Param_Colour();
            else if (columnType == typeof(Point3dDataFrameColumn)) return new Param_Point();
            else
                throw new System.Exception("Column type " + columnType.ToString() + " not supported.");

        }

        public static DataFrameColumn ExtractColumnFromDA(IGH_TypeHint hint, string columnName, IGH_DataAccess DA, int DA_index)
        {
            switch (hint.TypeName)
            {
                //case TypeConstants.TypeName_Object: 
                case TypeName_Boolean: return new BooleanDataFrameColumn(columnName, ExtractList<bool>(DA, DA_index, hint)); 
                case TypeName_Double: return new DoubleDataFrameColumn(columnName, ExtractList<double>(DA, DA_index, hint)); 
                case TypeName_Integer: return new Int32DataFrameColumn(columnName, ExtractList<int>(DA, DA_index, hint));    
                case TypeName_String: return new StringDataFrameColumn(columnName, ExtractList<string>(DA, DA_index, hint)); 
                case TypeName_Colour: return new ColorDataFrameColumn(columnName, ExtractList<Color?>(DA, DA_index, hint));  
                case TypeName_Point3d: return new Point3dDataFrameColumn(columnName, ExtractList<Point3d?>(DA, DA_index, hint));
                case TypeName_GeometryBase: return new GeometryBaseDataFrameColumn(columnName, ExtractList<GeometryBase>(DA, DA_index, hint));
                default:
                    throw new Exception("A valid type must be selected for the parameter at index " + DA_index.ToString());
            }
        }

        public static DataFrameColumn ExtractColumnFromDA_DuplicateItem(IGH_TypeHint hint, string columnName, IGH_DataAccess DA, int DA_index, int length)
        {
            switch (hint.TypeName)
            {
                //case TypeConstants.TypeName_Object: 
                case TypeName_Boolean: return new BooleanDataFrameColumn(columnName, ExtractItemAndDuplicateArray<bool>(DA, DA_index, length, hint));  
                case TypeName_Double:  return new DoubleDataFrameColumn(columnName, ExtractItemAndDuplicateArray<double>(DA, DA_index, length, hint)); 
                case TypeName_Integer: return new Int32DataFrameColumn(columnName, ExtractItemAndDuplicateArray<int>(DA, DA_index, length, hint));     
                case TypeName_String:  return new StringDataFrameColumn(columnName, ExtractItemAndDuplicateArray<string>(DA, DA_index, length, hint)); 
                case TypeName_GeometryBase: return new GeometryBaseDataFrameColumn(columnName, ExtractItemAndDuplicateArray<GeometryBase>(DA, DA_index, length, hint)); 
                case TypeName_Point3d: return new Point3dDataFrameColumn(columnName, ExtractItemAndDuplicateArray<Point3d?>(DA, DA_index, length, hint)); 
                case TypeName_Colour:  return new ColorDataFrameColumn(columnName, ExtractItemAndDuplicateArray<Color?>(DA, DA_index, length, hint)); 
                default:
                    throw new Exception("A valid type must be selected for the parameter at index " + DA_index.ToString());
            }
        }

        private static T[] ExtractList<T>(IGH_DataAccess DA, int index, IGH_TypeHint hint)
        {
            var goolist = new List<IGH_Goo>();
            DA.GetDataList<IGH_Goo>(index, goolist);
            return goolist.Select(item => CastWithHint<T>(item, hint)).ToArray();
        }

        private static T[] ExtractItemAndDuplicateArray<T>(IGH_DataAccess DA, int index, int reqLength, IGH_TypeHint hint)
        {
            IGH_Goo goo = null;
            DA.GetData<IGH_Goo>(index, ref goo);
            var item = CastWithHint<T>(goo, hint);
            var result = new T[reqLength];
            for (int i = 0; i < reqLength; i++)
                result[i] = item;
            return result;
        }

        private static T CastWithHint<T>(IGH_Goo goo, IGH_TypeHint hint)
        {
            if (goo == null) return default(T);
            var scriptValue = goo.ScriptVariable();

            object resultAsObj;
            if (!hint.Cast(scriptValue, out resultAsObj))
                return default(T);

            return (T)resultAsObj;
        }

        public static DataFrameColumn ExtractColumnFromDA(ITypeFunction tf, string columnName, IGH_DataAccess DA, int DA_index)
        {
            switch (tf.PrintName())
            { 
                //case TypeConstants.TypeName_Object: 
                case TypeFunctionConstants.Boolean:         return new BooleanDataFrameColumn(columnName, ExtractList<bool?>(DA, DA_index, tf));
                case TypeFunctionConstants.Double :         return new DoubleDataFrameColumn(columnName, ExtractList<double?>(DA, DA_index, tf));
                case TypeFunctionConstants.Integer:         return new Int32DataFrameColumn(columnName, ExtractList<int?>(DA, DA_index, tf));
                case TypeFunctionConstants.String :         return new StringDataFrameColumn(columnName, ExtractList<string>(DA, DA_index, tf));
                case TypeFunctionConstants.Color  :         return new ColorDataFrameColumn(columnName, ExtractList<Color?>(DA, DA_index, tf));
                case TypeFunctionConstants_GH.Point3d:      return new Point3dDataFrameColumn(columnName, ExtractList<Point3d?>(DA, DA_index, tf));
                case TypeFunctionConstants_GH.GeometryBase: return new GeometryBaseDataFrameColumn(columnName, ExtractList<GeometryBase>(DA, DA_index, tf));
                default:
                    throw new Exception("A valid type must be selected for the parameter at index " + DA_index.ToString());
            }
        }

        public static DataFrameColumn ExtractColumnFromDA_DuplicateItem(ITypeFunction tf, string columnName, IGH_DataAccess DA, int DA_index, int length)
        {
            switch (tf.PrintName())
            {
                //case TypeConstants.TypeName_Object: 
                case TypeFunctionConstants.Boolean:  return new BooleanDataFrameColumn(columnName, ExtractItemAndDuplicateArray<bool?>(DA, DA_index, length, tf));
                case TypeFunctionConstants.Double : return new DoubleDataFrameColumn(columnName, ExtractItemAndDuplicateArray<double?>(DA, DA_index, length, tf));
                case TypeFunctionConstants.Integer: return new Int32DataFrameColumn(columnName, ExtractItemAndDuplicateArray<int?>(DA, DA_index, length, tf));
                case TypeFunctionConstants.String : return new StringDataFrameColumn(columnName, ExtractItemAndDuplicateArray<string>(DA, DA_index, length, tf));
                case TypeFunctionConstants.Color  : return new ColorDataFrameColumn(columnName, ExtractItemAndDuplicateArray<Color?>(DA, DA_index, length, tf));
                case TypeFunctionConstants_GH.Point3d: return new Point3dDataFrameColumn(columnName, ExtractItemAndDuplicateArray<Point3d?>(DA, DA_index, length, tf));
                case TypeFunctionConstants_GH.GeometryBase: return new GeometryBaseDataFrameColumn(columnName, ExtractItemAndDuplicateArray<GeometryBase>(DA, DA_index, length, tf)); 
                default:
                    throw new Exception("A valid type must be selected for the parameter at index " + DA_index.ToString());
            }
        }

        private static T[] ExtractList<T>(IGH_DataAccess DA, int index, ITypeFunction tf)
        {
            var goolist = new List<IGH_Goo>();
            DA.GetDataList<IGH_Goo>(index, goolist);
            return goolist.Select(item => CastWithHint<T>(item, tf)).ToArray();
        }

        private static T[] ExtractItemAndDuplicateArray<T>(IGH_DataAccess DA, int index, int reqLength, ITypeFunction tf)
        {
            IGH_Goo goo = null;
            DA.GetData<IGH_Goo>(index, ref goo);
            var item = CastWithHint<T>(goo, tf);
            var result = new T[reqLength];
            for (int i = 0; i < reqLength; i++)
                result[i] = item;
            return result;
        }

        private static T CastWithHint<T>(IGH_Goo goo, ITypeFunction tf) 
        { 
            if (goo == null) return default(T);
            var scriptValue = goo.ScriptVariable();

            object resultAsObj;
            if (!tf.TryConvert_FromObject(scriptValue, out resultAsObj))
                return default(T);

            return (T)resultAsObj;
        }
    }



    public abstract class TypeData
    {
        public abstract string TypeName { get; }

        public abstract Type Type { get; }

        public abstract IGH_Param GetNewParameter();
    }
}
