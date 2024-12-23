using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MenuOrder
{
    public class VisibilityConverter : IValueConverter
    {
        public string TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == TrueValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
