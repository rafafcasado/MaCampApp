using System.Globalization;
using Foundation;
using MaCamp.Dependencias;

namespace MaCamp.Platforms.iOS.Services
{
    public class Localize : ILocalize
    {
        public CultureInfo ObterCultureInfoDoUsuario()
        {
            var netLanguage = "en";
            var prefLanguageOnly = "en";

            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                prefLanguageOnly = pref.Substring(0, 2);

                if (prefLanguageOnly == "pt")
                {
                    if (pref == "pt")
                    {
                        pref = "pt-BR"; // get the correct Brazilian language strings from the PCL RESX (note the local iOS folder is still "pt")
                    }
                }

                netLanguage = pref.Replace("_", "-");
                Console.WriteLine($@"preferred language: {netLanguage}");
            }

            try
            {
                var cultureInfo = new CultureInfo(netLanguage);

                return cultureInfo;
            }
            catch
            {
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
                // fallback to first characters, in this case "en"
                var cultureInfo = new CultureInfo(prefLanguageOnly);

                return cultureInfo;
            }
        }
    }
}