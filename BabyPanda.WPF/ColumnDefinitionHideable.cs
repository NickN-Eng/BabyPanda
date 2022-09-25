//This class is originally by immortalus from CodeProject.
//https://www.codeproject.com/Articles/437237/WPF-Grid-Column-and-Row-Hiding
//It is licensed under The Code Project Open License (CPOL)
//https://www.codeproject.com/info/cpol10.aspx 

using System;
using System.Windows;
using System.Windows.Controls;

namespace BabyPanda.WPF
{
    public class ColumnDefinitionHideable : ColumnDefinition
    {
        // Variables
        public static DependencyProperty VisibleProperty;

        // Properties
        public Boolean Visible { get { return (Boolean)GetValue(VisibleProperty); } set { SetValue(VisibleProperty, value); } }

        // Constructors
        static ColumnDefinitionHideable()
        {
            VisibleProperty = DependencyProperty.Register("Visible", typeof(Boolean), typeof(ColumnDefinitionHideable), new PropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged)));
            ColumnDefinition.WidthProperty.OverrideMetadata(typeof(ColumnDefinitionHideable), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, new CoerceValueCallback(CoerceWidth)));
            ColumnDefinition.MinWidthProperty.OverrideMetadata(typeof(ColumnDefinitionHideable), new FrameworkPropertyMetadata((Double)0, null, new CoerceValueCallback(CoerceMinWidth)));
        }

        // Get/Set
        public static void SetVisible(DependencyObject obj, Boolean nVisible)
        {
            obj.SetValue(VisibleProperty, nVisible);
        }
        public static Boolean GetVisible(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(VisibleProperty);
        }

        static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(ColumnDefinition.WidthProperty);
            obj.CoerceValue(ColumnDefinition.MinWidthProperty);
        }
        static Object CoerceWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinitionHideable)obj).Visible) ? nValue : new GridLength(0);
        }
        static Object CoerceMinWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinitionHideable)obj).Visible) ? nValue : (Double)0;
        }
    }
}
