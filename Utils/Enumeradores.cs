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
            AbrirClassificados,
            CadastreUmCamping,
            AbrirParceiros,
            AbrirDicasCampismo,
            AtualizarCampings
        }

        public enum TipoIdentificador
        {
            Destaque = 1,
            CampingEmFuncionamento = 2,
            ApoioRV = 3,
            CampingInformal = 4,
            CampingEmSituacaoIncerta = 5,
            CampingSelvagem = 6,
            CampingParaGruposOuEventos = 7,
            CampingEmReformas = 8,
            CampingFechadoOuSemFuncao = 9
        }

        public enum TipoListagemFotos
        {
            Masonary,
            Carousel
        }

        public enum TipoEnumValue
        {
            Biggest,
            Smallest
        }
    }
}
