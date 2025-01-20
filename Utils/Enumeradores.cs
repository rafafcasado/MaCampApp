namespace MaCamp.Utils
{
    public static class Enumeradores
    {
        public enum TipoChave
        {
            ChaveUsuario,
            ControleInterno,
            Cor
        }

        public enum TipoAnuncio
        {
            Banner = 0,
            Nativo = 1,
            Popup = 2
        }

        public enum TipoListagem
        {
            Noticias,
            Camping
        }

        public enum TipoMedia
        {
            Nenhum,
            Telefone,
            WhatsApp,
            Email,
            Endereco,
            URL,
            Mapa
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
            AtualizarCampings
        }
    }
}
