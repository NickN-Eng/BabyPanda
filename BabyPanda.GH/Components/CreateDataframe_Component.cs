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

    public class CreateDataframe_Component : GH_Component, IGH_VariableParameterComponent
    {

        public CreateDataframe_Component()
          : base("Create Dataframe", "CDF",
            "Description",
            GhUIConstants.RibbonCategory, GhUIConstants.Ribbon_Data)
        {
        }

        public override Guid ComponentGuid => new Guid("af484adb-c52a-44a3-85e4-c9e9bf4d36bd");

        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            BPData_Parameter bpdata1 = CreateNewParameter("Col1");
            pManager.AddParameter(bpdata1);

            var bpdata2 = CreateNewParameter("Col2");
            pManager.AddParameter(bpdata2);

            this.VariableParameterMaintenance();
        }

        private static BPData_Parameter CreateNewParameter(string colName)
        {
            var bpdata = new BPData_Parameter();
            bpdata.Name = colName;
            bpdata.NickName = colName;
            bpdata.Description = "Data for a DataFrameColumn";
            bpdata.SelectedTypeName = TypeFunctionConstants.String;
            return bpdata;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            var dfParam = new DataFrame_Parameter();
            dfParam.Name = "DataFrame";
            dfParam.NickName = "DF";
            dfParam.Description = "A Microsoft.Data.Analysis Dataframe.";
            dfParam.Access = GH_ParamAccess.item;
            pManager.AddParameter(dfParam);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var txt = new List<string>();
            txt.Add("Run " + DA.Iteration.ToString());
            var columns = new DataFrameColumn[this.Params.Input.Count];
            int length = -1;
            for (int i = 0; i < this.Params.Input.Count; i++)
            {
                //Extract variables from param
                var param = Params.Input[i];
                GH_ParamAccess access = param.Access;
                BPData_Parameter bpdataParam = (BPData_Parameter)param;
                ITypeFunction hint = bpdataParam.SelectedTypeFunction;

                if (hint == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A valid type must be selected for the parameter named " + bpdataParam.Name + ".");
                    return;
                }

                //Create the appropriate DataFrameColumn
                if(access == GH_ParamAccess.list)
                {
                    DataFrameColumn iCol = GHTypeFunctions.ExtractColumnFromDA(bpdataParam.SelectedTypeFunction, param.Name, DA, i); ;
                    columns[i] = iCol;

                    //Save the first column length encountered. Check all subsequent column lengths against this one
                    //Alternative implementation could allow columns of varying length, but fill with null
                    if (length == -1) length = (int)iCol.Length;
                    else if (length != iCol.Length)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Columns must all be of the same length");
                        return;
                    }
                }
            }

            if (length == -1) length = 1; //If the length is still not populated, set it to 1.

            for (int i = 0; i < this.Params.Input.Count; i++)
            {
                var param = Params.Input[i];
                GH_ParamAccess access = param.Access;
                BPData_Parameter bpdataParam = (BPData_Parameter)param;

                if (access == GH_ParamAccess.item)
                {
                    columns[i] = GHTypeFunctions.ExtractColumnFromDA_DuplicateItem(bpdataParam.SelectedTypeFunction, param.Name, DA, i, length);
                }
            }

            //Check that column names are unique
            var uniqueNames = columns.Select(c => c.Name).ToHashSet();
            if (uniqueNames.Count != columns.Length)
                throw new Exception("Column names must be unique. Please rename the input parameters to edit column names.");

            var df = new DataFrame(columns);

            DA.SetData(0, df);
        }

        public override void AddedToDocument(GH_Document document)
        {
            this.Params.ParameterChanged -= new GH_ComponentParamServer.ParameterChangedEventHandler(this.ParameterChanged);
            this.Params.ParameterChanged += new GH_ComponentParamServer.ParameterChangedEventHandler(this.ParameterChanged);
        }

        public override void RemovedFromDocument(GH_Document document) => this.Params.ParameterChanged -= new GH_ComponentParamServer.ParameterChangedEventHandler(this.ParameterChanged);

        private void ParameterChanged(object sender, GH_ParamServerEventArgs e)
        {
            switch (e.OriginalArguments.Type)
            {
                case GH_ObjectEventType.NickName:
                    break;
                case GH_ObjectEventType.NickNameAccepted:
                    this.ExpireSolution(true);
                    break;
                case GH_ObjectEventType.Icon:
                    break;
                case GH_ObjectEventType.IconDisplayMode:
                    break;
                case GH_ObjectEventType.Sources:
                    break;
                case GH_ObjectEventType.Selected:
                    break;
                case GH_ObjectEventType.Enabled:
                    break;
                case GH_ObjectEventType.Preview:
                    break;
                case GH_ObjectEventType.PersistentData:
                    break;
                case GH_ObjectEventType.DataMapping:
                    break;
                default:
                    this.ExpireSolution(true);
                    break;
            }
        }

        public string TooltipText { get; private set; }

        //protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        //{
        //    //Adding a menu item which is bold
        //    ToolStripMenuItem toolStripMenuItem1 = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Edit Source…", new EventHandler(this.Menu_Placeholder));
        //    toolStripMenuItem1.Font = GH_FontServer.NewFont(toolStripMenuItem1.Font, FontStyle.Bold);

        //    if (true)
        //        GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Remove out", new EventHandler(this.Menu_ReinstateOutClicked)).ToolTipText = "Hide the [out] parameter";
        //    else
        //        GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Reinstate out", new EventHandler(this.Menu_ReinstateOutClicked)).ToolTipText = "Display the [out] parameter again";

        //    //Add seperator
        //    GH_DocumentObject.Menu_AppendSeparator((ToolStrip)menu);

        //    //Assigning a value to the component. How to make this data persistent??
        //    ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Tooltip Text");
        //    GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem2.DropDown, this.TooltipText, new GH_MenuTextBox.KeyDownEventHandler(this.Menu_TextEntryKeyDown), (GH_MenuTextBox.TextChangedEventHandler)null, true, 200, true);

        //    //Add seperator
        //    GH_DocumentObject.Menu_AppendSeparator((ToolStrip)menu);

        //}

        //private void Menu_Placeholder(object sender, EventArgs e)
        //{

        //}

        //private void Menu_TextEntryKeyDown(GH_MenuTextBox sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode != Keys.Return || this.TooltipText == sender.Text)
        //        return;
        //    this.RecordUndoEvent("Tooltip Text");
        //    this.TooltipText = sender.Text;
        //}

        //private void Menu_ReinstateOutClicked(object sender, EventArgs e)
        //{
        //    if (this.HasOutParameter)
        //    {
        //        this.RecordUndoEvent("Remove out");
        //        this.Params.UnregisterOutputParameter(this.Params.Output[0], true);
        //        this.ExpireSolution(true);
        //    }
        //    else
        //    {
        //        this.RecordUndoEvent("Reinstate out");
        //        Param_String new_param = new Param_String();
        //        new_param.Access = GH_ParamAccess.list;
        //        new_param.Name = "out";
        //        new_param.NickName = "out";
        //        new_param.Description = "Print, Reflect and Error streams";
        //        this.Params.RegisterOutputParam((IGH_Param)new_param, 0);
        //        this.ExpireSolution(true);
        //    }
        //}

        //internal bool HasOutParameter => this.Params.Output.Count != 0 && this.Params.Output[0] is Param_String;

        //internal int FirstOutputIndex => !this.HasOutParameter ? 0 : 1;



        public bool CanInsertParameter(GH_ParameterSide side, int index) => side == GH_ParameterSide.Input;

        public bool CanRemoveParameter(GH_ParameterSide side, int index) => true;

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            IGH_Param parameter;
            switch (side)
            {
                case GH_ParameterSide.Input:
                    string name = GH_ComponentParamServer.InventUniqueNickname("xyzuvw", Params.Input);
                    parameter = CreateNewParameter(name);
                    break;
                case GH_ParameterSide.Output:
                default:
                    parameter = null;
                    break;
            }
            return parameter;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 0; i < this.Params.Input.Count; i++)
            {
                IGH_Param ghParam = this.Params.Input[i];
                ghParam.Name = ghParam.NickName;
                ghParam.Description = string.Format("BPData {0}", (object)ghParam.NickName);
                //if(ghParam is BPData_Parameter bPData_Parameter)
                //{
                //     //Some code to configure bpData settings??
                //}
            }
        }
    }
}
