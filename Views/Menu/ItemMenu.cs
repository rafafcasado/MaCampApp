using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Menu
{
    public class ItemMenu
    {
        public string? Titulo { get; set; }
        public string? TituloPagina { get; set; }
        public string? TituloListagem { get; set; }
        public string? IconSource { get; set; }
        public int IdExterno { get; set; }
        public TipoLayoutMenu TipoLayout { get; set; }
        public TipoAcaoMenu? TipoAcao { get; set; }
        public bool ExibirIcone { get; set; } = false;
        public string? ValorComportamento { get; set; }
        public string HexCorTexto { get; set; }
        public Type TargetType { get; set; }

        public ItemMenu()
        {
            HexCorTexto = "#000000";
            TargetType = typeof(MainPage);
        }
    }
}