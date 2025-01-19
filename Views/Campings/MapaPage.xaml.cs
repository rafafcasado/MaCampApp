using System.Collections.ObjectModel;
using FluentIcons.Common;
using FluentIcons.Maui;
using MaCamp.AppSettings;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.ViewModels;
using MaCamp.Views.Detalhes;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp.Views.Campings
{
    public partial class MapaPage : ContentPage
    {
        public MapaPage(bool usarFiltros = true)
        {
            InitializeComponent();

            Title = "Mapa";
            NavigationPage.SetBackButtonTitle(this, "");
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

            Task.Run(async () =>
            {
                var vm = new ListagemInfinitaVM();
                await vm.Carregar("", -1, "", "", TipoListagem.Camping, usarFiltros);

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
            NavigationPage.SetBackButtonTitle(this, "");

            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 35,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Task.Run(async () =>
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
                Console.WriteLine(ex.Message);
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
                if (item.Latitude is double latitude && item.Longitude is double longitude)
                {
                    var tipos = item.Identificadores.Where(i => i.Opcao == 0 && i.Identificador != null && identificadoresPermitidos.Contains(i.Identificador.Replace("`", "").Replace("çã", "ca").Replace("/", "").ToLower())).ToList();

                    if (tipos.Any())
                    {
                        var identificador = tipos.FirstOrDefault()?.Identificador ?? string.Empty;
                        var imagem = "pointer_" + identificador.Replace("`", "").Replace("çã", "ca").Replace("/", "").ToLower() + "_small.png";
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

            //var valorChaveEstadoSelecionado = DBContract.Instance.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);

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

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cvMapa.Content = map;
                toggleButton.IsVisible = true;
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