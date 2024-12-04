using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.ViewModels;
using Aspbrasil.Views.Detalhes;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPage : ContentPage
    {
        Map map;

        public MapaPage(bool usarFiltros = true)
        {
            InitializeComponent();
            Title = "Mapa";
            NavigationPage.SetBackButtonTitle(this, "");

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView(Title);

            cvMapa.Content = new ActivityIndicator { IsRunning = true, IsVisible = true, HeightRequest = 35, Color = AppColors.COR_PRIMARIA, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            Task.Run(async () =>
            {
                var vm = new ListagemInfinitaVM();
                await vm.Carregar("", -1, "", "", TipoListagem.Camping, usarFiltros);

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        ExibirMapa(vm.Itens);
                        break;
                    case Device.Android:
                        bool possuiPermissao = await VerificarPermissaoLocalizacao();
                        if (possuiPermissao) { ExibirMapa(vm.Itens); }
                        break;
                }
            });
        }

        public MapaPage(System.Collections.ObjectModel.ObservableCollection<Models.Item> itens)
        {
            InitializeComponent();
            Title = "Ver no Mapa";
            NavigationPage.SetBackButtonTitle(this, "");

            cvMapa.Content = new ActivityIndicator { IsRunning = true, IsVisible = true, HeightRequest = 35, Color = AppColors.COR_PRIMARIA, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            Task.Run(async () =>
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        ExibirMapa(itens);
                        break;
                    case Device.Android:
                        bool possuiPermissao = await VerificarPermissaoLocalizacao();
                        if (possuiPermissao) { ExibirMapa(itens); }
                        break;
                }
            });
        }

        private async Task<bool> VerificarPermissaoLocalizacao()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Localização necessária", "A permissão de localização será necessária para exibir o mapa", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Localização negada", "Não é possível continuar, tente novamente.", "OK");
                    return false;
                }
            }
            catch { }
            return false;
        }

        private async void ExibirMapa(System.Collections.ObjectModel.ObservableCollection<Models.Item> itens)
        {
            map = new Map();

            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;

            string tamanho = "";

            if (Device.RuntimePlatform == Device.Android && App.ScreenPixelsSize.Height <= 700)
            {
                tamanho = "_small";
            }
            else 
            {
                tamanho = "_small";
            }

            var identificadoresPermitidos = new List<string> { "campingemreformas", "empresa", "destaque", "campinginformal", "campingemsituacaoincerta", "pontodeapoioarvs", "campingselvagemwildcampingbushcraft", "semfuncaocampingapoiooufechado", "campingemfuncionamento" };

            List<Position> positionsCampings = new List<Position>();
            foreach (var item in itens)
            {
                if (item.Latitude != 0 && item.Longitude != 0)
                {
                    var tipos = item.Identificadores.Where(i => i.Opcao == 0 && identificadoresPermitidos.Contains(i.Identificador.Replace("`", "").Replace("çã", "ca").Replace("/", "").ToLower())).ToList();

                    if (tipos.Count() > 0)
                    {
                        string imagem = "pointer_" + tipos[0].Identificador.Replace("`", "").Replace("çã", "ca").Replace("/", "").ToLower() + tamanho + ".png";
                        map.Pins.Add(new Pin
                        {
                            Label = item.Nome,
                            Icon = BitmapDescriptorFactory.FromBundle(imagem),
                            Address = item.EnderecoCompleto,
                            Position = new Position(item.Latitude.Value, item.Longitude.Value),
                            Tag = item
                        });
                        positionsCampings.Add(new Position(item.Latitude.Value, item.Longitude.Value));
                    }
                }
            }

            map.InfoWindowClicked += Map_InfoWindowClicked;

            string valorChaveEstadoSelecionado = DBContract.NewInstance().ObterValorChave("FILTROS_ESTADO_SELECIONADO");
            //if (valorChaveEstadoSelecionado != null && valorChaveEstadoSelecionado != null)
            //{
            map.MoveToRegion(MapSpan.FromPositions(positionsCampings));
            //}
            //else
            //{
            //    if (App.LOCALIZACAO_USUARIO == null)
            //    {
            //        App.LOCALIZACAO_USUARIO = await CrossGeolocator.Current.GetLastKnownLocationAsync();
            //        if (App.LOCALIZACAO_USUARIO == null)
            //        {
            //            App.LOCALIZACAO_USUARIO = await CrossGeolocator.Current.GetPositionAsync();
            //        }
            //    }
            //    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(App.LOCALIZACAO_USUARIO.Latitude, App.LOCALIZACAO_USUARIO.Longitude), Distance.FromKilometers(10)));
            //}

            Device.BeginInvokeOnMainThread(() =>
            {
                cvMapa.Content = map;
            });
        }

        private async void Map_InfoWindowClicked(object sender, InfoWindowClickedEventArgs e)
        {
            Item itemSelecionado = (e.Pin.Tag as Item);
            await Navigation.PushAsync(new DetalhesCampingPage(itemSelecionado));
        }
    }
}