using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda
{
    public static class ITypeFunctionExtensions
    {
        public static string[] ConvertToDisplayStringArray(this ITypeFunction tf, DataFrameColumn col)
        {
            var result = new string[col.Length];
            for (int i = 0; i < col.Length; i++)
            {
                result[i] = tf.ToDisplayString(col[i]);
            }
            return result;
        }
    }
}
