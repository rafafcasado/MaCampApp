using System.Diagnostics;

namespace MaCamp.Utils
{
    public static class AppConstants
    {
        public static string Path => Workaround.GetPath();
        public static Dictionary<string, object?> DictionaryData => new Dictionary<string, object?>();
        public static Page CurrentPage => Application.Current?.MainPage ?? throw new NullReferenceException();

        public static string CharsAcentuados => "αßÁáÀàÂâÃãĀāÅåÄäÆæÇçÉéÈèÊêÍíÌìÎîÑñÓóÒòÔôÖöŌōØøÚúÙùÜüŽž";
        public static string CharsRegulares => "abAaAaAaAaAaAaAaAaCcEeEeEeIiIiIiNnOoOoOoOoOoOoUuUuUuZz";

        public static string EmailRegex => @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public static string AdmobIdBannerAndroid => Debugger.IsAttached ? "ca-app-pub-8959365990645001/9280664891" : "ca-app-pub-3940256099942544/6300978111";
        public static string AdmobIdBannerIOs => Debugger.IsAttached ? "ca-app-pub-8959365990645001/7917696236" : "ca-app-pub-3940256099942544/6300978111";

        public static string FavoritesFilename => "favoritos.json";
        public static string SqliteFilename => "app.db3";
        public static string SqliteBackupFilename => "backup.db3";
        public static string SqliteTemporaryFilename => "temp.db3";
        public static string NomeApp => "MaCamp";
        public static string ParametroTodasTags => "Todas";
        public static int QuantidadeNoticiasPorLote => 20;

        // OneSignal
        public static string OneSignal_AppId => "5b60ade4-26c8-452b-86a8-039457585240";

        // GAService
        public static string GAService_TrackingId = "UA-49846110-44";
        public static string GAService_TrackerName = "Guia de Camping";
        public static string GAService_AllowTrackingKey => "AllowTracking";

        // GoogleAnalytics
        public static string GoogleAnalytics_TrackerName => "Aspbrasil";
        public static string GoogleAnalytics_TrackingId => Debugger.IsAttached ? "UA-49846110-44" : "UA-49846110-36";

        // Titulos
        public static string Titulo_SemInternet => "Este conteúdo requer conexão com a internet";

        // Descrições
        public static string Descricao_SemInternet => "Verifique sua conexão e/ou tente novamente mais tarde";

        // URLs
        public static string Url_Anuncios => "https://guiadecampingsanuncios.homologacao.net/?altTemplate=anuncio";
        public static string Url_ConfiguracoesAnuncios => "https://guiadecampingsanuncios.homologacao.net/?altTemplate=Configs_Anuncios";
        public static string Url_ListaCampings => "https://guiadecampings.homologacao.net/api/Campings/GetCampings";
        public static string Url_ListaIdentificadores => "https://guiadecampings.homologacao.net/api/Campings/GetIdentificadores";
        public static string Url_ListaCidades => "https://guiadecampings.homologacao.net/api/Cidades/GetCidades";
        public static string Url_EnviarEmail => "https://guiadecampings.homologacao.net/API/Colaboracao/EnviarEmail";
        public static string Url_PegarPosts => "https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts";
        public static string Url_DominioTemporario => "https://macamptecnologia1.websiteseguro.com/";
        public static string Url_DominioOficial => "https://macamp.com.br/";
        public static string Url_TermoUso => "https://macamp.com.br/guia/termos-de-uso/";
        public static string Url_EasyTransports => "http://www.easytransport.com.br/";

        // WeakReferenceMessenger
        public static string WeakReferenceMessenger_AtualizarListagemCampings => "AtualizarListagemCampings";
        public static string WeakReferenceMessenger_AtualizarListagemFavoritos => "AtualizarListagemFavoritos";
        public static string WeakReferenceMessenger_AlternarModoNoturno => "AtualizarModoNoturno";
        public static string WeakReferenceMessenger_BuscaRealizada => "BUSCA_REALIZADA";
        public static string WeakReferenceMessenger_BuscarCampingsAtualizados => "BuscarCampingsAtualizados";
        public static string WeakReferenceMessenger_ExibirBuscaCampings => "EXIBIR_BUSCA_CAMPINGS";
        public static string WeakReferenceMessenger_AtualizarProgressoWebView => "ATUALIZAR_PROGRESSO_WEBVIEW";

        // Chaves
        public static string Chave_TituloNotificacao => "TituloNotificacao";
        public static string Chave_MensagemNotificacao => "MensagemNotificacao";
        public static string Chave_UrlNotificacao => "URLNotificacao";
        public static string Chave_IdItemNotificacao => "IdItemNotificacao";
        public static string Chave_AppJaAvaliado => "AppJaAvaliado";
        public static string Chave_ModoNoturnoAtivo => "ModoNoturnoAtivo";
        public static string Chave_RecarregarHtmlLogin => "RecarregarHTMLLogin";
        public static string Chave_DownloadCampingsCompleto => "DownloadCampingsCompleto";
        public static string Chave_DataUltimaAtualizacaoConteudo => "DataUltimaAtualizacaoConteudo";
        public static string Chave_UltimaAtualizacao => "UltimaAtualizacao";

        // Filtros
        public static string Filtro_NomeCamping => "FILTROS_NOME_DO_CAMPING";
        public static string Filtro_EstadoSelecionado => "FILTROS_ESTADO_SELECIONADO";
        public static string Filtro_CidadeSelecionada => "FILTROS_CIDADE_SELECIONADA";
        public static string Filtro_LocalizacaoSelecionada => "FILTROS_LOCALIZACAO_SELECIONADA";
        public static string Filtro_ServicoSelecionados => "FILTROS_SERVICO_SELECIONADOS";
        public static string Filtro_EstabelecimentoSelecionados => "FILTROS_ESTABELECIMENTO_SELECIONADOS";

        public static string Busca_InicialRealizada => "BUSCA_INICIAL_REALIZADA";
        public static string Quantidade_AberturasDetalhes => "QTD_ABERTURAS_DETALHES";
        public static string Atualizar_ProgressoWebView => "ATUALIZAR_PROGRESSO_WEBVIEW";

        // NotificationManagerService
        public static string NotificationManager_ChannelId => "default";
        public static string NotificationManager_ChannelName => "Atualizações";
        public static string NotificationManager_ChannelDescription => "Usado para atualização do banco de dados.";

        public static string NotificationManager_Id => "NotificationManager_Id";
        public static string NotificationManager_Data => "NotificationManager_Data";

        static AppConstants()
        {
            try
            {
                var appPath = Workaround.GetPath();

                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppConstants), "constructor", ex);
            }
        }
    }
}