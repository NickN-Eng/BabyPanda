using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda
{

    public interface ITypeFunction
    {
        string PrintName();

        Type GetDataType(); //Get the data type this function operates on. For structs, this should be the nullable type (i.e. appended with ?)
        Type GetDataType_NonNullable(); //If this type is a nullable type (e.g. int?, get the primitive type instead i.e. int)
        bool IsDataTypeMatch(Type type);
        bool IsColTypeMatch(DataFrameColumn col);

        //How to convert from one column type to another (data frame column conversion etc...)
        //bool IsValidCast<Udata>();
        Type[] PermittedConversionTypes();
        bool TryConvert_Column(DataFrameColumn col, Type convColType, out DataFrameColumn convDFCol);
        bool TryConvert_ToObject(object obj, Type convType, out object convObj);
        bool TryConvert_FromObject(object objOfAnotherTyp, out object objOfThisTyp);

        //Create columns, return false in the input types are not correct
        bool TryCreate_Column_GenericArray(string name, object genericArray, out DataFrameColumn column); //Not used???
        bool TryCreate_IOColumn(DataFrameColumn column, out IO_DataFrameColumn io_column);
        DataFrameColumn Create_Column(string name, IList<object> array);
        DataFrameColumn Create_Column(string name, int length);

        //Try to convert single values (used for individual input)
        bool IsConvertibleFromString();
        bool TryConvertFromString(string str, out object result);
        bool IsCollectableFromModalInput();
        bool TryCollectFromModalInput(out object result);
        bool IsConvertibleToColor();
        bool TryGetColourRepresentation(object objDT, out System.Drawing.Color color);

        string ToDisplayString(object item); //String representation should be convertable back and forth between ToDisplayString & TryConvertFromString

    }

    public abstract class TypeFunction<Col,DT> : ITypeFunction where Col : DataFrameColumn
    {
        public Type GetDataType() => typeof(DT);
        
        public bool IsDataTypeMatch(Type type) => type == typeof(DT);
        public bool IsColTypeMatch(DataFrameColumn col) => col.DataType == typeof(DT);

        public abstract Type[] PermittedConversionTypes();

        public bool TryCreate_Column_GenericArray(string name, object array, out DataFrameColumn column)
        {
            //var poop = array.GetType();
            //var poopy = typeof(IEnumerable<DT>);
            //var crap = (IEnumerable<DT>)array;
            if (!typeof(IEnumerable<DT>).IsAssignableFrom(array.GetType()))
            //if (!array.GetType().IsAssignableFrom(typeof(IEnumerable<DT>)))
            {
                column = null;
                return false;
            }

            column = CreateColumn_Implementation(name, (IEnumerable<DT>)array);
            return true;
        }

        //protected abstract DataFrameColumn CreateColumnImplementation<T>(string name, T[] values);
        protected abstract DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<DT> values);
        protected abstract IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column);



        public bool TryCreate_IOColumn(DataFrameColumn column, out IO_DataFrameColumn io_column)
        {
            if (column.DataType != GetDataType() && column.DataType != GetDataType_NonNullable())
            {
                io_column = null;
                return false;
            }

            io_column = CreateIOColumn_Implementation(column);
            return true;
        }

        protected bool IsPermittedType(Type type, params Type[] permittedTypes)
        {
            return permittedTypes.Any(t => t == type);
        }



        public bool TryConvert_Column(DataFrameColumn col, Type convColType, out DataFrameColumn resultcol)
        {
            if (col != null || col.GetType() != typeof(Col))
            {
                //Incorrect type requested from this TypeFunction. Throw exception???
                resultcol = null;
                return false;
            }
            var result = TryConvert_Column_Implementation((Col)col, convColType, out DataFrameColumn resultColTemp);
            resultcol = resultColTemp;
            return result;
        }
        public abstract bool TryConvert_Column_Implementation(Col col, Type convColType, out DataFrameColumn resultCol);

        public abstract string PrintName();
        
        public abstract bool IsConvertibleFromString();

        /// <summary>
        /// Converts a string into a result_DT object (of type DataType DT)
        /// Returning true if sucessful
        /// </summary>
        public abstract bool TryConvertFromString(string str, out object result_DT);

        public abstract bool IsCollectableFromModalInput();
        public abstract bool TryCollectFromModalInput(out object result);
        //public abstract bool TryCollectFromModalInput_Implementation<T>(out T result);

        public virtual string ToDisplayString(object item)
        {
            return item.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Uobj"></typeparam>
        /// <param name="obj"></param>
        /// <param name="convObj"></param>
        /// <returns></returns>
        public abstract bool TryConvert_ToObject(object obj, Type convType, out object convObj);
        public abstract bool TryConvert_FromObject(object objOfAnotherTyp, out object objOfThisTyp);


        public Type GetDataType_NonNullable()
        {
            return TypeHelpers.GetDataType_NonNullable(typeof(DT));
        }

        public abstract bool IsConvertibleToColor();
        public abstract bool TryGetColourRepresentation(object objDT, out Color color);
        public abstract DataFrameColumn Create_Column(string name, IList<object> array);
        public abstract DataFrameColumn Create_Column(string name, int length);
    }

}
