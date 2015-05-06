using System;
using System.Windows;
using System.Windows.Data;

namespace VeniceTide.Code.Utils
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            bool visibility = ( bool )value;
            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            Visibility visibility = ( Visibility )value;
            return visibility == Visibility.Visible ? true : false;
        }
    }
}
