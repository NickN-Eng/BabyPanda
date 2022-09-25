using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using sd = System.Drawing;

namespace BabyPanda.WPF
{

    public class DatatableEditor_Model : ModelBase
    {
        public DatatableEditor_Model(DatatableEditorDialog dialog, DatatableEditor_State initialState = DatatableEditor_State.View)
        {
            Dialog = dialog;
            Table = new DataTable();
            ColumnPanel = new ColumnInspectorPanel_Model(this);
            UtilityPanel = new UtilityPanel_Model(this);

            editorState = initialState;
            EditorStateUpdate();
            HasBeenEdited = false;

            SetSelectedColumn_Command = new RelayCommand(o => SelectedColumn = (string)o);
            ClearSelectedColumn_Command = new RelayCommand(o => SelectedColumn = null);
            //TestFunction_Command = new RelayCommand(o => Testfunction());

            ResetChanges_Command = new RelayCommand(o => { TriggerRefreshFromSource(); });
            SaveChanges_Command = new RelayCommand(o => { TriggerPushToSource(); });
            SaveAndClose_Command = new RelayCommand(o => { TriggerPushToSource(); Dialog.Close(); });

            SetEditMode_Command = new RelayCommand(o => EditorState = DatatableEditor_State.Edit);
            SetViewMode_Command = new RelayCommand(o => EditorState = DatatableEditor_State.View);

            Dialog.Closed += Dialog_Closed;

        }

        private void Dialog_Closed(object sender, EventArgs e)
        {
            DialogClosed?.Invoke(Dialog);
        }

        private DatatableEditorDialog Dialog { get; set; }

        private DataTable table;
        private bool isShown_ColumnPanel = false;
        private bool isShown_UtilityPanel = false;
        private bool isShown_FilterPanel = false;
        private string selectedColumn = null; //what to do about null selected column??
        private bool hasBeenEdited;

        public DatatableEditor_State EditorState
        {
            get => editorState;
            set
            {
                if (editorState == value) return;
                editorState = value;
                EditorStateUpdate();
            }
        }

