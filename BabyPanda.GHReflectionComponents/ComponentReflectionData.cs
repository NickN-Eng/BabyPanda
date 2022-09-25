using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

namespace BabyPanda.GHReflectionComponents
{
    public class ComponentReflectionData
    {
        /// <summary>
        /// Dictionary for the SetData method which is used within Solve Instance to input data from parameters into the properties.
        /// </summary>
        public static Dictionary<GH_ParamAccess, MethodInfo> SetDataMethods;

        /// <summary>
        /// Dictionary for the GetData method which is used within Solve Instance to send data from the properties into the parameters.
        /// </summary>
        public static Dictionary<GH_ParamAccess, MethodInfo> GetDataMethods;

        //public static Dictionary<Type, MethodInfo> AddInputParameters;

        static ComponentReflectionData()
        {
            /*
            bool SetData(int paramIndex, object data); //THIS ONE
            bool SetData(int paramIndex, object data, int itemIndexOverride);
            bool SetData(string paramName, object data);
            bool SetDataList(int paramIndex, IEnumerable data); //THIS ONE
            bool SetDataList(int paramIndex, IEnumerable data, int listIndexOverride);
            bool SetDataList(string paramName, IEnumerable data);
            bool SetDataTree(int paramIndex, IGH_DataTree tree); //THIS ONE
            bool SetDataTree(int paramIndex, IGH_Structure tree);

            bool GetData<T>(int index, ref T destination);
            bool GetData<T>(string name, ref T destination);
            bool GetDataList<T>(int index, List<T> list);
            bool GetDataList<T>(string name, List<T> list);
            bool GetDataTree<T>(int index, out GH_Structure<T> tree) where T : IGH_Goo;
            bool GetDataTree<T>(string name, out GH_Structure<T> tree) where T : IGH_Goo;
            */
            //mInfo.GetParameters()[0].ParameterType
            var setDataMInfos = typeof(IGH_DataAccess).GetMethod("SetData", new Type[] { typeof(int), typeof(object) });
            var setListMInfos = typeof(IGH_DataAccess).GetMethod("SetDataList", new Type[] { typeof(int), typeof(IEnumerable) });
            var setTreeMInfos = typeof(IGH_DataAccess).GetMethod("SetDataTree", new Type[] { typeof(int), typeof(IGH_DataTree) });

            var getDataMInfos = typeof(IGH_DataAccess).GetMethods().Where(mi => mi.Name == "GetData" && mi.GetParameters()[0].ParameterType == typeof(int)).FirstOrDefault();
            var getListMInfos = typeof(IGH_DataAccess).GetMethods().Where(mi => mi.Name == "GetDataList" && mi.GetParameters()[0].ParameterType == typeof(int)).FirstOrDefault();
            var getTreeMInfos = typeof(IGH_DataAccess).GetMethods().Where(mi => mi.Name == "GetDataTree" && mi.GetParameters()[0].ParameterType == typeof(int)).FirstOrDefault();

            SetDataMethods = new Dictionary<GH_ParamAccess, MethodInfo>()
            {
                { GH_ParamAccess.item, setDataMInfos},
                { GH_ParamAccess.list, setListMInfos},
                { GH_ParamAccess.tree, setTreeMInfos }
            };

            GetDataMethods = new Dictionary<GH_ParamAccess, MethodInfo>()
            {
                { GH_ParamAccess.item, getDataMInfos},
                { GH_ParamAccess.list, getListMInfos},
                { GH_ParamAccess.tree, getTreeMInfos }
            };

        }

        public ComponentReflectionData(Type typ)
        {
            ComponentTemplateType = typ;
            ParseInputOutputs(typ);


        }

        public Type ComponentTemplateType { get; set; }

        public List<InputReflectionItem> Inputs { get; set; }
        public List<OutputReflectionItem> Outputs { get; set; }



