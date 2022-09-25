//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Data;
//using Grasshopper.Kernel.Types;
//using Rhino.DocObjects;
//using Rhino.Geometry;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;

//namespace BabyPanda.GHReflectionComponents
//{
//    /// <summary>
//    /// WORK IN PROGRESS
//    /// </summary>
//    public abstract partial class ComponentTemplate : GH_Component
//    {
//        protected abstract class TypedMethod
//        {
//            /// <summary>
//            /// Adds a parameter to the input manager based on the reflectionItem.
//            /// Returns the index number of the created parameter.
//            /// </summary>
//            public abstract int AddInputParameterToPManager(ReflectionItemBase reflectionItem, GH_InputParamManager pManager);

//            /// <summary>
//            /// Adds a parameter to the output manager based on the reflectionItem.
//            /// Returns the index number of the created parameter.
//            /// </summary>
//            public abstract int AddOutputParameterToPManager(ReflectionItemBase reflectionItem, GH_InputParamManager pManager);

//            /// <summary>
//            /// At the end of SolveInstance, set the result back to the IGH_DataAccess object
//            /// </summary>
//            public abstract bool SetDataToDataAccess(ReflectionItemBase reflectionItem, IGH_DataAccess dataAccess, object data);

//            /// <summary>
//            /// At the beginning of SolveInstance, set the result back to the IGH_DataAccess object
//            /// </summary>
//            public abstract bool GetDataFromDataAccess(ReflectionItemBase reflectionItem, IGH_DataAccess dataAccess, ref object data);

//            public abstract bool IsSuitableType(ReflectionItemBase reflectionItem);
//        }

//        protected abstract class TypedMethodBase<T, Goo> : TypedMethod where Goo : IGH_Goo
//        {
//            public abstract int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access);

//            public abstract int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, T defaultValue);

//            public abstract int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<T> defaultValues);

//            public override int AddInputParameterToPManager(ReflectionItemBase reflectionItem, GH_InputParamManager pManager)
//            {
//                string name = reflectionItem.Name;
//                if (!(reflectionItem is InputReflectionItem inputReflectionItem))
//                {
//                    throw new Exception("AddInputParameterToPManager must be supplied with an InputReflectionItem.");
//                }
//                string nickname = inputReflectionItem.InputAttribute.ShortName;
//                string description = inputReflectionItem.InputAttribute.Description;
//                var access = reflectionItem.ParamAccess;
//                var generic = reflectionItem.GenericType;
//                int resultIndex = -1; //-1 causes AddInputParameter to use no default value.

//                if (inputReflectionItem.InputAttribute.OptionalParameterType != null)
//                {
//                    //TBC
//                }

//                if (access == GH_ParamAccess.item)
//                {
//                    if (inputReflectionItem.InputAttribute.HasDefaultValue)
//                    {
//                        //Get the default value and cast it to T
//                        T defaultValue;
//                        if (typeof(T).IsAssignableFrom(reflectionItem.UnitType)) defaultValue = (T)inputReflectionItem.DefaultValue;
//                        //Supporting conversion from Goo to primitive type, is this even necessary???
//                        else if (typeof(Goo).IsAssignableFrom(reflectionItem.UnitType) &&
//                            ((IGH_Goo)inputReflectionItem.DefaultValue).CastTo<T>(out defaultValue)) { }
//                        else throw new System.Exception("Could not match TypedMethod <T:" + typeof(T).ToString() + ",Goo:" + typeof(Goo).ToString() + "> type with the ReflectionItem UnitType " + reflectionItem.UnitType.ToString());

