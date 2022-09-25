using BabyPanda.WPF;
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
using System.Linq;

namespace BabyPanda.GH
{
    public class DataFrame_Parameter : GH_PersistentParam<DataFrame_Goo>
    {
        /*
         WIP, open in viewing mode???
         
         
         */



        /// <summary>
        /// Only allow editing on parameters which are floating and input.
        /// For these parameters, the data type must be local (otherwise setting the persistent data will not cause the param to change) 
        /// ...since GH_ParamData.remote parameters inherit data upstream rather than show persistent data.
        /// An alternative behaviour is to allow GH_ParamData.remote, but this means saving changes will not cause immediate change
        /// ...because the param value will only change when the input is removed.
        /// </summary>
        public bool AllowEditing => (Kind == GH_ParamKind.floating || Kind == GH_ParamKind.input) && (DataType == GH_ParamData.local);


        public DataFrame_Parameter() :base("DataFrame", "DF",
            "A table of data.",
            GhUIConstants.RibbonCategory, GhUIConstants.Ribbon_Data)
        { }

        public override Guid ComponentGuid => new Guid("b67cb78e-54d6-4106-9fae-83b85a82d6cf");

        protected override void Menu_AppendPromptOne(ToolStripDropDown menu)
        {
            base.Menu_AppendPromptOne(menu);
        }

        protected override GH_GetterResult Prompt_Plural(ref List<DataFrame_Goo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref DataFrame_Goo value)
        {
            //WIP - Start on edit mode
            ShowDialog(DatatableEditor_State.Edit);
            return GH_GetterResult.cancel;
        }

        DatatableEditorDialog DataTableDialog { get; set; }

        private void ShowDialog(DatatableEditor_State initialState)
        {
            if (DataTableDialog != null)
            {
                DataTableDialog.Activate();
                return;
            }


            DataTableDialog = new DatatableEditorDialog(initialState, AllowEditing);

            Model_RefreshFromSource(DataTableDialog);

            DataTableDialog.Model.RefreshFromSource += Model_RefreshFromSource;
            DataTableDialog.Model.PushToSource += Model_PushToSource;
            DataTableDialog.Model.DialogClosed += Model_DialogClosed;

            DataTableDialog.Model.HasBeenEdited = false;

            DataTableDialog.Show();
        }

        private void Model_PushToSource(DatatableEditorDialog dialog)
        {
            var table = dialog.Model.Table;
            var df = DataTableHelpers.Convert(table);
            if (df == null) return;

            RecordPersistentDataEvent("See data from DataFrame dialog.");
            PersistentData.Clear();
            var dfGoo = new DataFrame_Goo(df);
            SetPersistentData(dfGoo);
        }

        private void Model_RefreshFromSource(DatatableEditorDialog dialog)
        {
            var x = VolatileData.AllData(true);
            var data = x.FirstOrDefault();
            if (data == null)
            {
                dialog.Model.Table = new System.Data.DataTable();
                return;
            }

            var dfGoo = (DataFrame_Goo)data;
            var table = DataTableHelpers.Convert(dfGoo.Value);
            dialog.Model.Table = table;
        }

        private void Model_DialogClosed(DatatableEditorDialog dialog)
        {
            if(DataTableDialog != null)
            {
                DataTableDialog.Model.RefreshFromSource -= Model_RefreshFromSource;
                DataTableDialog.Model.PushToSource -= Model_PushToSource;
                DataTableDialog.Model.DialogClosed -= Model_DialogClosed;

                DataTableDialog = null;
            }
        }

        private void ShowDialogHandler(object sender, EventArgs e)
        {
            ShowDialog(DatatableEditor_State.View);
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            string tooltipName, tooltipText;
            if (AllowEditing)
            {
                tooltipName = "View / edit datatable";
                tooltipText = "Shows the DataFrame viewer, with the option to edit.";
            }
            else
            {
                tooltipName = "View datatable";
                tooltipText = "Shows the DataFrame viewer, in View only mode. To edit a dataframe, internalise the parameter first.";
            }
            ToolStripMenuItem item = Menu_AppendItem(menu, tooltipName, ShowDialogHandler, true);
            item.ToolTipText = tooltipText;

            base.AppendAdditionalMenuItems(menu);
        }
    }
}
