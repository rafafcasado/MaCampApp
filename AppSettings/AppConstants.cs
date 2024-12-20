﻿using System.Diagnostics;

namespace MaCamp.AppSettings
{
    public static class AppConstants
    {
        public static Page CurrentPage => Application.Current?.Windows[0].Page ?? throw new NullReferenceException();

        public static string CharsAcentuados => "αßÁáÀàÂâÃãĀāÅåÄäÆæÇçÉéÈèÊêÍíÌìÎîÑñÓóÒòÔôÖöŌōØøÚúÙùÜüŽž";
        public static string CharsRegulares => "abAaAaAaAaAaAaAaAaCcEeEeEeIiIiIiNnOoOoOoOoOoOoUuUuUuZz";

        public static string EmailRegex => @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";


        public static string AdmobIdBannerAndroid => Debugger.IsAttached ? "ca-app-pub-8959365990645001/9280664891" : "ca-app-pub-3940256099942544/6300978111";
        public static string AdmobIdBannerIOs => Debugger.IsAttached ? "ca-app-pub-8959365990645001/7917696236" : "ca-app-pub-3940256099942544/6300978111";

        public static string SqliteFilename => "app.db3";
        public static string NomeApp => "MaCamp";
        public static string ParametroTodasTags => "Todas";
        public static int QuantidadeNoticiasPorLote => 20;
        public static string OnesignalAppId => "5b60ade4-26c8-452b-86a8-039457585240";

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

        // MessagingCenter
        public static string MessagingCenter_AtualizarListagemCampings => "AtualizarListagemCampings";
        public static string MessagingCenter_AtualizarListagemFavoritos => "AtualizarListagemFavoritos";
        public static string MessagingCenter_AlternarModoNoturno => "AtualizarModoNoturno";
        public static string MessagingCenter_BuscaRealizada => "BUSCA_REALIZADA";
        public static string MessagingCenter_BuscarCampingsAtualizados => "BuscarCampingsAtualizados";
        public static string MessagingCenter_ExibirBuscaCampings => "EXIBIR_BUSCA_CAMPINGS";

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
    }
}