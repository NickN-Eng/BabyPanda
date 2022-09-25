using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Microsoft.Data.Analysis;
using ScriptComponents;

namespace BabyPanda.GH
{
    //WIP!!!
    public class Component_CSNET_Script_WithBP : Component_CSNET_Script
    {
        public Component_CSNET_Script_WithBP() : base()
        {
            Name = "C# Script w/ BP";
            NickName = "C# BP";
            Description = "A C#.Net scriptable component with BabyPanda references.";
            Category = GhUIConstants.RibbonCategory;
            SubCategory = GhUIConstants.Ribbon_Processing;

            _availableTypeHints = base.AvailableTypeHints;
            _availableTypeHints.Add(new GH_HintSeparator());
            _availableTypeHints.Add(new BP_DataFramehint());

            VariableParameterMaintenance();

            var dfAssembly = new DataFrame().GetType().Assembly.Location;
            var bpAssembly = new Boolean_TypeFunction().GetType().Assembly.Location;
            var bpghAssembly = new DataFrame_Goo().GetType().Assembly.Location;
            ScriptSource.References.Add(dfAssembly);
            ScriptSource.References.Add(bpAssembly);
            ScriptSource.References.Add(bpghAssembly);
            ScriptSource.UsingCode = @"using Microsoft.Data.Analysis;
using BabyPanda;
using BabyPanda.GH;";
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            //IGH_Param parameter1 = CreateParameter(GH_ParameterSide.Input, 0);
            //IGH_Param parameter2 = CreateParameter(GH_ParameterSide.Input, 1);
            //parameter1.NickName = "df";
            //parameter2.NickName = "y";
            //pManager.AddParameter(parameter1);
            //pManager.AddParameter(parameter2);
            base.RegisterInputParams(pManager);

            IGH_Param param = Params.Input[0];
            param.Name = "DataFrame";
            param.NickName = "DF";
            var scriptParam = param as Param_ScriptVariable;
            scriptParam.TypeHint = new BP_DataFramehint();
        }

        public override Guid ComponentGuid => new Guid("52e843fa-9d8c-4e68-b6d4-85954c0d9118");

        protected override Bitmap Icon => base.Icon;

        private List<IGH_TypeHint> _availableTypeHints;
        protected override List<IGH_TypeHint> AvailableTypeHints => _availableTypeHints;

    }

    public class BP_DataFramehint : IGH_TypeHint
    {
        public string TypeName => "DataFrame";

        public Guid HintID => new Guid("bb1dcb2f-fd5a-41e0-99b1-e1cde9694ad2");

        public bool Cast(object data, out object target)
        {
            if(data is DataFrame)
            {
                target = data;
                return true;
            }
            if (data is DataFrame_Goo dfgoo)
            {
                target = dfgoo.Value;
                return true;
            }

            target = null;
            return false;
        }
    }
}
