using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using sd = System.Drawing;

namespace BabyPanda.WPF
{
    /// <summary>
    /// Interaction logic for DatatableEditorDialog.xaml
    /// </summary>
    public partial class DatatableEditorDialog : Window
    {
        /*
        Dialog states
        1) View and apply temp filters
           TBC:an option to save changes made by temp filters as internalise parameter
        2) Edit values and columns
         */

        //public static DependencyProperty IsEditModeProperty = DependencyProperty.Register("IsEditMode", typeof(bool), typeof(DatatableEditorDialog), new PropertyMetadata(false, OnIsEditModePropertyChanged));
        //public bool IsEditMode { get => (bool)GetValue(IsEditModeProperty); set => SetValue(IsEditModeProperty, value); }

        //private static void OnIsEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var dialog = (DatatableEditorDialog)d;
        //    if (dialog.IsEditMode)
        //    {
        //        dialog.MainDataGrid.IsReadOnly = false;
        //    }
        //    else
        //    {

        //    }
        //}



        public DatatableEditorDialog(DatatableEditor_State initialState = DatatableEditor_State.View, bool? allowEditing = null)
        {
            Model = new DatatableEditor_Model(this, initialState);
            Model.ColumnRefreshRequired += Model_ColumnRefreshRequired;

            InitializeComponent();
            
            //SetupDataTable();
            //MainDataGrid.RefreshColumns();

            Model.HasBeenEdited = false;
            if (allowEditing.HasValue)
                Model.AllowEditing = allowEditing.Value;
            else
                Model.AllowEditing = initialState != DatatableEditor_State.View;
        }

        private void Model_ColumnRefreshRequired()
        {
            MainDataGrid.RefreshColumns();

        }
        

        #region Bound data

        public DatatableEditor_Model Model { get; set; } 

        #endregion




        //private void SetupDataTable()
        //{
        //    RandomDataTableGenerator.AddSomeColumns(Model.Table);
        //    RandomDataTableGenerator.AddSomeRows(Model.Table);
        //}

        //private void Click(object sender, RoutedEventArgs e)
        //{
        //    RandomDataTableGenerator.AddSomeColumns(Model.Table);
        //    RandomDataTableGenerator.AddSomeRows(Model.Table);
        //}

        //private void ColumnPanel_ColumnSelection_Click(object sender, RoutedEventArgs e)
        //{
        //    var butt = (Button)sender;
        //    var colName = butt.DataContext as string;

        //    if (colName == null) return;
        //    Model.SelectedColumn = colName;
        //}


    }

}
