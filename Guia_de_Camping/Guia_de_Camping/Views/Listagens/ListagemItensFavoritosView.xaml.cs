using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Views.CustomCells;
using Aspbrasil.Views.Detalhes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListagemItensFavoritosView : ContentView
    {
        public ListagemItensFavoritosView()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Campings Favoritos");

            lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
            lvItens.RefreshCommand = new Command((obj) => RecarregarConteudo(null, null));

            Task.Run(() => CarregarConteudo());

            MessagingCenter.Subscribe<Application>(this, AppConstants.MENSAGEM_ATUALIZAR_LISTAGEM_FAVORITOS, (s) => { RecarregarConteudo(null, null); });
        }

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Item;
            if (item != null)
            {
                lvItens.SelectedItem = null;
                if (item.DeveAbrirExternamente)
                {
                    Device.OpenUri(new System.Uri(item.UrlExterna));
                }
                else
                {
                    await Navigation.PushAsync(new DetalhesCampingPage(item));
                }
            }
        }

        private void RecarregarConteudo(object sender, System.EventArgs e)
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
            List<Item> itensFavoritos = DBContract.NewInstance().ListarItens(i => i.Favoritado);
            Device.BeginInvokeOnMainThread(() =>
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