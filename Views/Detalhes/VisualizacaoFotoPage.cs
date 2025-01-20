using MaCamp.Utils;
using MaCamp.Models;
using MaCamp.Views.CustomViews;

namespace MaCamp.Views.Detalhes
{
    public class VisualizacaoFotoPage : ContentPage
    {
        public VisualizacaoFotoPage(string imageSource, string titulo)
        {
            NewVersion(imageSource, false, titulo);
        }

        public async void NewVersion(string url, bool nativo, string? titulo = null)
        {
            try
            {
                if (nativo)
                {
                    var isUri = Uri.IsWellFormedUriString(url, UriKind.Absolute);

                    if (isUri)
                    {
                        var imagePath = await AppMedia.SalvarImagemTemporariaAsync(url);

                        if (imagePath != null)
                        {
                            await Launcher.OpenAsync(imagePath);
                        }
                        else
                        {
                            var resposta = await AppConstants.CurrentPage.DisplayAlert(string.Empty, "Não foi possível abrir a imagem usando aplicativo do sistema.\nDeseja tentar abrir usando o navegador?", "Sim", "Não");

                            if (resposta)
                            {
                                await Launcher.OpenAsync(url);
                            }
                        }
                    }
                    else
                    {
                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(url)
                        });
                    }
                }
                else
                {
                    var zoomableView = new ZoomableView();
                    //var imageSource = ImageSource.FromFile(url);
                    var image = new Image
                    {
                        Source = url,
                        Aspect = Aspect.AspectFit
                    };

                    zoomableView.Content = image;

                    Title = titulo ?? Path.GetFileNameWithoutExtension(url);
                    Content = zoomableView;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OldVersion(string imageSource)
        {
            var grid = new Grid();
            var contentView = new ContentView
            {
                Padding = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };
            var label = new Label
            {
                Text = "Voltar",
                TextColor = Colors.White,
                FontSize = 15
            };
            var gestureRecognizer = new TapGestureRecognizer();

            grid.Children.Add(new WebView
            {
                BackgroundColor = Colors.Black,
                Source = new HtmlWebViewSource
                {
                    Html = $"<html><body style='background-color:black'><center><img src='{imageSource}' style='width:100%;position: absolute;margin:auto;top: 0;left: 0;right: 0;bottom: 0;' align='center' /></div></center></body></html>"
                }
            });

            gestureRecognizer.Tapped += async delegate
            {
                await Navigation.PopModalAsync();
            };

            contentView.GestureRecognizers.Add(gestureRecognizer);

            contentView.Content = label;

            grid.Children.Add(contentView);

            Content = grid;
        }
    }
}