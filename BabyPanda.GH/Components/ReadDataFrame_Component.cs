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
using System.Collections;
using GH_IO.Serialization;

namespace BabyPanda.GH
{
    public class ReadDataFrame_Component : GH_Component, IGH_VariableParameterComponent, IDisposable
    {
        /*
         TODO
            Explain how locking works in help message.
            Can lock component to Add/Remove column headings
            Once locked, can use Add/Remove features.
         
         */

        public ReadDataFrame_Component()
          : base("Read Dataframe", "RDF",
            "Description",
            GhUIConstants.RibbonCategory, GhUIConstants.Ribbon_Data)
        {
            State = ReadDataFrame_State.Auto_Generate;
        }

        public override Guid ComponentGuid => new Guid("111c8893-5f73-4d3f-9eaa-5a18f4500111");

        public enum ReadDataFrame_State
        {
            Auto_Generate = 0,
            Manual_Edit = 1,
            Locked = 2
        }

        private ReadDataFrame_State state;
        public ReadDataFrame_State State 
        { 
            get => state;
            set
            {
                state = value;
                Message = value.ToString().Replace('_', ' ');
            }
        }

        public bool IsLocked => State == ReadDataFrame_State.Locked;
        public bool IsAuto => State == ReadDataFrame_State.Auto_Generate;
        public bool IsEdit => State == ReadDataFrame_State.Manual_Edit;


        protected override System.Drawing.Bitmap Icon => null;


        //Create enum of column type as integer
        //TBC, store enum of column type, easier to serialise
        //private Dictionary<string, int> ColumnName_ParamNo; //Set to determine full range of column names
        //private Dictionary<string, (int index, Type columnType)> ColumnName_ParamNoColType; //Set to determine full range of column names
        private List<string> ColumnNames;

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
        }

        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            if (IsAuto)
            {
                AutogenerateOutputs();
            }

