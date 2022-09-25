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

    public class OpenXlWorkbook_Component : GH_Component
    {
        public OpenXlWorkbook_Component()
          : base(name: "Open Xl Workbook", nickname: "OXlWb",
            description: "Open an existing excel workbook from the Filepath/Filename prompts.",
            category: GhUIConstants.RibbonCategory, subCategory: GhUIConstants.Ribbon_Excel)
        {
        }

        public override Guid ComponentGuid => new Guid("ec25d94c-61f6-4651-831c-0d8554c7f76a");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Filepath", "FP", "Filepath to excel workbook. Takes precedence over Filename, which is an alternative.", GH_ParamAccess.item, "");

            //TBC, edit ExcelWbOpener code to make it less susceptible to file extension errors??
            pManager.AddTextParameter("Filename", "FN", "Filename of excel workbook, leading to a search of all open workbooks. The file extension (i.e. .xlsx is required - unless try to open an unsaved workbook which has no file extension). This will be ignored if a valid Filepath is provided.", GH_ParamAccess.item, "");
            
            pManager.AddBooleanParameter("TryGrabExisting", "TGE", "Tries to grab a window which is already open according to the filepath/filename. This setting takes priority over TryWrite, so if the existing window is ready only, it shall remain read only & TryWrite will be ignored.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("TryWrite", "TW", "If a new window is opened, shall attempt to open in Write mode.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("ShowWindow", "SW", "Sets the application to visible if true. Hiding the window is not recommended.", GH_ParamAccess.item, true);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            var wbParam = new XlWorkbook_Parameter();
            pManager.AddParameter(wbParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string filepath = null, filename = null;
            bool tryGrabExisting = true, tryWrite = true, showWindow = true;
            DA.GetData(0, ref filepath);
            DA.GetData(1, ref filename);
            DA.GetData(2, ref tryGrabExisting);
            DA.GetData(3, ref tryWrite);
            DA.GetData(4, ref showWindow);

            if(string.IsNullOrEmpty(filepath) && string.IsNullOrEmpty(filename))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Filepath or filename must be provided.");
                return;
            }

            var opener = new ExcelWbOpener()
            {
                Filepath = string.IsNullOrEmpty(filepath)? null : filepath,
                Filename = string.IsNullOrEmpty(filename)? null : filename,
                TryGrabExisting = tryGrabExisting,
                TryWrite = tryWrite,
                ShowWindow = showWindow
            };

            bool success = opener.Execute(out Workbook wb, out bool isNewWindow);

            if (success)
            {
                DA.SetData(0, new XlWorkbook_Goo(wb));
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Existing workbook could not be found.");
            }
        }

        protected override System.Drawing.Bitmap Icon => null;

    }
}
