using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BabyPanda
{
    public class String_TypeFunction : TypeFunction<StringDataFrameColumn, string>
    {
        public override string PrintName() => TypeFunctionConstants.String;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(double?), typeof(double), typeof(int?), typeof(int), typeof(bool?), typeof(bool), typeof(string) };

        //Think about forcibly casting????
        public override bool TryConvert_Column_Implementation(StringDataFrameColumn col, Type convColType, out DataFrameColumn convCol)
        {
            if (convColType == typeof(StringDataFrameColumn))
            {
                convCol = new StringDataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(DoubleDataFrameColumn))
            {
                convCol = new DoubleDataFrameColumn(col.Name, col.Select(str => TypeConversionHelpers.ParseStringToDouble(str)));
                return true;
            }
            if (convColType == typeof(BooleanDataFrameColumn))
            {
                convCol = new BooleanDataFrameColumn(col.Name, col.Select(str => TypeConversionHelpers.ParseStringToBool(str)));
                return true;
            }
            if (convColType == typeof(Int32DataFrameColumn))
            {
                convCol = new Int32DataFrameColumn(col.Name, col.Select(str => TypeConversionHelpers.ParseStringToInt(str)));
                return true;
            }
            convCol = null;
            return false;
        }

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<string> values)
        {
            return new StringDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_StringDataFrameColumn().ToIOColumn(column);
        }



        public override bool IsConvertibleFromString() => true;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            result_DT = str;
            return true;
        }

        public override bool IsCollectableFromModalInput() => false;

        public override bool TryCollectFromModalInput(out object result)
        {
            throw new NotImplementedException();
        }

        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(string))
            {
                convObj = default;
                return false;
            }

            var value = (string)obj;

            if (convType == typeof(int))
            {
                convObj = TypeConversionHelpers.ParseStringToInt(value);
                //if (convObj == null) return false;
                return true;
            }
            if (convType == typeof(double))
            {
                convObj = TypeConversionHelpers.ParseStringToDouble(value);
                //if (convObj == null) return false;
                return true;
            }
            if (convType == typeof(bool))
            {
                convObj = TypeConversionHelpers.ParseStringToBool(value);
                //if (convObj == null) return false;
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

            if (objOfAnotherTyp is string s)
            {
                objOfThisTyp = s;
                return true;
            }
            if (objOfAnotherTyp is int || objOfAnotherTyp is double || objOfAnotherTyp is bool)
            {
                objOfThisTyp = objOfAnotherTyp.ToString();
                return true;
            }

            var tf = TypeMaster.GetTypeFunction(objOfAnotherTyp.GetType());
            if(tf != null)
            {
                objOfThisTyp = tf.ToDisplayString(objOfAnotherTyp);
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
            return new StringDataFrameColumn(name, array.Select(x => (string)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        { 
            return new StringDataFrameColumn(name, length);
        }
    }

}
