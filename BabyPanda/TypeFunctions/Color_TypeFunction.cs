using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using wf = System.Windows.Forms;

namespace BabyPanda
{
    public class Color_TypeFunction : TypeFunction<ColorDataFrameColumn, Color?>
    {
        public override string PrintName() => TypeFunctionConstants.Color;

        public override Type[] PermittedConversionTypes() => new Type[] { typeof(Color), typeof(Color?), typeof(string) };

        public override bool TryConvert_Column_Implementation(ColorDataFrameColumn col, Type convColType, out DataFrameColumn convDFCol)
        {
            if (convColType == typeof(ColorDataFrameColumn))
            {
                convDFCol = new ColorDataFrameColumn(col.Name, col);
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

        protected override DataFrameColumn CreateColumn_Implementation(string name, IEnumerable<Color?> values)
        {
            return new ColorDataFrameColumn(name, values);
        }

        protected override IO_DataFrameColumn CreateIOColumn_Implementation(DataFrameColumn column)
        {
            return new IO_StringDataFrameColumn().ToIOColumn(column);
        }

        public override bool IsConvertibleFromString() => true;

        public override bool TryConvertFromString(string str, out object result_DT)
        {
            string numbersOnly = Regex.Replace(str, "[^0-9.,]", "");
            var rgbArray = numbersOnly.Split(',');
            if(rgbArray.Length != 3)
            {
                result_DT = null;
                return false;
            }
            bool rResult = int.TryParse(rgbArray[0], out int r);
            bool gResult = int.TryParse(rgbArray[1], out int g);
            bool bResult = int.TryParse(rgbArray[2], out int b);
            if(!rResult || !gResult || !bResult || r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            {
                result_DT = null;
                return false;
            }
            result_DT = Color.FromArgb(r, g, b);
            return true;
        }

        public override bool IsCollectableFromModalInput() => true;

        public override bool TryCollectFromModalInput(out object result)
        {
            wf.ColorDialog colorDialog = new wf.ColorDialog();

            if (colorDialog.ShowDialog() == wf.DialogResult.OK)
            {
                result = colorDialog.Color;
                return true;
            }

            result = null;
            return false;
        }



        public override bool TryConvert_ToObject(object obj, Type convType, out object convObj)
        {
            if (obj == null || obj.GetType() != typeof(Color))
            {
                convObj = default;
                return false;
            }

            var value = (Color)obj;

            if (convType == typeof(Color))
            {
                convObj = value;
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

            if (objOfAnotherTyp is Color c)
            {
                objOfThisTyp = c;
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

        public override bool IsConvertibleToColor() => true;

        public override bool TryGetColourRepresentation(object objDT, out Color color)
        {
            if (objDT != null && objDT.GetType() == typeof(Color))
            {
                var poop = typeof(Color);
                var poopy = objDT.GetType();

                color = (Color)objDT;
                return true;
            }
            color = Color.Transparent;
            return false;
        }

        public override DataFrameColumn Create_Column(string name, IList<object> array)
        {
            return new ColorDataFrameColumn(name, array.Select(x => (Color?)x));
        }

        public override DataFrameColumn Create_Column(string name, int length)
        {
            return new ColorDataFrameColumn(name, length);
        }

        public override string ToDisplayString(object item)
        {
            if (item == null) return "";
            if (item is Color c)
            {
                return $"Color [{c.R},{c.G},{c.B}]";
            }
            else return "";
        }
    }

}
