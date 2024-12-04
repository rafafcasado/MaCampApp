using System;
using System.Globalization;
using Xamarin.Forms;

namespace Aspbrasil.Models.Converters
{
    class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "-";
            }
            else
            {
                return "";
            }
        }
    }
}
