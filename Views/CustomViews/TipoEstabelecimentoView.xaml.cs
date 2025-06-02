using FFImageLoading.Maui;
using MaCamp.Models;
using MaCamp.Utils;
using Microsoft.Maui.Controls.Shapes;

namespace MaCamp.Views.CustomViews
{
    public partial class TipoEstabelecimentoView : ContentView
    {
        public TipoEstabelecimentoView(List<ItemIdentificador> identificadores)
        {
            InitializeComponent();

            var tipos = identificadores.Where(x => x.Opcao == 0).ToList();
            var tamanhoIcone = DeviceInfo.Platform == DevicePlatform.iOS ? 15 : 20;
            var tamanhoFonte = DeviceInfo.Platform == DevicePlatform.iOS ? 10 : 13;

            // Grid
            if (flTipos.IsVisible)
            {
                var columnsDefinitions = Enumerable.Range(0, (tipos.Count + 1) / 2).Select(x => new ColumnDefinition(GridLength.Star)).ToArray();
                var rowDefinitions = Enumerable.Range(0, (tipos.Count + 1) / 2).Select(x => new RowDefinition(100)).ToArray();
                var coluna = 0;
                var linha = 0;

                flTipos.ColumnDefinitions = new ColumnDefinitionCollection(columnsDefinitions);
                flTipos.RowDefinitions = new RowDefinitionCollection(rowDefinitions);

                foreach (var tipo in tipos)
                {
                    var frTipo = new Border
                    {
                        BackgroundColor = AppColors.CorDestaque,
                        Padding = new Thickness(5),
                        Margin = 5,
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10
                        }
                    };
                    var slTipo = new StackLayout
                    {
                        Padding = 5,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children =
                        {
                            new CachedImage
                            {
                                Source = tipo.Identificador?.Replace("`", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + ".png",
                                HeightRequest = tamanhoIcone,
                                WidthRequest = tamanhoIcone,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.Center
                            },
                            new Label
                            {
                                Text = tipo.NomeExibicao ?? string.Empty,
                                FontSize = tamanhoFonte,
                                TextColor = Colors.White,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    };

                    frTipo.Content = slTipo;

                    Grid.SetColumn(frTipo, coluna);
                    Grid.SetRow(frTipo, linha);

                    flTipos.Children.Add(frTipo);

                    coluna++;

                    if (coluna == 2)
                    {
                        linha++;
                        coluna = 0;
                    }
                }
            }

            // StackLayout
            if (slTipos.IsVisible)
            {
                foreach (var tipo in tipos)
                {
                    var frTipo = new Border
                    {
                        BackgroundColor = AppColors.CorDestaque,
                        Padding = new Thickness(20, 5, 20, 5),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10
                        }
                    };
                    var slTipo = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children =
                        {
                            new CachedImage
                            {
                                Source = tipo.Identificador?.Replace("`", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + ".png",
                                HeightRequest = tamanhoIcone,
                                WidthRequest = tamanhoIcone,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.Center
                            },
                            new Label
                            {
                                Text = tipo.NomeExibicao ?? string.Empty,
                                FontSize = tamanhoFonte,
                                TextColor = Colors.White,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    };

                    frTipo.Content = slTipo;

                    slTipos.Children.Add(frTipo);
                }
            }
        }
    }
}