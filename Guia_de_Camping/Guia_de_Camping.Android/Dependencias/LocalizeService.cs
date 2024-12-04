using Xamarin.Forms;

[assembly: Dependency(typeof(Aspbrasil.Droid.Dependencias.LocalizeService))]
namespace Aspbrasil.Droid.Dependencias
{
    class LocalizeService : Aspbrasil.Dependencias.ILocalize
    {
        public System.Globalization.CultureInfo ObterCultureInfoDoUsuario()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-"); // turns pt_BR into pt-BR
            return new System.Globalization.CultureInfo(netLanguage);
        }
    }
}