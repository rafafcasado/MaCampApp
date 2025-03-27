using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Dependencias;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;

namespace MaCamp.Views.Listagens
{
    public partial class ListagemItensFavoritosView : ContentView
    {
        public ListagemItensFavoritosView()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Campings Favoritos");

            cvItens.ItemTemplate = new ItemDataTemplateSelector();

            Loaded += ListagemItensFavoritosView_Loaded;

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_AtualizarListagemFavoritos, (recipient, message) =>
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
                    if (item.IdCamping != 0)
                    {
                        await Navigation.PushAsync(new DetalhesCampingPage(item));
                    }
                    else if (item.IdPost != 0)
                    {
                        await ControladorDeAnuncios.VerificarEExibirAnuncioPopup();
                        await Navigation.PushAsync(new DetalhesPage(item));
                    }
                    else
                    {
                        throw new Exception("Item não foi tratado");
                    }
                }
            }
        }

        private async void Handle_Refreshing(object? sender, EventArgs? e)
        {
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = true;
                loaderConteudoInicial.IsRunning = true;
            }

            cvItens.ItemsSource = null;

            await CarregarConteudoAsync();
        }

        private async Task CarregarConteudoAsync()
        {
            var storagePermissionService = await Workaround.GetServiceAsync<IStoragePermission>();
            var storagePermissionResult = await storagePermissionService.Request();

            if (storagePermissionResult)
            {
                var itensFavoritos = StorageHelper.LoadData<List<Item>>(AppConstants.FavoritesFilename);

                loaderConteudoInicial.IsVisible = false;
                loaderConteudoInicial.IsRunning = false;
                loaderConteudoAdicional.IsVisible = false;

                if (itensFavoritos != null && itensFavoritos.Count > 0)
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
            }
        }

        private async void ListagemItensFavoritosView_Loaded(object? sender, EventArgs e)
        {
            if (cvItens.ItemsSource is not List<Item> listaFavoritos || listaFavoritos.Count == 0)
            {
                await CarregarConteudoAsync();
            }
        }
    }
}