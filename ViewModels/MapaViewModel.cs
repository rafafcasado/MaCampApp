using System.Collections.ObjectModel;
using System.Diagnostics;
using DynamicData;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using static MaCamp.Utils.Enumeradores;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp.ViewModels
{
    public class MapaViewModel
    {
        public Map? Mapa { get; set; }
        public ObservableCollection<Item> Itens { get; set; }
        public bool UsarFiltros { get; set; }

        public EventHandler<PinClickedEventArgs> PinOnInfoWindowClicked { get; }

        public MapaViewModel(EventHandler<PinClickedEventArgs> pinOnInfoWindowClicked)
        {
            PinOnInfoWindowClicked = pinOnInfoWindowClicked;
            Itens = new ObservableCollection<Item>();
        }

        public async Task CarregarAsync()
        {
            if (!Itens.Any())
            {
                await CarregarItensAsync();
            }

            var permissao = await Workaround.CheckPermissionAsync<Permissions.LocationWhenInUse>("Localização", "A permissão de localização será necessária para exibir o mapa");

            if (permissao)
            {
                await CarregarMapaAsync();
            }
        }

        private async Task CarregarItensAsync()
        {
            var viewModel = new ListagemInfinitaViewModel();

            if (!Debugger.IsAttached && Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await viewModel.CarregarAsync(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, UsarFiltros);
            }
            else
            {
                var itensSalvos = await DBContract.ListAsync<Item>();

                viewModel.Itens = new ObservableCollection<Item>(itensSalvos);
            }

            Itens = new ObservableCollection<Item>(viewModel.Itens.Where(x => x.Longitude != 0 && x.Latitude != 0).ToList());
        }

        private async Task CarregarMapaAsync()
        {
            var map = new Map
            {
                IsShowingUser = true
            };
            var pins = CriarListaPins(Itens);

            map.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(map.VisibleRegion))
                {
                    await Workaround.DebounceAsync($"{nameof(MapaViewModel)}_{nameof(CarregarMapaAsync)}", 200, async cancel =>
                    {
                        await AtualizarListaPinsAsync(pins, map.VisibleRegion, cancel);
                    });
                }
            };

            Mapa = map;

            var chave = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstadoSelecionado);

            if (chave != null)
            {
                map.MoveMapToRegion(pins.Select(x => x.Location).ToList());
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

        private List<Pin> CriarListaPins(IEnumerable<Item> itens)
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

        private async Task AtualizarListaPinsAsync(List<Pin> listPins, MapSpan? region, CancellationToken token)
        {
            if (Mapa != null && region != null)
            {
                var maxClusters = Environment.ProcessorCount;
                var visiblePins = listPins.Where(x => region.IsInside(x.Location)).ToList();

                await Workaround.TaskUIAsync(() => Mapa.Pins.Clear(), token);

                if (visiblePins.Count > maxClusters)
                {
                    var clusters = PegarListaClusters(visiblePins, region, maxClusters);

                    foreach (var cluster in clusters)
                    {
                        if (cluster.Count == 1)
                        {
                            await Workaround.TaskUIAsync(() => Mapa.Pins.Add(cluster.First()), token);
                        }
                        else
                        {
                            var avgLat = cluster.Average(x => x.Location.Latitude);
                            var avgLon = cluster.Average(x => x.Location.Longitude);

                            await Workaround.TaskUIAsync(() =>
                            {
                                Mapa.Pins.Add(new StylishPin
                                {
                                    Label = cluster.Count.ToString(),
                                    Location = new Location(avgLat, avgLon)
                                });
                            }, token);
                        }
                    }
                }
                else
                {
                    await Workaround.TaskUIAsync(() => Mapa.Pins.AddRange(visiblePins), token);
                }
            }
        }

        private List<List<Pin>> PegarListaClusters(List<Pin> pins, MapSpan region, int maxClusters)
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
