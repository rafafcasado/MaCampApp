using System.Globalization;

namespace Aspbrasil.Dependencias
{
    public interface ILocalize
    {
        CultureInfo ObterCultureInfoDoUsuario();
    }
}