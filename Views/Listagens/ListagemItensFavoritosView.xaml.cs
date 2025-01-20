using MaCamp.Utils;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;

namespace MaCamp.Views.Listagens
{
    public partial class ListagemItensFavoritosView : ContentView
    {
        public ListagemItensFavoritosView()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Campings Favoritos");

            cvItens.ItemTemplate = new DataTemplate(typeof(CampingContentView));

            Task.Run(() => CarregarConteudo());

            MessagingCenter.Subscribe<Application>(this, AppConstants.MessagingCenter_AtualizarListagemFavoritos, s =>
            {
                Handle_Refreshing(null, null);
            });
        }

        private async void Handle_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Item item)
            {
                cvItens.SelectedItem = null;

                if (item.DeveAbrirExternamente && item.UrlExterna != null)
                {
                    await Launcher.OpenAsync(item.UrlExterna);
                }
                else
                {
                    await Navigation.PushAsync(new DetalhesCampingPage(item));
                }
            }
        }

        private void Handle_Refreshing(object? sender, EventArgs? e)
        {
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = true;
                loaderConteudoInicial.IsRunning = true;
            }

            cvItens.ItemsSource = null;

            CarregarConteudo();
        }

        private void CarregarConteudo()
        {
            var itensFavoritos = DBContract.Instance.ListarItens(i => i.Favoritado);

            Dispatcher.Dispatch(() =>
            {
                loaderConteudoInicial.IsVisible = false;
                loaderConteudoInicial.IsRunning = false;
                loaderConteudoAdicional.IsVisible = false;

                if (itensFavoritos.Count > 0)
                {
                    lbMensagemAviso.IsVisible = false;
                    cvItens.ItemsSource = itensFavoritos;
                    rvItens.IsRefreshing = false;
                    cvItens.IsVisible = true;
                }
                else
                {
                    lbMensagemAviso.Text = "Favorite itens para que eles sejam exibidos aqui";
                    lbMensagemAviso.IsVisible = true;
                    cvItens.IsVisible = false;
                }
            });
        }
    }
}