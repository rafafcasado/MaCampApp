using System.Text;
using System.Text.RegularExpressions;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Models.Services;

namespace MaCamp.Views.Detalhes
{
    public partial class DetalhesCampingPage : ContentPage
    {
        private Item ItemAtual { get; }

        public DetalhesCampingPage(Item item)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            Title = AppConstants.NomeApp;
            ItemAtual = item;
            BindingContext = ItemAtual;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Detalhes - " + item.Nome);

            if (!string.IsNullOrWhiteSpace(item.LinkUltimaFoto))
            {
                imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

                //imgFotoItem.Source = item.LinkUltimaFoto;
                imgFotoItem.Source = CampingServices.MontarUrlImagemTemporaria(item.LinkUltimaFoto);

                //imgFotoItem.Source = "https://img.freepik.com/fotos-premium/cachorro-andando-na-rua_41691-381.jpg?w=1380";

                if (!string.IsNullOrWhiteSpace(item.LinksFotos))
                {
                    //O id do item será obtido no clique através do ClassID do Grid
                    /*grdFotoPrincipal.ClassId = item.LinksFotos;*/
                    //imIconeGaleria.Source = ImageSource.FromResource("Keer.Resources.icone_galeria.jpg");
                    imIconeGaleria.Source = "icone_galeria.png";
                    slMaisFotos.IsVisible = true;
                    var tapGesture = new TapGestureRecognizer();

                    tapGesture.Tapped += async delegate
                    {
                        await Navigation.PushAsync(new ListagemFotosPage(item.LinksFotos.Split('|')));
                    };

                    grdFotoPrincipal.GestureRecognizers.Add(tapGesture);
                }
            }
            else
            {
                grdFotoPrincipal.IsVisible = false;
            }

            lbTitulo.Text = item.Nome?.ToUpper() ?? string.Empty;

            Task.Delay(500).ContinueWith(r =>
            {
                if (item.Descricao != null)
                {
                    var descricao = Encoding.UTF8.GetString(Convert.FromBase64String(item.Descricao));

                    Dispatcher.Dispatch(() =>
                    {
                        //lbDescricao.Text = descricao;
                        lbDescricao.Text = descricao.Replace("\r\n", "<br/>");
                        cvTipo.Content = new TipoEstabelecimentoView(item.Identificadores);
                        cvComodidades.Content = new ComodidadesView(item.Identificadores);
                    });
                }
            });

            ConfigurarToolbar(item);

            var abrirMapa = new TapGestureRecognizer();
            var abrirSite = new TapGestureRecognizer();
            var abrirPreco = new TapGestureRecognizer();
            var abrirFacebook = new TapGestureRecognizer();
            var abrirInstragram = new TapGestureRecognizer();
            var abrirYoutube = new TapGestureRecognizer();
            var abrirEmail = new TapGestureRecognizer();
            var abrirTelefone = new TapGestureRecognizer();
            var abrirTelefone2 = new TapGestureRecognizer();
            var abrirTelefone3 = new TapGestureRecognizer();
            var abrirTelefone4 = new TapGestureRecognizer();

            if (item.Latitude == 0 && item.Longitude == 0)
            {
                slCoordenadas.IsVisible = false;

                abrirMapa.Tapped += async (s, e) =>
                {
                    await AbrirEndereco(s, null);
                };
            }
            else
            {
                abrirMapa.Tapped += async (s, e) =>
                {
                    await AbrirLatitudeLongitude(s, null);
                };
            }

            abrirSite.Tapped += (s, e) => AbrirURI2(s, ItemAtual.Site);
            abrirPreco.Tapped += (s, e) => AbrirURI2(s, ItemAtual.LinkPrecos);
            abrirFacebook.Tapped += (s, e) => AbrirURI2(s, ItemAtual.Facebook);
            abrirInstragram.Tapped += (s, e) => AbrirURI2(s, ItemAtual.Instagram);
            abrirYoutube.Tapped += (s, e) => AbrirURI2(s, ItemAtual.Youtube);
            abrirEmail.Tapped += (s, e) => AbrirMail2(s, ItemAtual.Email);
            abrirTelefone.Tapped += (s, e) => AbrirTelefone2(s, ItemAtual.Telefone);
            abrirTelefone2.Tapped += (s, e) => AbrirTelefone2(s, ItemAtual.Telefone2);
            abrirTelefone3.Tapped += (s, e) => AbrirTelefone2(s, ItemAtual.Telefone3);
            abrirTelefone4.Tapped += (s, e) => AbrirTelefone2(s, ItemAtual.Telefone4);

