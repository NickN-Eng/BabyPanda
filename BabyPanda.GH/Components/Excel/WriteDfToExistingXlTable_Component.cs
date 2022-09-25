using Grasshopper.Kernel;
using System;
using BabyPanda.Excel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Data.Analysis;

namespace BabyPanda.GH
{
    public class WriteDfToExistingXlTable_Component : GH_Component
    {
        public WriteDfToExistingXlTable_Component()
          : base(name: "Write df to existing Xl Table", nickname: "WDfXlT",
            description: "Write a dataframe to an EXISTING table within excel.",
            category: GhUIConstants.RibbonCategory, subCategory: GhUIConstants.Ribbon_Excel)
        {
        }

        public override Guid ComponentGuid => new Guid("e7d94e9d-629b-48e8-b9a8-e36ccf4131df");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var wbParam = new XlWorkbook_Parameter();
            pManager.AddParameter(wbParam);

            pManager.AddTextParameter("TableName", "TN", "Name of the table within excel.", GH_ParamAccess.item);

            var dfParam = new DataFrame_Parameter();
            pManager.AddParameter(dfParam);

            pManager.AddBooleanParameter("Append", "A", "Set to true to append data to the end of the table (default). Otherwise, it will insert and override the first row of the table", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Success", "S", "Returns true if write operation succeeded. Will fail if an exisitng excel table is not found.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            XlWorkbook_Goo workbookgoo = null;
            string tableName = null;
            DataFrame_Goo dfgoo = null;
            bool append = true;
            DA.GetData(0, ref workbookgoo);
            DA.GetData(1, ref tableName);
            DA.GetData(2, ref dfgoo);
            DA.GetData(3, ref append);

            bool success = DataFrameReadAndWrite.WriteDFToXlTable(workbookgoo.Value, dfgoo.Value, tableName, append);

            DA.SetData(0, success);

        }

        protected override System.Drawing.Bitmap Icon => null;

    }
}
