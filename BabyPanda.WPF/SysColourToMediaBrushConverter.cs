using System;
using System.Globalization;
using System.Windows.Data;

namespace BabyPanda.WPF
{
    //--------------------------------------------------------------------------------------------
    public class SysColourToMediaBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(System.Drawing.Color)) return new System.Windows.Media.SolidColorBrush();
            var val = (System.Drawing.Color)value;
            return ConvertToMediaBrush(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(System.Windows.Media.SolidColorBrush)) return System.Drawing.Color.Transparent;
            var val = (System.Windows.Media.SolidColorBrush)value;
            return ConvertToDrawingColor(val);
        }

        public static System.Windows.Media.SolidColorBrush ConvertToMediaBrush(System.Drawing.Color sysColour)
        {
            var medColor = System.Windows.Media.Color.FromArgb(sysColour.A, sysColour.R, sysColour.G, sysColour.B);
            return new System.Windows.Media.SolidColorBrush(medColor);
        }

        public static System.Drawing.Color ConvertToDrawingColor(System.Windows.Media.SolidColorBrush medBrush)
        {
            System.Windows.Media.Color drgColor = medBrush.Color;
            return System.Drawing.Color.FromArgb(drgColor.A, drgColor.R, drgColor.G, drgColor.B);
        }
    }
    //--------------------------------------------------------------------------------------------
}
