using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CalculatorWPF.Converters
{
    public class TabIndexToVisibilityConverter : IValueConverter
    {
        public static readonly TabIndexToVisibilityConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentIndex && parameter != null && int.TryParse(parameter.ToString(), out int targetIndex))
            {
                return currentIndex == targetIndex ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
