using System;
using System.Globalization;
using System.Windows.Data;

namespace CalculatorWPF.Converters
{
    public class TabIndexToBoolConverter : IValueConverter
    {
        public static readonly TabIndexToBoolConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentIndex && parameter != null && int.TryParse(parameter.ToString(), out int targetIndex))
            {
                return currentIndex == targetIndex;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter != null && int.TryParse(parameter.ToString(), out int targetIndex))
            {
                return targetIndex;
            }
            return Binding.DoNothing;
        }
    }
}
