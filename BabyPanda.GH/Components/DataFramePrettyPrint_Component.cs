using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using Microsoft.Data.Analysis;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BabyPanda;
using System.Linq;

namespace BabyPanda.GH
{
    public class DataFramePrettyPrint_Component : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DataFramePrettyPrint_Component()
          : base("Pretty Print Dataframe", "PPDF",
            "Description",
            GhUIConstants.RibbonCategory, GhUIConstants.Ribbon_Display)
        {
        }

        public override Guid ComponentGuid => new Guid("10b29c35-93ef-4a8f-ab77-b7e69ef163b0");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var dfParam = new DataFrame_Parameter();
            dfParam.Name = "DataFrame";
            dfParam.NickName = "DF";
            dfParam.Description = "A Microsoft.Data.Analysis Dataframe.";
            dfParam.Access = GH_ParamAccess.item;
            pManager.AddParameter(dfParam);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("DF Text", "Txt", "Dataframe as pretty text.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var df = new DataFrame(new DataFrameColumn[0]);
            if (!DA.GetData<DataFrame>(0, ref df))
                return;

            DA.SetData(0, PrettyPrinters.PrettyText(df));
        }
    }
}
