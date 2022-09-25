using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BabyPanda
{
    public class Integer_TypeFunction : TypeFunction<Int32DataFrameColumn, int?>
    {
        public override string PrintName() => TypeFunctionConstants.Integer;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(double?), typeof(double), typeof(int?), typeof(int), typeof(bool?), typeof(bool), typeof(string) };

        public override bool TryConvert_Column_Implementation(Int32DataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(Int32DataFrameColumn))
            {
                convDFCol = new Int32DataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(DoubleDataFrameColumn))
            {
                convDFCol = new DoubleDataFrameColumn(col.Name, col.Select(x => x.HasValue ? (double?)x : null));
                return true;
            }
            if (convColType == typeof(BooleanDataFrameColumn))
            {
                convDFCol = new BooleanDataFrameColumn(col.Name, col.Select(x => x.HasValue ? TypeConversionHelpers.ParseIntToBool(x.Value) : null));
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

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<int?> values)
        {
            return new Int32DataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_IntDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => true;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            if (int.TryParse(str, out int intResult))
            {
                result_DT = intResult;
                return true;
            }
            else if (double.TryParse(str, out double dResult))
            {
                result_DT = Math.Round(dResult);
                return true;
            }
            else
            {
                result_DT = null;
                return false;
            }
        }

        public override bool IsCollectableFromModalInput() => false;

        public override bool TryCollectFromModalInput(out object result)
        {
            throw new NotImplementedException();
        }

        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(int))
            {
                convObj = default;
                return false;
            }

            var value = (int)obj;

            if (convType == typeof(int))
            {
                convObj = value;
                return true;
            }
            if (convType == typeof(double))
            {
                convObj = (double)value;
                return true;
            }
            if (convType == typeof(bool))
            {
                convObj = TypeConversionHelpers.ParseIntToBool(value);
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
            if (objOfAnotherTyp == null)
            {
                objOfThisTyp = null;
                return false;
            }

            if (objOfAnotherTyp is int i)
            {
                objOfThisTyp = i;
                return true;
            }
            if (objOfAnotherTyp is double d)
            {
                objOfThisTyp = (int)Math.Round(d);
                return true;
            }
            if (objOfAnotherTyp is bool boolean)
            {
                objOfThisTyp = boolean ? 1 : 0;
                return true;
            }
            if (objOfAnotherTyp is string s)
            {
                objOfThisTyp = TypeConversionHelpers.ParseStringToInt(s);
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
            return new Int32DataFrameColumn(name, array.Select(x => (int?)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new Int32DataFrameColumn(name, length);
        }
    }

}
