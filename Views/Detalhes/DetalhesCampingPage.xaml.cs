using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Views.CustomViews;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Detalhes
{
    public partial class DetalhesCampingPage : SmartContentPage
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

            if (!string.IsNullOrEmpty(item.LinkUltimaFoto))
            {
                imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

                //imgFotoItem.Source = item.LinkUltimaFoto;
                imgFotoItem.Source = CampingServices.MontarUrlImagemTemporaria(item.LinkUltimaFoto);

                //imgFotoItem.Source = "https://img.freepik.com/fotos-premium/cachorro-andando-na-rua_41691-381.jpg?w=1380";

                if (!string.IsNullOrEmpty(item.LinksFotos))
                {
                    //O id do item será obtido no clique através do ClassID do Grid
                    /*grdFotoPrincipal.ClassId = item.LinksFotos;*/
                    //imIconeGaleria.Source = ImageSource.FromResource("Keer.Resources.icone_galeria.jpg");
                    imIconeGaleria.Source = "icone_galeria.png";
                    slMaisFotos.IsVisible = true;

                    var gestureRecognizer = new TapGestureRecognizer();

                    gestureRecognizer.Tapped += async delegate
                    {
                        await Navigation.PushAsync(new ListagemFotosPage(TipoListagemFotos.Carousel, item.LinksFotos.Split('|').ToList()));
                    };

                    grdFotoPrincipal.GestureRecognizers.Add(gestureRecognizer);
                }
            }
            else
            {
                grdFotoPrincipal.IsVisible = false;
            }

            lbTitulo.Text = item.Nome?.ToUpper() ?? string.Empty;

            ConfigurarToolbar(item);

            var abrirMapa = new TapGestureRecognizer();
            var abrirEmail = new TapGestureRecognizer();
            var existeCoordenadas = item.IsValidLocation();

            slCoordenadas.IsVisible = existeCoordenadas;

            abrirMapa.Tapped += async delegate
            {
                var tipoMedia = existeCoordenadas ? TipoMedia.Mapa : TipoMedia.Endereco;

                await AppMedia.AbrirAsync(tipoMedia, ItemAtual);
            };
            abrirEmail.Tapped += async delegate
            {
                await AppMedia.AbrirAsync(TipoMedia.Email, ItemAtual.Email);
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
                    await AppMedia.AbrirAsync(TipoMedia.URL, url);
                };

                element.GestureRecognizers.Add(tapGestureRecognizer);
            });

            var listaTelefones = new List<string?>
            {
                ItemAtual.Telefone1,
                ItemAtual.Telefone2,
                ItemAtual.Telefone3,
                ItemAtual.Telefone4,
            };
            var listaTelefonesValidos = listaTelefones.Where(x => !string.IsNullOrEmpty(x)).ToList();
            var visivel = listaTelefonesValidos.Count > 0;

            gridTelefones.IsVisible = visivel;
            separadorTelefones.IsVisible = visivel;

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
                    Text = x ?? string.Empty,
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

            FirstAppeared += DetalhesCampingPage_FirstAppeared;
        }

        private async void DetalhesCampingPage_FirstAppeared(object? sender, EventArgs e)
        {
            await Task.Delay(500);

            if (ItemAtual.Descricao != null)
            {
                var descricao = Encoding.UTF8.GetString(Convert.FromBase64String(ItemAtual.Descricao));

                lbDescricao.Text = descricao.Replace("\r\n", "<br/>");
                cvTipo.Content = new TipoEstabelecimentoView(ItemAtual.Identificadores);
                cvComodidades.Content = new ComodidadesView(ItemAtual.Identificadores);
            }
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
                            await AppMedia.AbrirAsync(TipoMedia.Telefone, label.Text);
                            break;
                        case "whatsapp":
                            await AppMedia.AbrirAsync(TipoMedia.WhatsApp, label.Text);
                            break;
                    }
                }
            }
        }

        private void ConfigurarToolbar(Item item)
        {
            ToolbarItems.Clear();

            if (StorageHelper.IsFavoriteItem(item.Id))
            {
                var imagem = "icone_favoritos_on.png";

                ToolbarItems.Add(new ToolbarItem("Remover Favorito", imagem, () =>
                {
                    item.Favoritado = false;

                    StorageHelper.AddOrUpdateItem(item);
                    DBContract.Update(item);
                    ConfigurarToolbar(item);

                    WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_AtualizarListagemFavoritos);
                }));
            }
            else
            {
                var imagem = "icone_favoritos_off.png";

                ToolbarItems.Add(new ToolbarItem("Favoritar", imagem, () =>
                {
                    item.Favoritado = true;

                    StorageHelper.AddOrUpdateItem(item);
                    DBContract.Update(item);
                    ConfigurarToolbar(item);

                    WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_AtualizarListagemFavoritos);
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

        private async void OnMaisFotosTapped(object sender, EventArgs e)
        {
            if (sender is Grid grid && grid.ClassId is string url)
            {
                await Navigation.PushModalAsync(new VisualizacaoFotoPage(url, Title));
            }
        }

        public static async Task AbrirMapaAsync(string uriArquivoKml)
        {
            var permissionGranted = await Workaround.CheckPermissionAsync<Permissions.LocationWhenInUse>("Localização", "A permissão de localização será necessária para exibir o mapa");

            if (permissionGranted)
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

        private async void GoToSO(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(AppConstants.Url_TermoUso);
        }
    }
}