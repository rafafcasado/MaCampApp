using FFImageLoading.Maui;
using MaCamp.AppSettings;
using MaCamp.Models;
using Microsoft.Maui.Controls.Shapes;

namespace MaCamp.Views
{
    public partial class TipoEstabelecimentoView : ContentView
    {
        public TipoEstabelecimentoView(List<ItemIdentificador> identificadores)
        {
            InitializeComponent();

            var tipos = identificadores.Where(x => x.Opcao == 0).ToList();
            var tamanhoIcone = DeviceInfo.Platform == DevicePlatform.iOS ? 15 : 20;
            var tamanhoFonte = DeviceInfo.Platform == DevicePlatform.iOS ? 10 : 13;
            var coluna = 0;
            var linha = 0;

            foreach (var tipo in tipos)
            {
                var frTipo = new Border
                {
                    BackgroundColor = AppColors.CorDestaque,
                    Padding = new Thickness(5),
                    Margin = 5,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = 25
                    }
                };
                var slTipo = new StackLayout
                {
                    Padding = 5,
                    Spacing = 1,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                var icone = new CachedImage
                {
                    Source = tipo.Identificador?.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png",
                    WidthRequest = tamanhoIcone, HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center
                };
                var lbNome = new Label
                {
                    Text = tipo.NomeExibicao ?? string.Empty,
                    FontSize = tamanhoFonte,
                    TextColor = Colors.White,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                slTipo.Children.Add(icone);
                slTipo.Children.Add(lbNome);

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
    }
}