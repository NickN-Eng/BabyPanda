using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace BabyPanda
{
    public static class TypeMaster
    {
        /// <summary>
        /// Static constructor loads all TypeFunctions from this assembly. 
        /// Other assemblies which add type functions must also call RegisterTypesFromAssembly in this way.
        /// </summary>
        static TypeMaster()
        {
            RegisterTypesFromAssembly(typeof(TypeMaster).Assembly);
        }

        /// <summary>
        /// A dictionary of the DataType (i.e. bool, double, int) paired with the ITypeFunction which works with that type.
        /// </summary>
        private static Dictionary<Type, ITypeFunction> AllTypeFunctions { get; set; } = new Dictionary<Type, ITypeFunction>();
        public static IReadOnlyDictionary<Type, ITypeFunction> TypeFuctions => AllTypeFunctions;

        /// <summary>
        /// A dictionary of the data type STRING (i.e. "bool", "double", "int") paired with the ITypeFunction which works with that type.
        /// </summary>
        private static Dictionary<string, ITypeFunction> AllTypeFunctions_ByName { get; set; } = new Dictionary<string, ITypeFunction>();
        public static IReadOnlyDictionary<string, ITypeFunction> TypeFuctions_ByName => AllTypeFunctions_ByName;

        public static ITypeFunction GetTypeFunction(Type type)
        {
            if (AllTypeFunctions.TryGetValue(type, out ITypeFunction value)) return value;
            return null;
        }

        public static ITypeFunction GetTypeFunction(string type)
        {
            if (AllTypeFunctions_ByName.TryGetValue(type, out ITypeFunction value)) return value;
            return null;
        }

        public static DataFrameColumn CreateColumn(string name, object array) //TBC Check if this is actually used
        {
            foreach (ITypeFunction tf in AllTypeFunctions.Values)
            {
                if (tf.TryCreate_Column_GenericArray(name, array, out DataFrameColumn col))
                    return col;
            }
            return null;
        }

        public static IO_DataFrameColumn ConvertToIOColumn(DataFrameColumn col)
        {
            var tf = AllTypeFunctions[col.DataType];
            if (tf.TryCreate_IOColumn(col, out IO_DataFrameColumn ioCol))
                return ioCol;

            return null;
        }



        public static void RegisterType(Type type, ITypeFunction typeFunction)
        {
            AllTypeFunctions[type] = typeFunction;
            string name = typeFunction.PrintName();
            
            if (AllTypeFunctions_ByName.TryGetValue(name, out ITypeFunction tf) && typeFunction != tf)
                throw new Exception("It appears that 2 different type functions have the same PrintName. Check this and make PrintNames unique.");
            
            AllTypeFunctions_ByName[typeFunction.PrintName()] = typeFunction;
        }



        public static void RegisterTypesFromAssembly(Assembly asm)
        {
            Type[] allTypes = asm.GetTypes();
            foreach (Type type in allTypes)
            {
                if (typeof(ITypeFunction).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var tf = (ITypeFunction)Activator.CreateInstance(type);
                    var typ = tf.GetDataType();
                    RegisterType(typ, tf);

                    var typNonNullable = tf.GetDataType_NonNullable();
                    if(typ != typNonNullable)
                        RegisterType(typNonNullable, tf);
                }
            }
        }

    }

}
