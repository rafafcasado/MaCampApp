using MaCamp.CustomControls;
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
        public View? Content { get; private set; }
        public List<Item> Itens { get; set; }
        public bool UsarFiltros { get; set; }

        private List<Pin> ListaPins { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private List<string> ListaIdentificadoresPermitidos { get; }
        private EventHandler<InfoWindowClickedEventArgs> ClusteredMap_InfoWindowClicked { get; }

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
            ClusteredMap_InfoWindowClicked = clusteredMap_InfoWindowClicked;
            ListaPins = new List<Pin>();
            Itens = new List<Item>();
        }

        public async Task CarregarAsync(CancellationToken cancellationToken = default)
        {
            if (App.LOCALIZACAO_USUARIO == null)
            {
                App.LOCALIZACAO_USUARIO = await Workaround.GetLocationAsync(AppConstants.Mensagem_Localizacao_Mapa);
            }

            if (!Itens.Any())
            {
                //await Workaround.TaskWorkAsync(async () => await CarregarItensAsync(cancellationToken), cancellationToken);

                Content = new CustomWebView
                {
                    Source = $"{AppConstants.Url_WebViewMapa}?lat={App.LOCALIZACAO_USUARIO?.Latitude}&long={App.LOCALIZACAO_USUARIO?.Longitude}"
                };
            }
            else
            {
                var position = PegarPosicaoInicial();
                var listaItensFiltrados = Itens.Where(x => x.IsValidLocation()).ToList();
                var listaItensFiltradosOrdenados = listaItensFiltrados.OrderBy(x => x.GetDistanceKilometersFromUser() ?? double.MaxValue).ToList();

                ListaPins = await Workaround.TaskWorkAsync(() => CriarListaPins(listaItensFiltradosOrdenados), new List<Pin>(), CancellationTokenSource.Token);

                await Workaround.TaskUIAsync(() => CarregarMapa(position));
            }

        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        private async Task CarregarItensAsync(CancellationToken cancellationToken)
        {
            if (!System.Diagnostics.Debugger.IsAttached && Connectivity.NetworkAccess == NetworkAccess.Internet)
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

        private void CarregarMapa(Position position)
        {
            var bounds = ListaPins.Select(x => x.Position).ToBounds();
            var cameraUpdate = bounds != null ? CameraUpdateFactory.NewBounds(bounds, 25) : CameraUpdateFactory.NewPosition(position);
            var map = new ClusteredMap
            {
                MyLocationEnabled = true,
                InitialCameraUpdate = cameraUpdate,
                GeoJson = ListaPins.ToGeoJson()
            };

            map.InfoWindowClicked += ClusteredMap_InfoWindowClicked;

            Content = map;
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
                    var imagem = "pointer_" + identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + "_small.png";
                    var pin = new Pin
                    {
                        Label = item.Nome,
                        Address = item.EnderecoCompleto,
                        Position = item.GetPosition(),
                        Tag = item,
                        NativeObject = imagem
                    };

                    listaPins.Add(pin);
                }
            });

            return listaPins;
        }
    }
}
