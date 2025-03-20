using System.Globalization;
using MaCamp.Services;

namespace MaCamp.Utils.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string url)
            {
                return CampingServices.MontarUrlImagemTemporaria(url);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
