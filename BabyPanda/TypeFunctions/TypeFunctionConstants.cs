using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda
{
    /// <summary>
    /// A class for statically typed strings used in the TypeFunction name.
    /// These should uniquely identify the TypeFunction, and are used when loading 
    /// the Grasshopper parameters, so changing this will caused parameter settings not to load correctly.
    /// </summary>
    public static class TypeFunctionConstants
    {
        public const string Boolean = "Boolean";
        public const string Double  = "Double";
        public const string Integer = "Integer";
        public const string String  = "String";
        public const string Color   = "Color";
    }
}
