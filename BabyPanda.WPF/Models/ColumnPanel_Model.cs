using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace BabyPanda.WPF
{
    public class ColumnInspectorPanel_Model : DataTable_SidePanel
    {
        private string columnName_Editable;
        private string columnName_Original;
        private TypeItem dataType_Original;
        private ObservableCollection<TypeItem> dataType_Options = new ObservableCollection<TypeItem>();
        private TypeItem dataType_Selected;

        public ColumnInspectorPanel_Model(DatatableEditor_Model parent)
            : base(parent)
        {
            RenameColumn_Command = new RelayCommand(o => RenameColumn());
            ChangeColumnType_Command = new RelayCommand(o => ChangeColumnType());
        }

        public string ColumnName_Editable
        {
            get => columnName_Editable; 
            set
            {
                if (columnName_Editable == value) return;
                columnName_Editable = value;
                OnPropertyChanged();
            }
        }

        public string ColumnName_Original
        {
            get => columnName_Original; 
            set
            {
                columnName_Original = value;
                if (columnName_Original == value) return;
                OnPropertyChanged();
            }
        }

        public TypeItem DataType_Original
        {
            get => dataType_Original; 
            set
            {
                if (dataType_Original == value) return;
                dataType_Original = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeItem> DataType_Options
        {
            get => dataType_Options; 
            set
            {
                if (dataType_Options == value) return;
                dataType_Options = value;
                OnPropertyChanged();
            }
        }
        public TypeItem DataType_Selected
        {
            get => dataType_Selected; 
            set
            {
                if (dataType_Selected == value) return;
                dataType_Selected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UniqueItem> UniqueObjects { get; set; } = new ObservableCollection<UniqueItem>();

        /// <summary>
        /// Updates the data in this panel based on the table 
        /// </summary>
        /// <param name="columnName"></param>
        public void UpdateSelectedColumn(string columnName = null)
        {
            if (columnName == null) columnName = columnName_Original;
            if (columnName == null) return;

            var col = Parent.Table.Columns[columnName];
            if (col == null) return;

            //Update column name data
            ColumnName_Editable = col.ColumnName;
            ColumnName_Original = col.ColumnName;

            //Update type change data
            var typ = col.DataType;
            var typFunc = TypeMaster.GetTypeFunction(typ);
            var uniqueTypFuncOptions = typFunc.PermittedConversionTypes()
                                            .Select(t => TypeMaster.GetTypeFunction(t))
                                            .Where(tf => tf != null)
                                            .ToHashSet(); //Hashset to eliminate duplicate TF's (i.e. from nullable type & primitive)
            DataType_Options.Clear();
            foreach (var item in uniqueTypFuncOptions)
            {
                DataType_Options.Add(new TypeItem()
                {
                    DisplayName = item.PrintName(),
                    Type = item.GetDataType_NonNullable(),
                    TypeFunction = item
                });
            }

            DataType_Selected = DataType_Options.Where(dt => dt.TypeFunction == typFunc).FirstOrDefault();
            DataType_Original = DataType_Selected;

            //Update unique item data
            var dict = new Dictionary<object, UniqueItem>();
            var prevDict = new Dictionary<object, UniqueItem>();
            foreach (var item in UniqueObjects)
                prevDict[item.Item] = item;

            var array = DataTableHelpers.GetColumnAsArray(Parent.Table, col, false);
            for (int i = 0; i < array.Length; i++)
            {
                var obj = array[i];
                if (dict.ContainsKey(obj)) dict[obj].Count += 1;
                else
                {
                    //Reuse the previous UniqueItem such that the filter is consistent
                    if (prevDict.ContainsKey(obj))
                    {
                        var uItem = prevDict[obj];
                        uItem.Count = 1;
                        dict[obj] = uItem;
                    }
                    else
                        dict[obj] = new UniqueItem() { Item = obj, Count = 1, Selected = false };
                }
            }
            UniqueObjects.Clear();
            foreach (var uItem in dict.Values)
            {
                UniqueObjects.Add(uItem);
            }
        }

        public ICommand RenameColumn_Command { get; set; }
        public ICommand ChangeColumnType_Command { get; set; }

        public void RenameColumn()
        {
            if (!Parent.IsEditingState) return;

            string proposedName = ColumnName_Editable;
            if (proposedName == columnName_Original)
                return;
            if (Parent.Table.Columns.Contains(proposedName))
            {
                MessageBox.Show("The column name {" + proposedName + "} is already used in the table. Names must be unique.");
                return;
            }

            Parent.HasBeenEdited = true;

            Parent.Table.Columns[ColumnName_Original].ColumnName = proposedName;
            ColumnName_Original = proposedName;
            Parent.NotifyColumsUpdated();
        }

        public void ChangeColumnType()
        {
            if (!Parent.IsEditingState) return;

            var datatype = DataType_Selected;

            //This function changes column type by removing/adding column so need to refresh the entire table. (bindings not up to date)
            var result = DataTableHelpers.TryCovertColumnType(Parent.Table, ColumnName_Original, DataType_Selected.TypeFunction.GetDataType_NonNullable());

            if(!result)
            {
                MessageBox.Show("Column conversion failed. Is the type valid?");
                return;
            }

            Parent.HasBeenEdited = true;
            Parent.NotifyColumsUpdated();
            UpdateSelectedColumn();

        }
    }

    /// <summary>
    /// An item which forms a part of the unique items list in the ColumnPanel.
    /// </summary>
    public class UniqueItem : ModelBase
    {
        private object item;
        private int count;
        private bool selected;

        public object Item
        {
            get => item; set
            {
                if (item == value) return;
                item = value;
                OnPropertyChanged();
            }
        }

        public int Count
        {
            get => count; set
            {
                if (count == value) return;
                count = value;
                OnPropertyChanged();
            }
        }

        public bool Selected
        {
            get => selected; set
            {
                if (selected == value) return;
                selected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Represents a type for type selection (e.g. in a combobox).
    /// </summary>
    public class TypeItem : ModelBase
    {
        private Type type;
        private ITypeFunction typeFunction;
        private string displayName;

        public Type Type
        {
            get => type; 
            set
            {
                if (type == value) return;
                type = value;
                OnPropertyChanged();
            }
        }

        public ITypeFunction TypeFunction
        {
            get => typeFunction; 
            set
            {
                if (typeFunction == value) return;
                typeFunction = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => displayName; 
            set
            {
                if (displayName == value) return;
                displayName = value;
                OnPropertyChanged();
            }
        }
    }
}
