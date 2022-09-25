using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;

namespace BabyPanda.GH
{
    public class XlWorkbook_Parameter : GH_Param<XlWorkbook_Goo>
    {
        public XlWorkbook_Parameter() : base("XlWorkbook", "XlWb","A link to an Excel workbook for reading and writing.",GhUIConstants.RibbonCategory, GhUIConstants.Ribbon_Excel, GH_ParamAccess.item)
        { }

        public override Guid ComponentGuid => new Guid("7af1dbf4-4cd6-4c4c-b5b5-fdba5d12faaa");
    }
}
