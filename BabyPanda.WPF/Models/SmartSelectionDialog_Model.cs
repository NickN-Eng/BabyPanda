using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using wf = System.Windows.Forms;

namespace BabyPanda.WPF
{
    public class SmartSelectionDialog_Model : ModelBase
    {
        public SmartSelectionDialog_Model()
        {
            OptionsList_InsertSelection_Command = new RelayCommand(o => OptionsList_InsertSelection(o));
            Listbox_ClearSelection_Command = new RelayCommand(o => OptionsList_ClearSelection(o));

            SelectionList_RemoveItem_Command = new RelayCommand(o => SelectionList_RemoveItem(o));
            SelectionList_RemoveAll_Command = new RelayCommand(o => SelectionList_RemoveAll());
            SelectionList_RemoveSelected_Command = new RelayCommand(o => SelectionList_RemoveSelected(o));

            TextFieldBox_AddToList_Command = new RelayCommand(o => TextFieldBox_AddToList());

            

        }

        private void TextFieldBox_AddToList()
        {
            MessageToUser = TextFieldBox_Object?.ToString();

            var objToAdd = TextFieldBox_Object;

            if (objToAdd == null)
            {
                TextFieldBox_Object = null;
                return;
            }

            if (!Selection.Contains(objToAdd))
            {
                Selection.Add(objToAdd);
                MaintainOptionsList_SelectionListRemoval(objToAdd);

            }

            TextFieldBox_Object = null;
        }

        #region Bindable data
        public ObservableCollection<object> Options { get; set; } = new ObservableCollection<object>();
        public ObservableCollection<object> Selection { get; set; } = new ObservableCollection<object>();

        public object TextFieldBox_Object
        {
            get => textFieldBox_Object; 
            set
            {
                if (textFieldBox_Object == value) return;
                textFieldBox_Object = value;
                OnPropertyChanged();
            }
        }
        public ITypeFunction TextFieldBox_TypeFunction
        {
            get => textFieldBox_TypeFunction; 
            set
            {
                if (textFieldBox_TypeFunction == value) return;
                textFieldBox_TypeFunction = value;
                OnPropertyChanged();
            }
        }

        public string MessageToUser
        {
            get => messageToUser;
            set
            {
                messageToUser = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands linked to buttons
        public ICommand OptionsList_InsertSelection_Command { get; set; }
        public ICommand Listbox_ClearSelection_Command { get; set; }
        public ICommand SelectionList_RemoveItem_Command { get; set; }
        public ICommand SelectionList_RemoveAll_Command { get; set; }
        public ICommand SelectionList_RemoveSelected_Command { get; set; }

        public ICommand TextFieldBox_AddToList_Command { get; set; }
        #endregion Commands

        public HashSet<object> OriginalOptions = new HashSet<object>();
        private string messageToUser;
        private object textFieldBox_Object;
        private ITypeFunction textFieldBox_TypeFunction;

        public void SetOptions(IEnumerable options, bool clearPrevious = true)
        {
            if (clearPrevious)
                OriginalOptions.Clear();
            foreach (var item in options)
            {
                if (OriginalOptions.Contains(item)) continue;
                OriginalOptions.Add(item);
                Options.Add(item);
            }
        }


        public void OptionsList_InsertSelection(object parameter)
        {
            var selected = new List<object>();
            foreach (var item in (IEnumerable)parameter)
            {
                selected.Add(item);
            }
            foreach (var item in selected)
            {
                Selection.Add(item);
                Options.Remove(item);
            }
        }

        public void OptionsList_ClearSelection(object parameter)
        {
            var listbox = (ListBox)parameter;
            listbox.UnselectAll();
        }

        public void SelectionList_RemoveSelected(object parameter)
        {
            var listbox = (ListBox)parameter;
            var itemList = new List<object>();
            foreach (var item in listbox.SelectedItems)
            {
                itemList.Add(item);
                MaintainOptionsList_SelectionListRemoval(item);
            }
            foreach (var item in itemList)
            {
                Selection.Remove(item);
            }
        }

        public void SelectionList_RemoveAll()
        {
            foreach (var item in Selection)
            {
                MaintainOptionsList_SelectionListRemoval(item);
            }
            Selection.Clear();
        }

        public void SelectionList_RemoveItem(object parameter)
        {
            var data = parameter;
            MaintainOptionsList_SelectionListRemoval(data);
            Selection.Remove(data);
        }

        private void MaintainOptionsList_SelectionListRemoval(object removedItem)
        {
            if (OriginalOptions.Contains(removedItem) && !Options.Contains(removedItem))
                Options.Add(removedItem);
        }


    }
}