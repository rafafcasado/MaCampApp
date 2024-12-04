using System.Text;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;

namespace MaCamp.Views.Detalhes
{
    public partial class DetalhesCampingPage : ContentPage
    {
        private Item ItemAtual { get; }

        public DetalhesCampingPage(Item item)
        {
            InitializeComponent();
            Title = AppConstants.NomeApp;
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            ItemAtual = item;
            BindingContext = ItemAtual;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Detalhes - " + item.Nome);
            if (!string.IsNullOrWhiteSpace(item.LinkUltimaFoto))
            {
                imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

                //imgFotoItem.Source = item.LinkUltimaFoto;
                imgFotoItem.Source = Models.Services.CampingServices.MontarUrlImagemTemporaria(item.LinkUltimaFoto);

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

            Task.Delay(500).ContinueWith((r) =>
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

            slEndereco.GestureRecognizers.Add(abrirMapa);
            var abrirSite = new TapGestureRecognizer();

            {
                abrirSite.Tapped += (s, e) =>
                {
                    AbrirURI2(s, ItemAtual.Site);
                };
            }

            slSite.GestureRecognizers.Add(abrirSite);
            var abrirPreco = new TapGestureRecognizer();

            {
                abrirPreco.Tapped += (s, e) =>
                {
                    AbrirURI2(s, ItemAtual.LinkPrecos);
                };
            }

            slPreco.GestureRecognizers.Add(abrirPreco);
            var abrirFacebook = new TapGestureRecognizer();

            {
                abrirFacebook.Tapped += (s, e) =>
                {
                    AbrirURI2(s, ItemAtual.Facebook);
                };
            }

            slFacebook.GestureRecognizers.Add(abrirFacebook);
            var abrirInstragram = new TapGestureRecognizer();

            {
                abrirInstragram.Tapped += (s, e) =>
                {
                    AbrirURI2(s, ItemAtual.Instagram);
                };
            }

            slInstagram.GestureRecognizers.Add(abrirInstragram);
            var abrirYoutube = new TapGestureRecognizer();

            {
                abrirYoutube.Tapped += (s, e) =>
                {
                    AbrirURI2(s, ItemAtual.Youtube);
                };
            }

            slYoutube.GestureRecognizers.Add(abrirYoutube);
            var abrirEmail = new TapGestureRecognizer();

            {
                abrirEmail.Tapped += (s, e) =>
                {
                    AbrirMail2(s, ItemAtual.Email);
                };
            }

            slEmail.GestureRecognizers.Add(abrirEmail);
            var abrirTelefone = new TapGestureRecognizer();

            {
                abrirTelefone.Tapped += (s, e) =>
                {
                    AbrirTelefone2(s, ItemAtual.Telefone);
                };
            }

            slTelefone1.GestureRecognizers.Add(abrirTelefone);
            var abrirTelefone2 = new TapGestureRecognizer();

            {
                abrirTelefone2.Tapped += (s, e) =>
                {
                    AbrirTelefone2(s, ItemAtual.Telefone2);
                };
            }

            slTelefone2.GestureRecognizers.Add(abrirTelefone2);
            var abrirTelefone3 = new TapGestureRecognizer();

            {
                abrirTelefone3.Tapped += (s, e) =>
                {
                    AbrirTelefone2(s, ItemAtual.Telefone3);
                };
            }

            slTelefone3.GestureRecognizers.Add(abrirTelefone3);
            var abrirTelefone4 = new TapGestureRecognizer();

            {
                abrirTelefone4.Tapped += (s, e) =>
                {
                    AbrirTelefone2(s, ItemAtual.Telefone4);
                };
            }

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

                    DBContract.NewInstance().InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    //MessagingCenter.Send(Application.Current, AppConstants.MensagemAtualizarListagemFavoritos);
                }));
            }
            else
            {
                var imagem = "icone_favoritos_on.png";

                ToolbarItems.Add(new ToolbarItem("Remover Favorito", imagem, () =>
                {
                    item.Favoritado = false;

                    DBContract.NewInstance().InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    //MessagingCenter.Send(Application.Current, AppConstants.MensagemAtualizarListagemFavoritos);
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

        //private void AbrirURI(object sender, TappedEventArgs e)
        //{
        //    if (e.Parameter is string parameter)
        //    {
        //        Launcher.TryOpenAsync(parameter);
        //    }
        //}

        private void AbrirURI2(object? sender, string? url)
        {
            if (url != null)
            {
                Launcher.TryOpenAsync(url);
            }
        }

        private void AbrirMail2(object? sender, string? email)
        {
            Launcher.TryOpenAsync("mailto:" + email);
        }

        //private void AbrirTelefone(object? sender, TappedEventArgs e)
        //{
        //    Launcher.TryOpenAsync("tel:" + e.Parameter?.ToString()?.Replace(".", "").Replace("(", "").Replace(")", ""));
        //}

        private void AbrirTelefone2(object? sender, string? telefone)
        {
            Launcher.TryOpenAsync("tel:" + telefone?.Replace(".", "").Replace("(", "").Replace(")", ""));
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

        protected void GoToSO(object sender, EventArgs e)
        {
            Launcher.TryOpenAsync(AppConstants.UrlTermoUso);
        }
    }
}