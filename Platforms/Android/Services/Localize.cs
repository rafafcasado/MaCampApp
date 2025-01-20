using System.Globalization;
using MaCamp.Dependencias;
using Locale = Java.Util.Locale;

namespace MaCamp.Platforms.Android.Services
{
    public class Localize : ILocalize
    {
        public CultureInfo PegarCultureInfoUsuario()
        {
            var androidLocale = Locale.Default;
            var netLanguage = androidLocale.ToString()?.Replace("_", "-"); // turns pt_BR into pt-BR

            if (netLanguage != null)
            {
                return new CultureInfo(netLanguage);
            }

            throw new ArgumentNullException(nameof(netLanguage));
        }
    }
}