//                        resultIndex = AddInputParameter(pManager, name, nickname, description, access, defaultValue);
//                    }
//                    if (resultIndex == -1)
//                        resultIndex = AddInputParameter(pManager, name, nickname, description, access);
//                }
//                else
//                {
//                    if (inputReflectionItem.InputAttribute.HasDefaultValue)
//                    {
//                        IEnumerable<T> defaultValueEnum = null;
//                        if (typeof(IEnumerable<T>).IsAssignableFrom(reflectionItem.OriginalType)) defaultValueEnum = (IEnumerable<T>)inputReflectionItem.DefaultValue;
//                        //Supporting conversion from Goo to primitive type, is this even necessary???
//                        else if (typeof(IEnumerable<Goo>).IsAssignableFrom(reflectionItem.OriginalType))
//                        {
//                            //TBC - extract method to cast Goo to T. Look for similar cases elsewhere.
//                            var defaultValueGoo = (IEnumerable<Goo>)inputReflectionItem.DefaultValue;
//                            var defaultValueList = new List<T>();
//                            foreach (Goo goo in defaultValueGoo)
//                            {
//                                T defaultValueItem;
//                                bool castSuccess2 = ((IGH_Goo)goo).CastTo<T>(out defaultValueItem);
//                                defaultValueList.Add(defaultValueItem);
//                                if (!castSuccess2)
//                                    throw new System.Exception("When converting Goo to T for the default value, failed to cast " + goo.GetType().ToString() + " to " + typeof(T).ToString());
//                            }
//                        }
//                        else
//                            throw new System.Exception("Could not match TypedMethod <T:" + typeof(T).ToString() + ",Goo:" + typeof(Goo).ToString() + "> type with the ReflectionItem UnitType " + reflectionItem.UnitType.ToString());

//                        if (defaultValueEnum != null)
//                            resultIndex = AddInputParameter(pManager, name, nickname, description, access, defaultValueEnum);

//                    }

//                    if (resultIndex == -1)
//                        resultIndex = AddInputParameter(pManager, name, nickname, description, access);
//                }

//                return resultIndex;
//            }

//            public override int AddOutputParameterToPManager(ReflectionItemBase reflectionItem, GH_InputParamManager pManager)
//            {
//                throw new NotImplementedException();
//            }

//            public override bool GetDataFromDataAccess(ReflectionItemBase reflectionItem, IGH_DataAccess dataAccess, ref object data)
//            {
//                var access = reflectionItem.ParamAccess;
//                var index = reflectionItem.Index;
//                bool result;
//                switch (access)
//                {
//                    case GH_ParamAccess.item:
//                        var obj = default(T);
//                        result = dataAccess.GetData<T>(index, ref obj);
//                        data = obj;
//                        break;
//                    case GH_ParamAccess.list:
//                        var listobj = new List<T>();
//                        result = dataAccess.GetDataList<T>(index, listobj);
//                        data = listobj;
//                        break;
//                    case GH_ParamAccess.tree:
//                        //Add option to process primitive datatype AND Goo.
//                        //If using Goo, extract GH_Structure should be the input, otherwise
//                        //If using primitive, use a datatree or gh_structure as appropriate!!
//                        GH_Structure<Goo> struc;
//                        result = dataAccess.GetDataTree<Goo>(index, out struc);
//                        if (!result) { }
//                        else if (reflectionItem.GenericType == GenericType.DataTree)
//                        {
//                            if (reflectionItem.UnitIsGoo)
//                                data = ComponentHelpers.ConvertToDataTree(struc);
//                            else
//                                data = ComponentHelpers.CastToDataTree<T, Goo>(struc); //NOT a goo is requested, so must cast
//                        }
//                        else if (reflectionItem.GenericType == GenericType.GHStructure)
//                        {
//                            //GHStructure UnitType must be Goo
//                            data = struc;
//                        }
//                        else
//                        {
//                            throw new Exception("Incorrect generic type classification. GH_ParamAccess.tree should be either GenericType.DataTree or GenericType.GHStructure.");
//                        }
//                        break;
//                    default:
//                        result = false;
//                        break;
//                }
//                return result;
//            }

//            public override bool SetDataToDataAccess(ReflectionItemBase reflectionItem, IGH_DataAccess dataAccess, object data)
//            {
//                var access = reflectionItem.ParamAccess;
//                var index = reflectionItem.Index;
//                bool result;

//                switch (access)
//                {
//                    case GH_ParamAccess.item:
//                        dataAccess.SetData(reflectionItem.Index, data);
//                        return false;
//                    case GH_ParamAccess.list:
//                        IEnumerable dataIenum = data as IEnumerable;
//                        if (dataIenum != null)
//                        {
//                            dataAccess.SetDataList(reflectionItem.Index, dataIenum);
//                            return true;
//                        }
//                        else
//                            throw new Exception("Data of type " + data.GetType().ToString() + "could not be cast to Ienumerable.");
//                        break;
//                    case GH_ParamAccess.tree:
//                        IGH_DataTree dataTree = data as IGH_DataTree;
//                        if (dataTree != null)
//                        {
//                            dataAccess.SetDataTree(reflectionItem.Index, dataTree);
//                            return true;
//                        }
//                        else
//                            throw new Exception("Data of type " + data.GetType().ToString() + "could not be cast to IGH_DataTree.");
//                        break;
//                    default:
//                        return false;
//                        break;
//                }


