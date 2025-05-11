using System.Globalization;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using MPowerKit.GoogleMaps;
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
        private Action<Pin> Map_InfoWindowClick { get; }

        public MapaViewModel(Action<Pin> map_InfoWindowClick)
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
            Map_InfoWindowClick = map_InfoWindowClick;
            ListaPins = new List<Pin>();
            Itens = new List<Item>();
        }

        public async Task CarregarAsync(CancellationToken cancellationToken = default)
        {
            App.LOCALIZACAO_USUARIO = await Workaround.GetLocationAsync(AppConstants.Mensagem_Localizacao_Mapa, true, cancellationToken);

            if (!Itens.Any())
            {
                //await Workaround.TaskWorkAsync(async () => await CarregarItensAsync(cancellationToken), cancellationToken);

                var latitude = Convert.ToString(App.LOCALIZACAO_USUARIO?.Latitude ?? 0, CultureInfo.InvariantCulture).Replace(",", ".");
                var longitude = Convert.ToString(App.LOCALIZACAO_USUARIO?.Longitude ?? 0, CultureInfo.InvariantCulture).Replace(",", ".");

                Content = new WebView
                {
                    Source = $"{AppConstants.Url_WebViewMapa}?lat={latitude}&long={longitude}"
                };
            }
            else
            {
                var listaItensFiltrados = Itens.Where(x => x.IsValidLocation()).ToList();
                var listaItensFiltradosOrdenados = listaItensFiltrados.OrderBy(x => x.GetDistanceKilometersFromUser() ?? double.MaxValue).ToList();

                ListaPins = await Workaround.TaskWorkAsync(() => CriarListaPins(listaItensFiltradosOrdenados), new List<Pin>(), CancellationTokenSource.Token);

                await Workaround.TaskUIAsync(() => CarregarMapa());
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

        private void CarregarMapa()
        {
            var cameraUpdate = ListaPins.ToCameraUpdate(App.LOCALIZACAO_USUARIO);
            var map = new GoogleMap
            {
                MyLocationEnabled = true,
                MyLocationButtonEnabled = true,
                ZoomControlsEnabled = true,
                CompassEnabled = true,
                MapToolbarEnabled = true,
                BuildingsEnabled = true,
                IndoorEnabled = true,
                IndoorLevelPickerEnabled = true,
                RotateGesturesEnabled = true,
                ScrollGesturesEnabled = true,
                TiltGesturesEnabled = true,
                ZoomGesturesEnabled = true,
                MapType = MapType.Normal,
                InitialCameraPosition = cameraUpdate,
                GeoJson = ListaPins.ToGeoJson()
            };

            map.InfoWindowClick += Map_InfoWindowClick;

            //map.Pins = ListaPins;

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
                        Title = item.Nome,
                        Snippet = item.EnderecoCompleto,
                        Position = item.GetPosition(),
                        Icon = ResourceImageSource.From(imagem)
                    };

                    listaPins.Add(pin);
                }
            });

            return listaPins;
        }
    }
}
