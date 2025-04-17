using System.Collections.ObjectModel;
using FluentIcons.Common;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.Detalhes;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp.Views.Campings
{
    public partial class MapaPage : SmartContentPage
    {
        private MapaPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BackgroundColor = Colors.White;
            cvMapa.Content = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                HeightRequest = 35,
                Color = AppColors.CorPrimaria,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            toggleButton.ImageSource = new SymbolImageSource
            {
                Symbol = Symbol.Map,
                IconVariant = IconVariant.Regular,
                Color = AppColors.CorPrimaria
            };

            FirstAppeared += MapaPage_FirstAppeared;
        }

        public MapaPage(bool usarFiltros) : this()
        {
            Title = "Mapa";
            BindingContext = new MapaViewModel(PinOnInfoWindowClicked)
            {
                UsarFiltros = usarFiltros
            };
        }

        public MapaPage(ObservableCollection<Item> itens) : this()
        {
            Title = "Ver no Mapa";
            BindingContext = new MapaViewModel(PinOnInfoWindowClicked)
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
        }

        private async void MapaPage_FirstAppeared(object? sender, EventArgs e)
        {
            if (BindingContext is MapaViewModel viewModel)
            {
                await viewModel.CarregarAsync();

                cvMapa.Content = viewModel.Mapa;

                await Task.Delay(500);

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
                toggleButton.BackgroundColor = isStreet ? Color.FromArgb("#FFFFFFFF") : Color.FromArgb("#FFFFFFD9");
                toggleButton.ImageSource = new SymbolImageSource
                {
                    Symbol = Symbol.Map,
                    Color = AppColors.CorPrimaria,
                    IconVariant = isStreet ? IconVariant.Filled : IconVariant.Regular
                };
            }
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