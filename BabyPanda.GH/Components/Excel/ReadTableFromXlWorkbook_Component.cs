using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyPanda;
using BabyPanda.Excel;
using Microsoft.Office.Interop.Excel;

namespace BabyPanda.GH
{


    public class ReadTableFromXlWorkbook_Component : BP_Component
    {
        public ReadTableFromXlWorkbook_Component()
          : base(name: "Read table from Xl Workbook", nickname: "RTXlWb",
            description: "Read an existing table from the given excel workbook.",
            category: GhUIConstants.RibbonCategory, subCategory: GhUIConstants.Ribbon_Excel)
        {
        }

        public override Guid ComponentGuid => new Guid("6c04d4fb-9221-4430-8e22-45a23d2b69d0");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var wbParam = new XlWorkbook_Parameter();
            pManager.AddParameter(wbParam);

            pManager.AddTextParameter("TableName", "TN", "Name of the table within excel.", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            var dfParam = new DataFrame_Parameter();
            pManager.AddParameter(dfParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA_GetDataAndCheckNull(DA, 0, out XlWorkbook_Goo workbookgoo)) return;
            if (!DA_GetDataAndCheckNull(DA, 1, out string tableName )) return;

            Microsoft.Data.Analysis.DataFrame df = DataFrameReadAndWrite.ReadDFFromXlTable(workbookgoo.Value, tableName);

            DA.SetData(0, df);

        }

        protected override System.Drawing.Bitmap Icon => null;

    }
}
