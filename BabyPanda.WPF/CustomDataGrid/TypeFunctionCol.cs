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
    public class TypeFunctionCol : DataGridBoundColumn
    {
        public ITypeFunction TypeFunc { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var tb = new TextBoxForBPTypes { IsHitTestVisible = false, HorizontalAlignment = HorizontalAlignment.Stretch, TypeFunc = TypeFunc };

            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay };
            tb.SetBinding(TextBoxForBPTypes.SourceObjectProperty, b);
            return tb;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var tb = new TextBoxForBPTypes { IsHitTestVisible = false, HorizontalAlignment = HorizontalAlignment.Stretch, TypeFunc = TypeFunc };

            var bb = this.Binding as Binding;
            var b = new Binding { Path = bb.Path, Source = dataItem, Mode = BindingMode.TwoWay };
            tb.SetBinding(TextBoxForBPTypes.SourceObjectProperty, b);
            return tb;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            var tb = editingElement as TextBoxForBPTypes;
            if (tb == null) 
                return null;

            tb.IsHitTestVisible = true;
            return tb.SourceObject;
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            var tb = editingElement as TextBoxForBPTypes;
            if (tb == null) return;

            tb.SourceObject = uneditedValue;
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            // The following 2 lines seem to help when sometimes the commit doesn't happen (for unknown to me reasons).
            //var cb = editingElement as CheckBox;
            //cb.IsChecked = cb.IsChecked;
            BindingExpression binding = editingElement.GetBindingExpression(TextBoxForBPTypes.SourceObjectProperty);
            if (binding != null) binding.UpdateSource();
            return true;// base.CommitCellEdit(editingElement);
        }
    }
    //--------------------------------------------------------------------------------------------
}