//            }

//            public override bool IsSuitableType(ReflectionItemBase reflectionItem)
//            {
//                return reflectionItem.UnitType == typeof(T) || reflectionItem.UnitType == typeof(Goo);
//            }
//        }

//        protected class TypedMethod_Angle : TypedMethodBase<double, GH_Number>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddAngleParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, double defaultValue)
//            {
//                return pManager.AddAngleParameter(name, nickname, description, access, (double)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<double> defaultValue)
//            {
//                return pManager.AddAngleParameter(name, nickname, description, access, (IEnumerable<double>)defaultValue);
//            }
//        }

//        protected class TypedMethod_Arc : TypedMethodBase<Arc, GH_Arc>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddArcParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Arc defaultValue)
//            {
//                return pManager.AddArcParameter(name, nickname, description, access, (Arc)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Arc> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_bool : TypedMethodBase<bool, GH_Boolean>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddBooleanParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, bool defaultValue)
//            {
//                return pManager.AddBooleanParameter(name, nickname, description, access, (bool)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<bool> defaultValue)
//            {
//                return pManager.AddBooleanParameter(name, nickname, description, access, (IEnumerable<bool>)defaultValue);
//            }
//        }

//        protected class TypedMethod_Box : TypedMethodBase<Box, GH_Box>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddBoxParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Box defaultValue)
//            {
//                return pManager.AddBoxParameter(name, nickname, description, access, (Box)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Box> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_Brep : TypedMethodBase<Brep, GH_Brep>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddBrepParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Brep defaultValue)
//            {
//                return -1;
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Brep> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_Circle : TypedMethodBase<Circle, GH_Circle>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddCircleParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Circle defaultValue)
//            {
//                return pManager.AddCircleParameter(name, nickname, description, access, (Circle)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Circle> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_Color : TypedMethodBase<System.Drawing.Color, GH_Colour>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddColourParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, System.Drawing.Color defaultValue)
//            {
//                return pManager.AddColourParameter(name, nickname, description, access, (System.Drawing.Color)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<System.Drawing.Color> defaultValue)
//            {
//                return pManager.AddColourParameter(name, nickname, description, access, (IEnumerable<System.Drawing.Color>)defaultValue);
//            }
//        }

//        protected class TypedMethod_GH_ComplexNumber : TypedMethodBase<GH_ComplexNumber, GH_ComplexNumber>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddComplexNumberParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GH_ComplexNumber defaultValue)
//            {
//                return pManager.AddComplexNumberParameter(name, nickname, description, access, (GH_ComplexNumber)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GH_ComplexNumber> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_CultureInfo : TypedMethodBase<CultureInfo, GH_Culture>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddCultureParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, CultureInfo defaultValue)
//            {
//                return pManager.AddCultureParameter(name, nickname, description, access, (CultureInfo)defaultValue);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<CultureInfo> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_Curve : TypedMethodBase<Curve, GH_Curve>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddCurveParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Curve defaultValue)
//            {
//                return -1;
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Curve> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_GH_Field : TypedMethodBase<GH_Field, GH_Field>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddFieldParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GH_Field defaultValue)
//            {
//                return -1;
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GH_Field> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_GeometryBase : TypedMethodBase<GeometryBase, GH_GeometricGooWrapper>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddGeometryParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GeometryBase defaultValue)
//            {
//                return -1;
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GeometryBase> defaultValue)
//            {
//                return -1;
//            }
//        }

//        protected class TypedMethod_Group : TypedMethodBase<GH_GeometryGroup, GH_GeometryGroup>
//        {
//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//            {
//                return pManager.AddGroupParameter(name, nickname, description, access);
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GH_GeometryGroup defaultValue)
//            {
//                return -1;
//            }

//            public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GH_GeometryGroup> defaultValue)
//            {
//                return -1;
//            }
//        }

