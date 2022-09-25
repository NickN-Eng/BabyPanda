using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BabyPanda.GHReflectionComponents
{
    /// <summary>
    /// WORK IN PROGRESS
    /// </summary>
    public abstract class GHDataType
    {
        public abstract IGH_Param GetInputParam();
        public abstract IGH_Param GetInputParam(object defaultData);
        public abstract IGH_Param GetOutputParam();
        
    }
}
