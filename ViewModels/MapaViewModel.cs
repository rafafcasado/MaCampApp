using System.Text.RegularExpressions;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Clustering;
using static MaCamp.Utils.Enumeradores;
using Map = Maui.GoogleMaps.Map;

namespace MaCamp.ViewModels
{
    public class MapaViewModel
    {
        public Map? Mapa { get; set; }
        public List<Item> Itens { get; set; }
        public bool UsarFiltros { get; set; }

        private List<Pin> ListaPins { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private List<string> ListaIdentificadoresPermitidos { get; }
        private Dictionary<string, Stream?> DictionaryImages { get; set; }
        private EventHandler<InfoWindowClickedEventArgs> ClusteredMap_InfoWindowClicked { get; }
        private int ChunkLoad { get; }

        public MapaViewModel(EventHandler<InfoWindowClickedEventArgs> clusteredMap_InfoWindowClicked)
        {
            CancellationTokenSource = new CancellationTokenSource();
            ListaIdentificadoresPermitidos = new List<string>
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
            DictionaryImages = new Dictionary<string, Stream?>();
            ClusteredMap_InfoWindowClicked = clusteredMap_InfoWindowClicked;
            Itens = new List<Item>();
            ListaPins = new List<Pin>();
            ChunkLoad = Environment.ProcessorCount * 50;
        }

        public async Task CarregarAsync(CancellationToken cancellationToken = default)
        {
            var assembly = typeof(MapaViewModel).Assembly;
            var listResourceNames = assembly.GetManifestResourceNames();

            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            DictionaryImages = listResourceNames.Where(x => x.EndsWith("_small.png")).ToDictionary(x => Regex.Replace(x, @"^.*(?=\bpointer_)", string.Empty), x => assembly.GetManifestResourceStream(x));

            if (App.LOCALIZACAO_USUARIO == null)
            {
                App.LOCALIZACAO_USUARIO = await Workaround.GetLocationAsync(AppConstants.Mensagem_Localizacao_Mapa);
            }

            if (!Itens.Any())
            {
                await Workaround.TaskWorkAsync(async () => await CarregarItensAsync(cancellationToken), cancellationToken);
            }

            ListaPins = await Workaround.TaskWorkAsync(() => CriarListaPins(Itens.Where(x => x.IsValidLocation()).ToList()), new List<Pin>(), CancellationTokenSource.Token);

            await Workaround.TaskUIAsync(() => CarregarMapa());
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        private async Task CarregarItensAsync(CancellationToken cancellationToken)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var viewModel = new ListagemInfinitaViewModel();

                await viewModel.CarregarAsync(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, UsarFiltros);

                Itens = viewModel.Itens.ToList();
            }
            else
            {
                Itens = DBContract.List<Item>();
            }
        }

        private Position PegarPosicaoInicial()
        {
            var chave = DBContract.GetKeyValue(AppConstants.Filtro_EstadoSelecionado);

            if (!string.IsNullOrEmpty(chave))
            {
                var centerPosition = ListaPins.Select(x => x.Position).GetCenterPosition();

                if (centerPosition is Position position)
                {
                    return position;
                }
            }

            if (App.LOCALIZACAO_USUARIO != null)
            {
                return App.LOCALIZACAO_USUARIO.GetPosition();
            }

            // Se a localização do usuário não estiver disponível, use uma posição padrão (São Paulo)
            return new Position(-23.550520, -46.633308);
        }

        private void CarregarMapa()
        {
            var position = PegarPosicaoInicial();
            var map = new ClusteredMap
            {
                MyLocationEnabled = true,
                InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(position, 5)
            };
            var listaTuplaPinsDistancia = ListaPins.Select(x => new
            {
                Pin = x,
                Distance = Location.CalculateDistance(position.Latitude, position.Longitude, x.Position.Latitude, x.Position.Longitude, DistanceUnits.Kilometers)
            }).OrderBy(x => x.Distance).ToList();

            map.Loaded += Map_Loaded;
            map.CameraIdled += MapOnCameraIdled;
            map.InfoWindowClicked += ClusteredMap_InfoWindowClicked;

            ListaPins = listaTuplaPinsDistancia.Select(x => x.Pin).ToList();
            Mapa = map;
        }

        private void Map_Loaded(object? sender, EventArgs e)
        {
            if (sender is ClusteredMap clusteredMap)
            {
                ListaPins.Take(ChunkLoad).ForEach(x => clusteredMap.Pins.Add(x));
            }
        }

        private async void MapOnCameraIdled(object? sender, CameraIdledEventArgs e)
        {
            if (sender is ClusteredMap clusteredMap)
            {
                var listaPins = ListaPins.Where(x => !clusteredMap.Pins.Contains(x) && e.Position.Contains(x.Position)).ToList();

                await clusteredMap.Pins.AddRangeAsync(listaPins);
            }
        }

        private List<Pin> CriarListaPins(IEnumerable<Item> itens)
        {
            var listaPins = new List<Pin>();

            Parallel.ForEach(itens, item =>
            {
                var tipos = item.Identificadores.Where(x => x.Opcao == 0 && x.Identificador != null && ListaIdentificadoresPermitidos.Contains(x.Identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower())).ToList();

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

                    if (DictionaryImages.TryGetValue(imagem, out var stream) && stream != null && stream != Stream.Null)
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
