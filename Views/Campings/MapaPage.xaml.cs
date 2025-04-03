using System.Collections.ObjectModel;
using DynamicData;
using FluentIcons.Common;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.Detalhes;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using static MaCamp.Utils.Enumeradores;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp.Views.Campings
{
    public partial class MapaPage : ContentPage
    {
        private bool UsarFiltros { get; set; }
        private ObservableCollection<Item>? Itens { get; set; }

        private MapaPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);
            BackgroundColor = Colors.White;

            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 35,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            toggleButton.ImageSource = new SymbolImageSource
            {
                Symbol = Symbol.Map,
                IconVariant = IconVariant.Regular,
                Color = AppColors.CorPrimaria
            };

            Loaded += MapaPage_Loaded;
        }

        public MapaPage(bool usarFiltros) : this()
        {
            Title = "Mapa";
            UsarFiltros = usarFiltros;
        }

        public MapaPage(ObservableCollection<Item> itens) : this()
        {
            Title = "Ver no Mapa";
            Itens = itens;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DeviceDisplay.KeepScreenOn = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            DeviceDisplay.KeepScreenOn = false;
        }

        private async void MapaPage_Loaded(object? sender, EventArgs e)
        {
            await Workaround.TaskWork(async () =>
            {
                if (Itens == null)
                {
                    Itens = await CarregarItensAsync(UsarFiltros);
                }
            });
            await Workaround.TaskUI(async () =>
            {
                var permissionGranted = await Workaround.CheckPermission<Permissions.LocationWhenInUse>("Localização", "A permissão de localização será necessária para exibir o mapa");

                if (permissionGranted && Itens != null)
                {
                    var map = new Map
                    {
                        IsShowingUser = true
                    };
                    var listPins = CriarListaPins(Itens);

                    map.PropertyChanged += async (sender, e) =>
                    {
                        if (e.PropertyName == nameof(map.VisibleRegion))
                        {
                            await Workaround.DebounceAsync($"{nameof(MapaPage)}_{nameof(Map.PropertyChanged)}", 200, async x =>
                            {
                                await AtualizarPinsAsync(listPins, map.VisibleRegion, x);
                            });
                        }
                    };

                    cvMapa.Content = map;

                    await Task.Delay(500);

                    toggleButton.IsVisible = true;

                    try
                    {
                        var valorChaveEstadoSelecionado = DBContract.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);

                        if (valorChaveEstadoSelecionado != null)
                        {
                            map.MoveMapToRegion(listPins.Select(x => x.Location).ToList());
                        }
                        else
                        {
                            if (App.LOCALIZACAO_USUARIO == null)
                            {
                                App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();
                            }

                            if (App.LOCALIZACAO_USUARIO == null)
                            {
                                App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
                            }

                            if (App.LOCALIZACAO_USUARIO != null)
                            {
                                map.MoveToRegion(MapSpan.FromCenterAndRadius(App.LOCALIZACAO_USUARIO, Distance.FromKilometers(10)));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Workaround.ShowExceptionOnlyDevolpmentMode(nameof(MapaPage), nameof(MapaPage_Loaded), ex);
                    }
                }
            });
        }

        private async Task<ObservableCollection<Item>> CarregarItensAsync(bool usarFiltros)
        {
            var vm = new ListagemInfinitaVM();

            if (!System.Diagnostics.Debugger.IsAttached && Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await vm.Carregar(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, usarFiltros);
            }
            else
            {
                var itensSalvos = DBContract.ListarItens();

                vm.Itens = new ObservableCollection<Item>(itensSalvos);
            }

            return vm.Itens;
        }

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            if (cvMapa.Content is Map map)
            {
                var isStreet = map.MapType == MapType.Street;

                map.MapType = isStreet ? MapType.Satellite : MapType.Street;

                toggleButton.Text = isStreet ? "Satélite" : "Terreno";
                toggleButton.BackgroundColor = isStreet ? Color.FromArgb("#FFFFFFFF") : Color.FromArgb("#FFFFFFD9");
                toggleButton.ImageSource = new SymbolImageSource
                {
                    Symbol = Symbol.Map,
                    Color = AppColors.CorPrimaria,
                    IconVariant = isStreet ? IconVariant.Filled : IconVariant.Regular
                };
            }
        }

        private async void PinOnInfoWindowClicked(object? sender, PinClickedEventArgs e)
        {
            if (sender is StylishPin stylishPin && stylishPin.Data is Item item)
            {
                await Navigation.PushAsync(new DetalhesCampingPage(item));
            }
        }

        private List<Pin> CriarListaPins(ObservableCollection<Item> itens)
        {
            var listPins = new List<Pin>();
            var identificadoresPermitidos = new List<string>
            {
                "campingemreformas",
                "empresa",
                "destaque",
                "campinginformal",
                "campingemsituacaoincerta",
                "pontodeapoioarvs",
                "campingselvagemwildcampingbushcraft",
                "semfuncaocampingapoiooufechado",
                "campingemfuncionamento"
            };

            foreach (var item in itens)
            {
                var tipos = item.Identificadores.Where(x => x.Opcao == 0 && x.Identificador != null && identificadoresPermitidos.Contains(x.Identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower())).ToList();

                if (!string.IsNullOrEmpty(item.Nome) && tipos.Any())
                {
                    var identificador = tipos.FirstOrDefault()?.Identificador ?? string.Empty;
                    var imagem = "pointer_" + identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + "_small.png";
                    var pin = new StylishPin
                    {
                        Label = item.Nome,
                        ImageSource = imagem,
                        Address = item.EnderecoCompleto,
                        Location = item.GetLocation(),
                        Data = item
                    };

                    pin.InfoWindowClicked += PinOnInfoWindowClicked;

                    listPins.Add(pin);
                }
            }

            return listPins;
        }

        private async Task AtualizarPinsAsync(List<Pin> listPins, MapSpan? region, CancellationToken cancellationToken = default)
        {
            if (cvMapa.Content is Map map && region != null)
            {
                var maxClusters = Environment.ProcessorCount;
                var visiblePins = listPins.Where(x => region.IsInside(x.Location)).ToList();

                await Workaround.TaskUI(() => map.Pins.Clear(), cancellationToken);

                if (visiblePins.Count > maxClusters)
                {
                    var clusters = ObterClusters(visiblePins, region, maxClusters);

                    foreach (var cluster in clusters)
                    {
                        if (cluster.Count == 1)
                        {
                            await Workaround.TaskUI(() => map.Pins.Add(cluster.First()), cancellationToken);
                        }
                        else
                        {
                            var avgLat = cluster.Average(x => x.Location.Latitude);
                            var avgLon = cluster.Average(x => x.Location.Longitude);

                            await Workaround.TaskUI(() => map.Pins.Add(new StylishPin
                            {
                                Label = cluster.Count.ToString(),
                                Location = new Location(avgLat, avgLon)
                            }), cancellationToken);
                        }
                    }
                }
                else
                {
                    await Workaround.TaskUI(() => map.Pins.AddRange(visiblePins), cancellationToken);
                }
            }
        }

        private List<List<Pin>> ObterClusters(List<Pin> pins, MapSpan region, int maxClusters)
        {
            var clusters = Enumerable.Range(0, maxClusters).Select(x => new List<Pin>()).ToList();
            var latStep = region.LatitudeDegrees / Math.Sqrt(maxClusters);
            var lonStep = region.LongitudeDegrees / Math.Sqrt(maxClusters);

            foreach (var pin in pins)
            {
                var latIndex = (int)((pin.Location.Latitude - (region.Center.Latitude - region.LatitudeDegrees / 2)) / latStep);
                var lonIndex = (int)((pin.Location.Longitude - (region.Center.Longitude - region.LongitudeDegrees / 2)) / lonStep);
                var clusterIndex = (latIndex * (int)Math.Sqrt(maxClusters) + lonIndex) % maxClusters;

                clusters[clusterIndex].Add(pin);
            }

            return clusters.Where(c => c.Count > 0).ToList();
        }
    }
}