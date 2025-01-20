using System.Text;
using MaCamp.Utils;
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
                        await Navigation.PushAsync(new ListagemFotosPage(item.LinksFotos.Split('|').ToList()));
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
            var abrirEmail = new TapGestureRecognizer();
            var existeCoordenadas = item.Latitude != 0 || item.Longitude != 0;

            slCoordenadas.IsVisible = existeCoordenadas;

            abrirMapa.Tapped += async delegate
            {
                var tipoMedia = existeCoordenadas ? Enumeradores.TipoMedia.Mapa : Enumeradores.TipoMedia.Endereco;

                await AppMedia.AbrirAsync(tipoMedia, ItemAtual);
            };
            abrirEmail.Tapped += async delegate
            {
                await AppMedia.AbrirAsync(Enumeradores.TipoMedia.Email, ItemAtual.Email);
            };

            slEndereco.GestureRecognizers.Add(abrirMapa);
            slEmail.GestureRecognizers.Add(abrirEmail);

            var listaElementosURL = new Dictionary<View, string?>
            {
                { slSite, ItemAtual.Site },
                { slPreco, ItemAtual.LinkPrecos },
                { slFacebook, ItemAtual.Facebook },
                { slInstagram, ItemAtual.Instagram },
                { slYoutube, ItemAtual.Youtube }
            };

            listaElementosURL.ForEach(x =>
            {
                var (element, url) = x;
                var tapGestureRecognizer = new TapGestureRecognizer();

                tapGestureRecognizer.Tapped += async delegate
                {
                    await AppMedia.AbrirAsync(Enumeradores.TipoMedia.URL, url);
                };

                element.GestureRecognizers.Add(tapGestureRecognizer);
            });

            var listaTelefones = new List<string?>
            {
                ItemAtual.Telefone,
                ItemAtual.Telefone2,
                ItemAtual.Telefone3,
                ItemAtual.Telefone4,
            };
            var listaTelefonesValidos = listaTelefones.Where(x => !string.IsNullOrEmpty(x)).ToList();

            gridTelefones.IsVisible = listaTelefonesValidos.Count > 0;

            listaTelefonesValidos.ForEach(x =>
            {
                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection(new ColumnDefinition(GridLength.Star), new ColumnDefinition(40), new ColumnDefinition(40))
                };
                var label = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    Text = x,
                    TextColor = Colors.Gray,
                    VerticalTextAlignment = TextAlignment.Center
                };
                var whatsAppButton = new ImageButton
                {
                    HeightRequest = 25,
                    WidthRequest = 25,
                    Source = "whatsapp.png",
                    ClassId = "whatsapp"
                };
                var dialerButton = new ImageButton
                {
                    HeightRequest = 25,
                    WidthRequest = 25,
                    Source = "chamada.png",
                    ClassId = "dialer"
                };

                Grid.SetColumn(label, 0);
                Grid.SetColumn(whatsAppButton, 1);
                Grid.SetColumn(dialerButton, 2);

                whatsAppButton.Clicked += ImageButton_Clicked;
                dialerButton.Clicked += ImageButton_Clicked;

                grid.Children.Add(label);
                grid.Children.Add(whatsAppButton);
                grid.Children.Add(dialerButton);

                layoutTelefones.Add(grid);
            });
        }

        private async void ImageButton_Clicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.ClassId != null && imageButton.Parent is Grid grid)
            {
                var label = grid.Children.OfType<Label>().FirstOrDefault();

                if (label != null)
                {
                    switch (imageButton.ClassId)
                    {
                        case "dialer":
                            await AppMedia.AbrirAsync(Enumeradores.TipoMedia.Telefone, label.Text);
                            break;
                        case "whatsapp":
                            await AppMedia.AbrirAsync(Enumeradores.TipoMedia.WhatsApp, label.Text);
                            break;
                    }
                }
            }
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

        //private async void OnMaisFotosTapped(object sender, EventArgs e)
        //{
        //    var url = sender is Grid grid && grid.ClassId;
        //
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
            await Launcher.OpenAsync(AppConstants.Url_TermoUso);
        }
    }
}