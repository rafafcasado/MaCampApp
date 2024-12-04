using System.Globalization;

namespace MaCamp.Dependencias
{
    public interface ILocalize
    {
        CultureInfo ObterCultureInfoDoUsuario();
    }
}