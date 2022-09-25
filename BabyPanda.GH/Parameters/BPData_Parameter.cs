using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GH_IO.Serialization;
using Grasshopper.GUI.HTML;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel;

namespace BabyPanda.GH
{
    public class BPData_Parameter : Param_GenericObject
    {
        //private IGH_TypeHint m_hint;
        //private bool m_showHints;
        //private List<IGH_TypeHint> m_hints;
        //private bool m_allow_trees;

        private string _SelectedTypeName = null;
        public string SelectedTypeName
        {
            get => _SelectedTypeName;
            set
            {
                _SelectedTypeName = value;
                _SelectedTypeFunction = TypeMaster.GetTypeFunction(SelectedTypeName);
                if (_SelectedTypeFunction == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "BPData data type converter could not be found for the name : " + _SelectedTypeName + ".");
                }
            }
        }

        private ITypeFunction _SelectedTypeFunction = null;

        public ITypeFunction SelectedTypeFunction
        {
            get => _SelectedTypeFunction;
        }

        private bool isListInput = true;
        public bool IsListInput
        {
            get { return isListInput; }
            set
            {
                isListInput = value;
                var prevAccess = Access;
                Access = isListInput ? GH_ParamAccess.list : GH_ParamAccess.item;
                if(prevAccess != Access)
                    OnObjectChanged(GH_ObjectEventType.DataMapping);

            }
        }
        public IEnumerable<string> AvailableTypes => TypeMaster.TypeFuctions_ByName.Keys;

        public BPData_Parameter()
        {
            //this.m_showHints = false;
            //this.m_allow_trees = false;
            IsListInput = true;
            Name = "BPData";
            NickName = "BPD";
            Description = "A collection of data filtered for use in a dataframe. Right click to set type.";
            Category = GhUIConstants.RibbonCategory;
            SubCategory = GhUIConstants.Ribbon_Data;
        }



        //public bool ShowHints
        //{
        //    get => this.m_showHints;
        //    set => this.m_showHints = value;
        //}

        //public IGH_TypeHint TypeHint
        //{
        //    get => this.m_hint;
        //    set => this.m_hint = value;
        //}

        //public List<IGH_TypeHint> Hints
        //{
        //    get => this.m_hints;
        //    set => this.m_hints = value;
        //}

        protected override void OnVolatileDataCollected()
        {
            if (m_data.DataCount == 0) return;

            if (SelectedTypeFunction == null) return;

            foreach (List<IGH_Goo> branch in m_data.Branches)
            {
                if (branch != null)
                {
                    for (int i = 0; i <= branch.Count - 1; i++)
                    {
                        if (branch[i] != null)
                        {
                            var value = branch[i].ScriptVariable();
                            
                            //Check for invalid values and set them to null
                            if (!SelectedTypeFunction.TryConvert_FromObject(value, out object obj))
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                                    string.Format("BP data could not convert data from {0} to {1}",
                                    branch[i].TypeName,
                                    SelectedTypeFunction.PrintName()));
                                branch[i] = null;
                            }
                            else
                            {
                                //It would be more efficient to save the data as soon as it is converted.
                                //But this is tricky because only goos can be stored in a parameter.
                                //To fix this, the TyeFunctions would need to know about the applicable Goo!!
                                //branch[i] = obj;

                                //Or store the data in a object goo?? 
                                //But this causes unecessary boxing, espcially if object was originally in a primitive goo which did not need to be converted
                                branch[i] = new GH_ObjectWrapper(obj);
                            }

                        }
                    }
                }
            }
            

        }

        public override string InstanceDescription => SelectedTypeFunction != null ? "BP data : " + SelectedTypeFunction.PrintName() + Environment.NewLine + base.InstanceDescription : base.InstanceDescription;


        protected override Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("{4fed6f9f-025f-4a0d-84c4-6ffb71cd3b2d}");

        public override GH_Exposure Exposure => GH_Exposure.primary; //TBC - exposed for debug only
        //public override GH_Exposure Exposure => GH_Exposure.hidden;

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);

            if (this.Kind == GH_ParamKind.output)
                return;

            Menu_AppendItem(menu, "List", new EventHandler(Menu_ListClicked), true, IsListInput);

            Menu_AppendHintItems(menu);
        }

        private void Menu_AppendHintItems(ToolStripDropDown menu)
        {
            if (AvailableTypes.Count() == 0)
            {
                Menu_AppendItem(menu, "Select Data Type", null, false);
            }
            else
            {
                ToolStripMenuItem toolStripMenuItem = Menu_AppendItem(menu, "Select Data Type");

                foreach (string typeName in AvailableTypes)
                {
                    bool isCurrentSelected = false;
                    if (SelectedTypeName != null)
                        isCurrentSelected = SelectedTypeName == typeName;
                    var tsmItem = Menu_AppendItem(toolStripMenuItem.DropDown, typeName, new EventHandler(Menu_HintClicked), true, isCurrentSelected);
                    tsmItem.Tag = typeName;
                }
            }
        }

        private void Menu_ListClicked(object sender, EventArgs e)
        {
            IsListInput = !IsListInput;
            this.ExpireSolution(true);
        }

        private void Menu_HintClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            string menuTypeName = (string)toolStripMenuItem.Tag;
            if (SelectedTypeName == menuTypeName)
                return;
            SelectedTypeName = menuTypeName;
            OnObjectChanged("BP data type changed", menuTypeName);
            ExpireSolution(true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("ShowTypeHints", IsListInput);
            writer.SetString("SelectedTypeName", SelectedTypeName);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool baseReadResult = base.Read(reader);

            bool isListInput = true;
            reader.TryGetBoolean("ShowTypeHints", ref isListInput);
            IsListInput = isListInput;

            string selectedTypeName = null;
            reader.TryGetString("SelectedTypeName", ref selectedTypeName);
            SelectedTypeName = selectedTypeName;

            return baseReadResult;
        }
    }
}
