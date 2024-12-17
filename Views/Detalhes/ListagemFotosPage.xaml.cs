using FFImageLoading.Maui;
using MaCamp.Models.Services;

namespace MaCamp.Views.Detalhes
{
    public partial class ListagemFotosPage : ContentPage
    {
        public ListagemFotosPage(IEnumerable<string> urlsFotos)
        {
            InitializeComponent();

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de fotos ");

            grContent.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });

            grContent.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });

            var enumerable = urlsFotos.ToList();

            if (enumerable.Count == 1)
            {
                grContent.RowDefinitions.Add(new RowDefinition
                {
                    Height = App.SCREEN_HEIGHT / 3
                });
            }
            else
            {
                for (var i = 0; i < enumerable.Count / 2; i++)
                {
                    grContent.RowDefinitions.Add(new RowDefinition
                    {
                        Height = App.SCREEN_HEIGHT / 3
                    });
                }
            }

            var linha = 0;
            var coluna = 0;

            foreach (var url in enumerable)
            {
                var urlTemp = CampingServices.MontarUrlImagemTemporaria(url);
                var imagem = new CachedImage
                {
                    Source = urlTemp, HeightRequest = 240, DownsampleHeight = 300, Aspect = Aspect.AspectFill,
                    LoadingPlaceholder = "placeholder.jpg"
                };
                var abrirFoto = new TapGestureRecognizer();

                abrirFoto.Tapped += async (s, e) =>
                {
                    await Navigation.PushModalAsync(new VisualizacaoFotoPage(url));
                };

                imagem.GestureRecognizers.Add(abrirFoto);

                Grid.SetColumn(imagem, coluna);
                Grid.SetRow(imagem, linha);

                //flContent.Children.Add(imagem);
                grContent.Children.Add(imagem);

                coluna++;

                if (coluna > 1)
                {
                    linha++;
                    coluna = 0;
                }
            }
        }
    }
}