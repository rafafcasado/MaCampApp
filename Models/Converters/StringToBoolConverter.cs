using System.Globalization;

namespace MaCamp.Models.Converters
{
    internal class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string valueConverted && !string.IsNullOrWhiteSpace(valueConverted);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool ? "-" : "";
        }
    }
}