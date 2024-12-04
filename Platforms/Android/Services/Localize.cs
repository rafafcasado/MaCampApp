using System.Globalization;
using MaCamp.Dependencias;

namespace MaCamp.Platforms.Android.Services
{
    internal class Localize : ILocalize
    {
        public CultureInfo ObterCultureInfoDoUsuario()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString()?.Replace("_", "-"); // turns pt_BR into pt-BR

            if (netLanguage != null)
            {
                return new CultureInfo(netLanguage);
            }

            throw new ArgumentNullException(nameof(netLanguage));
        }
    }
}