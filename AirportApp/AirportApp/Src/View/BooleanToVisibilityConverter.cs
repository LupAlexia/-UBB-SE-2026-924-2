using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace AirportApp.Src.View
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool isVisible && isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility isVisible && isVisible == Visibility.Visible;
        }
    }
}