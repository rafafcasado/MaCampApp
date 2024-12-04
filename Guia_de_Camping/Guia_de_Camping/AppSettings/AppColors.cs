using Xamarin.Forms;

namespace Aspbrasil.AppSettings
{
    /// <summary>
    ///     Classe de configuração com a "paleta de cores" do aplicativo
    /// </summary>
    public class AppColors
    {
        // *** As cores PRIMARIA, PRIMARIA ESCURA e DESTAQUE devem ser definidas também no projeto .Android/Resources/values/colors ***
        public static readonly Color COR_PRIMARIA = Color.FromHex("#2ab079");
        public static readonly Color COR_PRIMARIA_ESCURA = Color.FromHex("#19714d");

        public static readonly Color COR_SECUNDARIA = Color.FromHex("#e99a1e");
        public static readonly Color COR_SECUNDARIA_ESCURA = Color.FromHex("#bd7f1d");

        public static readonly Color COR_DESTAQUE = Color.FromHex("#f39d17");

        public static readonly Color COR_TEXTO_SOBRE_COR_PRIMARIA = Color.FromHex("#FFFFFF");
    }
}