            slEndereco.GestureRecognizers.Add(abrirMapa);
            slSite.GestureRecognizers.Add(abrirSite);
            slPreco.GestureRecognizers.Add(abrirPreco);
            slFacebook.GestureRecognizers.Add(abrirFacebook);
            slInstagram.GestureRecognizers.Add(abrirInstragram);
            slYoutube.GestureRecognizers.Add(abrirYoutube);
            slEmail.GestureRecognizers.Add(abrirEmail);
            slTelefone1.GestureRecognizers.Add(abrirTelefone);
            slTelefone2.GestureRecognizers.Add(abrirTelefone2);
            slTelefone3.GestureRecognizers.Add(abrirTelefone3);
            slTelefone4.GestureRecognizers.Add(abrirTelefone4);
        }

        private void ConfigurarToolbar(Item item)
        {
            ToolbarItems.Clear();

            if (!item.Favoritado)
            {
                var imagem = "icone_favoritos_off.png";

                ToolbarItems.Add(new ToolbarItem("Favoritar", imagem, () =>
                {
                    item.Favoritado = true;

                    DBContract.Instance.InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_AtualizarListagemFavoritos);
                }));
            }
            else
            {
                var imagem = "icone_favoritos_on.png";

                ToolbarItems.Add(new ToolbarItem("Remover Favorito", imagem, () =>
                {
                    item.Favoritado = false;

                    DBContract.Instance.InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_AtualizarListagemFavoritos);
                }));
            }

            ToolbarItems.Add(new ToolbarItem("Compartilhar", "share.png", async () =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = ItemAtual.Nome,
                    Uri = ItemAtual.UrlExterna
                });
            }));
        }

        //async void OnMaisFotosTapped(object sender, EventArgs e)
        //{
        //    string url = (sender as Grid).ClassId;
        //    await Navigation.PushModalAsync(new VisualizacaoFotoPage(url));
        //}

        public static async Task AbrirMapa(string uriArquivoKml)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                try
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                    if (status != PermissionStatus.Granted)
                    {
                        var shouldShowRationale = Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();

                        if (shouldShowRationale)
                        {
                            await AppConstants.CurrentPage.DisplayAlert("Localização necessária", "A permissão de localização será necessária para exibir o mapa", "OK");
                        }

                        var requestedStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                        if (requestedStatus == PermissionStatus.Granted)
                        {
                            //await (AppConstants.CurrentPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
                        }
                        else if (requestedStatus == PermissionStatus.Unknown)
                        {
                            await AppConstants.CurrentPage.DisplayAlert("Localização negada", "Não é possível continuar, tente novamente.", "OK");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                //await (AppConstants.CurrentPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
            }
        }

        private async Task AbrirEndereco(object? sender, TappedEventArgs? e)
        {
            if (ItemAtual.Latitude != null && ItemAtual.Longitude != null)
            {
                var enderecoCompleto = Uri.EscapeDataString($"{ItemAtual.Endereco}, {ItemAtual.Cidade}, {ItemAtual.Estado}, {ItemAtual.Pais}");
                var url = $"https://www.google.com/maps/search/?api=1&query={enderecoCompleto}";

                await Launcher.OpenAsync(new Uri(url));
            }
        }

        private async Task AbrirLatitudeLongitude(object? sender, TappedEventArgs? e)
        {
            if (ItemAtual.Latitude != null && ItemAtual.Longitude != null)
            {
                await Map.OpenAsync(ItemAtual.Latitude.Value, ItemAtual.Longitude.Value, new MapLaunchOptions
                {
                    Name = ItemAtual.Nome
                });
            }
        }

        //private async void AbrirURI(object sender, TappedEventArgs e)
        //{
        //    if (e.Parameter is string parameter)
        //    {
        //        await Launcher.TryOpenAsync(parameter);
        //    }
        //}

        private async void AbrirURI2(object? sender, string? url)
        {
            if (url != null)
            {
                await Launcher.TryOpenAsync(url);
            }
        }

        private async void AbrirMail2(object? sender, string? email)
        {
            await Launcher.TryOpenAsync("mailto:" + email);
        }

        //private async void AbrirTelefone(object? sender, TappedEventArgs e)
        //{
        //    await Launcher.TryOpenAsync("tel:" + e.Parameter?.ToString()?.Replace(".", "").Replace("(", "").Replace(")", ""));
        //}

        private async void AbrirTelefone2(object? sender, string? telefone)
        {
            if (telefone != null)
            {
                await Launcher.TryOpenAsync("tel:" + Regex.Replace(telefone, @"[.\(\)\s]", ""));
            }
        }

        private async void AbrirTelaColaboracao(object sender, EventArgs? e)
        {
            if (ItemAtual.Nome != null)
            {
                await Navigation.PushAsync(new FormularioColaboracaoCampingPage(ItemAtual.Nome, ItemAtual.IdCamping));
            }
        }

        private async void CopiarCoordenadas(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(ItemAtual.LatitudeLongitude);
            await DisplayAlert("Coordenadas Copiadas", "Basta colar essa informação no seu aplicativo de mapas favorito.", "Ok");
        }

        protected async void GoToSO(object sender, EventArgs e)
        {
            await Launcher.TryOpenAsync(AppConstants.Url_TermoUso);
        }
    }
}