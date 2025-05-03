using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Resources.Locale;
using MaCamp.Utils;
using MaCamp.Views.Campings;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Listagens
{
    public partial class ListagemItensPage : ContentPage
    {
        /// <summary>
        /// Carrega a Listagem de itens Favoritos do usuário
        /// </summary>
        public ListagemItensPage()
        {
            InitializeComponent();

            Title = AppLanguage.Titulo_Favoritos;

            Appearing += (sender, e) =>
            {
                //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de itens");
            };

            NavigationPage.SetBackButtonTitle(this, AppLanguage.Texto_Voltar);
            cvListagemItens.Content = new ListagemItensFavoritosView();
        }

        /// <summary>
        /// Carrega uma Listagem de itens OnLine
        /// </summary>
        /// <param name="endpoint">URL de onde será carregado o JSON de Itens</param>
        /// <param name="nome">Nome da listagem, que será exibido no Título da Página e no Google Analytics</param>
        public ListagemItensPage(string endpoint, string? nome, TipoListagem tipoListagem = TipoListagem.Noticias, string? tag = null)
        {
            InitializeComponent();

            Title = nome;

            Appearing += (sender, e) =>
            {
                //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem: " + nome);
            };

            NavigationPage.SetBackButtonTitle(this, AppLanguage.Texto_Voltar);

            if (/*System.Diagnostics.Debugger.IsAttached ||*/ tipoListagem == TipoListagem.Camping)
            {
                cvListagemItens.Content = new ListagemCampingsView(endpoint);

                WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_AtualizarListagemCampings);

                WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_AtualizarListagemCampings, (recipient, message) =>
                {
                    cvListagemItens.Content = new ListagemCampingsView(endpoint);
                });
            }
            else
            {
                cvListagemItens.Content = new ListagemItensOnlineView(endpoint, tag);
            }
        }
    }
}