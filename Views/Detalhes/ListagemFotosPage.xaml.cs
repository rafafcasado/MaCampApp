using FFImageLoading.Maui;
using MaCamp.Utils;
using MaCamp.Models.Services;
using Microsoft.Maui.Controls.Shapes;

namespace MaCamp.Views.Detalhes
{
    public partial class ListagemFotosPage : ContentPage
    {
        public ListagemFotosPage(List<string> urlsFotos)
        {
            InitializeComponent();

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de fotos ");

            Padding = new Thickness(1);
            BackgroundColor = Colors.White;

            var largura = App.SCREEN_WIDTH / layout.Children.Count - 10;

            for (var i = 0; i < urlsFotos.Count; i++)
            {
                var url = urlsFotos[i];
                var layoutIndex = i % layout.Children.Count;
                var random = new Random();
                var altura = largura + random.Next(50, 150);
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
                var imagem = new CachedImage
                {
                    Source = source,
                    DownsampleHeight = altura,
                    Aspect = Aspect.AspectFill,
                    LoadingPlaceholder = "placeholder.jpg"
                };
                var abrirFoto = new TapGestureRecognizer();

                abrirFoto.Tapped += async (sender, args) =>
                {
                    await Navigation.PushAsync(new VisualizacaoFotoPage(url, Title));
                };

                frame.GestureRecognizers.Add(abrirFoto);

                frame.Content = imagem;

                if (layout.Children[layoutIndex] is StackLayout stackLayout)
                {
                    stackLayout.Children.Add(frame);
                }
            }
        }
    }
}