using Grasshopper.Kernel;
using System;
using BabyPanda.Excel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Data.Analysis;

namespace BabyPanda.GH
{
    public class WriteDfToNewXlTable_Component : GH_Component
    {
        public WriteDfToNewXlTable_Component()
          : base(name: "Write df to new Xl Table", nickname: "WDfXlT",
            description: "Write a dataframe to a new table within excel.",
            category: GhUIConstants.RibbonCategory, subCategory: GhUIConstants.Ribbon_Excel)
        {
        }

        public override Guid ComponentGuid => new Guid("cabc4704-4a98-411a-8a80-96ca77b7c8b7");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var wbParam = new XlWorkbook_Parameter();
            pManager.AddParameter(wbParam);

            pManager.AddTextParameter("StartCell", "SR", "The cell in the top right corner of the new table", GH_ParamAccess.item);
            pManager.AddTextParameter("Worksheet", "WS", "The worksheet for the new table. A new one will be created if it does not already exist.", GH_ParamAccess.item);

            var dfParam = new DataFrame_Parameter();
            pManager.AddParameter(dfParam);

            pManager.AddTextParameter("NewTableName", "TN", "The name for the new table.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Success", "S", "Returns true if write operation succeeded. Will fail if an exisitng excel table is not found.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            XlWorkbook_Goo workbookGoo = null;
            string startCell = null, sheetName = null, newTableName = null;
            DataFrame_Goo dfGoo = null;
            DA.GetData(0, ref workbookGoo);
            DA.GetData(1, ref startCell);
            DA.GetData(2, ref sheetName);
            DA.GetData(3, ref dfGoo);
            DA.GetData(4, ref newTableName);

            Workbook workbook = workbookGoo.Value;
            Worksheet sheet = null;
            try
            {
                sheet = (Worksheet)workbook.Sheets[sheetName];
            }
            catch (Exception)
            {
                sheet = (Worksheet)workbook.Sheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                sheet.Name = sheetName;
            }

            Range startCellRange = null;
            try
            {
                startCellRange = sheet.Range[startCell];
            }
            catch (Exception)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Start cell range could not be found.");
            }

            var df = dfGoo.Value;
            var table = DataFrameReadAndWrite.WriteDFToXlRange_AsNewTable(df, startCellRange, newTableName);

            DA.SetData(0, true); //Potential errors TBC



        }

        protected override System.Drawing.Bitmap Icon => null;

    }
}
