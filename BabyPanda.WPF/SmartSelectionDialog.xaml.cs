using System;
using System.Collections.Generic;
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

namespace BabyPanda.WPF
{
    /// <summary>
    /// Interaction logic for SmartSelectionDialog.xaml
    /// </summary>
    public partial class SmartSelectionDialog : Window
    {
        public SmartSelectionDialog_Model Model { get; set; } 
            = new SmartSelectionDialog_Model();

        public SmartSelectionDialog()
        {
            InitializeComponent();

            //DataContext = Model;
            
            Model.SetOptions(new List<object>() { 3, "hihi", new List<object>(), "hihi", 456, 345, 34334, "dsfsfsf", "sdf" });

            //Model.TextFieldBox_TypeFunction = TypeMaster.GetTypeFunction(typeof(string));
            //Model.TextFieldBox_Object = "hiii";

            //Model.TextFieldBox_TypeFunction = TypeMaster.GetTypeFunction(typeof(System.Drawing.Color));
            //Model.TextFieldBox_Object = Color.FromArgb(100, 100, 100, 100);

            Model.TextFieldBox_TypeFunction = TypeMaster.GetTypeFunction(typeof(double));
            Model.TextFieldBox_Object = 45.34;

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var butt = (Button)sender;
            var data = butt.DataContext;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var model = Model;
            var tbbp = TextBoxBP;
        }
    }
}