        public void ParseInputOutputs(Type typ)
        {
            Inputs = new List<InputReflectionItem>();
            Outputs = new List<OutputReflectionItem>();
            object obj = Activator.CreateInstance(typ);
            foreach (var prop in typ.GetProperties())
            {
                var inputAttr = (InputAttribute)Attribute.GetCustomAttribute(prop, typeof(InputAttribute));
                if (inputAttr != null)
                {
                    var input = new InputReflectionItem();
                    input.InputAttribute = inputAttr;
                    input.Name = prop.Name;
                    input.Property = prop;
                    Inputs.Add(input);
                    input.DefaultValue = prop.GetValue(obj); //Does the input need to be cast to Array???
                    input.OriginalType = prop.GetType();
                    if (!prop.PropertyType.IsGenericType)
                    {
                        input.ParamAccess = GH_ParamAccess.item;
                        input.UnitType = prop.GetType();
                        input.GenericType = GenericType.UnitType;
                    }
                    else
                    {
                        var genericType = prop.PropertyType.GetGenericTypeDefinition();
                        if (genericType == typeof(List<>))
                        {
                            input.ParamAccess = GH_ParamAccess.list;
                            input.UnitType = prop.PropertyType.GenericTypeArguments[0];
                            input.GenericType = GenericType.List;
                        }
                        else if (genericType == typeof(DataTree<>))
                        {
                            input.ParamAccess = GH_ParamAccess.tree;
                            input.GenericType = GenericType.DataTree;
                            input.UnitType = prop.PropertyType.GenericTypeArguments[0];
                        }
                        else if (genericType == typeof(GH_Structure<>))
                        {
                            input.ParamAccess = GH_ParamAccess.tree;
                            input.GenericType = GenericType.GHStructure;
                            input.UnitType = prop.PropertyType.GenericTypeArguments[0];
                        }
                        else
                        {
                            throw new Exception("Type " + input.OriginalType.ToString() + " is not supported");
                        }
                    }

                }

                var outputAttr = (OutputAttribute)Attribute.GetCustomAttribute(prop, typeof(OutputAttribute));
                if (outputAttr != null)
                {
                    var output = new OutputReflectionItem();
                    output.OutputAttribute = outputAttr;
                    output.Name = prop.Name;
                    output.Property = prop;
                    Outputs.Add(output);
                    if (!prop.PropertyType.IsGenericType)
                    {
                        output.ParamAccess = GH_ParamAccess.item;
                        output.UnitType = prop.GetType();
                        //If not generic 
                    }
                    else
                    {
                        var genericType = prop.PropertyType.GetGenericTypeDefinition();
                        if (genericType == typeof(List<>))
                        {
                            output.ParamAccess = GH_ParamAccess.list;
                            output.UnitType = prop.PropertyType.GenericTypeArguments[0];
                        }
                        else if (genericType == typeof(DataTree<>))
                        {
                            output.ParamAccess = GH_ParamAccess.tree;
                            output.UnitType = prop.PropertyType.GenericTypeArguments[0];
                        }
                        else
                        {
                            throw new Exception("Type " + output.OriginalType.ToString() + " is not supported");
                        }
                    }

                }
            }

            //Give each input an index no
            int i = 0;
            foreach (var input in Inputs)
            {
                input.Index = i;
                i++;
            }

            //try
            //{
            //    // TODO: equivalent for GetDataList
            //    Type equivalentType = GetEquivalentType(type);
            //    MemberInfo[] mInfos;
            //    var pAccess = ParamAccess(inputAtt);
            //    if (pAccess == GH_ParamAccess.item)
            //        mInfos = typeof(IGH_DataAccess).GetMember("GetData");
            //    else
            //    {
            //        var listType = typeof(List<>);
            //        var constructedListType = listType.MakeGenericType(equivalentType);
            //        result = Activator.CreateInstance(constructedListType);
            //        mInfos = typeof(IGH_DataAccess).GetMember("GetDataList");
            //    }
            //    //TODO: Trees?
            //    MethodInfo getDataMethod = null;
            //    foreach (MethodInfo mInfo in mInfos)
            //    {
            //        Type firstPT = mInfo.GetParameters()[0].ParameterType;
            //        if (firstPT == typeof(string)) getDataMethod = mInfo;
            //    }
            //    if (getDataMethod != null)
            //    {
            //        object[] args = new object[] { name, result };
            //        MethodInfo getDataGeneric = getDataMethod.MakeGenericMethod(new Type[] { equivalentType });
            //        bool success = (bool)getDataGeneric.Invoke(DA, args);
            //        if (success)
            //        {
            //            result = args[1];
            //            if (result.GetType() != type)
            //            {
            //                result = Convert(result, type);
            //            }
            //        }
            //        else result = null;
            //    }
            //}
        }

    }

}
