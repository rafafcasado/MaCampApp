namespace MaCamp.Views.Detalhes
{
    public class VisualizacaoFotoPage : ContentPage
    {
        public VisualizacaoFotoPage(string imageSource)
        {
            var grid = new Grid();

            var contentView = new ContentView()
            {
                Padding = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            var label = new Label
            {
                Text = "Voltar",
                TextColor = Colors.White,
                FontSize = 15,
                InputTransparent = false
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