using System.Globalization;

namespace MaCamp.Utils.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string valueConverted && !string.IsNullOrEmpty(valueConverted);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool ? "-" : string.Empty;
        }
    }
}