using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BabyPanda.WPF
{
    public class IsShownToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                if (b) return Visibility.Visible;
                return Visibility.Collapsed;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Visibility v)
            {
                switch (v)
                {
                    case Visibility.Visible:
                        return true;
                    case Visibility.Hidden:
                    case Visibility.Collapsed:
                    default:
                        return false;
                }
            }
            return false;
        }
    }

    public class IsHiddenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b) return Visibility.Collapsed;
                return Visibility.Visible;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                switch (v)
                {
                    case Visibility.Visible:
                        return false;
                    case Visibility.Hidden:
                    case Visibility.Collapsed:
                    default:
                        return true;
                }

            }
            return false;
        }
    }
}
