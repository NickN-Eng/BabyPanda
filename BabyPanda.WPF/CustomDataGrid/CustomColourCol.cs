using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using wf = System.Windows.Forms;

namespace BabyPanda.WPF
{
    public class CustomColourCol : DataGridBoundColumn
    {
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var cb = new Button() { IsHitTestVisible = false, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, MinWidth = 30 };

            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay, Converter = new SysColourToMediaBrushConverter() };
            cb.SetBinding(Button.BackgroundProperty, b);
            cb.Click += SelectColourButton_Click;
            return cb;
        }

        private void SelectColourButton_Click(object sender, RoutedEventArgs e)
        {
            wf.ColorDialog colorDialog = new wf.ColorDialog();

            if (colorDialog.ShowDialog() == wf.DialogResult.OK)
            {
                var butt = (Button)sender;
                butt.Background = SysColourToMediaBrushConverter.ConvertToMediaBrush(colorDialog.Color);
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var cb = new Button() { IsHitTestVisible = false, HorizontalAlignment = HorizontalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Stretch, MinWidth = 30 };

            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay, Converter = new SysColourToMediaBrushConverter() };
            cb.SetBinding(Button.BackgroundProperty, b);
            cb.Click += SelectColourButton_Click;
            return cb;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            var butt = editingElement as Button;
            if (butt == null) 
                return new System.Windows.Media.SolidColorBrush();

            butt.IsHitTestVisible = true;
            return butt.Background;
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            var butt = editingElement as Button;
            if (butt == null) return;

            butt.Background = (System.Windows.Media.Brush)uneditedValue;
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            // The following 2 lines seem to help when sometimes the commit doesn't happen (for unknown to me reasons).
            //var cb = editingElement as CheckBox;
            //cb.IsChecked = cb.IsChecked;
            BindingExpression binding = editingElement.GetBindingExpression(Button.BackgroundProperty);
            if (binding != null) binding.UpdateSource();
            return true;// base.CommitCellEdit(editingElement);
        }
    }
    //--------------------------------------------------------------------------------------------
}
