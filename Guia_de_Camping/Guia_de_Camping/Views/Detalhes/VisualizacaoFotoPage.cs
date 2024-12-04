using Xamarin.Forms;

namespace Aspbrasil.Views.Detalhes
{
    public class VisualizacaoFotoPage : ContentPage
    {
        public VisualizacaoFotoPage(string imageSource)
        {
            var gr = new Grid();
            gr.Children.Add(new WebView
            {                
                BackgroundColor = Color.Black,
                Source = new HtmlWebViewSource
                {
                    Html = $"<html><body style='background-color:black'><center><img src='{imageSource}' style='width:100%;position: absolute;margin:auto;top: 0;left: 0;right: 0;bottom: 0;' align='center' /></div></center></body></html>"
                }
            });

            var cvVoltar = new ContentView() { Padding = 20, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start };
            var voltar = new TapGestureRecognizer();
            voltar.Tapped += async (s, e) => { await Navigation.PopModalAsync(); };
            cvVoltar.GestureRecognizers.Add(voltar);

            var x = new Label { Text = "Voltar", TextColor = Color.White, FontSize = 15, InputTransparent = false };

            cvVoltar.Content = x;

            gr.Children.Add(cvVoltar);
            Content = gr;
        }
    }
}