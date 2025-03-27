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

                    map.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(map.VisibleRegion))
                        {
                            AtualizarPins(listPins, map.VisibleRegion);
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

        private void AtualizarPins(List<Pin> listPins, MapSpan? region)
        {
            if (cvMapa.Content is Map map)
            {
                map.Pins.Clear();

                if (region != null)
                {
                    var maxClusters = Convert.ToInt32(Environment.ProcessorCount * 1.5);
                    var visiblePins = listPins.Where(x => region.IsInside(x.Location)).ToList();

                    if (visiblePins.Count > maxClusters)
                    {
                        var clusters = ObterClusters(visiblePins, region, maxClusters);

                        foreach (var cluster in clusters)
                        {
                            if (cluster.Count == 1)
                            {
                                map.Pins.Add(cluster.First());
                            }
                            else
                            {
                                var avgLat = cluster.Average(x => x.Location.Latitude);
                                var avgLon = cluster.Average(x => x.Location.Longitude);
                                var clusterPin = new StylishPin
                                {
                                    Label = cluster.Count.ToString(),
                                    Location = new Location(avgLat, avgLon),
                                };

                                map.Pins.Add(clusterPin);
                            }
                        }
                    }
                    else
                    {
                        map.Pins.AddRange(visiblePins);
                    }
                }
            }
        }

        private List<List<Pin>> ObterClusters(List<Pin> pins, MapSpan region, int maxClusters)
        {
            var cellCount = Convert.ToInt32(Math.Sqrt(maxClusters));
            var latCellSize = region.LatitudeDegrees / cellCount;
            var lonCellSize = region.LongitudeDegrees / cellCount;
            var clusters = pins.GroupBy(x =>
            {
                var row = Convert.ToInt32((x.Location.Latitude - (region.Center.Latitude - region.LatitudeDegrees / 2)) / latCellSize);
                var col = Convert.ToInt32((x.Location.Longitude - (region.Center.Longitude - region.LongitudeDegrees / 2)) / lonCellSize);

                return (row, col);
            }).Select(x => x.ToList()).ToList();

            if (clusters.Count > maxClusters)
            {
                var newCellCount = Convert.ToInt32(Math.Sqrt(clusters.Count));

                return ObterClusters(pins, region, newCellCount);
            }

            return clusters;
        }
    }
}