using Grasshopper.Kernel.Types;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.GH
{
    public class XlWorkbook_Goo : GH_Goo<Workbook>
    {
        public override bool IsValid
        {
            get
            {
                if (Value == null) return false;

                //Try to access the worksheets. Should throw an error if the workbook has since been closed etc...
                try
                {
                    var worksheets = Value.Worksheets;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public override string TypeName => "XlWorkbook";

        public override string TypeDescription => throw new NotImplementedException();

        public XlWorkbook_Goo(Workbook wb)
        {
            Value = wb;
        }

        public XlWorkbook_Goo(XlWorkbook_Goo wbGoo)
        {
            Value = wbGoo.Value;
        }

        public XlWorkbook_Goo()
        {
            Value = null;
        }

        public override IGH_Goo Duplicate()
        {
            return new XlWorkbook_Goo(Value);
        }

        public override string ToString()
        {
            return $"XlWorkbook [{Value.Name}]";
        }
    }
}
