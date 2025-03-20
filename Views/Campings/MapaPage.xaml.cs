using System.Collections.ObjectModel;
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
        public MapaPage(bool usarFiltros)
        {
            InitializeComponent();

            Title = "Mapa";
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            BackgroundColor = Colors.White;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView(Title);

            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 35,
                WidthRequest = 35,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var vm = new ListagemInfinitaVM();

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await vm.Carregar(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, usarFiltros);
                }
                else
                {
                    var itensSalvos = DBContract.ListarItens();

                    vm.Itens = new ObservableCollection<Item>(itensSalvos);
                }

                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    ExibirMapa(vm.Itens);
                }
                else if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var possuiPermissao = await VerificarPermissaoLocalizacao();

                    if (possuiPermissao)
                    {
                        ExibirMapa(vm.Itens);
                    }
                }
            });
        }

        public MapaPage(ObservableCollection<Item> itens)
        {
            InitializeComponent();

            Title = "Ver no Mapa";
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 35,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    ExibirMapa(itens);
                }
                else if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var possuiPermissao = await VerificarPermissaoLocalizacao();

                    if (possuiPermissao)
                    {
                        ExibirMapa(itens);
                    }
                }
            });
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

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            if (cvMapa.Content is Map map)
            {
                var isStreet = map.MapType == MapType.Street;

                map.MapType = isStreet ? MapType.Satellite : MapType.Street;

                toggleButton.Text = isStreet ? "Satélite" : "Terreno";
                toggleButton.BackgroundColor = isStreet ? Colors.White : Color.FromArgb("#b8bdcb");
                toggleButton.ImageSource = new SymbolImageSource
                {
                    Symbol = Symbol.Map,
                    IconVariant = isStreet ? IconVariant.Filled : IconVariant.Regular
                };
            }
        }

        private async Task<bool> VerificarPermissaoLocalizacao()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    var shouldShowRationale = Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();

                    if (shouldShowRationale)
                    {
                        await DisplayAlert("Localização necessária", "A permissão de localização será necessária para exibir o mapa", "OK");
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }

                if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Localização negada", "Não é possível continuar, tente novamente.", "OK");

                    return false;
                }
            }
            catch (Exception ex)
            {
                await AppConstants.CurrentPage.DisplayAlert("Erro ao carregar o mapa", ex.Message, "OK");
            }

            return false;
        }

        private async void ExibirMapa(ObservableCollection<Item> itens)
        {
            var map = new Map
            {
                IsShowingUser = true
            };
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
            var positionsCampings = new List<Location>();

            foreach (var item in itens)
            {
                if (item.Latitude is double latitude && latitude != 0 && item.Longitude is double longitude && longitude != 0)
                {
                    var tipos = item.Identificadores.Where(x => x.Opcao == 0 && x.Identificador != null && identificadoresPermitidos.Contains(x.Identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower())).ToList();

                    if (tipos.Any())
                    {
                        var identificador = tipos.FirstOrDefault()?.Identificador ?? string.Empty;
                        var imagem = "pointer_" + identificador.Replace("`", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + "_small.png";
                        var pin = new StylishPin
                        {
                            Label = item.Nome ?? string.Empty,
                            Type = PinType.Generic,

                            ImageSource = imagem,
                            Address = item.EnderecoCompleto,
                            Location = new Location(latitude, longitude),
                            Data = item
                        };

                        pin.InfoWindowClicked += PinOnInfoWindowClicked;

                        map.Pins.Add(pin);
                        positionsCampings.Add(new Location(latitude, longitude));
                    }
                }
            }

            //var valorChaveEstadoSelecionado = DBContract.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);

            //if (valorChaveEstadoSelecionado != null && valorChaveEstadoSelecionado != null)
            //{
            map.MoveMapToRegion(positionsCampings);
            //}
            //else
            //{
            //    if (App.LOCALIZACAO_USUARIO == null)
            //    {
            //        App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();
            //        if (App.LOCALIZACAO_USUARIO == null)
            //        {
            //            App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
            //        }
            //    }
            //    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(App.LOCALIZACAO_USUARIO.Latitude, App.LOCALIZACAO_USUARIO.Longitude), Distance.FromKilometers(10)));
            //}

            map.Loaded += async (sender, args) =>
            {
                await Task.Delay(500);

                toggleButton.IsVisible = true;
            };

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cvMapa.Content = map;
            });
        }

        private async void PinOnInfoWindowClicked(object? sender, PinClickedEventArgs e)
        {
            if (sender is StylishPin stylishPin && stylishPin.Data is Item item)
            {
                await Navigation.PushAsync(new DetalhesCampingPage(item));
            }
        }
    }
}