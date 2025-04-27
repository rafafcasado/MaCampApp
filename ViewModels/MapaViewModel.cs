using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DynamicData;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Clustering;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.ViewModels
{
    public class MapaViewModel
    {
        public ClusteredMap? Mapa { get; set; }
        public ObservableCollection<Item> Itens { get; set; }
        public bool UsarFiltros { get; set; }

        private EventHandler<InfoWindowClickedEventArgs> ClusteredMap_InfoWindowClicked { get; }

        public MapaViewModel(EventHandler<InfoWindowClickedEventArgs> clusteredMap_InfoWindowClicked)
        {
            ClusteredMap_InfoWindowClicked = clusteredMap_InfoWindowClicked;
            Itens = new ObservableCollection<Item>();
        }

        public async Task CarregarAsync()
        {
            if (!Itens.Any())
            {
                await Workaround.TaskWorkAsync(async () => await CarregarItensAsync());
            }

            await CarregarMapaAsync();
        }

        private async Task CarregarItensAsync()
        {
            var viewModel = new ListagemInfinitaViewModel();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await viewModel.CarregarAsync(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, UsarFiltros);
            }
            else
            {
                var itensSalvos = DBContract.List<Item>();

                viewModel.Itens = new ObservableCollection<Item>(itensSalvos);
            }

            Itens = new ObservableCollection<Item>(viewModel.Itens.Where(x => x.IsValidLocation()).ToList());
        }

        private async Task CarregarMapaAsync()
        {
            var clusteredMap = new ClusteredMap();
            var listaPins = await Workaround.TaskWorkAsync(() => CriarListaPins(Itens), new List<Pin>());

            clusteredMap.Pins.AddRange(listaPins);

            clusteredMap.Loaded += Map_Loaded;
            clusteredMap.InfoWindowClicked += ClusteredMap_InfoWindowClicked;

            Mapa = clusteredMap;
        }

        private async void Map_Loaded(object? sender, EventArgs e)
        {
            if (sender is ClusteredMap clusteredMap)
            {
                await Workaround.TaskWorkAsync(async () =>
                {
                    var quantidadeCampingsVisualizar = 10;
                    var chave = DBContract.GetKeyValue(AppConstants.Filtro_EstadoSelecionado);
                    var listaPositions = clusteredMap.Pins.Select(x => x.Position).ToList();

                    if (!string.IsNullOrEmpty(chave))
                    {
                        await Workaround.TaskUIAsync(() => clusteredMap.MoveMapToRegion(listaPositions));
                    }
                    else
                    {
                        if (App.LOCALIZACAO_USUARIO == null)
                        {
                            App.LOCALIZACAO_USUARIO = await Workaround.GetLocationAsync(AppConstants.Mensagem_Localizacao_Mapa);

                            await Workaround.TaskUIAsync(() => clusteredMap.MyLocationEnabled = true);
                        }

                        // Se a localização do usuário não estiver disponível, use uma posição padrão (São Paulo)
                        var position = App.LOCALIZACAO_USUARIO != null ? App.LOCALIZACAO_USUARIO.GetPosition() : new Position(-23.550520, -46.633308);
                        var orderedByDistance = listaPositions.Select(x => Location.CalculateDistance(position.Latitude, position.Longitude, x.Latitude, x.Longitude, DistanceUnits.Kilometers)).OrderBy(x => x).ToList();
                        var radiusKm = orderedByDistance.Count >= quantidadeCampingsVisualizar ? orderedByDistance[quantidadeCampingsVisualizar - 1] : orderedByDistance.Last();

                        await Workaround.TaskUIAsync(() => clusteredMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(radiusKm * 0.5))));
                    }
                });
            }
        }

        private List<Pin> CriarListaPins(IEnumerable<Item> itens)
        {
            var listaPins = new List<Pin>();
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
            var assembly = typeof(MapaViewModel).Assembly;
            var names = assembly.GetManifestResourceNames();
            var dictionaryImages = names.Where(x => x.EndsWith("_small.png")).ToDictionary(x => Regex.Replace(x, @"^.*(?=\bpointer_)", string.Empty), x => assembly.GetManifestResourceStream(x));

            Parallel.ForEach(itens, item =>
            {
                var tipos = item.Identificadores.Where(x => x.Opcao == 0 && x.Identificador != null && identificadoresPermitidos.Contains(x.Identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower())).ToList();

                if (!string.IsNullOrEmpty(item.Nome) && tipos.Any())
                {
                    var identificador = tipos.FirstOrDefault()?.Identificador ?? string.Empty;
                    var pin = new Pin
                    {
                        Label = item.Nome,
                        Address = item.EnderecoCompleto,
                        Position = item.GetPosition(),
                        Tag = item
                    };
                    var imagem = "pointer_" + identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + "_small.png";

                    if (dictionaryImages.TryGetValue(imagem, out var stream))
                    {
                        pin.Icon = BitmapDescriptorFactory.FromStream(stream);
                    }

                    listaPins.Add(pin);
                }
            });

            return listaPins;
        }
    }
}
