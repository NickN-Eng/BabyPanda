using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BabyPanda
{
    public static class TypeConversionHelpers
    {
        public static bool? ParseIntToBool(int i)
        {
            switch (i)
            {
                case 1:
                    return true;
                case 0:
                case -1:
                    return false;
                default:
                    return null;
            }
        }

        public static bool? ParseDoubleToBool(double d)
        {
            if (d < 1.001 && d > 0.999) return true;
            if (d < 0.001 && d > -0.001) return false;
            return null;
        }

        public static bool? ParseStringToBool(string str)
        {
            switch (str.ToLower())
            {
                case "yes":
                case "true":
                    return true;
                case "no":
                case "false":
                    return false;
                default:
                    return null;
            }
        }

        public static double? ParseStringToDouble(string str)
        {
            if (double.TryParse(str, out double value))
                return value;
            else
                return null;
        }

        public static int? ParseStringToInt(string str)
        {
            if (int.TryParse(str, out int value))
                return value;
            else if (double.TryParse(str, out double dvalue))
                return (int)Math.Round(dvalue);
            else
                return null;
        }

        public static (double, double, double)? ParseStringToVector3d(string str)
        {
            string numbersOnly = Regex.Replace(str, "[^0-9.,]", "");
            var rgbArray = numbersOnly.Split(',');
            if (rgbArray.Length != 3)
            {
                return null;
            }
            bool xResult = double.TryParse(rgbArray[0], out double x);
            bool yResult = double.TryParse(rgbArray[1], out double y);
            bool zResult = double.TryParse(rgbArray[2], out double z);
            if (!xResult || !yResult || !zResult )
            {
                return null;
            }
            return (x, y, z);
        }
    }
}
