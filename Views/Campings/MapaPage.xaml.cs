using FluentIcons.Common;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.Detalhes;
using Maui.GoogleMaps;
using Map = Maui.GoogleMaps.Map;

namespace MaCamp.Views.Campings
{
    public partial class MapaPage : SmartContentPage
    {
        public MapaPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BackgroundColor = Colors.White;
            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 70,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            toggleButton.ImageSource = new FluentImageSource
            {
                Color = AppColors.CorPrimaria,
                Icon = Icon.Map,
                IconVariant = IconVariant.Regular
            };

            FirstAppeared += MapaPage_FirstAppeared;
        }

        public MapaPage(bool usarFiltros) : this()
        {
            Title = "Mapa";
            BindingContext = new MapaViewModel(Map_InfoWindowClicked)
            {
                UsarFiltros = usarFiltros
            };
        }

        public MapaPage(List<Item> itens) : this()
        {
            Title = "Ver no Mapa";
            BindingContext = new MapaViewModel(Map_InfoWindowClicked)
            {
                Itens = itens
            };
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

            if (BindingContext is MapaViewModel viewModel)
            {
                viewModel.Cancel();
            }
        }

        private async void MapaPage_FirstAppeared(object? sender, EventArgs e)
        {
            if (BindingContext is MapaViewModel viewModel)
            {
                await Workaround.TaskWorkAsync(async () => await viewModel.CarregarAsync());

                cvMapa.Content = viewModel.Mapa;
                toggleButton.IsVisible = true;
            }
        }

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            if (cvMapa.Content is Map map)
            {
                var isStreet = map.MapType == MapType.Street;

                map.MapType = isStreet ? MapType.Satellite : MapType.Street;

                toggleButton.Text = isStreet ? "Satélite" : "Terreno";
                toggleButton.ImageSource = new FluentImageSource
                {
                    Color = AppColors.CorPrimaria,
                    Icon = Icon.Map,
                    IconVariant = isStreet ? IconVariant.Filled : IconVariant.Regular
                };
            }
        }

        private async void Map_InfoWindowClicked(object? sender, InfoWindowClickedEventArgs e)
        {
            if (e.Pin.Tag is Item item)
            {
                await Navigation.PushAsync(new DetalhesCampingPage(item));
            }
        }
    }
}