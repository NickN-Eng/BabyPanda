using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BabyPanda
{
    /*
            new IO_BoolDataFrameColumn(),
            new IO_DoubleDataFrameColumn(),
            new IO_IntDataFrameColumn(),
            new IO_StringDataFrameColumn(),
            new IO_GeometryBaseDataFrameColumn()
     
     */
    public class Double_TypeFunction : TypeFunction<DoubleDataFrameColumn, double?>
    {
        public override string PrintName() => TypeFunctionConstants.Double;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(double?), typeof(double), typeof(int?), typeof(int), typeof(string) };

        public override bool TryConvert_Column_Implementation(DoubleDataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(DoubleDataFrameColumn))
            {
                convDFCol = new DoubleDataFrameColumn(col.Name, col);
                return true;
            }
            if (convColType == typeof(Int32DataFrameColumn))
            {
                convDFCol = new Int32DataFrameColumn(col.Name, col.Select(x => x.HasValue ? (int?) Math.Round(x.Value) : null));
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

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<double?> values)
        {
            return new DoubleDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_DoubleDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => true;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            bool success = double.TryParse(str, out double parseResult);
            result_DT = parseResult;
            return success;
        }

        public override bool IsCollectableFromModalInput() => false;

        public override bool TryCollectFromModalInput(out object result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to convert an object that is the SAME type as this TypeFunction into another type.
        /// If 
        /// </summary>
        /// <typeparam name="Uobj"></typeparam>
        /// <param name="obj"></param>
        /// <param name="convObj"></param>
        /// <returns></returns>
        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(double))
            {
                convObj = default;
                return false;
            }

            var value = (double)obj;

            if (convType == typeof(double))
            {
                convObj = value;
                return true;
            }
            if (convType == typeof(int))
            {
                convObj = (int)Math.Round(value);
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

            if (objOfAnotherTyp is double d)
            {
                objOfThisTyp = d;
                return true;
            }
            if (objOfAnotherTyp is int i)
            {
                objOfThisTyp = (double)i;
                return true;
            }
            if (objOfAnotherTyp is bool boolean)
            {
                objOfThisTyp = boolean ? 1d : 0d;
                return true;
            }
            if (objOfAnotherTyp is string s)
            {
                objOfThisTyp = TypeConversionHelpers.ParseStringToDouble(s);
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
            return new DoubleDataFrameColumn(name, array.Select(x => (double?)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new DoubleDataFrameColumn(name, length);
        }
    }

}
