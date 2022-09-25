using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BabyPanda
{
    public class Boolean_TypeFunction : TypeFunction<BooleanDataFrameColumn, bool?>
    {
        public override string PrintName() => TypeFunctionConstants.Boolean;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(int?), typeof(int), typeof(string), typeof(bool?), typeof(bool) };

        public override bool TryConvert_Column_Implementation(BooleanDataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(BooleanDataFrameColumn))
            {
                convDFCol = new BooleanDataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(Int32DataFrameColumn))
            {
                convDFCol = new Int32DataFrameColumn(col.Name, col.Select(flag => flag.HasValue ? (flag.Value ? 1:0) : (int?)null));
                return true;
            }
            if (convColType == typeof(StringDataFrameColumn))
            {
                convDFCol = new StringDataFrameColumn(col.Name, col.Select(x => x.HasValue ? x.ToString() : null));
                return true;
            }
            convDFCol = null;
            return false;
        }

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<bool?> values)
        {
            return new BooleanDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_BoolDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => true;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            result_DT = TypeConversionHelpers.ParseStringToBool(str);
            if (result_DT != null) return true;

            if (int.TryParse(str, out int intResult))
            {
                result_DT = TypeConversionHelpers.ParseStringToBool(str);
                if (result_DT == null) return false;
                return true;
            }

            result_DT = null;
            return false;
        }

        public override bool IsCollectableFromModalInput() => false;

        public override bool TryCollectFromModalInput(out object result)
        {
            throw new NotImplementedException();
        }



        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(bool))
            {
                convObj = default;
                return false;
            }

            var value = (bool)obj;

            if (convType == typeof(bool))
            {
                convObj = value;
                return true;
            }
            if (convType == typeof(int))
            {
                convObj = value ? 1 : 0;
                return true;
            }
            if (convType == typeof(double))
            {
                convObj = value ? 1 : 0;
                return true;
            }
            if (convType == typeof(string))
            {
                convObj = value.ToString();
                return true;
            }
            convObj = default;
            return false;
        }

        public override bool TryConvert_FromObject(object objOfAnotherTyp, out object objOfThisTyp)
        {
            if(objOfAnotherTyp == null)
            {
                objOfThisTyp = null;
                return false;
            }

            if(objOfAnotherTyp is bool boolean)
            {
                objOfThisTyp = boolean;
                return true;
            }
            if (objOfAnotherTyp is int i)
            { 
                objOfThisTyp = TypeConversionHelpers.ParseIntToBool(i);
                return true;
            }
            if (objOfAnotherTyp is double d)
            {
                objOfThisTyp = TypeConversionHelpers.ParseDoubleToBool(d);
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
            return new BooleanDataFrameColumn(name, array.Select(x => (bool?)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new BooleanDataFrameColumn(name, length);
        }

        //public bool? Cast(object obj)
        //{
        //    var result = obj is bool?;
        //    return result;
        //}
    }

}
