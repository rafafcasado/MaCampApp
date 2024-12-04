namespace MaCamp.AppSettings
{
    /// <summary>
    ///     Classe de configuração com a "paleta de cores" do aplicativo
    /// </summary>
    public class AppColors
    {
        // *** As cores PRIMARIA, PRIMARIA ESCURA e DESTAQUE devem ser definidas também no projeto .Android/Resources/values/colors ***
        public static Color CorPrimaria => Color.FromArgb("#2ab079");
        public static Color CorPrimariaEscura => Color.FromArgb("#19714d");
        public static Color CorSecundaria => Color.FromArgb("#e99a1e");
        public static Color CorSecundariaEscura => Color.FromArgb("#bd7f1d");
        public static Color CorDestaque => Color.FromArgb("#f39d17");
        public static Color CorTextoSobreCorPrimaria => Color.FromArgb("#FFFFFF");
    }
}