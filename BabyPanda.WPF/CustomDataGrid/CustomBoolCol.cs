using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BabyPanda.WPF
{
    public class MyCheckBoxColumn : DataGridBoundColumn
    {
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var cb = new CheckBox() { IsHitTestVisible = false, HorizontalAlignment = HorizontalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center };
            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay };
            cb.SetBinding(ToggleButton.IsCheckedProperty, b);
            return cb;
            //var cb = new TextBlock() { TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            //var bb = this.Binding as Binding;
            //var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay, Converter = new MyBoolToMarkConverter() };
            //cb.SetBinding(TextBlock.TextProperty, b);
            //return cb;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var cb = new CheckBox() { HorizontalAlignment = HorizontalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center };
            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay };
            cb.SetBinding(ToggleButton.IsCheckedProperty, b);
            return cb;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            var cb = editingElement as CheckBox;
            if (cb != null) return cb.IsChecked;
            return false;
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            var cb = editingElement as CheckBox;
            if (cb != null) cb.IsChecked = (bool)uneditedValue;
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            // The following 2 lines seem to help when sometimes the commit doesn't happen (for unknown to me reasons).
            //var cb = editingElement as CheckBox;
            //cb.IsChecked = cb.IsChecked;
            BindingExpression binding = editingElement.GetBindingExpression(ToggleButton.IsCheckedProperty);
            if (binding != null) binding.UpdateSource();
            return true;// base.CommitCellEdit(editingElement);
        }
    }
    //--------------------------------------------------------------------------------------------
    //public class MyBoolToMarkConverter : IValueConverter
    //{
    //    const string cTick = "■";

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value.GetType() != typeof(bool)) return "";
    //        bool val = (bool)value;
    //        return val ? cTick : "";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value.GetType() != typeof(string)) return false;
    //        string val = (string)value;
    //        return val == cTick;
    //    }
    //}
    //--------------------------------------------------------------------------------------------
}
