using Aspbrasil.Views;
using System;

namespace Aspbrasil
{
    public class ItemMenu
    {
        public string Titulo { get; set; }
        public string TituloPagina { get; set; }
        public string TituloListagem { get; set; }
        public string IconSource { get; set; }
        public int IdExterno { get; set; }
        public TipoLayoutMenu TipoLayout { get; set; }
        public TipoAcaoMenu? TipoAcao { get; set; }
        public bool ExibirIcone { get; set; } = false;
        public string ValorComportamento { get; set; }
        public string HexCorTexto { get; set; } = "#000000";
        public Type TargetType { get; set; } = typeof(MainPage);
    }

    public enum TipoLayoutMenu
    {
        Divisoria,
        Item,
        SubItem
    }

    public enum TipoAcaoMenu
    {
        Home,
        AbrirBuscaCamping,
        AbrirMapa,
        Item,
        AtualizarConteudo,
        AbrirSobreAEmpresa,
        Configuracoes,
        AbrirURI,
        Sair,
        Favoritos,
        Nenhuma,
        NaoImplementadoNessaVersao,
        AbrirNoticias,
        AbrirEventos,
        CadastreUmCamping,
        AbrirParceiros,
        AbrirDicasCampismo,
        AtualizarCampings,
    }
}