            ParamUsed = new bool[Params.Output.Count];
        }

        private void AutogenerateOutputs()
        {
            //Determine all the output parameters (column names) before running solve instance
            var dfParam = (DataFrame_Parameter)Params.Input[0];
            //ColumnName_ParamNo = new Dictionary<string, int>();
            //ColumnName_ParamNoColType = new Dictionary<string, (int index, Type columnType)>();

            ColumnNames = new List<string>(); //A list of column names in order for output parameter creation
            var columnNameSet = new HashSet<string>();

            foreach (DataFrame_Goo dfgoo in dfParam.VolatileData.AllData(true))
            {
                if (dfgoo.Value == null) return;

                foreach (DataFrameColumn col in dfgoo.Value.Columns)
                {
                    if (!columnNameSet.Contains(col.Name))
                    {
                        columnNameSet.Add(col.Name);
                        ColumnNames.Add(col.Name);
                    }
                    //if (!ColumnName_ParamNo.ContainsKey(col.Name))
                    //{
                    //    ColumnName_ParamNo.Add(col.Name, ColumnName_ParamNo.Count);
                    //    ColumnNames.Add(col.Name);
                    //}
                    //else
                    //{
                    //    //Check for the edge case that amongst the different dataframes in this datatree, there is another dataframe with the same column heading but different column types.  
                    //    var colType = ColumnName_ParamNoColType[col.Name].columnType;
                    //    {
                    //        //Unecessary? This is rare!
                    //        if (col.GetType() != colType)
                    //            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "DataFrameColumn named " + col.Name + " has types of both " + col.GetType().ToString() + " and " + value.columnType.ToString() + ".");
                    //    }
                    //}
                }
            }

            int reqOutputCount = ColumnNames.Count;
            if (reqOutputCount != Params.Output.Count)
            {
                //Add/remove output parameters as necessary
                if (Params.Output.Count < reqOutputCount)
                {
                    while (Params.Output.Count < reqOutputCount)
                    {
                        var new_param = CreateParameter(GH_ParameterSide.Output, Params.Output.Count);
                        Params.RegisterOutputParam(new_param);
                    }
                }
                else if (Params.Output.Count > reqOutputCount)
                {
                    while (Params.Output.Count > reqOutputCount)
                    {
                        Params.UnregisterOutputParameter(Params.Output[Params.Output.Count - 1]);
                    }
                }
            }
            UpdateVariableParamsFromColumnNames(ColumnNames);
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// A cache modified during SolveInstance that enables a popup message to be shown during AfterSolveInstance if some column parameters have not been used.
        /// </summary>
        private bool[] ParamUsed;


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DataFrame df = null;
            DA.GetData(0, ref df);
            if (df == null) return;

            for (int i = 0; i < Params.Output.Count; i++)
            {
                var param = Params.Output[i];
                var nickname = param.NickName;

                var colIndex = df.Columns.IndexOf(nickname);
                
                if(colIndex != -1)
                {
                    var col = df.Columns[colIndex];
                    ParamUsed[i] = true;
                    DA.SetDataList(i, col);
                }
            }

        }

        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            //If all parameters are used (from SolveInstance), skip this step
            if (ParamUsed.All(i => i)) return;

            //Raise a warning if any output parameters are unused.
            var unusedColNames = new List<string>();
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (!ParamUsed[i]) unusedColNames.Add(Params.Output[i].NickName);
            }
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The following output parameters have not been used: " + string.Join(", ", unusedColNames.ToArray()));
        }
        #region Variable parameter stuff


        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Output && IsEdit;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Output && IsEdit;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            var param = new Param_GenericObject();
            var name = GH_ComponentParamServer.InventUniqueNickname("xyzuvw", Params.Output);
            SetParamProperties(param, name);

            return param;
        }

        private void SetParamProperties(IGH_Param param, string name)
        {
            param.Name = name;
            param.NickName = name;
            param.Description = $"Data from column: {name}";
            param.MutableNickName = IsEdit;
            param.Access = GH_ParamAccess.list;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            if (IsLocked || IsEdit)
                CleanUpLockedVariableParams();
            else if (ColumnNames != null)
                UpdateVariableParamsFromColumnNames(ColumnNames);
        }

        private void CleanUpLockedVariableParams()
        {
            for (var i = 0; i < Params.Output.Count; i++)
            {
                SetParamProperties(Params.Output[i], Params.Output[i].NickName);
            }
        }

        private void UpdateVariableParamsFromColumnNames(IList<string> columnNames)
        {
            for (var i = 0; i < Params.Output.Count; i++)
            {
                if (i > columnNames.Count - 1) return;
                SetParamProperties(Params.Output[i], columnNames[i]);
            }
        }

        #endregion

        #region Misc component stuff

        public override bool Write(GH_IWriter writer)
        {
            
            writer.SetInt32("State", ((int)State));
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int stateInt = -1;
            if (reader.TryGetInt32("State", ref stateInt) && Enum.IsDefined(typeof(ReadDataFrame_State), stateInt))
            {
                State = (ReadDataFrame_State)stateInt;
            }
            return base.Read(reader);
        }

        //private void UpdateMessage()
        //{
        //    Message = State.ToString().Replace('_',' ');
        //}

        #endregion

        #region Disposable
        public override void ClearData()
        {
            base.ClearData();
            ColumnNames?.Clear();
            if (Params == null) return;
            foreach (var param in Params)
            {
                param?.ClearData();
            }
        }

        public void Dispose()
        {
            ClearData();
            foreach (var ghParam in Params)
            {
                ghParam.ClearData();
            }
        }
        #endregion

        #region Menu items

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            switch (State)
            {
                case ReadDataFrame_State.Auto_Generate:
                    GH_DocumentObject.Menu_AppendItem(menu, "Lock outputs", Lock_Clicked, true, false);
                    GH_DocumentObject.Menu_AppendItem(menu, "Manually edit outputs", Edit_Clicked, true, false);
                    break;
                case ReadDataFrame_State.Manual_Edit:
                    GH_DocumentObject.Menu_AppendItem(menu, "Lock outputs", Lock_Clicked, true, false);
                    GH_DocumentObject.Menu_AppendItem(menu, "Auto generate outputs from DF", Auto_Clicked, true, false);
                    break;
                case ReadDataFrame_State.Locked:
                    GH_DocumentObject.Menu_AppendItem(menu, "Manually edit outputs", Edit_Clicked, true, false);
                    GH_DocumentObject.Menu_AppendItem(menu, "Auto generate outputs from DF", Auto_Clicked, true, false);
                    break;
                default:
                    break;
            }

            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void Auto_Clicked(object sender, EventArgs e)
        {
            State = ReadDataFrame_State.Auto_Generate;
            CleanUpLockedVariableParams();
            if (IsAuto) ExpireSolution(true);
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            State = ReadDataFrame_State.Manual_Edit;
            CleanUpLockedVariableParams();
        }

        private void Lock_Clicked(object sender, EventArgs e)
        {
            State = ReadDataFrame_State.Locked;
            CleanUpLockedVariableParams();
        }

        #endregion

    }
}