//        //protected class TypedMethod_int : TypedMethodBase<int>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddIntegerParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, int defaultValue)
//        //    {
//        //        return pManager.AddIntegerParameter(name, nickname, description, access, (int)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<int> defaultValue)
//        //    {
//        //        return pManager.AddIntegerParameter(name, nickname, description, access, (IEnumerable<int>)defaultValue);
//        //    }
//        //}

//        //protected class TypedMethod_GH_Interval2D : TypedMethodBase<GH_Interval2D>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddInterval2DParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GH_Interval2D defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GH_Interval2D> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Interval : TypedMethodBase<Interval>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddIntervalParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Interval defaultValue)
//        //    {
//        //        return pManager.AddIntervalParameter(name, nickname, description, access, (Interval)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Interval> defaultValue)
//        //    {
//        //        return pManager.AddIntervalParameter(name, nickname, description, access, (IEnumerable<Interval>)defaultValue);
//        //    }
//        //}

//        //protected class TypedMethod_Line : TypedMethodBase<Line>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddLineParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Line defaultValue)
//        //    {
//        //        return pManager.AddLineParameter(name, nickname, description, access, (Line)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Line> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Matrix : TypedMethodBase<Matrix>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddMatrixParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Matrix defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Matrix> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_MeshFace : TypedMethodBase<MeshFace>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddMeshFaceParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, MeshFace defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<MeshFace> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Mesh : TypedMethodBase<Mesh>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddMeshParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Mesh defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Mesh> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_double : TypedMethodBase<double>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddNumberParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, double defaultValue)
//        //    {
//        //        return pManager.AddNumberParameter(name, nickname, description, access, (double)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<double> defaultValue)
//        //    {
//        //        return pManager.AddNumberParameter(name, nickname, description, access, (IEnumerable<double>)defaultValue);
//        //    }
//        //}

//        //protected class TypedMethod_GH_Path : TypedMethodBase<GH_Path>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddPathParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, GH_Path defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<GH_Path> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Plane : TypedMethodBase<Plane>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddPlaneParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Plane defaultValue)
//        //    {
//        //        return pManager.AddPlaneParameter(name, nickname, description, access, (Plane)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Plane> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Point3d : TypedMethodBase<Point3d>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddPointParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Point3d defaultValue)
//        //    {
//        //        return pManager.AddPointParameter(name, nickname, description, access, (Point3d)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Point3d> defaultValue)
//        //    {
//        //        return pManager.AddPointParameter(name, nickname, description, access, (IEnumerable<Point3d>)defaultValue);
//        //    }
//        //}

//        //protected class TypedMethod_Rectangle3d : TypedMethodBase<Rectangle3d>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddRectangleParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Rectangle3d defaultValue)
//        //    {
//        //        return pManager.AddRectangleParameter(name, nickname, description, access, (Rectangle3d)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Rectangle3d> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_SubD : TypedMethodBase<SubD>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddSubDParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, SubD defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<SubD> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Surface : TypedMethodBase<Surface>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddSurfaceParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Surface defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Surface> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_string : TypedMethodBase<string>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddTextParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, string defaultValue)
//        //    {
//        //        return pManager.AddTextParameter(name, nickname, description, access, defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<string> defaultValue)
//        //    {
//        //        return pManager.AddTextParameter(name, nickname, description, access, defaultValue);
//        //    }
//        //}

//        //protected class TypedMethod_DateTime : TypedMethodBase<DateTime>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddTimeParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, DateTime defaultValue)
//        //    {
//        //        return pManager.AddTimeParameter(name, nickname, description, access, (DateTime)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<DateTime> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Transform : TypedMethodBase<Transform>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddTransformParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Transform defaultValue)
//        //    {
//        //        return -1;
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Transform> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}

//        //protected class TypedMethod_Vector3d : TypedMethodBase<Vector3d>
//        //{
//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access)
//        //    {
//        //        return pManager.AddVectorParameter(name, nickname, description, access);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, Vector3d defaultValue)
//        //    {
//        //        return pManager.AddVectorParameter(name, nickname, description, access, (Vector3d)defaultValue);
//        //    }

//        //    public override int AddInputParameter(GH_InputParamManager pManager, string name, string nickname, string description, GH_ParamAccess access, IEnumerable<Vector3d> defaultValue)
//        //    {
//        //        return -1;
//        //    }
//        //}
//    }
//}