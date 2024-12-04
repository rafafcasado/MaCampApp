namespace MaCamp.AppSettings
{
    public class AppConstants
    {
        public static Page CurrentPage => Application.Current?.Windows[0].Page ?? throw new NullReferenceException();

        public static string CharsAcentuados = "αßÁáÀàÂâÃãĀāÅåÄäÆæÇçÉéÈèÊêÍíÌìÎîÑñÓóÒòÔôÖöŌōØøÚúÙùÜüŽž";
        public static string CharsRegulares = "abAaAaAaAaAaAaAaAaCcEeEeEeIiIiIiNnOoOoOoOoOoOoUuUuUuZz";

        public static string EmailRegex => @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";


        public static string AdmobIdBannerAndroid => System.Diagnostics.Debugger.IsAttached ? "ca-app-pub-8959365990645001/9280664891" : "ca-app-pub-3940256099942544/6300978111";
        public static string AdmobIdBannerIOs => System.Diagnostics.Debugger.IsAttached ? "ca-app-pub-8959365990645001/7917696236" : "ca-app-pub-3940256099942544/6300978111";

        public static string SqliteFilename => "app.db3";
        public static string NomeApp => "MaCamp";
        public static string ParametroTodasTags => "Todas";
        public static int QuantidadeNoticiasPorLote => 20;
        public static string OnesignalAppId => "5b60ade4-26c8-452b-86a8-039457585240";

        // URLs
        public static string UrlAnuncios => "https://guiadecampingsanuncios.homologacao.net/?altTemplate=anuncio";
        public static string UrlConfiguracoesAnuncios => "https://guiadecampingsanuncios.homologacao.net/?altTemplate=Configs_Anuncios";
        public static string UrlListaCampings => "https://guiadecampings.homologacao.net/api/Campings/GetCampings";
        public static string UrlListaIdentificadores => "https://guiadecampings.homologacao.net/api/Campings/GetIdentificadores";
        public static string UrlListaCidades => "https://guiadecampings.homologacao.net/api/Cidades/GetCidades";
        public static string UrlEnviarEmail => "https://guiadecampings.homologacao.net/API/Colaboracao/EnviarEmail";
        public static string UrlPegarPosts => "https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts";
        public static string UrlDominioTemporario => "https://macamptecnologia1.websiteseguro.com/";
        public static string UrlDominioOficial => "https://macamp.com.br/";
        public static string UrlTermoUso => "https://macamp.com.br/guia/termos-de-uso/";
        public static string UrlEasyTransports => "http://www.easytransport.com.br/";

        // Mensagens
        public static string MensagemAtualizarListagemCampings => "AtualizarListagemCampings";
        public static string MensagemAtualizarListagemFavoritos => "AtualizarListagemFavoritos";
        public static string MensagemAlternarModoNoturno => "AtualizarModoNoturno";
        public static string MensagemBuscaRealizada => "BUSCA_REALIZADA";
        public static string MensagemBuscarCampingsAtualizados => "BuscarCampingsAtualizados";
        public static string MensagemExibirBuscaCampings => "EXIBIR_BUSCA_CAMPINGS";

        // Chaves
        public static string ChaveTituloNotificacao => "TituloNotificacao";
        public static string ChaveMensagemNotificacao => "MensagemNotificacao";
        public static string ChaveUrlNotificacao => "URLNotificacao";
        public static string ChaveIdItemNotificacao => "IdItemNotificacao";
        public static string ChaveAppJaAvaliado => "AppJaAvaliado";
        public static string ChaveModoNoturnoAtivo => "ModoNoturnoAtivo";
        public static string ChaveRecarregarHtmlLogin => "RecarregarHTMLLogin";
        public static string ChaveDownloadCampingsCompleto => "DownloadCampingsCompleto";
        public static string ChaveDataUltimaAtualizacaoConteudo => "DataUltimaAtualizacaoConteudo";
    }
}