        private void EditorStateUpdate()
        {
            if (editorState == DatatableEditor_State.Edit)
                OnChangeToEditorState();
            else if (editorState == DatatableEditor_State.View)
                OnChangeToViewState();
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEditingState));
            OnPropertyChanged(nameof(IsViewState));
        }

        private void OnChangeToViewState()
        {
            ColumnPanel.UpdateSelectedColumn();
        }

        private void OnChangeToEditorState()
        {

        }

        public bool IsEditingState { get => editorState == DatatableEditor_State.Edit; }
        public bool IsViewState { get => editorState == DatatableEditor_State.View; }

        public DataTable Table
        {
            get => table;
            set
            {
                if (table != null)
                {
                    table.Columns.CollectionChanged -= Columns_CollectionChanged;
                    table.RowChanged -= Table_RowChanged;
                }
                table = value;

                if (!TableContainsColName(selectedColumn)) SelectedColumn = null;

                ColumnRefreshRequired?.Invoke();
                table.Columns.CollectionChanged += Columns_CollectionChanged;
                table.RowChanged += Table_RowChanged;
                NotifyColumsUpdated();
                OnPropertyChanged();
            }
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            HasBeenEdited = true;
        }

        public bool IsShown_ColumnPanel
        {
            get => isShown_ColumnPanel;
            set
            {
                if (value == isShown_ColumnPanel) return;
                var anyPanelIsShownCache = IsShown_AnyPanels;
                isShown_ColumnPanel = value;
                OnPropertyChanged();
                if (anyPanelIsShownCache != IsShown_AnyPanels)
                    OnPropertyChanged(nameof(IsShown_AnyPanels));
            }
        }

        public bool IsShown_UtilityPanel
        {
            get => isShown_UtilityPanel;
            set
            {
                if (value == isShown_UtilityPanel) return;
                var anyPanelIsShownCache = IsShown_AnyPanels;
                isShown_UtilityPanel = value;
                OnPropertyChanged();
                if (anyPanelIsShownCache != IsShown_AnyPanels)
                    OnPropertyChanged(nameof(IsShown_AnyPanels));
            }
        }

        public bool IsShown_FilterPanel
        {
            get => isShown_FilterPanel;
            set
            {
                if (value == isShown_FilterPanel) return;
                var anyPanelIsShownCache = IsShown_AnyPanels;
                isShown_FilterPanel = value;
                OnPropertyChanged();
                if (anyPanelIsShownCache != IsShown_AnyPanels)
                    OnPropertyChanged(nameof(IsShown_AnyPanels));
            }
        }

        public bool IsShown_AnyPanels
        {
            get => isShown_ColumnPanel || isShown_UtilityPanel || isShown_FilterPanel;
        }

        public string SelectedColumn
        {
            get
            {
                return selectedColumn;
            }

            set
            {
                if (value == selectedColumn) return;
                //OnPropertyChanged(nameof(HasNOTSelectedColumn));
                ColumnPanel.UpdateSelectedColumn(value);
                selectedColumn = value;
                OnPropertyChanged(nameof(HasSelectedColumn));
                OnPropertyChanged();

                if (HasSelectedColumn) IsShown_ColumnPanel = true;
            }
        }

        public bool AllowEditing
        {
            get => allowEditing; 
            set
            {
                if (allowEditing == value) return;
                allowEditing = value;
                OnPropertyChanged();
            }
        }

        public bool HasSelectedColumn { get => selectedColumn != null; }
        //public bool HasNOTSelectedColumn { get => selectedColumn == null; }

        public ColumnInspectorPanel_Model ColumnPanel { get; set; }
        public UtilityPanel_Model UtilityPanel { get; set; }

        #region Commands
        public ICommand SetSelectedColumn_Command { get; set; }
        public ICommand ClearSelectedColumn_Command { get; set; }

        //Misc edit mode
        public ICommand TestFunction_Command { get; set; }

        //Save/reset commands
        public ICommand ResetChanges_Command { get; set; }
        public ICommand SaveChanges_Command { get; set; }
        public ICommand SaveAndClose_Command { get; set; }

        //Set view / edit mode
        public ICommand SetEditMode_Command { get; set; }
        public ICommand SetViewMode_Command { get; set; }

        #endregion

        /// <summary>
        /// A collection of column names which is updated through the 
        /// DataTable.Columns observable collection and manually through 
        /// the NotifyColumnsRenamed method.
        /// Cannot bind directly to Table.Columns[].ColumnNames because the 
        /// DataColumn does not implement INotifyPropertyChanged. 
        /// </summary>
        public ObservableCollection<string> ColumnNames { get; set; } = new ObservableCollection<string>();

        public bool HasBeenEdited
        {
            get => hasBeenEdited;
            set
            {
                if (hasBeenEdited == value) return;
                hasBeenEdited = value;
                OnPropertyChanged();
            }
        }

        private void Columns_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            Columns_CollectionChanged();
        }

        private void Columns_CollectionChanged()
        {
            //Update the ColumnNames list
            ColumnNames.Clear();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var x = table.Columns[i];
                ColumnNames.Add(table.Columns[i].ColumnName);
            }

            ColumnRefreshRequired?.Invoke();
            HasBeenEdited = true;
        }

        public void NotifyColumsUpdated()
        {
            Columns_CollectionChanged();
        }

        private bool TableContainsColName(string colName)
        {
            if (table == null || colName == null) return false;
            return table.Columns.Contains(colName);
        }

        private DataFrame originalDataFrame;
        private DatatableEditor_State editorState;
        private bool allowEditing;

        public DataFrame GetOriginalDataFrame()
        {
            return originalDataFrame;
        }

        public void SetOriginalDataFrame(DataFrame value)
        {
            originalDataFrame = value;
            //convert dataframe to 
        }

        private void TriggerRefreshFromSource()
        {
            RefreshFromSource?.Invoke(Dialog);
            HasBeenEdited = false;
        }

        private void TriggerPushToSource()
        {
            PushToSource?.Invoke(Dialog);
        }


        //public void Testfunction()
        //{
        //    var colInspector = ColumnPanel;
        //    var items = colInspector.UniqueObjects;

        //}


        #region Events
        //Source refers to the source parameter
        public delegate void RefreshFromSourceHandler(DatatableEditorDialog dialog);
        public delegate void PushToSourceHandler(DatatableEditorDialog dialog);
        public delegate void DialogClosedHandler(DatatableEditorDialog dialog);
        public delegate void ColumnRefreshRequiredHandler();

        public event RefreshFromSourceHandler RefreshFromSource;
        public event PushToSourceHandler PushToSource;
        public event DialogClosedHandler DialogClosed;

        public event ColumnRefreshRequiredHandler ColumnRefreshRequired;
        #endregion
    }

    public enum DatatableEditor_State
    {
        View,
        Edit
    }

    //public class TableColumnsChangedEventArgs : EventArgs
    //{
    //    public DataTable Table;

    //    public TableColumnsChangedEventArgs(DataTable table)
    //    {
    //        Table = table;
    //    }
    //}
}
