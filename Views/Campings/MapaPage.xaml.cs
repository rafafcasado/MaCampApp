using FluentIcons.Common;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.Detalhes;
using MPowerKit.GoogleMaps;

namespace MaCamp.Views.Campings
{
    public partial class MapaPage : SmartContentPage
    {
        private Action<Pin> Map_InfoWindowClick { get; }

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
            Map_InfoWindowClick = async pin =>
            {
                if (BindingContext is MapaViewModel viewModel)
                {
                    var item = viewModel.Itens.Find(x => Equals(x.Latitude, pin.Position.X) && Equals(x.Longitude, pin.Position.Y));

                    if (item != null)
                    {
                        await Navigation.PushAsync(new DetalhesCampingPage(item));
                    }
                }
            };

            FirstAppeared += MapaPage_FirstAppeared;
        }

        public MapaPage(bool usarFiltros) : this()
        {
            Title = "Mapa";
            BindingContext = new MapaViewModel(Map_InfoWindowClick)
            {
                UsarFiltros = usarFiltros
            };
        }

        public MapaPage(List<Item> itens) : this()
        {
            Title = "Ver no Mapa";
            BindingContext = new MapaViewModel(Map_InfoWindowClick)
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

                cvMapa.Content = viewModel.Content;
                toggleButton.IsVisible = viewModel.Itens.Any();
            }
        }

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            if (cvMapa.Content is GoogleMap map)
            {
                var isNormal = map.MapType == MapType.Normal;

                map.MapType = isNormal ? MapType.Satellite : MapType.Normal;

                toggleButton.Text = isNormal ? "Satélite" : "Terreno";
                toggleButton.ImageSource = new FluentImageSource
                {
                    Color = AppColors.CorPrimaria,
                    Icon = Icon.Map,
                    IconVariant = isNormal ? IconVariant.Filled : IconVariant.Regular
                };
            }
        }
    }
}