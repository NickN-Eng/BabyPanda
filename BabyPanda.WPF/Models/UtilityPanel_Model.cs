using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BabyPanda;
using System.Linq;
namespace BabyPanda.WPF
{
    public class UtilityPanel_Model : DataTable_SidePanel
    {
        public UtilityPanel_Model(DatatableEditor_Model parent) : base(parent)
        {
            foreach (var item in TypeMaster.TypeFuctions.Values.ToHashSet())
            {
                AddColumn_DataType_Options.Add(new TypeItem()
                {
                    DisplayName = item.PrintName(),
                    Type = item.GetDataType_NonNullable(),
                    TypeFunction = item
                });
            }

            PasteExcelDataFromClipboard_FirstRowAsHeaders_Command = new RelayCommand(o => PasteExcelDataFromClipboard(true));
            PasteExcelDataFromClipboard_Command = new RelayCommand(o => PasteExcelDataFromClipboard(false));
            AddColumn_Command = new RelayCommand(o => AddColumn());
        }

        public ICommand PasteExcelDataFromClipboard_FirstRowAsHeaders_Command { get; set; }
        public ICommand PasteExcelDataFromClipboard_Command { get; set; }

        public ICommand AddColumn_Command { get; set; }

        private ObservableCollection<TypeItem> addColumn_dataType_Options = new ObservableCollection<TypeItem>();
        private TypeItem addColumn_dataType_Selected;
        private string addColumn_ColumnName;

        public ObservableCollection<TypeItem> AddColumn_DataType_Options
        {
            get => addColumn_dataType_Options;
            set
            {
                if (addColumn_dataType_Options == value) return;
                addColumn_dataType_Options = value;
                OnPropertyChanged();
            }
        }
        public TypeItem AddColumn_DataType_Selected
        {
            get => addColumn_dataType_Selected;
            set
            {
                if (addColumn_dataType_Selected == value) return;
                addColumn_dataType_Selected = value;
                OnPropertyChanged();
            }
        }
        public string AddColumn_ColumnName
        {
            get => addColumn_ColumnName; 
            set
            {
                if (addColumn_ColumnName == value) return;
                addColumn_ColumnName = value;
                OnPropertyChanged();
            }
        }

        public void PasteExcelDataFromClipboard(bool firstRowAsHeaders)
        {
            Parent.Table = DataTableHelpers.GetExcelDataFromClipboard(firstRowAsHeaders);
            Parent.HasBeenEdited = true;
        }

        public void AddColumn()
        {
            //Check if column name is already used and raise message
            var proposedName = AddColumn_ColumnName;

            if (proposedName == null)
            {
                MessageBox.Show("Please enter a column name for the new column.");
                return;
            }

            if (Parent.Table.Columns.Contains(proposedName))
            {
                MessageBox.Show("The column name {" + proposedName + "} is already used in the table. Column names must be unique.");
                return;
            }

            if(AddColumn_DataType_Selected == null)
            {
                MessageBox.Show("Please select a data type for the new column.");
                return;
            }

            Parent.Table.Columns.Add(proposedName, AddColumn_DataType_Selected.Type);
            Parent.HasBeenEdited = true;
            Parent.NotifyColumsUpdated();
        }
    }
}
