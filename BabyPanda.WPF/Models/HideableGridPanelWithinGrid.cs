//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;

//namespace BabyPanda.WPF
//{
//    /// <summary>
//    /// WIP
//    /// A class to control the visibility of a grid column based on a columnIsVisible boolean property.
//    /// Need to use a grid rather than a stackpanel/dockpanel because this enables the GridSplitter to be used.
//    /// Desired behaviour: hide/show gridColumn by toggling a boolean property. 
//    ///     This must be done by setting column width to zero.
//    ///     However, the previous column width needs to be saved for when the column is un-hidden.
//    /// Previously tried to use VM bindings to 
//    /// </summary>
//    public class HideableGridPanelWithinGrid : ModelBase
//    {
//        public DatatableEditor_Model Parent { get; set; }

//        private bool columnIsVisible = false;
//        private ColumnDefinition columnDefinition;

//        public double CachedWidth { get; private set; }

//        public double RequiredWidth => columnIsVisible ? CachedWidth : 0;

//        public bool ColumnIsVisible
//        {
//            get
//            {
//                return columnIsVisible;
//            }

//            set
//            {
//                if (value == columnIsVisible) return;
//                columnIsVisible = value;

//                //Cache the last width of the column before setting to zero
//                if (!columnIsVisible) CachedWidth = columnDefinition.ActualWidth;

//                //Change the visibility of the column by adusting the column width
//                columnDefinition.Width = new System.Windows.GridLength(RequiredWidth);
//                OnPropertyChanged();
//            }
//        }

//        public ColumnDefinition ColumnDefinition
//        {
//            get => columnDefinition; 
//            set
//            {
//                columnDefinition = value;
//                columnDefinition.Width = new System.Windows.GridLength(RequiredWidth);
//            }
//        }

//        public HideableGridPanelWithinGrid(DatatableEditor_Model parent, double startingWidth, bool startingVisibility)
//        {
//            CachedWidth = startingWidth;
//            columnIsVisible = startingVisibility;
//        }


//    }
//}
