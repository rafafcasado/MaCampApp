using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Views.CustomCells;
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
            lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
            lvItens.RefreshCommand = new Command((obj) => RecarregarConteudo(null, null));
            Task.Run(() => CarregarConteudo());

            MessagingCenter.Subscribe<Application>(this, AppConstants.MensagemAtualizarListagemFavoritos, (s) =>
            {
                RecarregarConteudo(null, null);
            });
        }

        private async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Item item)
            {
                lvItens.SelectedItem = null;

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

        private void RecarregarConteudo(object? sender, EventArgs? e)
        {
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = true;
            }

            lvItens.ItemsSource = null;
            lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
            CarregarConteudo();
        }

        private void CarregarConteudo()
        {
            var itensFavoritos = DBContract.NewInstance().ListarItens(i => i.Favoritado);

            Dispatcher.Dispatch(() =>
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;

                if (itensFavoritos.Count > 0)
                {
                    lbMensagemAviso.IsVisible = false;
                    lvItens.ItemsSource = itensFavoritos;
                    lvItens.IsRefreshing = false;
                    lvItens.IsVisible = true;
                }
                else
                {
                    lbMensagemAviso.Text = "Favorite itens para que eles sejam exibidos aqui";
                    lbMensagemAviso.IsVisible = true;
                    lvItens.IsVisible = false;
                }
            });
        }
    }
}