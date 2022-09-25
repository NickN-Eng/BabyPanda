using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda
{
    public static class TypeHelpers
    {
        public static Type GetDataType_NonNullable(Type typ)
        {
            if (typ.IsGenericType && typ.UnderlyingSystemType.Name == typeof(Nullable<>).Name)
                return typ.GenericTypeArguments[0];
            else
                return typ;
        }
    }
}
