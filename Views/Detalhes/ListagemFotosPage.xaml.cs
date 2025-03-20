using FFImageLoading.Maui;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Utils.Converters;
using Microsoft.Maui.Controls.Shapes;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Detalhes
{
    public partial class ListagemFotosPage : ContentPage
    {
        public ListagemFotosPage(TipoListagemFotos tipoListagemFotos, List<string> urlsFotos)
        {
            InitializeComponent();

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de fotos ");

            Padding = new Thickness(1);
            BackgroundColor = Colors.White;

            switch (tipoListagemFotos)
            {
                case TipoListagemFotos.Masonary:
                    var scrollView = new ScrollView();
                    var layoutMasonary = new StackLayout
                    {
                        Margin = new Thickness(5),
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 5
                    };
                    var evenLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 5
                    };
                    var oddLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 5
                    };
                    var largura = App.SCREEN_WIDTH / 2 - 10;

                    for (var i = 0; i < urlsFotos.Count; i++)
                    {
                        var url = urlsFotos[i];
                        var isEvenIndex = i % 2 == 0;
                        var maisAltura = i % 4 == 0 || i % 4 == 3;
                        var altura = largura * (maisAltura ? 1.5 : 1);
                        var frame = new Border
                        {
                            HeightRequest = altura,
                            WidthRequest = largura,
                            BackgroundColor = AppColors.CorPrimaria,
                            StrokeShape = new RoundRectangle
                            {
                                CornerRadius = 5
                            }
                        };
                        var source = CampingServices.MontarUrlImagemTemporaria(url);
                        var imagemMasonary = new CachedImage
                        {
                            Source = source,
                            DownsampleHeight = altura,
                            Aspect = Aspect.AspectFill,
                            LoadingPlaceholder = "placeholder.jpg"
                        };
                        var abrirFotoMasonary = new TapGestureRecognizer();

                        abrirFotoMasonary.Tapped += async (sender, args) =>
                        {
                            await Navigation.PushAsync(new VisualizacaoFotoPage(url, Title ?? string.Empty));
                        };

                        frame.GestureRecognizers.Add(abrirFotoMasonary);

                        frame.Content = imagemMasonary;

                        if (isEvenIndex)
                        {
                            evenLayout.Children.Add(frame);
                        }
                        else
                        {
                            oddLayout.Children.Add(frame);
                        }
                    }

                    layoutMasonary.Children.Add(evenLayout);
                    layoutMasonary.Children.Add(oddLayout);

                    scrollView.Content = layoutMasonary;
                    Content = scrollView;

                    break;
                case TipoListagemFotos.Carousel:
                    var layoutCarousel = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 15
                    };
                    var indicatorView = new IndicatorView
                    {
                        IndicatorSize = 10,
                        IndicatorColor = Colors.White,
                        SelectedIndicatorColor = Colors.DarkSlateGray,
                        HorizontalOptions = LayoutOptions.Center,
                    };
                    var carouselView = new CarouselView
                    {
                        Loop = false,
                        IndicatorView = indicatorView,
                        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
                        {
                            ItemSpacing = 5
                        },
                        PeekAreaInsets = new Thickness(20),
                        ItemTemplate = new DataTemplate(() =>
                        {
                            var frame = new Border
                            {
                                HeightRequest = App.SCREEN_HEIGHT * 0.8,
                                WidthRequest = App.SCREEN_WIDTH * 0.9,
                                StrokeShape = new RoundRectangle
                                {
                                    CornerRadius = 5,
                                    StrokeThickness = 1,
                                    Stroke = new SolidColorBrush
                                    {
                                        Color = Colors.White
                                    }
                                }
                            };
                            var image = new CachedImage
                            {
                                Aspect = Aspect.AspectFill,
                                LoadingPlaceholder = "placeholder.jpg"
                            };
                            var abrirFotoCarousel = new TapGestureRecognizer();

                            image.SetBinding(CachedImage.SourceProperty, new Binding(".", converter: new ImageUrlConverter()));

                            abrirFotoCarousel.Tapped += async (sender, args) =>
                            {
                                if (sender is Border border && border.BindingContext is string imageUrl)
                                {
                                    await Navigation.PushAsync(new VisualizacaoFotoPage(imageUrl, Title ?? string.Empty));
                                }
                            };

                            frame.GestureRecognizers.Add(abrirFotoCarousel);

                            frame.Content = image;

                            return frame;
                        })
                    };

                    carouselView.ItemsSource = urlsFotos;
                    carouselView.Position = 0;

                    Background = AppColors.CorPrimaria;

                    layoutCarousel.Children.Add(carouselView);
                    layoutCarousel.Children.Add(indicatorView);

                    Content = layoutCarousel;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}